using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDMS
{
    public interface ICCTVFormOwner
    {
        bool ThumbnailMode
        {
            get;
        }
    }


    public class CCTVSelectionManager
    {
        private static CCTVSelectionManager m_instance = null;

        public static CCTVSelectionManager Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new CCTVSelectionManager();
                return m_instance;
            }
        }

        public CCTVSelectionManager()
        {

        }

        private Form4CCTV m_currentCCTV = null;
        public void SetCurrent(Form4CCTV form)
        {
            m_currentCCTV = form;
        }

        public Form4CCTV GetCurrent()
        {
            return m_currentCCTV;
        }
    }
}
