using GeoCidadao.Database;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace GeoCidadao.TestShared.Helpers
{
    public class GeoCidadaoDatabaseMockTest
    {
        public static GeoDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<GeoDbContext>()
                .UseSqlite("Filename=:memory:")
                .Options;

            var context = new GeoDbContext(options);
            context.Database.OpenConnection();
            context.Database.EnsureCreated();
            return context;
        }

        public static Mock<GeoDbContext> CreateMockContext()
        {
            return new Mock<GeoDbContext>(new DbContextOptions<GeoDbContext>());
        }
    }
}