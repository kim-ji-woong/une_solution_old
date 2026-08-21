using System.Collections.Generic;

namespace Vacation.BLL.Rollback
{
    using Model;
    using Vacation.IDAL;

    public class InsertRollbackData : IRollbackData
    {
        private List<Request> m_insertRequests = new List<Request>();
        private List<Response> m_insertResponses = new List<Response>();

        public void AddInsertRequest(Request request)
        {
            m_insertRequests.Add(request);
        }

        public void AddInsertResponse(Response response)
        {
            m_insertResponses.Add(response);
        }

        public bool Rollback(IDataManager dataManager)
        {
            foreach (Request request in m_insertRequests)
            {
                if (dataManager.GetCreateManager().CreateRequest(request.ID, request.RequestTime, request.MemberID, request.Days, request.ManagerIDs, request.Response, request.RequestDescription, request.Year, request.Year2, request.MailSendTime) == null)
                    return false;
            }

            foreach (Response response in m_insertResponses)
            {
                if (dataManager.GetCreateManager().CreateResponse(response.ID, response.RequestID, response.ManagerID, response.Result, response.ResponseTime, response.Description, response.PrevResponseID) == null)
                    return false;
            }

            return true;
        }
    }
}
