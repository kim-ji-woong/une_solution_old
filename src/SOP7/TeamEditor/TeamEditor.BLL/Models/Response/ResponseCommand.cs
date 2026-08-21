using System;
using System.Collections.Generic;
using System.Text;

namespace TeamEditor.BLL.Models.Response
{
    public class ResponseCommand
    {
        private string m_strErrorMessage = null;
        public string StrErrorMessage
        {
            get { return m_strErrorMessage; }
            set { m_strErrorMessage = value; }
        }
    }
}
