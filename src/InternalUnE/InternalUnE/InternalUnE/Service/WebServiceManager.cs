using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Vacation.BLL.Models.Account;
using Vacation.BLL.Models;

namespace InternalUnE.Service
{
    using Data;

    public static class WebServiceManager
    {
        public static ExternalLoginResult ExternalLogin(string strUserID, string strHashCode, string strExternalURL)
        {
            JObject jsonData = new JObject();

            jsonData.Add("userID", strUserID);
            jsonData.Add("hashCode", strHashCode);

            JObject json = new JObject();
            json.Add("externalLogin", jsonData);

            string strJson = json.ToString();

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strJson);
            int len = bytes.Length;

            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(new Uri(strExternalURL));
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.ContentLength = len + 3;

            ExternalLoginResult result = null;
            string strResult = "";

            try
            {
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
                writer.Write(strJson);
                writer.Close();

                System.Net.HttpWebResponse wRes = (System.Net.HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                strResult = readerPost.ReadToEnd().Trim();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

                string strErrorMessage = null;
                string strUserName, strTeamName, strLoginKey;
                bool success = GetJsonResult(JObject.Parse(strResult), out strUserID, out strUserName, out strTeamName, out strLoginKey, out strErrorMessage);

                if (success == false)
                    result = new ExternalLoginResult(false, strErrorMessage);
                else
                {
                    result = new ExternalLoginResult(true, "");

                    result.Name = strUserName;
                    result.UserID = strUserID;
                    result.TeamName = strTeamName;
                    result.LoginKey = strLoginKey;
                }
            }
            catch (System.Net.WebException ex)
            {
                result = new ExternalLoginResult(false, ex.Message);
            }

            return result;
        }

        public static ExternalLoginResult RequestNewLoginKey(string strBeginCode, string strExternalURL)
        {
            JObject jsonData = new JObject();

            jsonData.Add("beginCode", strBeginCode);

            JObject json = new JObject();
            json.Add("requestNewLoginKey", jsonData);

            string strJson = json.ToString();

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strJson);
            int len = bytes.Length;

            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(new Uri(strExternalURL));
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.ContentLength = len + 3;

            ExternalLoginResult result = null;
            string strResult = "";

            try
            {
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
                writer.Write(strJson);
                writer.Close();

                System.Net.HttpWebResponse wRes = (System.Net.HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                strResult = readerPost.ReadToEnd().Trim();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

                string strErrorMessage = null;
                string strUserName, strTeamName, strLoginKey, strUserID;
                bool success = GetJsonResult(JObject.Parse(strResult), out strUserID, out strUserName, out strTeamName, out strLoginKey, out strErrorMessage);

                if (success == false)
                    result = new ExternalLoginResult(false, strErrorMessage);
                else
                {
                    result = new ExternalLoginResult(true, "");

                    result.Name = strUserName;
                    result.UserID = strUserID;
                    result.TeamName = strTeamName;
                    result.LoginKey = strLoginKey;
                }
            }
            catch (System.Net.WebException ex)
            {
                result = new ExternalLoginResult(false, ex.Message);
            }

            return result;
        }

        public static RegisterResult Regist(RegisterData data, string strExternalURL)
        {
            JObject jsonData = new JObject();

            jsonData.Add("name", data.Name);
            jsonData.Add("email", data.Email);
            jsonData.Add("returnUrl", data.ReturnUrl);

            JObject json = new JObject();
            json.Add("register", jsonData);

            string strJson = json.ToString();

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strJson);
            int len = bytes.Length;

            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(new Uri(strExternalURL));
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.ContentLength = len + 3;

            RegisterResult result = new RegisterResult();
            string strResult = "";

            try
            {
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
                writer.Write(strJson);
                writer.Close();

                System.Net.HttpWebResponse wRes = (System.Net.HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                strResult = readerPost.ReadToEnd().Trim();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

                string strErrorMessage = null;
                bool success = GetJsonMessageResult(JObject.Parse(strResult), out strErrorMessage);

                if (success == false)
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                }
                else
                {
                    result.Success = true;
                    result.Message = "";
                }
            }
            catch (System.Net.WebException ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return result;
        }

        public static RegisterParamResult CheckRegisterParam(string strValue, string strExternalURL)
        {
            JObject jsonData = new JObject();

            jsonData.Add("value", strValue);

            JObject json = new JObject();
            json.Add("registerParam", jsonData);

            string strJson = json.ToString();

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strJson);
            int len = bytes.Length;

            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(new Uri(strExternalURL));
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.ContentLength = len + 3;

            RegisterParamResult result = new RegisterParamResult();
            string strResult = "";

