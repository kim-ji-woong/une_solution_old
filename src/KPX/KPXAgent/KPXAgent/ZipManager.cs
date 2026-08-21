using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KPXAgent
{
    public class ZipManager
    {
        public static bool ExtractToTrg(string strSrcFile, string strTrgPath)
        {
            try
            {
                if (!Directory.Exists(strTrgPath))
                    Directory.CreateDirectory(strTrgPath);

                System.IO.FileStream fs = new System.IO.FileStream(strSrcFile,
                                                     System.IO.FileMode.Open,
                                             System.IO.FileAccess.Read, System.IO.FileShare.Read);

                ICSharpCode.SharpZipLib.Zip.ZipInputStream zis =
                                        new ICSharpCode.SharpZipLib.Zip.ZipInputStream(fs);

                ICSharpCode.SharpZipLib.Zip.ZipEntry ze;

                while ((ze = zis.GetNextEntry()) != null)
                {
                    if (!ze.IsDirectory)
                    {
                        string fileName = System.IO.Path.GetFileName(ze.Name);

                        string destDir = System.IO.Path.Combine(strTrgPath,
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
                System.Diagnostics.Trace.WriteLine(e.Message);
                return false;
            }

            return true;
            //return Core.UZip.ExtractFile(strSrcFile, strTrgPath);
        }
    }
}
