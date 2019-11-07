using System;
using System.ComponentModel.DataAnnotations;

namespace HotelPortal.Models.AccountViewModels
{
    public class RegisterViewModel : SiteUserViewModel
    {

        /// <summary>
        /// Username.
        /// </summary>
        [Required(ErrorMessage = "Yout must enter a valid username.")]
        [RegularExpression("^[A-Za-z0-9_-]{5,40}$", ErrorMessage = "Invalid username")]
        public String UserName { get; set; }

        /// <summary>
        /// Password.
        /// </summary>
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        /// <summary>
        /// ConfirmPassword.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
