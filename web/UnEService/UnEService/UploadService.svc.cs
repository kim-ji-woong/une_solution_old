using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;

namespace UnEService
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드, svc 및 config 파일에서 클래스 이름 "UploadService"을 변경할 수 있습니다.
    // 참고: 이 서비스를 테스트하기 위해 WCF 테스트 클라이언트를 시작하려면 솔루션 탐색기에서 UploadService.svc나 UploadService.svc.cs를 선택하고 디버깅을 시작하십시오.
    public class UploadService : IUpload
    {
        public static int MaxSegmentSize
        {
            // 20MB
            get { return 20971520; }
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
        public string Upload(string fileName, byte[] bytes, bool isFirst)
        {
            return Upload2(fileName, bytes, isFirst, "");
        }

        public string Upload2(string fileName, byte[] bytes, bool isFirst, string folderPath)
        {
            if (fileName == null || fileName.Length == 0)
                return WebDBService.ErrorMessage2("파일명이 입력되지 않았습니다.", "Upload");

            string strFolder = folderPath;
            if (strFolder.Length <= 0)
                strFolder = System.Configuration.ConfigurationManager.AppSettings["uploadFolder"].ToString();

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
            // 20MB
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
        public string RemoveFile(string fileName)
        {
            return RemoveFile2(fileName, "");
        }
        public string RemoveFile2(string fileName, string folderPath)
        {
            if (fileName == null || fileName.Length == 0)
                return WebDBService.ErrorMessage2("파일명이 입력되지 않았습니다.", "RemoveFile");

            string strFolder = folderPath;
            if (strFolder.Length <= 0)
                strFolder = System.Configuration.ConfigurationManager.AppSettings["uploadFolder"].ToString();

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
        public string RemoveAll()
        {
            return RemoveAll2("");
        }
        public string RemoveAll2(string folderPath)
        {
            string strFolder = folderPath;
            if (strFolder.Length <= 0)
                strFolder = System.Configuration.ConfigurationManager.AppSettings["uploadFolder"].ToString();

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

        #region 압축 풀기        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSrcFile">zip 파일 이름</param>
        /// <returns></returns>
        public string ExtractToTrg(string strSrcFile)
        {
            return ExtractToTrg2(strSrcFile, "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSrcFile">zip 파일 이름</param>
        /// <param name="strTrgPath">zip 파일을 해제할 위치</param>
        /// <returns></returns>
        public string ExtractToTrg2(string strSrcFile, string strTrgPath)
        {
            try
            {
                string strFolder = strTrgPath;
                if (strFolder.Length <= 0)
                    strFolder = System.Configuration.ConfigurationManager.AppSettings["uploadFolder"].ToString();

                string strFilePath = strFolder + "\\" + strSrcFile + ".zip";

                if (!Directory.Exists(strFolder))
                    Directory.CreateDirectory(strFolder);

                System.IO.FileStream fs = new System.IO.FileStream(strFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);

                ICSharpCode.SharpZipLib.Zip.ZipInputStream zis = new ICSharpCode.SharpZipLib.Zip.ZipInputStream(fs);

                ICSharpCode.SharpZipLib.Zip.ZipEntry ze;

                while ((ze = zis.GetNextEntry()) != null)
                {
                    if (!ze.IsDirectory)
                    {
                        string fileName = System.IO.Path.GetFileName(ze.Name);

                        string destDir = System.IO.Path.Combine(strFolder + "\\" + strSrcFile,
                                         System.IO.Path.GetDirectoryName(ze.Name));

                        if (false == Directory.Exists(destDir))
                        {
                            System.IO.Directory.CreateDirectory(destDir);
                        }

                        string destPath = System.IO.Path.Combine(destDir, fileName);

                        System.IO.FileStream writer = new System.IO.FileStream(
                                        destPath, System.IO.FileMode.Create,
                                                System.IO.FileAccess.Write,
                                                    System.IO.FileShare.Write);

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
        #endregion
    }
}
