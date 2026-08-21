using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;

namespace UnEService
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드 및 config 파일에서 인터페이스 이름 "IUploadService"을 변경할 수 있습니다.
    [ServiceContract]
    public interface IUpload
    {
        [OperationContract]
        string Upload(string fileName, byte[] bytes, bool isFirst);
        [OperationContract]
        string Upload2(string fileName, byte[] bytes, bool isFirst, string folderPath);
        [OperationContract]
        int GetMaxSegmentSize();
        [OperationContract]
        string RemoveFile(string fileName);
        [OperationContract]
        string RemoveFile2(string fileName, string folderPath);
        [OperationContract]
        string RemoveAll();
        [OperationContract]
        string RemoveAll2(string folderPath);

        [OperationContract]
        string ExtractToTrg(string strSrcFile);
        [OperationContract]
        string ExtractToTrg2(string strSrcFile, string strTrgPath);
    }
}
