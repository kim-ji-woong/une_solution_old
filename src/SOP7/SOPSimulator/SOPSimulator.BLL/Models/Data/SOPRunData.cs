using Common.Model.History;
using SOPManager.BLL.Models.SOP;
using SOPManager.Model.Sop.Category;
using System;
using System.Collections.Generic;
using System.Text;

namespace SOPSimulator.BLL.Models.Data
{
    public class SOPRunData
    {
        private string m_strKey = "";
        public string Key
        {
            get { return m_strKey; }
            set { m_strKey = value; }
        }
        
        private string m_strPosition = "";
                
        public string Position
        {
            get { return m_strPosition; }
            set { m_strPosition = value; }
        }

        private int? m_nSensorZoneHistoryID = null;
        public int? SensorZoneHistoryID
        {
            get { return m_nSensorZoneHistoryID; }
            set { m_nSensorZoneHistoryID = value; }
        }

        private SOPData m_sopData = null;
        public SOPData SOPData
        {
            get { return m_sopData; }
            set { m_sopData = value; }
        }
    }
}
