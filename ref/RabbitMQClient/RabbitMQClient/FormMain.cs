using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;
using System.IO;

namespace RabbitMQClient
{
    public partial class FormMain : Form
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

        public FormMain()
        {
            InitializeComponent();

            //InitModel();

            Thread t = new Thread(new ThreadStart(ReceiveThread));
            t.Start();
        }

        private void InitModel()
        {
            var factory = new ConnectionFactory() { HostName = m_strServerIP };

            m_connection = factory.CreateConnection();

            if (m_connection != null)
            {
                m_channel = m_connection.CreateModel();

                if (m_channel != null)
                {
                    m_channel.ExchangeDeclare(m_strMessageName, ExchangeType.Fanout);
                    /*m_channel.QueueDeclare(queue: m_strMessageName,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);*/
                }
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string strMessage = textBoxMessage.Text.Trim();

            if (strMessage.Length == 0)
            {
                textBoxMessage.Focus();
                MessageBox.Show("전송할 메시지를 입력하세요.");
                return;
            }

            //var properties = m_channel.CreateBasicProperties();
            // 멀티 클라이언트 사용여부
            //properties.Persistent = true;

            string message = strMessage;
            var body = Encoding.UTF8.GetBytes(message);

            m_channel.BasicPublish(exchange: m_strMessageName,
                                 routingKey: "",
                                 basicProperties: null,
                                 body: body);
            System.Diagnostics.Trace.WriteLine(" [x] Sent {0}", message);

            /*var factory = new ConnectionFactory() { HostName = m_strServerIP };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: m_strMessageName,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var properties = channel.CreateBasicProperties();
                    // 멀티 클라이언트 사용여부
                    properties.Persistent = true;

                    string message = strMessage;
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                         routingKey: m_strMessageName,
                                         basicProperties: properties,
                                         body: body);
                    System.Diagnostics.Trace.WriteLine(" [x] Sent {0}", message);
                    label2.Text = string.Format(" [x] Received {0}", message);
                }
            }*/
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_runThread = false;
        }

        private void ReceiveThread()
        {
            m_runThread = true;

            var factory = new ConnectionFactory() { HostName = m_strServerIP };

            using (var connection = factory.CreateConnection())
            {
                object data;

                if (connection.ServerProperties.TryGetValue("version", out data))
                {
                    byte[] arr = (byte[])data;
                    string str = Encoding.UTF8.GetString(arr);
                    System.Diagnostics.Trace.WriteLine("RabbitMQ Version : " + str);
                }
                using (var channel = connection.CreateModel())
                {
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
                        ProcSerialize(ea.Body);

                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        string strLog = textBoxLog.Text.Trim();

                        if (strLog.Length == 0)
                            strLog = message;
                        else
                            strLog += "\r\n" + message;

                        this.Invoke((MethodInvoker)delegate
                        {
                            textBoxLog.Text = strLog;
                        });

                        System.Diagnostics.Trace.WriteLine(" [x] Received {0}", message);
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

        private void textBoxMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnSend_Click(null, null);
        }

        private void ProcSerialize(byte[] arr)
        {
            IBS.OWS.Defines.IBSAlarm alarm = null;
            System.Runtime.Serialization.IFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(arr))
            {
                alarm = (IBS.OWS.Defines.IBSAlarm)formatter.Deserialize(stream);
            }
        }
    }
}
