using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PersonalSOP.Models
{
    public class SOPTalkMessage
    {
        private string m_strContentsWriter = "";
        private string m_strMessage = "";
        private string m_strImageURL = "";
        private string m_strFileName = "";
        private List<string> m_comments = new List<string>();
        private HttpPostedFileBase m_file = null;
        private string m_strUploadResult = "";
        private string m_strTitle = "";

        /// <summary>
        /// 작성자
        /// </summary>
        public string Writer
        {
            get { return m_strContentsWriter; }
            set { m_strContentsWriter = value; }
        }

        public HttpPostedFileBase File
        {
            get { return m_file; }
            set { m_file = value; }
        }

        public string Message
        {
            get { return m_strMessage; }
            set { m_strMessage = value; }
        }
        
        public string ImageURL
        {
            get { return m_strImageURL; }
            set { m_strImageURL = value; }
        }

        public string FileName
        {
            get { return m_strFileName; }
            set { m_strFileName = value; }
        }

        public string UploadResult
        {
            get { return m_strUploadResult; }
            set { m_strUploadResult = value; }
        }

        public string Title
        {
            get { return m_strTitle; }
            set { m_strTitle = value; }
        }

        /// <summary>
        /// 댓글
        /// </summary>
        public List<string> Comments
        {
            get { return m_comments; }
        }
    }
}
