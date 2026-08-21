using SensorMaker.BLL.Models.Basic;
using SensorMaker.BLL.Models.Data;
using SensorMaker.BLL.Models.Data.Sensor;
using SensorMaker.BLL.Models.Request;
using SensorMaker.BLL.Models.Response;
using SensorMaker.BLL.Models.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using UnE.Geometry;
using static dnsData.Sensor.Facility;

namespace SensorMaker.BLL
{
    public class XmlManager
    {
        private string m_strSiteName = "";

        //Gltf Model
        private List<GltfModel> m_gltfModels = new List<GltfModel>();
        Dictionary<int, int> m_dicGltfModelParentIDs = new Dictionary<int, int>();
        private GltfOption m_gltfOption = new GltfOption();

        // SensorType
        private List<SensorSubType> m_fireSensorSubTypes = new List<SensorSubType>();
        private List<SensorType> m_sensorTypes = new List<SensorType>();

        // Key : BuildingGroup ID
        // Value : Parent ID
        private Dictionary<int, int> dicBuildingGroupParents = new Dictionary<int, int>();

        // Spatial
        private Dictionary<int, BuildingGroupData> m_dicBuildingGroups = new Dictionary<int, BuildingGroupData>();
        private Dictionary<int, BuildingData> m_dicBuildings = new Dictionary<int, BuildingData>();
        private Dictionary<int, ZoneData> m_dicZones = new Dictionary<int, ZoneData>();
        private Dictionary<int, EquipmentZoneData> m_dicEquipZones = new Dictionary<int, EquipmentZoneData>();

        // Sensor
        private List<FireSensor> m_fireSensors = new List<FireSensor>();
        private List<PSMSensor> m_psmSensors = new List<PSMSensor>();
        private List<EtcSensor> m_etcSensors = new List<EtcSensor>();
        private List<CCTVSensor> m_cctvs = new List<CCTVSensor>();

        public XmlManager()
        {

        }

        public ResponseOpenXML OpenXML(string strFilePath)
        {
            ResponseOpenXML res = new ResponseOpenXML();
            string strErrorMessage = null;

            try
            {
                XElement xe = System.Xml.Linq.XElement.Load(strFilePath);
                if (!ReadSpaces(xe, out strErrorMessage))
                {
                    res.Success = false;
                    res.Message = strErrorMessage;
                    return res;
                }
                
                if (!ReadSensors(xe, out strErrorMessage))
                {
                    res.Success = false;
                    res.Message = strErrorMessage;
                    return res;
                }

                /*
                using (XmlReader reader = XmlReader.Create(strFilePath))
                {
                    while (reader.Read())
                    {
                        bool isStartElement = reader.IsStartElement();
                        if (!isStartElement)
                            break;

                        string readerName = reader.Name.ToString();
                        switch (readerName)
                        {
                            case XmlKey.XName_Spaces:
                                if (!ReadSpaces(reader, out strErrorMessage))
                                {
                                    res.Success = false;
                                    res.Message = strErrorMessage;
                                    return res;
                                }
                                break;
                            case XmlKey.XName_Sensors:
                                if (!ReadSensors(reader, out strErrorMessage))
                                {
                                    res.Success = false;
                                    res.Message = strErrorMessage;
                                    return res;
                                }                                    
                                break;
                        }
                    }
                }
                */

                res.Success = true;
                res.SiteName = m_strSiteName;
                res.Models = m_gltfModels;
                res.GltfOption = m_gltfOption;
                res.SensorTypes = m_sensorTypes;

                ArrangeSpatial();
                foreach (BuildingGroupData bg in m_dicBuildingGroups.Values)
                {
                    if (res.BuildingGroups == null)
                        res.BuildingGroups = new List<BuildingGroupData>();

                    res.BuildingGroups.Add(bg);
                }
                foreach (KeyValuePair<int, ZoneData> pair in m_dicZones)
                {
                    if (pair.Value.BuildingID == null)
                        res.OutdoorZones.Add(pair.Value);
                }

                res.FireSensors = m_fireSensors;
                res.PSMSensors = m_psmSensors;
                res.EtcSensors = m_etcSensors;
                res.Cctvs = m_cctvs;

            }
            catch (Exception ex)
            {
                strErrorMessage = "OpenXML : " + ex.Message;
                res.Success = false;
                res.Message = strErrorMessage;                
            }

            return res;
        }

        private XElement FindElement(XElement node, string strNodeName, bool bFindChildNodes = true)
        {
            if (node.Name == strNodeName)
                return node;

            foreach (XElement element in node.Elements())
            {
                if (bFindChildNodes)
                { 
                    XElement _element = FindElement(element, strNodeName);

                    if (_element != null)
                        return _element;
                }
                else
                {
                    if (element.Name == strNodeName)
                        return element;
                }
                    

            }

            return null;
        }

        private List<XElement> FindElements(XElement node, string strNodeName, bool bFindChildNodes = true)
        {
            List<XElement> _elements = new List<XElement>();

            if (node.Name == strNodeName)
                return _elements;

            foreach (XElement element in node.Elements())
            {
                if (bFindChildNodes)
                {
                    XElement _element = FindElement(element, strNodeName, bFindChildNodes);

                    if (_element != null)
                        _elements.Add(_element); 
                }
                else
                {
                    if (element.Name == strNodeName)
                        _elements.Add(element);
                }
            }

            return _elements;
        }

        private string FindElementValue(XElement node, string strNodeName)
        {
            if (node.Name == strNodeName)
                return "";

            foreach (XElement element in node.Elements())
            {
                XElement _element = FindElement(element, strNodeName);

                if (_element != null)
                    return _element.Value;
            }

            return "";
        }

        #region Spaces
        private bool ReadSpaces(XElement xe, out string strErrorMessage)
        {
            try
            {
                strErrorMessage = null;

                XElement xeSpace = FindElement(xe, XmlKey.XName_Spaces);
                
                ReadSite(xeSpace, out strErrorMessage);
                ReadGltf(xeSpace, out strErrorMessage);

                XElement xeBuildingGroups = FindElement(xe, XmlKey.XName_BuildingGroups);
                List<XElement> xeBuildingGroupList = FindElements(xeBuildingGroups, XmlKey.XName_BuildingGroup);
                foreach (XElement xeBuildingGroup in xeBuildingGroupList)
                {
                    ReadBuildingGroup(xeBuildingGroup, out strErrorMessage);
                }

                XElement xeBuildings = FindElement(xe, XmlKey.XName_Buildings);
                List<XElement> xeBuildingList = FindElements(xeBuildings, XmlKey.XName_Building);
                foreach (XElement xeBuilding in xeBuildingList)
                {
                    ReadBuilding(xeBuilding, out strErrorMessage);
                }

                XElement xeZones = FindElement(xe, XmlKey.XName_Zones);
                List<XElement> xeZoneList = FindElements(xeZones, XmlKey.XName_Zone);
                foreach (XElement xeZone in xeZoneList)
                {
                    ReadZone(xeZone, out strErrorMessage);
                }

                XElement xeEquipmentZoneZones = FindElement(xe, XmlKey.XName_EquipmentZones);
                List<XElement> xeEquipmentZoneList = FindElements(xeEquipmentZoneZones, XmlKey.XName_EquipmentZone);
                foreach (XElement xeEquipmentZone in xeEquipmentZoneList)
                {
                    ReadEquipmentZone(xeEquipmentZone, out strErrorMessage);
                }

                //case XmlKey.XName_BuildingGroup:
                //            if (!ReadBuildingGroup(reader, out strErrorMessage))
                //    return false;
                //break;
                //        case XmlKey.XName_Building:
                //            if (!ReadBuilding(reader, out strErrorMessage))
                //    return false;
                //break;
                //        case XmlKey.XName_Zone:
                //            if (!ReadZone(reader, out strErrorMessage))
                //    return false;
                //break;
                //        case XmlKey.XName_EquipmentZone:
                //            if (!ReadEquipmentZone(reader, out strErrorMessage))
                //    return false;
                //break;

                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadSpaces : " + ex.Message;
                return false;
            }
        }

