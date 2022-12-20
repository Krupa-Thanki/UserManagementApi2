using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UserManagementApi.Models
{
    public class Registration
    {
        public int Id { get; set; }
        
        public string? Username { get; set; }
        public string? Password { get; set; }
       
        public string? Email { get; set; }
        public int Isactive { get; set; }
        
        public int Isblock { get; set; }
             
        public string? newname { get; set; }
        
        public string? newpwd { get; set; }
       
    }
}
