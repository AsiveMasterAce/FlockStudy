using System.ComponentModel.DataAnnotations;

namespace FlockStudy.Models.ViewModels
{
    public class PrayerViewModel
    {
    }
    public class AddPrayerViewModel
    {
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }
    }
}