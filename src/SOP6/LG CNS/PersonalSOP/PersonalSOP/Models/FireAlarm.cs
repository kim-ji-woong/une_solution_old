using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PersonalSOP.Models
{
    public class FireAlarm
    {
        private string m_strLocation = "";
        private string m_strDisasterInfo = "";
        private bool m_isRealMode = false;

        [Required]
        public string Location
        {
            get { return m_strLocation; }
            set { m_strLocation = value; }
        }

        public string DisasterInfo
        {
            get { return m_strDisasterInfo; }
            set { m_strDisasterInfo = value; }
        }

        public bool RealMode
        {
            get { return m_isRealMode; }
            set { m_isRealMode = value; }
        }
    }
}