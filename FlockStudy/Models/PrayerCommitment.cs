using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlockStudy.Models
{
    public class PrayerCommitment
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("PrayerRequest")]
        public int? PrayerRequestId { get; set; }
        [ForeignKey("User")]

        public int? UserId { get; set; }

        public DateTime CommittedAt { get; set; } = DateTime.UtcNow;
        public DateTime? WeekStart { get; set; }
        public bool IsCompleted { get; set; } = false;


        public virtual PrayerRequest? PrayerRequest { get; set; }
        public virtual User? User { get; set; }
    }
}
