using AutoMapper;
using FluentAssertions;
using Moq;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.BLL.Exceptions;
using OnlineLearningPlatform.BLL.Mappings;
using OnlineLearningPlatform.BLL.ServicesUtils;
using OnlineLearningPlatform.Core;
using OnlineLearningPlatform.DAL.DTOs;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.BLL.Tests;

public class UsersServiceTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly Mapper _mapper;
    private readonly UsersService _sut;

    public UsersServiceTests()
    {
        _usersRepositoryMock = new();
        var config = new MapperConfiguration(
        cfg =>
        {
            cfg.AddProfile(new BLLUserMapperProfile());
            cfg.AddProfile(new BLLCourseMapperProfile());
        });
        _mapper = new Mapper(config);
        _sut = new(
            _usersRepositoryMock.Object,
            _mapper,
            new UsersUtils(_usersRepositoryMock.Object)
            );
    }

    [Fact]
    public async Task RegisterAsync_UserNotAlreadyRegisteredValidMapping_RegisterSuccess()
    {
        // Arrange
        var user = new UserRegistrationModel()
        {
            Login = "TestLogin",
            Password = "TestPassword"
        };
        _usersRepositoryMock.Setup(t => t.GetUserByLoginAsync(user.Login)).ReturnsAsync(null as User);

        // Act
        await _sut.RegisterAsync(user);

        //Assert
        _usersRepositoryMock.Verify(t =>
            t.RegisterAsync(It.Is<User>(t => t.Login == user.Login)),
            Times.Once
        );
    }

    [Fact]
    public async Task RegisterAsync_UserAlreadyRegistered_EntityConflictExceptionThrown()
    {
        // Arrange
        var user = new UserRegistrationModel()
        {
            Login = "TestLogin"
        };
        var message = $"User with login {user.Login} already exists.";
        _usersRepositoryMock.Setup(t => t.GetUserByLoginAsync(user.Login)).ReturnsAsync(new User() { Login = user.Login });

        // Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.RegisterAsync(user));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void RegisterAsync_ValidModel_MappingSuccess()
    {
        // Arrange
        var userModel = new UserRegistrationModel()
        {
            FirstName = "John",
            LastName = "Doe",
            Login = "JohnDoeTest",
            Password = "Password",
            Email = "JohnDoeTest@gmail.com",
            Phone = "89121234567"
        };

        //Act 
        var user = _mapper.Map<User>(userModel);

        //Assert
        Assert.Equal(user.Id, Guid.Empty);
        Assert.Equal(userModel.FirstName, user.FirstName);
        Assert.Equal(userModel.LastName, user.LastName);
        Assert.Equal(userModel.Login, user.Login);
        Assert.Equal(userModel.Password, user.Password);
        Assert.Equal(userModel.Email, user.Email);
        Assert.Equal(userModel.Phone, user.Phone);
        Assert.Equal(Core.Role.Student, user.Role);
        Assert.False(user.IsDeactivated);
        Assert.Empty(user.TaughtCourses);
        Assert.Empty(user.Enrollments);
    }

    [Fact]
    public async Task AuthenticateAsync_InvalidLogin_WrongCredentialsExceptionThrown()
    {
        // Arrange
        var login = "InvalidLogin";
        var password = "Password";
        var message = "The credentials are not correct.";

        // Act
        var exception = await Assert.ThrowsAsync<WrongCredentialsException>(async () => await _sut.AuthenticateAsync(login, password));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task AuthenticateAsync_InvalidPassword_WrongCredentialsExceptionThrown()
    {
        // Arrange
        var login = "Login";
        var invalidPassword = "InvalidPassword";
        var validPassword = "ValidPassword";
        var validPasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(validPassword);
        var message = "The credentials are not correct.";
        _usersRepositoryMock.Setup(t => t.GetUserByLoginAsync(login)).ReturnsAsync(new User() { Login = login, Password = validPasswordHash});

        // Act
        var exception = await Assert.ThrowsAsync<WrongCredentialsException>(async () => await _sut.AuthenticateAsync(login, invalidPassword));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task GetAllActiveUsersAsync_GetAllActiveUsersSuccess()
    {
        // Act
        var result = await _sut.GetAllActiveUsersAsync();

        // Assert
        _usersRepositoryMock.Verify(t =>
            t.GetAllActiveUsersAsync(),
            Times.Once
        );
    }

    [Fact]
    public void GetAllActiveUsersAsync_ValidModel_MappingSuccess()
    {
        // Arrange
        var usersDTOs = new List<User>()
        {
            new User { FirstName = "FirstTestName1", LastName = "LastTestName1", Email = "TestEmail1" },
            new User { FirstName = "FirstTestName2", LastName = "LastTestName2", Email = "TestEmail2" },
            new User { FirstName = "FirstTestName3", LastName = "LastTestName3", Email = "TestEmail3" },
        };

        //Act
        var courses = _mapper.Map<List<UserModel>>(usersDTOs);

        //Assert
        courses.Should().BeEquivalentTo(usersDTOs, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GetTaughtCoursesByUserIdAsync_WrongUserRole_EntityConflictExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var message = "The role of the user is not correct.";
        _usersRepositoryMock.Setup(t => t.GetUserByIdWithFullInfoAsync(userId)).ReturnsAsync(new User() { Id = userId, Role = Role.Student });
        // Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.GetTaughtCoursesByUserIdAsync(userId));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void GetTaughtCoursesByUserIdAsync_ValidModel_MappingSuccess()
    {
        // Arrange
        var courseDTOs = new List<Course>()
        {
            new Course { Name = "TestCourse1", Description = "This is the first test course." },
            new Course { Name = "TestCourse2", Description = "This is the second test course." }
        };

        //Act
        var courses = _mapper.Map<List<CourseModel>>(courseDTOs);

        //Assert
        courses.Should().BeEquivalentTo(courseDTOs, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GetEnrollmentsByUserIdAsync_WrongUserRole_EntityConflictExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var message = "The role of the user is not correct.";
        _usersRepositoryMock.Setup(t => t.GetUserByIdWithFullInfoAsync(userId)).ReturnsAsync(new User() { Id = userId, Role = Role.Teacher });
        // Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.GetEnrollmentsByUserIdAsync(userId));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void GetEnrollmentsByUserIdAsync_ValidModel_MappingSuccess()
    {
        // Arrange
        var enrollmentDTOs = new List<Enrollment>()
        {
            new Enrollment 
            { 
                Attendance = 10,
                Grade = 5,
                StudentReview = "TestReview1",
                Course = new Course()
                {
                    Name = "TestCourse1",
                    Description = "This is the first test course description."
                }
            },
            new Enrollment
            {
                Attendance = 12,
                Grade = 6,
                StudentReview = null,
                Course = new Course()
                {
                    Name = "TestCourse2",
                    Description = "This is the second test course description."
                }
            },
        };

        //Act
        var courses = _mapper.Map<List<CourseEnrollmentModel>>(enrollmentDTOs);

        //Assert
        courses.Should().BeEquivalentTo(enrollmentDTOs, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public void GetUserByIdAsync_ValidModel_MappingSuccess()
    {
        //Arrange
        var userDTO = new User()
        {
            Id = Guid.NewGuid(),
            FirstName = "TestFirstName",
            LastName = "TestSecondName",
            Email = "EmailTest",
            Phone = "78901234567",
            Role = Role.Admin,
            IsDeactivated = false
        };

        //Act
        var user = _mapper.Map<ExtendedUserModel>(userDTO);

        //Assert
        user.Should().BeEquivalentTo(userDTO, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task UpdatePasswordAsync_ActiveUserValidPassword_UpdatePasswordSuccess()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var currentPassword = "CurrentPassword";
        var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(currentPassword);
        var newPassword = "NewPassword";
        var user = new User() { Id = userId, Password = hashedPassword };
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(user);
        var passwordModel = new UpdateUserPasswordModel() { CurrentPassword = currentPassword, NewPassword = newPassword };

        //Act
        await _sut.UpdatePasswordAsync(userId, passwordModel);

        //Assert
        _usersRepositoryMock.Verify(t =>
           t.UpdatePasswordAsync(It.Is<User>(t => t.Id == userId), It.IsAny<string>()),
           Times.Once
        );
    }

    [Fact]
    public async Task UpdatePasswordAsync_UserDeactivated_EntityConflictExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var message = $"User with id {userId} is deactivated.";
        var currentPassword = "CurrentPassword";
        var newPassword = "NewPassword";
        var user = new User() { Id = userId, IsDeactivated = true };
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(user);
        var passwordModel = new UpdateUserPasswordModel() { CurrentPassword = currentPassword, NewPassword = newPassword };

        //Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.UpdatePasswordAsync(userId, passwordModel));

        //Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WrongCurrentPassword_WrongCredentialsException()
    {
        //Arrange
        var userId = Guid.NewGuid();
        var message = "The credentials are not correct.";
        var invalidCurrentPassword = "WrongCurrentPassword";
        var newPassword = "NewPassword";
        var validCurrentPassword = "ValidCurrentPassword";
        var validCurrentPasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(validCurrentPassword);
        var user = new User() { Id = userId, Password = validCurrentPasswordHash };
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(user);
        var passwordModel = new UpdateUserPasswordModel() { CurrentPassword = invalidCurrentPassword, NewPassword = newPassword };

        //Act
        var exception = await Assert.ThrowsAsync<WrongCredentialsException>(async () => await _sut.UpdatePasswordAsync(userId, passwordModel));

        //Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task UpdateProfileAsync_ActiveUserValidMapping_UpdateProfileSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User() { Id = userId };
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(user);
        var userProfileModel = new UpdateUserProfileModel() { FirstName = "NewFirstName" };
        var courseUpdateDTO = new User() { FirstName = userProfileModel.FirstName };

        // Act
        await _sut.UpdateProfileAsync(userId, userProfileModel);

        // Assert
        _usersRepositoryMock.Verify(t =>
            t.UpdateProfileAsync(It.Is<User>(t => t.Id == userId),
            It.Is<User>(t => t.FirstName == courseUpdateDTO.FirstName)),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateProfileAsync_UserDeactivated_EntityConflictExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var message = $"User with id {userId} is deactivated.";
        var user = new User() { Id = userId, IsDeactivated = true };
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(user);
        var userProfileModel = new UpdateUserProfileModel() { FirstName = "NewFirstName" };

        //Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.UpdateProfileAsync(userId, userProfileModel));

        //Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void UpdateProfileAsync_ValidModel_MappingSuccess()
    {
        // Arrange
        var userProfileModel = new UpdateUserProfileModel()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "JohnDoeTest@gmail.com",
            Phone = "89121234567"
        };

        //Act 
        var user = _mapper.Map<User>(userProfileModel);

        //Assert
        Assert.Equal(user.Id, Guid.Empty);
        Assert.Equal(userProfileModel.FirstName, user.FirstName);
        Assert.Equal(userProfileModel.LastName, user.LastName);
        Assert.Equal(userProfileModel.Email, user.Email);
        Assert.Equal(userProfileModel.Phone, user.Phone);
        Assert.Equal(Role.Student, user.Role);
        Assert.False(user.IsDeactivated);
        Assert.Empty(user.TaughtCourses);
        Assert.Empty(user.Enrollments);
    }

    [Fact]
    public async Task UpdateRoleAsync_ActiveUserValidRoleNoCourses_UpdateRoleSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newRole = Role.Admin;
        var user = new User() { Id = userId, Role = Role.Student };
        _usersRepositoryMock.Setup(t => t.GetUserByIdWithFullInfoAsync(userId)).ReturnsAsync(user);

        // Act
        await _sut.UpdateRoleAsync(userId, newRole);

        // Assert
        _usersRepositoryMock.Verify(t =>
            t.UpdateRoleAsync(It.Is<User>(t => t.Id == userId), newRole),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateRoleAsync_UserDeactivated_EntityConflictExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newRole = Role.Admin;
        var message = $"User with id {userId} is deactivated.";
        var user = new User() { Id = userId, IsDeactivated = true };
        _usersRepositoryMock.Setup(t => t.GetUserByIdWithFullInfoAsync(userId)).ReturnsAsync(user);
       
        //Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.UpdateRoleAsync(userId, newRole));

        //Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task UpdateRoleAsync_WrongRole_EntityConflictExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newRole = Role.Admin;
        var message = $"User with id {userId} does not satisfy the requirements.";
        var user = new User() { Id = userId, Role = Role.Teacher};
        _usersRepositoryMock.Setup(t => t.GetUserByIdWithFullInfoAsync(userId)).ReturnsAsync(user);

        //Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.UpdateRoleAsync(userId, newRole));

        //Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task UpdateRoleAsync_HasEnrollments_EntityConflictExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newRole = Role.Admin;
        var message = $"User with id {userId} does not satisfy the requirements.";
        var user = new User() 
        { 
            Id = userId, 
            Role = Role.Student,
            Enrollments = new List<Enrollment>()
            {
                new Enrollment()
            }
        };
        _usersRepositoryMock.Setup(t => t.GetUserByIdWithFullInfoAsync(userId)).ReturnsAsync(user);

        //Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.UpdateRoleAsync(userId, newRole));

        //Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task DeactivateUserAsync_DeactivateUserSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User() { Id = userId };
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(user);

        // Act
        await _sut.DeactivateUserAsync(userId);

        // Assert
        _usersRepositoryMock.Verify(t =>
            t.DeactivateUserAsync(It.Is<User>(t => t.Id == userId)),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteUserAsync_DeleteUserSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User() { Id = userId };
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(user);

        // Act
        await _sut.DeleteUserAsync(userId);

        // Assert
        _usersRepositoryMock.Verify(t =>
            t.DeleteUserAsync(It.Is<User>(t => t.Id == userId)),
            Times.Once
        );
    }
}
