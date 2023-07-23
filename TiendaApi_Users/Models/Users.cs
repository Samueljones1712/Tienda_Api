using System.ComponentModel.DataAnnotations;

namespace TiendaApi_Users.Models
{
    public class Users
    {
        [Key]
        public int idUsers { get; set; }
        public string username { get; set; }
        public string fullName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string rol { get; set; }

    }
}
