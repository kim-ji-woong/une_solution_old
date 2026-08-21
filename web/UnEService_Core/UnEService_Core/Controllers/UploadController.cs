using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnEService_Core.Models;
using UnEService_Core.Service;
using System.Diagnostics;
using System.Text;

namespace UnEService_Core.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private UploadService uploadService;

        public UploadController()
        {
            if (uploadService == null)
            {
                uploadService = UploadService.Instance;
            }
        }

        [HttpPost]
        [ActionName("")]
        public string Upload([FromForm] UploadModel uploadInfo)
        {
            // 28 MB 까지 테스트, 우선 기존의 20 MB로 사용
            //byte[] stringToByteArr = Encoding.UTF8.GetBytes(uploadInfo.Bytes);

            string res = "";

            using (var ms = new MemoryStream())
            {
                uploadInfo.File.CopyTo(ms);
                res = uploadService.Upload(uploadInfo.File.FileName, ms.ToArray(), uploadInfo.IsFirst, uploadInfo.FolderPath);
            }

            return res;
        }

        [HttpPost]
        [ActionName("GetMaxSegmentSize")]
        public int GetMaxSegmentSize()
        {
            return uploadService.GetMaxSegmentSize();
        }

        [HttpPost]
        [ActionName("RemoveFile")]
        public string RemoveFile([FromBody] RemoveModel removeInfo)
        {
            return uploadService.RemoveFile(removeInfo.FileName, removeInfo.FolderPath);
        }

        [HttpPost]
        [ActionName("RemoveAll")]
        public string RemoveAll([FromBody] RemoveModel removeInfo)
        {
            return uploadService.RemoveAll(removeInfo.FolderPath);
        }

        [HttpPost]
        [ActionName("ExtractToTrg")]
        public string ExtractToTrg([FromBody] ExtractModel extractInfo)
        {
            return uploadService.ExtractToTrg(extractInfo.SrcFile, extractInfo.TrgPath);
        }
    }
}
