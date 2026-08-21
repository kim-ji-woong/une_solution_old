namespace UnEService_Core.Interface
{
    public interface IUpload
    {
        string Upload(string fileName, byte[] bytes, bool isFirst, string folderPath);
        int GetMaxSegmentSize();
        string RemoveFile(string fileName, string folderPath);
        string RemoveAll(string folderPath);
        string ExtractToTrg(string strSrcFile, string strTrgPath);
    }
}
