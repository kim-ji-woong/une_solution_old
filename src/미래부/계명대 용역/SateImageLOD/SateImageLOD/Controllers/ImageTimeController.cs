using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SateImageLOD.Models;

namespace SateImageLOD.Controllers
{
    public class ImageTimeController : ApiController
    {
        // GetImageTimeList
        public IHttpActionResult Get(string imageName)
        {
            List<SateImage> images = ImageRepository.GetImageList(imageName);

            if (images == null)
                return NotFound();

            return Ok(images);
        }
    }
}
