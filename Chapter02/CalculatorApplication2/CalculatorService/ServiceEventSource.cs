using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;

namespace CalculatorService
{
    [EventSource(Name = "MyCompany-CalculatorApplication2-CalculatorService")]
    internal sealed class ServiceEventSource : EventSource
    {
        public static readonly ServiceEventSource Current = new ServiceEventSource();

        static ServiceEventSource()
        {
            // タスク インフラストラクチャが初期化されるまで ETW アクティビティが追跡されないという問題の回避策です。
            // この問題は .NET Framework 4.6.2 で修正される予定です。
            Task.Run(() => { });
        }

        // インスタンス コンストラクターは、シングルトン セマンティックを適用するためにプライベートになっています
        private ServiceEventSource() : base() { }

        #region キーワード
        // イベント キーワードは、イベントを分類するために使用できます。
        // 各キーワードは、ビット フラグです。1 つのイベントを (EventAttribute.Keywords プロパティを介して) 複数のキーワードに関連付けることができます。
        // キーワードは、キーワードを使用する EventSource 内で 'Keywords' という名前のパブリック クラスとして定義されていなければなりません。
        public static class Keywords
        {
            public const EventKeywords Requests = (EventKeywords)0x1L;
            public const EventKeywords ServiceInitialization = (EventKeywords)0x2L;
        }
        #endregion

        #region イベント
        // 記録して [Event] 属性を適用したいイベントごとにインスタンス メソッドを定義します。
        // メソッド名は、イベントの名前です。
        // イベントで記録したい任意のパラメーターを渡します (プリミティブの整数型、DateTime、Guid、文字列のみが許可されています)。
        // 各イベント メソッドの実装では、イベント ソースが有効かどうかをチェックする必要があります。有効な場合、WriteEvent() メソッドを呼び出してイベントを発生させます。
        // 各イベント メソッドに渡される引数の数と型は、WriteEvent() に渡される数と型とまったく一致していなければなりません。
        // [NonEvent] 属性を、イベントを定義しないすべてのメソッドに設定します。
        // 詳しくは、https://msdn.microsoft.com/ja-jp/library/system.diagnostics.tracing.eventsource.aspx をご覧ください

        [NonEvent]
        public void Message(string message, params object[] args)
        {
            if (this.IsEnabled())
            {
                string finalMessage = string.Format(message, args);
                Message(finalMessage);
            }
        }

        private const int MessageEventId = 1;
        [Event(MessageEventId, Level = EventLevel.Informational, Message = "{0}")]
        public void Message(string message)
        {
            if (this.IsEnabled())
            {
                WriteEvent(MessageEventId, message);
            }
        }

        [NonEvent]
        public void ServiceMessage(StatelessServiceContext serviceContext, string message, params object[] args)
        {
            if (this.IsEnabled())
            {
                string finalMessage = string.Format(message, args);
                ServiceMessage(
                    serviceContext.ServiceName.ToString(),
                    serviceContext.ServiceTypeName,
                    serviceContext.InstanceId,
                    serviceContext.PartitionId,
                    serviceContext.CodePackageActivationContext.ApplicationName,
                    serviceContext.CodePackageActivationContext.ApplicationTypeName,
                    serviceContext.NodeContext.NodeName,
                    finalMessage);
            }
        }

