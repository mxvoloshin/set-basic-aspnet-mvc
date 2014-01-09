using System;
using System.Linq.Expressions;
using System.Threading;

using Moq;
using NUnit.Framework;

using set_basic_aspnet_mvc.Domain.Entities;
using set_basic_aspnet_mvc.Domain.Repositories;
using set_basic_aspnet_mvc.Domain.Services;

namespace set_basic_aspnet_mvc.test.Services
{
    [TestFixture]
    public class UserServiceTests
    {
        [Test]
        public void create_should_return_user_id()
        {
            // Arrange
            var user = new User { FullName = "test", Email = "test@test.com", RoleId = SetRole.User.Value, Language = Thread.CurrentThread.CurrentUICulture.Name };
            var userRepository = new Mock<IRepository<User>>();

            userRepository.Setup(x => x.Create(user)).Returns(user);
            userRepository.Setup(x => x.SaveChanges()).Returns(true);

            // Act
            var userService = new UserService(userRepository.Object);
            var userId = userService.Create(user.FullName, user.Email, "password", user.RoleId, user.Language);

            // Assert
            userRepository.Verify(x => x.Create(It.IsAny<User>()), Times.Once);
            userRepository.Verify(x => x.SaveChanges(), Times.AtLeastOnce);

            Assert.NotNull(userId);
        }

        [Test]
        public void authenticate_should_return_user_result()
        {
            // Arrange
            var user = new User { FullName = "test", Email = "test@test.com", RoleId = SetRole.User.Value, Language = Thread.CurrentThread.CurrentUICulture.Name };
            var userRepository = new Mock<IRepository<User>>();

            // Act
            var userService = new UserService(userRepository.Object);
            var userId = userService.Create(user.FullName, user.Email, "password", user.RoleId, user.Language);

            // Assert
            userRepository.Verify(x => x.Create(It.IsAny<User>()), Times.Once);
            userRepository.Verify(x => x.SaveChanges(), Times.AtLeastOnce);

            Assert.NotNull(userId);
        }

        [Test]
        public async void get_user_by_email_should_return_user()
        {
            // Arrange
            const string email = "test@test.com";

            var userRepository = new Mock<IRepository<User>>();
            userRepository.Setup(x => x.FindOne(It.IsAny<Expression<Func<User, bool>>>())).Returns(new User { Email = email });

            // Act
            var userService = new UserService(userRepository.Object);
            var user = await userService.GetByEmail(email);

            // Assert
            Assert.NotNull(user);
            Assert.AreEqual(user.Email, email);
            userRepository.Verify(x => x.FindOne(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
        }

        [Test]
        public void change_status_test()
        {
            // Arrange
            const long userId = 8;
            const long updatedBy = 1;
            const bool isActive = true;
            var userRepository = new Mock<IRepository<User>>();
            userRepository.Setup(x => x.FindOne(It.IsAny<Expression<Func<User, bool>>>())).Returns(new User { Id = userId });

            // Act
            var userService = new UserService(userRepository.Object);
            var user = userService.ChangeStatus(userId, updatedBy, isActive);

            // Assert
            Assert.NotNull(user);
            userRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
            userRepository.Verify(x => x.SaveChanges(), Times.AtLeastOnce);
        }

        [Test]
        public async void get_should_return_user()
        {
            // Arrange
            const long id = 2;

            var userRepository = new Mock<IRepository<User>>();
            userRepository.Setup(x => x.FindOne(It.IsAny<Expression<Func<User, bool>>>())).Returns(new User { Id = id });

            // Act
            var userService = new UserService(userRepository.Object);
            var user = await userService.Get(id);

            // Assert
            Assert.NotNull(user);
            userRepository.Verify(x => x.FindOne(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
        }

        [Test]
        public void change_password_test()
        {
            // Arrange
            const string email = "test@test.com";
            const string token = "test";
            const string password = "password";

            var userRepository = new Mock<IRepository<User>>();
            userRepository.Setup(x => x.FindOne(It.IsAny<Expression<Func<User, bool>>>())).Returns(new User { Email = email, PasswordResetToken = token, PasswordHash = BCrypt.Net.BCrypt.HashPassword(password), PasswordResetRequestedAt = DateTime.Now });

            // Act
            var userService = new UserService(userRepository.Object);
            var user = userService.ChangePassword(email, token, password);

            // Assert
            Assert.NotNull(user);
            userRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
            userRepository.Verify(x => x.SaveChanges(), Times.AtLeastOnce);
        }

        [Test]
        public void request_password_reset_test()
        {
            // Arrange
            const string email = "test@test.com";
            const string token = "test";
            const long userId = 1;            

            var userRepository = new Mock<IRepository<User>>();
            userRepository.Setup(x => x.FindOne(It.IsAny<Expression<Func<User, bool>>>()))
                          .Returns(new User { Email = email, PasswordResetToken = token, PasswordResetRequestedAt = null, UpdatedAt = DateTime.Now, UpdatedBy = userId });

            // Act
            var userService = new UserService(userRepository.Object);
            var user = userService.RequestPasswordReset(email);

            // Assert
            Assert.NotNull(user);
            userRepository.Verify(x => x.Update(It.IsAny<User>()), Times.Once);
            userRepository.Verify(x => x.SaveChanges(), Times.AtLeastOnce);
        }

        [Test]
        public void is_password_reset_request_valid_test()
        {
            // Arrange
            const string email = "test@test.com";
            const string token = "test";

            var userRepository = new Mock<IRepository<User>>();
            userRepository.Setup(x => x.FindOne(It.IsAny<Expression<Func<User, bool>>>())).Returns(new User { Email = email, PasswordResetToken = token});

            // Act
            var userService = new UserService(userRepository.Object);
            var user = userService.IsPasswordResetRequestValid(email,token);

            // Assert
            Assert.NotNull(user);
        }

        [Test]
        public async void is_email_exists_test()
        {
            // Arrange
            const string email ="test@test.com";

            var userRepository = new Mock<IRepository<User>>();
            userRepository.Setup(x=>x.FindOne(It.IsAny<Expression<Func<User, bool>>>())).Returns(new User { Email = email});          
            
            // Act
            var userService = new UserService(userRepository.Object);
            var user = await userService.IsUserWithEmailExists(email);

            // Assert
            Assert.NotNull(user);
        }
    }
}
