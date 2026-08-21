using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AspNetCaller
{
    class Program
    {
        static void Main(string[] args)
        {

            string logfileName = "smslog.txt";

            if (args == null)
            {
                File.WriteAllText(logfileName, "args is null");                
            }
            else if(args.Length == 4)
            {
                MessageClient messageClient = new MessageClient(args[0]);

                messageClient.SendSMS(args[1], args[2], args[3]);

                File.WriteAllText(logfileName, string.Format("{0} {1} {2} {3}", args[0], args[1], args[2], args[3]));
            } 
            else
            {
                string text = "";
                
                foreach(string arg in args)
                {
                    text += arg + "/";
                }
                File.WriteAllText(logfileName, "args is invalid:" + text);        
            }
        }
    }
}
