using System;

namespace Vacation.BLL
{
    using IDAL;

    public class ProcessManager
    {
        private IDataManager m_dataManager = null;
        private AccountManager m_accountManager = null;
        private VacationManager m_vacationManager = null;
        private TeamManager m_teamManager = null;

        public ProcessManager(IDataManager dataManager)
        {
            m_dataManager = dataManager;
            m_accountManager = new AccountManager(m_dataManager, this);
            m_vacationManager = new VacationManager(m_dataManager, this);
            m_teamManager = new TeamManager(m_dataManager, this);

            ScheduleManager.InitInstance(dataManager, this);
            KakaoManager.InitInstance(dataManager);
        }

        public AccountManager GetAccountManager()
        {
            return m_accountManager;
        }

        public VacationManager GetVacationManager()
        {
            return m_vacationManager;
        }

        public TeamManager GetTeamManager()
        {
            return m_teamManager;
        }

        public IDataManager GetDataManager()
        {
            return m_dataManager;
        }
    }
}
