using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace VirtualSeoul
{
    public class WebServiceManager
    {
        private string BaseAddress = "open-api";
        //private const string BaseAddress = "http://39.119.118.191:8080/open-api";
        private const string Token = "a01439ed-851c-4ebb-9ed4-68e8d8037f04";

        private string m_strDoubleFormat = "F0";

        public WebServiceManager(string strURL)
        {
            if (strURL.EndsWith("/"))
                BaseAddress = strURL + BaseAddress;
            else
                BaseAddress = strURL + "/" + BaseAddress;
        }

        public bool AddLevel(string strBuildingID, Level level)
        {
            string resResult = string.Empty;
            string strURL = BaseAddress + "/spatial_level";

            string strParameter = "return-type=json";

            string strJson = "{\"targetVal\":{";
            strJson += "\"name\":" + "\"" + level.Name + "\"";
            strJson += ",\"building_id\":" + "\"" + strBuildingID + "\"";
            strJson += ",\"elevation\":" + level.Elevation.ToString();
            strJson += "}}";

            byte[] bytes = Encoding.UTF8.GetBytes(strJson);
            int len = bytes.Count();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strURL + "?" + strParameter));
            request.Method = "PUT";
            request.ContentType = "application/json; charset=utf-8";
            request.ContentLength = len + 3;
            request.Headers.Add("token", Token);
            request.Accept = "application/json";

            try
            {
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.UTF8);
                writer.Write(strJson);
                writer.Close();

                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                resResult = readerPost.ReadToEnd();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

                bool result;
                List<JObject> objList = ReadJson(resResult, out result);

                if (objList == null || objList.Count == 0)
                    return false;

                JObject obj = objList[0];

                JToken token;

                if (obj.TryGetValue("id", out token))
                {
                    string strLevelID = token.ToString();
                    level.ID = strLevelID;

                    foreach (POI poi in level.POIs)
                    {
                        if (AddPOI(strBuildingID, level.ID, poi) == false)
                            return false;
                    }

                    return result;
                }

                return false;
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("AddLevel Fail : " + ex.Message);
            }

            return false;
        }

        private string GetDoubleString(double data)
        {
            return string.Format("{0:" + m_strDoubleFormat + "}", data);
        }

        private bool AddPOI(string strBuildingID, string strLevelID, POI poi)
        {
            string resResult = string.Empty;
            string strURL = BaseAddress + "/spatial_poi";

            string strParameter = "return-type=json";

            string strJson = "{\"targetVal\":{";
            strJson += "\"type_id\":" + "\"" + poi.POIType.Code + "\"";
            strJson += ",\"name\":" + "\"" + poi.Name + "\"";
            strJson += ",\"x\":" + GetDoubleString(poi.Position.x);
            strJson += ",\"y\":" + GetDoubleString(poi.Position.y);
            strJson += ",\"z\":0";
            strJson += ",\"angle\":0";
            strJson += ",\"level_id\":" + "\"" + strLevelID + "\"";
            strJson += ",\"building_id\":" + "\"" + strBuildingID + "\"";
            strJson += ",\"alertarea_id\":" + "\"d92bc894-6f17-4b73-8b1a-53ed05e63762\"";
            strJson += "}}";

            byte[] bytes = Encoding.UTF8.GetBytes(strJson);
            int len = bytes.Count();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strURL + "?" + strParameter));
            request.Method = "PUT";
            request.ContentType = "application/json; charset=utf-8";
            request.ContentLength = len + 3;
            request.Headers.Add("token", Token);
            request.Accept = "application/json";

            try
            {
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.UTF8);
                writer.Write(strJson);
                writer.Close();

                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                resResult = readerPost.ReadToEnd();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

                bool result;
                List<JObject> objList = ReadJson(resResult, out result);

                if (objList == null || objList.Count == 0)
                    return false;

                return true;
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("AddPOI Fail : " + ex.Message);
            }

            return false;
        }

        public bool RemoveLevels(string strBuildingID, Dictionary<string, POIType> dicPOITypes)
        {
            string resResult = string.Empty;
            string strURL = BaseAddress + "/spatial_level";

            string strParameter = "return-type=json";
            strParameter += "&" + "building_id=" + strBuildingID;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strURL + "?" + strParameter));
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("token", Token);
            request.Accept = "application/json";

            try
            {
                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                resResult = readerPost.ReadToEnd();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

                bool result;
                List<JObject> objList = ReadJson(resResult, out result);

                if (objList == null)
                    return result;

                foreach (JObject obj in objList)
                {
                    JToken token;

                    if (obj.TryGetValue("id", out token))
                    {
                        string strLevelID = token.ToString();

                        List<POI> pois = SearchPOIs(strBuildingID, strLevelID, dicPOITypes);

                        foreach (POI poi in pois)
                        {
                            if (RemovePOI(strBuildingID, strLevelID, poi.ID) == false)
                                return false;
                        }

                        if (RemoveLevel(strLevelID) == false)
                            return false;
                    }
                    else
                        return false;
                }

                return true;
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("RemoveLevels Fail : " + ex.Message);
            }

            return false;
        }

        private bool RemovePOI(string strBuildingID, string strLevelID, string strPOIID)
        {
            string resResult = string.Empty;
            string strURL = BaseAddress + "/spatial_poi";

            string strParameter = "return-type=json";
            strParameter += "&id=" + strPOIID;
            strParameter += "&level_id=" + strLevelID;
            strParameter += "&building_id=" + strBuildingID;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strURL + "?" + strParameter));
            request.Method = "DELETE";
            request.ContentType = "application/json";
            request.Headers.Add("token", Token);
            request.Accept = "application/json";

            try
            {
                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                resResult = readerPost.ReadToEnd();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

                return GetResult(resResult);
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("RemovePOI Fail : " + ex.Message);
            }

            return false;
        }

        private bool RemoveLevel(string strLevelID)
        {
            string resResult = string.Empty;
            string strURL = BaseAddress + "/spatial_level";

            string strParameter = "return-type=json";
            strParameter += "&id=" + strLevelID;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strURL + "?" + strParameter));
            request.Method = "DELETE";
            request.ContentType = "application/json";
            request.Headers.Add("token", Token);
            request.Accept = "application/json";

            try
            {
                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                resResult = readerPost.ReadToEnd();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

                return GetResult(resResult);
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("RemoveLevel Fail : " + ex.Message);
            }

            return false;
        }

        public List<Level> SearchLevels(string strBuildingID, Dictionary<string, POIType> dicPOITypes)
        {
            string resResult = string.Empty;
            string strURL = BaseAddress + "/spatial_level";

            string strParameter = "return-type=json";
            strParameter += "&" + "building_id=" + strBuildingID;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strURL + "?" + strParameter));
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("token", Token);
            request.Accept = "application/json";

            try
            {
                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                resResult = readerPost.ReadToEnd();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

                bool result;
                List<JObject> objList = ReadJson(resResult, out result);

                if (objList == null || objList.Count == 0)
                    return null;

                List<Level> levels = new List<Level>();

                foreach (JObject obj in objList)
                {
                    JToken token;
                    Level level = new Level();

                    if (obj.TryGetValue("id", out token))
                        level.ID = token.ToString();
                    else
                        return null;

                    if (obj.TryGetValue("elevation", out token))
                    {
                        int nElevation;
                        string strElevation = token.ToString();

                        if (int.TryParse(strElevation, out nElevation) == false)
                            return null;

                        level.Elevation = nElevation;
                    }
                    else
                        return null;

                    List<POI> poiTypes = SearchPOIs(strBuildingID, level.ID, dicPOITypes);

                    if (poiTypes == null)
                        return null;

                    level.POIs.AddRange(poiTypes);
                    levels.Add(level);
                }

                levels.Sort();

                int nFirstIndex = -1;
                Level levelPrev = null;

                for (int i=0;i<levels.Count;i++)
                {
                    Level level = levels[i];

                    if (nFirstIndex < 0)
                    {
                        if (level.Elevation == 0)
                        {
                            nFirstIndex = i;
                            levelPrev = level;
                            level.FloorIndex = 0;
                        }
                    }
                    else
                    {
                        level.FloorIndex = levelPrev.FloorIndex + 1;
                        levelPrev.Height = level.Elevation - levelPrev.Elevation;

                        if (i == levels.Count - 1)
                            level.Height = levelPrev.Height;

                        levelPrev = level;
                    }
                }

                if (nFirstIndex >= 0)
                {
                    levelPrev = levels[nFirstIndex];

                    for (int i = nFirstIndex - 1; i >= 0; i--)
                    {
                        Level level = levels[i];
                        level.Height = levelPrev.Elevation - level.Elevation;
                        level.FloorIndex = levelPrev.FloorIndex - 1;
                        levelPrev = level;
                    }
                }

                return levels;
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("SearchLevels Fail : " + ex.Message);
            }

            return null;
        }

        public List<POI> SearchPOIs(string strBuildingID, string strLevelID, Dictionary<string, POIType> dicPOITypes)
        {
            string resResult = string.Empty;
            string strURL = BaseAddress + "/spatial_poi";

            string strParameter = "return-type=json";
            strParameter += "&" + "building_id=" + strBuildingID;
            strParameter += "&" + "level_id=" + strLevelID;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strURL + "?" + strParameter));
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("token", Token);
            request.Accept = "application/json";

            try
            {
                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                resResult = readerPost.ReadToEnd();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

                bool result;
                List<JObject> objList = ReadJson(resResult, out result);

                if (objList == null || objList.Count == 0)
                {
                    if (result == false)
                        return null;
                    else
                        return new List<POI>();
                }

                List<POI> pois = new List<POI>();

                foreach (JObject obj in objList)
                {
                    JToken token;
                    POI poi = new POI();

                    if (obj.TryGetValue("id", out token))
                        poi.ID = token.ToString();
                    else
                        return null;

                    if (obj.TryGetValue("name", out token))
                        poi.Name = token.ToString();
                    else
                        return null;

                    if (obj.TryGetValue("x", out token))
                    {
                        double x;
                        string strX = token.ToString();

                        if (double.TryParse(strX, out x) == false)
                            return null;

                        if (poi.Position == null)
                            poi.Position = new UnE.Geometry.Vertex2D();

                        poi.Position.x = x;
                    }
                    else
                        return null;

                    if (obj.TryGetValue("y", out token))
                    {
                        double y;
                        string strY = token.ToString();

                        if (double.TryParse(strY, out y) == false)
                            return null;

                        if (poi.Position == null)
                            poi.Position = new UnE.Geometry.Vertex2D();

                        poi.Position.y = y;
                    }
                    else
                        return null;

                    if (obj.TryGetValue("type_id", out token))
                    {
                        POIType poiType = null;
                        string strCode = token.ToString();

                        if (dicPOITypes.TryGetValue(strCode, out poiType) == false)
                            return null;

                        poi.POIType = poiType;
                    }
                    else
                        return null;

                    POI _poi = poi.POIType.MakePOI(poi.Position);

                    if (_poi != null)
                    {
                        poi.Shapes.AddRange(_poi.Shapes);

                        poi.TL = poi.POIType.TL + poi.Position;
                        poi.BL = poi.POIType.BL + poi.Position;
                        poi.BR = poi.POIType.BR + poi.Position;
                        poi.SetShapePosition();
                    }

                    pois.Add(poi);
                }

                return pois;
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("SearchLevels Fail : " + ex.Message);
            }

            return null;
        }

        public bool SearchBuilding(string strBuildingID)
        {
            string resResult = string.Empty;
            string strURL = BaseAddress + "/spatial_project";

            string strParameter = "return-type=json";
            strParameter += "&" + "id=" + strBuildingID;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(strURL + "?" + strParameter));
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("token", Token);
            request.Accept = "application/json";

            try
            {
                HttpWebResponse wRes = (HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                resResult = readerPost.ReadToEnd();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

                bool result;
                List<JObject> objList = ReadJson(resResult, out result);

                if (objList == null || objList.Count == 0)
                    return false;

                JObject obj = objList[0];

                JToken token;

                if (obj.TryGetValue("name", out token))
                {
                    string strBuildingName = token.ToString();
                    System.Diagnostics.Trace.WriteLine("SearchBuilding Result : " + strBuildingName);
                    return result;
                }

                return false;
            }
            catch (System.Net.WebException ex)
            {
                System.Diagnostics.Trace.WriteLine("SearchBuilding Fail : " + ex.Message);
            }

            return false;
        }

        private bool GetResult(string strJson)
        {
            JObject obj = JObject.Parse(strJson);
            JToken token;

            if (obj.TryGetValue("result", out token))
            {
                bool result = token.Value<bool>();
                return result;
            }

            return false;
        }

        private List<JObject> ReadJson(string strJson, out bool result)
        {
            List<JObject> objectList = new List<JObject>();
            result = false;

            JObject obj = JObject.Parse(strJson);
            JToken token;

            if (obj.TryGetValue("result", out token))
            {
                result = token.Value<bool>();

                if (result == false)
                    return null;

                if (obj.TryGetValue("data", out token))
                {
                    try
                    {
                        int nChildCount = token.Count();

                        for (int i = 0; i < nChildCount; i++)
                        {
                            JToken _token = token.ElementAt(i);

                            string strToken = _token.ToString();
                            string str = GetJson(strToken);

                            JObject data = JObject.Parse(str);
                            objectList.Add(data);
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Trace.WriteLine(e.Message);
                    }

                    return objectList;

                    /*string strToken = token.ToString();
                    string str = GetJson(strToken);

                    try
                    {
                        JObject data = JObject.Parse(str);
                        return data;
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Trace.WriteLine(e.Message);
                    }*/
                }
            }

            return null;
        }

        private string GetJson(string str)
        {
            if (str.StartsWith("["))
            {
                str = str.Substring(1, str.Length - 2).Trim();
            }
            else if (str.StartsWith("{"))
                return str;
            else
            {
                str = "{" + str + "}";
            }

            return str;
        }
    }
}
