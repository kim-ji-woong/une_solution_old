using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MSBuildEx
{
    class Program
    {
        static void Main(string[] args)
        {
            string strSolutionPath = "33";
            string strConfig = "";
            string strPlatform = "";

            string strConfigTag = ":configuration=";
            string strPlatformTag = "platform=";
            string strConstants = "", strArguments = "";

            foreach (string param in args)
            {
                string strParam = param.ToLower();

                if (strParam.EndsWith("sln"))
                    strSolutionPath = param;
                else if (strParam.Contains(strConfigTag))
                {
                    int nIndex = strParam.IndexOf(strConfigTag);
                    strConfig = param.Substring(nIndex + strConfigTag.Length);
                }
                else if (strParam.Contains(strPlatformTag))
                {
                    int nIndex = strParam.IndexOf(strPlatformTag);
                    strPlatform = param.Substring(nIndex + strPlatformTag.Length);
                }

                if (strArguments.Length == 0)
                    strArguments = "\"" + param + "\"";
                else
                    strArguments += " \"" + param + "\"";
            }

            if (strSolutionPath.Length > 0 && strConfig.Length > 0 && strPlatform.Length > 0)
            {
                strConstants = GetDefineConstants(strSolutionPath, strConfig, strPlatform);
            }

            if (strConstants.Length > 0)
            {
                strArguments += " /p:DefineConstants=\"" + strConstants + "\"";
            }

            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = "msbuild.exe";
            startInfo.WorkingDirectory = ".\\";
            startInfo.ErrorDialog = false;
            startInfo.Arguments = strArguments;

            System.Diagnostics.Process process;

            try
            {
                process = System.Diagnostics.Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

        private static string GetDefineConstants(string strSolutionPath, string strConfig, string strPlatform)
        {
            string strProjectPath = GetProjectPath(strSolutionPath);

            if (strProjectPath.Length == 0)
                return "";

            string strConstants = GetUserDefineConstants(strProjectPath, strConfig, strPlatform);
            return strConstants;
        }

        private static string GetUserDefineConstants(string strProjectPath, string strConfig, string strPlatform)
        {
            string strCondition = strConfig + "|" + strPlatform;

            StreamReader reader = new StreamReader(strProjectPath, Encoding.UTF8);
            bool findCondition = false;
            string strBeginTag = "<DefineConstants>", strEndTag = "</DefineConstants>";
            string strConstants = "";

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                if (findCondition == false)
                {
                    if (strLine.Contains(strCondition))
                    {
                        findCondition = true;
                        continue;
                    }
                }
                else
                {
                    int nIndex1 = strLine.IndexOf(strBeginTag);

                    if (nIndex1 >= 0)
                    {
                        int nIndex2 = strLine.IndexOf(strEndTag);

                        if (nIndex2 > 2)
                        {
                            strConstants = strLine.Substring(nIndex1 + strBeginTag.Length, nIndex2 - nIndex1 - strBeginTag.Length);
                            break;
                        }
                    }
                }
            }

            reader.Close();

            if (strConstants.Length == 0)
                return strConstants;

            string[] tokens = strConstants.Split(';');
            strConstants = "";

            foreach (string strToken in tokens)
            {
                if (strToken != "DEBUG" && strToken != "TRACE")
                {
                    if (strConstants.Length == 0)
                        strConstants = strToken;
                    else
                        strConstants += ";" + strToken;
                }
            }

            return strConstants;
        }

        private static string GetProjectPath(string strSolutionPath)
        {
            StreamReader reader = new StreamReader(strSolutionPath, Encoding.UTF8);

            while (reader.EndOfStream == false)
            {
                string strLine = reader.ReadLine().Trim();

                if (strLine.Length == 0)
                    continue;

                string strLower = strLine.ToLower();

                if (strLower.Contains("csproj"))
                {
                    string strTag = "\", \"";

                    int nIndex1 = strLower.IndexOf(strTag);

                    if (nIndex1 < 0)
                        continue;

                    int nIndex2 = strLine.IndexOf(strTag, nIndex1 + 1);

                    if (nIndex2 < 0)
                        continue;

                    string strFolder = ".\\";
                    string strProjectName = strLine.Substring(nIndex1 + strTag.Length, nIndex2 - nIndex1 - strTag.Length);

                    int nIndex = strSolutionPath.LastIndexOf('\\');

                    if (nIndex >= 0)
                    {
                        strFolder = strSolutionPath.Substring(0, nIndex + 1);
                    }

                    reader.Close();

                    string strProjectPath = strFolder + strProjectName;
                    return strProjectPath;
                }
            }

            reader.Close();
            return "";
        }
    }
}
