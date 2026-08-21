using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnEService_Core.Models;
using UnEService_Core.Service;

namespace UnEService_Core.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        private DownloadService downloadService;

        public DownloadController()
        {
            if (downloadService == null)
            {
                downloadService = DownloadService.Instance;
            }
        }

        [HttpPost]
        [ActionName("")]
        public FileResult Download([FromBody] DownloadModel downloadInfo)
        {
            int readCount = 0;
            string errorMessage = "";
            byte[] resTemp = downloadService.Download(downloadInfo.FilePath, downloadInfo.SegmentIndex, out readCount, out errorMessage);

            errorMessage = "This is Test Message";

            byte[] intToByteArray = BitConverter.GetBytes(readCount);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(intToByteArray);
            }

            byte[] stringToByteArray = Encoding.UTF8.GetBytes(errorMessage);

            int byteArrayLength = stringToByteArray.Length;
            byte[] lengthByteArray = BitConverter.GetBytes(byteArrayLength);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(lengthByteArray);
            }
            Array.Reverse(lengthByteArray);
            Array.Resize(ref lengthByteArray, 8);
            Array.Reverse(lengthByteArray);
            

            byte[] res = new byte[resTemp.Length + intToByteArray.Length + stringToByteArray.Length + lengthByteArray.Length];
            Array.Copy(resTemp, 0, res, 0, resTemp.Length);
            Array.Copy(intToByteArray, 0, res, resTemp.Length, intToByteArray.Length);
            Array.Copy(stringToByteArray, 0, res, resTemp.Length + intToByteArray.Length, stringToByteArray.Length);
            Array.Copy(lengthByteArray, 0, res, resTemp.Length + intToByteArray.Length + stringToByteArray.Length, lengthByteArray.Length);

            return File(res, "application/octet-stream");
        }

        [HttpPost]
        [ActionName("GetFileSegmentCount")]
        public string[] GetFileSegmentCount(FilePathModel segmentInfo)
        {
            return downloadService.GetFileSegmentCount(segmentInfo.FilePath);
        }

        [HttpPost]
        [ActionName("GetFolder")]
        public bool GetFolder(FolderPathModel folderInfo)
        {
            return downloadService.GetFolder(folderInfo.Path);
        }
    }
}
