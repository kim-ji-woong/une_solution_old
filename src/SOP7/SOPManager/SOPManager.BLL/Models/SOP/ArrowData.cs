using System;
using System.Collections.Generic;
using System.Text;

namespace SOPManager.BLL.Models.SOP
{
    public class ArrowData
    {
        private int m_nID = -1;
        private int m_nBeginComponentID = -1;
        private int m_nBeginComponentColumnIndex = -1;
        private int m_nBeginComponentRowIndex = -1;
        private int m_nBeginComponentPosition = -1;
        private int m_nEndComponentID = -1;
        private int m_nEndComponentColumnIndex = -1;
        private int m_nEndComponentRowIndex = -1;
        private int m_nEndComponentPosition = -1;
        private bool? m_isYes = true;
        private string m_strText = null;

        public int ID
        {
            get { return m_nID; }
            set { m_nID = value; }
        }

        public int BeginComponentID
        {
            get { return m_nBeginComponentID; }
            set { m_nBeginComponentID = value; }
        }

        public int BeginComponentColumnIndex
        {
            get { return m_nBeginComponentColumnIndex; }
            set { m_nBeginComponentColumnIndex = value; }
        }

        public int BeginComponentRowIndex
        {
            get { return m_nBeginComponentRowIndex; }
            set { m_nBeginComponentRowIndex = value; }
        }

        public int BeginComponentPosition
        {
            get { return m_nBeginComponentPosition; }
            set { m_nBeginComponentPosition = value; }
        }

        public int EndComponentID
        {
            get { return m_nEndComponentID; }
            set { m_nEndComponentID = value; }
        }

        public int EndComponentColumnIndex
        {
            get { return m_nEndComponentColumnIndex; }
            set { m_nEndComponentColumnIndex = value; }
        }

        public int EndComponentRowIndex
        {
            get { return m_nEndComponentRowIndex; }
            set { m_nEndComponentRowIndex = value; }
        }

        public int EndComponentPosition
        {
            get { return m_nEndComponentPosition; }
            set { m_nEndComponentPosition = value; }
        }

        public bool? IsYes
        {
            get { return m_isYes; }
            set { m_isYes = value; }
        }

        public string Text
        {
            get { return m_strText; }
            set { m_strText = value; }
        }
    }
}
