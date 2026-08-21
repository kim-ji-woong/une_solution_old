using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Collections.Concurrent;

namespace SOPWebServer
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드, svc 및 config 파일에서 클래스 이름 "PostBoxService"을 변경할 수 있습니다.
    // 참고: 이 서비스를 테스트하기 위해 WCF 테스트 클라이언트를 시작하려면 솔루션 PostBoxService.svc나 PostBoxService.svc.cs를 선택하고 디버깅을 시작하십시오.
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class PostBoxService : IPostBox
    {
        private OperationContext m_context = null;
        private List<byte[]> m_segmentBytes = new List<byte[]>();
        private int m_nSegmentBytesCount = 0;

        public PostBoxService()
        {
            m_context = OperationContext.Current;
        }

        /// <summary>
        /// SOPWebServer 서비스를 사용하는 모든 Client들은 반드시 Regist()를 호출하여 서버에 자신을 등록해야 한다.
        /// 그래야만, 콜백(OnRing)을 받을수 있다.
        /// </summary>
        /// <param name="clientType"></param>
        /// <param name="clientSubType"></param>
        /// <returns></returns>
        public bool Regist(int clientType, int clientSubType)
        {
            System.Diagnostics.Trace.WriteLine("Regist : " + clientType.ToString() + ", " + m_context.SessionId);

            string strIP;
            int nPort;

            bool result = PostOffice.Instance.AddClient(clientType, clientSubType, m_context, out strIP, out nPort);

            if (result)
            {
                PostOffice.Instance.AddClient(clientType, clientSubType, strIP, nPort);
            }

            return result;
        }

        /// <summary>
        /// messages 크기가 GetMaxMailSize()보다 클 경우 SendMail()을 통하여 메시지를 전달할 수 없다.
        /// 이 경우 메시지를 쪼개어 보낼수 있다.
        /// 서버는 클라이언트가 보내오는 메시지를 순서대로 받아 처리한다.
        /// </summary>
        /// <param name="isLast">
        /// 이 값이 true이면 이제까지 받았던 모든 messages를 더하여 한꺼번에 처리한다.
        /// false이면 다음 SendMail() 함수를 기다린다.
        /// </param>
        /// <returns> 메시지 전송에 성공하면 0을 리턴한다.
        ///           0이 아닌 값은 모두 에러인데, 그 의미는 ErrorMessageType을 참조한다.
        /// </returns>
        public int SendMail(int header, byte[] messages, bool isLast)
        {
            if (messages != null)
            {
                m_segmentBytes.Add(messages);
                m_nSegmentBytesCount += messages.Length;
            }

            if (isLast)
            {
                int nIndex = 0;
                byte[] bytes = m_nSegmentBytesCount > 0 ? new byte[m_nSegmentBytesCount] : null;

                foreach (byte[] message in m_segmentBytes)
                {
                    int len = message.Length;
                    Buffer.BlockCopy(message, 0, bytes, nIndex, len);
                    nIndex += len;
                }

                m_segmentBytes.Clear();
                m_nSegmentBytesCount = 0;
                return PostOffice.Instance.ReceiveMail(m_context, header, bytes);
            }

            return ErrorMessageType.SUCCESS;
        }

        /// <summary>
        /// SendMail()을 통하여 보낼수 있는 최대 메시지 크기를 알려준다.
        /// 이 보다 큰 메시지를 보내려고 하면 클라이언트 측에서 오류가 발생한다.
        /// </summary>
        /// <returns> 최대 메시지의 Byte 크기
        /// </returns>
        public int GetMaxMailSize()
        {
            return 16300;
        }
    }
}
