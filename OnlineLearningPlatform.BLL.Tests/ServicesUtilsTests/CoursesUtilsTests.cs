using Moq;
using OnlineLearningPlatform.BLL.Exceptions;
using OnlineLearningPlatform.BLL.ServicesUtils;
using OnlineLearningPlatform.DAL.DTOs;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.BLL.Tests.ServicesUtilsTests;

public class CoursesUtilsTests
{
    private readonly Mock<ICoursesRepository> _coursesRepositoryMock;
    private readonly CoursesUtils _sut;

    public CoursesUtilsTests()
    {
        _coursesRepositoryMock = new();
        _sut = new(_coursesRepositoryMock.Object);
    }

    [Fact]
    public async Task GetCourseByIdAsync_ExistingCourse_GetCourseSuccess()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course() { Id = courseId };
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdAsync(courseId)).ReturnsAsync(course);

        // Act
        await _sut.GetCourseByIdAsync(courseId);

        // Assert
        _coursesRepositoryMock.Verify(t =>
            t.GetCourseByIdAsync(courseId),
            Times.Once
        );
    }

    [Fact]
    public async Task GetCourseByIdAsync_NotExistingCourseSent_EntityNotFoundExceptionThrown()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var message = $"Course with id {courseId} was not found.";

        // Act
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _sut.GetCourseByIdAsync(courseId));

        // Assert
        Assert.Equal(message, exception.Message);
    }
}
