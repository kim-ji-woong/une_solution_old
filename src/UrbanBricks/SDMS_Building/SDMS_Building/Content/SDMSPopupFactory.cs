using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Sensor;

namespace SDMS_Building.Content
{
    public class SDMSPopupFactory : IPopupFactory
    {
        private static SDMSPopupFactory m_Instance = new SDMSPopupFactory();
        public static SDMSPopupFactory Instance
        {
            get { return SDMSPopupFactory.m_Instance; }
            set { SDMSPopupFactory.m_Instance = value; }
        }

        private SDMSPopupFactory()
        {
        }

        public void Init()
        {
            PopupFactoryHelper.SetFactory(m_Instance);
        }

        public IPOIPopup CreatePopup(ISensorTooltipOwner view, ISensor sensor, int nType)
        {
            return null;
            //return new TooltipSensor(view, sensor, nType);
        }

        public IPOIPopup CreatePopup(ISensorTooltipOwner view, IFacility sensor)
        {
            if (sensor.GetType() == typeof(CCTV))
                return TooltipCCTVCtrl2.MakeInstance(view, (CCTV)sensor);
            
            return null;
        }

        public IPOIPopup CreatePopup(ISensorTooltipOwner view, IFacility equip, IFacility.FacilityType type)
        {
            return new TooltipHandler(equip);
        }

        public void CloseAll()
        {
            TooltipCCTVCtrl2.CloseAll();
        }
    }
}
