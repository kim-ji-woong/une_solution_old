using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace PersonalSOP.Controllers
{
    using Models;
    using Common;
    using Network;

    public class SOPTalkController : Controller
    {
        private const string TempImageFolder = "TempImage";
        private const string RegularImageFolder = "Image";
        private const string SuccessUploadMessage = "업로드가 완료되었습니다.";

        // GET: SOPTalk
        public ActionResult Index(string ash = "", string uid = "")
        {
            if (ash.Length == 0 && uid.Length > 0)
            {
                SOPHistory history = History.SOPHistoryManager.Instance.SOPHistory.Clone();

                if (history.ActionStepHistory == null)
                {
                    int nActionStepHistoryID, nUserID;

                    if (ParameterManager.SetAccount(ash, uid, Session, out nActionStepHistoryID, out nUserID) == false)
                        return View("Error");
                }
                else
                {
                    int nUserID;
                    int nActionStepHistoryID = history.ActionStepHistory.ActionStepHistoryID;
                    ash = ParameterManager.IDtoString(nActionStepHistoryID);

                    if (ParameterManager.SetAccount(ash, uid, Session, out nActionStepHistoryID, out nUserID) == false)
                        return View("Error");
                }
            }
            else
            {
                int nActionStepHistoryID, nUserID;

                if (ParameterManager.SetAccount(ash, uid, Session, out nActionStepHistoryID, out nUserID) == false)
                    return View("Error");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file, string title, string message, string fileName, string submit)
        {
            int ash = -1;
            int uid = -1;

            if (ParameterManager.SetAccount(ref ash, ref uid, Session) == false)
                return View("Error");

            if (submit == "전송")
            {
                if (file == null)
                    return RegularImage(fileName, title, message);
                else
                    return RegularImage(file, title, message);
            }

            SOPTalkMessage talkMessage = new SOPTalkMessage();
            talkMessage.Message = message;
            talkMessage.Title = title;

            if (file != null && file.ContentLength > 0)
            {
                DateTime dtNow = DateTime.Now;
                string strDate = string.Format("{0}{1:00}{2:00}", dtNow.Year, dtNow.Month, dtNow.Day);
                string strFileName = string.Format("{0}_{1}_{2}", strDate, uid, file.FileName);

                try
                {
                    talkMessage.File = file;
                    talkMessage.FileName = strFileName;

                    string strImageFolder;
                    string strOption = GetImageOption(strFileName, submit, out strImageFolder);

                    string strFolderPath = Server.MapPath("~/" + strImageFolder);
                    string path = Path.Combine(strFolderPath, Path.GetFileName(strFileName));
                    string strUploadResult = "";

                    if (file.InputStream.CanRead)
                    {
                        long length = file.InputStream.Length;

                        if (length > 0)
                        {
                            if (Directory.Exists(strFolderPath) == false)
                                Directory.CreateDirectory(strFolderPath);

                            strUploadResult = SaveToImage(path, strFileName, file.InputStream, strOption, out strFileName);
                            talkMessage.ImageURL = "/" + strImageFolder + "/" + strFileName;
                            talkMessage.FileName = strFileName;
                        }
                    }

                    //if (strImageFolder.Contains("Temp") == false)
                    //    strUploadResult = SuccessUploadMessage + "\r\n" + strUploadResult;

                    talkMessage.UploadResult = strUploadResult;
                }
                catch (Exception ex)
                {
                    talkMessage.UploadResult = "ERROR:" + ex.Message.ToString();
                }
            }
            else
            {
                talkMessage.Message = message;
            }

            return View(talkMessage);
        }

        private ActionResult RegularImage(HttpPostedFileBase file, string title, string message)
        {
            int userID = -1, actionStepHistoryID = -1;
            object id = Session[ParameterManager.UserID];

            if (id != null && id is int)
                userID = (int)id;

            id = Session[ParameterManager.ActionStepHistoryID];

            if (id != null && id is int)
                actionStepHistoryID = (int)id;

            if (userID < 0 || actionStepHistoryID < 0)
                return View("Error");

            SOPTalkMessage talkMessage = new SOPTalkMessage();
            talkMessage.Message = message;
            talkMessage.Title = title;

            string strImage = null;

            if (file.ContentLength > 0)
            {
                try
                {
                    talkMessage.File = file;

                    string strTempFolder = Server.MapPath("~/" + TempImageFolder);
                    string strTempPath = Path.Combine(strTempFolder, file.FileName);

                    // 임시파일을 삭제한다.
                    if (System.IO.File.Exists(strTempPath))
                        System.IO.File.Delete(strTempPath);

                    string strFolderPath = Server.MapPath("~/" + RegularImageFolder);
                    string path = Path.Combine(strFolderPath, Path.GetFileName(file.FileName));
                    string strUploadResult = "";

                    if (file.InputStream.CanRead)
                    {
                        long length = file.InputStream.Length;

                        if (length > 0)
                        {
                            if (Directory.Exists(strFolderPath) == false)
                                Directory.CreateDirectory(strFolderPath);

                            string strFileName;
                            strUploadResult = SaveToImage(path, file.FileName, file.InputStream, "", out strFileName);
                            strImage = "/" + RegularImageFolder + "/" + strFileName;
                            talkMessage.ImageURL = strImage;
                        }
                    }

                    strUploadResult = SuccessUploadMessage + "\r\n" + strUploadResult;
                    talkMessage.UploadResult = strUploadResult;
                }
                catch (Exception ex)
                {
                    talkMessage.UploadResult = "ERROR:" + ex.Message.ToString();
                }
            }

            SaveDB(actionStepHistoryID, userID, strImage, title, message);
            return View(talkMessage);
        }

        private ActionResult RegularImage(string strFileName, string title, string message)
        {
            int userID = -1, actionStepHistoryID = -1;
            object id = Session[ParameterManager.UserID];

            if (id != null && id is int)
                userID = (int)id;

            id = Session[ParameterManager.ActionStepHistoryID];

            if (id != null && id is int)
                actionStepHistoryID = (int)id;

            if (userID < 0 || actionStepHistoryID < 0)
                return View("Error");

            SOPTalkMessage talkMessage = new SOPTalkMessage();
            talkMessage.Message = message;
            talkMessage.Title = title;

            string strImage = null;

            if (strFileName != null && strFileName.Length > 0)
            {
                string strFolderPath = Server.MapPath("~/" + TempImageFolder);
                string path = Path.Combine(strFolderPath, Path.GetFileName(strFileName));

                if (System.IO.File.Exists(path))
                {
                    string strFolderPath2 = Server.MapPath("~/" + RegularImageFolder);

                    if (Directory.Exists(strFolderPath2) == false)
                        Directory.CreateDirectory(strFolderPath2);

                    string strFilePath = Path.Combine(strFolderPath2, Path.GetFileName(strFileName));

                    if (System.IO.File.Exists(strFilePath))
                        System.IO.File.Delete(strFilePath);

                    System.IO.File.Move(path, strFilePath);

                    strImage = "/" + RegularImageFolder + "/" + strFileName;
                    talkMessage.ImageURL = strImage;
                }

                talkMessage.UploadResult = SuccessUploadMessage;
            }

            SaveDB(actionStepHistoryID, userID, strImage, title, message);
            return View(talkMessage);
        }

        private void SaveDB(int nActionStepHistoryID, int nUserID, string strImage, string strTitle, string strMessage)
        {
            DateTime dtNow = DateTime.Now;
            string strTime = string.Format("{0}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}", dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, dtNow.Second);

            if (strImage == null || strImage.Length == 0)
                strImage = "NULL";
            else
                strImage = "'" + strImage + "'";

            int nID = NetworkWebManager.Instance.GetMaxTableID("ActionStepHistoryMessage") + 1;

            string strSQL = "Insert into ActionStepHistoryMessage (ID, ActionStepHistoryID, UserID, TimeStamp, Image, Title, Message) ";
            strSQL += string.Format("Values ({6}, {0}, {1}, '{2}', {3}, '{4}', '{5}')", "NULL"/*nActionStepHistoryID*/, nUserID, strTime, strImage, strTitle, strMessage, nID);

            NetworkWebManager.Instance.DBMgr.GetResultData(strSQL);
            Controllers.SOPBulletinController.nCurrentIndex[nActionStepHistoryID] = nID;
        }

        private string SaveToImage(string strFilePath, string strFileName, Stream stream, string strOption, out string strFinalFileName)
        {
            string strFileSize = GetFileSize((int)stream.Length);
            System.Drawing.Image img = System.Drawing.Bitmap.FromStream(stream);
            strFinalFileName = strFileName;

            stream.Seek(0, SeekOrigin.Begin);
            CheckExif(strFilePath, ref strFinalFileName, img, stream);

            string strImageInfo = string.Format("파일 이름 {0}, 파일 크기 {1}", strFinalFileName, strFileSize);
            return strImageInfo;
        }

        private void CheckExif(string strFilePath, ref string strFinalFileName, System.Drawing.Image img, Stream stream)
        {
            try
            {
                img.Save(strFilePath);
                ushort orientation = 0;

                using (var reader = new ExifLib.ExifReader(strFilePath))
                {
                    object value;

                    if (reader.GetTagValue(ExifLib.ExifTags.Orientation, out value))
                    {
                        orientation = (ushort)value;
                    }
                }

                if (orientation == 3)
                    RotateImage(strFilePath, ref strFinalFileName, img, 180);
                else if (orientation == 6)
                    RotateImage(strFilePath, ref strFinalFileName, img, 90);
                else if (orientation == 8)
                    RotateImage(strFilePath, ref strFinalFileName, img, 270);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }

        private void RotateImage(string strFilePath, ref string strFinalFileName, System.Drawing.Image img, int angle)
        {
            System.IO.File.Delete(strFilePath);

            if (angle == 90)
                img.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
            else if (angle == 180)
                img.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            else if (angle == 270)
                img.RotateFlip(System.Drawing.RotateFlipType.Rotate270FlipNone);

            int nDotIndex = strFilePath.LastIndexOf('.');
            int nDotIndex2 = strFinalFileName.LastIndexOf('.');

            if (nDotIndex >= 0 && nDotIndex2 >= 0)
            {
                strFilePath = strFilePath.Substring(0, nDotIndex) + ".png";
                strFinalFileName = strFinalFileName.Substring(0, nDotIndex2) + ".png";
            }

            img.Save(strFilePath, System.Drawing.Imaging.ImageFormat.Png);
        }

        private string GetFileSize(int nFileSize)
        {
            if (nFileSize < 1024)
            {
                return nFileSize.ToString() + "Bytes";
            }
            else if (nFileSize >= 1024 && nFileSize < 1048576)
            {
                return string.Format("{0:F1}KB", nFileSize / 1024.0);
            }
            else// if (nFileSize >= 1048576)
            {
                return string.Format("{0:F1}MB", nFileSize / 1048576.0);
            }
        }

        private string GetImageOption(string strFileName, string str, out string strImageFolder)
        {
            bool isTemporary = false;
            string strType = "", strOption = "";
            int nIndex = str.LastIndexOf('_');

            if (nIndex >= 0)
            {
                strType = str.Substring(0, nIndex).Trim();
                strOption = str.Substring(nIndex + 1).Trim();

                if (strOption == "3")
                    strOption = "180";
                else if (strOption == "6")
                    strOption = "90";
                else if (strOption == "8")
                    strOption = "270";
            }
            else
                strType = str;

            isTemporary = strType != "전송";

            if (isTemporary)
                strImageFolder = TempImageFolder;
            else
            {
                strImageFolder = RegularImageFolder;

                string strTempFolder = Server.MapPath("~/" + TempImageFolder);
                string path = Path.Combine(strTempFolder, strFileName);

                // 임시파일을 삭제한다.
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

            return strOption;
        }

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult Upload(FileData data)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            data.Message = "'" + data.File.FileName + "' file has been successfully! uploaded";
        //            data.IsValid = true;
        //        }
        //        else
        //        {
        //            data.Message = "'" + data.File.FileName + "' file size exceeds maximum limit.";
        //            data.IsValid = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Trace.WriteLine(ex.Message);
        //    }

        //    return View("Index2", data);
        //    /*string strFileName = $"{oHostingEnvironment.WebRootPath}\\UploadedFiles\\{file.FileName}";

        //    using (FileStream fs = System.IO.File.Create(strFileName))
        //    {
        //        file.CopyTo(fs);
        //        fs.Flush();
        //    }

        //    ViewData["message"] = $"File uploaded Successful. File Length : {file.Length} bytes";
        //    return View("Index2");*/
        //}
    }
}