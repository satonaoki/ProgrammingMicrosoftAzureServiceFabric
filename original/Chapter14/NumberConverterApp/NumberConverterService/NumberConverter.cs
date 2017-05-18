using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace NumberConverterService
{
public class NumberConverter : IEventProcessor
{
    Stopwatch checkpointStopWatch;
    EventHubClient eventHubClient = EventHubClient.CreateFromConnectionString("[Event Hub Connection string 1]", "hub-2");
    public async Task CloseAsync(PartitionContext context, CloseReason reason)
    {
        if (reason == CloseReason.Shutdown)
        {
            await context.CheckpointAsync();
        }
    }

    public Task OpenAsync(PartitionContext context)
    {
        this.checkpointStopWatch = new Stopwatch();
        this.checkpointStopWatch.Start();
        return Task.FromResult(1);
    }

    public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
    {
        foreach (EventData eventData in messages)
        {
            int data = int.Parse(Encoding.UTF8.GetString(eventData.GetBytes()));
            eventHubClient.Send(new EventData(Encoding.UTF8.GetBytes(ConvertNumberToWords(data))));
        }
        if (this.checkpointStopWatch.Elapsed > TimeSpan.FromMinutes(5))
        {
            await context.CheckpointAsync();
            this.checkpointStopWatch.Restart();
        }
    }
    private string ConvertNumberToWords(int number)
    {
        if (number == 0)
            return "zero";
        string ret = "";
        if (number / 1000000 > 0)
        {
            ret += ConvertNumberToWords(number / 1000000) + " million ";
            number %= 1000000;
        }

        if ((number / 1000) > 0)
        {
            ret += ConvertNumberToWords(number / 1000) + " thousand ";
            number %= 1000;
        }

        if ((number / 100) > 0)
        {
            ret += ConvertNumberToWords(number / 100) + " hundred ";
            number %= 100;
        }

        if (number > 0)
        {
            if (ret != "")
                ret += "and ";

            var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

            if (number < 20)
                ret += unitsMap[number];
            else
            {
                ret += tensMap[number / 10];
                if ((number % 10) > 0)
                    ret += "-" + unitsMap[number % 10];
            }
        }

        return ret;

    }
}
}
