using System.Collections.Generic;

namespace Vacation.BLL.Rollback
{
    using Model;
    using Vacation.IDAL;

    public class UpdateRollbackData : IRollbackData
    {
        private List<Response> m_updateResponses = new List<Response>();
        private List<Request> m_updateRequests = new List<Request>();
        private List<History> m_updateHistories = new List<History>();

        public void AddUpdateResponse(Response response)
        {
            m_updateResponses.Add(response);
        }

        public void AddUpdateRequest(Request request)
        {
            m_updateRequests.Add(request);
        }

        public void AddUpdateHistory(History history)
        {
            m_updateHistories.Add(history);
        }

        public bool Rollback(IDataManager dataManager)
        {
            string strErrorMessage;

            foreach (Response response in m_updateResponses)
            {
                if (dataManager.GetUpdateManager().UpdateResponse(response, out strErrorMessage) == false)
                    return false;
            }

            foreach (Request request in m_updateRequests)
            {
                if (dataManager.GetUpdateManager().UpdateRequest(request, out strErrorMessage) == false)
                    return false;
            }

            foreach (History history in m_updateHistories)
            {
                if (dataManager.GetUpdateManager().UpdateHistory(history, out strErrorMessage) == false)
                    return false;
            }

            return true;
        }
    }
}
