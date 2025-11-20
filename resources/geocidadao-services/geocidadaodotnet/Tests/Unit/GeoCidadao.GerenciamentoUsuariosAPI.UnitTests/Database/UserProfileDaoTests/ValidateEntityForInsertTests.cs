using System.Reflection;
using FluentAssertions;
using GeoCidadao.Database;
using GeoCidadao.Database.EFDao;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.CacheContracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.EFDao;
using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;
using GeoCidadao.Models.Exceptions;
using GeoCidadao.TestShared.Fixtures;
using GeoCidadao.TestShared.Helpers;
using Moq;

namespace GeoCidadao.GerenciamentoUsuariosAPI.UnitTests.Database.UserProfileDaoTests
{
    public class ValidateEntityForInsertTests
    {
        private readonly UserProfileDao _contextDao;
        private readonly Mock<GeoDbContext> _contextMock;
        private readonly Mock<IUserProfileDaoCache> _cacheMock;

        public ValidateEntityForInsertTests()
        {
            _cacheMock = new Mock<IUserProfileDaoCache>();
            _contextMock = GeoCidadaoDatabaseMockTest.CreateMockContext();
            _contextDao = new UserProfileDao(_contextMock.Object, _cacheMock.Object);
        }

        [Fact]
        public async Task ValidateEntityForInsert_GivenAnUserProfileWithEmptyUsername_ShouldThrowEntityValidationException()
        {
            // Arrange
            UserProfile user = new UserProfile
            {
                Id = Guid.NewGuid(),
                Username = string.Empty,
                Email = string.Empty,
                FirstName = "John",
                LastName = "Doe"
            };
            // Act
            Func<Task> act = async () => await InvokeValidateForInsert(user);

            // Assert
            var exception = await act.Should().ThrowAsync<TargetInvocationException>();

            exception.And.InnerException.Should().BeOfType<EntityValidationException>();
        }

        [Fact]
        public async Task ValidateEntityForInsert_GivenAnUserProfileWithNullUsername_ShouldThrowEntityValidationException()
        {
            // Arrange
            UserProfile user = new UserProfile
            {
                Id = Guid.NewGuid(),
                Username = null!,
                Email = string.Empty,
                FirstName = "John",
                LastName = "Doe"
            };
            // Act
            Func<Task> act = async () => await InvokeValidateForInsert(user);

            // Assert
            var exception = await act.Should().ThrowAsync<TargetInvocationException>();

            exception.And.InnerException.Should().BeOfType<EntityValidationException>();
        }

        [Fact]
        public async Task ValidateEntityForInsert_GivenAnUserProfileWithEmptyEmail_ShouldThrowEntityValidationException()
        {
            // Arrange
            UserProfile user = new UserProfile
            {
                Id = Guid.NewGuid(),
                Username = TestFixtures.GetRandomName(),
                Email = string.Empty,
                FirstName = "John",
                LastName = "Doe"
            };
            // Act
            Func<Task> act = async () => await InvokeValidateForInsert(user);

            // Assert
            var exception = await act.Should().ThrowAsync<TargetInvocationException>();

            exception.And.InnerException.Should().BeOfType<EntityValidationException>();
        }

        [Fact]
        public async Task ValidateEntityForInsert_GivenAnUserProfileWithNullEmail_ShouldThrowEntityValidationException()
        {
            // Arrange
            UserProfile user = new UserProfile
            {
                Id = Guid.NewGuid(),
                Username = TestFixtures.GetRandomName(),
                Email = null!,
                FirstName = "John",
                LastName = "Doe"
            };
            // Act
            Func<Task> act = async () => await InvokeValidateForInsert(user);

            // Assert
            var exception = await act.Should().ThrowAsync<TargetInvocationException>();

            exception.And.InnerException.Should().BeOfType<EntityValidationException>();
        }

        [Fact]
        public async Task ValidateEntityForInsert_GivenAnUserProfileWithEmptyFirstName_ShouldThrowEntityValidationException()
        {
            // Arrange
            UserProfile user = new UserProfile
            {
                Id = Guid.NewGuid(),
                Username = TestFixtures.GetRandomName(),
                Email = TestFixtures.GetRandomEmail(),
                FirstName = string.Empty,
                LastName = "Doe"
            };
            // Act
            Func<Task> act = async () => await InvokeValidateForInsert(user);

            // Assert
            var exception = await act.Should().ThrowAsync<TargetInvocationException>();

            exception.And.InnerException.Should().BeOfType<EntityValidationException>();
        }

        [Fact]
        public async Task ValidateEntityForInsert_GivenAnUserProfileWithNullFirstName_ShouldThrowEntityValidationException()
        {
            // Arrange
            UserProfile user = new UserProfile
            {
                Id = Guid.NewGuid(),
                Username = TestFixtures.GetRandomName(),
                Email = TestFixtures.GetRandomEmail(),
                FirstName = null!,
                LastName = "Doe"
            };
            // Act
            Func<Task> act = async () => await InvokeValidateForInsert(user);

            // Assert
            var exception = await act.Should().ThrowAsync<TargetInvocationException>();

            exception.And.InnerException.Should().BeOfType<EntityValidationException>();
        }

        [Fact]
        public async Task ValidateEntityForInsert_GivenAnUserProfileWithEmptyLastName_ShouldThrowEntityValidationException()
        {
            // Arrange
            UserProfile user = new UserProfile
            {
                Id = Guid.NewGuid(),
                Username = TestFixtures.GetRandomName(),
                Email = TestFixtures.GetRandomEmail(),
                FirstName = "John",
                LastName = string.Empty
            };
            // Act
            Func<Task> act = async () => await InvokeValidateForInsert(user);

            // Assert
            var exception = await act.Should().ThrowAsync<TargetInvocationException>();

            exception.And.InnerException.Should().BeOfType<EntityValidationException>();
        }

        [Fact]
        public async Task ValidateEntityForInsert_GivenAnUserProfileWithNullLastName_ShouldThrowEntityValidationException()
        {
            // Arrange
            UserProfile user = new UserProfile
            {
                Id = Guid.NewGuid(),
                Username = TestFixtures.GetRandomName(),
                Email = TestFixtures.GetRandomEmail(),
                FirstName = "John",
                LastName = null!
            };
            // Act
            Func<Task> act = async () => await InvokeValidateForInsert(user);

            // Assert
            var exception = await act.Should().ThrowAsync<TargetInvocationException>();

            exception.And.InnerException.Should().BeOfType<EntityValidationException>();
        }


        private Task InvokeValidateForInsert(UserProfile entity)
        {
            // Use reflection to call the private method
            MethodInfo? methodInfo = typeof(BaseDao<UserProfile>).GetMethod("ValidateEntityForInsert",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (methodInfo == null)
                throw new Exception("ValidateEntityForInsert method not found");


            return (Task)methodInfo.Invoke(
                    _contextDao, new object[] { new UserProfile[] { entity } })!;
        }
    }
}