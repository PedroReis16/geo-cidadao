using GeoCidadao.Database.Configurations;
using GeoCidadao.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoCidadao.Database
{
    public partial class GeoDbContext : DbContext
    {
        public DbSet<UserProfile> UserProfiles { get; set; } = default!;
        public DbSet<UserPicture> UserPictures { get; set; } = default!;

        static GeoDbContext()
        {

        }

        public GeoDbContext(DbContextOptions<GeoDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            _ = modelBuilder.ApplyConfiguration(new UserProfileConfiguration());
            _ = modelBuilder.ApplyConfiguration(new UserPictureConfiguration());

            _ = modelBuilder.Ignore<BaseEntity>();
        }
    }
}