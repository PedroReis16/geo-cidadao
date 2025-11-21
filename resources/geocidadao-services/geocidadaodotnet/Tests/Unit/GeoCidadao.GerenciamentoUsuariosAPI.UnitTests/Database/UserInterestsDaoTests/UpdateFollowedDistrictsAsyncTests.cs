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
    public class UpdateFollowedDistrictsAsyncTests
    {
        private readonly UserInterestsDao _contextDao;
        private readonly Mock<GeoDbContext> _contextMock;
        private readonly Mock<IUserInterestsDaoCache> _cacheMock;

        public UpdateFollowedDistrictsAsyncTests()
        {
            _cacheMock = new Mock<IUserInterestsDaoCache>();
            _contextMock = GeoCidadaoDatabaseMockTest.CreateMockContext();
            _contextDao = new UserInterestsDao(_contextMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task UpdateFollowedDistrictsAsync_GivenAnInexistentUserId_ShouldThrowEntityValidationException()
        {
            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>());

            Func<Task> act = async () => await _contextDao.UpdateFollowedDistrictsAsync(Guid.NewGuid(), "SampleDistrict");

            await act.Should().ThrowAsync<EntityValidationException>();
        }

        [Fact]
        public async Task UpdateFollowedDistrictsAsync_GivenAEmptyDistrictName_ShouldDoNothing()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedDistrictsAsync(userId, string.Empty);

            // Assert
            _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Never);
            _cacheMock.Verify(c => c.RemoveEntity(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UpdateFollowedDistrictsAsync_GivenANullDistrictName_ShouldDoNothing()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedDistrictsAsync(userId, null!);

            // Assert
            _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Never);
            _cacheMock.Verify(c => c.RemoveEntity(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UpdateFollowedDistrictsAsync_GivenADistrictAlreadyFollowed_ShouldRemoveTheDistrictFromFollowedDistricts()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string districtToRemove = TestFixtures.GetRandomDistrict().ToLower();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);
            trackedInterests.FollowedDistricts.Add(districtToRemove);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedDistrictsAsync(userId, districtToRemove);

            // Assert
            trackedInterests.FollowedDistricts.Should().NotContain(districtToRemove);
        }

        [Fact]
        public async Task UpdateFollowedDistrictsAsync_GivenADistrictNotFollowed_ShouldAddTheDistrictToFollowedDistricts()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string districtToAdd = TestFixtures.GetRandomDistrict().ToLower();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedDistrictsAsync(userId, districtToAdd);

            // Assert
            trackedInterests.FollowedDistricts.Should().Contain(districtToAdd);
        }

        [Fact]
        public async Task UpdateFollowedDistrictsAsync_GivenACapsLockDistrictName_ShouldAddTheDistrictInOriginalCaseToFollowedDistricts()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string districtToAdd = TestFixtures.GetRandomDistrict().ToUpper();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedDistrictsAsync(userId, districtToAdd);

            // Assert
            trackedInterests.FollowedDistricts.Should().Contain(districtToAdd.ToLower());
        }

        [Fact]
        public async Task UpdateFollowedDistrictsAsync_GivenAValidDistrictUpdate_ShouldReturnUpdatedAtPropertyNotNull()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string districtToAdd = TestFixtures.GetRandomDistrict();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedDistrictsAsync(userId, districtToAdd);

            // Assert
            trackedInterests.UpdatedAt.Should().NotBeNull();
        }
        
        [Fact]
        public async Task UpdateFollowedDistrictsAsync_GivenAValidDistrictUpdate_ShouldCallSaveChangesOnce()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            string districtToAdd = TestFixtures.GetRandomDistrict();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedDistrictsAsync(userId, districtToAdd);

            // Assert
            _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }
    }
}