using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnEService_Core.Common;
using UnEService_Core.Interface;
using ICSharpCode.SharpZipLib.Zip;

namespace UnEService_Core.Service
{
    public class UploadService : IUpload
    {
        private static readonly object _lock = new object();
        private static UploadService instance;
        public static UploadService Instance
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new UploadService();
                    }
                    return instance;
                }
            }
        }

        public static int MaxSegmentSize
        {
            // 20 MB
            get { return 20971520; }
        }

        public static string UploadFolderPath
        {
            get { return Startup.Configuration.GetSection("AppConfiguration").GetSection("uploadFolder").Value; }
        }

        /// <summary>
        /// 지정된 경로에 파일을 업로드한다.
        /// 이미 같은 이름의 파일이 존재할 경우 덮어쓰여진다.
        /// GetMaxSegmentSize() 바이트 미만일 경우 이 함수 한번만 호출하면 업로드 할수 있다.
        /// 그보다 큰 파일은 분할하여 Upload()를 여러번 호출하면서 Upload한다.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bytes"></param>
        /// <param name="isFirst">파일의 첫번째 Segment인가?</param>
        /// <param name="descPath">Upload할 서버의 폴더를 지정한다.</param>
        /// <returns>업로드가 성공하면 빈 문자열을 리턴한다.
        ///          실패하면 에러 메시지를 리턴한다.
        /// </returns>

        public string Upload(string fileName, byte[] bytes, bool isFirst, string folderPath)
        {
            if (fileName == null || fileName.Length == 0)
                return WebDBService.ErrorMessage2("파일명이 입력되지 않았습니다.", "Upload");

            string strFolder = folderPath;
            if (strFolder == null || strFolder.Length <= 0)
                strFolder = UploadFolderPath;

            try
            {
                if (Directory.Exists(strFolder) == false)
                    Directory.CreateDirectory(strFolder);

                FileStream fs = null;
                string strFilePath = strFolder + "\\" + fileName;

                if (isFirst)
                {
                    fs = File.Open(strFilePath, FileMode.Create);
                }
                else
                {
                    if (File.Exists(strFilePath) == false)
                        return WebDBService.ErrorMessage2("isFirst가 false이면 파일이 이미 존재하여야만 합니다.", "Upload");

                    fs = File.Open(strFilePath, FileMode.Append);
                }

                if (bytes != null && bytes.Count() > 0)
                {
                    BinaryWriter writer = new BinaryWriter(fs);
                    writer.Write(bytes);
                    writer.Close();
                }
                else
                    fs.Close();
            }
            catch (Exception e)
            {
                Logger.Instance.Write("Upload : " + e.Message);
                return e.Message;
            }

            return "";
        }

        /// <summary>
        /// Uploaad() 호출시 한번에 전송할 수 있는 최대 바이너리의 바이트 크기를 나타낸다.
        /// 이보다 더 큰 파일을 전송하려하면 서비스에서 거부당할수 있다.
        /// </summary>
        /// <returns>한번에 전송가능한 바이너리 크기
        /// </returns>
        public int GetMaxSegmentSize()
        {
            // 20 MB
            return MaxSegmentSize;
        }

        /// <summary>
        /// 지정된 경로에 업로드되어 있는 파일을 삭제한다.
        /// </summary>
        /// <param name="fileName"></param>
        /// /// <param name="folderPath"></param>
        /// <returns>파일 삭제가 성공하면 빈 문자열을 리턴한다.
        ///          실패하면 에러 메시지를 리턴한다.
        /// </returns>

        public string RemoveFile(string fileName, string folderPath)
        {
            if (fileName == null || fileName.Length == 0)
                return WebDBService.ErrorMessage2("파일명이 입력되지 않았습니다.", "RemoveFile");

            string strFolder = folderPath;
            if (strFolder == null || strFolder.Length <= 0)
                strFolder = UploadFolderPath;

            try
            {
                if (Directory.Exists(strFolder) == false)
                    return "";

                File.Delete(strFolder + "\\" + fileName);
            }
            catch (Exception e)
            {
                Logger.Instance.Write("RemoveFile : " + e.Message);
                return e.Message;
            }

            return "";
        }

        /// <summary>
        /// 지정된 경로에 업로드되어 있는 모든 파일을 삭제한다.
        /// </summary>
        /// <returns>파일 삭제가 성공하면 빈 문자열을 리턴한다.
        ///          실패하면 에러 메시지를 리턴한다.
        /// </returns>

        public string RemoveAll(string folderPath)
        {
            string strFolder = folderPath;
            if (strFolder == null || strFolder.Length <= 0)
                strFolder = UploadFolderPath;

            try
            {
                if (Directory.Exists(strFolder) == false)
                    return "";

                foreach (string strFile in Directory.GetFiles(strFolder))
                {
                    File.Delete(strFile);
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write("RemoveAll : " + e.Message);
                return e.Message;
            }

            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSrcFile">zip 파일 이름</param>
        /// <param name="strTrgPath">zip 파일을 해제할 위치</param>
        /// <returns></returns>
        public string ExtractToTrg(string strSrcFile, string strTrgPath)
        {
            try
            {
                string strFolder = strTrgPath;
                if (strFolder == null || strFolder.Length <= 0)
                    strFolder = UploadFolderPath;

                string strFilePath = strFolder + "\\" + strSrcFile + ".zip";

                if (!Directory.Exists(strFolder))
                    Directory.CreateDirectory(strFolder);

                FileStream fs = new FileStream(strFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

                // .Net Core 환경에서 기본적으로 CP949 (EUC-KR)을 지원하지 않음. 따라서 SharpZipLib 사용 시 DefaultCodePage를 65001(UTF-8)로 변경
                ZipConstants.DefaultCodePage = 65001;
                ZipInputStream zis = new ZipInputStream(fs);
                ZipEntry ze;

                while ((ze = zis.GetNextEntry()) != null)
                {
                    if (!ze.IsDirectory)
                    {
                        string fileName = Path.GetFileName(ze.Name);

                        string destDir = Path.Combine(strFolder + "\\" + strSrcFile,
                                         Path.GetDirectoryName(ze.Name));

                        if (false == Directory.Exists(destDir))
                        {
                            Directory.CreateDirectory(destDir);
                        }

                        string destPath = Path.Combine(destDir, fileName);

                        FileStream writer = new FileStream(destPath, FileMode.Create, FileAccess.Write, FileShare.Write);

                        byte[] buffer = new byte[2048];
                        int len;
                        while ((len = zis.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            writer.Write(buffer, 0, len);
                        }

                        writer.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Instance.Write("Upload : " + e.Message);
                return e.Message;
            }

            return "";
            //return Core.UZip.ExtractFile(strSrcFile, strTrgPath);
        }
    }
}
