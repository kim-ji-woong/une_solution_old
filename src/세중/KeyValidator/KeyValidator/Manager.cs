using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections;

namespace UnE
{
    namespace KeyValidator
    {
        public enum CertOption
        {
            NEW_CREATE = 0, // 새로운 계정을 생성한다.
                            // 기존에 같은 이름의 계정이 존재하면 계정 생성이 실패한다.
            UPDATE,         // 기존에 존재하는 계정이 사용할 Mac Address List를 바꾼다.
                            // 정보 수정시 기존에 존재하는 계정과 비밀번호를 그대로 입력하여야 한다.
                            // 기존에 사용하던 Mac Address List는 모두 사라지고 새로운 Mac Address List가 DB에 저장된다.
            INSERT,         // 기존에 존재하는 계정이 추가로 사용할 Mac Address List를 입력한다.
                            // 정보 수정시 기존에 존재하는 계정과 비밀번호를 그대로 입력하여야 한다.
                            // 기존에 사용하던 Mac Address List는 그대로 남아있는 상태에서 새로 입력받은 Mac Address List만 추가된다.
            TYPE_COUNT
        }

        public enum CertResult
        {
            SUCCESS = 0,
            INVALID_CODE,
            ALREADY_USED_CODE,
            INVALID_MAC_ADDRESS,
            INVALID_CERT_OPTION,
            TYPE_COUNT
        }

        public sealed class Manager
        {
            // Return 값 : 성공(0)
            //             인증키가 잘못 되었음(1)
            //             인증키가 이미 사용중(2)
            static public int CheckKey(string strDBURL, string strDBID, string strDBPW, string strDBName, string strTableName, string strFieldName, string strCertCode, out string strCode, out bool isAdmin)
            {
                strCode = "";
                isAdmin = false;

                int nCodeLength = strCertCode.Length;

                if (nCodeLength < 27)
                    return 1;

                byte[] bytes = ToBytes(strCertCode);

                if (bytes == null)
                    return 1;

                if (!CheckDBName(strDBName, bytes))
                    return 1;

                if (!CheckMacAddr(bytes))
                    return 1;

                if (!CheckReuse(strCertCode, strDBURL, strDBID, strDBPW, strDBName, strTableName, strFieldName))
                    return 2;

                isAdmin = bytes[12] == 1;
                int hash = strCertCode.GetHashCode();

                if (hash >= 0)
                    strCode = hash.ToString() + "!";
                else
                    strCode = (-hash).ToString() + "~";

                return 0;
            }

            static private CertResult ValidKey1(string strDBName, string strCertCode, out byte[] bytes, out string strCode, out bool isAdmin, out CertOption option)
            {
                strCode = "";
                isAdmin = false;
                bytes = null;
                option = CertOption.NEW_CREATE;

                int nCodeLength = strCertCode.Length;

                if (nCodeLength < 27)
                    return CertResult.INVALID_CODE;

                bytes = ToBytes(strCertCode);

                if (bytes == null)
                    return CertResult.INVALID_CODE;

                if (!CheckDBName(strDBName, bytes))
                    return CertResult.ALREADY_USED_CODE;

                int nOption = (int)bytes[13];

                if (nOption < 0 || nOption >= (int)CertOption.TYPE_COUNT)
                    return CertResult.INVALID_CERT_OPTION;

                option = (CertOption)nOption;
                return CertResult.SUCCESS;
            }

            static private CertResult ValidKey2(string strCertCode, byte[] bytes, out string strCode, out bool isAdmin)
            {
                isAdmin = bytes[12] == 1;
                int hash = strCertCode.GetHashCode();

                if (hash >= 0)
                    strCode = hash.ToString() + "!";
                else
                    strCode = (-hash).ToString() + "~";

                return CertResult.SUCCESS;
            }

            // arrMacAddrList에 담겨진 Mac Address를 이용하여 인증키를 검증한다.
            static public CertResult VaildKey(string strDBURL, string strDBID, string strDBPW, string strDBName, string strTableName, string strFieldName, string strCertCode, ArrayList arrMacAddrList, out string strCode, out bool isAdmin, out CertOption option)
            {
                byte[] bytes = null;
                CertResult result = ValidKey1(strDBName, strCertCode, out bytes, out strCode, out isAdmin, out option);

                if (result != CertResult.SUCCESS)
                    return result;

                if (!CheckMacAddr(bytes, arrMacAddrList))
                    return CertResult.INVALID_MAC_ADDRESS;

                return ValidKey2(strCertCode, bytes, out strCode, out isAdmin);
            }

            // 현재 PC의 Mac Address를 이용하여 인증키를 검증한다.
            static public CertResult VaildKey(string strDBURL, string strDBID, string strDBPW, string strDBName, string strTableName, string strFieldName, string strCertCode, out string strCode, out bool isAdmin, out CertOption option)
            {
                byte[] bytes = null;
                CertResult result = ValidKey1(strDBName, strCertCode, out bytes, out strCode, out isAdmin, out option);

                if (result != CertResult.SUCCESS)
                    return result;

                if (!CheckMacAddr(bytes))
                    return CertResult.INVALID_MAC_ADDRESS;

                return ValidKey2(strCertCode, bytes, out strCode, out isAdmin);
                /*strCode = "";
                isAdmin = false;

                int nCodeLength = strCertCode.Length;

                if (nCodeLength < 27)
                    return 1;

                byte[] bytes = ToBytes(strCertCode);

                if (bytes == null)
                    return 1;

                if (!CheckDBName(strDBName, bytes))
                    return 2;

                if (!CheckMacAddr(bytes))
                    return 3;               

                isAdmin = bytes[12] == 1;
                int hash = strCertCode.GetHashCode();

                if (hash >= 0)
                    strCode = hash.ToString() + "!";
                else
                    strCode = (-hash).ToString() + "~";

                return 0;*/
            }

