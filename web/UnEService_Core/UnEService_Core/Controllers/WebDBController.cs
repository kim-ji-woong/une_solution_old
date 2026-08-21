using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Pages;
using Microsoft.AspNetCore.Mvc;
using UnEService_Core.Models;
using UnEService_Core.Service;

namespace UnEService_Core.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WebDBController : ControllerBase
    {
        private WebDBService webDBService;
        private string errorMessage;

        public WebDBController()
        {
            if (webDBService == null)
            {
                webDBService = WebDBService.Instance;
                errorMessage = "";
            }
        }

        [HttpPost]
        [ActionName("RunQuery")]
        public string[] RunQuery([FromBody] DBModel dbInfo)
        {
            return webDBService.RunQuery(dbInfo.DbName, dbInfo.DbType, dbInfo.Query);
        }

        [HttpPost]
        [ActionName("RunStoredProcedure")]
        public string[] RunStoredProcedure([FromBody] ProcedureModel model)
        {
            return webDBService.RunStoredProcedure(model.DbName, model.DbType, model.ProcedureName, model.FieldNames, model.FieldValues);
        }

        [HttpPost]
        [ActionName("BeginBatch")]
        public string[] BeginBatch([FromBody] DBModel dbInfo)
        {
            long res = webDBService.BeginBatch(dbInfo.DbName, dbInfo.DbType, out errorMessage);

            return new string[] { res.ToString(), errorMessage };
        }
        
        [HttpPost]
        [ActionName("BatchCommit")]
        public string BatchCommit([FromBody] TransactionModel tranInfo)
        {
            return webDBService.BatchCommit(tranInfo.TransactionKey);
        }
        
        [HttpPost]
        [ActionName("BatchRollback")]
        public string BatchRollback([FromBody] TransactionModel tranInfo)
        {
            return webDBService.BatchRollback(tranInfo.TransactionKey);
        }
        
        [HttpPost]
        [ActionName("BatchQuery")]
        public string[] BatchQuery([FromBody] TransactionModel tranInfo)
        {
            return webDBService.BatchQuery(tranInfo.Query, tranInfo.TransactionKey);
        }

        [HttpPost]
        [ActionName("BatchStoredProcedure")]
        public string[] BatchStoredProcedure([FromBody] TransactionProcedureModel tranInfo)
        {
            return webDBService.BatchStoredProcedure(tranInfo.ProcedureName, tranInfo.FieldNames, tranInfo.FieldValues, tranInfo.TransactionKey);
        }
    }
}
