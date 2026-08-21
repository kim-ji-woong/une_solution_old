using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Http;

namespace SensorMaker.Areas.Account.Controllers
{
    using BLL;
    using BLL.Models;
    using BLL.Models.Account;
    using SensorMaker.BLL.Models.Response;
    using Service;

    [Area("Account")]
    public class AccountController : Controller
    {
        // DBUtil의 AES 클래스와 다른 알고리즘을 사용한다.
        // javascript와 호환하기 위한 클래스
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

        private const string LoginKey = "LoginKey";
        private const string LoginUser = "LoginUser";

        private OptionManager m_optManager = null;
        private ProcessManager m_processManager = null;

        public AccountController(TeamEditor.IDAL.IDataManager dataManager, Common.IDAL.IDataManager commonDataManager, OptionManager optionManager, global::SDMS.IDAL.IDataManager sdmsDataManager, SOPManager.IDAL.IDataManager sopDataManager)
        {
            m_optManager = optionManager;
            m_processManager = new ProcessManager(dataManager, commonDataManager, sdmsDataManager, sopDataManager);
        }

        // GET Account/Account/Get
        [HttpGet]
        public string Get(long num)
        {
            string strKey = AesHelper.MakeRandomKey(num);
            SessionManager.SetData(HttpContext.Session, LoginKey, strKey);
            return strKey;
        }

        // POST api/<AccountController>
        [HttpPost]
        public IActionResult RequestData([FromBody] AccountData data)
        {
            if (data.Login != null)
                return Login(data.Login);
            else if (data.Logout != null)
                return Logout(data.Logout);
            else if (data.AutoLogin != null)
                return AutoLogin(data.AutoLogin);
            else if (data.CurrentUser != null)
                return CurrentUser();
            else if (data.Register != null)
                return Regist(data.Register);
            else if (data.RequestRegist != null)
                return RequestRegist(data.RequestRegist);
            /*else if (data.RegisterParam != null)
                return CheckRegisterParam(data.RegisterParam);
            else if (data.RegisterPassword != null)
                return SetPassword(data.RegisterPassword);*/

            return BadRequest();
        }

        private IActionResult Login(LoginData data)
        {
            if (SessionManager.HasData(HttpContext.Session, LoginUser))
                return BadRequest();

            string strKey = "";
            LoginResult result = null;

            SessionManager.SetData(HttpContext.Session, LoginUser, null);

            if (SessionManager.TryGetData<string>(HttpContext.Session, LoginKey, ref strKey))
            {
                try
                {
                    string str = AesHelper.Decrypt(data.Value, strKey);

                    int nIndex = str.IndexOf('|');

                    if (nIndex > 0)
                    {
                        string strID = str.Substring(0, nIndex).Trim();
                        string strPW = str.Substring(nIndex + 1).Trim();

                        result = m_processManager.GetAccountManager().Login(strID, strPW, Startup.TempResourceRootPath, Startup.ResourceRootPath, m_optManager.ExternalLogin);

                        if (result.Success)
                        {
                            result.Message = "로그인에 성공하였습니다.";
                            SessionManager.SetData(HttpContext.Session, LoginUser, result.User);
                        }
                    }
                    else
                    {
                        result = new LoginResult();
                        result.Message = "비밀번호 등록에 실패하였습니다.";
                        result.Success = false;
                    }
                }
                catch (Exception e)
                {
                    result = new LoginResult();
                    result.Message = e.Message;
                    result.Success = false;
                }
            }
            else
            {
                result = new LoginResult();
                result.Success = false;
            }

            return Ok(result);
        }

        private IActionResult Logout(LogoutData data)
        {
            SessionManager.SetData(HttpContext.Session, LoginUser, null);

            LogoutResult result = new LogoutResult();
            result.Success = true;
            return Ok(result);
        }

        private IActionResult AutoLogin(AutoLoginData data)
        {
            LoginResult result = m_processManager.GetAccountManager().AutoLogin(data.BeginCode, Startup.TempResourceRootPath, Startup.ResourceRootPath, m_optManager.ExternalLogin);

            if (result.Success)
            {
                result.Message = "로그인에 성공하였습니다.";
                SessionManager.SetData(HttpContext.Session, LoginUser, result.User);
            }

            return Ok(result);
        }

        private IActionResult CurrentUser()
        {
            ApplicationUser user = null;

            if (SessionManager.TryGetData<ApplicationUser>(HttpContext.Session, LoginUser, ref user))
            {
                LoginResult result = new LoginResult();
                result.User = user;
                result.Success = true;
                return Ok(result);
            }

            return Ok(new ApplicationUser());
        }

        private IActionResult Regist(RegisterData data)
        {
            if (SessionManager.HasData(HttpContext.Session, LoginUser))
                return BadRequest();

            MessageResult result = new MessageResult();

            if (data == null)
            {
                result.Message = "잘못된 데이터 형식입니다.";
            }
            else
            {
                string strErrorMessage;
                string strPW = GetPassword(data.Password, out strErrorMessage);

                if (strPW == null)
                {
                    result.Success = false;
                    result.Message = strErrorMessage;
                    return Ok(result);
                }
                else
                    data.Password = strPW;

                string strID = data.IsValidEmail(out strErrorMessage);

                if (strID == null || strErrorMessage != null)
                {
                    result.Message = strErrorMessage;
                }
                else
                {
                    string strSystemMail, strSystemCode, strAdminMail, strSiteURL;
                    IActionResult actionResult = GetSystemOptions(out strSystemMail, out strSystemCode, out strAdminMail, out strSiteURL);

                    if (actionResult != null)
                        return actionResult;

                    result = RegistData(data, strID, strSystemMail, strAdminMail, strSystemCode, strSiteURL, m_optManager.SolutionName);
                }
            }

            return Ok(result);
        }

