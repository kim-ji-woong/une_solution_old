using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace DBUtility2
{
    using UploadService;
    using DownloadService;
    using ICSharpCode.SharpZipLib.Zip;
    using ICSharpCode.SharpZipLib.Core;

    public class UpDownManager
    {
        private const string NOT_CONNECTED_EXCEPTION = "WebDB 접속이 끊어졌습니다.\r\n서버 관리자에게 문의하세요.";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <param name="strWebServerURL"></param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        public static bool UploadFile(string strFilePath, string strWebServerURL, out string strErrorMessage, string destFolder = "")
        {
            strErrorMessage = "";

            FileStream fs = File.Open(strFilePath, FileMode.Open);
            BinaryReader reader = new BinaryReader(fs);
            long nFileSize = fs.Length;

            /*if (nFileSize == 0)
            {
                reader.Close();
                return false;
            }*/

            try
            {
                ChannelFactory<IUpload> factory;
                IUpload uploader = GetProxy(strWebServerURL, out factory);
                //UploadClient uploader = GetUploadClient(strWebServerURL);

                int nIndex = strFilePath.LastIndexOf('\\');
                string strFileName = nIndex < 0 ? strFilePath : strFilePath.Substring(nIndex + 1);

                if (nFileSize == 0)
                {
                    string result = uploader.Upload2(strFileName, null, true, destFolder);

                    if (result.Length > 0)
                    {
                        reader.Close();
                        //uploader.Close();
                        factory.Close();

                        strErrorMessage = result;
                        return false;
                    }
                }
                else
                {
                    int nSectionSize = uploader.GetMaxSegmentSize();
                    long nReadCount = 0;

                    while (nReadCount < nFileSize)
                    {
                        byte[] bytes = reader.ReadBytes(nSectionSize);
                        string result = uploader.Upload2(strFileName, bytes, nReadCount == 0, destFolder);
                        nReadCount += bytes.LongLength;

                        if (result.Length > 0)
                        {
                            reader.Close();
                            //uploader.Close();
                            factory.Close();

                            strErrorMessage = result;
                            return false;
                        }
                    }
                }

                reader.Close();
                //uploader.Close();
                factory.Close();
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                strErrorMessage = NOT_CONNECTED_EXCEPTION;
                return false;
            }

            return true;
        }

        public static bool UploadFile(byte[] fileData, string strFileName, string strWebServerURL, out string strErrorMessage, string destFolder = "")
        {
            if (fileData == null)
            {
                strErrorMessage = "fileData가 null입니다.";
                return false;
            }

            if (strFileName == null || strFileName.Length == 0)
            {
                strErrorMessage = "파일 이름이 지정되지 않았습니다.";
                return false;
            }

            strErrorMessage = "";
            long nFileSize = fileData.LongLength;

            try
            {
                ChannelFactory<IUpload> factory;
                IUpload uploader = GetProxy(strWebServerURL, out factory);
                
                if (nFileSize == 0)
                {
                    string result = uploader.Upload2(strFileName, null, true, destFolder);

                    if (result.Length > 0)
                    {
                        factory.Close();

                        strErrorMessage = result;
                        return false;
                    }
                }
                else
                {
                    int nSectionSize = uploader.GetMaxSegmentSize();
                    long nReadCount = 0;

                    while (nReadCount < nFileSize)
                    {
                        long bytesCount = nSectionSize;

                        if (nReadCount + bytesCount > nFileSize)
                            bytesCount = nFileSize - nReadCount;

                        byte[] bytes = new byte[bytesCount];

                        for (long i=0;i<bytesCount;i++)
                        {
                            bytes[i] = fileData[nReadCount + i];
                        }
                         
                        string result = uploader.Upload2(strFileName, bytes, nReadCount == 0, destFolder);
                        nReadCount += bytesCount;

                        if (result.Length > 0)
                        {
                            factory.Close();

                            strErrorMessage = result;
                            return false;
                        }
                    }
                }

                factory.Close();
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                strErrorMessage = NOT_CONNECTED_EXCEPTION;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 폴더인 경우 : 압축 -> 업로드 -> 압축해제 -> 압축파일 삭제
        /// </summary>
        /// <returns></returns>
        public static bool UploadFolder(string strFilePath, string strWebServerURL, out string strErrorMessage, string destFolder = "")
        {
            strErrorMessage = "";

            int nIndex = strFilePath.LastIndexOf('\\');
            string strFileName = strFilePath.Substring(nIndex + 1);
            DirectoryInfo dir = new DirectoryInfo(strFilePath);

            string strZipPath = ChangeHangul(strFilePath);
            strFileName = ChangeHangul(strFileName);

            // 1. 압축
            string outPathName = strZipPath + ".zip";
            if (File.Exists(outPathName + ".zip"))
                File.Delete(outPathName + ".zip");
                        
            if (Compress(outPathName, strFilePath))
            {
                // 2. 서버에 업로드
                UpDownManager.UploadFile(outPathName, strWebServerURL, out strErrorMessage, destFolder);
                if (strErrorMessage.Length > 0)
                    return false;

                // 3. 서버 업로드 경로에 압축 해제
                    UpDownManager.ExtractToTrg(strWebServerURL, out strErrorMessage, strFileName);
                if (strErrorMessage.Length > 0)
                    return false;

                // 4. 서버 경로 압축파일 삭제
                UpDownManager.RemoveUploadFile(strFileName + ".zip", strWebServerURL, out strErrorMessage, destFolder);

                // 5. 로컬 경로 압축파일 삭제
                if (File.Exists(outPathName))
                    File.Delete(outPathName);
            }
            else
                return false;

            
            return true;
        }

        private static string ChangeHangul(string strOrigin)
        {
            int nSlash = strOrigin.LastIndexOf('\\');
            string strTemp = "";

            for (int i = nSlash + 1; i < strOrigin.Length; i++)
            {
                char ch = strOrigin.ElementAt(i);

                if (ch > 256)
                    strTemp += '_';
                else
                    strTemp += ch;
            }

            strOrigin = strOrigin.Substring(0, nSlash + 1) + strTemp;
            return strOrigin;
        }

        #region 압축
        private static bool Compress(string outPathName, string folderName)
        {
            try
            {
                if (File.Exists(outPathName))
                    File.Delete(outPathName);

                FileStream fsOut = File.Create(outPathName);
                ZipOutputStream zipStream = new ZipOutputStream(fsOut);

                zipStream.SetLevel(9);

                int folderOffset = folderName.Length + (folderName.EndsWith("\\") ? 0 : 1);

                CompressFolder(folderName, zipStream, folderOffset);

                zipStream.IsStreamOwner = true;
                
                zipStream.Close();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static void CompressFolder(string path, ZipOutputStream zipStream, int folderOffset)
        {
            string[] files = Directory.GetFiles(path);

            foreach (string fileName in files)
            {
                try
                {
                    FileInfo fi = new FileInfo(fileName);

                    string entryName = fileName.Substring(folderOffset);
                    entryName = ZipEntry.CleanName(entryName);
                    ZipEntry newEntry = new ZipEntry(entryName);
                    newEntry.DateTime = fi.LastWriteTime;
                    newEntry.Size = fi.Length;
                    
                    byte[] buffer = new byte[4096];
                    using (FileStream streamReader = File.OpenRead(fileName))
                    {
                        zipStream.PutNextEntry(newEntry);
                        StreamUtils.Copy(streamReader, zipStream, buffer);
                    }
                    zipStream.CloseEntry();
                }
                catch (Exception ex)
                {
                    // 사용중인 파일은 제외한다 ex:sdmsserver.log
                    continue;
                }
            }

            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
            {
                CompressFolder(folder, zipStream, folderOffset);
            }
        }
        #endregion

        #region 압축 풀기
        public static bool ExtractToTrg(string strWebServerURL, out string strError, string strSrcFile, string strTrgPath = "")
        {
            strError = "";

            try
            {
                ChannelFactory<IUpload> factory;
                IUpload uploader = GetProxy(strWebServerURL, out factory);
                string result = uploader.ExtractToTrg2(strSrcFile, strTrgPath);
                if (result.Length > 0)
                {
                    strError = "[ERROR] ExtractToTrg() : " + result;
                    return false;
                }
            }
            catch (Exception e)
            {
                strError = "[ERROR] ExtractToTrg() : " + e.Message;
                return false;
            }

            return true;
            //return Core.UZip.ExtractFile(strSrcFile, strTrgPath);
        }
        #endregion

        public static bool RemoveUploadFile(string strFileName, string strWebServerURL, out string strErrorMessage, string removeFolderPath = "")
        {
            try
            {
                ChannelFactory<IUpload> factory;
                IUpload uploader = GetProxy(strWebServerURL, out factory);
                //UploadClient uploader = GetUploadClient(strWebServerURL);
                strErrorMessage = uploader.RemoveFile2(strFileName, removeFolderPath);
                //uploader.Close();
                factory.Close();
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                strErrorMessage = NOT_CONNECTED_EXCEPTION;
            }

            return strErrorMessage.Length == 0;
        }

        public static bool RemoveAllUploadFile(string strWebServerURL, out string strErrorMessage, string removeFolderPath = "")
        {
            try
            {
                ChannelFactory<IUpload> factory;
                IUpload uploader = GetProxy(strWebServerURL, out factory);
                //UploadClient uploader = GetUploadClient(strWebServerURL);
                strErrorMessage = uploader.RemoveAll2(removeFolderPath);
                //uploader.Close();
                factory.Close();
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                strErrorMessage = NOT_CONNECTED_EXCEPTION;
            }

            return strErrorMessage.Length == 0;
        }
        
        public static bool DownloadFile(string strRemoteFilePath, string strLocalFilePath, string strWebServerURL, out string strErrorMessage)
        {
            strErrorMessage = "";
            try
            {
                //DownloadClient downloader = GetDownloadClient(strWebServerURL);
                ChannelFactory<IDownload> factory;
                IDownload downloader = GetProxy(strWebServerURL, out factory);
                
                bool isCompress = false;
                string strFileName = "";
                
                // 폴더인 경우 압축해서 가져옴
                bool isFolder = downloader.GetFolder(strRemoteFilePath);
                if (isFolder)
                {
                    strRemoteFilePath = strRemoteFilePath + ".zip";
                }
                
                string[] results = downloader.GetFileSegmentCount(strRemoteFilePath);
                
                if (results[0] != "1")
                {
                    strErrorMessage = results[1];
                }
                else
                {
                    int nSegmentCount = int.Parse(results[1]);
                    //int nReadCount;

                    for (int i = 0; i < nSegmentCount; i++)
                    {                        
                        DownloadRequest request = new DownloadRequest(strRemoteFilePath, i);
                        DownloadResponse response = downloader.Download(request);
                        //byte[] bytes = downloader.DownloadFile(strRemoteFilePath, i, out nReadCount, out strErrorMessage);
                        
                        if (response.errorMessage.Length > 0)
                        {
                            strErrorMessage = response.errorMessage;
                            factory.Close();
                            //downloader.Close();
                            return false;
                        }
                        
                        byte[] bytes = response.DownloadResult;

                        if (bytes != null && response.readCount > 0)
                        {
                            strLocalFilePath = strLocalFilePath.Replace("\\", @"\");
                            FileStream fs = i == 0 ? File.Open(strLocalFilePath, FileMode.Create) : File.Open(strLocalFilePath, FileMode.Append);
                            
                            BinaryWriter writer = new BinaryWriter(fs);
                            writer.Write(bytes);
                            writer.Close();
                        }
                        else if (i == 0)
                        {
                            FileStream fs = File.Open(strLocalFilePath, FileMode.Create);
                            fs.Close();
                        }
                    }

                    // 압축파일을 생성한 경우 삭제
                    if (isCompress && strFileName.Length > 0)
                    {
                        int nIndex = strRemoteFilePath.LastIndexOf('\\');
                        string strDestPath = strRemoteFilePath.Substring(0, nIndex);
                        UpDownManager.RemoveUploadFile(strFileName + ".zip", strWebServerURL, out strErrorMessage, strDestPath);                            
                    }
                }
                factory.Close();
            }
            catch (System.ServiceModel.EndpointNotFoundException ex)
            {
                strErrorMessage = NOT_CONNECTED_EXCEPTION;
                return false;
            }

            return true;
        }

        private static IUpload GetProxy(string strWebServerURL, out ChannelFactory<IUpload> factory)
        {
            ServiceEndpoint ep = MakeEndpoint(strWebServerURL, "UploadService", typeof(IUpload));

            factory = new ChannelFactory<IUpload>(ep);
            IUpload proxy = factory.CreateChannel();

            return proxy;
        }

        private static IDownload GetProxy(string strWebServerURL, out ChannelFactory<IDownload> factory)
        {
            ServiceEndpoint ep = MakeEndpoint(strWebServerURL, "DownloadService", typeof(IDownload));

            factory = new ChannelFactory<IDownload>(ep);
            IDownload proxy = factory.CreateChannel();

            return proxy;
        }

        private static ServiceEndpoint MakeEndpoint(string strWebServerURL, string strServiceName, Type contractType)
        {
            System.Xml.XmlDictionaryReaderQuotas readerQuotas = new System.Xml.XmlDictionaryReaderQuotas();
            readerQuotas.MaxDepth = 128;
            readerQuotas.MaxStringContentLength = 2147483647;
            readerQuotas.MaxArrayLength = 2147483647;
            readerQuotas.MaxBytesPerRead = 31457280;
            readerQuotas.MaxNameTableCharCount = 16384;

            WSHttpSecurity security = new WSHttpSecurity();
            security.Mode = SecurityMode.None;
            //security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
            //security.Message.ClientCredentialType = MessageCredentialType.Windows;

            WSHttpBinding binding = new WSHttpBinding();
            binding.MessageEncoding = WSMessageEncoding.Mtom;
            binding.MaxBufferPoolSize = 31457280;
            binding.MaxReceivedMessageSize = 2147483647;
            binding.ReaderQuotas = readerQuotas;
            binding.Security = security;

            Uri uri = new Uri(strWebServerURL + "/" + strServiceName + ".svc");
            ServiceEndpoint ep = new ServiceEndpoint(
                ContractDescription.GetContract(contractType),
                binding,
                new EndpointAddress(uri));

            return ep;
        }

        /*private static UploadClient GetUploadClient(string strWebServerURL)
        {
            UploadClient uploader = new UploadClient("WSHttpBinding_IUpload", strWebServerURL + "/UploadService.svc");
            return uploader;
        }

        private static DownloadClient GetDownloadClient(string strWebServerURL)
        {
            DownloadClient downloader = new DownloadClient("WSHttpBinding_IDownload", strWebServerURL + "/DownloadService.svc");
            return downloader;
        }*/
    }
}
