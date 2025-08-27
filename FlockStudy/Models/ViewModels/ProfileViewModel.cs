using System.ComponentModel.DataAnnotations;

namespace FlockStudy.Models.ViewModels
{
    public class ProfileViewModel
    {
    }
    public class UpdateUsernameViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string? NewUsername { get; set; }
    }

    public class UpdatePasswordViewModel
    {
        [Required(ErrorMessage = "Current password is required.")]
        public string? CurrentPassword { get; set; }

        [Required(ErrorMessage = "New password is required.")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Please confirm your new password.")]
        public string? ConfirmNewPassword { get; set; }
    }
}
