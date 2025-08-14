using FlockStudy.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace FlockStudy.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<PrayerRequest> PrayerRequests { get; set; }
        public DbSet<PrayerCommitment> PrayerCommitments { get; set; }

      
    }
}
