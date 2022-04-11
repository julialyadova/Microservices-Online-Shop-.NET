using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APIGateway.Models
{
    public class UserModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        public ICollection<string> Roles { get; set; }
    }
}
