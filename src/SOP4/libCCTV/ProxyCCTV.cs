using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;

namespace UnE.CCTV
{
    public class ProxyCCTV
    {
        private static ProxyCCTV m_Instance = null;
        public static ProxyCCTV Instance
        {
            get 
            {
                if (m_Instance == null)
                    m_Instance = new ProxyCCTV();

                return m_Instance; 
            }
        }

        private bool m_bShowEquipZoneCCTV = false;
        public bool ShowEquipZoneCCTV
        {
            get { return m_bShowEquipZoneCCTV; }
            set { m_bShowEquipZoneCCTV = value; }
        }

        private EquipmentZone m_CurrentZone = null;
        public EquipmentZone CurrentEquipZone
        {
            get { return m_CurrentZone; }
            set { m_CurrentZone = value; }
        }

        private bool m_bEquipZoneCCTVMode = false;
        public bool EquipZoneCCTVMode
        {
            get { return m_bEquipZoneCCTVMode; }
            set { m_bEquipZoneCCTVMode = value; }
        }

        private FormCCTVList m_ListForm = null;
        public FormCCTVList CCTVList
        {
            get { return m_ListForm; }
            set { m_ListForm = value; }
        }

        public ProxyCCTV()
        {
           
        }        
    }
}
