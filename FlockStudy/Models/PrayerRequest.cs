using System.ComponentModel.DataAnnotations;

namespace FlockStudy.Models
{
    public class PrayerRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int? UserId { get; set; }

        [Required]
        [StringLength(200)]
        public string? Title { get; set; }

        [Required]
        [StringLength(1000)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


        public virtual User? User { get; set; }
        public virtual ICollection<PrayerCommitment> PrayerCommitments { get; set; } = new List<PrayerCommitment>();
    }
}
