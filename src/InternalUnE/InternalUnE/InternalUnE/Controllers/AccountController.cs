using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Vacation.BLL.Models;
using Vacation.BLL.Models.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InternalUnE.Controllers
{
    using Data;
    using Service;

    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
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

        public AccountController()
        {
        }

        // GET api/<AccountController>/5
        [HttpGet("{num}")]
        public string Get(long num)
        {
            string strKey = AesHelper.MakeRandomKey(num);
            SessionManager.SetData(HttpContext.Session, LoginKey, strKey);
            return strKey;
        }

        // POST api/<AccountController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RequestData data)
        {
            if (data.Login != null)
                return await Login(data.Login);
            else if (data.Logout != null)
                return await Logout(data.Logout);
            else if (data.CurrentUser != null)
                return CurrentUser();
            else if (data.RequestNewLoginKey != null)
                return RequestNewLoginKey(data.RequestNewLoginKey);
            else if (data.Register != null)
                return Regist(data.Register);
            else if (data.RegisterParam != null)
                return CheckRegisterParam(data.RegisterParam);
            else if (data.RegisterPassword != null)
                return SetPassword(data.RegisterPassword);
            else if (data.RequestLinks != null)
                return RequestLinks();

            return BadRequest();
        }

        private async Task<IActionResult> Login(LoginData data)
        {
            if (SessionManager.HasData(HttpContext.Session, LoginUser))
                return BadRequest();

            string strKey = "";
            ExternalLoginResult result = new ExternalLoginResult(false, "");

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

                        ExternalLoginResult value = WebServiceManager.ExternalLogin(strID, strPW, Startup.SiteURL);

                        if (value.Success)
                        {
                            // 로그인된 사용자 인증처리
                            ApplicationUser user = await AuthUser(value);

                            SessionManager.SetData(HttpContext.Session, LoginUser, user);
                        }

                        return Ok(value);
                    }
                    else
                        result.Message = "사용자 ID 또는 비밀번호 오류입니다.";
                }
                catch (Exception e)
                {
                    result.Message = e.Message;
                }
            }
            else
            {
                result.Message = "로그인 권한이 없습니다.";
            }

            return Ok(result);
        }

        private async Task<ApplicationUser> AuthUser(ExternalLoginResult data)
        {
            await AuthenticationHttpContextExtensions.SignOutAsync(HttpContext);

            ApplicationUser user = AuthenticateUser(data);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                //new Claim(ClaimTypes.Sid, user.UserID),
                //new Claim("TeamName", user.TeamName),
                new Claim(ClaimTypes.Role, "Administrator")
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.

                //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            ClaimsPrincipal principal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties);

            return user;
        }

        private ApplicationUser AuthenticateUser(ExternalLoginResult data)
        {
            return new ApplicationUser()
            {
                Name = data.Name,
                UserID = data.UserID,
                TeamName = data.TeamName,
                LoginKey = data.LoginKey
            };
        }

        private async Task<IActionResult> Logout(LogoutData data)
        {
            // 인증 해제
            var authProperties = new AuthenticationProperties();
            await AuthenticationHttpContextExtensions.SignOutAsync(HttpContext, authProperties);

            SessionManager.SetData(HttpContext.Session, LoginUser, null);

            MessageResult result = new MessageResult(true, "");
            return Ok(result);
        }

        public static ApplicationUser GetCurrentUser(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            ApplicationUser user = null;

            if (SessionManager.TryGetData<ApplicationUser>(httpContext.Session, LoginUser, ref user))
                return user;

            return null;
        }

        private IActionResult CurrentUser()
        {
            ExternalLoginResult result = null;
            ApplicationUser user = null;

            if (SessionManager.TryGetData<ApplicationUser>(HttpContext.Session, LoginUser, ref user))
            {
                result = new ExternalLoginResult(true, "");

                result.Name = user.Name;
                result.UserID = user.UserID;
                result.TeamName = user.TeamName;
                result.LoginKey = user.LoginKey;

                return Ok(result);
            }
            else
            {
                result = new ExternalLoginResult(false, "로그인된 사용자를 찾을수 없습니다.");
            }

            return Ok(result);
        }

        private IActionResult RequestNewLoginKey(RequestNewLoginKey data)
        {
            ExternalLoginResult result = WebServiceManager.RequestNewLoginKey(data.BeginCode, Startup.SiteURL);
            return Ok(result);
        }

        private IActionResult Regist(RegisterData data)
        {
            RegisterResult result = WebServiceManager.Regist(data, Startup.SiteURL);
            return Ok(result);
        }

        private IActionResult CheckRegisterParam(RegisterParam param)
        {
            RegisterParamResult result = WebServiceManager.CheckRegisterParam(param.Value, Startup.SiteURL);
            return Ok(result);
        }

        private IActionResult SetPassword(LoginData data)
        {
            MessageResult result = WebServiceManager.SetPassword(data.Value, Startup.SiteURL);
            return Ok(result);
        }

        private IActionResult RequestLinks()
        {
            ResponseLinks response = new ResponseLinks(true, "");
            response.LinkDatas.AddRange(Startup.Links);
            return Ok(response);
        }
    }
}
