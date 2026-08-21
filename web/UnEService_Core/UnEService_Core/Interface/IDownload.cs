using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnEService_Core.Interface
{
    public interface IDownload
    {
        byte[] Download(string filePath, int segmentIndex, out int readCount, out string errorMessage);
        string[] GetFileSegmentCount(string filePath);
        bool GetFolder(string path);
    }
}
