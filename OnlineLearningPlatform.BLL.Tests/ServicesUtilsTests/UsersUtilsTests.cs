using Moq;
using OnlineLearningPlatform.BLL.Exceptions;
using OnlineLearningPlatform.BLL.ServicesUtils;
using OnlineLearningPlatform.DAL.DTOs;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.BLL.Tests.ServicesUtilsTests;

public class UsersUtilsTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly UsersUtils _sut;

    public UsersUtilsTests()
    {
        _usersRepositoryMock = new();
        _sut = new(_usersRepositoryMock.Object);
    }

    [Fact]
    public async Task GetUserByIdAsync_ExistingUser_GetUserSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User() { Id = userId };
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(user);

        // Act
        await _sut.GetUserByIdAsync(userId);

        // Assert
        _usersRepositoryMock.Verify(t =>
            t.GetUserByIdAsync(userId),
            Times.Once
        );
    }

    [Fact]
    public async Task GetUserByIdAsync_NotExistingUserSent_EntityNotFoundExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var message = $"User with id {userId} was not found.";

        // Act
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _sut.GetUserByIdAsync(userId));

        // Assert
        Assert.Equal(message, exception.Message);
    }
}
