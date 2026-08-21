using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HSMSServer2
{
    /// <summary>
    /// Edit 데이터의 기본 클래스
    /// </summary>
    public class EditData
    {
        // HSMS 데이터가 업데이트 되는 경우
        public const int UPDATE = 1;
        // HSMS 데이터가 삭제 되는 경우
        public const int DELETE = 2;
        // HSMS 데이터가 추가 되는 경우
        public const int INSERT = 3;
        // ERP 데이터가 변경되어 데이터가 삭제되는 경우
        public const int REMOVE = 4;

        // ToDo : 공통으로 필요한 사항은 여기에..
    }


    
}
