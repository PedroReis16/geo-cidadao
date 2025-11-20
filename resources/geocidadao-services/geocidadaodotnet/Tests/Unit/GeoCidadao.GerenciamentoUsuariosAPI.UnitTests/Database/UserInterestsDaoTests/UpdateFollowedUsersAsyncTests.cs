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
    public class UpdateFollowedUsersAsyncTests
    {
        private readonly UserInterestsDao _contextDao;
        private readonly Mock<GeoDbContext> _contextMock;
        private readonly Mock<IUserInterestsDaoCache> _cacheMock;

        public UpdateFollowedUsersAsyncTests()
        {
            _cacheMock = new Mock<IUserInterestsDaoCache>();
            _contextMock = GeoCidadaoDatabaseMockTest.CreateMockContext();
            _contextDao = new UserInterestsDao(_contextMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task UpdateFollowedUsersAsync_GivenAnInexistentUserId_ShouldThrowEntityValidationException()
        {
            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>());

            Func<Task> act = async () => await _contextDao.UpdateFollowedUsersAsync(Guid.NewGuid(), Guid.NewGuid());

            await act.Should().ThrowAsync<EntityValidationException>();
        }

        [Fact]
        public async Task UpdateFollowedUserAsync_GivenAUserAlreadyFollowed_ShouldRemoveTheUserFromFollowedUsers()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            Guid userToRemove = Guid.NewGuid();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);
            trackedInterests.FollowedUsers.Add(userToRemove);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedUsersAsync(userId, userToRemove);

            // Assert
            trackedInterests.FollowedUsers.Should().NotContain(userToRemove);
        }

        [Fact]
        public async Task UpdateFollowedUsersAsync_GivenAUserNotFollowed_ShouldAddTheUserToFollowedUsers()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            Guid userToAdd = Guid.NewGuid();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedUsersAsync(userId, userToAdd);

            // Assert
            trackedInterests.FollowedUsers.Should().Contain(userToAdd);
        }

        [Fact]
        public async Task UpdateFollowedUsersAsync_GivenTheSameUserIdAndUserToFollowed_ShouldDoNothing()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedUsersAsync(userId, userId);

            // Assert
            _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Never);
            _cacheMock.Verify(c => c.RemoveEntity(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UpdateFollowedUsersAsync_GivenAnEmptyUserToFollowed_ShouldDoNothing()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedUsersAsync(userId, Guid.Empty);

            // Assert
            _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Never);
            _cacheMock.Verify(c => c.RemoveEntity(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UpdateFollowedUsersAsync_GivenAValidUserSaveCondition_ShouldReturnUpdatedAtPropertyNotNull()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            Guid userToAdd = Guid.NewGuid();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedUsersAsync(userId, userToAdd);

            // Assert
            trackedInterests.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateFollowedUsersAsync_GivenAValidUserSaveCondition_ShouldRemoveTheEntityFromCache()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            Guid userToAdd = Guid.NewGuid();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedUsersAsync(userId, userToAdd);

            // Assert
            _cacheMock.Verify(c => c.RemoveEntity(trackedInterests), Times.Once);
        }

        [Fact]
        public async Task UpdateFollowedUsersAsync_GivenAValidUserSaveConditionWithCacheNull_ShouldSaveChangesAndNotThrowException()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            Guid userToAdd = Guid.NewGuid();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);

            var contextDao = new UserInterestsDao(_contextMock.Object, null);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            Func<Task> act = async () => await contextDao.UpdateFollowedUsersAsync(userId, userToAdd);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async Task UpdatedFollowedUsersAsync_GivenAValidUserSaveCondition_ShouldSaveChangesToDatabase()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            Guid userToAdd = Guid.NewGuid();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedUsersAsync(userId, userToAdd);

            // Assert
            _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }
    }
}