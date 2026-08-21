using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace dnsDBUtil
{
    class UpDownManager
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

            try
            {
                FileStream fileStream = new FileStream(strFilePath, FileMode.Open, FileAccess.Read);
                BinaryReader reader = new BinaryReader(fileStream);
                long nFileSize = fileStream.Length;

                HttpWebRequest uploadRequest = (HttpWebRequest)WebRequest.Create(string.Format("{0}/api/Upload", strWebServerURL));
                uploadRequest.ContentType = "multipart/form-data;";
                uploadRequest.Method = "POST";

                int nIndex = strFilePath.LastIndexOf('\\');
                string strFileName = nIndex < 0 ? strFilePath : strFilePath.Substring(nIndex + 1);

                if (nFileSize == 0)
                {
                    Dictionary<string, string> values = new Dictionary<string, string>
                    {
                        { "IsFirst", "True" },
                        { "FolderPath", destFolder }
                    };

                    string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                    byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

                    uploadRequest.ContentType = "multipart/form-data; boundary=" + boundary;
                    uploadRequest.Method = "POST";
                    uploadRequest.KeepAlive = true;
                    uploadRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;

                    Stream rs = uploadRequest.GetRequestStream();

                    string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                    foreach (string key in values.Keys)
                    {
                        rs.Write(boundarybytes, 0, boundarybytes.Length);
                        string formitem = string.Format(formdataTemplate, key, values[key]);
                        byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                        rs.Write(formitembytes, 0, formitembytes.Length);
                    }
                    rs.Write(boundarybytes, 0, boundarybytes.Length);

                    string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                    string header = string.Format(headerTemplate, "File", strFileName, "application/octet-stream");
                    byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                    rs.Write(headerbytes, 0, headerbytes.Length);

                    byte[] buffer = new byte[4096];
                    int bytesRead = 0;
                    int readCnt = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        rs.Write(buffer, 0, bytesRead);

                        if (bytesRead > 0)
                        {
                            readCnt += bytesRead;
                        }
                    }
                    fileStream.Close();

                    byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                    rs.Write(trailer, 0, trailer.Length);
                    rs.Close();

                    WebResponse wresp = null;
                    try
                    {
                        wresp = uploadRequest.GetResponse();
                        Stream stream2 = wresp.GetResponseStream();
                        StreamReader reader2 = new StreamReader(stream2);

                    }
                    catch (Exception e)
                    {
                        strErrorMessage = e.Message;

                        if (wresp != null)
                        {
                            wresp.Close();
                            wresp = null;
                        }

                        return false;
                    }
                    finally
                    {
                        uploadRequest = null;
                    }
                }
                else
                {
                    HttpWebRequest maxSizeRequest = (HttpWebRequest)WebRequest.Create(string.Format("{0}/api/Upload/GetMaxSegmentSize", strWebServerURL));
                    maxSizeRequest.ContentType = "application/json; charset=utf-8";
                    maxSizeRequest.Method = "POST";

                    string resTemp = "";

                    using (HttpWebResponse response = maxSizeRequest.GetResponse() as HttpWebResponse)
                    {
                        if (maxSizeRequest.HaveResponse && response != null)
                        {
                            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                            {
                                resTemp = sr.ReadToEnd();
                            }
                        }
                    }

                    string result = JsonManager.Deserialize<string>(resTemp);

                    int nSectionSize = Convert.ToInt32(result);
                    long nReadCount = 0;

                    while (nReadCount < nFileSize)
                    {
                        Dictionary<string, string> values = new Dictionary<string, string>
                        {
                            { "IsFirst", (nReadCount == 0).ToString() },
                            { "FolderPath", destFolder }
                        };

                        string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                        byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

                        uploadRequest.ContentType = "multipart/form-data; boundary=" + boundary;
                        uploadRequest.Method = "POST";
                        uploadRequest.KeepAlive = true;
                        uploadRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;

                        Stream rs = uploadRequest.GetRequestStream();

                        string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                        foreach (string key in values.Keys)
                        {
                            rs.Write(boundarybytes, 0, boundarybytes.Length);
                            string formitem = string.Format(formdataTemplate, key, values[key]);
                            byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                            rs.Write(formitembytes, 0, formitembytes.Length);
                        }
                        rs.Write(boundarybytes, 0, boundarybytes.Length);

                        string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                        string header = string.Format(headerTemplate, "File", strFileName, "application/octet-stream");
                        byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                        rs.Write(headerbytes, 0, headerbytes.Length);

                        //FileStream fileStream = new FileStream(strFilePath, FileMode.Open, FileAccess.Read);
                        byte[] buffer = new byte[4096];
                        int bytesRead = 0;
                        int readCnt = 0;
                        while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            rs.Write(buffer, 0, bytesRead);

                            if (bytesRead > 0)
                            {
                                readCnt += bytesRead;
                            }
                        }
                        fileStream.Close();

                        byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                        rs.Write(trailer, 0, trailer.Length);
                        rs.Close();

                        WebResponse wresp = null;
                        try
                        {
                            wresp = uploadRequest.GetResponse();
                            Stream stream2 = wresp.GetResponseStream();
                            StreamReader reader2 = new StreamReader(stream2);

                        }
                        catch (Exception e)
                        {
                            strErrorMessage = e.Message;

                            if (wresp != null)
                            {
                                wresp.Close();
                                wresp = null;
                            }

                            return false;
                        }
                        finally
                        {
                            uploadRequest = null;
                        }

                        nReadCount += readCnt;

                        //Dictionary<string, string> values = new Dictionary<string, string>
                        //{
                        //    { "FileName", strFileName },
                        //    { "Bytes", Encoding.Default.GetString(bytes) },
                        //    { "IsFirst", (nReadCount == 0).ToString() },
                        //    { "FolderPath", destFolder }
                        //};
                        //
                        //string json = JsonManager.Serialize(values);
                        //
                        //using (StreamWriter sw = new StreamWriter(uploadRequest.GetRequestStream()))
                        //{
                        //    sw.Write(json);
                        //}
                        //
                        //resTemp = "";
                        //
                        //using (HttpWebResponse response = uploadRequest.GetResponse() as HttpWebResponse)
                        //{
                        //    if (uploadRequest.HaveResponse && response != null)
                        //    {
                        //        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        //        {
                        //            resTemp = sr.ReadToEnd();
                        //        }
                        //    }
                        //}
                        //
                        //result = JsonManager.Deserialize<string>(resTemp);
                        //
                        //nReadCount += bytes.LongLength;
                        //
                        //if (result != null && result.Length > 0)
                        //{
                        //    reader.Close();
                        //
                        //    strErrorMessage = result;
                        //    return false;
                        //}
                    }
                }

                reader.Close();
            }
            catch (Exception e)
            {
                strErrorMessage = e.Message;
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
                UploadFile(outPathName, strWebServerURL, out strErrorMessage, destFolder);
                if (strErrorMessage.Length > 0)
                    return false;

                // 3. 서버 업로드 경로에 압축 해제
                ExtractToTrg(strWebServerURL, out strErrorMessage, strFileName);
                if (strErrorMessage.Length > 0)
                    return false;

                // 4. 서버 경로 압축파일 삭제
                RemoveUploadFile(strFileName + ".zip", strWebServerURL, out strErrorMessage, destFolder);

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

                    // .Net Core 환경에서 기본적으로 CP949 (EUC-KR)을 지원하지 않음. 따라서 SharpZipLib 사용 시 DefaultCodePage를 65001(UTF-8)로 변경
                    ZipConstants.DefaultCodePage = 65001;

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
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/api/Upload/ExtractToTrg", strWebServerURL));
                request.ContentType = "application/json; charset=utf-8";
                request.Method = "POST";

                Dictionary<string, string> values = new Dictionary<string, string>
                {
                    { "SrcFile", strSrcFile },
                    { "TrgPath", strTrgPath }
                };

                string json = JsonManager.Serialize(values);

                using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
                {
                    sw.Write(json);
                }

                string resTemp = "";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (request.HaveResponse && response != null)
                    {
                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            resTemp = sr.ReadToEnd();
                        }
                    }
                }

                string result = JsonManager.Deserialize<string>(resTemp);
                
                if (result != null && result.Length > 0)
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
        }
        #endregion

        public static bool RemoveUploadFile(string strFileName, string strWebServerURL, out string strErrorMessage, string removeFolderPath = "")
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/api/Upload/RemoveFile", strWebServerURL));
                request.ContentType = "application/json; charset=utf-8";
                request.Method = "POST";

                Dictionary<string, string> values = new Dictionary<string, string>
                {
                    { "FileName", strFileName },
                    { "FolderPath", removeFolderPath }
                };

                string json = JsonManager.Serialize(values);

                using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
                {
                    sw.Write(json);
                }

                string resTemp = "";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (request.HaveResponse && response != null)
                    {
                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            resTemp = sr.ReadToEnd();
                        }
                    }
                }

                strErrorMessage = JsonManager.Deserialize<string>(resTemp);
            }
            catch (Exception e)
            {
                strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        public static bool RemoveAllUploadFile(string strWebServerURL, out string strErrorMessage, string removeFolderPath = "")
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("{0}/api/Upload/RemoveAll", strWebServerURL));
                request.ContentType = "application/json; charset=utf-8";
                request.Method = "POST";

                Dictionary<string, string> values = new Dictionary<string, string>
                {
                    { "FolderPath", removeFolderPath }
                };

                string json = JsonManager.Serialize(values);

                using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
                {
                    sw.Write(json);
                }

                string resTemp = "";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (request.HaveResponse && response != null)
                    {
                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            resTemp = sr.ReadToEnd();
                        }
                    }
                }

                strErrorMessage = JsonManager.Deserialize<string>(resTemp);
            }
            catch (Exception e)
            {
                strErrorMessage = e.Message;
                return false;
            }

            return true;
        }

        public static bool DownloadFile(string strRemoteFilePath, string strLocalFilePath, string strWebServerURL, out string strErrorMessage)
        {
            strErrorMessage = "";
            try
            {
                bool isCompress = false;
                string strFileName = "";

                HttpWebRequest getFolderRequest = (HttpWebRequest)WebRequest.Create(string.Format("{0}/api/Download/GetFolder", strWebServerURL));
                getFolderRequest.ContentType = "application/json; charset=utf-8";
                getFolderRequest.Method = "POST";

                Dictionary<string, string> folderValues = new Dictionary<string, string>
                {
                    { "Path", strRemoteFilePath }
                };

                string json = JsonManager.Serialize(folderValues);

                using (StreamWriter sw = new StreamWriter(getFolderRequest.GetRequestStream()))
                {
                    sw.Write(json);
                }

                string resTemp = "";

                using (HttpWebResponse response = getFolderRequest.GetResponse() as HttpWebResponse)
                {
                    if (getFolderRequest.HaveResponse && response != null)
                    {
                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            resTemp = sr.ReadToEnd();
                        }
                    }
                }

                // 폴더인 경우 압축해서 가져옴
                bool isFolder = Convert.ToBoolean(JsonManager.Deserialize<string>(resTemp));
                
                if (isFolder)
                {
                    strRemoteFilePath = strRemoteFilePath + ".zip";
                }

                HttpWebRequest getFileSegmentRequest = (HttpWebRequest)WebRequest.Create(string.Format("{0}/api/Download/GetFileSegmentCount", strWebServerURL));
                getFileSegmentRequest.ContentType = "application/json; charset=utf-8";
                getFileSegmentRequest.Method = "POST";

                Dictionary<string, string> fileSegmentValues = new Dictionary<string, string>
                {
                    { "FilePath", strRemoteFilePath }
                };

                json = JsonManager.Serialize(fileSegmentValues);

                using (StreamWriter sw = new StreamWriter(getFileSegmentRequest.GetRequestStream()))
                {
                    sw.Write(json);
                }

                resTemp = "";

                using (HttpWebResponse response = getFileSegmentRequest.GetResponse() as HttpWebResponse)
                {
                    if (getFolderRequest.HaveResponse && response != null)
                    {
                        using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                        {
                            resTemp = sr.ReadToEnd();
                        }
                    }
                }

                string[] results = JsonManager.Deserialize<string[]>(resTemp);

                if (results[0] != "1")
                {
                    strErrorMessage = results[1];
                    return false;
                }
                else
                {
                    int nSegmentCount = int.Parse(results[1]);

                    for (int i = 0; i < nSegmentCount; i++)
                    {
                        HttpWebRequest downLoadRequest = (HttpWebRequest)WebRequest.Create(string.Format("{0}/api/Download", strWebServerURL));
                        downLoadRequest.ContentType = "application/json; charset=utf-8";
                        downLoadRequest.Method = "POST";

                        Dictionary<string, string> downloadValues = new Dictionary<string, string>
                        {
                            { "FilePath", strRemoteFilePath },
                            { "SegmentIndex", i.ToString() }
                        };

                        json = JsonManager.Serialize(downloadValues);

                        using (StreamWriter sw = new StreamWriter(downLoadRequest.GetRequestStream()))
                        {
                            sw.Write(json);
                        }

                        resTemp = "";

                        byte[] data = new byte[4096];
                        byte[] totalData = new byte[0];
                        
                        byte[] lengthByteArray = new byte[8];                        
                        byte[] readCntByteArray = new byte[4];
                        byte[] msgByteArray = new byte[0];
                        byte[] fileByteArray = new byte[0];

                        int pos = 0;
                        int count = 0;

                        using (HttpWebResponse response = downLoadRequest.GetResponse() as HttpWebResponse)
                        {
                            if (getFolderRequest.HaveResponse && response != null)
                            {
                                using (Stream sr = response.GetResponseStream())
                                {
                                    do
                                    {
                                        count = sr.Read(data, pos, data.Length);
                                        if (count > 0)
                                        {
                                            int tempSize = totalData.Length;
                                            Array.Resize(ref totalData, totalData.Length + count);
                                            Array.Copy(data, 0, totalData, tempSize, count);
                                        }

                                    } while (count > 0);

                                    int startIdx = totalData.Length - lengthByteArray.Length;
                                    Array.Copy(totalData, startIdx, lengthByteArray, 0, lengthByteArray.Length);
                                    if (BitConverter.IsLittleEndian)
                                    {
                                        Array.Reverse(lengthByteArray);
                                    }
                                    int stringLength = BitConverter.ToInt32(lengthByteArray, 0);


                                    startIdx -= stringLength;
                                    Array.Resize(ref msgByteArray, stringLength);
                                    Array.Copy(totalData, startIdx, msgByteArray, 0, stringLength);
                                    strErrorMessage = Encoding.UTF8.GetString(msgByteArray);

                                    startIdx -= readCntByteArray.Length;
                                    Array.Copy(totalData, startIdx, readCntByteArray, 0, readCntByteArray.Length);
                                    if (BitConverter.IsLittleEndian)
                                    {
                                        Array.Reverse(readCntByteArray);
                                    }
                                    int fileSize = BitConverter.ToInt32(readCntByteArray, 0);

                                    Array.Resize(ref fileByteArray, fileSize);
                                    Array.Copy(totalData, 0, fileByteArray, 0, fileByteArray.Length);
                                }
                            }
                        }

                        if (strErrorMessage.Length > 0)
                        {
                            return false;
                        }

                        if (fileByteArray != null)
                        {
                            strLocalFilePath = strLocalFilePath.Replace("\\", @"\");
                            FileStream fs = i == 0 ? File.Open(strLocalFilePath, FileMode.Create) : File.Open(strLocalFilePath, FileMode.Append);

                            BinaryWriter writer = new BinaryWriter(fs);
                            writer.Write(fileByteArray);
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
                        RemoveUploadFile(strFileName + ".zip", strWebServerURL, out strErrorMessage, strDestPath);
                    }
                }
            }
            catch (Exception e)
            {
                strErrorMessage = e.Message;
                return false;
            }

            return true;
        }
    }
}
