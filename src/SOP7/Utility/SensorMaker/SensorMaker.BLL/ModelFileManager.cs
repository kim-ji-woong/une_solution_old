using System;
using System.Collections.Generic;
using System.IO;
using TeamEditor.IDAL;
using TeamEditor.Model.Sop.Team;

namespace SensorMaker.BLL
{
    using Models.Request;
    using Models.Response;
    using Models.Account;

    public class ModelFileManager
    {
        private const string _3DModelBaseURL = "gltf";
        private const string _3DTextureBaseURL = "textures";
        private const string _3DBackgroundImage = "bg.png";

        private const string ResourceTarget = "resource";
        private const string CommonTextureBaseFolder = "common\\textures";

        public static GltfOption GetGltfOption(int nUserID, string strUserName, string strRootResource)
        {
            string strBaseFolder = GetResourceFolder(nUserID, strUserName, strRootResource);
            string strBaseFolderLower = strBaseFolder.ToLower();

            string strTarget = ResourceTarget;
            int nIndex = strBaseFolderLower.IndexOf(strTarget);

            string strBaseURL = nIndex < 0 ? strBaseFolder : strBaseFolder.Substring(nIndex);
            strBaseURL = strBaseURL.Replace('\\', '/');

            GltfOption option = new GltfOption();
            option._3DBackgroundImage = _3DBackgroundImage;

            if (strBaseURL.EndsWith("/"))
            {
                option._3DModelBaseURL = "/" + strBaseURL + _3DModelBaseURL;
                option._3DTextureBaseURL = "/" + strBaseURL + _3DTextureBaseURL;
            }
            else
            {
                option._3DModelBaseURL = "/" + strBaseURL + "/" + _3DModelBaseURL;
                option._3DTextureBaseURL = "/" + strBaseURL + "/" + _3DTextureBaseURL;
            }

            return option;
        }

        public static string GetTextureBaseFolder(int nUserID, string strUserName, string strRootResource, out string strBackgroundImagePath)
        {
            int nIndex = strRootResource.ToLower().IndexOf(ResourceTarget);
            string strFolder = nIndex < 0 ? strRootResource : strRootResource.Substring(0, nIndex);

            if (strFolder.EndsWith("\\"))
                strBackgroundImagePath = strFolder + CommonTextureBaseFolder + "\\" + _3DBackgroundImage;
            else
                strBackgroundImagePath = strFolder + "\\" + CommonTextureBaseFolder + "\\" + _3DBackgroundImage;

            string strResourcePath = GetResourceFolder(nUserID, strUserName, strRootResource);

            if (strResourcePath.EndsWith("\\"))
                return strResourcePath + _3DTextureBaseURL;

            return strResourcePath + "\\" + _3DTextureBaseURL;
        }

        public static MessageResult UploadTempFile(Stream stream, string strFileName, int nUserID, string strRootResource, IDataManager dataManager)
        {
            string strErrorMessage;
            RegularMember member = dataManager.GetSelectManager().SelectRegularMember(nUserID, out strErrorMessage);

            if (member == null)
                return new MessageResult(false, strErrorMessage);

            string strFolderPath = GetResourceFolder(nUserID, member.MemberName, strRootResource);

            if (Directory.Exists(strFolderPath) == false)
                Directory.CreateDirectory(strFolderPath);

            string strFilePath = strFolderPath + "\\" + strFileName;

            try
            {
                using (var file = new FileStream(strFilePath, FileMode.Create))
                {
                    stream.CopyTo(file);
                    return new MessageResult(true, "");
                }
            }
            catch (Exception e)
            {
                strErrorMessage = e.Message;
            }

            return new MessageResult(false, strErrorMessage);
        }

        public static string GetResourceFolder(int nUserID, string strUserName, string strRootResource)
        {
            string strFolderPath = string.Format("{0}\\{1}_{2}", strRootResource, nUserID, strUserName);
            return strFolderPath;
        }

