using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SOPWebServer
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드 및 config 파일에서 인터페이스 이름 "IPostBox"을 변경할 수 있습니다.
    [ServiceContract(CallbackContract = typeof(IPostMan), SessionMode = SessionMode.Required)]
    public interface IPostBox
    {
        [OperationContract]
        bool Regist(int clientType, int clientSubType);
        [OperationContract]
        int SendMail(int header, byte[] messages, bool isLast);
        [OperationContract]
        int GetMaxMailSize();
    }

    [ServiceContract]
    public interface IPostMan
    {
        [OperationContract(IsOneWay = true)]
        void OnRing(int header, byte[] messages);
    }
}
