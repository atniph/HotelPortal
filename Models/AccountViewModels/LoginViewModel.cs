using System.ComponentModel.DataAnnotations;

namespace HotelPortal.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "You must enter a valid username.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "You must enter a valid password.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
