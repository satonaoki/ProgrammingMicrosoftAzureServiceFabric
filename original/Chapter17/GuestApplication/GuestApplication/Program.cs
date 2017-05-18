using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GuestApplication
{
    class Program
    {
        static int Main(string[] args)
        {
            const string commandKey = "COMMAND";
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://+:8088/");
            listener.Start();
            while (true)
            {
                var context = listener.GetContext();
                var request = context.Request;
                if (request.QueryString.AllKeys.Contains(commandKey))
                {
                    switch (request.QueryString[commandKey])
                    {
                        case "kill":
                            return 1;
                        case "stop":
                            return 0;
                    }
                }

                var response = context.Response;
                string responseString = "Server time now is: " + DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                using (Stream output = response.OutputStream)
                {
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }
            }
        }
    }
}
