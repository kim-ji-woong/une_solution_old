using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SateImageLOD.Models;

namespace SateImageLOD.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string strDBPath = GetMDFPath();
            ImageRepository.ConnectDB(strDBPath);

            ViewBag.Title = "Home Page";
            return View();
        }

        private string GetMDFPath()
        {
            string strMDFPath = "\\Common\\Data\\EdenSensors.mdf";
            string strCurrentPath = Server.MapPath(".");

            int nIndex = strCurrentPath.LastIndexOf('\\');

            if (nIndex < 0)
                return strCurrentPath + strMDFPath;

            int nIndex2 = strCurrentPath.LastIndexOf('\\', nIndex - 1);

            if (nIndex2 < 0)
                return strCurrentPath + strMDFPath;

            string strPath = strCurrentPath.Substring(0, nIndex2) + strMDFPath;
            return strPath;
        }
    }
}
