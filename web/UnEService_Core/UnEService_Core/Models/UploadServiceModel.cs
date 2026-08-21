using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnEService_Core.Models
{
    public class UploadModel
    {
        public IFormFile File { get; set; }
        public bool IsFirst { get; set; }
        public string FolderPath { get; set; }
    }

    public class RemoveModel
    {
        public string FileName { get; set; }
        public string FolderPath { get; set; }
    }

    public class ExtractModel
    {
        public string SrcFile { get; set; }
        public string TrgPath { get; set; }
    }
}
