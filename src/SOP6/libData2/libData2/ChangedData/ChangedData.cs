namespace SDMS
{
	public abstract class ChangedData
	{
		public abstract bool Update(DBUtility2.WebDBManager dbMgr);

		public abstract void AddToManager(IChangedDataManager mgr);

		public bool IsDeleting
		{
			get;
			set;
		}

        public abstract bool IsOriginStatus();

		protected void AddQueryString(ref string strSQL, string strValue)
		{
			if (strSQL.Length == 0)
				strSQL = strValue;
			else
				strSQL += ", " + strValue;
		}
	}
}