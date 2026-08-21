using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnEService_Core.Models
{
    public class DownloadModel
    {
        public string FilePath { get; set; }
        public int SegmentIndex { get; set; }
    }

    public class FilePathModel
    {
        public string FilePath { get; set; }
    }

    public class FolderPathModel
    {
        public string Path { get; set; }
    }
}
