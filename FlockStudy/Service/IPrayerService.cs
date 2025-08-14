using FlockStudy.Models.ViewModels;
using FlockStudy.Models;
using FlockStudy.Data;
using Microsoft.EntityFrameworkCore;

namespace FlockStudy.Service
{
    public interface IPrayerService
    {
        Task<PrayerDashboardViewModel> GetDashboardAsync(int userId);
        Task<bool> CommitToPrayerAsync(int userId, int prayerRequestId);
        Task<bool> MarkCompletedAsync(int userId, int prayerRequestId);
        Task<PrayerRequest> CreatePrayerRequestAsync(int userId, string title, string description);
    }
    public class PrayerService : IPrayerService
    {
        private readonly ApplicationDbContext _context;

        public PrayerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PrayerDashboardViewModel> GetDashboardAsync(int userId)
        {
            var currentWeekStart = GetWeekStart(DateTime.UtcNow);

            var availableRequests = await _context.PrayerRequests
                .Where(pr => pr.IsActive && pr.UserId != userId)
                .Include(pr => pr.User)
                .Include(pr => pr.PrayerCommitments.Where(pc => pc.WeekStart == currentWeekStart))
                .Select(pr => new PrayerRequestCardViewModel
                {
                    Id = pr.Id,
                    Title = pr.Title,
                    Description = pr.Description,
                    RequestedBy = pr.User.Username,
                    CreatedAt = pr.CreatedAt,
                    IsCommittedByCurrentUser = pr.PrayerCommitments.Any(pc => pc.UserId == userId),
                    IsCompletedThisWeek = pr.PrayerCommitments.Any(pc => pc.UserId == userId && pc.IsCompleted),
                    CommitmentCount = pr.PrayerCommitments.Count()
                })
                .ToListAsync();

            var myCommitments = await _context.PrayerCommitments
                .Where(pc => pc.UserId == userId && pc.WeekStart == currentWeekStart)
                .Include(pc => pc.PrayerRequest)
                .ThenInclude(pr => pr.User)
                .Select(pc => new PrayerRequestCardViewModel
                {
                    Id = pc.PrayerRequest.Id,
                    Title = pc.PrayerRequest.Title,
                    Description = pc.PrayerRequest.Description,
                    RequestedBy = pc.PrayerRequest.User.Username,
                    CreatedAt = pc.PrayerRequest.CreatedAt,
                    IsCommittedByCurrentUser = true,
                    IsCompletedThisWeek = pc.IsCompleted,
                    CommitmentCount = pc.PrayerRequest.PrayerCommitments.Count(pcc => pcc.WeekStart == currentWeekStart)
                })
                .ToListAsync();

            var myRequests = await _context.PrayerRequests
                .Where(pr => pr.UserId == userId && pr.IsActive)
                .Include(pr => pr.PrayerCommitments.Where(pc => pc.WeekStart == currentWeekStart))
                .Select(pr => new PrayerRequestCardViewModel
                {
                    Id = pr.Id,
                    Title = pr.Title,
                    Description = pr.Description,
                    RequestedBy = "You",
                    CreatedAt = pr.CreatedAt,
                    IsCommittedByCurrentUser = false,
                    IsCompletedThisWeek = false,
                    CommitmentCount = pr.PrayerCommitments.Count()
                })
                .ToListAsync();

            return new PrayerDashboardViewModel
            {
                AvailableRequests = availableRequests,
                MyCommitments = myCommitments,
                MyRequests = myRequests
            };
        }

        public async Task<bool> CommitToPrayerAsync(int userId, int prayerRequestId)
        {
            var currentWeekStart = GetWeekStart(DateTime.UtcNow);

            var existingCommitment = await _context.PrayerCommitments
                .FirstOrDefaultAsync(pc => pc.UserId == userId &&
                                         pc.PrayerRequestId == prayerRequestId &&
                                         pc.WeekStart == currentWeekStart);

            if (existingCommitment != null)
                return false;

            var commitment = new PrayerCommitment
            {
                UserId = userId,
                PrayerRequestId = prayerRequestId,
                WeekStart = currentWeekStart
            };

            _context.PrayerCommitments.Add(commitment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkCompletedAsync(int userId, int prayerRequestId)
        {
            var currentWeekStart = GetWeekStart(DateTime.UtcNow);

            var commitment = await _context.PrayerCommitments
                .FirstOrDefaultAsync(pc => pc.UserId == userId &&
                                         pc.PrayerRequestId == prayerRequestId &&
                                         pc.WeekStart == currentWeekStart);

            if (commitment == null)
                return false;

            commitment.IsCompleted = !commitment.IsCompleted;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PrayerRequest> CreatePrayerRequestAsync(int userId, string title, string description)
        {
            var request = new PrayerRequest
            {
                UserId = userId,
                Title = title,
                Description = description
            };

            _context.PrayerRequests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        private DateTime GetWeekStart(DateTime date)
        {
            var diff = (7 + (date.DayOfWeek - DayOfWeek.Sunday)) % 7;
            return date.AddDays(-1 * diff).Date;
        }
    }
}
