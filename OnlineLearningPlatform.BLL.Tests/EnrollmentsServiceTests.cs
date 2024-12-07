using Moq;
using OnlineLearningPlatform.BLL.Exceptions;
using OnlineLearningPlatform.DAL.DTOs;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.BLL.Tests;

public class EnrollmentsServiceTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly Mock<ICoursesRepository> _coursesRepositoryMock;
    private readonly Mock<IEnrollmentsRepository> _enrollmentsRepositoryMock;
    private readonly EnrollmentsService _sut;

    public EnrollmentsServiceTests()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _coursesRepositoryMock = new Mock<ICoursesRepository>();
        _enrollmentsRepositoryMock = new Mock<IEnrollmentsRepository>();
        _sut = new EnrollmentsService(_usersRepositoryMock.Object, _coursesRepositoryMock.Object, _enrollmentsRepositoryMock.Object);
    }

    [Fact]
    public async Task EnrollAsync_ExistingActiveCourseAndExistingActiveStudentAndEnrollmentDoesNotExist_StudentEnrollSuccess()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingCourse = new Course() { Id = courseId };
        var existingStudent = new User() { Id = userId, Role = Core.Role.Student };
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdAsync(courseId)).ReturnsAsync(existingCourse);
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(existingStudent);

        // Act
        await _sut.EnrollAsync(courseId, userId);

        // Assert
        _enrollmentsRepositoryMock.Verify(t => 
            t.EnrollAsync(It.Is<Enrollment>(t => t.Course == existingCourse && t.User == existingStudent)), 
            Times.Once
        );
    }

    [Fact]
    public async Task EnrollAsync_NotExistingCourseSent_EntityNotFoundExceptionThrown()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var message = $"Course with id {courseId} was not found.";

        // Act
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _sut.EnrollAsync(courseId, Guid.NewGuid()));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task EnrollAsync_DeactivatedCourseSent_EntityConflictExceptionThrown()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var message = $"Course with id {courseId} is deactivated.";
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdAsync(courseId)).ReturnsAsync(new Course() { Id = courseId, IsDeactivated = true });

        // Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.EnrollAsync(courseId, Guid.NewGuid()));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task EnrollAsync_ActiveCourseAndNotExistingUserSent_EntityNotFoundExceptionThrown()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var message = $"User with id {userId} was not found.";
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdAsync(courseId)).ReturnsAsync(new Course() { Id = courseId });

        // Act
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _sut.EnrollAsync(courseId, userId));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task EnrollAsync_ActiveCourseAndDeactivatedUserSent_EntityConflictExceptionThrown()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var message = $"User with id {userId} is deactivated.";
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdAsync(courseId)).ReturnsAsync(new Course() { Id = courseId });
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(new User() { Id = userId, IsDeactivated = true });

        // Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.EnrollAsync(courseId, userId));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task EnrollAsync_ActiveCourseAndActiveUserButNotStudentSent_EntityConflictExceptionThrown()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var message = "The role of the user is not correct.";
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdAsync(courseId)).ReturnsAsync(new Course() { Id = courseId });
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(new User() { Id = userId, Role = Core.Role.Teacher });

        // Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.EnrollAsync(courseId, userId));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task EnrollAsync_StudentTriesToEnrollTwice_EntityConflictExceptionThrown()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var message = $"Enrollment with user id {userId} and course id {courseId} already exists.";
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdAsync(courseId)).ReturnsAsync(new Course() { Id = courseId });
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(new User() { Id = userId, Role = Core.Role.Student });
        _enrollmentsRepositoryMock.Setup(t => t.GetEnrollmentByIdAsync(courseId, userId)).ReturnsAsync(new Enrollment() { UserId = userId, CourseId = courseId });

        // Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.EnrollAsync(courseId, userId));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    //[Fact]
    //public void Enroll_NotExistingCourseSent_EntityNotFoundExceptionThrown()
    //{
    //    // Arrange
    //    var courseId = Guid.NewGuid();
    //    var message = $"Course with id {courseId} was not found.";

    //    // Act
    //    Assert.Throws<EntityNotFoundException>(() => _sut.Enroll(courseId, Guid.NewGuid()));

    //    // Assert
    //}
}