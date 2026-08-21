using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace UnEService
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드 및 config 파일에서 인터페이스 이름 "IWebDB"을 변경할 수 있습니다.
    [ServiceContract]
    public interface IWebDB
    {
        [OperationContract]
        string[] RunQuery(string dbName, string dbType, string query);

        [OperationContract]
        long BeginBatch(string dbName, string dbType, out string errorMessage);

        [OperationContract]
        string BatchCommmit(long transactionKey);

        [OperationContract]
        string BatchRollback(long transactionKey);

        [OperationContract]
        string[] BatchQuery(string query, long transactionKey);

        [OperationContract]
        long BeginMultiQuery();

        [OperationContract]
        string AddMultiQuery(string strSQL, long key);

        [OperationContract]
        string[] RunMultiQuery(string dbName, string dbType, long key);

        [OperationContract]
        string[] BatchMultiQuery(long key, long transactionKey);

        [OperationContract]
        string CancelMultiQuery(long key);

        // TODO: 여기에 서비스 작업을 추가합니다.
    }
}
