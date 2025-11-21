using System.Reflection;
using FluentAssertions;
using GeoCidadao.Database;
using GeoCidadao.Database.EFDao;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.CacheContracts;
using GeoCidadao.GerenciamentoUsuariosAPI.Database.EFDao;
using GeoCidadao.Models.Entities.GerenciamentoUsuariosAPI;
using GeoCidadao.Models.Exceptions;
using GeoCidadao.TestShared.Helpers;
using Moq;

namespace GeoCidadao.GerenciamentoUsuariosAPI.UnitTests.Database.UserPictureDaoTests
{
    public class ValidateEntityForInsertTests
    {
        private readonly UserPictureDao _contextDao;
        private readonly Mock<GeoDbContext> _contextMock;
        private readonly Mock<IUserPictureDaoCache> _cacheMock;

        public ValidateEntityForInsertTests()
        {
            _cacheMock = new Mock<IUserPictureDaoCache>();
            _contextMock = GeoCidadaoDatabaseMockTest.CreateMockContext();
            _contextDao = new UserPictureDao(_contextMock.Object, _cacheMock.Object);
        }


        [Fact]
        public async Task ValidateEntityForInsert_GivenAnUserPictureWithoutFileHash_ShouldThrowEntityValidationException()
        {
            // Arrange
            UserPicture picture = new UserPicture
            {
                Id = Guid.NewGuid(),
                FileExtension = ".png",
                FileHash = null!
            };

            // Act
            Func<Task> act = async () => await InvokeValidateForInsert(picture);

            // Assert
            var exception = await act.Should().ThrowAsync<TargetInvocationException>();

            exception.And.InnerException.Should().BeOfType<EntityValidationException>();
        }

        [Fact]
        public async Task ValidateEntityForInsert_GivenAnUserPictureWithStringEmptyFileHash_ShouldThrowEntityValidationException()
        {
            // Arrange
            UserPicture picture = new UserPicture
            {
                Id = Guid.NewGuid(),
                FileExtension = ".png",
                FileHash = string.Empty
            };

            // Act
            Func<Task> act = async () => await InvokeValidateForInsert(picture);

            // Assert
            var exception = await act.Should().ThrowAsync<TargetInvocationException>();

            exception.And.InnerException.Should().BeOfType<EntityValidationException>();
        }

        [Fact]
        public async Task ValidateEntityForInsert_GivenAnUserPictureWithoutFileExtension_ShouldThrowEntityValidationException()
        {
            // Arrange
            UserPicture picture = new UserPicture
            {
                Id = Guid.NewGuid(),
                FileExtension = null!,
                FileHash = "somehashvalue"
            };

            // Act
            Func<Task> act = async () => await InvokeValidateForInsert(picture);

            
            // Assert
            var exception = await act.Should().ThrowAsync<TargetInvocationException>();

            exception.And.InnerException.Should().BeOfType<EntityValidationException>();
        }

        [Fact]
        public async Task ValidateEntityForInsert_GivenAnUserPictureWithStringEmptyFileExtension_ShouldThrowEntityValidationException()
        {
            // Arrange
            UserPicture picture = new UserPicture
            {
                Id = Guid.NewGuid(),
                FileExtension = string.Empty,
                FileHash = "somehashvalue"
            };

            // Act
            Func<Task> act = async () => await InvokeValidateForInsert(picture);

            
            // Assert
            var exception = await act.Should().ThrowAsync<TargetInvocationException>();

            exception.And.InnerException.Should().BeOfType<EntityValidationException>();
        }

        private Task InvokeValidateForInsert(UserPicture entity)
        {
            // Use reflection to call the private method
            MethodInfo? methodInfo = typeof(BaseDao<UserPicture>).GetMethod("ValidateEntityForInsert",
                BindingFlags.NonPublic | BindingFlags.Instance);

            if (methodInfo == null)
                throw new Exception("ValidateEntityForInsert method not found");


            return (Task)methodInfo.Invoke(
                    _contextDao, new object[] { new UserPicture[] { entity } })!;
        }
    }
}