        private IActionResult GetSystemOptions(out string strSystemMail, out string strSystemCode, out string strAdminMail, out string strSiteURL)
        {
            strSystemMail = m_optManager.SystemMail;
            strSystemCode = m_optManager.SystemCode;
            strAdminMail = m_optManager.AdminMail;
            strSiteURL = m_optManager.SiteURL;

            MessageResult result = null;

            if (strSystemMail == null || strSystemMail.Length == 0)
            {
                result = new MessageResult(false, "시스템 메일계정을 찾을수 없습니다.");
                return Ok(result);
            }

            if (strAdminMail == null || strAdminMail.Length == 0)
            {
                result = new MessageResult(false, "관리자 메일계정을 찾을수 없습니다.");
                return Ok(result);
            }

            if (strSystemCode == null || strSystemCode.Length == 0)
            {
                result = new MessageResult(false, "시스템 코드를 찾을수 없습니다.");
                return Ok(result);
            }

            if (strSiteURL == null || strSiteURL.Length == 0)
            {
                result = new MessageResult(false, "SiteURL을 찾을수 없습니다.");
                return Ok(result);
            }

            return null;
        }

        private RegisterResult RegistData(RegisterData data, string strUserID, string strSystemMail, string strAdminMail, string strSystemCode, string strSiteURL, string strSolutionName)
        {
            AccountManager mgr = m_processManager.GetAccountManager();

            string strHost = this.Request.Host.ToString();

            if (strHost.EndsWith("/") == false)
                strHost += "/";

            string strURL = this.Request.Scheme + "://" + strHost + "Account/Regist";
            return mgr.SendRegistEmail(strSystemMail, strAdminMail, strSystemCode, strSiteURL, data, strURL, strSolutionName);
        }

        private IActionResult RequestRegist(RequestRegist data)
        {
            string strSystemMail, strSystemCode, strAdminMail, strSiteURL;
            IActionResult actionResult = GetSystemOptions(out strSystemMail, out strSystemCode, out strAdminMail, out strSiteURL);

            if (actionResult != null)
                return actionResult;

            LoginResult result = m_processManager.GetAccountManager().UpdateMemberRegist(data, strSiteURL, m_optManager.SolutionName, strSystemMail, strSystemCode);
            return Ok(result);
        }

        /*private IActionResult CheckRegisterParam(RegisterParam param)
        {
            string strCode = HttpUtility.UrlDecode(param.Value);
            string strUserName, strJobLevel, strUserID, strErrorMessage;

            bool success = m_processManager.GetAccountManager().CheckRegisterParam(strCode, out strUserName, out strJobLevel, out strUserID, out strErrorMessage);

            RegisterParamResult result = new RegisterParamResult();

            result.Success = success;
            result.Message = strErrorMessage;
            result.Name = strUserName;
            result.UserID = strUserID;
            result.Level = strJobLevel;

            return Ok(result);
        }

        private IActionResult SetPassword(LoginData data)
        {
            string strKey = "";
            Vacation.BLL.Models.MessageResult result = new Vacation.BLL.Models.MessageResult();

            if (SessionManager.TryGetData<string>(HttpContext.Session, LoginKey, ref strKey))
            //if (HttpContext.Session.TryGetValue(LoginKey, out bytes))
            {
                try
                {
                    string str = AesHelper.Decrypt(data.Value, strKey);

                    int nIndex = str.IndexOf('|');

                    if (nIndex > 0)
                    {
                        string strID = str.Substring(0, nIndex).Trim();
                        string strPW = str.Substring(nIndex + 1).Trim();
                        string strErrorMessage;

                        bool success = m_processManager.GetAccountManager().SetPassword(strID, strPW, out strErrorMessage);
                        result.Message = strErrorMessage;
                        result.Success = success;
                    }
                    else
                    {
                        result.Message = "비밀번호 등록에 실패하였습니다.";
                        result.Success = false;
                    }
                }
                catch (Exception e)
                {
                    result.Message = e.Message;
                    result.Success = false;
                }
            }

            return Ok(result);
        }*/

        private string GetPassword(string strValue, out string strErrorMessage)
        {
            strErrorMessage = null;

            string strKey = "";
            SessionManager.SetData(HttpContext.Session, LoginUser, null);

            if (SessionManager.TryGetData<string>(HttpContext.Session, LoginKey, ref strKey))
            {
                try
                {
                    string str = AesHelper.Decrypt(strValue, strKey);

                    int nIndex = str.IndexOf('|');

                    if (nIndex > 0)
                    {
                        string strPW = str.Substring(nIndex + 1).Trim();
                        return strPW;
                    }
                    else
                    {
                        strErrorMessage = "잘못된 비밀번호입니다.";
                    }
                }
                catch (Exception e)
                {
                    strErrorMessage = e.Message;
                    System.Diagnostics.Trace.WriteLine(e.Message);
                }
            }
            else
            {
                strErrorMessage = "세션이 만료되었습니다.";
            }

            return null;
        }
    }
}
