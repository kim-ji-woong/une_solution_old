using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnEService_Core.Common;
using UnEService_Core.Interface;

namespace UnEService_Core.Service
{
    public class DownloadService : IDownload
    {
        private static readonly object _lock = new object();
        private static DownloadService instance;
        public static DownloadService Instance
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new DownloadService();
                    }
                    return instance;
                }
            }
        }

        /// <summary>
        /// 파일을 다운로드한다.
        /// 다운로드 받을 파일 크기에 따라 Download()를 여러번 호출하면서 다운로드해야한다.
        /// 파일을 몇번에 걸쳐 분할하여 다운로드 받아야 하는지는 GetFileSegmentCount()를 호출하여 확인해야 한다.
        /// </summary>
        /// <param name="filePath">다운로드할 파일의 경로이며, 서버의 로컬경로가 사용된다.</param>
        /// <param name="segmentIndex">파일이 분할될 경우 분할된 Segment의 Index</param>
        /// <param name="readCount">다운로드가 성공하면 다운로드된 byte 개수를 알려준다.</param>
        /// <param name="errorMessage">다운로드가 성공하면 빈 문자열의 값을 갖는다.
        ///                            실패하면 에러 메시지를 갖는다.
        /// </param>
        /// <returns>다운로드가 성공하면 readCount 만큼의 byte 배열을 리턴한다.
        ///          실패하면 null을 리턴한다.
        /// </returns>
        public byte[] Download(string filePath, int segmentIndex, out int readCount, out string errorMessage)
        {
            errorMessage = "";
            readCount = 0;

            byte[] bytes = null;

            try
            {
                if (segmentIndex < 0)
                {
                    errorMessage = "segmentIndex는 0 또는 그 이상의 값이어야 합니다.\r\n" + segmentIndex.ToString();
                    Logger.Instance.Write("DownloadFile : " + errorMessage);
                    return null;
                }
                else if (File.Exists(filePath) == false)
                {
                    errorMessage = "존재하지 않는 파일 경로입니다.\r\n" + filePath;
                    Logger.Instance.Write("DownloadFile : " + errorMessage);
                    return null;
                }
                else
                {
                    FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    long nFileSize = fs.Length;

                    if (nFileSize == 0 && segmentIndex == 0)
                    {
                        fs.Close();
                        readCount = 0;
                        return bytes;
                    }

                    BinaryReader reader = new BinaryReader(fs);

                    long nSegmentCount = UploadService.MaxSegmentSize;
                    long nReadCount = nSegmentCount * segmentIndex;

                    fs.Seek(nReadCount, SeekOrigin.Begin);

                    long nSize = nFileSize - nReadCount > nSegmentCount ? nSegmentCount : nFileSize - nReadCount;

                    if (nSize == 0)
                    {
                        readCount = 0;
                        reader.Close();
                        return null;
                    }
                    else if (nSize < 0)
                    {
                        errorMessage = "segmentIndex가 파일의 Segment 개수를 초과하였습니다.\r\n" + segmentIndex.ToString();
                        reader.Close();
                        Logger.Instance.Write("DownloadFile : " + errorMessage);
                        return null;
                    }

                    bytes = reader.ReadBytes((int)nSize);
                    reader.Close();

                    nReadCount += bytes.Length;
                    readCount = bytes.Length;
                }
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                Logger.Instance.Write("DownloadFile : " + errorMessage);
            }

            return bytes;
        }

        public string[] GetFileSegmentCount(string filePath)
        {
            string[] results = new string[2];

            try
            {
                if (File.Exists(filePath) == false)
                {
                    results[0] = "0";
                    results[1] = "존재하지 않는 파일 경로입니다.\r\n" + filePath;
                    Logger.Instance.Write("DownloadFile : " + results[1]);
                }
                else
                {
                    FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    long nSegmentCount = fs.Length / UploadService.MaxSegmentSize;

                    if (fs.Length == 0)
                        nSegmentCount = 1;
                    else
                    {
                        if (fs.Length % UploadService.MaxSegmentSize > 0)

                            nSegmentCount++;
                    }

                    fs.Close();

                    results[0] = "1";
                    results[1] = nSegmentCount.ToString();
                }
            }
            catch (Exception e)
            {
                results[0] = "0";
                results[1] = e.Message;
                Logger.Instance.Write("DownloadFile : " + e.Message);
            }

            return results;
        }

        public bool GetFolder(string path)
        {
            string errorMessage = "";
            string strFileName = "";

            // 폴더인 경우 압축하기
            FileAttributes attr = File.GetAttributes(path);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                int nIndex = path.LastIndexOf('\\');
                strFileName = path.Substring(nIndex + 1);

                DirectoryInfo dir = new DirectoryInfo(path);

                string strZipPath = ChangeHangul(path);
                strFileName = ChangeHangul(strFileName);

                string outPathName = strZipPath + ".zip";
                if (File.Exists(outPathName + ".zip"))
                    File.Delete(outPathName + ".zip");

                if (Compress(outPathName, path))
                {
                    path = path + ".zip";
                }
                else
                {
                    errorMessage = "폴더 압축 실패";
                }

                return true;
            }
            else
                return false;
        }

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

                    ZipConstants.DefaultCodePage = 65001;

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
    }
}
