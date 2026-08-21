using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;

namespace Pipelib
{
    internal class PipeAsyncClientSender
    {
        public void Dispose()
        {
        }


        private object _interLock = new Object();
        internal void Send(string SendStr, string PipeName, int TimeOut = 1000)
        {
            if (SendStr == null || SendStr == "")
                return;


            //using(StreamWriter tw = new StreamWriter(File.Open("c:\\temp\\aaaaaa.txt", FileMode.Append)))
            //{
                 //lock (_interLock)
            {
                try
                {
                    //tw.WriteLine("Begin send");
                    //tw.Flush();
                    using(NamedPipeClientStream pipeStream = new NamedPipeClientStream(".", PipeName, PipeDirection.Out))
                    {
                        //tw.WriteLine("Create pipe");
                        //tw.Flush();
                        pipeStream.Connect();

                        //tw.WriteLine("connect pipe");
                        //tw.Flush();
                        if (pipeStream.IsConnected == true)
                        {
                            
                            using (StreamWriter writer = new StreamWriter(pipeStream))
                            {
                                //tw.WriteLine("create write stream");
                                //tw.Flush();
                                writer.WriteLine(SendStr);

                                //tw.WriteLine("write data");
                                //tw.Flush();
                                writer.Flush();
                                //tw.WriteLine("flush stream");
                                //tw.Flush();
                                writer.Close();
                                //tw.WriteLine("close stream");
                                //tw.Flush();
                            }
                            //byte[] _buffer = Encoding.UTF8.GetBytes(SendStr);
                           // pipeStream.Write(_buffer, 0, _buffer.Length);//, AsyncSend, pipeStream);
                            //pipeStream.Flush();
                            pipeStream.Close();
                           // tw.WriteLine("close pipe");
                           // tw.Flush();
                        }   
                    }                                  
                }
                catch (Exception ex)
                {

                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    System.Diagnostics.Trace.WriteLine(ex.Source);
                    System.Diagnostics.Trace.WriteLine(ex.StackTrace);
                }
//tw.Close();
            //}         
            }

              
        }

        private void AsyncSend(IAsyncResult iar)
        {
            try
            {
                // Get the pipe
                NamedPipeClientStream pipeStream = (NamedPipeClientStream)iar.AsyncState;

                // End the write
                pipeStream.EndWrite(iar);
                pipeStream.Flush();
                pipeStream.Close();
                pipeStream.Dispose();
            }
            catch (Exception)
            {
            }
        }
    }
}
