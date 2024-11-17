using System.Data;

namespace EventManagament.Models
{
    public class ProcedureParameterModel
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public DbType DbType { get; set; } = DbType.String; // Mặc định là DbType.String nếu không được chỉ định
        public ParameterDirection Direction { get; set; } = ParameterDirection.Input; // Mặc định là Input
    }
}
