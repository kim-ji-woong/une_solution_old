using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOPWebAPI.Models
{
    public class ResponseResult
    {
        public bool Success
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }
    }
}