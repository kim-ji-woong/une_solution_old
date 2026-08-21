using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingSMS
{
    public abstract class SendManager
    {
        protected Building m_building = null;
        protected int m_nFloorIndex = 0;
        protected Dictionary<int, bool> m_dicSend = new Dictionary<int, bool>();
        protected int m_nTimes = 0;

        public SendManager(Building building, int nFloorIndex)
        {
            m_building = building;
            m_nFloorIndex = nFloorIndex;

            for (int i=building.MinFloor;i<=building.MaxFloor;i++)
            {
                m_dicSend[i] = false;
            }
        }

        public abstract bool GetNext(List<int> floors, out bool isLast);

        protected void AddFloor(int nFloorIndex, List<int> floors)
        {
            floors.Add(nFloorIndex);
            m_dicSend[nFloorIndex] = true;
        }
    }
}
