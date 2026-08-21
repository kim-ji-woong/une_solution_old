using System;
using System.Collections.Generic;
using System.Text;

namespace TeamEditor.IDAL
{
    public interface IDelete
    {
        bool DeleteRegularMember(int id, out string strErrorMessage);
        bool DeleteRegular(int id, out string strErrorMessage);
        bool DeleteTemporary(int id, out string strErrorMessage);
        bool DeleteTemporaryMember(int id, out string strErrorMessage);
        bool DeleteOptions(int id, out string strErrorMessage);
        bool DeleteRegularMember2(int id, out string strErrorMessage);
    }
}
