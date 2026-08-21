using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace libSMS
{
    public interface IMessageClient : IDisposable
    {
        // 여러명에게 동시에 메시지를 보낼 경우 사용
        // 첫번째 메시지를 보낼때 호출
        void BeginSend();
        // 마지막 메시지를 보낸후 호출
        void EndSend();

        bool SendSMS(string szCaller, string szReciver, string szContent, bool bEncryptCaller = false);
        bool SendSMS(List<MessageContent> arMessages);

        // 이미지, 동영상등을 포함한 MMS를 보낼수 있는가?
        bool CanUseMMS();
        // strContentPath : 외부 컨텐츠 파일의 경로
        bool SendMMS(string szCaller, string szReciver, string szContent, string strTitle = "", MessageContentMMS.ContentType contentType = MessageContentMMS.ContentType.None, string strContentPath = "");
        bool SendMMS(List<MessageContentMMS> arMessages);

        // 메시지의 길이제한 바이트 수
        int GetMessageLength();
    }
}
