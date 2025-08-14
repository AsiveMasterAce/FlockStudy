using System.ComponentModel.DataAnnotations;

namespace FlockStudy.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? Username { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? PasswordHash { get; set; }

        public string? PushSubscription { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<PrayerRequest> PrayerRequests { get; set; } = new List<PrayerRequest>();
        public virtual ICollection<PrayerCommitment> PrayerCommitments { get; set; } = new List<PrayerCommitment>();
    }
}
