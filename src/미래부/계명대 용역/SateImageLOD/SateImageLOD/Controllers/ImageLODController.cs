using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SateImageLOD.Models;

namespace SateImageLOD.Controllers
{
    public class ImageLODController : ApiController
    {
        // GetLastImageLODDepth
        public IHttpActionResult Get(string imageName)
        {
            int nDepth = ImageRepository.GetLastImageLODDepth(imageName);
            return Ok(nDepth);
        }

        // GetImageLODDepth
        public IHttpActionResult Get(string imageName, string time)
        {
            int nDepth = ImageRepository.GetImageLODDepth(imageName, time);
            return Ok(nDepth);
        }

        // image.Time이 null이거나 빈 문자열일 경우 GetLastImageList
        //              그렇지 않을 경우 GetImageList
        public HttpResponseMessage Post(LODImage image)
        {
            List<SateImage> images = null;

            if (image.Time == null || image.Time.Length == 0)
                images = ImageRepository.GetLastImageList(image.ImageName, image.LODIndex);
            else
                images = ImageRepository.GetImageList(image.ImageName, image.LODIndex, image.Time);

            HttpStatusCode code = images == null ? HttpStatusCode.NotAcceptable : HttpStatusCode.Created;
            var response = Request.CreateResponse<List<SateImage>>(code, images);
            return response;
        }
    }
}
