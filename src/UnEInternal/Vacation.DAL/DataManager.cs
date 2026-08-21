using System;
using System.Security.Cryptography;
using dnsDBUtil;

namespace Vacation.DAL
{
    using IDAL;

    public class DataManager : IDataManager
    {
        private class AES256Cipher
        {
            public static String AES_encrypt(String Input, String key)
            {
                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = System.Text.Encoding.UTF8.GetBytes(key);
                aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] xBuff = null;
                using (var ms = new System.IO.MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = System.Text.Encoding.UTF8.GetBytes(Input);
                        cs.Write(xXml, 0, xXml.Length);
                    }

                    xBuff = ms.ToArray();
                }

                String Output = Convert.ToBase64String(xBuff);
                return Output;
            }

            public static byte[] AES_encrypt(byte[] input, string key)
            {
                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = System.Text.Encoding.UTF8.GetBytes(key);
                aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
                byte[] xBuff = null;
                using (var ms = new System.IO.MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                    {
                        cs.Write(input, 0, input.Length);
                    }

                    xBuff = ms.ToArray();
                }

                return xBuff;
            }

            private static string aaa(string str)
            {
                int nLen = str.Length;
                string strResult = "";

                for (int i = 0; i < nLen; i++)
                {
                    char ch = str[i];

                    if (ch != ' ' && ch != '\t')
                        strResult += ch;
                }

                return strResult;
            }

            public static String AES_decrypt(String Input, String key)
            {
                // FormatException 유발
                if (Input.Length % 4 > 0)
                    return Input;

                byte[] base64Xml = null;

                try
                {
                    base64Xml = Convert.FromBase64String(Input);
                }
                catch (System.FormatException)
                {
                    return Input;
                }

                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = System.Text.Encoding.UTF8.GetBytes(key);
                aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                Input = aaa(Input);

                var decrypt = aes.CreateDecryptor();
                byte[] xBuff = null;
                using (var ms = new System.IO.MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = base64Xml;//Convert.FromBase64String(Input);
                        cs.Write(xXml, 0, xXml.Length);
                    }

                    xBuff = ms.ToArray();
                }

                String Output = System.Text.Encoding.UTF8.GetString(xBuff);
                return Output;
            }

            public static byte[] AES_decrypt(byte[] input, String key)
            {
                RijndaelManaged aes = new RijndaelManaged();
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = System.Text.Encoding.UTF8.GetBytes(key);
                aes.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                var decrypt = aes.CreateDecryptor();
                byte[] xBuff = null;
                using (var ms = new System.IO.MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                    {
                        cs.Write(input, 0, input.Length);
                    }

                    xBuff = ms.ToArray();
                }

                return xBuff;
            }
        }

        private WebDBManager m_dbMgr = null;
        private CreateManager m_createManager = null;
        private SelectManager m_selectManager = null;
        private UpdateManager m_updateManager = null;
        private DeleteManager m_deleteManager = null;

        public DataManager(string strDBName, int nDBType, int nSiteID, string strWebServerURL)
        {
            m_dbMgr = new WebDBManager(strDBName, nDBType, nSiteID, strWebServerURL);
            
            m_createManager = new CreateManager(m_dbMgr, this);
            m_selectManager = new SelectManager(m_dbMgr);
            m_updateManager = new UpdateManager(m_dbMgr);
            m_deleteManager = new DeleteManager(m_dbMgr);
        }

        public ICreateManager GetCreateManager()
        {
            return m_createManager;
        }

        public ISelectManager GetSelectManager()
        {
            return m_selectManager;
        }

        public IUpdateManager GetUpdateManager()
        {
            return m_updateManager;
        }

        public IDeleteManager GetDeleteManager()
        {
            return m_deleteManager;
        }

        private static string key = new string(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6' });
        public string Encrypt(string input)
        {
            return AES256Cipher.AES_encrypt(input, key);
        }

        public string Decrypt(string input)
        {
            return AES256Cipher.AES_decrypt(input, key);
        }
    }
}