            try
            {
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
                writer.Write(strJson);
                writer.Close();

                System.Net.HttpWebResponse wRes = (System.Net.HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                strResult = readerPost.ReadToEnd().Trim();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

                string strErrorMessage = null;
                string strName, strLevel, strUserID;
                bool success = GetJsonResult2(JObject.Parse(strResult), out strName, out strLevel, out strUserID, out strErrorMessage);

                if (success == false)
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                }
                else
                {
                    result.Success = true;
                    result.Message = "";

                    result.Name = strName;
                    result.Level = strLevel;
                    result.UserID = strUserID;
                }
            }
            catch (System.Net.WebException ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return result;
        }

        public static MessageResult SetPassword(string strValue, string strExternalURL)
        {
            JObject jsonData = new JObject();

            jsonData.Add("value", strValue);

            JObject json = new JObject();
            json.Add("registerPassword", jsonData);

            string strJson = json.ToString();

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strJson);
            int len = bytes.Length;

            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(new Uri(strExternalURL));
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.ContentLength = len + 3;

            MessageResult result = new MessageResult();
            string strResult = "";

            try
            {
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
                writer.Write(strJson);
                writer.Close();

                System.Net.HttpWebResponse wRes = (System.Net.HttpWebResponse)request.GetResponse();

                Stream respPostStream = wRes.GetResponseStream();
                StreamReader readerPost = new StreamReader(respPostStream, System.Text.Encoding.UTF8);

                strResult = readerPost.ReadToEnd().Trim();
                request.Abort();
                readerPost.Close();
                respPostStream.Close();

                string strErrorMessage = null;
                bool success = GetJsonMessageResult(JObject.Parse(strResult), out strErrorMessage);

                if (success == false)
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                }
                else
                {
                    result.Success = true;
                    result.Message = "";
                }
            }
            catch (System.Net.WebException ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }

            return result;
        }

        private static bool GetJsonResult(JObject json, out string strUserID, out string strUserName, out string strTeamName, out string strLoginKey, out string strErrorMessage)
        {
            strUserID = strUserName = strTeamName = null;
            strLoginKey = null;
            strErrorMessage = null;

            if (json == null)
                return false;

            JToken tokenName = json.GetValue("name");
            JToken tokenUserID = json.GetValue("userID");
            JToken tokenTeamName = json.GetValue("teamName");
            JToken tokenLoginKey = json.GetValue("loginKey");
            JToken tokenMessage = json.GetValue("message");
            JToken tokenSuccess = json.GetValue("success");

            if (tokenMessage != null)
                strErrorMessage = tokenMessage.Value<string>();

            if (tokenName == null || tokenUserID == null || tokenLoginKey == null)
                return false;

            strUserID = tokenUserID.Value<string>();
            strUserName = tokenName.Value<string>();
            strLoginKey = tokenLoginKey.Value<string>();

            if (tokenTeamName != null)
                strTeamName = tokenTeamName.Value<string>();

            if (tokenSuccess != null)
            {
                string strSuccess = tokenSuccess.Value<string>().ToLower();

                if (strSuccess == "true")
                    return true;
            }

            return false;
        }

        private static bool GetJsonResult2(JObject json, out string strName, out string strLevel, out string strUserID, out string strErrorMessage)
        {
            strUserID = strName = strLevel = null;
            strErrorMessage = null;

            if (json == null)
                return false;

            JToken tokenName = json.GetValue("name");
            JToken tokenLevel = json.GetValue("level");
            JToken tokenUserID = json.GetValue("userID");
            JToken tokenMessage = json.GetValue("message");
            JToken tokenSuccess = json.GetValue("success");

            if (tokenMessage != null)
                strErrorMessage = tokenMessage.Value<string>();

            if (tokenName == null || tokenUserID == null || tokenLevel == null)
                return false;

            strUserID = tokenUserID.Value<string>();
            strName = tokenName.Value<string>();
            strLevel = tokenLevel.Value<string>();

            if (tokenSuccess != null)
            {
                string strSuccess = tokenSuccess.Value<string>().ToLower();

                if (strSuccess == "true")
                    return true;
            }

            return false;
        }

        private static bool GetJsonMessageResult(JObject json, out string strErrorMessage)
        {
            strErrorMessage = null;

            if (json == null)
                return false;

            JToken tokenMessage = json.GetValue("message");
            JToken tokenSuccess = json.GetValue("success");

            if (tokenMessage != null)
                strErrorMessage = tokenMessage.Value<string>();

            if (tokenSuccess != null)
            {
                string strSuccess = tokenSuccess.Value<string>().ToLower();

                if (strSuccess == "true")
                    return true;
            }

            return false;
        }
    }
}
