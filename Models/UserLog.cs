using System.Data.SqlClient;

namespace UserManagementApi.Models
{
    public class UserLog
    {
        public int logid { get; set; }
        public String? logdate { get; set; }
        public string? ipaddress { get; set; }
        public int Id { get; set; }
        public string? activity { get; set; }    



    }
}
