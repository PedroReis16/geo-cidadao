using FluentAssertions;
using GeoCidadao.Database;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.CacheContracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.EFDao;
using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;
using GeoCidadao.Models.Enums;
using GeoCidadao.Models.Exceptions;
using GeoCidadao.TestShared.Fixtures;
using GeoCidadao.TestShared.Helpers;
using Moq;
using Moq.EntityFrameworkCore;

namespace GeoCidadao.GerenciamentoUsuariosAPI.UnitTests.Database.UserInterestsDaoTests
{
    public class UpdateFollowedCategoriesAsyncTests
    {
        private readonly UserInterestsDao _contextDao;
        private readonly Mock<GeoDbContext> _contextMock;
        private readonly Mock<IUserInterestsDaoCache> _cacheMock;

        public UpdateFollowedCategoriesAsyncTests()
        {
            _cacheMock = new Mock<IUserInterestsDaoCache>();
            _contextMock = GeoCidadaoDatabaseMockTest.CreateMockContext();
            _contextDao = new UserInterestsDao(_contextMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task UpdateFollowedCategoriesAsync_GivenAnInexistentUserId_ShouldThrowEntityValidationException()
        {
            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>());

            Func<Task> act = async () => await _contextDao.UpdateFollowedCategoriesAsync(Guid.NewGuid(), new());

            await act.Should().ThrowAsync<EntityValidationException>();
        }

        [Fact]
        public async Task UpdateFollowedCategoriesAsync_GivenAnCategoryListWithExistentItem_ShouldRemoveTheCategoryFromFollowedCategories()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);
            PostCategory categoriesToRemove = TestFixtures.GetRandomEnumValue<PostCategory>();

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedCategoriesAsync(userId, new List<PostCategory>() { categoriesToRemove });

            // Assert
            trackedInterests.FollowedCategories.Should().NotContain(categoriesToRemove);
        }

        [Fact]
        public async Task UpdateFollowedCategoriesAsync_GivenAnCategoryListWithNonExistentItem_ShouldAddTheCategoryToFollowedCategories()
        {
            // Arrange 
            Guid userId = Guid.NewGuid();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);
            PostCategory categoriesToAdd = TestFixtures.GetRandomEnumValue<PostCategory>();
            trackedInterests.FollowedCategories.Remove(categoriesToAdd);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedCategoriesAsync(userId, new List<PostCategory>() { categoriesToAdd });

            // Assert
            trackedInterests.FollowedCategories.Should().Contain(categoriesToAdd);
        }

        [Fact]
        public async Task UpdateFollowedCategoriesAsync_GivenAnCategoryListThatRemoveAllFollowedCategories_ShouldThrowEntityValidationException()
        {
            // Arrange
            Guid userId = Guid.NewGuid();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);
            List<PostCategory> categoriesToRemove = trackedInterests.FollowedCategories.ToList();

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            Func<Task> act = async () => await _contextDao.UpdateFollowedCategoriesAsync(userId, categoriesToRemove);

            // Assert
            await act.Should().ThrowAsync<EntityValidationException>();
        }

        [Fact]
        public async Task UpdateFollowedCategoriesAsync_GivenAnValidCategoryUpdateCondition_ShouldUpdateTheUpdatedAtProperty()
        {
            // Arrange 
            Guid userId = Guid.NewGuid();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);
            
            PostCategory categoriesToAdd = TestFixtures.GetRandomEnumValue<PostCategory>();
            trackedInterests.FollowedCategories.Remove(categoriesToAdd);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedCategoriesAsync(userId, new List<PostCategory>() { categoriesToAdd });

            // Assert
            trackedInterests.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateFollowedCategoriesAsync_GivenAnValidCategoryUpdateCondition_ShouldRemoveTheEntityFromCache()
        {
            // Arrange 
            Guid userId = Guid.NewGuid();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);

            PostCategory categoriesToAdd = TestFixtures.GetRandomEnumValue<PostCategory>();
            trackedInterests.FollowedCategories.Remove(categoriesToAdd);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedCategoriesAsync(userId, new List<PostCategory>() { categoriesToAdd });

            // Assert
            _cacheMock.Verify(c => c.RemoveEntity(trackedInterests), Times.Once);
        }

        [Fact]
        public async Task UpdateFollowedCategoriesAsync_GivenAnValidCategoryUpdateCondition_ShouldSaveChangesInDatabase()
        {
            // Arrange 
            Guid userId = Guid.NewGuid();
            UserInterests trackedInterests = UserInterestsFixtures.CreateUserInterests(userId: userId);

            PostCategory categoriesToAdd = TestFixtures.GetRandomEnumValue<PostCategory>();
            trackedInterests.FollowedCategories.Remove(categoriesToAdd);

            _contextMock.Setup(c => c.Set<UserInterests>())
                .ReturnsDbSet(new List<UserInterests>() { trackedInterests });

            // Act
            await _contextDao.UpdateFollowedCategoriesAsync(userId, new List<PostCategory>() { categoriesToAdd });

            // Assert
            _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once);
        }
    }
}