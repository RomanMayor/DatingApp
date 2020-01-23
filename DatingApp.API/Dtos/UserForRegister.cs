using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class UserForRegister
    {
        [Required]
        public string UserName { get; set; }
        
        [Required]
        [StringLength(8, MinimumLength=4, ErrorMessage="from 4 to 8")]
        public string Password { get; set; }
    }
}