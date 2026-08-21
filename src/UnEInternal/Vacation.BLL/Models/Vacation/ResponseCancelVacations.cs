namespace Vacation.BLL.Models.Vacation
{
    public class ResponseCancelVacations : MessageResult
    {
        private History m_history = null;
        private History m_historyNextYear = null;

        public History History
        {
            get { return m_history; }
            set { m_history = value; }
        }

        public History HistoryNextYear
        {
            get { return m_historyNextYear; }
            set { m_historyNextYear = value; }
        }

        public ResponseCancelVacations()
        {
        }

        public ResponseCancelVacations(bool success, string strMessage)
            : base(success, strMessage)
        {
        }

        public ResponseCancelVacations(bool success, string strMessage, History history, History historyNextYear)
            : base(success, strMessage)
        {
            m_history = history;
            m_historyNextYear = history;
        }
    }
}
