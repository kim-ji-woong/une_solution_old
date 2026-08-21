using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartCity.BLL.Models.Request;
using SmartCity.BLL.Models.Response;
using System;
using System.Collections.Generic;

namespace CrisisAlertWeb.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private global::SmartCity.BLL.ProcessManager m_processManager = null;
        public AccountController(global::SmartCity.IDAL.IDataManager dataManager)
        {
            m_processManager = new global::SmartCity.BLL.ProcessManager(dataManager);
        }

        private class AesHelper
        {
            private const int KeySize = 32;

            private static char[] BaseArr = MakeBaseArray();

            private static char[] MakeBaseArray()
            {
                char[] arr = new char[62];
                int i = 0;

                for (char ch = '0'; ch <= '9'; ch++)
                {
                    arr[i++] = ch;
                }

                for (char ch = 'a'; ch <= 'z'; ch++)
                {
                    arr[i++] = ch;
                }

                for (char ch = 'A'; ch <= 'Z'; ch++)
                {
                    arr[i++] = ch;
                }

                return arr;
            }

            public static string MakeRandomKey(long? num)
            {
                string strKey = "";
                int max = BaseArr.Length - 1;

                int seed = num == null ? DateTime.Now.GetHashCode() : (int)num;
                Random rand = new Random(seed);

                for (int i = 0; i < KeySize; i++)
                {
                    int nIndex = rand.Next(max);
                    strKey += BaseArr[nIndex];
                }

                return strKey;
            }

            /// <summary>  
            /// AES encryption algorithm  
            /// </summary>  
            /// <param name="input">plain string</param>  
            /// <param name="key">key (32 bit)</param>  

            public static string Encrypt(string input, string key)
            {
                byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key.Substring(0, 32));
                using (System.Security.Cryptography.AesCryptoServiceProvider aesAlg = new System.Security.Cryptography.AesCryptoServiceProvider())
                {
                    aesAlg.Key = keyBytes;
                    aesAlg.IV = System.Text.Encoding.UTF8.GetBytes(key.Substring(0, 16));

                    System.Security.Cryptography.ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                    using (System.IO.MemoryStream msEncrypt = new System.IO.MemoryStream())
                    {
                        using (System.Security.Cryptography.CryptoStream csEncrypt = new System.Security.Cryptography.CryptoStream(msEncrypt, encryptor, System.Security.Cryptography.CryptoStreamMode.Write))
                        {
                            using (System.IO.StreamWriter swEncrypt = new System.IO.StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(input);
                            }
                            byte[] bytes = msEncrypt.ToArray();
                            return ByteArrayToHexString(bytes);
                        }
                    }
                }
            }

            /// <summary>  
            /// AES decryption  
            /// </summary>  
            /// <param name="input"> ciphertext byte array</param>  
            /// <param name="key">key (32 bit)</param>  
            /// <returns> returns the decrypted string</returns>  
            public static string Decrypt(string input, string key)
            {
                byte[] inputBytes = HexStringToByteArray(input);
                byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(key.Substring(0, 32));
                using (System.Security.Cryptography.AesCryptoServiceProvider aesAlg = new System.Security.Cryptography.AesCryptoServiceProvider())
                {
                    aesAlg.Key = keyBytes;
                    aesAlg.IV = System.Text.Encoding.UTF8.GetBytes(key.Substring(0, 16));

                    System.Security.Cryptography.ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (System.IO.MemoryStream msEncrypt = new System.IO.MemoryStream(inputBytes))
                    {
                        using (System.Security.Cryptography.CryptoStream csEncrypt = new System.Security.Cryptography.CryptoStream(msEncrypt, decryptor, System.Security.Cryptography.CryptoStreamMode.Read))
                        {
                            using (System.IO.StreamReader srEncrypt = new System.IO.StreamReader(csEncrypt))
                            {
                                return srEncrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }

            public static string GetHashCode(string input)
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(input);
                byte[] hashed = System.Security.Cryptography.SHA256.Create().ComputeHash(bytes);

                string strHashed = "";

                foreach (byte b in hashed)
                {
                    strHashed += string.Format("{0:x2}", b);
                }

                return strHashed;
            }

            /// <summary>
            /// Convert the specified hex string to a byte array
            /// </summary>
            /// <param name="s">hexadecimal string (eg "7F 2C 4A" or "7F2C4A")</param>
            /// <returns>byte array corresponding to hexadecimal string</returns>
            public static byte[] HexStringToByteArray(string s)
            {
                s = s.Replace(" ", "");
                byte[] buffer = new byte[s.Length / 2];
                for (int i = 0; i < s.Length; i += 2)
                    buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
                return buffer;
            }

            /// <summary>
            /// Convert a byte array into a formatted hex string
            /// </summary>
            /// <param name="data">byte array</param>
            /// <returns> formatted hexadecimal string</returns>
            public static string ByteArrayToHexString(byte[] data)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(data.Length * 3);
                foreach (byte b in data)
                {
                    //hexadecimal number
                    sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
                    //16 digits separated by spaces
                    //sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
                }
                return sb.ToString().ToUpper();
            }
        }

        // GET api/Account/5
        [HttpGet("{num}")]
        public string Get(long num)
        {
            string strKey = AesHelper.MakeRandomKey(num);

            return strKey;
        }

        // POST api/Account
        [HttpPost]
        public IActionResult Post([FromBody] RequestData data)
        {
            if (data == null)
                return BadRequest();

            if (data.RequestLogin != null)
                return RequestLogin(data.RequestLogin);
            else if (data.RequestSessionLogin != null)
                return RequestSessionLogin(data.RequestSessionLogin);
            else if (data.RequestChangePassword != null)
                return RequestChangePassword(data.RequestChangePassword);
            else if (data.RequestLogout != null)
                return RequestLogout(data.RequestLogout);
            else if (data.RequestCheckUserID != null)
                return RequestCheckUserID(data.RequestCheckUserID);
            else if (data.RequestCheckCode != null)
                return RequestCheckCode(data.RequestCheckCode);
            else if (data.RequestPWDFind != null)
                return RequestPWDFind(data.RequestPWDFind);

            return BadRequest();
        }

        private IActionResult RequestLogin(RequestLogin data)
        {
            ResponseLogin result = null;

            if (data.Value != "" || data.Key != "")
            {
                try
                {
                    string str = AesHelper.Decrypt(data.Value, data.Key);

                    int nIndex = str.IndexOf('|');

                    if (nIndex > 0)
                    {
                        string strID = str.Substring(0, nIndex).Trim();
                        string strPW = str.Substring(nIndex + 1).Trim();

                        result = m_processManager.GetAccountManager().Login(strID, strPW, data.Key);

                        if (result.Success)
                        {
                            //Login(strID, strPW, data.Key) 메소드에서 이미 result 값을 지정하고 있음.
                        }
                    }
                    else 
                    {
                        result = new ResponseLogin();
                        result.Success = false;
                    }
                }
                catch (Exception e)
                {
                    result = new ResponseLogin();
                    result.Message = e.Message;
                    result.Success = false;
                }
            }
            else
            {
                result = new ResponseLogin();
                result.Success = false;
            }

            return Ok(result);
        }

        private IActionResult RequestSessionLogin(RequestSessionLogin data)
        {
            ResponseLogin result = null;

            if (data.Key != null)
            {
                try
                {
                    string strKey = data.Key.Trim();
                    result = m_processManager.GetAccountManager().SessionLogin(strKey);
                }
                catch (Exception e)
                {
                    result = new ResponseLogin();
                    result.Message = e.Message;
                    result.Success = false;
                }
            }
            else
            {
                result = new ResponseLogin();
                result.Success = false;
            }

            return Ok(result);
        }


        private IActionResult RequestChangePassword(RequestChangePassword data)
        {
            MessageResult result = null;

            if (data.Value != "" || data.Key != "")
            {
                try
                {
                    string str = AesHelper.Decrypt(data.Value, data.Key);

                    int nIndex = str.IndexOf('|');

                    if (nIndex > 0)
                    {
                        string strID = str.Substring(0, nIndex).Trim();
                        int nID = Int32.Parse(strID);
                        string strPW = str.Substring(nIndex + 1).Trim();

                        nIndex = strPW.IndexOf('|');

                        string strOldPW = strPW.Substring(0, nIndex).Trim();
                        string strNewPW = strPW.Substring(nIndex + 1).Trim();

                        result = m_processManager.GetAccountManager().ChangePassword(nID, strOldPW, strNewPW);

                        if (result.Success)
                        {
                            // ChangePassword 메소드에서 이미 result 값을 지정하고 있음.
                        }
                    }
                    else
                    {
                        result = new MessageResult();
                        result.Success = false;
                    }
                }
                catch (Exception e)
                {
                    result = new MessageResult();
                    result.Message = e.Message;
                    result.Success = false;
                }
            }
            else
            {
                result = new MessageResult();
                result.Success = false;
            }

            return Ok(result);
        }

        private IActionResult RequestLogout(RequestLogout data)
        {
            MessageResult result = null;

            if (data.Key != null)
            {
                try
                {
                    string strKey = data.Key.Trim();
                    result = m_processManager.GetAccountManager().Logout(strKey);
                }
                catch (Exception e)
                {
                    result = new MessageResult();
                    result.Message = e.Message;
                    result.Success = false;
                }
            }
            else
            {
                result = new MessageResult();
                result.Success = false;
            }

            return Ok(result);
        }

        private IActionResult RequestCheckUserID(RequestCheckUserID data)
        {
            MessageResult result = null;

            if (data.UserID != null)
            {
                try
                {
                    string strUserID = data.UserID.Trim();
                    result = m_processManager.GetAccountManager().CheckUserID(strUserID);
                }
                catch (Exception e)
                {
                    result = new MessageResult();
                    result.Message = e.Message;
                    result.Success = false;
                }
            }
            else
            {
                result = new MessageResult();
                result.Success = false;
            }

            return Ok(result);
        }

        private IActionResult RequestCheckCode(RequestCheckCode data)
        {
            MessageResult result = null;

            if (data.Value != "" || data.Key != "")
            {
                try
                {
                    string strCode = AesHelper.Decrypt(data.Value, data.Key);

                    result = m_processManager.GetAccountManager().CheckCode(strCode);
                }
                catch (Exception e)
                {
                    result = new ResponseLogin();
                    result.Message = e.Message;
                    result.Success = false;
                }
            }
            else
            {
                result = new ResponseLogin();
                result.Success = false;
            }

            return Ok(result);
        }

        private IActionResult RequestPWDFind(RequestPWDFind data)
        {
            MessageResult result = null;

            if (data.Value != "" || data.Key != "")
            {
                try
                {
                    string str = AesHelper.Decrypt(data.Value, data.Key);

                    int nIndex = str.IndexOf('|');

                    if (nIndex > 0)
                    {
                        string strID = str.Substring(0, nIndex).Trim();
                        string strPW = str.Substring(nIndex + 1).Trim();

                        result = m_processManager.GetAccountManager().PWDFind(strID, strPW);
                    }
                    else
                    {
                        result = new ResponseLogin();
                        result.Success = false;
                    }
                }
                catch (Exception e)
                {
                    result = new ResponseLogin();
                    result.Message = e.Message;
                    result.Success = false;
                }
            }
            else
            {
                result = new ResponseLogin();
                result.Success = false;
            }

            return Ok(result);
        }
    }

    

}
