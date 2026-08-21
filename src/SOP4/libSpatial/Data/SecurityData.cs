using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnE.Security
{
    public class SecurityType
    {
        protected List<UnE.Sensor.IFacility.FacilityType> m_facilityTypes = new List<Sensor.IFacility.FacilityType>();
        protected int m_nTypeID = 0;
        protected string m_strTypeName = "";

        public List<UnE.Sensor.IFacility.FacilityType> LinkedFacilityTypes
        {
            get { return m_facilityTypes; }
        }

        public int TypeID
        {
            get { return m_nTypeID; }
            set { m_nTypeID = value; }
        }

        public string TypeName
        {
            get { return m_strTypeName; }
            set { m_strTypeName = value; }
        }

        public override string ToString()
        {
            return TypeName;
        }
    }
}
