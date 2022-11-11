using Microsoft.EntityFrameworkCore;

namespace SolarFarm.Entities
{
    public class IotDataContext : DbContext
    {
        public IotDataContext(DbContextOptions<IotDataContext> options) : base(options)
        {
        }

        protected IotDataContext()
        {
        }

        public virtual DbSet<SolarPanel> SolarPanels { get; set; }
        public virtual DbSet<SolarPanelData> SolarPanelData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost,1434;Database=SolarFarmIot;Trusted_Connection=False;User Id=sa;Password=zaQ@123456!");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SolarPanel>(builder =>
            {
                builder.HasData(new[]
                {
                    new SolarPanel { Id = Guid.NewGuid(), Name = "Solar Panel 1" },
                    new SolarPanel { Id = Guid.NewGuid(), Name = "Solar Panel 2" },
                    new SolarPanel { Id = Guid.NewGuid(), Name = "Solar Panel 3" },
                    new SolarPanel { Id = Guid.NewGuid(), Name = "Solar Panel 4" },
                    new SolarPanel { Id = Guid.NewGuid(), Name = "Solar Panel 5" },
                    new SolarPanel { Id = Guid.NewGuid(), Name = "Solar Panel 6" },
                    new SolarPanel { Id = Guid.NewGuid(), Name = "Solar Panel 7" },
                    new SolarPanel { Id = Guid.NewGuid(), Name = "Solar Panel 8" },
                    new SolarPanel { Id = Guid.NewGuid(), Name = "Solar Panel 9" },
                    new SolarPanel { Id = Guid.NewGuid(), Name = "Solar Panel 10" },
                });
            });

            modelBuilder.Entity<SolarPanelData>(builder =>
            {
                builder.HasOne(d => d.SolarPanel)
                    .WithMany(d => d.Data)
                    .HasForeignKey(d => d.PanelId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
