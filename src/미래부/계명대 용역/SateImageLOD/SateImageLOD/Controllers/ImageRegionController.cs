using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SateImageLOD.Models;

namespace SateImageLOD.Controllers
{
    public class ImageRegionController : ApiController
    {
        // GetImageNameList
        public HttpResponseMessage Post(Rect rect)
        {
            List<SateImage> images = ImageRepository.GetImageList(rect.TLx, rect.TLy, rect.BLx, rect.BLy, rect.BRx, rect.BRy);

            HttpStatusCode code = images == null ? HttpStatusCode.NotAcceptable : HttpStatusCode.Created;
            var response = Request.CreateResponse<List<SateImage>>(code, images);
            return response;
        }
    }
}
