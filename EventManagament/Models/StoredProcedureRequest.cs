using System.Collections.Generic;

namespace EventManagament.Models
{
    public class StoredProcedureRequest
    {
        public string ProcedureName { get; set; }
        public Dictionary<string, object> InputParameters { get; set; }
    }
}
