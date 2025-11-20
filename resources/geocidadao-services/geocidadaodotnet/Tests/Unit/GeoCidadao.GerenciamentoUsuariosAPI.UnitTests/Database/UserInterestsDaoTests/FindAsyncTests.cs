using GeoCidadao.Database;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.CacheContracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.EFDao;
using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;
using GeoCidadao.TestShared.Fixtures;
using GeoCidadao.TestShared.Helpers;
using Moq;
using FluentAssertions;
using Moq.EntityFrameworkCore;

namespace GeoCidadao.GerenciamentoUsuariosAPI.UnitTests.Database.UserInterestsDaoTests
{
    public class FindAsyncTests
    {
        private readonly UserInterestsDao _contextDao;
        private readonly Mock<GeoDbContext> _contextMock;
        private readonly Mock<IUserInterestsDaoCache> _cacheMock;

        public FindAsyncTests()
        {
            _cacheMock = new Mock<IUserInterestsDaoCache>();
            _contextMock = GeoCidadaoDatabaseMockTest.CreateMockContext();
            _contextDao = new UserInterestsDao(_contextMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task FindAsync_GivenAnExistentUserId_ShouldReturnUserInterestsFromCache()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            UserInterests trackedUser = UserInterestsFixtures.CreateUserInterests(userId: userId);

            _cacheMock.Setup(c => c.GetEntity(It.IsAny<string>())).Returns(trackedUser);

            // Act
            UserInterests? result = await _contextDao.FindAsync(userId, track: false);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(trackedUser, result);
            _contextMock.Verify(c => c.Set<UserInterests>(), Times.Never);
            _cacheMock.Verify(c => c.AddEntity(It.IsAny<UserInterests>()), Times.Never);
            _cacheMock.Verify(c => c.GetEntity(userId.ToString()!), Times.Once);
        }

        [Fact]
        public async Task FindAsync_GivenAExistentSavedOnlyInDatabaseUserInterestsWithCacheAndTrackFalse_ShouldReturnTheUserInterestsFromDatabaseAndSaveItInCache()
        {
            // Arrange 
            Guid userId = Guid.NewGuid();
            UserInterests untrackedUser = UserInterestsFixtures.CreateUserInterests(userId: userId);

            _cacheMock
                .Setup(c => c.GetEntity(It.IsAny<string>()))
                .Returns((UserInterests?)null);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { untrackedUser });

            // Act
            UserInterests? result = await _contextDao.FindAsync(userId, track: false);

            // Assert            
            _contextMock.Verify(c => c.Set<UserInterests>(), Times.Once);
            _cacheMock.Verify(c => c.AddEntity(It.IsAny<UserInterests>()), Times.Once);
        }

        [Fact]
        public async Task FindAsync_GivenAnInexistentUserId_ShouldReturnNull()
        {
            // Arrange
            _cacheMock
                .Setup(c => c.GetEntity(It.IsAny<string>()))
                .Returns((UserInterests?)null);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>());

            // Act
            UserInterests? result = await _contextDao.FindAsync("inexistent-id", track: false);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindAsync_GivenAnExistentTrackedUserIdWithCacheNull_ShouldReturnTheUserInterestsFromDatabaseWithoutSaveInCache()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            UserInterests trackedUser = UserInterestsFixtures.CreateUserInterests(userId: userId);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedUser });
            UserInterestsDao contextDao = new(_contextMock.Object, null);

            // Act
            UserInterests? result = await contextDao.FindAsync(userId, track: false);

            // Assert
            Assert.NotNull(result);
            _contextMock.Verify(c => c.Set<UserInterests>(), Times.Once);
        }

        [Fact]
        public async Task FindAsync_GivenAnExistentUserIdWithTrackTrue_ShouldReturnTheUserInterestsFromDatabaseWithoutUseCache()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            UserInterests trackedUser = UserInterestsFixtures.CreateUserInterests(userId: userId);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedUser });

            // Act
            UserInterests? result = await _contextDao.FindAsync(userId, track: true);

            // Assert
            Assert.NotNull(result);
            _contextMock.Verify(c => c.Set<UserInterests>(), Times.Once);
            _cacheMock.Verify(c => c.GetEntity(It.IsAny<string>()), Times.Never);
            _cacheMock.Verify(c => c.AddEntity(It.IsAny<UserInterests>()), Times.Never);
        }
    }
}