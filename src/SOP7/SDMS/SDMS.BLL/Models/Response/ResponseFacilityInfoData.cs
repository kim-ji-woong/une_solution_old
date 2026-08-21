using System;
using System.Collections.Generic;
using SDMS.Model.Facility;
using SDMS.Model.Spatial;

namespace SDMS.BLL.Models.Response
{
    public class ResponseFacilityInfoData : MessageResult
    {
        private string m_strModelName = "";
        private string m_strFacilityName = "";
        private List<InfoData> m_datas = new List<InfoData>();

        public string ModelName
        {
            get { return m_strModelName; }
            set { m_strModelName = value; }
        }

        public string FacilityName
        {
            get { return m_strFacilityName; }
            set { m_strFacilityName = value; }
        }

        public List<InfoData> Datas
        {
            get { return m_datas; }
        }
    }

    public class ResponseAllFacilityInfo : MessageResult
    {
        private List<Info> m_infos = new List<Info>();

        public List<Info> Infos
        {
            get { return m_infos; }
            set { m_infos = value; }
        }
    }

    public class ResponseFacilityInfoDatas : MessageResult
    {
        private List<ResponseFacilityInfoData> m_datas = new List<ResponseFacilityInfoData>();
        public List<ResponseFacilityInfoData> Datas
        {
            get { return m_datas; }
        }
    }

    public class ResponseBuildingData : MessageResult
    {
        private string m_strDisplayText = "";
        private List<BuildingData> m_datas = new List<BuildingData>();

        public string DisplayText
        {
            get { return m_strDisplayText; }
            set { m_strDisplayText = value; }
        }

        public List<BuildingData> Datas
        {
            get { return m_datas; }
        }
    }

    public class ResponseBuildingGroupData : MessageResult
    {
        private string m_strDisplayText = "";
        private List<BuildingGroupData> m_datas = new List<BuildingGroupData>();

        public string DisplayText
        {
            get { return m_strDisplayText; }
            set { m_strDisplayText = value; }
        }

        public List<BuildingGroupData> Datas
        {
            get { return m_datas; }
        }
    }
}
