using FluentAssertions;
using GeoCidadao.Database;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.CacheContracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.EFDao;
using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;
using GeoCidadao.Models.Exceptions;
using GeoCidadao.TestShared.Helpers;
using Moq;
using Moq.EntityFrameworkCore;

namespace GeoCidadao.GerenciamentoUsuariosAPI.UnitTests.Database.UserPictureDaoTests
{
    public class AddOrUpdatePictureAsyncTests
    {
        private readonly UserPictureDao _contextDao;
        private readonly Mock<GeoDbContext> _contextMock;
        private readonly Mock<IUserPictureDaoCache> _cacheMock;

        public AddOrUpdatePictureAsyncTests()
        {
            _cacheMock = new Mock<IUserPictureDaoCache>();
            _contextMock = GeoCidadaoDatabaseMockTest.CreateMockContext();
            _contextDao = new UserPictureDao(_contextMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task AddOrUpdatePictureAsync_GivenAnUserWithProfilePicture_ShouldUpdateUserPicture()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string newFileExtension = ".jpg";
            string newFileHash = "newFileHash123";

            UserPicture existingPicture = new UserPicture
            {
                Id = userId,
                FileExtension = ".png",
                FileHash = "oldFileHash456",
                CreatedAt = DateTime.Now.AddDays(-10).ToUniversalTime(),
                UpdatedAt = DateTime.Now.AddDays(-5).ToUniversalTime()
            };

            _contextMock.Setup(db => db.Set<UserPicture>()).ReturnsDbSet(new List<UserPicture> { existingPicture });

            // Act
            await _contextDao.AddOrUpdatePictureAsync(userId, newFileExtension, newFileHash);

            // Assert
            _contextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);

            existingPicture.FileHash.Should().Be(newFileHash);
            existingPicture.UpdatedAt.Should().BeAfter(existingPicture.CreatedAt);
            existingPicture.FileExtension.Should().Be(".jpg");
        }

        [Fact]
        public async Task AddOrUpdatePictureAsync_GivenAnUserWithProfilePicture_ShouldRemoveOldPictureFromCache()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string newFileExtension = ".jpg";
            string newFileHash = "newFileHash123";

            UserPicture existingPicture = new UserPicture
            {
                Id = userId,
                FileExtension = ".png",
                FileHash = "oldFileHash456",
                CreatedAt = DateTime.Now.AddDays(-10).ToUniversalTime(),
                UpdatedAt = DateTime.Now.AddDays(-5).ToUniversalTime()
            };

            _contextMock.Setup(db => db.Set<UserPicture>()).ReturnsDbSet(new List<UserPicture> { existingPicture });

            // Act
            await _contextDao.AddOrUpdatePictureAsync(userId, newFileExtension, newFileHash);

            // Assert
            _cacheMock.Verify(cache => cache.RemoveEntity(existingPicture), Times.Once);
        }

        [Fact]
        public async Task AddOrUpdatePictureAsync_GivenAnUserWithProfilePictureWithoutCache_ShouldNotThrowException()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string newFileExtension = ".jpg";
            string newFileHash = "newFileHash123";

            UserPicture existingPicture = new UserPicture
            {
                Id = userId,
                FileExtension = ".png",
                FileHash = "oldFileHash456",
                CreatedAt = DateTime.Now.AddDays(-10).ToUniversalTime(),
                UpdatedAt = DateTime.Now.AddDays(-5).ToUniversalTime()
            };

            _contextMock.Setup(db => db.Set<UserPicture>()).ReturnsDbSet(new List<UserPicture> { existingPicture });
            UserPictureDao contextDaoWithoutCache = new UserPictureDao(_contextMock.Object, null!);

            // Act
            Func<Task> act = async () => await contextDaoWithoutCache.AddOrUpdatePictureAsync(userId, newFileExtension, newFileHash);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task AddOrUpdatePictureAsync_GivenAnInexistentUser_ShouldThrowEntityValidationException()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string fileExtension = ".jpg";
            string fileHash = "fileHash123";

            _contextMock.Setup(db => db.Set<UserPicture>()).ReturnsDbSet(new List<UserPicture>());
            _contextMock.Setup(db => db.Set<UserProfile>()).ReturnsDbSet(new List<UserProfile>());

            // Act
            Func<Task> act = async () => await _contextDao.AddOrUpdatePictureAsync(userId, fileExtension, fileHash);

            // Assert
            await act.Should().ThrowAsync<EntityValidationException>();
        }

        [Fact]
        public async Task AddOrUpdatePictureAsync_GivenAnUserWithoutProfilePicture_ShouldAddUserPicture()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string fileExtension = ".jpg";
            string fileHash = "fileHash123";

            UserProfile userProfile = new UserProfile
            {
                Id = userId,
                CreatedAt = DateTime.Now.AddDays(-15).ToUniversalTime(),
                UpdatedAt = DateTime.Now.AddDays(-10).ToUniversalTime()
            };

            _contextMock.Setup(db => db.Set<UserPicture>()).ReturnsDbSet(new List<UserPicture>());
            _contextMock.Setup(db => db.Set<UserProfile>()).ReturnsDbSet(new List<UserProfile> { userProfile });

            // Act
            await _contextDao.AddOrUpdatePictureAsync(userId, fileExtension, fileHash);

            // Assert
            _contextMock.Verify(db => db.SaveChangesAsync(default), Times.Once);
            _contextMock.Verify(db => db.Set<UserPicture>().Add(It.Is<UserPicture>(p =>
                p.Id == userId &&
                p.FileExtension == fileExtension &&
                p.FileHash == fileHash &&
                p.User == userProfile
            )), Times.Once);
        }
    }
}