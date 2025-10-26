using GeoCidadao.Database.Configurations;
using GeoCidadao.Model.Entities;
using GeoCidadao.Database.Configurations.GerenciamentoUsuariosAPI;
using GeoCidadao.Model.Entities.GerenciamentoUsuariosAPI;
using Microsoft.EntityFrameworkCore;

namespace GeoCidadao.Database
{
    public partial class GeoDbContext : DbContext
    {
        //Gerenciamento de usuários API
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

            // Gerenciamento de usuários API
            _ = modelBuilder.ApplyConfiguration(new UserProfileConfiguration());
            _ = modelBuilder.ApplyConfiguration(new UserPictureConfiguration());

            _ = modelBuilder.Ignore<BaseEntity>();
        }
    }
}