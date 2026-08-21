using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnEService_Core.Models
{
    public class DBModel
    {
        public string DbName { get; set; }
        public string DbType { get; set; }
        public string Query { get; set; }
    }

    public class ProcedureModel
    {
        public string DbName { get; set; }
        public string DbType { get; set; }
        public string ProcedureName { get; set; }
        public List<string> FieldNames { get; set; }
        public List<string> FieldValues { get; set; }
    }

    public class TransactionModel
    {
        public string Query { get; set; }
        public long TransactionKey { get; set; }
    }

    public class TransactionProcedureModel
    {
        public string ProcedureName { get; set; }
        public List<string> FieldNames { get; set; }
        public List<string> FieldValues { get; set; }
        public long TransactionKey { get; set; }
    }
}
