using AutoMapper;
using FluentAssertions;
using Moq;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.BLL.Exceptions;
using OnlineLearningPlatform.BLL.Mappings;
using OnlineLearningPlatform.BLL.ServicesUtils;
using OnlineLearningPlatform.BLL.Tests.TestCases;
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

    [Theory]
    [MemberData(nameof(UsersServiceTestCases.UserToRegister), MemberType = typeof(UsersServiceTestCases))]
    public void RegisterAsync_ValidModel_MappingSuccess( UserRegistrationModel userModel)
    {
        //Act 
        var user = _mapper.Map<User>(userModel);

        //Assert
        user.Should().BeEquivalentTo(userModel, options => options.ExcludingMissingMembers());
        Assert.Equal(user.Id, Guid.Empty);
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

    [Theory]
    [MemberData(nameof(UsersServiceTestCases.ActiveUsers), MemberType = typeof(UsersServiceTestCases))]
    public void GetAllActiveUsersAsync_ValidModel_MappingSuccess(List<User> userDTOs)
    {
        //Act
        var courses = _mapper.Map<List<UserModel>>(userDTOs);

        //Assert
        courses.Should().BeEquivalentTo(userDTOs, options => options.ExcludingMissingMembers());
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

    [Theory]
    [MemberData(nameof(UsersServiceTestCases.UserTaughtCourses), MemberType = typeof(UsersServiceTestCases))]
    public void GetTaughtCoursesByUserIdAsync_ValidModel_MappingSuccess(List<Course> courseDTOs)
    { 
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

    [Theory]
    [MemberData(nameof(UsersServiceTestCases.UserEnrollments), MemberType = typeof(UsersServiceTestCases))]
    public void GetEnrollmentsByUserIdAsync_ValidModel_MappingSuccess(List<Enrollment> enrollmentDTOs)
    {
        //Act
        var courses = _mapper.Map<List<CourseEnrollmentModel>>(enrollmentDTOs);

        //Assert
        courses.Should().BeEquivalentTo(enrollmentDTOs, options => options.ExcludingMissingMembers());
    }

    [Theory]
    [MemberData(nameof(UsersServiceTestCases.UserFullInfo), MemberType = typeof(UsersServiceTestCases))]
    public void GetUserByIdAsync_ValidModel_MappingSuccess(User userDTO)
    {
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

    [Theory]
    [MemberData(nameof(UsersServiceTestCases.UserToUpdate), MemberType = typeof(UsersServiceTestCases))]
    public void UpdateProfileAsync_ValidModel_MappingSuccess(UpdateUserProfileModel userProfileModel)
    {
        //Act 
        var user = _mapper.Map<User>(userProfileModel);

        //Assert
        user.Should().BeEquivalentTo(userProfileModel, options => options.ExcludingMissingMembers());
        Assert.Equal(user.Id, Guid.Empty);
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
