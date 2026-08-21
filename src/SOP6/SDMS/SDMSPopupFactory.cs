using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnE.Spatial;
using UnE.Sensor;

namespace SDMS
{
    public class SDMSPopupFactory : IPopupFactory
    {
        private static SDMSPopupFactory m_Instance = new SDMSPopupFactory();
        public static SDMSPopupFactory Instance
        {
            get { return SDMSPopupFactory.m_Instance; }
            set { SDMSPopupFactory.m_Instance = value; }
        }

        static SDMSPopupFactory()
        {
            PopupFactoryHelper.SetFactory(m_Instance);
        }

        public IPOIPopup CreatePopup(ISensorTooltipOwner view, ISensor sensor, int nType)
        {
            return new TooltipSensor(view, sensor, nType);
        }

        public IPOIPopup CreatePopup(ISensorTooltipOwner view, IFacility sensor)
        {
            if (sensor.GetType() == typeof(CCTV))
            {
                return TooltipCCTVCtrl2.MakeInstance(view, (CCTV)sensor);
                //return TooltipCCTVCtrl.MakeInstance(view, (CCTV)sensor);
            }

            return null;
        }

        public IPOIPopup CreatePopup(ISensorTooltipOwner view, IFacility equip, IFacility.FacilityType type)
        {
            if (equip.GetType() == typeof(FireEquipment))
                return new TooltipFireEquipment(view, (FireEquipment)equip, type);

            return null;
        }
    }
}
