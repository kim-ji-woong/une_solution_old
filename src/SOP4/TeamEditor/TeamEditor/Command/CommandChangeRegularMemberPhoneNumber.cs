using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamEditor.Command
{
    // 휴대전화번호 변경
    public class CommandChangeRegularMemberPhoneNumber : CommandEx
    {
        public override void Do()
        {
            throw new NotImplementedException();
        }

        public override void RollBack()
        {
            throw new NotImplementedException();
        }

        public override void SaveDB(DBUtility.WebDBManager dbMgr, bool dir)
        {
            throw new NotImplementedException();
        }
    }
}
