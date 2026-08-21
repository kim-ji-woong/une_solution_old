using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SateImageLOD.Models;

namespace SateImageLOD.Controllers
{
    public class SateImagesController : ApiController
    {
        // AddImage
        public HttpResponseMessage Post(SateImage image)
        {
            image.ID = ImageRepository.LastImageID;
            ImageRepository.AddImage(image);

            var response = Request.CreateResponse<SateImage>(System.Net.HttpStatusCode.Created, image);
            return response;
        }

        // GetImage
        public IHttpActionResult Get(int id)
        {
            SateImage image = ImageRepository.GetImage(id);

            if (image == null)
                return NotFound();

            return Ok(image);
        }
    }
}
