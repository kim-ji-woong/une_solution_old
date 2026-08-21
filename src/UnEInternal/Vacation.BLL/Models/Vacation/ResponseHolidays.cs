using System.Collections.Generic;

namespace Vacation.BLL.Models.Vacation
{
    public class ResponseHolidays : MessageResult
    {
        private List<int> m_holidays = new List<int>();

        public List<int> Holidays
        {
            get { return m_holidays; }
            set { m_holidays = value; }
        }

        public ResponseHolidays()
            : base()
        {
        }

        public ResponseHolidays(bool success, string message)
            : base(success, message)
        {
        }
    }
}
