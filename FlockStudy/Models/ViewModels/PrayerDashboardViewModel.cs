namespace FlockStudy.Models.ViewModels
{
    public class PrayerDashboardViewModel
    {
        public List<PrayerRequestCardViewModel> AvailableRequests { get; set; } = new();
        public List<PrayerRequestCardViewModel> MyCommitments { get; set; } = new();
        public List<PrayerRequestCardViewModel> MyRequests { get; set; } = new();
    }
    public class PrayerRequestCardViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? RequestedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsCommittedByCurrentUser { get; set; }
        public bool IsCompletedThisWeek { get; set; }
        public int CommitmentCount { get; set; }
    }
}
