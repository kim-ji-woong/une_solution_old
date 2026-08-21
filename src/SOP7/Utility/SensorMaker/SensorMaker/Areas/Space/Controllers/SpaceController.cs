using Microsoft.AspNetCore.Mvc;
using SensorMaker.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using SensorMaker.BLL;
using SensorMaker.BLL.Models.Response;
using SensorMaker.BLL.Models.Request;

namespace SensorMaker.Areas.Space.Controllers
{
    [Area("Space")]
    public class SpaceController : Controller
    {
        private global::SensorMaker.BLL.ProcessManager m_processManager = null;

        public SpaceController(global::SDMS.IDAL.IDataManager sdmsDataManager, global::Common.IDAL.IDataManager commonDataManager, global::SOPManager.IDAL.IDataManager sopDataManager, global::TeamEditor.IDAL.IDataManager teamDataManager, OptionManager optionManager)
        {
            m_processManager = new global::SensorMaker.BLL.ProcessManager(teamDataManager, commonDataManager, sdmsDataManager, sopDataManager);
        }

        [HttpPost]
        public IActionResult UploadFireSensorFile(List<IFormFile> files)
        {
            string strFilePath = GetUploadFilePath(files);

            if (strFilePath.Length == 0)
                return BadRequest();

            ResponseOpenSensorFile result = ExcelManager.OpenFireSensorFile(strFilePath);
            System.IO.File.Delete(strFilePath);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult UploadPSMSensorFile(List<IFormFile> files)
        {
            string strFilePath = GetUploadFilePath(files);

            if (strFilePath.Length == 0)
                return BadRequest();

            ResponseOpenSensorFile result = ExcelManager.OpenPSMSensorFile(strFilePath);
            System.IO.File.Delete(strFilePath);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult UploadEtcSensorFile(List<IFormFile> files)
        {
            string strFilePath = GetUploadFilePath(files);

            if (strFilePath.Length == 0)
                return BadRequest();

            ResponseOpenSensorFile result = ExcelManager.OpenEtcSensorFile(strFilePath);
            System.IO.File.Delete(strFilePath);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult UploadCCTVFile(List<IFormFile> files)
        {
            string strFilePath = GetUploadFilePath(files);

            if (strFilePath.Length == 0)
                return BadRequest();

            ResponseOpenSensorFile result = ExcelManager.OpenCCTVFile(strFilePath);
            System.IO.File.Delete(strFilePath);

            return Ok(result);
        }

        [HttpPost]
        public IActionResult UploadTempModelFile(List<IFormFile> files)
        {
            if (files.Count == 0)
                return BadRequest();

            MessageResult result = null;

            string key;
            string value = GetParmeter(files, out key);

            if (value != null && value.Length > 0 && string.Compare(key, "userID", true) == 0)
            {
                int nUserID;

                if (int.TryParse(value, out nUserID))
                {
                    result = ModelFileManager.UploadTempFile(files[0].OpenReadStream(), files[0].FileName, nUserID, Startup.TempResourceRootPath, m_processManager.TeamDataManager);
                }
            }

            if (result == null)
                result = new MessageResult(false, "파일 업로드에 실패하였습니다.");

            return Ok(result);
        }

        private string GetUploadFilePath(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            var filePath = Path.GetTempFileName();
            string strFileName = "";
            string strFilePath = "";

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    strFileName = formFile.FileName;

                    using (var stream = new FileStream(strFileName, FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                        strFilePath = stream.Name;
                        break;
                    }
                }
            }

            return strFilePath;
        }

        private string GetParmeter(List<IFormFile> files, out string strParamKey)
        {
            strParamKey = null;

            if (files.Count >= 2)
            {
                IFormFile file = files[1];
                int nIndex = file.FileName.IndexOf('_');

                if (nIndex < 0)
                    return null;

                strParamKey = file.FileName.Substring(0, nIndex).Trim();
                string strParamValue = file.FileName.Substring(nIndex + 1).Trim();
                return strParamValue;
            }

            return null;
        }

        [HttpPost]
        public IActionResult RequestData([FromBody] RequestData data)
        {
            if (data == null)
                return BadRequest();

            if (data.RequestSensorExcelFile != null)
                return RequestSensorExcelFile(data.RequestSensorExcelFile);
            else if (data.RequestUploadModelFile != null)
                return RequestUploadModelFile(data.RequestUploadModelFile);
            else if (data.RequestRemoveTempFile != null)
                return RequestRemoveTempFile(data.RequestRemoveTempFile);

            return null;
        }

        private IActionResult RequestRemoveTempFile(RequestRemoveTempFile data)
        {
            MessageResult result = ModelFileManager.RemoveTempFile(data, Startup.TempResourceRootPath);
            return Ok(result);
        }

        private IActionResult RequestUploadModelFile(RequestUploadModelFile data)
        {
            MessageResult result = ModelFileManager.UploadModelFiles(data, Startup.ResourceRootPath, Startup.TempResourceRootPath);
            return Ok(result);
        }

        public IActionResult RequestSensorExcelFile(RequestSensorExcelFile data)
        {
            string strFileName = "";
            ResponseExcelInfo response = null;

            if (data.FireSensors != null)
            {
                strFileName = "화재센서.xls";
                response = ExcelManager.DownloadSensorFile(data.FireSensors);
            }
            else if (data.PSMSensors != null)
            {
                strFileName = "누출센서.xls";
                response = ExcelManager.DownloadSensorFile(data.PSMSensors);
            }
            else if (data.EtcSensors != null)
            {
                strFileName = "기타센서.xls";
                response = ExcelManager.DownloadSensorFile(data.EtcSensors);
            }
            else if (data.Cctvs != null)
            {
                strFileName = "CCTV.xls";
                response = ExcelManager.DownloadSensorFile(data.Cctvs);
            }
            else
            {
                response = new ResponseExcelInfo(false, "변환할 센서데이터가 존재하지 않습니다.");
                return Ok(response);
            }

            byte[] bytes = response.Bytes;

            if (response.Success == false || bytes == null)
                return Ok(response);

            return File(bytes, "application/vnd.ms-excel", strFileName);
        }
    }
}
