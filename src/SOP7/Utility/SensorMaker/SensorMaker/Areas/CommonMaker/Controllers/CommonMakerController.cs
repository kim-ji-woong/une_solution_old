using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SensorMaker.BLL.Models.Request;
using SensorMaker.BLL.Models.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace SensorMaker.Areas.CommonMaker.Controllers
{
    [Area("CommonMaker")]
    public class CommonMakerController : Controller
    {
        private global::SensorMaker.BLL.ProcessManager m_sensorMakerProcessManager = null;

        public CommonMakerController(global::SDMS.IDAL.IDataManager sdmsDataManager, global::Common.IDAL.IDataManager commonDataManager, global::SOPManager.IDAL.IDataManager sopDataManager, global::TeamEditor.IDAL.IDataManager teamDataManager)
        {
            m_sensorMakerProcessManager = new global::SensorMaker.BLL.ProcessManager(teamDataManager, commonDataManager, sdmsDataManager, sopDataManager);
        }

        [HttpPost]
        public IActionResult RequestOpenXML(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);

            var filePath = Path.GetTempFileName();
            string strFileName = "";
            string strFilePath = "";

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    strFileName = files[0].FileName;

                    using (var stream = new FileStream(strFileName, FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                        strFilePath = stream.Name;
                    }
                }
            }

            BLL.Models.Response.ResponseOpenXML res = m_sensorMakerProcessManager.GetXmlManager().OpenXML(strFilePath);
            System.IO.File.Delete(strFilePath);

            return Ok(res);
        }

        [HttpPost]
        public IActionResult RequestData([FromBody] RequestData data)
        {
            if (data == null)
                return BadRequest();

            if (data.RequestSaveXML != null)
                return RequestSaveXML(data.RequestSaveXML);
            else if (data.RequestOpenTempXML != null)
                return RequestOpenTempXML(data.RequestOpenTempXML);

            return null;
        }

        private IActionResult RequestSaveXML(RequestSaveXML req)
        {
            ResponseSaveXML res = m_sensorMakerProcessManager.GetXmlManager().SaveXML(req);

            if (res.Success == false)
                return Ok(res);
            
            if (!req.bTempSave)
            {
                string strFilePath = "WSOP.xml";
                byte[] bytes = null;

                using (MemoryStream stream = new MemoryStream())
                {
                    res.XDocument.Save(stream);
                    bytes = stream.ToArray();
                }

                return File(bytes, "text/xml", strFilePath);
            }
            else
            {                
                string strFilePath = BLL.ModelFileManager.GetResourceFolder(req.UserID, req.UserName, Startup.TempResourceRootPath) + "\\TempXML";
                if (!Directory.Exists(strFilePath))
                    Directory.CreateDirectory(strFilePath);

                string strFileName = strFilePath + "\\temp.xml";
                using (FileStream stream = new FileStream(strFileName, FileMode.Create))
                using (XmlWriter xmlWriter = XmlWriter.Create(stream))
                {
                    res.XDocument.WriteTo(xmlWriter);
                    xmlWriter.Flush();
                }

                return Ok(true);
            }
        }

        private IActionResult RequestOpenTempXML(RequestOpenTempXML req)
        {
            BLL.Models.Response.ResponseOpenXML res = new ResponseOpenXML();
            string strFilePath = BLL.ModelFileManager.GetResourceFolder(req.UserID, req.UserName, Startup.TempResourceRootPath) + "\\TempXML\\temp.xml";
            if (!System.IO.File.Exists(strFilePath))
            {
                res.Success = false;
                res.Message = "temp.xml이 없습니다.";
                return Ok(res);
            }

            res = m_sensorMakerProcessManager.GetXmlManager().OpenXML(strFilePath);
            return Ok(res);
        }
    }
}