        // 非常に頻度の高いイベントの場合、WriteEventCore API を使用してイベントを発生させると便利なときがあります。
        // これによってパラメーターの処理が効率的になりますが、EventData 構造とアンセーフ コードを明示的に割り当てることが必要になります。
        // このコード パスを有効にするには、UNSAFE 条件付きコンパイル シンボルを定義し、プロジェクト プロパティでアンセーフ コードのサポートを有効にします。
        private const int ServiceMessageEventId = 2;
        [Event(ServiceMessageEventId, Level = EventLevel.Informational, Message = "{7}")]
        private
#if UNSAFE
        unsafe
#endif
        void ServiceMessage(
            string serviceName,
            string serviceTypeName,
            long replicaOrInstanceId,
            Guid partitionId,
            string applicationName,
            string applicationTypeName,
            string nodeName,
            string message)
        {
#if !UNSAFE
            WriteEvent(ServiceMessageEventId, serviceName, serviceTypeName, replicaOrInstanceId, partitionId, applicationName, applicationTypeName, nodeName, message);
#else
            const int numArgs = 8;
            fixed (char* pServiceName = serviceName, pServiceTypeName = serviceTypeName, pApplicationName = applicationName, pApplicationTypeName = applicationTypeName, pNodeName = nodeName, pMessage = message)
            {
                EventData* eventData = stackalloc EventData[numArgs];
                eventData[0] = new EventData { DataPointer = (IntPtr) pServiceName, Size = SizeInBytes(serviceName) };
                eventData[1] = new EventData { DataPointer = (IntPtr) pServiceTypeName, Size = SizeInBytes(serviceTypeName) };
                eventData[2] = new EventData { DataPointer = (IntPtr) (&replicaOrInstanceId), Size = sizeof(long) };
                eventData[3] = new EventData { DataPointer = (IntPtr) (&partitionId), Size = sizeof(Guid) };
                eventData[4] = new EventData { DataPointer = (IntPtr) pApplicationName, Size = SizeInBytes(applicationName) };
                eventData[5] = new EventData { DataPointer = (IntPtr) pApplicationTypeName, Size = SizeInBytes(applicationTypeName) };
                eventData[6] = new EventData { DataPointer = (IntPtr) pNodeName, Size = SizeInBytes(nodeName) };
                eventData[7] = new EventData { DataPointer = (IntPtr) pMessage, Size = SizeInBytes(message) };

                WriteEventCore(ServiceMessageEventId, numArgs, eventData);
            }
#endif
        }

        private const int ServiceTypeRegisteredEventId = 3;
        [Event(ServiceTypeRegisteredEventId, Level = EventLevel.Informational, Message = "Service host process {0} registered service type {1}", Keywords = Keywords.ServiceInitialization)]
        public void ServiceTypeRegistered(int hostProcessId, string serviceType)
        {
            WriteEvent(ServiceTypeRegisteredEventId, hostProcessId, serviceType);
        }

        private const int ServiceHostInitializationFailedEventId = 4;
        [Event(ServiceHostInitializationFailedEventId, Level = EventLevel.Error, Message = "Service host initialization failed", Keywords = Keywords.ServiceInitialization)]
        public void ServiceHostInitializationFailed(string exception)
        {
            WriteEvent(ServiceHostInitializationFailedEventId, exception);
        }

        // 同じ名前のプレフィックスと "Start"/"Stop" というサフィックスを持つイベントのペアによって、イベント追跡アクティビティの境界が暗黙的にマークされます。
        // これらのアクティビティはデバッグ ツールとプロファイル ツールによって自動的に選択されます。それにより、実行時間、子アクティビティ、
        // その他の統計情報を計算できます。
        private const int ServiceRequestStartEventId = 5;
        [Event(ServiceRequestStartEventId, Level = EventLevel.Informational, Message = "Service request '{0}' started", Keywords = Keywords.Requests)]
        public void ServiceRequestStart(string requestTypeName)
        {
            WriteEvent(ServiceRequestStartEventId, requestTypeName);
        }

        private const int ServiceRequestStopEventId = 6;
        [Event(ServiceRequestStopEventId, Level = EventLevel.Informational, Message = "Service request '{0}' finished", Keywords = Keywords.Requests)]
        public void ServiceRequestStop(string requestTypeName, string exception = "")
        {
            WriteEvent(ServiceRequestStopEventId, requestTypeName, exception);
        }
        #endregion

        #region プライベート メソッド
#if UNSAFE
        private int SizeInBytes(string s)
        {
            if (s == null)
            {
                return 0;
            }
            else
            {
                return (s.Length + 1) * sizeof(char);
            }
        }
#endif
        #endregion
    }
}
