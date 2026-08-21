using System;
using System.Collections.Generic;
using System.Text;

namespace Dashboard.BLL.Models.Request
{
    public class RequestData
    {
        private bool? m_requestUseSensor = null;
        private bool? m_requestWeeklyStatus = null;
        private bool? m_requestCurrentWorkPermit = null;

        public bool? RequestUseSensor
        {
            get { return m_requestUseSensor; }
            set { m_requestUseSensor = value; }
        }

        public bool? RequestWeeklyStatus
        {
            get { return m_requestWeeklyStatus; }
            set { m_requestWeeklyStatus = value; }
        }

        public bool? RequestCurrentWorkPermit
        {
            get { return m_requestCurrentWorkPermit; }
            set { m_requestCurrentWorkPermit = value; }
        }
    }

    public class RequestUseSensor
    {
        private int m_nBuildingGroup = -1;
        private int m_nBuilding = -1;
        private int m_nZone = -1;

        public int BuildingGroup
        {
            get { return m_nBuildingGroup; }
            set { m_nBuildingGroup = value; }
        }

        public int Building
        {
            get { return m_nBuilding; }
            set { m_nBuilding = value; }
        }

        public int Zone
        {
            get { return m_nZone; }
            set { m_nZone = value; }
        }
    }
}
