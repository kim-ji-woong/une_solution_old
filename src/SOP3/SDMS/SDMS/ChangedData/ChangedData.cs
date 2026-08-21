using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDMS
{
    public abstract class ChangedData
    {
        public abstract bool Update(DBUtility.WebDBManager dbMgr);
        public abstract void AddToManager(IChangedDataManager mgr);

        public bool IsDeleting
        {
            get;
            set;
        }

        protected void AddQueryString(ref string strSQL, string strValue)
        {
            if (strSQL.Length == 0)
                strSQL = strValue;
            else
                strSQL += ", " + strValue;
        }
    }

    // struct와 같이 null이 허용되지 않는 데이터를 위한 Wrapper 클래스
    public class VariousData<DataType>
    {
        private DataType data;

        public DataType Data
        {
            get { return data; }
            set { data = value; }
        }

        public VariousData()
        {
        }

        public VariousData(DataType data)
        {
            this.data = data;
        }
    }
}
