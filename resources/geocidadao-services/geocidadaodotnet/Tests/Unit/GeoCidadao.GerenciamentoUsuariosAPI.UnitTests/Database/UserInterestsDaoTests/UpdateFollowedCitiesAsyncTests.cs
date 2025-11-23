using FluentAssertions;
using GeoCidadao.Database;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.CacheContracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.EFDao;
using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;
using GeoCidadao.Models.Exceptions;
using GeoCidadao.TestShared.Fixtures;
using GeoCidadao.TestShared.Helpers;
using Moq;
using Moq.EntityFrameworkCore;

namespace GeoCidadao.GerenciamentoUsuariosAPI.UnitTests.Database.UserInterestsDaoTests
{
    public class UpdateFollowedCitiesAsyncTests
    {
        private readonly UserInterestsDao _contextDao;
        private readonly Mock<GeoDbContext> _contextMock;
        private readonly Mock<IUserInterestsDaoCache> _cacheMock;

        public UpdateFollowedCitiesAsyncTests()
        {
            _cacheMock = new Mock<IUserInterestsDaoCache>();
            _contextMock = GeoCidadaoDatabaseMockTest.CreateMockContext();
            _contextDao = new UserInterestsDao(_contextMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task UpdateFollowedCitiesAsync_GivenAnInexistentUserId_ShouldThrowEntityValidationException()
        {
            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>());

            Func<Task> act = async () => await _contextDao.UpdateFollowedCitiesAsync(Guid.NewGuid(), "SampleCity");

            await act.Should().ThrowAsync<EntityValidationException>();
        }

        [Fact]
        public async Task UpdateFollowedCitiesAsync_GivenACityAlreadyFollowed_ShouldRemoveTheCityFromFollowedCities()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string cityToRemove = TestFixtures.GetRandomCity().ToLower();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);
            trackedInterests.FollowedCities.Add(cityToRemove);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedCitiesAsync(userId, cityToRemove);

            // Assert
            trackedInterests.FollowedCities.Should().NotContain(cityToRemove);
        }

        [Fact]
        public async Task UpdateFollowedCitiesAsync_GivenACityNotFollowed_ShouldAddTheCityToFollowedCities()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string cityToAdd = TestFixtures.GetRandomCity();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedCitiesAsync(userId, cityToAdd);

            // Assert
            trackedInterests.FollowedCities.Should().Contain(cityToAdd.ToLower());
        }

        [Fact]
        public async Task UpdateFollowedCitiesAsync_GivenACapsLockCityName_ShouldAddTheCityInLowercaseToFollowedCities()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string cityToAdd = TestFixtures.GetRandomCity().ToUpper();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedCitiesAsync(userId, cityToAdd);

            // Assert
            trackedInterests.FollowedCities.Should().Contain(cityToAdd.ToLower());
        }

        [Fact]
        public async Task UpdateFollowedCitiesAsync_GivenAnEmptyCityName_ShouldNotModifyFollowedCities()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedCitiesAsync(userId, string.Empty);

            // Assert
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
            trackedInterests.FollowedCities.Should().BeEmpty();
        }

        [Fact]
        public async Task UpdateFollowedCitiesAsync_GivenAnValidCityUpdate_ShouldUpdateTheUpdatedAtTimestamp()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string cityToAdd = TestFixtures.GetRandomCity();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedCitiesAsync(userId, cityToAdd);

            // Assert
            trackedInterests.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateFollowedCitiesAsync_GivenAnValidCityUpdate_ShouldRemoveTheEntityFromCache()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string cityToAdd = TestFixtures.GetRandomCity();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedCitiesAsync(userId, cityToAdd);

            // Assert
            _cacheMock.Verify(c => c.RemoveEntity(It.Is<UserInterests>(ui => ui.User.Id == userId)), Times.Once);
        }

        [Fact]
        public async Task UpdatedFollowedCitiesAsync_GivenAnValidCityUpdateWithCacheNull_ShouldNotThrowException()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string cityToAdd = TestFixtures.GetRandomCity();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);

            var contextDao = new UserInterestsDao(_contextMock.Object, null!);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            Func<Task> act = async () => await contextDao.UpdateFollowedCitiesAsync(userId, cityToAdd);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task UpdateFollowedCitiesAsync_GivenAnValidCityUpdate_ShouldSaveChangesToDatabase()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string cityToAdd = TestFixtures.GetRandomCity();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedCitiesAsync(userId, cityToAdd);

            // Assert
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}