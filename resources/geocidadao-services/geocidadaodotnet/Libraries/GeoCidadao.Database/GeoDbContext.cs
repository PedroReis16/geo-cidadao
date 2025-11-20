using GeoCidadao.Database.Configurations.GerenciamentoPostsAPI;
using GeoCidadao.Models.Entities.GerenciamentoPostsAPI;
using GeoCidadao.Models.Entities;
using GeoCidadao.Database.Configurations.GerenciamentoUsuariosAPI;
using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;
using Microsoft.EntityFrameworkCore;
using GeoCidadao.Database.Entities.GerenciamentoPostsAPI;
using GeoCidadao.Models.Enums;

namespace GeoCidadao.Database
{
    public partial class GeoDbContext : DbContext
    {
        //Gerenciamento de usuários API
        public DbSet<UserProfile> UserProfiles { get; set; } = default!;
        public DbSet<UserPicture> UserPictures { get; set; } = default!;
        public DbSet<UserInterests> UserInterests { get; set; } = default!;

        //Gerenciamento de posts API
        public DbSet<Post> Posts { get; set; } = default!;
        public DbSet<PostMedia> PostMedias { get; set; } = default!;
        public DbSet<PostLocation> PostLocations { get; set; } = default!;
        public DbSet<PostLike> PostLikes { get; set; } = default!;
        public DbSet<PostComment> PostComments { get; set; } = default!;

        //Analytics Service API
        

        static GeoDbContext()
        {

        }

        public GeoDbContext(DbContextOptions<GeoDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            _ = modelBuilder.HasPostgresEnum<PostCategory>(name: "post_categories");

            // Gerenciamento de usuários API
            _ = modelBuilder.ApplyConfiguration(new UserProfilesConfiguration());
            _ = modelBuilder.ApplyConfiguration(new UsersPictureConfiguration());
            _ = modelBuilder.ApplyConfiguration(new UserInterestsConfiguration());

            // Gerenciamento de posts API

            _ = modelBuilder.ApplyConfiguration(new PostsConfiguration());
            _ = modelBuilder.ApplyConfiguration(new PostMediasConfiguration());
            _ = modelBuilder.ApplyConfiguration(new PostLocationConfiguration());
            _ = modelBuilder.ApplyConfiguration(new PostLikesConfiguration());
            _ = modelBuilder.ApplyConfiguration(new PostCommentsConfiguration());
            // Analytics Service API
            
            _ = modelBuilder.Ignore<BaseEntity>();
        }
    }
}