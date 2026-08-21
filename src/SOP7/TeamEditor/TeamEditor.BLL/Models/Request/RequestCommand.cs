using System;
using System.Collections.Generic;
using System.Text;
using TeamEditor.BLL.Models.Response;
using TeamEditor.IDAL;

namespace TeamEditor.BLL.Models.Request
{
    public abstract class RequestCommand
    {
        private IDataManager m_dataManager = null;
        public IDataManager DataManager
        {
            get { return m_dataManager; }
            set { m_dataManager = value; }
        }

        private bool m_isRedo = false;
        public bool IsRedo
        {
            get { return m_isRedo; }
            set { m_isRedo = value; }
        }

        private string m_strKey = "";
        public string Key
        {
            get { return m_strKey; }
            set { m_strKey = value; }
        }

        public abstract void SaveDB();
    }
}
