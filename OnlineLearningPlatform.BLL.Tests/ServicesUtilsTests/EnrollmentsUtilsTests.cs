using Moq;
using OnlineLearningPlatform.BLL.Exceptions;
using OnlineLearningPlatform.BLL.ServicesUtils;
using OnlineLearningPlatform.DAL.DTOs;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.BLL.Tests.ServicesUtilsTests;

public class EnrollmentsUtilsTests
{
    private readonly Mock<IEnrollmentsRepository> _enrollmentsRepositoryMock;
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly Mock<ICoursesRepository> _coursesRepositoryMock;
    private readonly EnrollmentsUtils _sut;

    public EnrollmentsUtilsTests()
    {
        _enrollmentsRepositoryMock = new();
        _usersRepositoryMock = new();
        _coursesRepositoryMock = new();
        _sut = new(_enrollmentsRepositoryMock.Object);
    }

    [Fact]
    public async Task GetEnrollmentAsync_ExistingEnrollment_GetEnrollmentSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var user = new User() { Id = userId };
        var course = new Course() { Id = courseId };
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(user);
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdAsync(courseId)).ReturnsAsync(course);
        _enrollmentsRepositoryMock.Setup(t => t.GetEnrollmentByIdAsync(courseId, userId)).ReturnsAsync(new Enrollment() { User = user, Course = course });

        // Act
        await _sut.GetEnrollmentAsync(courseId, userId);

        // Assert
        _enrollmentsRepositoryMock.Verify(t =>
            t.GetEnrollmentByIdAsync(courseId, userId),
            Times.Once
        );
    }

    [Fact]
    public async Task GetEnrollmentAsync_NotExistingEnrollmentSent_EntityNotFoundExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var message = $"Enrollment with user id {userId} and course id {courseId} was not found.";

        // Act
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _sut.GetEnrollmentAsync(courseId, userId));

        // Assert
        Assert.Equal(message, exception.Message);
    }
}