        public static MessageResult UploadModelFiles(RequestUploadModelFile data, string strRootResource, string strTempRootResource)
        {
            string strTempFolderPath = GetResourceFolder(data.UserID, data.UserName, strTempRootResource);

            if (Directory.Exists(strTempFolderPath) == false)
            {
                if (data.CancelTempFiles)
                    return new MessageResult(true, "");
                else
                    return new MessageResult(false, "업로드할 파일들이 존재하지 않습니다.");
            }

            if (data.CancelTempFiles)
            {
                string strErrorMessage = RemoveFiles(strTempFolderPath);

                if (strErrorMessage != null)
                    return new MessageResult(false, strErrorMessage);
            }
            else
            {
                string strRegularFolderPath = GetResourceFolder(data.UserID, data.UserName, strRootResource);

                if (strRegularFolderPath.EndsWith("\\"))
                    strRegularFolderPath += _3DModelBaseURL;
                else
                    strRegularFolderPath += "\\" + _3DModelBaseURL;

                if (Directory.Exists(strRegularFolderPath) == false)
                    Directory.CreateDirectory(strRegularFolderPath);

                if (data.RemoveNCopy)
                {
                    Dictionary<string, string> noRemoveFiles = null;

                    if (data.FileNames.Count > 0)
                    {
                        noRemoveFiles = new Dictionary<string, string>();

                        foreach (string strFileName in data.FileNames)
                        {
                            noRemoveFiles[strFileName.ToLower()] = strFileName;
                        }
                    }

                    string strErrorMessage = RemoveFiles(strRegularFolderPath, noRemoveFiles);

                    if (strErrorMessage != null)
                        return new MessageResult(false, strErrorMessage);

                    strErrorMessage = MoveFiles(strTempFolderPath, strRegularFolderPath);

                    if (strErrorMessage != null)
                        return new MessageResult(false, strErrorMessage);
                }
                else if (data.AppendFiles)
                {
                    string strErrorMessage = MoveFiles(strTempFolderPath, strRegularFolderPath);

                    if (strErrorMessage != null)
                        return new MessageResult(false, strErrorMessage);
                }
            }

            return new MessageResult(true, "");
        }

        private static string MoveFiles(string strSourceFolderPath, string strTargetFolderPath)
        {
            string[] files = Directory.GetFiles(strSourceFolderPath);

            foreach (string strFilePath in files)
            {
                int nIndex = strFilePath.LastIndexOf('\\');

                if (nIndex < 0)
                    continue;

                string strFileName = strFilePath.Substring(nIndex + 1).Trim();
                string strTargetPath = strTargetFolderPath.EndsWith("\\") ? strTargetFolderPath + strFileName : strTargetFolderPath + "\\" + strFileName;

                try
                {
                    File.Copy(strFilePath, strTargetPath, true);
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }

            return RemoveFiles(strSourceFolderPath);
        }

        private static string RemoveFiles(string strFolderPath, Dictionary<string, string> noRemoveFiles = null)
        {
            string[] files = Directory.GetFiles(strFolderPath);

            foreach (string strFilePath in files)
            {
                try
                {
                    if (noRemoveFiles != null)
                    {
                        int nIndex = strFilePath.LastIndexOf('\\');
                        string strFileName = nIndex < 0 ? strFilePath.ToLower() : strFilePath.Substring(nIndex + 1).ToLower();

                        if (noRemoveFiles.ContainsKey(strFileName))
                            continue;
                    }

                    File.Delete(strFilePath);
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }

            return null;
        }

        public static MessageResult RemoveTempFile(RequestRemoveTempFile data, string strRootResource)
        {
            string strFolderPath = GetResourceFolder(data.UserID, data.UserName, strRootResource);

            if (Directory.Exists(strFolderPath))
            {
                string strFilePath = strFolderPath.EndsWith("\\") ? strFolderPath + data.FileName : strFolderPath + "\\" + data.FileName;

                try
                {
                    File.Delete(strFilePath);
                }
                catch (Exception e)
                {
                    return new MessageResult(false, e.Message);
                }
            }

            return new MessageResult(true, "");
        }

        // 이전 세션에서 작업하던 임시 파일들이 남아있으면 모두 삭제한다.
        public static void ClearTempFiles(int nUserID, string strUserName, string strRootResource)
        {
            string strFolderPath = GetResourceFolder(nUserID, strUserName, strRootResource);

            if (Directory.Exists(strFolderPath))
                RemoveFiles(strFolderPath);
        }
    }
}
