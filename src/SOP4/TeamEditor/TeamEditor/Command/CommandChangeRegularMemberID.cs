using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamEditor.Command
{
    // 사번 변경
    public class CommandChangeRegularMemberID : CommandEx
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