            static private bool CheckDBName(string strDBName, byte[] bytes)
            {
                int hash1 = BitConverter.ToInt32(bytes, 0);
                int hash2 = strDBName.GetHashCode();
                return hash1 == hash2;
            }

            static private bool CheckMacAddr(byte[] bytes)
            {
                int hash = BitConverter.ToInt32(bytes, 4);
                return FindMacAddress(hash);
            }

            static private bool CheckMacAddr(byte[] bytes, ArrayList arrMacAddrList)
            {
                int hash = BitConverter.ToInt32(bytes, 4);

                string strMacAddrList = "";

                foreach (string strMacAddr in arrMacAddrList)
                {
                    strMacAddrList += strMacAddr;
                }

                int hash2 = strMacAddrList.GetHashCode();
                return hash == hash2;
            }

            static private bool CheckReuse(string strCertCode, string strDBURL, string strDBID, string strDBPW, string strDBName, string strTableName, string strFieldName)
            {
                string strConnection = GetStringConnection(strDBURL, strDBID, strDBPW, strDBName);
                SqlConnection dbConnection = new SqlConnection(strConnection);

                if (!OpenConnection(dbConnection))
                    return false;

                ArrayList arrCodes = new ArrayList();

                if (!ReadCertCodeList(dbConnection, arrCodes, strTableName, strFieldName))
                {
                    dbConnection.Close();
                    return false;
                }

                dbConnection.Close();

                return !FindCode(strCertCode, arrCodes);
            }

            static private bool FindCode(string strCertCode, ArrayList arrCodes)
            {
                int nHash = strCertCode.GetHashCode();
                int hash;

                foreach (string strCode in arrCodes)
                {
                    int len = strCode.Length;

                    if (len == 0)
                        continue;

                    char ch = strCode.ElementAt(len - 1);
                    string strCode2 = strCode.Substring(0, len - 1);

                    if (!int.TryParse(strCode2, out hash))
                        continue;

                    if (ch == '~')
                        hash = -hash;
                    else if (ch != '!')
                        continue;

                    if (hash == nHash)
                        return true;
                }

                return false;
            }

            static private bool ReadCertCodeList(SqlConnection dbConnection, ArrayList arrCodes, string strTableName, string strFieldName)
            {
                SqlDataReader reader;
                string strSql = "SELECT " + strFieldName + " from " + strTableName;

                SqlCommand cmd = new SqlCommand(strSql, dbConnection);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string strCode = reader[0] == null ? "" : (string)reader[0];
                    arrCodes.Add(strCode);
                }

                reader.Close();
                return true;
            }

            static private bool OpenConnection(SqlConnection dbConnection)
            {
                try
                {
                    dbConnection.Open();
                    return true;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                }

                return false;
            }

            static private string GetStringConnection(string strDBURL, string strDBID, string strDBPW, string strDBName)
            {
                string strConnection = "";

                strConnection = "server=" + strDBURL + ";" +
                                "database=" + strDBName + ";" +
                                "uid=" + strDBID + ";" +
                                "password=" + strDBPW + ";";

                return strConnection;
            }

            static private bool FindMacAddress(int hash)
            {
                System.Net.NetworkInformation.NetworkInterface[] adapters = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();

                foreach (System.Net.NetworkInformation.NetworkInterface adapter in adapters)
                {
                    System.Net.NetworkInformation.PhysicalAddress addr = adapter.GetPhysicalAddress();

                    if (addr != null && !addr.ToString().Equals(""))
                    {
                        string strMacAddr = addr.ToString();
                        int nHash = strMacAddr.GetHashCode();

                        if (hash == nHash)
                            return true;
                    }
                }

                return false;
            }

            static private byte[] ToBytes(string strCertCode)
            {
                string strOrder = strCertCode.Substring(28);

                int len = strOrder.Length;

                if (len == 0)
                    return null;

                char ch = strOrder.ElementAt(len - 1);

                long nOrder;
                if (!long.TryParse(strOrder.Substring(0, len - 1), out nOrder))
                    return null;

                if (ch == '~')
                    nOrder = -nOrder;
                else if (ch != '!')
                    return null;

                byte[] bytes = new byte[14];
                byte[] bytesTemp = new byte[14];

                for (int i = 0; i < 14; i++)
                {
                    char ch1 = strCertCode.ElementAt(i * 2);
                    char ch2 = strCertCode.ElementAt(i * 2 + 1);

                    bytesTemp[i] = (byte)((ToByte(ch1) << 4) + ToByte(ch2));
                }

                byte[] bytesOrder = BitConverter.GetBytes(nOrder);

                try
                {
                    for (int i = 0, j = 0; i < 8; i++)
                    {
                        int nIndex1 = bytesOrder[i] >> 4;
                        int nIndex2 = bytesOrder[i] & 0x0f;

                        if (nIndex1 < 0x0e)
                            bytes[nIndex1] = bytesTemp[j++];

                        if (nIndex2 < 0x0e)
                            bytes[nIndex2] = bytesTemp[j++];
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.WriteLine(e.Message);
                    return null;
                }

                return bytes;
            }

            static private byte ToByte(char ch)
            {
                if (ch >= '0' && ch <= '9')
                    return (byte)(ch - '0');
                else if (ch >= 'a' && ch <= 'f')
                    return (byte)(10 + ch - 'a');
                else if (ch >= 'A' && ch <= 'F')
                    return (byte)(10 + ch - 'A');

                return 0;
            }
        }
    }
}