        private bool ReadSpaces(XmlReader reader, out string strErrorMessage)
        {
            try
            {
                strErrorMessage = null;

                while (reader.Read())
                {
                    bool isStartElement = reader.IsStartElement();
                    string readerName = reader.Name.ToString();
                    if (!isStartElement && readerName == XmlKey.XName_Spaces)
                        break;

                    switch (readerName)
                    {
                        case XmlKey.XName_Site:
                            if (!ReadSite(reader, out strErrorMessage))
                                return false;
                            break;
                        case XmlKey.XName_Gltf:
                            if (!ReadGltf(reader, out strErrorMessage))
                                return false;
                            break;
                        case XmlKey.XName_BuildingGroup:
                            if (!ReadBuildingGroup(reader, out strErrorMessage))
                                return false;
                            break;
                        case XmlKey.XName_Building:
                            if (!ReadBuilding(reader, out strErrorMessage))
                                return false;
                            break;
                        case XmlKey.XName_Zone:
                            if (!ReadZone(reader, out strErrorMessage))
                                return false;
                            break;
                        case XmlKey.XName_EquipmentZone:
                            if (!ReadEquipmentZone(reader, out strErrorMessage))
                                return false;
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadSpaces : " + ex.Message;
                return false;
            }
        }

        private bool ReadSite(XElement xe, out string strErrorMessage)
        {
            try
            {                
                strErrorMessage = null;

                m_strSiteName = FindElementValue(xe, XmlKey.XName_Site);
                
                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadSite : " + ex.Message;
                return false;
            }
        }

        private bool ReadSite(XmlReader reader, out string strErrorMessage)
        {
            try
            {
                strErrorMessage = null;

                while (reader.Read())
                {                    
                    bool isStartElement = reader.IsStartElement();                    
                    if (!isStartElement)
                        break;

                    string readerName = reader.Name.ToString();

                    reader.MoveToContent();
                    string strInner = reader.ReadInnerXml();

                    switch (readerName)
                    {
                        case XmlKey.XName_Name:
                            m_strSiteName = strInner; // reader.ReadInnerXml();
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadSite : " + ex.Message;
                return false;
            }
        }

        private void ArrangeSpatial()
        {
            foreach (KeyValuePair<int, int> pair in dicBuildingGroupParents)
            {
                BuildingGroupData bg, parent;

                if (m_dicBuildingGroups.TryGetValue(pair.Key, out bg) && m_dicBuildingGroups.TryGetValue(pair.Value, out parent))
                {
                    bg.Parent = parent;
                }
            }
        }

        private bool ReadGltf(XElement xe, out string strErrorMessage)
        {
            try
            {
                strErrorMessage = null;

                XElement xeGltfOption = FindElement(xe, XmlKey.XName_GltfOption);
                if (!ReadGltfOption(xeGltfOption, out strErrorMessage))
                    return false;

                if (!ReadGltfModels(xe, out strErrorMessage))
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadGltf : " + ex.Message;
                return false;
            }
        }

        private bool ReadGltf(XmlReader reader, out string strErrorMessage)
        {
            try
            {
                strErrorMessage = null;

                while (reader.Read())
                {
                    bool isStartElement = reader.IsStartElement();
                    string readerName = reader.Name.ToString();
                    if (!isStartElement && readerName == XmlKey.XName_Gltf)
                        break;

                    switch (readerName)
                    {
                        case XmlKey.XName_GltfModels:
                            if (!ReadGltfModels(reader, out strErrorMessage))
                                return false;
                            break;
                        case XmlKey.XName_GltfOption:
                            if (!ReadGltfOption(reader, out strErrorMessage))
                                return false;
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadGltf : " + ex.Message;
                return false;
            }
        }

        private bool ReadGltfModels(XElement xe, out string strErrorMessage)
        {
            try
            {
                strErrorMessage = null;

                XElement xeGltfModels = FindElement(xe, XmlKey.XName_GltfModels);
                List<XElement> xeGltfModelList = FindElements(xeGltfModels, XmlKey.XName_GltfModel, false);
                foreach (XElement xe2 in xeGltfModelList)
                {
                    GltfModel gltf = ReadGltfModel(xe2, out strErrorMessage);
                    if (gltf != null)
                    {
                        m_gltfModels.Add(gltf);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadGltfModels : " + ex.Message;
                return false;
            }
        }

        private GltfModel ReadGltfModel(XElement xe, out string strErrorMessage)
        {
            strErrorMessage = null;

            try
            {
                GltfModel gltf = new GltfModel();

                string strID = FindElementValue(xe, XmlKey.XName_ID);
                int nID;
                if (int.TryParse(strID, out nID))
                    gltf.ID = nID;
                else
                    throw new ApplicationException("ID가 잘못되었습니다.");

                gltf.ModelName = FindElementValue(xe, XmlKey.XName_ModelName);

                string strParentID = FindElementValue(xe, XmlKey.XName_ParentID);
                int nParentID;
                if (int.TryParse(strParentID, out nParentID))
                    gltf.ParentID = nParentID;
                else
                    gltf.ParentID = null;

                string strSiteID = FindElementValue(xe, XmlKey.XName_SiteID);
                int nSiteID;
                if (int.TryParse(strSiteID, out nSiteID))
                    gltf.SiteID = nSiteID;
                else
                    throw new ApplicationException("SiteID가 잘못되었습니다.");

                List<GltfModel> childModels = ReadChildModels(xe, out strErrorMessage);
                if (childModels != null)
                    foreach (GltfModel childModel in childModels)
                        gltf.ChildModels.Add(childModel);

                List<SDMS.Model.GLTF.ModelData> modelDatas = ReadModelDatas(xe, out strErrorMessage);
                if (modelDatas != null)
                {
                    foreach (SDMS.Model.GLTF.ModelData item in modelDatas)
                        gltf.ModelDatas.Add(item);
                }

                List<SDMS.Model.GLTF.ModelOrthoData> modelOrthoData = ReadModelOrthoDatas(xe, out strErrorMessage);
                if (modelOrthoData != null)
                {
                    foreach (SDMS.Model.GLTF.ModelOrthoData item in modelOrthoData)
                        gltf.ModelOrthoDatas.Add(item);
                }

                if (gltf.ID > 0)
                {
                    if (gltf.ParentID != null)
                        m_dicGltfModelParentIDs[gltf.ID] = (int)gltf.ParentID;
                    return gltf;
                }

                return null;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadGltfModel : " + ex.Message;
                return null;
            }
        }

        private bool ReadGltfModels(XmlReader reader, out string strErrorMessage)
        {
            try
            {
                strErrorMessage = null;

                while (reader.Read())
                {
                    bool isStartElement = reader.IsStartElement();
                    if (!isStartElement)
                        break;

                    string readerName = reader.Name.ToString();
                    switch (readerName)
                    {
                        case XmlKey.XName_GltfModel:
                            GltfModel gltf = ReadGltfModel(reader, out strErrorMessage);
                            if (gltf != null)
                            {
                                m_gltfModels.Add(gltf);
                            }
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadGltfModels : " + ex.Message;
                return false;
            }
        }

        private GltfModel ReadGltfModel(XmlReader reader, out string strErrorMessage)
        {
            try
            {
                GltfModel gltf = new GltfModel();

                strErrorMessage = null;

                while (reader.Read())
                {
                    bool isStartElement = reader.IsStartElement();
                    if (!isStartElement)
                        break;

                    string readerName = reader.Name.ToString();
                    switch (readerName)
                    {
                        case XmlKey.XName_ID:
                            int nID;
                            if (int.TryParse(reader.ReadInnerXml(), out nID))
                                gltf.ID = nID;
                            else
                                throw new ApplicationException("ID가 잘못되었습니다.");
                            break;
                        case XmlKey.XName_ModelName:
                            gltf.ModelName = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_ParentID:
                            int nParentID;
                            if (int.TryParse(reader.ReadInnerXml(), out nParentID))
                                gltf.ParentID = nParentID;
                            else
                                gltf.ParentID = null;
                            break;
                        case XmlKey.XName_SiteID:
                            int nSiteID;
                            if (int.TryParse(reader.ReadInnerXml(), out nSiteID))
                                gltf.SiteID = nSiteID;
                            else
                                throw new ApplicationException("SiteID가 잘못되었습니다.");
                            break;
                        case XmlKey.XName_ChildModels:
                            List<GltfModel> childModels = ReadChildModels(reader, out strErrorMessage);
                            if (childModels != null)
                                foreach (GltfModel childModel in childModels)
                                    gltf.ChildModels.Add(childModel);
                            break;
                        case XmlKey.XName_ModelDatas:
                            List<SDMS.Model.GLTF.ModelData> modelDatas = ReadModelDatas(reader, out strErrorMessage);
                            if (modelDatas != null)
                            {
                                foreach (SDMS.Model.GLTF.ModelData item in modelDatas)
                                    gltf.ModelDatas.Add(item);
                            }
                            break;
                        case XmlKey.XName_ModelOrthoDatas:
                            List<SDMS.Model.GLTF.ModelOrthoData> modelOrthoData = ReadModelOrthoDatas(reader, out strErrorMessage);
                            if (modelOrthoData != null)
                            {
                                foreach (SDMS.Model.GLTF.ModelOrthoData item in modelOrthoData)
                                    gltf.ModelOrthoDatas.Add(item);
                            }
                            break;
                    }
                }

                if (gltf.ID > 0)
                {
                    if (gltf.ParentID != null)
                        m_dicGltfModelParentIDs[gltf.ID] = (int)gltf.ParentID;
                    return gltf; 
                }

                return null;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadGltfModel : " + ex.Message;
                return null;
            }
        }

        private List<GltfModel> ReadChildModels(XElement xe, out string strErrorMessage)
        {
            strErrorMessage = null;
            List<GltfModel> childModels = new List<GltfModel>();

            try
            {
                XElement xe2 = FindElement(xe, XmlKey.XName_ChildModels);
                List<XElement> xChildModels = FindElements(xe2, XmlKey.XName_ChildModel);
                foreach (XElement xChildModel in xChildModels)
                {
                    GltfModel childModel = ReadGltfModel(xChildModel, out strErrorMessage);
                    if (childModel != null)
                        childModels.Add(childModel); 
                }

                return childModels;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadChildModels : " + ex.Message;
                return null;
            }
        }

        private List<GltfModel> ReadChildModels(XmlReader reader, out string strErrorMessage)
        {
            List<GltfModel> childModels = new List<GltfModel>();
            try
            {
                strErrorMessage = null;

                while (reader.Read())
                {
                    bool isStartElement = reader.IsStartElement();
                    if (!isStartElement)
                        break;

                    string readerName = reader.Name.ToString();
                    switch (readerName)
                    {
                        case XmlKey.XName_ChildModel:
                            GltfModel childModel = ReadGltfModel(reader, out strErrorMessage);
                            if (childModel != null)
                                childModels.Add(childModel);
                            break;
                    }
                }

                return childModels;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadChildModels : " + ex.Message;
                return null;
            }
        }

        private List<SDMS.Model.GLTF.ModelData> ReadModelDatas(XElement xe, out string strErrorMessage)
        {
            strErrorMessage = null;
            List<SDMS.Model.GLTF.ModelData> modelDatas = new List<SDMS.Model.GLTF.ModelData>();

            try
            {
                XElement xe2 = FindElement(xe, XmlKey.XName_ModelDatas, false);
                List<XElement> xModelDatas = FindElements(xe2, XmlKey.XName_ModelData, false);
                foreach (XElement xModelData in xModelDatas)
                {
                    SDMS.Model.GLTF.ModelData modelData = ReadModelData(xModelData, out strErrorMessage);
                    if (modelData != null)
                        modelDatas.Add(modelData); 
                }

                return modelDatas;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadModelDatas : " + ex.Message;
                return null;
            }
        }

        private List<SDMS.Model.GLTF.ModelData> ReadModelDatas(XmlReader reader, out string strErrorMessage)
        {
            List<SDMS.Model.GLTF.ModelData> modelDatas = new List<SDMS.Model.GLTF.ModelData>();
            try
            {
                strErrorMessage = null;

                while (reader.Read())
                {
                    bool isStartElement = reader.IsStartElement();
                    if (!isStartElement)
                        break;

                    string readerName = reader.Name.ToString();
                    switch (readerName)
                    {
                        case XmlKey.XName_ModelData:
                            SDMS.Model.GLTF.ModelData modelData = ReadModelData(reader, out strErrorMessage);
                            if (modelData != null)
                                modelDatas.Add(modelData);
                            break;
                    }
                }

                return modelDatas;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadModelDatas : " + ex.Message;
                return null;
            }
        }

        private SDMS.Model.GLTF.ModelData ReadModelData(XElement xe, out string strErrorMessage)
        {
            SDMS.Model.GLTF.ModelData modelData = new SDMS.Model.GLTF.ModelData();
            strErrorMessage = null;

            try
            {
                string strID = FindElementValue(xe, XmlKey.XName_ID);
                int nID;
                if (int.TryParse(strID, out nID))
                    modelData.ID = nID;
                else
                    throw new ApplicationException("ID가 잘못되었습니다.");

                string strModelID = FindElementValue(xe, XmlKey.XName_ModelID);
                int nModelID;
                if (int.TryParse(strModelID, out nModelID))
                    modelData.ModelID = nModelID;
                else
                    throw new ApplicationException("ModelID가 잘못되었습니다.");

                modelData.ModelDisplayText = FindElementValue(xe, XmlKey.XName_ModelDisplayText);
                modelData.ModelFile = FindElementValue(xe, XmlKey.XName_ModelFile);

                string strBuildingGroupID = FindElementValue(xe, XmlKey.XName_BuildingGroupID);
                int nBuildingGroupID;
                if (int.TryParse(strBuildingGroupID, out nBuildingGroupID))
                    modelData.BuildingGroupID = nBuildingGroupID;

                string strBuildingID = FindElementValue(xe, XmlKey.XName_BuildingID);
                int nBuildingID;
                if (int.TryParse(strBuildingID, out nBuildingID))
                    modelData.BuildingID = nBuildingID;

                string strZoneID = FindElementValue(xe, XmlKey.XName_ZoneID);
                int nZoneID;
                if (int.TryParse(strZoneID, out nZoneID))
                    modelData.ZoneID = nZoneID;

                string strFloorIndex = FindElementValue(xe, XmlKey.XName_FloorIndex);
                float nFloorIndex;
                if (float.TryParse(strFloorIndex, out nFloorIndex))
                    modelData.FloorIndex = nFloorIndex;

                string strCameraFar = FindElementValue(xe, XmlKey.XName_CameraFar);
                float nCameraFar;
                if (float.TryParse(strCameraFar, out nCameraFar))
                    modelData.CameraFar = nCameraFar;
                else
                    throw new ApplicationException("CameraFar이 잘못되었습니다.");

                string strCameraFov = FindElementValue(xe, XmlKey.XName_CameraFov);                        
                int nCameraFov;
                if (int.TryParse(strCameraFov, out nCameraFov))
                    modelData.CameraFov = nCameraFov;
                else
                    throw new ApplicationException("CameraFov이 잘못되었습니다.");

                string strCameraNear = FindElementValue(xe, XmlKey.XName_CameraNear);
                float nCameraNear;
                if (float.TryParse(strCameraNear, out nCameraNear))
                    modelData.CameraNear = nCameraNear;
                else
                    throw new ApplicationException("nCameraNear이 잘못되었습니다.");

                string strCameraPosition = FindElementValue(xe, XmlKey.XName_CameraPosition);                        
                string[] strPositions = strCameraPosition.Split(',');
                if (strPositions != null && strPositions.Length == 3)
                {
                    float nCameraPositionX;
                    if (float.TryParse(strPositions[0], out nCameraPositionX))
                        modelData.CameraPositionX = nCameraPositionX;
                    else
                        throw new ApplicationException("CameraPositionX가 잘못되었습니다.");
                    float nCameraPositionY;
                    if (float.TryParse(strPositions[1], out nCameraPositionY))
                        modelData.CameraPositionY = nCameraPositionY;
                    else
                        throw new ApplicationException("CameraPositionY가 잘못되었습니다.");
                    float nCameraPositionZ;
                    if (float.TryParse(strPositions[2], out nCameraPositionZ))
                        modelData.CameraPositionZ = nCameraPositionZ;
                    else
                        throw new ApplicationException("CameraPositionZ가 잘못되었습니다.");
                }
                else
                    throw new ApplicationException("CameraPosition이 잘못되었습니다.");

                string strCameraQuaternion = FindElementValue(xe, XmlKey.XName_CameraQuaternion);
                string[] strQuaternions = strCameraQuaternion.Split(',');
                if (strQuaternions != null && strQuaternions.Length == 4)
                {
                    float nCameraQuaternionW;
                    if (float.TryParse(strQuaternions[0], out nCameraQuaternionW))
                        modelData.CameraQuaternionW = nCameraQuaternionW;
                    else
                        throw new ApplicationException("CameraQuaternionW가 잘못되었습니다.");
                    float nCameraQuaternionX;
                    if (float.TryParse(strQuaternions[1], out nCameraQuaternionX))
                        modelData.CameraQuaternionX = nCameraQuaternionX;
                    else
                        throw new ApplicationException("CameraQuaternionX가 잘못되었습니다.");
                    float nCameraQuaternionY;
                    if (float.TryParse(strQuaternions[2], out nCameraQuaternionY))
                        modelData.CameraQuaternionY = nCameraQuaternionY;
                    else
                        throw new ApplicationException("CameraPositionY가 잘못되었습니다.");
                    float nCameraQuaternionZ;
                    if (float.TryParse(strQuaternions[3], out nCameraQuaternionZ))
                        modelData.CameraQuaternionZ = nCameraQuaternionZ;
                    else
                        throw new ApplicationException("CameraPositionZ가 잘못되었습니다.");
                }
                else
                    throw new ApplicationException("CameraQuaternion가 잘못되었습니다.");

                string strCameraRotation = FindElementValue(xe, XmlKey.XName_CameraRotation);
                string[] strRotations = strCameraRotation.Split(',');
                if (strRotations != null && strRotations.Length == 3)
                {
                    float nCameraRotationX;
                    if (float.TryParse(strRotations[0], out nCameraRotationX))
                        modelData.CameraRotationX = nCameraRotationX;
                    else
                        throw new ApplicationException("CameraRotationX가 잘못되었습니다.");
                    float nCameraRotationY;
                    if (float.TryParse(strRotations[1], out nCameraRotationY))
                        modelData.CameraRotationY = nCameraRotationY;
                    else
                        throw new ApplicationException("CameraRotationY가 잘못되었습니다.");
                    float nCameraRotationZ;
                    if (float.TryParse(strRotations[2], out nCameraRotationZ))
                        modelData.CameraRotationZ = nCameraRotationZ;
                    else
                        throw new ApplicationException("CameraRotationZ가 잘못되었습니다.");
                }
                else
                    throw new ApplicationException("CameraRotation이 잘못되었습니다.");

                string strOrbitTarget = FindElementValue(xe, XmlKey.XName_OrbitTarget);
                string[] strOrbitTargets = strOrbitTarget.Split(',');
                if (strOrbitTargets != null && strOrbitTargets.Length == 3)
                {
                    float nOrbitTargetX;
                    if (float.TryParse(strOrbitTargets[0], out nOrbitTargetX))
                        modelData.OrbitTargetX = nOrbitTargetX;
                    else
                        throw new ApplicationException("OrbitTargetX가 잘못되었습니다.");
                    float nOrbitTargetY;
                    if (float.TryParse(strOrbitTargets[1], out nOrbitTargetY))
                        modelData.OrbitTargetY = nOrbitTargetY;
                    else
                        throw new ApplicationException("OrbitTargetY가 잘못되었습니다.");
                    float nOrbitTargetZ;
                    if (float.TryParse(strOrbitTargets[2], out nOrbitTargetZ))
                        modelData.OrbitTargetZ = nOrbitTargetZ;
                    else
                        throw new ApplicationException("OrbitTargetZ가 잘못되었습니다.");
                }
                else
                    throw new ApplicationException("OrbitTarget이 잘못되었습니다.");

                return modelData;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadModelData : " + ex.Message;
                return null;
            }
        }

        private SDMS.Model.GLTF.ModelData ReadModelData(XmlReader reader, out string strErrorMessage)
        {
            try
            {
                SDMS.Model.GLTF.ModelData modelData = new SDMS.Model.GLTF.ModelData();

                strErrorMessage = null;

                while (reader.Read())
                {
                    bool isStartElement = reader.IsStartElement();
                    if (!isStartElement)
                        break;

                    string readerName = reader.Name.ToString();
                    switch (readerName)
                    {
                        case XmlKey.XName_ID:
                            int nID;
                            if (int.TryParse(reader.ReadInnerXml(), out nID))
                                modelData.ID = nID;
                            else
                                throw new ApplicationException("ID가 잘못되었습니다.");
                            break;
                        case XmlKey.XName_ModelID:
                            int nModelID;
                            if (int.TryParse(reader.ReadInnerXml(), out nModelID))
                                modelData.ModelID = nModelID;
                            else
                                throw new ApplicationException("ModelID가 잘못되었습니다.");
                            break;
                        case XmlKey.XName_ModelDisplayText:
                            modelData.ModelDisplayText = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_ModelFile:
                            modelData.ModelFile = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_BuildingGroupID:
                            int nBuildingGroupID;
                            if (int.TryParse(reader.ReadInnerXml(), out nBuildingGroupID))
                                modelData.BuildingGroupID = nBuildingGroupID;                                
                            break;
                        case XmlKey.XName_BuildingID:
                            int nBuildingID;
                            if (int.TryParse(reader.ReadInnerXml(), out nBuildingID))
                                modelData.BuildingID = nBuildingID;
                            break;
                        case XmlKey.XName_ZoneID:
                            int nZoneID;
                            if (int.TryParse(reader.ReadInnerXml(), out nZoneID))
                                modelData.ZoneID = nZoneID;
                            break;
                        case XmlKey.XName_FloorIndex:
                            float nFloorIndex;
                            if (float.TryParse(reader.ReadInnerXml(), out nFloorIndex))
                                modelData.FloorIndex = nFloorIndex;
                            break;
                        case XmlKey.XName_CameraFar:
                            float nCameraFar;
                            if (float.TryParse(reader.ReadInnerXml(), out nCameraFar))
                                modelData.CameraFar = nCameraFar;
                            else
                                throw new ApplicationException("CameraFar이 잘못되었습니다.");
                            break;
                        case XmlKey.XName_CameraFov:
                            int nCameraFov;
                            if (int.TryParse(reader.ReadInnerXml(), out nCameraFov))
                                modelData.CameraFov = nCameraFov;
                            else
                                throw new ApplicationException("CameraFov이 잘못되었습니다.");
                            break;
                        case XmlKey.XName_CameraNear:
                            float nCameraNear;
                            if (float.TryParse(reader.ReadInnerXml(), out nCameraNear))
                                modelData.CameraNear = nCameraNear;
                            else
                                throw new ApplicationException("nCameraNear이 잘못되었습니다.");
                            break;
                        case XmlKey.XName_CameraPosition:
                            string[] strPositions = reader.ReadInnerXml().Split(',');
                            if (strPositions != null && strPositions.Length == 3)
                            {
                                float nCameraPositionX;
                                if (float.TryParse(strPositions[0], out nCameraPositionX))
                                    modelData.CameraPositionX = nCameraPositionX;
                                else
                                    throw new ApplicationException("CameraPositionX가 잘못되었습니다.");
                                float nCameraPositionY;
                                if (float.TryParse(strPositions[1], out nCameraPositionY))
                                    modelData.CameraPositionY = nCameraPositionY;
                                else
                                    throw new ApplicationException("CameraPositionY가 잘못되었습니다.");
                                float nCameraPositionZ;
                                if (float.TryParse(strPositions[2], out nCameraPositionZ))
                                    modelData.CameraPositionZ = nCameraPositionZ;
                                else
                                    throw new ApplicationException("CameraPositionZ가 잘못되었습니다.");
                            }
                            else
                                throw new ApplicationException("CameraPosition이 잘못되었습니다.");
                            break;
                        case XmlKey.XName_CameraQuaternion:
                            string[] strQuaternions = reader.ReadInnerXml().Split(',');
                            if (strQuaternions != null && strQuaternions.Length == 4)
                            {
                                float nCameraQuaternionW;
                                if (float.TryParse(strQuaternions[0], out nCameraQuaternionW))
                                    modelData.CameraQuaternionW = nCameraQuaternionW;
                                else
                                    throw new ApplicationException("CameraQuaternionW가 잘못되었습니다.");
                                float nCameraQuaternionX;
                                if (float.TryParse(strQuaternions[1], out nCameraQuaternionX))
                                    modelData.CameraQuaternionX = nCameraQuaternionX;
                                else
                                    throw new ApplicationException("CameraQuaternionX가 잘못되었습니다.");
                                float nCameraQuaternionY;
                                if (float.TryParse(strQuaternions[2], out nCameraQuaternionY))
                                    modelData.CameraQuaternionY = nCameraQuaternionY;
                                else
                                    throw new ApplicationException("CameraPositionY가 잘못되었습니다.");
                                float nCameraQuaternionZ;
                                if (float.TryParse(strQuaternions[3], out nCameraQuaternionZ))
                                    modelData.CameraQuaternionZ = nCameraQuaternionZ;
                                else
                                    throw new ApplicationException("CameraPositionZ가 잘못되었습니다.");
                            }
                            else
                                throw new ApplicationException("CameraQuaternion가 잘못되었습니다.");
                            break;
                        case XmlKey.XName_CameraRotation:
                            string[] strRotations = reader.ReadInnerXml().Split(',');
                            if (strRotations != null && strRotations.Length == 3)
                            {
                                float nCameraRotationX;
                                if (float.TryParse(strRotations[0], out nCameraRotationX))
                                    modelData.CameraRotationX = nCameraRotationX;
                                else
                                    throw new ApplicationException("CameraRotationX가 잘못되었습니다.");
                                float nCameraRotationY;
                                if (float.TryParse(strRotations[1], out nCameraRotationY))
                                    modelData.CameraRotationY = nCameraRotationY;
                                else
                                    throw new ApplicationException("CameraRotationY가 잘못되었습니다.");
                                float nCameraRotationZ;
                                if (float.TryParse(strRotations[2], out nCameraRotationZ))
                                    modelData.CameraRotationZ = nCameraRotationZ;
                                else
                                    throw new ApplicationException("CameraRotationZ가 잘못되었습니다.");
                            }
                            else
                                throw new ApplicationException("CameraRotation이 잘못되었습니다.");
                            break;
                        case XmlKey.XName_OrbitTarget:
                            string[] strOrbitTargets = reader.ReadInnerXml().Split(',');
                            if (strOrbitTargets != null && strOrbitTargets.Length == 3)
                            {
                                float nOrbitTargetX;
                                if (float.TryParse(strOrbitTargets[0], out nOrbitTargetX))
                                    modelData.OrbitTargetX = nOrbitTargetX;
                                else
                                    throw new ApplicationException("OrbitTargetX가 잘못되었습니다.");
                                float nOrbitTargetY;
                                if (float.TryParse(strOrbitTargets[1], out nOrbitTargetY))
                                    modelData.OrbitTargetY = nOrbitTargetY;
                                else
                                    throw new ApplicationException("OrbitTargetY가 잘못되었습니다.");
                                float nOrbitTargetZ;
                                if (float.TryParse(strOrbitTargets[2], out nOrbitTargetZ))
                                    modelData.OrbitTargetZ = nOrbitTargetZ;
                                else
                                    throw new ApplicationException("OrbitTargetZ가 잘못되었습니다.");
                            }
                            else
                                throw new ApplicationException("OrbitTarget이 잘못되었습니다.");
                            break;
                    }
                }

                return modelData;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadModelData : " + ex.Message;
                return null;
            }
        }

        private List<SDMS.Model.GLTF.ModelOrthoData> ReadModelOrthoDatas(XElement xe, out string strErrorMessage)
        {
            List<SDMS.Model.GLTF.ModelOrthoData> modelOrthoDatas = new List<SDMS.Model.GLTF.ModelOrthoData>();
            strErrorMessage = null;

            try
            {
                XElement xe2 = FindElement(xe, XmlKey.XName_ModelOrthoDatas);
                List<XElement> xModelOrthoDatas = FindElements(xe2, XmlKey.XName_ModelOrthoData);
                foreach (XElement xModelOrthoData in xModelOrthoDatas)
                {
                    SDMS.Model.GLTF.ModelOrthoData modelOrthoData = ReadModelOrthoData(xModelOrthoData, out strErrorMessage);
                    if (modelOrthoData != null)
                        modelOrthoDatas.Add(modelOrthoData);
                }

                return modelOrthoDatas;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadModelOrthoDatas : " + ex.Message;
                return null;
            }
        }

        private SDMS.Model.GLTF.ModelOrthoData ReadModelOrthoData(XElement xe, out string strErrorMessage)
        {
            SDMS.Model.GLTF.ModelOrthoData modelOrthoData = new SDMS.Model.GLTF.ModelOrthoData();
            strErrorMessage = null;

            try
            {
                string strID = FindElementValue(xe, XmlKey.XName_ID);
                int nID;
                if (int.TryParse(strID, out nID))
                    modelOrthoData.ID = nID;
                else
                    throw new ApplicationException("ID가 잘못되었습니다.");

                string strModelID = FindElementValue(xe, XmlKey.XName_ModelID);
                int nModelID;
                if (int.TryParse(strModelID, out nModelID))
                    modelOrthoData.ModelID = nModelID;
                else
                    throw new ApplicationException("ModelID가 잘못되었습니다.");

                modelOrthoData.ModelFile = FindElementValue(xe, XmlKey.XName_ModelFile);
                string strZoneID = FindElementValue(xe, XmlKey.XName_ZoneID);
                int nZoneID;
                if (int.TryParse(strZoneID, out nZoneID))
                    modelOrthoData.ZoneID = nZoneID;

                string strCameraPosition = FindElementValue(xe, XmlKey.XName_CameraPosition);
                string[] strPositions = strCameraPosition.Split(',');
                if (strPositions != null && strPositions.Length == 3)
                {
                    float nCameraPositionX;
                    if (float.TryParse(strPositions[0], out nCameraPositionX))
                        modelOrthoData.CameraPositionX = nCameraPositionX;
                    else
                        throw new ApplicationException("CameraPositionX가 잘못되었습니다.");
                    float nCameraPositionY;
                    if (float.TryParse(strPositions[1], out nCameraPositionY))
                        modelOrthoData.CameraPositionY = nCameraPositionY;
                    else
                        throw new ApplicationException("CameraPositionY가 잘못되었습니다.");
                    float nCameraPositionZ;
                    if (float.TryParse(strPositions[2], out nCameraPositionZ))
                        modelOrthoData.CameraPositionZ = nCameraPositionZ;
                    else
                        throw new ApplicationException("CameraPositionZ가 잘못되었습니다.");
                }
                else
                    throw new ApplicationException("CameraPosition이 잘못되었습니다.");
                string strCameraQuaternion = FindElementValue(xe, XmlKey.XName_CameraQuaternion);
                string[] strQuaternions = strCameraQuaternion.Split(',');
                if (strQuaternions != null && strQuaternions.Length == 4)
                {
                    float nCameraQuaternionW;
                    if (float.TryParse(strQuaternions[0], out nCameraQuaternionW))
                        modelOrthoData.CameraQuaternionW = nCameraQuaternionW;
                    else
                        throw new ApplicationException("CameraQuaternionW가 잘못되었습니다.");
                    float nCameraQuaternionX;
                    if (float.TryParse(strQuaternions[1], out nCameraQuaternionX))
                        modelOrthoData.CameraQuaternionX = nCameraQuaternionX;
                    else
                        throw new ApplicationException("CameraQuaternionX가 잘못되었습니다.");
                    float nCameraQuaternionY;
                    if (float.TryParse(strQuaternions[2], out nCameraQuaternionY))
                        modelOrthoData.CameraQuaternionY = nCameraQuaternionY;
                    else
                        throw new ApplicationException("CameraPositionY가 잘못되었습니다.");
                    float nCameraQuaternionZ;
                    if (float.TryParse(strQuaternions[3], out nCameraQuaternionZ))
                        modelOrthoData.CameraQuaternionZ = nCameraQuaternionZ;
                    else
                        throw new ApplicationException("CameraPositionZ가 잘못되었습니다.");
                }
                else
                    throw new ApplicationException("CameraQuaternion가 잘못되었습니다.");

                string strCameraRotation = FindElementValue(xe, XmlKey.XName_CameraRotation);
                string[] strRotations = strCameraRotation.Split(',');
                if (strRotations != null && strRotations.Length == 3)
                {
                    float nCameraRotationX;
                    if (float.TryParse(strRotations[0], out nCameraRotationX))
                        modelOrthoData.CameraRotationX = nCameraRotationX;
                    else
                        throw new ApplicationException("CameraRotationX가 잘못되었습니다.");
                    float nCameraRotationY;
                    if (float.TryParse(strRotations[1], out nCameraRotationY))
                        modelOrthoData.CameraRotationY = nCameraRotationY;
                    else
                        throw new ApplicationException("CameraRotationY가 잘못되었습니다.");
                    float nCameraRotationZ;
                    if (float.TryParse(strRotations[2], out nCameraRotationZ))
                        modelOrthoData.CameraRotationZ = nCameraRotationZ;
                    else
                        throw new ApplicationException("CameraRotationZ가 잘못되었습니다.");
                }
                else
                    throw new ApplicationException("CameraRotation이 잘못되었습니다.");

                string strTarget = FindElementValue(xe, XmlKey.XName_Target);                        
                string[] strOrbitTargets = strTarget.Split(',');
                if (strOrbitTargets != null && strOrbitTargets.Length == 3)
                {
                    float nOrbitTargetX;
                    if (float.TryParse(strOrbitTargets[0], out nOrbitTargetX))
                        modelOrthoData.TargetX = nOrbitTargetX;
                    else
                        throw new ApplicationException("TargetX가 잘못되었습니다.");
                    float nOrbitTargetY;
                    if (float.TryParse(strOrbitTargets[1], out nOrbitTargetY))
                        modelOrthoData.TargetY = nOrbitTargetY;
                    else
                        throw new ApplicationException("TargetY가 잘못되었습니다.");
                    float nOrbitTargetZ;
                    if (float.TryParse(strOrbitTargets[2], out nOrbitTargetZ))
                        modelOrthoData.TargetZ = nOrbitTargetZ;
                    else
                        throw new ApplicationException("TargetZ가 잘못되었습니다.");
                }
                else
                    throw new ApplicationException("Target이 잘못되었습니다.");

                string strZoom = FindElementValue(xe, XmlKey.XName_Zoom);                        
                        
                float nZoom;
                if (float.TryParse(strZoom, out nZoom))
                    modelOrthoData.Zoom = nZoom;
                else
                    throw new ApplicationException("TargetX가 잘못되었습니다.");
                    
                return modelOrthoData;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadModelOrthoData : " + ex.Message;
                return null;
            }
        }

        private List<SDMS.Model.GLTF.ModelOrthoData> ReadModelOrthoDatas(XmlReader reader, out string strErrorMessage)
        {
            try
            {
                List<SDMS.Model.GLTF.ModelOrthoData> modelOrthoDatas = new List<SDMS.Model.GLTF.ModelOrthoData>();

                strErrorMessage = null;

                while (reader.Read())
                {
                    bool isStartElement = reader.IsStartElement();
                    if (!isStartElement)
                        break;

                    string readerName = reader.Name.ToString();
                    switch (readerName)
                    {
                        case XmlKey.XName_ModelOrthoData:
                            SDMS.Model.GLTF.ModelOrthoData modelOrthoData = ReadModelOrthoData(reader, out strErrorMessage);
                            if (modelOrthoData != null)
                                modelOrthoDatas.Add(modelOrthoData);
                            break;
                    }
                }

                return modelOrthoDatas;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadModelOrthoDatas : " + ex.Message;
                return null;
            }
        }

        private SDMS.Model.GLTF.ModelOrthoData ReadModelOrthoData(XmlReader reader, out string strErrorMessage)
        {
            try
            {
                SDMS.Model.GLTF.ModelOrthoData modelOrthoData = new SDMS.Model.GLTF.ModelOrthoData();

                strErrorMessage = null;

                while (reader.Read())
                {
                    bool isStartElement = reader.IsStartElement();
                    if (!isStartElement)
                        break;

                    string readerName = reader.Name.ToString();
                    switch (readerName)
                    {
                        case XmlKey.XName_ID:
                            int nID;
                            if (int.TryParse(reader.ReadInnerXml(), out nID))
                                modelOrthoData.ID = nID;
                            else
                                throw new ApplicationException("ID가 잘못되었습니다.");
                            break;
                        case XmlKey.XName_ModelID:
                            int nModelID;
                            if (int.TryParse(reader.ReadInnerXml(), out nModelID))
                                modelOrthoData.ModelID = nModelID;
                            else
                                throw new ApplicationException("ModelID가 잘못되었습니다.");
                            break;
                        case XmlKey.XName_ModelFile:
                            modelOrthoData.ModelFile = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_ZoneID:
                            int nZoneID;
                            if (int.TryParse(reader.ReadInnerXml(), out nZoneID))
                                modelOrthoData.ZoneID = nZoneID;
                            break;
                        
                        case XmlKey.XName_CameraPosition:
                            string[] strPositions = reader.ReadInnerXml().Split(',');
                            if (strPositions != null && strPositions.Length == 3)
                            {
                                float nCameraPositionX;
                                if (float.TryParse(strPositions[0], out nCameraPositionX))
                                    modelOrthoData.CameraPositionX = nCameraPositionX;
                                else
                                    throw new ApplicationException("CameraPositionX가 잘못되었습니다.");
                                float nCameraPositionY;
                                if (float.TryParse(strPositions[1], out nCameraPositionY))
                                    modelOrthoData.CameraPositionY = nCameraPositionY;
                                else
                                    throw new ApplicationException("CameraPositionY가 잘못되었습니다.");
                                float nCameraPositionZ;
                                if (float.TryParse(strPositions[2], out nCameraPositionZ))
                                    modelOrthoData.CameraPositionZ = nCameraPositionZ;
                                else
                                    throw new ApplicationException("CameraPositionZ가 잘못되었습니다.");
                            }
                            else
                                throw new ApplicationException("CameraPosition이 잘못되었습니다.");
                            break;
                        case XmlKey.XName_CameraQuaternion:
                            string[] strQuaternions = reader.ReadInnerXml().Split(',');
                            if (strQuaternions != null && strQuaternions.Length == 4)
                            {
                                float nCameraQuaternionW;
                                if (float.TryParse(strQuaternions[0], out nCameraQuaternionW))
                                    modelOrthoData.CameraQuaternionW = nCameraQuaternionW;
                                else
                                    throw new ApplicationException("CameraQuaternionW가 잘못되었습니다.");
                                float nCameraQuaternionX;
                                if (float.TryParse(strQuaternions[1], out nCameraQuaternionX))
                                    modelOrthoData.CameraQuaternionX = nCameraQuaternionX;
                                else
                                    throw new ApplicationException("CameraQuaternionX가 잘못되었습니다.");
                                float nCameraQuaternionY;
                                if (float.TryParse(strQuaternions[2], out nCameraQuaternionY))
                                    modelOrthoData.CameraQuaternionY = nCameraQuaternionY;
                                else
                                    throw new ApplicationException("CameraPositionY가 잘못되었습니다.");
                                float nCameraQuaternionZ;
                                if (float.TryParse(strQuaternions[3], out nCameraQuaternionZ))
                                    modelOrthoData.CameraQuaternionZ = nCameraQuaternionZ;
                                else
                                    throw new ApplicationException("CameraPositionZ가 잘못되었습니다.");
                            }
                            else
                                throw new ApplicationException("CameraQuaternion가 잘못되었습니다.");
                            break;
                        case XmlKey.XName_CameraRotation:
                            string[] strRotations = reader.ReadInnerXml().Split(',');
                            if (strRotations != null && strRotations.Length == 3)
                            {
                                float nCameraRotationX;
                                if (float.TryParse(strRotations[0], out nCameraRotationX))
                                    modelOrthoData.CameraRotationX = nCameraRotationX;
                                else
                                    throw new ApplicationException("CameraRotationX가 잘못되었습니다.");
                                float nCameraRotationY;
                                if (float.TryParse(strRotations[1], out nCameraRotationY))
                                    modelOrthoData.CameraRotationY = nCameraRotationY;
                                else
                                    throw new ApplicationException("CameraRotationY가 잘못되었습니다.");
                                float nCameraRotationZ;
                                if (float.TryParse(strRotations[2], out nCameraRotationZ))
                                    modelOrthoData.CameraRotationZ = nCameraRotationZ;
                                else
                                    throw new ApplicationException("CameraRotationZ가 잘못되었습니다.");
                            }
                            else
                                throw new ApplicationException("CameraRotation이 잘못되었습니다.");
                            break;
                        case XmlKey.XName_Target:
                            string[] strOrbitTargets = reader.ReadInnerXml().Split(',');
                            if (strOrbitTargets != null && strOrbitTargets.Length == 3)
                            {
                                float nOrbitTargetX;
                                if (float.TryParse(strOrbitTargets[0], out nOrbitTargetX))
                                    modelOrthoData.TargetX = nOrbitTargetX;
                                else
                                    throw new ApplicationException("TargetX가 잘못되었습니다.");
                                float nOrbitTargetY;
                                if (float.TryParse(strOrbitTargets[1], out nOrbitTargetY))
                                    modelOrthoData.TargetY = nOrbitTargetY;
                                else
                                    throw new ApplicationException("TargetY가 잘못되었습니다.");
                                float nOrbitTargetZ;
                                if (float.TryParse(strOrbitTargets[2], out nOrbitTargetZ))
                                    modelOrthoData.TargetZ = nOrbitTargetZ;
                                else
                                    throw new ApplicationException("TargetZ가 잘못되었습니다.");
                            }
                            else
                                throw new ApplicationException("Target이 잘못되었습니다.");
                            break;

                        case XmlKey.XName_Zoom:
                            float nZoom;
                            if (float.TryParse(reader.ReadInnerXml(), out nZoom))
                                modelOrthoData.Zoom = nZoom;
                            else
                                throw new ApplicationException("TargetX가 잘못되었습니다.");
                            break;
                    }
                }

                return modelOrthoData;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadModelOrthoData : " + ex.Message;
                return null;
            }
        }

        private bool ReadGltfOption(XElement xe, out string strErrorMessage)
        {
            try
            {
                strErrorMessage = null;

                m_gltfOption._3DModelBaseURL = FindElementValue(xe, XmlKey.XName_ModelBaseURL);
                m_gltfOption._3DTextureBaseURL = FindElementValue(xe, XmlKey.XName_TextureBaseURL);
                m_gltfOption._3DBackgroundImage = FindElementValue(xe, XmlKey.XName_BackgroundImage);
                string strIndoorModelOnMemory = FindElementValue(xe, XmlKey.XName_IndoorModelOnMemory);
                if (strIndoorModelOnMemory == "true")
                    m_gltfOption.IndoorModelOnMemory = true;
                else
                    m_gltfOption.IndoorModelOnMemory = false;

                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadGltfOption : " + ex.Message;
                return false;
            }
        }

        private bool ReadGltfOption(XmlReader reader, out string strErrorMessage)
        {
            try
            {
                strErrorMessage = null;

                while (reader.Read())
                {
                    bool isStartElement = reader.IsStartElement();
                    if (!isStartElement)
                        break;

                    string readerName = reader.Name.ToString();
                    switch (readerName)
                    {
                        case XmlKey.XName_ModelBaseURL:
                            m_gltfOption._3DModelBaseURL = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_TextureBaseURL:
                            m_gltfOption._3DTextureBaseURL = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_IndoorModelOnMemory:
                            string value = reader.ReadInnerXml();
                            if (value.ToLower() == "true")
                                m_gltfOption.IndoorModelOnMemory = true;
                            else
                                m_gltfOption.IndoorModelOnMemory = false;
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadGltfOption : " + ex.Message;
                return false;
            }
        }

        private bool ReadBuildingGroup(XElement xe, out string strErrorMessage)
        {
            strErrorMessage = null;

            try
            {                
                string strIDValues = xe.Attribute("id").Value;
                int? nID = GetID(strIDValues, XmlKey.KeyValue.BuildingGroup);
                if (nID == null)
                    throw new ApplicationException("ID 구하기 실패");

                string strName = FindElementValue(xe, XmlKey.XName_Name);
                string strDisplayText = FindElementValue(xe, XmlKey.XName_DisplayText);

                UnE.Geometry.Vertex3D vTextCenter = null;
                string strPoint3Ds = FindElementValue(xe, XmlKey.XName_Point3D);
                string[] strPoint3D = strPoint3Ds.Split(',');
                if (strPoint3D != null && strPoint3D.Length == 3)
                    vTextCenter = new UnE.Geometry.Vertex3D(Convert.ToDouble(strPoint3D[0]), Convert.ToDouble(strPoint3D[1]), Convert.ToDouble(strPoint3D[2]));

                string strParentID = FindElementValue(xe, XmlKey.XName_ParentID);
                string strSiteID = FindElementValue(xe, XmlKey.XName_SiteID);
                string strVisible = FindElementValue(xe, XmlKey.XName_Visible);

                BuildingGroupData bgData = new BuildingGroupData();
                bgData.ID = (int)nID;
                bgData.GroupName = strName;
                bgData.DisplayText = strDisplayText;
                bgData.TextCenter = vTextCenter;
                int nParentID;
                if (int.TryParse(strParentID, out nParentID))
                    bgData.ParentID = nParentID;
                int nSiteID;
                if (int.TryParse(strSiteID, out nSiteID))
                    bgData.SiteID = nSiteID;

                if (bgData.ParentID != null)
                    dicBuildingGroupParents[bgData.ID] = (int)bgData.ParentID;

                bool bVisible;
                if (bool.TryParse(strVisible, out bVisible))
                    bgData.Visible = bVisible;

                m_dicBuildingGroups[bgData.ID] = bgData;
                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadBuildingGroup : " + ex.Message;
                return false;
            }
        }

        private bool ReadBuilding(XElement xe, out string strErrorMessage)
        {
            strErrorMessage = null;

            try
            {
                string strIDValues = xe.Attribute("id").Value;
                int? nID = GetID(strIDValues, XmlKey.KeyValue.Building);
                if (nID == null)
                    throw new ApplicationException("ID 구하기 실패");

                string strCode = FindElementValue(xe, XmlKey.XName_Code);
                string strName = FindElementValue(xe, XmlKey.XName_Name);
                string strBuildingGroupID = FindElementValue(xe, XmlKey.XName_BuildingGroupID);
                string strMaxFloor = FindElementValue(xe, XmlKey.XName_MaxFloor);
                string strMinFloor = FindElementValue(xe, XmlKey.XName_MinFloor);
                UnE.Geometry.Vertex3D vTextCenter = null;
                string strPoint3Ds = FindElementValue(xe, XmlKey.XName_Point3D);
                string[] strPoint3D = strPoint3Ds.Split(',');
                if (strPoint3D != null && strPoint3D.Length == 3)
                    vTextCenter = new UnE.Geometry.Vertex3D(Convert.ToDouble(strPoint3D[0]), Convert.ToDouble(strPoint3D[1]), Convert.ToDouble(strPoint3D[2]));

                string strDisplayText = FindElementValue(xe, XmlKey.XName_DisplayText);
                string strBroadcastText = FindElementValue(xe, XmlKey.XName_BroadcastText);
                string strModelFile = FindElementValue(xe, XmlKey.XName_ModelFile);

                BuildingData bData = new BuildingData();
                bData.ID = (int)nID;
                bData.BuildingCode = strCode;
                bData.BuildingName = strName;
                int? nBuildingGroupID = GetID(strBuildingGroupID, XmlKey.KeyValue.BuildingGroup);
                bData.BuildingGroupID = nBuildingGroupID == null ? -1 : (int)nBuildingGroupID;
                int nMaxFloor;
                if (int.TryParse(strMaxFloor, out nMaxFloor))
                    bData.MaxFloor = nMaxFloor;
                int nMinFloor;
                if (int.TryParse(strMinFloor, out nMinFloor))
                    bData.MinFloor = nMinFloor;
                bData.TextCenter = vTextCenter;
                bData.DisplayText = strDisplayText;
                bData.BroadcastText = strBroadcastText;

                BuildingGroupData bg;

                if (m_dicBuildingGroups.TryGetValue(bData.BuildingGroupID, out bg))
                {
                    bg.BuildingDatas.Add(bData);
                }

                m_dicBuildings[bData.ID] = bData;

                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadBuilding : " + ex.Message;
                return false;
            }
        }

        private bool ReadZone(XElement xe, out string strErrorMessage)
        {
            try
            {
                strErrorMessage = null;

                string strIDValues = xe.Attribute("id").Value;
                int? nID = GetID(strIDValues, XmlKey.KeyValue.Zone);
                if (nID == null)
                    throw new ApplicationException("ID 구하기 실패");

                string strName = FindElementValue(xe, XmlKey.XName_Name);
                string strBuildingID = FindElementValue(xe, XmlKey.XName_BuildingID);
                string strFloorIndex = FindElementValue(xe, XmlKey.XName_FloorIndex);

                List<XElement> xe2Ds = FindElements(xe, XmlKey.XName_Point2D);
                string strBoundary = "";
                foreach (XElement xe2D in xe2Ds)
                {
                    string strPoint2Ds = FindElementValue(xe2D, XmlKey.XName_Point2D);
                    string[] strPoint2D = strPoint2Ds.Split(',');
                    if (strPoint2D != null && strPoint2D.Length == 2)
                    {
                        if (strBoundary.Length > 0)
                            strBoundary += strPoint2D[0] + "," + strPoint2D[1];
                    }
                }
                Polygon vBoundaries = StringToPolygon(strBoundary);

                UnE.Geometry.Vertex3D vTextCenter = null;
                string strPoint3Ds = FindElementValue(xe, XmlKey.XName_Point3D);
                string[] strPoint3D = strPoint3Ds.Split(',');
                if (strPoint3D != null && strPoint3D.Length == 3)
                    vTextCenter = new UnE.Geometry.Vertex3D(Convert.ToDouble(strPoint3D[0]), Convert.ToDouble(strPoint3D[1]), Convert.ToDouble(strPoint3D[2]));

                string strDisplayText = FindElementValue(xe, XmlKey.XName_DisplayText);
                string strBroadcastText = FindElementValue(xe, XmlKey.XName_BroadcastText);
                string strModelFile = FindElementValue(xe, XmlKey.XName_ModelFile);

                ZoneData zData = new ZoneData();
                zData.ID = (int)nID;
                zData.ZoneName = strName;
                int? nBuildingID = GetID(strBuildingID, XmlKey.KeyValue.Building);
                zData.BuildingID = nBuildingID;
                int nMaxFloor;
                if (int.TryParse(strFloorIndex, out nMaxFloor))
                    zData.FloorIndex = nMaxFloor;
                zData.TextCenter = vTextCenter;
                zData.Boundary = vBoundaries;
                zData.DisplayText = strDisplayText;
                zData.BroadcastText = strBroadcastText;

                BuildingData building;

                if (zData.BuildingID != null && m_dicBuildings.TryGetValue((int)zData.BuildingID, out building))
                {
                    building.ZoneDatas.Add(zData);
                }

                m_dicZones[zData.ID] = zData;

                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadZone : " + ex.Message;
                return false;
            }
        }

        private bool ReadEquipmentZone(XElement xe, out string strErrorMessage)
        {
            strErrorMessage = null;

            try
            {
                string strIDValues = xe.Attribute("id").Value;
                int? nID = GetID(strIDValues, XmlKey.KeyValue.EquipmentZone);
                if (nID == null)
                    throw new ApplicationException("ID 구하기 실패");

                string strName = FindElementValue(xe, XmlKey.XName_Name);
                List<int> linkedZoneIDList = new List<int>();
                XElement xeLinkedZoneIDList = FindElement(xe, XmlKey.XName_LinkedZoneIDList);
                List<XElement> xeZoneIDs = FindElements(xeLinkedZoneIDList, XmlKey.XName_ZoneID);
                foreach (XElement xeZoneID in xeZoneIDs)
                {
                    int? nZoneID = GetID(xeZoneID.Value, XmlKey.KeyValue.Zone);
                    if (nZoneID != null)
                        linkedZoneIDList.Add((int)nZoneID);
                }

                List<XElement> xe2Ds = FindElements(xe, XmlKey.XName_Point2D);
                string strBoundary = "";
                foreach (XElement xe2D in xe2Ds)
                {
                    string strPoint2Ds = FindElementValue(xe2D, XmlKey.XName_Point2D);
                    string[] strPoint2D = strPoint2Ds.Split(',');
                    if (strPoint2D != null && strPoint2D.Length == 2)
                    {
                        if (strBoundary.Length > 0)
                            strBoundary += strPoint2D[0] + "," + strPoint2D[1];
                    }
                }
                Polygon vBoundaries = StringToPolygon(strBoundary);

                UnE.Geometry.Vertex3D vTextCenter = null;
                string strPoint3Ds = FindElementValue(xe, XmlKey.XName_Point3D);
                string[] strPoint3D = strPoint3Ds.Split(',');
                if (strPoint3D != null && strPoint3D.Length == 3)
                    vTextCenter = new UnE.Geometry.Vertex3D(Convert.ToDouble(strPoint3D[0]), Convert.ToDouble(strPoint3D[1]), Convert.ToDouble(strPoint3D[2]));

                string strDisplayText = FindElementValue(xe, XmlKey.XName_DisplayText);
                string strBroadcastText = FindElementValue(xe, XmlKey.XName_BroadcastText);
                string strType = FindElementValue(xe, XmlKey.XName_Type);

                EquipmentZoneData ezData = new EquipmentZoneData();
                ezData.ID = (int)nID;
                ezData.ZoneName = strName;
                ezData.LinkedZoneIDs = linkedZoneIDList;
                ezData.Boundary = vBoundaries;
                ezData.TextCenter = vTextCenter;
                ezData.DisplayText = strDisplayText;
                ezData.BroadcastText = strBroadcastText;
                int nType;
                if (int.TryParse(strType, out nType))
                    ezData.Type = nType;

                ZoneData zone;
                foreach (int zoneID in ezData.LinkedZoneIDs)
                {
                    if (m_dicZones.TryGetValue(zoneID, out zone))
                    {
                        zone.EquipmentZoneDatas.Add(ezData);
                        ezData.LinkedZoneDatas.Add(zone);
                    }
                }

                m_dicEquipZones[ezData.ID] = ezData;

                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadEquipmentZone : " + ex.Message;
                return false;
            }
        }

        private bool ReadBuildingGroup(XmlReader reader, out string strErrorMessage)
        {
            try
            {
                strErrorMessage = null;

                string strIDValues = reader.GetAttribute("id");
                int? nID = GetID(strIDValues, XmlKey.KeyValue.BuildingGroup);
                if (nID == null)
                    throw new ApplicationException("ID 구하기 실패");

                string strName = "";
                string strDisplayText = "";
                UnE.Geometry.Vertex3D vTextCenter = null;
                string strParentID = "";
                string strSiteID = "";

                while (reader.Read())
                {
                    bool isStartElement = reader.IsStartElement();
                    if (!isStartElement)
                        break;

                    string readerName = reader.Name.ToString();
                    switch (readerName)
                    {
                        case XmlKey.XName_Name:
                            strName = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_DisplayText:
                            strDisplayText = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_TextCenter:
                            vTextCenter = ReadTextCenter(reader, out strErrorMessage);
                            break;
                        case XmlKey.XName_ParentID:
                            strParentID = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_SiteID:
                            strSiteID = reader.ReadInnerXml();
                            break;
                    }
                }

                BuildingGroupData bgData = new BuildingGroupData();
                bgData.ID = (int)nID;
                bgData.GroupName = strName;
                bgData.DisplayText = strDisplayText;
                bgData.TextCenter = vTextCenter;
                int nParentID;
                if (int.TryParse(strParentID, out nParentID))
                    bgData.ParentID = nParentID;
                int nSiteID;
                if (int.TryParse(strSiteID, out nSiteID))
                    bgData.SiteID = nSiteID;

                if (bgData.ParentID != null)
                    dicBuildingGroupParents[bgData.ID] = (int)bgData.ParentID;

                m_dicBuildingGroups[bgData.ID] = bgData;
                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadBuildingGroup : " + ex.Message;
                return false;
            }
        }

        private bool ReadBuilding(XmlReader reader, out string strErrorMessage)
        {
            try
            {
                strErrorMessage = null;

                string strIDValues = reader.GetAttribute("id");
                int? nID = GetID(strIDValues, XmlKey.KeyValue.Building);
                if (nID == null)
                    throw new ApplicationException("ID 구하기 실패");

                string strCode = "";
                string strName = "";
                string strBuildingGroupID = "";
                string strMaxFloor = "";
                string strMinFloor = "";
                UnE.Geometry.Vertex3D vTextCenter = null;
                string strDisplayText = "";
                string strBroadcastText = "";
                string strModelFile = "";

                while (reader.Read())
                {
                    bool isStartElement = reader.IsStartElement();
                    if (!isStartElement)
                        break;

                    string readerName = reader.Name.ToString();
                    switch (readerName)
                    {
                        case XmlKey.XName_Code:
                            strCode = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_Name:
                            strName = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_BuildingGroupID:
                            strBuildingGroupID = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_MaxFloor:
                            strMaxFloor = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_MinFloor:
                            strMinFloor = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_TextCenter:
                            vTextCenter = ReadTextCenter(reader, out strErrorMessage);
                            break;
                        case XmlKey.XName_DisplayText:
                            strDisplayText = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_BroadcastText:
                            strBroadcastText = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_ModelFile:
                            strModelFile = reader.ReadInnerXml();
                            break;
                    }
                }

                BuildingData bData = new BuildingData();
                bData.ID = (int)nID;
                bData.BuildingCode = strCode;
                bData.BuildingName = strName;
                int? nBuildingGroupID = GetID(strBuildingGroupID, XmlKey.KeyValue.BuildingGroup);
                bData.BuildingGroupID = nBuildingGroupID == null ? -1 : (int)nBuildingGroupID;
                int nMaxFloor;
                if (int.TryParse(strMaxFloor, out nMaxFloor))
                    bData.MaxFloor = nMaxFloor;
                int nMinFloor;
                if (int.TryParse(strMinFloor, out nMinFloor))
                    bData.MinFloor = nMinFloor;
                bData.TextCenter = vTextCenter;
                bData.DisplayText = strDisplayText;
                bData.BroadcastText = strBroadcastText;

                BuildingGroupData bg;

                if (m_dicBuildingGroups.TryGetValue(bData.BuildingGroupID, out bg))
                {
                    bg.BuildingDatas.Add(bData);
                }

                m_dicBuildings[bData.ID] = bData;

                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadBuilding : " + ex.Message;
                return false;
            }
        }

        private bool ReadZone(XmlReader reader, out string strErrorMessage)
        {
            try
            {
                strErrorMessage = null;

                string strIDValues = reader.GetAttribute("id");
                int? nID = GetID(strIDValues, XmlKey.KeyValue.Zone);
                if (nID == null)
                    throw new ApplicationException("ID 구하기 실패");

                string strName = "";
                string strBuildingID = "";
                string strFloorIndex = "";
                Polygon vBoundaries = null;
                UnE.Geometry.Vertex3D vTextCenter = null;
                string strDisplayText = "";
                string strBroadcastText = "";
                string strModelFile = "";

                while (reader.Read())
                {
                    bool isStartElement = reader.IsStartElement();
                    if (!isStartElement)
                        break;

                    string readerName = reader.Name.ToString();
                    switch (readerName)
                    {
                        case XmlKey.XName_Name:
                            strName = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_BuildingID:
                            strBuildingID = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_FloorIndex:
                            strFloorIndex = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_Boundary:
                            vBoundaries = ReadBoundary(reader, out strErrorMessage);
                            break;
                        case XmlKey.XName_TextCenter:
                            vTextCenter = ReadTextCenter(reader, out strErrorMessage);
                            break;
                        case XmlKey.XName_DisplayText:
                            strDisplayText = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_BroadcastText:
                            strBroadcastText = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_ModelFile:
                            strModelFile = reader.ReadInnerXml();
                            break;
                    }
                }

                ZoneData zData = new ZoneData();
                zData.ID = (int)nID;
                zData.ZoneName = strName;
                int? nBuildingID = GetID(strBuildingID, XmlKey.KeyValue.Building);
                zData.BuildingID = nBuildingID;
                int nMaxFloor;
                if (int.TryParse(strFloorIndex, out nMaxFloor))
                    zData.FloorIndex = nMaxFloor;
                zData.TextCenter = vTextCenter;
                zData.Boundary = vBoundaries;
                zData.DisplayText = strDisplayText;
                zData.BroadcastText = strBroadcastText;

                BuildingData building;

                if (zData.BuildingID != null && m_dicBuildings.TryGetValue((int)zData.BuildingID, out building))
                {
                    building.ZoneDatas.Add(zData);
                }

                m_dicZones[zData.ID] = zData;

                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadZone : " + ex.Message;
                return false;
            }
        }

        private bool ReadEquipmentZone(XmlReader reader, out string strErrorMessage)
        {
            try
            {
                strErrorMessage = null;

                string strIDValues = reader.GetAttribute("id");
                int? nID = GetID(strIDValues, XmlKey.KeyValue.EquipmentZone);
                if (nID == null)
                    throw new ApplicationException("ID 구하기 실패");

                string strName = "";
                List<int> linkedZoneIDList = new List<int>();
                Polygon vBoundaries = null;
                UnE.Geometry.Vertex3D vTextCenter = null;
                string strDisplayText = "";
                string strBroadcastText = "";
                string strType = "";

                while (reader.Read())
                {
                    bool isStartElement = reader.IsStartElement();
                    if (!isStartElement)
                        break;

                    string readerName = reader.Name.ToString();
                    switch (readerName)
                    {
                        case XmlKey.XName_Name:
                            strName = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_LinkedZoneIDList:
                            linkedZoneIDList = ReadLinkedZoneIDList(reader, out strErrorMessage);
                            break;
                        case XmlKey.XName_Boundary:
                            vBoundaries = ReadBoundary(reader, out strErrorMessage);
                            break;
                        case XmlKey.XName_TextCenter:
                            vTextCenter = ReadTextCenter(reader, out strErrorMessage);
                            break;
                        case XmlKey.XName_DisplayText:
                            strDisplayText = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_BroadcastText:
                            strBroadcastText = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_Type:
                            strType = reader.ReadInnerXml();
                            break;
                    }
                }

                EquipmentZoneData ezData = new EquipmentZoneData();
                ezData.ID = (int)nID;
                ezData.ZoneName = strName;
                ezData.LinkedZoneIDs = linkedZoneIDList;
                ezData.Boundary = vBoundaries;
                ezData.TextCenter = vTextCenter;
                ezData.DisplayText = strDisplayText;
                ezData.BroadcastText = strBroadcastText;
                int nType;
                if (int.TryParse(strType, out nType))
                    ezData.Type = nType;

                ZoneData zone;
                foreach (int zoneID in ezData.LinkedZoneIDs)
                {
                    if (m_dicZones.TryGetValue(zoneID, out zone))
                    {
                        zone.EquipmentZoneDatas.Add(ezData);
                        ezData.LinkedZoneDatas.Add(zone);
                    }
                }

                m_dicEquipZones[ezData.ID] = ezData;

                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadEquipmentZone : " + ex.Message;
                return false;
            }
        }

        private UnE.Geometry.Vertex3D ReadTextCenter(XmlReader reader, out string strErrorMessage)
        {
            strErrorMessage = null;
            UnE.Geometry.Vertex3D vertex3D = null;
            string strPoint3Ds = "";

            try
            {
                while (reader.Read())
                {
                    bool isStartElement = reader.IsStartElement();
                    if (!isStartElement)
                        break;

                    string readerName = reader.Name.ToString();
                    switch (readerName)
                    {                        
                        case XmlKey.XName_Point3D:
                            strPoint3Ds = reader.ReadInnerXml();
                            break;
                    }
                }

                string[] strPoint3D = strPoint3Ds.Split(',');
                if (strPoint3D != null && strPoint3D.Length == 3)
                    vertex3D = new UnE.Geometry.Vertex3D(Convert.ToDouble(strPoint3D[0]), Convert.ToDouble(strPoint3D[1]), Convert.ToDouble(strPoint3D[2]));

                return vertex3D;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadTextCenter : " + ex.Message;
                return null;
            }
        }

        private Polygon ReadBoundary(XmlReader reader, out string strErrorMessage)
        {
            strErrorMessage = null;
            string strBoundary = "";

            try
            {
                while (reader.Read())
                {
                    bool isStartElement = reader.IsStartElement();
                    if (!isStartElement && reader.Name == XmlKey.XName_Boundary)
                        break;

                    string readerName = reader.Name.ToString();
                    switch (readerName)
                    {
                        case XmlKey.XName_Point2D:                            
                            string[] strPoint2D = reader.ReadInnerXml().Split(',');
                            if (strPoint2D != null && strPoint2D.Length == 2)
                            {
                                if (strBoundary.Length > 0)
                                    strBoundary += strPoint2D[0] + "," + strPoint2D[1];
                            }

                            break;
                    }
                }

                Polygon polygon = StringToPolygon(strBoundary);
                return polygon;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadBoundary : " + ex.Message;
                return null;
            }
        }

        private Polygon StringToPolygon(string strVertices)
        {
            string[] tokens = strVertices.Split(',');

            if (tokens == null)
                return null;

            double x, y;
            int nTokenCount = tokens.Length;

            Polygon polygon = new Polygon();

            for (int i = 0; i < nTokenCount - 1; i += 2)
            {
                if (double.TryParse(tokens[i].Trim(), out x) &&
                    double.TryParse(tokens[i + 1].Trim(), out y))
                {
                    Vertex2D vertex = new Vertex2D(x, y);
                    polygon.AddVertex(vertex);
                }
                else
                    return null;
            }

            return polygon;
        }

        private List<int> ReadLinkedZoneIDList(XmlReader reader, out string strErrorMessage)
        {
            strErrorMessage = null;
            List<int> linkedZoneIDList = new List<int>();

            try
            {
                while (reader.Read())
                {
                    bool isStartElement = reader.IsStartElement();
                    if (!isStartElement)
                        break;

                    string readerName = reader.Name.ToString();
                    switch (readerName)
                    {
                        case XmlKey.XName_ZoneID:
                            int? nID = GetID(reader.ReadInnerXml(), XmlKey.KeyValue.Zone);
                            if (nID != null)
                                linkedZoneIDList.Add((int)nID);
                            break;
                    }
                }

                

                return linkedZoneIDList;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadLinkedZoneIDList : " + ex.Message;
                return null;
            }
        }
        #endregion

        #region Sensors
        private bool ReadSensors(XElement xe, out string strErrorMessage)
        {
            strErrorMessage = null;

            try
            {
                XElement xeSensors = FindElement(xe, XmlKey.XName_Sensors);

                XElement xeSensorTypes = FindElement(xeSensors, XmlKey.XName_SensorTypes);
                ReadSensorType(xeSensorTypes, out strErrorMessage);
                XElement xeFireSensorSubTypes = FindElement(xeSensors, XmlKey.XName_FireSensorSubTypes);
                ReadFireSensorSubType(xeFireSensorSubTypes, out strErrorMessage);

                XElement xeFireSensors = FindElement(xeSensors, XmlKey.XName_FireSensors);
                if (xeFireSensors != null)
                {
                    List<XElement> xeFireSensorList = FindElements(xeFireSensors, XmlKey.XName_Fire);
                    foreach (XElement xeFire in xeFireSensorList)
                        ReadEachSensor(xeFire, XmlKey.XName_Fire, out strErrorMessage); 
                }

                XElement xePsmSensors = FindElement(xeSensors, XmlKey.XName_PsmSensors);
                if (xePsmSensors != null)
                {
                    List<XElement> xePsmSensorList = FindElements(xePsmSensors, XmlKey.XName_Psm);
                    foreach (XElement xePsm in xePsmSensorList)
                        ReadEachSensor(xePsm, XmlKey.XName_Psm, out strErrorMessage); 
                }

                XElement xeEtcSensors = FindElement(xeSensors, XmlKey.XName_EtcSensors);
                if (xeEtcSensors != null)
                {
                    List<XElement> xeEtcSensorList = FindElements(xeEtcSensors, XmlKey.XName_Etc);
                    foreach (XElement xeEtc in xeEtcSensorList)
                        ReadEachSensor(xeEtc, XmlKey.XName_Etc, out strErrorMessage); 
                }

                XElement xeCCTVs = FindElement(xeSensors, XmlKey.XName_CCTVs);
                if (xeCCTVs != null)
                {
                    List<XElement> xeCCTVList = FindElements(xeCCTVs, XmlKey.XName_CCTV);
                    foreach (XElement xeCCTV in xeCCTVList)
                        ReadEachSensor(xeCCTV, XmlKey.XName_CCTV, out strErrorMessage); 
                }

                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadSensors : " + ex.Message;
                return false;
            }
        }

        private bool ReadSensors(XmlReader reader, out string strErrorMessage)
        {            
            try
            {
                strErrorMessage = null;

                while (reader.Read())
                {
                    bool isStartElement = reader.IsStartElement();
                    string readerName = reader.Name.ToString();
                    if (!isStartElement && readerName == XmlKey.XName_Sensors)
                        break;

                    switch (readerName)
                    {
                        case XmlKey.XName_SensorTypes:
                            if (!ReadSensorTypes(reader, out strErrorMessage))
                                return false;
                            break;
                        case XmlKey.XName_FireSensorSubTypes:
                            if (!ReadFireSensorSubTypes(reader, out strErrorMessage))
                                return false;
                            break;
                        case XmlKey.XName_FireSensors:
                        case XmlKey.XName_PsmSensors:
                        case XmlKey.XName_EtcSensors:
                        case XmlKey.XName_CCTVs:
                            if (!ReadEachSensors(reader, readerName, out strErrorMessage))
                                return false;
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadSensors : " + ex.Message;
                return false;
            }
        }

        private bool ReadSensorTypes(XmlReader reader, out string strErrorMessage)
        {
            try
            {
                strErrorMessage = null;

                while (reader.Read())
                {
                    bool isStartElement = reader.IsStartElement();
                    if (!isStartElement)
                        break;

                    string readerName = reader.Name.ToString();
                    switch (readerName)
                    {
                        case XmlKey.XName_SensorType:
                            if (!ReadSensorType(reader, out strErrorMessage))
                                return false;
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadSensorTypes : " + ex.Message;
                return false;
            }
        }

        private bool ReadSensorType(XElement xe, out string strErrorMessage)
        {
            strErrorMessage = null;

            try
            {
                List<XElement> xeSensorTypes = FindElements(xe, XmlKey.XName_SensorType);
                foreach (XElement xeSensorType in xeSensorTypes)
                {
                    string strIDValues = xeSensorType.Attribute("id").Value;
                    int? nID = GetID(strIDValues, XmlKey.KeyValue.SensorType);
                    if (nID == null)
                        throw new ApplicationException("ID 구하기 실패");

                    SensorType sensorType = new SensorType();
                    sensorType.ID = (int)nID;
                    sensorType.Name = FindElementValue(xeSensorType, XmlKey.XName_Name);

                    if (!m_sensorTypes.Contains(sensorType))
                        m_sensorTypes.Add(sensorType);
                }

                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadSensorType : " + ex.Message;
                return false;
            }
        }

        private bool ReadFireSensorSubType(XElement xe, out string strErrorMessage)
        {
            strErrorMessage = null;

            try
            {
                List<XElement> xeSensorTypes = FindElements(xe, XmlKey.XName_FireSensorSubType);
                foreach (XElement xeSensorType in xeSensorTypes)
                {
                    string strIDValues = xeSensorType.Attribute("id").Value;
                    int? nID = GetID(strIDValues, XmlKey.KeyValue.FireSensorSubType);
                    if (nID == null)
                        throw new ApplicationException("ID 구하기 실패");

                    SensorSubType subType = new SensorSubType();
                    subType.ID = (int)nID;
                    subType.Name = FindElementValue(xeSensorType, XmlKey.XName_Name);

                    if (!m_fireSensorSubTypes.Contains(subType))
                        m_fireSensorSubTypes.Add(subType);

                    // 화재만 Sub type 등록
                    foreach (SensorType item in m_sensorTypes)
                    {
                        if (item.ID == 0)
                        {
                            item.SubType.Add(subType);
                            break;
                        }
                    }
                } 
                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadFireSensorSubType : " + ex.Message;
                return false;
            }
        }

        private bool ReadEachSensor(XElement xe, string xName, out string strErrorMessage)
        {
            try
            {
                strErrorMessage = null;

                XmlKey.KeyValue keyValue = XmlKey.KeyValue.FireSensor;
                object sensor = null;
                if (xName == XmlKey.XName_Fire)
                {
                    sensor = new FireSensor();
                }
                else if (xName == XmlKey.XName_Psm)
                {
                    keyValue = XmlKey.KeyValue.PsmSensor;
                    sensor = new PSMSensor();
                }
                else if (xName == XmlKey.XName_Etc)
                {
                    keyValue = XmlKey.KeyValue.EtcSensor;
                    sensor = new EtcSensor();
                }
                else if (xName == XmlKey.XName_CCTV)
                {
                    keyValue = XmlKey.KeyValue.Cctv;
                    sensor = new CCTVSensor();
                }

                string strIDValues = xe.Attribute("id").Value;
                int? nID = GetID(strIDValues, keyValue);
                if (nID == null)
                    throw new ApplicationException("ID 구하기 실패");

                string strName = FindElementValue(xe, XmlKey.XName_Name);
                string strPositionName = FindElementValue(xe, XmlKey.XName_PositionName);
                string strPoint3Ds = FindElementValue(xe, XmlKey.XName_Point3D);
                float? x = null;
                float? y = null;
                float? z = null;
                string[] strPoint3D = strPoint3Ds.Split(',');
                if (strPoint3D != null && strPoint3D.Length == 3)
                {
                    float outX;
                    if (float.TryParse(strPoint3D[0], out outX))
                        x = outX;
                    float outY;
                    if (float.TryParse(strPoint3D[1], out outY))
                        y = outY;
                    float outZ;
                    if (float.TryParse(strPoint3D[2], out outZ))
                        z = outZ;
                }

                string strZoneID = FindElementValue(xe, XmlKey.XName_ZoneID);
                int? nZoneID = GetID(strZoneID, XmlKey.KeyValue.Zone);

                string strEquipzoneID = FindElementValue(xe, XmlKey.XName_EquipZoneID);
                int? nEquipzoneID = GetID(strEquipzoneID, XmlKey.KeyValue.EquipmentZone);

                string strSensorSubType = FindElementValue(xe, XmlKey.XName_SensorSubType);

                string strEquipZoneIDs = FindElementValue(xe, XmlKey.XName_EquipZoneIDs);
                List<int> equipZoneIDList = new List<int>();
                if (strEquipZoneIDs != null && strEquipZoneIDs.Length > 0)
                {
                    string[] splitEquipZones = strEquipZoneIDs.Split(',');
                    if (splitEquipZones != null && splitEquipZones.Length > 0)
                    {
                        for (int i = 0; i < splitEquipZones.Length; i++)
                        {
                            int? nTempEquipZoneID = GetID(splitEquipZones[i], XmlKey.KeyValue.EquipmentZone);
                            if (nTempEquipZoneID != null)
                                equipZoneIDList.Add((int)nTempEquipZoneID);
                        }
                    }
                }
                int? nSensorSubType = GetID(strSensorSubType, XmlKey.KeyValue.FireSensorSubType);

                string strMaterialType = FindElementValue(xe, XmlKey.XName_MaterialType);
                int? nMaterialType = GetID(strMaterialType, XmlKey.KeyValue.Material);

                string strTagNo = FindElementValue(xe, XmlKey.XName_TagNo);
                string strUniqueKey = FindElementValue(xe, XmlKey.XName_UniqueKey);
                string strUnitName = FindElementValue(xe, XmlKey.XName_UnitName);                

                if (sensor is FireSensor)
                {
                    FireSensor fs = sensor as FireSensor;
                    fs.ID = (int)nID;
                    fs.Name = strName;
                    fs.PositionName = strPositionName;
                    fs.X = x;
                    fs.Y = y;
                    fs.Z = z;
                    if (nZoneID != null && nZoneID > 0)
                    {
                        fs.ZoneID = (int)nZoneID;
                        ZoneData zoneData = null;
                        if (m_dicZones.TryGetValue((int)nZoneID, out zoneData))
                            fs.BuildingID = zoneData.BuildingID;
                    }

                    fs.SensorSubType = nSensorSubType;
                    fs.EquipZoneID = (nEquipzoneID == null || nEquipzoneID == -1) ? null : nEquipzoneID;
                    int nTagNo;
                    if (int.TryParse(strTagNo, out nTagNo))
                        fs.TagNo = nTagNo;
                    m_fireSensors.Add(fs);
                }
                else if (sensor is PSMSensor)
                {
                    PSMSensor ps = sensor as PSMSensor;
                    ps.ID = (int)nID;                    
                    ps.Name = strName;
                    ps.PositionName = strPositionName;
                    ps.X = x;
                    ps.Y = y;
                    ps.Z = z;
                    if (nZoneID != null && nZoneID > 0)
                    {
                        ps.ZoneID = (int)nZoneID;
                        ZoneData zoneData = null;
                        if (m_dicZones.TryGetValue((int)nZoneID, out zoneData))
                            ps.BuildingID = zoneData.BuildingID;
                    }
                    ps.MaterialType = nMaterialType;
                    if (nEquipzoneID != null)
                        ps.EquipZoneID = (int)nEquipzoneID;
                    ps.UniqueKey = strUniqueKey;
                    ps.UnitName = strUnitName;

                    m_psmSensors.Add(ps);
                }
                else if (sensor is EtcSensor)
                {
                    EtcSensor es = sensor as EtcSensor;
                    es.ID = (int)nID;
                    es.Name = strName;
                    es.PositionName = strPositionName;
                    es.X = x;
                    es.Y = y;
                    es.Z = z;
                    if (nZoneID != null && nZoneID > 0)
                    {
                        es.ZoneID = (int)nZoneID;
                        ZoneData zoneData = null;
                        if (m_dicZones.TryGetValue((int)nZoneID, out zoneData))
                            es.BuildingID = zoneData.BuildingID;
                    }
                    es.EquipZoneID = (nEquipzoneID == null || nEquipzoneID == -1) ? null : nEquipzoneID;
                    es.UniqueKey = strUniqueKey;
                    es.UnitName = strUnitName;

                    m_etcSensors.Add(es);
                }
                else if (sensor is CCTVSensor)
                {
                    string strType = FindElementValue(xe, XmlKey.XName_Type);
                    string strUserID = FindElementValue(xe, XmlKey.XName_UserID);
                    string strPassword = FindElementValue(xe, XmlKey.XName_Password);
                    string strURL = FindElementValue(xe, XmlKey.XName_Url);

                    CCTVSensor cc = sensor as CCTVSensor;
                    cc.ID = (int)nID;
                    cc.Name = strName;
                    cc.PositionName = strPositionName;
                    cc.X = x;
                    cc.Y = y;
                    cc.Z = z;
                    if (nZoneID != null)
                    {
                        cc.ZoneID = (int)nZoneID;
                        ZoneData zoneData = null;
                        if (m_dicZones.TryGetValue((int)nZoneID, out zoneData))
                            cc.BuildingID = zoneData.BuildingID;
                    }

                    cc.EquipZoneID = (nEquipzoneID == null || nEquipzoneID == -1) ? null : nEquipzoneID;
                    cc.EquipZoneIDs = equipZoneIDList;
                    cc.Type = strType;
                    cc.UserID = strUserID;
                    cc.Password = strPassword;
                    cc.URL = strURL;

                    m_cctvs.Add(cc);
                }

                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadEachSensor : " + ex.Message;
                return false;
            }
        }

        private bool ReadSensorType(XmlReader reader, out string strErrorMessage)
        {
            try
            {
                strErrorMessage = null;

                string strIDValues = reader.GetAttribute("id");
                int? nID = GetID(strIDValues, XmlKey.KeyValue.SensorType);
                if (nID == null)
                    throw new ApplicationException("ID 구하기 실패");

                string strName = "";

                while (reader.Read())
                {
                    bool isStartElement = reader.IsStartElement();
                    if (!isStartElement)
                        break;

                    string readerName = reader.Name.ToString();
                    switch (readerName)
                    {
                        case XmlKey.XName_Name:
                            strName = reader.ReadInnerXml();
                            break;
                    }
                }

                SensorType sensorType = new SensorType();
                sensorType.ID = (int)nID;
                sensorType.Name = strName;

                if (!m_sensorTypes.Contains(sensorType))
                    m_sensorTypes.Add(sensorType);

                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadSensorType : " + ex.Message;
                return false;
            }
        }

        private bool ReadFireSensorSubTypes(XmlReader reader, out string strErrorMessage)
        {
            try
            {
                strErrorMessage = null;

                while (reader.Read())
                {
                    bool isStartElement = reader.IsStartElement();
                    if (!isStartElement)
                        break;

                    string readerName = reader.Name.ToString();
                    switch (readerName)
                    {
                        case XmlKey.XName_FireSensorSubType:
                            if (!ReadFireSensorSubType(reader, out strErrorMessage))
                                return false;
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadFireSensorSubTypes : " + ex.Message;
                return false;
            }
        }

        private bool ReadFireSensorSubType(XmlReader reader, out string strErrorMessage)
        {
            try
            {
                strErrorMessage = null;

                string strIDValues = reader.GetAttribute("id");
                int? nID = GetID(strIDValues, XmlKey.KeyValue.FireSensorSubType);
                if (nID == null)
                    throw new ApplicationException("ID 구하기 실패");

                string strName = "";

                while (reader.Read())
                {
                    bool isStartElement = reader.IsStartElement();
                    if (!isStartElement)
                        break;

                    string readerName = reader.Name.ToString();
                    switch (readerName)
                    {
                        case XmlKey.XName_Name:
                            strName = reader.ReadInnerXml();
                            break;
                    }
                }

                SensorSubType subType = new SensorSubType();
                subType.ID = (int)nID;
                subType.Name = strName;

                if (!m_fireSensorSubTypes.Contains(subType))
                    m_fireSensorSubTypes.Add(subType);

                // 화재만 Sub type 등록
                foreach (SensorType item in m_sensorTypes)
                {
                    if (item.ID == 0)
                    {
                        item.SubType.Add(subType);
                        break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadFireSensorSubType : " + ex.Message;
                return false;
            }
        }

        private bool ReadEachSensors(XmlReader reader, string xName, out string strErrorMessage)
        {
            try
            {
                strErrorMessage = null;

                while (reader.Read())
                {
                    bool isStartElement = reader.IsStartElement();
                    string readerName = reader.Name.ToString();
                    if (!isStartElement && readerName == xName)
                        break;
                                        
                    switch (readerName)
                    {
                        case XmlKey.XName_Fire:                        
                        case XmlKey.XName_Psm:
                        case XmlKey.XName_Etc:
                        case XmlKey.XName_CCTV:
                            if (!ReadEachSensor(reader, readerName, out strErrorMessage))
                                return false;
                            break;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadEachSensors : " + ex.Message;
                return false;
            }
        }

        private bool ReadEachSensor(XmlReader reader, string xName, out string strErrorMessage)
        {
            try
            {
                strErrorMessage = null;

                XmlKey.KeyValue keyValue = XmlKey.KeyValue.FireSensor;
                object sensor = null;
                if (xName == XmlKey.XName_Fire)
                {
                    sensor = new FireSensor();
                }
                else if (xName == XmlKey.XName_Psm)
                {
                    keyValue = XmlKey.KeyValue.PsmSensor;
                    sensor = new PSMSensor();
                }
                else if (xName == XmlKey.XName_Etc)
                {
                    keyValue = XmlKey.KeyValue.EtcSensor;
                    sensor = new EtcSensor();
                }
                else if (xName == XmlKey.XName_CCTV)
                {
                    keyValue = XmlKey.KeyValue.Cctv;
                    sensor = new CCTVSensor();
                }

                string strIDValues = reader.GetAttribute("id");
                int? nID = GetID(strIDValues, keyValue);
                if (nID == null)
                    throw new ApplicationException("ID 구하기 실패");

                string strName = "";
                string strPositionName = "";
                float? x = null;
                float? y = null;
                float? z = null;
                int? nZoneID = null;
                int? nEquipzoneID = null;
                int? nSensorSubType = null;
                int? nMaterialType = null;

                while (reader.Read())
                {
                    bool isStartElement = reader.IsStartElement();
                    string readerName = reader.Name.ToString();
                    if (!isStartElement && readerName == xName)
                        break;
                                        
                    switch (readerName)
                    {
                        case XmlKey.XName_Name:
                            strName = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_PositionName:
                            strPositionName = reader.ReadInnerXml();
                            break;
                        case XmlKey.XName_Point3D:
                            string strPoint3Ds = reader.ReadInnerXml();
                            string[] strPoint3D = strPoint3Ds.Split(',');
                            if (strPoint3D != null && strPoint3D.Length == 3)
                            {
                                float outX;
                                if (float.TryParse(strPoint3D[0], out outX))
                                    x = outX;
                                float outY;
                                if (float.TryParse(strPoint3D[1], out outY))
                                    y = outY;
                                float outZ;
                                if (float.TryParse(strPoint3D[2], out outZ))
                                    z = outZ;
                            }
                            break;
                        case XmlKey.XName_ZoneID:
                            nZoneID = GetID(reader.ReadInnerXml(), XmlKey.KeyValue.Zone);                            
                            //if (nZoneID == null)
                            //    throw new ApplicationException("Zone ID는 Null일 수 없습니다.");
                            break;
                        case XmlKey.XName_EquipZoneID:
                            nEquipzoneID = GetID(reader.ReadInnerXml(), XmlKey.KeyValue.EquipmentZone);
                            break;
                        case XmlKey.XName_SensorSubType:
                            nSensorSubType = GetID(reader.ReadInnerXml(), XmlKey.KeyValue.FireSensorSubType);
                            break;
                        case XmlKey.XName_MaterialType:
                            nMaterialType = GetID(reader.ReadInnerXml(), XmlKey.KeyValue.Material);
                            break;
                    }
                }

                if (sensor is FireSensor)
                {
                    FireSensor fs = sensor as FireSensor;
                    fs.ID = (int)nID;
                    fs.Name = strName;
                    fs.PositionName = strPositionName;
                    fs.X = x;
                    fs.Y = y;
                    fs.Z = z;
                    if (nZoneID != null)
                        fs.ZoneID = (int)nZoneID;
                    fs.SensorSubType = nSensorSubType;

                    m_fireSensors.Add(fs);
                }
                else if (sensor is PSMSensor)
                {
                    PSMSensor ps = sensor as PSMSensor;
                    ps.ID = (int)nID;
                    ps.Name = strName;
                    ps.PositionName = strPositionName;
                    ps.X = x;
                    ps.Y = y;
                    ps.Z = z;
                    if (nZoneID != null)
                        ps.ZoneID = (int)nZoneID;
                    ps.MaterialType = nMaterialType;

                    m_psmSensors.Add(ps);
                }
                else if (sensor is EtcSensor)
                {
                    EtcSensor es = sensor as EtcSensor;
                    es.ID = (int)nID;
                    es.Name = strName;
                    es.PositionName = strPositionName;
                    es.X = x;
                    es.Y = y;
                    es.Z = z;
                    if (nZoneID != null)
                        es.ZoneID = (int)nZoneID;

                    m_etcSensors.Add(es);
                }
                else if (sensor is CCTVSensor)
                {
                    CCTVSensor cc = sensor as CCTVSensor;
                    cc.ID = (int)nID;
                    cc.Name = strName;                    
                    cc.PositionName = strPositionName;
                    cc.X = x;
                    cc.Y = y;
                    cc.Z = z;
                    if (nZoneID != null)
                        cc.ZoneID = (int)nZoneID;

                    m_cctvs.Add(cc);
                }

                return true;
            }
            catch (Exception ex)
            {
                strErrorMessage = "ReadEachSensor : " + ex.Message;
                return false;
            }
        }
        #endregion

        private int? GetID(string strValue, XmlKey.KeyValue keyValue)
        {
            if (strValue == null || strValue.Length == 0)
                return null;

            strValue = strValue.Replace(XmlKey.GetKeyValueSting(keyValue), "");
            
            int nID;
            if (int.TryParse(strValue, out nID))
                return nID;
            else
                return null;
        }

        public ResponseSaveXML SaveXML(RequestSaveXML req)
        {
            ResponseSaveXML res = new ResponseSaveXML();

            try
            {
                XDocument doc = new XDocument(new XDeclaration("1.0", "UTF-8", null));
                XElement xRoot = MakeRoot(doc);

                XElement xSpaces = MakeElement(xRoot, XmlKey.XName_Spaces);
                XElement xSite = MakeElement(xSpaces, XmlKey.XName_Site);
                MakeElement(xSite, XmlKey.XName_Name, req.SiteName);
                MakeGltf(xSpaces, req.Models, req.GltfOption);

                MakeSpatial(xSpaces, req.TestBuildingGroupData, req.TestBuildingData, req.TestZoneData, req.TestEquipmentZoneData, req.OutdoorZones);

                XElement xSensors = MakeElement(xRoot, XmlKey.XName_Sensors);
                MakeSensorTypes(xSensors, req.SensorTypes);
                MakeFireSensorSubTypes(xSensors, req.SensorTypes);

                MakeSensors(xSensors, req.FireSensors, req.PSMSensors, req.EtcSensors, req.Cctvs);

                res.Success = true;
                res.XDocument = doc;
            }
            catch (Exception ex)
            {
                res.Message = "SaveXML : " + ex.Message;
                res.Success = false;
            }
                        
            return res;
        }

        public XElement MakeRoot(XDocument doc)
        {
            XElement xRoot =
                new XElement("UnE.Space.Sensor",
                    new XAttribute("version", "1.0"),
                    new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                    new XAttribute(XNamespace.Get("xsi") + "noNamespaceSchemaLocation", "UnE.Space.Sensor.xsd"));
            doc.Add(xRoot);

            return xRoot;
        }

        public XElement MakeElement(XElement parentEle, string strName, params object[] content)
        {
            XElement xe = new XElement(strName, content);            
            parentEle.Add(xe);

            return xe;
        }

        public XElement MakeElement(XElement parentEle, string strName, string strValue, params object[] content)
        {
            XElement xe = new XElement(strName, content);
            xe.Value = strValue == null ? "" : strValue;
            parentEle.Add(xe);

            return xe;
        }
        
        private void MakeSpatial(XElement parentEle, List<BuildingGroupData> buildingGroups, List<SDMS.Model.Spatial.Building> buildings, List<SDMS.Model.Spatial.Zone> zones, List<SDMS.Model.Spatial.EquipmentZone> equipmentZones, List<ZoneData> outdoorZones)
        {
            XElement xBGs = MakeElement(parentEle, XmlKey.XName_BuildingGroups);
            XElement xBs = MakeElement(parentEle, XmlKey.XName_Buildings);
            XElement xZs = MakeElement(parentEle, XmlKey.XName_Zones);
            XElement xEzs = MakeElement(parentEle, XmlKey.XName_EquipmentZones);

            if (buildingGroups != null)
            {
                foreach (BuildingGroupData bg in buildingGroups)
                {
                    XElement xBG = MakeElement(xBGs, XmlKey.XName_BuildingGroup, new XAttribute("id", XmlKey.GetKeyValueSting(XmlKey.KeyValue.BuildingGroup) + bg.ID));
                    MakeElement(xBG, XmlKey.XName_Name, bg.GroupName);
                    XElement xTC = MakeElement(xBG, XmlKey.XName_TextCenter);
                    if (bg.TextCenter != null)
                        MakeElement(xTC, XmlKey.XName_Point3D, bg.TextCenter.x + "," + bg.TextCenter.y + "," + bg.TextCenter.z);
                    MakeElement(xBG, XmlKey.XName_DisplayText, bg.DisplayText);
                    MakeElement(xBG, XmlKey.XName_ParentID, bg.ParentID);
                    MakeElement(xBG, XmlKey.XName_SiteID, bg.SiteID);
                    MakeElement(xBG, XmlKey.XName_Visible, bg.Visible);
                }
            }

            if (buildings != null)
            {
                foreach (SDMS.Model.Spatial.Building b in buildings)
                {
                    XElement xB = MakeElement(xBs, XmlKey.XName_Building, new XAttribute("id", XmlKey.GetKeyValueSting(XmlKey.KeyValue.Building) + b.ID));
                    MakeElement(xB, XmlKey.XName_Code, b.BuildingCode);
                    MakeElement(xB, XmlKey.XName_Name, b.BuildingName);
                    MakeElement(xB, XmlKey.XName_BuildingGroupID, XmlKey.GetKeyValueSting(XmlKey.KeyValue.BuildingGroup) + b.BuildingGroupID);
                    MakeElement(xB, XmlKey.XName_MaxFloor, b.MaxFloor.ToString());
                    MakeElement(xB, XmlKey.XName_MinFloor, b.MinFloor.ToString());
                    XElement xBTc = MakeElement(xB, XmlKey.XName_TextCenter);
                    if (b.TextCenter != null)
                        MakeElement(xBTc, XmlKey.XName_Point3D, b.TextCenter.x + "," + b.TextCenter.y + "," + b.TextCenter.z);
                    else
                        MakeElement(xBTc, XmlKey.XName_Point3D, "");

                    MakeElement(xB, XmlKey.XName_DisplayText, b.DisplayText);
                    MakeElement(xB, XmlKey.XName_BroadcastText, b.BroadcastText);
                }
            }

            if (zones != null)
            {
                foreach (SDMS.Model.Spatial.Zone z in zones)
                {
                    XElement xZ = MakeElement(xZs, XmlKey.XName_Zone, new XAttribute("id", XmlKey.GetKeyValueSting(XmlKey.KeyValue.Zone) + z.ID));
                    MakeElement(xZ, XmlKey.XName_Name, z.ZoneName);
                    MakeElement(xZ, XmlKey.XName_BuildingID, XmlKey.GetKeyValueSting(XmlKey.KeyValue.Building) + z.BuildingID);
                    MakeElement(xZ, XmlKey.XName_FloorIndex, z.FloorIndex.ToString());
                    XElement xBoundary = MakeElement(xZ, XmlKey.XName_Boundary);
                    XElement xPolygon = MakeElement(xBoundary, XmlKey.XName_Polygon);
                    if (z.Boundary != null)
                    {
                        List<UnE.Geometry.Vertex2D> polygons = z.Boundary.GetVertexList();
                        foreach (UnE.Geometry.Vertex2D vertex in polygons)
                            MakeElement(xPolygon, XmlKey.XName_Point2D, vertex.x + "," + vertex.y);
                    }
                    XElement xZTc = MakeElement(xZ, XmlKey.XName_TextCenter);
                    if (z.TextCenter != null)
                        MakeElement(xZTc, XmlKey.XName_Point3D, z.TextCenter.x + "," + z.TextCenter.y + "," + z.TextCenter.z);
                    else
                        MakeElement(xZTc, XmlKey.XName_Point3D, "");

                    MakeElement(xZ, XmlKey.XName_DisplayText, z.DisplayText);
                    MakeElement(xZ, XmlKey.XName_BroadcastText, z.BroadcastText);
                }
            }

            if (equipmentZones != null)
            {
                foreach (SDMS.Model.Spatial.EquipmentZone ez in equipmentZones)
                {
                    XElement xEZ = MakeElement(xEzs, XmlKey.XName_EquipmentZone, new XAttribute("id", XmlKey.GetKeyValueSting(XmlKey.KeyValue.EquipmentZone) + ez.ID));
                    MakeElement(xEZ, XmlKey.XName_Name, ez.ZoneName);
                    XElement xEZBoundary = MakeElement(xEZ, XmlKey.XName_Boundary);
                    XElement xEZPolygon = MakeElement(xEZBoundary, XmlKey.XName_Polygon);
                    if (ez.Boundary != null)
                    {
                        List<UnE.Geometry.Vertex2D> ezpolygons = ez.Boundary.GetVertexList();
                        foreach (UnE.Geometry.Vertex2D vertex in ezpolygons)
                            MakeElement(xEZPolygon, XmlKey.XName_Point2D, vertex.x + "," + vertex.y);
                    }
                    XElement xLinkedZIDs = MakeElement(xEZ, XmlKey.XName_LinkedZoneIDList);
                    foreach (int linkedZoneID in ez.LinkedZoneIDs)
                        MakeElement(xLinkedZIDs, XmlKey.XName_ZoneID, XmlKey.GetKeyValueSting(XmlKey.KeyValue.Zone) + linkedZoneID);

                    MakeElement(xEZ, XmlKey.XName_Type, (ez.Type == null) ? "" : ez.Type.ToString());

                    XElement xEZTc = MakeElement(xEZ, XmlKey.XName_TextCenter);
                    if (ez.TextCenter != null)
                        MakeElement(xEZTc, XmlKey.XName_Point3D, ez.TextCenter.x + "," + ez.TextCenter.y + "," + ez.TextCenter.z);
                    else
                        MakeElement(xEZTc, XmlKey.XName_Point3D, "");

                    MakeElement(xEZ, XmlKey.XName_DisplayText, ez.DisplayText);
                    MakeElement(xEZ, XmlKey.XName_BroadcastText, ez.BroadcastText);
                }
            }

            if (outdoorZones != null)
            {
                foreach (ZoneData z in outdoorZones)
                {
                    XElement xZ = MakeElement(xZs, XmlKey.XName_Zone, new XAttribute("id", XmlKey.GetKeyValueSting(XmlKey.KeyValue.Zone) + z.ID));
                    MakeElement(xZ, XmlKey.XName_Name, z.ZoneName);
                    MakeElement(xZ, XmlKey.XName_BuildingID, XmlKey.GetKeyValueSting(XmlKey.KeyValue.Building) + z.BuildingID);
                    MakeElement(xZ, XmlKey.XName_FloorIndex, z.FloorIndex.ToString());
                    XElement xBoundary = MakeElement(xZ, XmlKey.XName_Boundary);
                    XElement xPolygon = MakeElement(xBoundary, XmlKey.XName_Polygon);
                    if (z.Boundary != null)
                    {
                        List<UnE.Geometry.Vertex2D> polygons = z.Boundary.GetVertexList();
                        foreach (UnE.Geometry.Vertex2D vertex in polygons)
                            MakeElement(xPolygon, XmlKey.XName_Point2D, vertex.x + "," + vertex.y);
                    }
                    XElement xZTc = MakeElement(xZ, XmlKey.XName_TextCenter);
                    if (z.TextCenter != null)
                        MakeElement(xZTc, XmlKey.XName_Point3D, z.TextCenter.x + "," + z.TextCenter.y + "," + z.TextCenter.z);
                    else
                        MakeElement(xZTc, XmlKey.XName_Point3D, "");

                    MakeElement(xZ, XmlKey.XName_DisplayText, z.DisplayText);
                    MakeElement(xZ, XmlKey.XName_BroadcastText, z.BroadcastText);

                    foreach (EquipmentZoneData ez in z.EquipmentZoneDatas)
                    {
                        XElement xEZ = MakeElement(xEzs, XmlKey.XName_EquipmentZone, new XAttribute("id", XmlKey.GetKeyValueSting(XmlKey.KeyValue.EquipmentZone) + ez.ID));
                        MakeElement(xEZ, XmlKey.XName_Name, ez.ZoneName);
                        XElement xEZBoundary = MakeElement(xEZ, XmlKey.XName_Boundary);
                        XElement xEZPolygon = MakeElement(xEZBoundary, XmlKey.XName_Polygon);
                        if (ez.Boundary != null)
                        {
                            List<UnE.Geometry.Vertex2D> ezpolygons = ez.Boundary.GetVertexList();
                            foreach (UnE.Geometry.Vertex2D vertex in ezpolygons)
                                MakeElement(xEZPolygon, XmlKey.XName_Point2D, vertex.x + "," + vertex.y);
                        }
                        XElement xLinkedZIDs = MakeElement(xEZ, XmlKey.XName_LinkedZoneIDList);
                        foreach (int linkedZoneID in ez.LinkedZoneIDs)
                            MakeElement(xLinkedZIDs, XmlKey.XName_ZoneID, XmlKey.GetKeyValueSting(XmlKey.KeyValue.Zone) + linkedZoneID);

                        MakeElement(xEZ, XmlKey.XName_Type, (ez.Type == null) ? "" : ez.Type.ToString());

                        XElement xEZTc = MakeElement(xEZ, XmlKey.XName_TextCenter);
                        if (ez.TextCenter != null)
                            MakeElement(xEZTc, XmlKey.XName_Point3D, ez.TextCenter.x + "," + ez.TextCenter.y + "," + ez.TextCenter.z);
                        else
                            MakeElement(xEZTc, XmlKey.XName_Point3D, "");

                        MakeElement(xEZ, XmlKey.XName_DisplayText, ez.DisplayText);
                        MakeElement(xEZ, XmlKey.XName_BroadcastText, ez.BroadcastText);
                    }
                }
            }
        }

        private void MakeSpatial(XElement parentEle, List<BuildingGroupData> buildingGroups, List<ZoneData> outdoorZones)
        {
            XElement xBGs = MakeElement(parentEle, XmlKey.XName_BuildingGroups);
            XElement xBs = MakeElement(parentEle, XmlKey.XName_Buildings);
            XElement xZs = MakeElement(parentEle, XmlKey.XName_Zones);
            XElement xEzs = MakeElement(parentEle, XmlKey.XName_EquipmentZones);

            if (buildingGroups != null)
            {
                foreach (BuildingGroupData bg in buildingGroups)
                {
                    XElement xBG = MakeElement(xBGs, XmlKey.XName_BuildingGroup, new XAttribute("id", XmlKey.GetKeyValueSting(XmlKey.KeyValue.BuildingGroup) + bg.ID));
                    MakeElement(xBG, XmlKey.XName_Name, bg.GroupName);
                    XElement xTC = MakeElement(xBG, XmlKey.XName_TextCenter);
                    if (bg.TextCenter != null)
                        MakeElement(xTC, XmlKey.XName_Point3D, bg.TextCenter.x + "," + bg.TextCenter.y + "," + bg.TextCenter.z); 
                    MakeElement(xBG, XmlKey.XName_DisplayText, bg.DisplayText);
                    MakeElement(xBG, XmlKey.XName_ParentID, bg.ParentID);
                    MakeElement(xBG, XmlKey.XName_SiteID, bg.SiteID);                    
                    MakeElement(xBG, XmlKey.XName_Visible, (buildingGroups.Count == 1) ? "false" : "true");

                    foreach (BuildingData b in bg.BuildingDatas)
                    {
                        XElement xB = MakeElement(xBs, XmlKey.XName_Building, new XAttribute("id", XmlKey.GetKeyValueSting(XmlKey.KeyValue.Building) + b.ID));
                        MakeElement(xB, XmlKey.XName_Code, b.BuildingCode);
                        MakeElement(xB, XmlKey.XName_Name, b.BuildingName);
                        MakeElement(xB, XmlKey.XName_BuildingGroupID, XmlKey.GetKeyValueSting(XmlKey.KeyValue.BuildingGroup) + b.BuildingGroupID);
                        MakeElement(xB, XmlKey.XName_MaxFloor, b.MaxFloor.ToString());
                        MakeElement(xB, XmlKey.XName_MinFloor, b.MinFloor.ToString());
                        XElement xBTc = MakeElement(xB, XmlKey.XName_TextCenter);
                        if (b.TextCenter != null)
                            MakeElement(xBTc, XmlKey.XName_Point3D, b.TextCenter.x + "," + b.TextCenter.y + "," + b.TextCenter.z);
                        else
                            MakeElement(xBTc, XmlKey.XName_Point3D, "");

                        MakeElement(xB, XmlKey.XName_DisplayText, b.DisplayText);
                        MakeElement(xB, XmlKey.XName_BroadcastText, b.BroadcastText);

                        foreach (ZoneData z in b.ZoneDatas)
                        {
                            XElement xZ = MakeElement(xZs, XmlKey.XName_Zone, new XAttribute("id", XmlKey.GetKeyValueSting(XmlKey.KeyValue.Zone) + z.ID));
                            MakeElement(xZ, XmlKey.XName_Name, z.ZoneName);
                            MakeElement(xZ, XmlKey.XName_BuildingID, XmlKey.GetKeyValueSting(XmlKey.KeyValue.Building) + z.BuildingID);
                            MakeElement(xZ, XmlKey.XName_FloorIndex, z.FloorIndex.ToString());
                            XElement xBoundary = MakeElement(xZ, XmlKey.XName_Boundary);
                            XElement xPolygon = MakeElement(xBoundary, XmlKey.XName_Polygon);
                            if (z.Boundary != null)
                            {
                                List<UnE.Geometry.Vertex2D> polygons = z.Boundary.GetVertexList();
                                foreach (UnE.Geometry.Vertex2D vertex in polygons)
                                    MakeElement(xPolygon, XmlKey.XName_Point2D, vertex.x + "," + vertex.y);
                            }
                            XElement xZTc = MakeElement(xZ, XmlKey.XName_TextCenter);
                            if (z.TextCenter != null)
                                MakeElement(xZTc, XmlKey.XName_Point3D, z.TextCenter.x + "," + z.TextCenter.y + "," + z.TextCenter.z);
                            else
                                MakeElement(xZTc, XmlKey.XName_Point3D, "");

                            MakeElement(xZ, XmlKey.XName_DisplayText, z.DisplayText);
                            MakeElement(xZ, XmlKey.XName_BroadcastText, z.BroadcastText);

                            foreach (EquipmentZoneData ez in z.EquipmentZoneDatas)
                            {
                                XElement xEZ = MakeElement(xEzs, XmlKey.XName_EquipmentZone, new XAttribute("id", XmlKey.GetKeyValueSting(XmlKey.KeyValue.EquipmentZone) + ez.ID));
                                MakeElement(xEZ, XmlKey.XName_Name, ez.ZoneName);
                                XElement xEZBoundary = MakeElement(xEZ, XmlKey.XName_Boundary);
                                XElement xEZPolygon = MakeElement(xEZBoundary, XmlKey.XName_Polygon);
                                if (ez.Boundary != null)
                                {
                                    List<UnE.Geometry.Vertex2D> ezpolygons = ez.Boundary.GetVertexList();
                                    foreach (UnE.Geometry.Vertex2D vertex in ezpolygons)
                                        MakeElement(xEZPolygon, XmlKey.XName_Point2D, vertex.x + "," + vertex.y);
                                }
                                XElement xLinkedZIDs = MakeElement(xEZ, XmlKey.XName_LinkedZoneIDList);
                                foreach (int linkedZoneID in ez.LinkedZoneIDs)
                                    MakeElement(xLinkedZIDs, XmlKey.XName_ZoneID, XmlKey.GetKeyValueSting(XmlKey.KeyValue.Zone) + linkedZoneID);

                                MakeElement(xEZ, XmlKey.XName_Type, (ez.Type == null) ? "" : ez.Type.ToString());

                                XElement xEZTc = MakeElement(xEZ, XmlKey.XName_TextCenter);
                                if (ez.TextCenter != null)
                                    MakeElement(xEZTc, XmlKey.XName_Point3D, ez.TextCenter.x + "," + ez.TextCenter.y + "," + ez.TextCenter.z);
                                else
                                    MakeElement(xEZTc, XmlKey.XName_Point3D, "");

                                MakeElement(xEZ, XmlKey.XName_DisplayText, ez.DisplayText);
                                MakeElement(xEZ, XmlKey.XName_BroadcastText, ez.BroadcastText);
                            }
                        }
                    }
                }
            }

            if (outdoorZones != null)
            {
                foreach (ZoneData z in outdoorZones)
                {
                    XElement xZ = MakeElement(xZs, XmlKey.XName_Zone, new XAttribute("id", XmlKey.GetKeyValueSting(XmlKey.KeyValue.Zone) + z.ID));
                    MakeElement(xZ, XmlKey.XName_Name, z.ZoneName);
                    MakeElement(xZ, XmlKey.XName_BuildingID, XmlKey.GetKeyValueSting(XmlKey.KeyValue.Building) + z.BuildingID);
                    MakeElement(xZ, XmlKey.XName_FloorIndex, z.FloorIndex.ToString());
                    XElement xBoundary = MakeElement(xZ, XmlKey.XName_Boundary);
                    XElement xPolygon = MakeElement(xBoundary, XmlKey.XName_Polygon);
                    if (z.Boundary != null)
                    {
                        List<UnE.Geometry.Vertex2D> polygons = z.Boundary.GetVertexList();
                        foreach (UnE.Geometry.Vertex2D vertex in polygons)
                            MakeElement(xPolygon, XmlKey.XName_Point2D, vertex.x + "," + vertex.y);
                    }
                    XElement xZTc = MakeElement(xZ, XmlKey.XName_TextCenter);
                    if (z.TextCenter != null)
                        MakeElement(xZTc, XmlKey.XName_Point3D, z.TextCenter.x + "," + z.TextCenter.y + "," + z.TextCenter.z);
                    else
                        MakeElement(xZTc, XmlKey.XName_Point3D, "");

                    MakeElement(xZ, XmlKey.XName_DisplayText, z.DisplayText);
                    MakeElement(xZ, XmlKey.XName_BroadcastText, z.BroadcastText);

                    foreach (EquipmentZoneData ez in z.EquipmentZoneDatas)
                    {
                        XElement xEZ = MakeElement(xEzs, XmlKey.XName_EquipmentZone, new XAttribute("id", XmlKey.GetKeyValueSting(XmlKey.KeyValue.EquipmentZone) + ez.ID));
                        MakeElement(xEZ, XmlKey.XName_Name, ez.ZoneName);
                        XElement xEZBoundary = MakeElement(xEZ, XmlKey.XName_Boundary);
                        XElement xEZPolygon = MakeElement(xEZBoundary, XmlKey.XName_Polygon);
                        if (ez.Boundary != null)
                        {
                            List<UnE.Geometry.Vertex2D> ezpolygons = ez.Boundary.GetVertexList();
                            foreach (UnE.Geometry.Vertex2D vertex in ezpolygons)
                                MakeElement(xEZPolygon, XmlKey.XName_Point2D, vertex.x + "," + vertex.y);
                        }
                        XElement xLinkedZIDs = MakeElement(xEZ, XmlKey.XName_LinkedZoneIDList);
                        foreach (int linkedZoneID in ez.LinkedZoneIDs)
                            MakeElement(xLinkedZIDs, XmlKey.XName_ZoneID, XmlKey.GetKeyValueSting(XmlKey.KeyValue.Zone) + linkedZoneID);

                        MakeElement(xEZ, XmlKey.XName_Type, (ez.Type == null) ? "" : ez.Type.ToString());

                        XElement xEZTc = MakeElement(xEZ, XmlKey.XName_TextCenter);
                        if (ez.TextCenter != null)
                            MakeElement(xEZTc, XmlKey.XName_Point3D, ez.TextCenter.x + "," + ez.TextCenter.y + "," + ez.TextCenter.z);
                        else
                            MakeElement(xEZTc, XmlKey.XName_Point3D, "");

                        MakeElement(xEZ, XmlKey.XName_DisplayText, ez.DisplayText);
                        MakeElement(xEZ, XmlKey.XName_BroadcastText, ez.BroadcastText);
                    }
                }
            }
        }

        private void MakeSensors(XElement parentEle, List<FireSensor> fireSensors, List<PSMSensor> psmSensors, List<EtcSensor> etcSensors, List<CCTVSensor> cctvs)
        {            
            if (fireSensors != null && fireSensors.Count > 0)
            {
                XElement xFss = MakeElement(parentEle, XmlKey.XName_FireSensors);
                foreach (FireSensor sensor in fireSensors)
                    MakeSensorInner(xFss, XmlKey.XName_FireSensors, sensor);
            }

            if (psmSensors != null && psmSensors.Count > 0)
            {
                XElement xPss = MakeElement(parentEle, XmlKey.XName_PsmSensors);
                foreach (PSMSensor sensor in psmSensors)
                    MakeSensorInner(xPss, XmlKey.XName_PsmSensors, sensor);
            }

            if (etcSensors != null && etcSensors.Count > 0)
            {
                XElement xEss = MakeElement(parentEle, XmlKey.XName_EtcSensors);
                foreach (EtcSensor sensor in etcSensors)
                    MakeSensorInner(xEss, XmlKey.XName_EtcSensors, sensor);
            }

            if (cctvs != null && cctvs.Count > 0)
            {
                XElement xCCs = MakeElement(parentEle, XmlKey.XName_CCTVs);
                foreach (CCTVSensor sensor in cctvs)
                    MakeSensorInner(xCCs, XmlKey.XName_CCTVs, sensor);
            }
        }

        private void MakeSensorInner(XElement parentEle, string strKey, object sensor)
        {
            try
            {
                string strXName = "";
                string strID = "";
                string strName = "";
                string strPositionName = "";
                string strPoint3D = "";
                string strBuildingID = XmlKey.GetKeyValueSting(XmlKey.KeyValue.Building);
                string strZoneID = XmlKey.GetKeyValueSting(XmlKey.KeyValue.Zone);
                string strEquipZoneID = XmlKey.GetKeyValueSting(XmlKey.KeyValue.EquipmentZone);
                string strEquipZoneIDs = "";
                string strSensorSubType = XmlKey.GetKeyValueSting(XmlKey.KeyValue.FireSensorSubType);
                string strTagNo = "";
                string strUniqueKey = "";
                string strMaterialType = "";
                string strUnitName = "";
                string strType = "";
                string strUserID = "";
                string strPassword = "";
                string strURL = "";

                if (sensor is FireSensor)
                {
                    FireSensor fs = sensor as FireSensor;                    
                    strXName = XmlKey.XName_Fire;
                    strID = XmlKey.GetKeyValueSting(XmlKey.KeyValue.FireSensor) + fs.ID.ToString();
                    strName = fs.Name;
                    strPositionName = (fs.PositionName == null) ? "" : fs.PositionName;
                    strPoint3D = fs.X + "," + fs.Y + "," + fs.Z;
                    strBuildingID += fs.BuildingID;
                    strZoneID += fs.ZoneID;
                    strEquipZoneID += fs.EquipZoneID;
                    strSensorSubType += (fs.SensorSubType == null) ? -1 : (int)fs.SensorSubType;
                    strTagNo = fs.TagNo == null ? "" : fs.TagNo.ToString();
                }
                else if (sensor is PSMSensor)
                {
                    PSMSensor ps = sensor as PSMSensor;
                    strXName = XmlKey.XName_Psm;
                    strID = XmlKey.GetKeyValueSting(XmlKey.KeyValue.PsmSensor) + ps.ID.ToString();
                    strName = ps.Name;
                    strPositionName = (ps.PositionName == null) ? "" : ps.PositionName;
                    strPoint3D = ps.X + "," + ps.Y + "," + ps.Z;
                    strBuildingID += ps.BuildingID;
                    strZoneID += ps.ZoneID;
                    strEquipZoneID += ps.EquipZoneID;                    
                    strUniqueKey = ps.UniqueKey;
                    if (ps.MaterialType != null)
                        strMaterialType = XmlKey.GetKeyValueSting(XmlKey.KeyValue.Material) + ps.MaterialType;
                    strUnitName = ps.UnitName;
                    
                }
                else if (sensor is EtcSensor)
                {
                    EtcSensor es = sensor as EtcSensor;
                    strXName = XmlKey.XName_Etc;
                    strID = XmlKey.GetKeyValueSting(XmlKey.KeyValue.EtcSensor) + es.ID.ToString();
                    strName = es.Name;
                    strPositionName = (es.PositionName == null) ? "" : es.PositionName;
                    strPoint3D = es.X + "," + es.Y + "," + es.Z;
                    strBuildingID += es.BuildingID;
                    strZoneID += es.ZoneID;
                    strEquipZoneID += es.EquipZoneID;
                    strUniqueKey = es.UniqueKey;
                    if (es.MaterialType != null)
                        strMaterialType = XmlKey.GetKeyValueSting(XmlKey.KeyValue.Material) + es.MaterialType;
                    strUnitName = es.UnitName;
                }
                else if (sensor is CCTVSensor)
                {
                    CCTVSensor cs = sensor as CCTVSensor;
                    strXName = XmlKey.XName_CCTV;
                    strID = XmlKey.GetKeyValueSting(XmlKey.KeyValue.Cctv) + cs.ID.ToString();
                    strName = cs.Name;
                    strPositionName = (cs.PositionName == null) ? "" : cs.PositionName;
                    strPoint3D = cs.X + "," + cs.Y + "," + cs.Z;
                    strBuildingID += cs.BuildingID;
                    strZoneID += cs.ZoneID;
                    strEquipZoneID += cs.EquipZoneID;
                    strType = cs.Type;
                    strUserID = cs.UserID;
                    strPassword = cs.Password;
                    strURL = cs.URL;
                    
                    if (cs.EquipZoneIDs != null && cs.EquipZoneIDs.Count > 0)
                    {
                        for (int i = 0; i < cs.EquipZoneIDs.Count; i++)
                        {
                            string strTempEquipZoneID = XmlKey.GetKeyValueSting(XmlKey.KeyValue.EquipmentZone) + cs.EquipZoneIDs[i];
                            if (strEquipZoneIDs.Length == 0)
                                strEquipZoneIDs = strTempEquipZoneID;
                            else
                                strEquipZoneIDs += ", " + strTempEquipZoneID;
                        }
                    }
                }

                XElement xFs = MakeElement(parentEle, strXName, new XAttribute("id", strID));
                MakeElement(xFs, XmlKey.XName_Name, strName);
                MakeElement(xFs, XmlKey.XName_PositionName, strPositionName);
                XElement xPos = MakeElement(xFs, XmlKey.XName_Position);
                MakeElement(xPos, XmlKey.XName_Point3D, strPoint3D);
                MakeElement(xFs, XmlKey.XName_BuildingID, strBuildingID);
                MakeElement(xFs, XmlKey.XName_ZoneID, strZoneID);
                MakeElement(xFs, XmlKey.XName_EquipZoneID, strEquipZoneID);

                if (sensor is FireSensor)
                {
                    MakeElement(xFs, XmlKey.XName_SensorSubType, strSensorSubType);
                    MakeElement(xFs, XmlKey.XName_TagNo, strTagNo);
                }
                else if (sensor is PSMSensor)
                {
                    MakeElement(xFs, XmlKey.XName_MaterialType, strMaterialType);
                    MakeElement(xFs, XmlKey.XName_UniqueKey, strUniqueKey);
                    MakeElement(xFs, XmlKey.XName_UnitName, strUnitName);
                }
                else if (sensor is EtcSensor)
                {
                    MakeElement(xFs, XmlKey.XName_MaterialType, strMaterialType);
                    MakeElement(xFs, XmlKey.XName_UniqueKey, strUniqueKey);
                    MakeElement(xFs, XmlKey.XName_UnitName, strUnitName);
                }
                else if (sensor is CCTVSensor)
                {
                    MakeElement(xFs, XmlKey.XName_EquipZoneIDs, strEquipZoneIDs);
                    MakeElement(xFs, XmlKey.XName_Type, strType);
                    MakeElement(xFs, XmlKey.XName_UserID, strUserID);
                    MakeElement(xFs, XmlKey.XName_Password, strPassword);
                    MakeElement(xFs, XmlKey.XName_Url, strURL);
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private XElement MakeSensorTypes(XElement parentEle, List<SensorType> sensorTypes)
        {
            XElement xSensorTypes = MakeElement(parentEle, XmlKey.XName_SensorTypes);

            if (sensorTypes == null)
                return xSensorTypes;
            
            foreach (SensorType type in sensorTypes)
            {                
                XElement xSensorType = MakeElement(xSensorTypes, XmlKey.XName_SensorType, new XAttribute("id", XmlKey.GetKeyValueSting(XmlKey.KeyValue.SensorType) + type.ID));
                MakeElement(xSensorType, XmlKey.XName_Name, type.Name);
            }
            
            return xSensorTypes;
        }

        private XElement MakeFireSensorSubTypes(XElement parentEle, List<SensorType> sensorTypes)
        {
            XElement xFireSensorSubTypes = MakeElement(parentEle, XmlKey.XName_FireSensorSubTypes);

            if (sensorTypes == null)
                return xFireSensorSubTypes;
            
            foreach (SensorType type in sensorTypes)
            {
                if (type.SubType == null)
                    continue;
                    
                foreach (SensorSubType subType in type.SubType)
                {
                    XElement xFireSensorSubType = MakeElement(xFireSensorSubTypes, XmlKey.XName_FireSensorSubType, new XAttribute("id", XmlKey.GetKeyValueSting(XmlKey.KeyValue.FireSensorSubType) + subType.ID));
                    MakeElement(xFireSensorSubType, XmlKey.XName_Name, subType.Name); 
                }                    
            }
            
            return xFireSensorSubTypes;
        }

        private XElement MakeGltf(XElement parentEle, List<GltfModel> gltfModels, GltfOption gltfOption)
        {
            XElement xGltf = MakeElement(parentEle, XmlKey.XName_Gltf);
            XElement xGltfModels = MakeElement(xGltf, XmlKey.XName_GltfModels);
            if (gltfModels != null && gltfModels.Count > 0)
            {
                foreach (GltfModel model in gltfModels)
                {
                    XElement xGltfModel = MakeElement(xGltfModels, XmlKey.XName_GltfModel);
                    MakeGltfModel(xGltfModel, model);
                }
            }
            //else
            //{
            //    XElement xGltfModel = MakeElement(xGltfModels, XmlKey.XName_GltfModel);
            //}

            XElement xGltfOption = MakeElement(xGltf, XmlKey.XName_GltfOption);
            if (gltfOption != null)
            {
                MakeElement(xGltfOption, XmlKey.XName_ModelBaseURL, gltfOption._3DModelBaseURL);
                MakeElement(xGltfOption, XmlKey.XName_TextureBaseURL, gltfOption._3DTextureBaseURL);
                MakeElement(xGltfOption, XmlKey.XName_IndoorModelOnMemory, gltfOption.IndoorModelOnMemory);
                MakeElement(xGltfOption, XmlKey.XName_BackgroundImage, gltfOption._3DBackgroundImage);
            }

            return xGltf;
        }

        private void MakeGltfModel(XElement parentEle, GltfModel model)
        {
            try
            {
                
                MakeElement(parentEle, XmlKey.XName_ID, model.ID);
                MakeElement(parentEle, XmlKey.XName_ModelName, model.ModelName);
                MakeElement(parentEle, XmlKey.XName_ParentID, model.ParentID);
                MakeElement(parentEle, XmlKey.XName_SiteID, model.SiteID);

                XElement xChildModels = MakeElement(parentEle, XmlKey.XName_ChildModels, "");
                if (model.ChildModels != null && model.ChildModels.Count > 0)
                {
                    foreach (GltfModel childModel in model.ChildModels)
                    {
                        XElement xChildModel = MakeElement(xChildModels, XmlKey.XName_ChildModel);
                        MakeGltfModel(xChildModel, childModel);
                    }
                }

                XElement xModelDatas = MakeElement(parentEle, XmlKey.XName_ModelDatas, "");
                if (model.ModelDatas != null && model.ModelDatas.Count > 0)
                {
                    foreach (SDMS.Model.GLTF.ModelData modelData in model.ModelDatas)
                    {
                        XElement xModelData = MakeElement(xModelDatas, XmlKey.XName_ModelData);
                        MakeElement(xModelData, XmlKey.XName_ID, modelData.ID);
                        MakeElement(xModelData, XmlKey.XName_ModelID, modelData.ModelID);
                        MakeElement(xModelData, XmlKey.XName_ModelDisplayText, modelData.ModelDisplayText);
                        MakeElement(xModelData, XmlKey.XName_ModelFile, modelData.ModelFile);
                        MakeElement(xModelData, XmlKey.XName_BuildingGroupID, modelData.BuildingGroupID);
                        MakeElement(xModelData, XmlKey.XName_BuildingID, modelData.BuildingID);
                        MakeElement(xModelData, XmlKey.XName_ZoneID, modelData.ZoneID);
                        MakeElement(xModelData, XmlKey.XName_FloorIndex, modelData.FloorIndex);
                        MakeElement(xModelData, XmlKey.XName_CameraFar, modelData.CameraFar);
                        MakeElement(xModelData, XmlKey.XName_CameraFov, modelData.CameraFov);
                        MakeElement(xModelData, XmlKey.XName_CameraNear, modelData.CameraNear);
                        MakeElement(xModelData, XmlKey.XName_CameraPosition, modelData.CameraPositionX + "," + modelData.CameraPositionY + "," + modelData.CameraPositionZ);
                        MakeElement(xModelData, XmlKey.XName_CameraQuaternion, modelData.CameraQuaternionW + "," + modelData.CameraQuaternionX + "," + modelData.CameraQuaternionY + "," + modelData.CameraQuaternionZ);
                        MakeElement(xModelData, XmlKey.XName_CameraRotation, modelData.CameraRotationX + "," + modelData.CameraRotationY + "," + modelData.CameraRotationZ);
                        MakeElement(xModelData, XmlKey.XName_OrbitTarget, modelData.OrbitTargetX + "," + modelData.OrbitTargetY + "," + modelData.OrbitTargetZ);
                    }
                }

                XElement xModelOrthoDatas = MakeElement(parentEle, XmlKey.XName_ModelOrthoDatas, "");
                if (model.ModelOrthoDatas != null && model.ModelOrthoDatas.Count > 0)
                {
                    foreach (SDMS.Model.GLTF.ModelOrthoData orthoData in model.ModelOrthoDatas)
                    {
                        XElement xModelOrthoData = MakeElement(xModelOrthoDatas, XmlKey.XName_ModelOrthoData);
                        MakeElement(xModelOrthoData, XmlKey.XName_ID, orthoData.ID);
                        MakeElement(xModelOrthoData, XmlKey.XName_ModelID, orthoData.ModelID);
                        MakeElement(xModelOrthoData, XmlKey.XName_ModelFile, orthoData.ModelFile);
                        MakeElement(xModelOrthoData, XmlKey.XName_ZoneID, orthoData.ZoneID);

                        MakeElement(xModelOrthoData, XmlKey.XName_CameraPosition, orthoData.CameraPositionX + "," + orthoData.CameraPositionY + "," + orthoData.CameraPositionZ);
                        MakeElement(xModelOrthoData, XmlKey.XName_CameraQuaternion, orthoData.CameraQuaternionW + "," + orthoData.CameraQuaternionX + "," + orthoData.CameraQuaternionY + "," + orthoData.CameraQuaternionZ);
                        MakeElement(xModelOrthoData, XmlKey.XName_CameraRotation, orthoData.CameraRotationX + "," + orthoData.CameraRotationY + "," + orthoData.CameraRotationZ);
                        MakeElement(xModelOrthoData, XmlKey.XName_Target, orthoData.TargetX + "," + orthoData.TargetY + "," + orthoData.TargetZ);
                        MakeElement(xModelOrthoData, XmlKey.XName_Zoom, orthoData.Zoom);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
