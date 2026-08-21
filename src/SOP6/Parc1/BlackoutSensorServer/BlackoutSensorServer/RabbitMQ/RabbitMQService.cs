using BlackoutSensorServer.Network;
using IBS.OWS.Defines;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlackoutSensorServer.RabbitMQ
{
    public class RabbitMQService
    {
        private static class ExchangeType
        {
            //
            // 요약:
            // 1:1 관계로 Unicast 방식에 적합, 운드 로빈 방식으로 여러 workers(Consumer)간 task를 분리
            // Exchange에 바인딩 된 Queue 중에서 메시지의 라우팅 키와 매핑되어 있는 Queue로 메시지를 전달한다.
            public const string Direct = "direct";
            //
            // 요약:
            // 메시지의 라우팅 키를 무시하고 Exchange에 바인딩 된 모든 Queue에 메시지를 전달한다. 
            // 1:N 관계로 메시지를 브로드캐스트하는 용도로 사용된다.
            public const string Fanout = "fanout";
            //
            // 요약:
            // Exchange에 바인딩 된 Queue 중에서 메시지의 라우팅 키가 패턴에 맞는 Queue에게 모두 메시지를 전달, Multicast 방식에 적합하다.
            public const string Headers = "headers";
            //
            // 요약:
            // 라우팅 키 대신 메시지 헤더에 여러 속성들을 더해 속성들이 매칭되는 큐에 메시지를 전달한다.
            public const string Topic = "topic";
        }

        private bool m_runThread = false;
        private string m_strServerIP = "192.168.5.4";
        private string m_strMessageName = "bacnetalarmqueue";

        private IConnection m_connection = null;
        private IModel m_channel = null;

        private NetworkWebManager m_netMgr = null;
        private bool m_bConnect = false;
        public bool bConnect
        {
            get { return m_bConnect; }
        }

        public RabbitMQService(string serverIP, NetworkWebManager netMgr)
        {
            m_strServerIP = serverIP;
            m_netMgr = netMgr;

            Thread t = new Thread(new ThreadStart(ReceiveThread));
            t.Start();
        }

        private void ReceiveThread()
        {
            m_runThread = true;

            var factory = new ConnectionFactory() { HostName = m_strServerIP };

            try
            {
                using (var connection = factory.CreateConnection())
                {
                    // 버전 정보
                    //object data;
                    //if (connection.ServerProperties.TryGetValue("version", out data))
                    //{
                    //    byte[] arr = (byte[])data;
                    //    string str = Encoding.UTF8.GetString(arr);
                    //    System.Diagnostics.Trace.WriteLine("RabbitMQ Version : " + str);
                    //}
                    using (var channel = connection.CreateModel())
                    {
                        m_bConnect = true;
#if !SERVICE
                        FormMain.Instance.CurrentState(m_strServerIP + " Connect " + m_bConnect);
#endif

                        channel.ExchangeDeclare(m_strMessageName, ExchangeType.Fanout);
                        /*channel.QueueDeclare(queue: m_strMessageName,
                                             durable: false,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);*/

                        //channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                        
                        var queueName = channel.QueueDeclare().QueueName;
                        channel.QueueBind(queueName, m_strMessageName, "");

                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Model = channel;

                        consumer.Received += (model, ea) =>
                        {
                            IBSAlarm alarm = ProcDeserialize(ea.Body);
                            ProcAlarm(alarm);
                        //var body = ea.Body.ToArray();
                        //var message = Encoding.UTF8.GetString(body);

                        //System.Diagnostics.Trace.WriteLine(" [x] Received {0}", message);
                    };

                        channel.BasicConsume(queue: queueName,
                                             autoAck: true,
                                             consumer: consumer);

                        while (m_runThread)
                        {
                            Thread.Sleep(1000);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#if !SERVICE
                FormMain.Instance.CurrentState(m_strServerIP + " Connect " + m_bConnect);
#endif
                Trace.WriteLine(ex.Message);
            }
        }
        private IBSAlarm ProcDeserialize(byte[] arr)
        {
            IBSAlarm alarm = null;
            IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(arr))
            {
                alarm = (IBSAlarm)formatter.Deserialize(stream);
            }

            return alarm;
        }

        private void ProcAlarm(IBSAlarm alarm)
        {
            if (alarm == null)
                return;

            if (alarm.ToState.ToString() == "OFFNORMAL" || alarm.ToState.ToString() == "NORMAL")
            {
                m_netMgr.OnBlackoutSignal(alarm.ToState.ToString(), alarm.Object_Name);
            }
        }

        public void Close()
        {
            m_runThread = false;
        }
    }
}
