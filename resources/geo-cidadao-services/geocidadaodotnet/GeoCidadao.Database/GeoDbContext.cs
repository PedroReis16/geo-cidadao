using GeoCidadao.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoCidadao.Database
{
    public class GeoDbContext : DbContext
    {
        static GeoDbContext()
        {

        }

        public GeoDbContext(DbContextOptions<GeoDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            _ = modelBuilder.Ignore<BaseEntity>();
        }
    }
}