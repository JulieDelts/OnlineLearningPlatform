using Moq;
using OnlineLearningPlatform.BLL.BusinessModels;
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

    [Fact]
    public async Task ControlAttendanceAsync_ExistingEnrollmentActiveUserActiveCourseValidAttendance_SetAttendanceSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var attendance = 9;
        var course = new Course() { Id = courseId, NumberOfLessons = 10 };
        var student = new User() { Id = userId };
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdAsync(courseId)).ReturnsAsync(course);
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(student);
        _enrollmentsRepositoryMock.Setup(t => t.GetEnrollmentByIdAsync(courseId, userId)).ReturnsAsync(new Enrollment() { User = student, Course = course });
        var enrollment = new EnrollmentManagementModel()
        {
            UserId = userId,
            CourseId = courseId
        };

        // Act
        await _sut.ControlAttendanceAsync(enrollment, attendance);

        // Assert
        _enrollmentsRepositoryMock.Verify(t =>
            t.ControlAttendanceAsync(It.Is<Enrollment>(t => t.User == student && t.Course == course), attendance),
            Times.Once
        );
    }

    [Fact]
    public async Task ControlAttendanceAsync_NotExistingEnrollmentSent_EntityNotFoundExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var attendance = 10;
        var message = $"Enrollment with user id {userId} and course id {courseId} was not found.";
        var enrollment = new EnrollmentManagementModel()
        {
            UserId = userId,
            CourseId = courseId
        };

        // Act
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _sut.ControlAttendanceAsync(enrollment, attendance));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task ControlAttendanceAsync_DeactivatedCourseSent_EntityConflictExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var attendance = 10;
        var message = $"Course with id {courseId} is deactivated.";
        var course = new Course() { Id = courseId, IsDeactivated = true };
        var student = new User() { Id = userId };
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdAsync(courseId)).ReturnsAsync(course);
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(student);
        _enrollmentsRepositoryMock.Setup(t => t.GetEnrollmentByIdAsync(courseId, userId)).ReturnsAsync(new Enrollment() { User = student, Course = course });
        var enrollment = new EnrollmentManagementModel()
        {
            UserId = userId,
            CourseId = courseId
        };

        // Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.ControlAttendanceAsync(enrollment, attendance));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task ControlAttendanceAsync_DeactivatedUserSent_EntityConflictExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var attendance = 10;
        var message = $"User with id {userId} is deactivated.";
        var course = new Course() { Id = courseId };
        var student = new User() { Id = userId, IsDeactivated = true };
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdAsync(courseId)).ReturnsAsync(course);
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(student);
        _enrollmentsRepositoryMock.Setup(t => t.GetEnrollmentByIdAsync(courseId, userId)).ReturnsAsync(new Enrollment() { User = student, Course = course });
        var enrollment = new EnrollmentManagementModel()
        {
            UserId = userId,
            CourseId = courseId
        };

        // Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.ControlAttendanceAsync(enrollment, attendance));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task ControlAttendanceAsync_AttendanceLessThanZero_ArgumentExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var attendance = -10;
        var message = "The attendance is out of the acceptable range.";
        var course = new Course() { Id = courseId, NumberOfLessons = 10 };
        var student = new User() { Id = userId };
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdAsync(courseId)).ReturnsAsync(course);
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(student);
        _enrollmentsRepositoryMock.Setup(t => t.GetEnrollmentByIdAsync(courseId, userId)).ReturnsAsync(new Enrollment() { User = student, Course = course });
        var enrollment = new EnrollmentManagementModel()
        {
            UserId = userId,
            CourseId = courseId
        };

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _sut.ControlAttendanceAsync(enrollment, attendance));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task ControlAttendanceAsync_AttendanceGreaterThanNumberOfLessons_ArgumentExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var attendance = 20;
        var message = "The attendance is out of the acceptable range.";
        var course = new Course() { Id = courseId, NumberOfLessons = 10 };
        var student = new User() { Id = userId };
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdAsync(courseId)).ReturnsAsync(course);
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(student);
        _enrollmentsRepositoryMock.Setup(t => t.GetEnrollmentByIdAsync(courseId, userId)).ReturnsAsync(new Enrollment() { User = student, Course = course });
        var enrollment = new EnrollmentManagementModel()
        {
            UserId = userId,
            CourseId = courseId
        };

        // Act
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _sut.ControlAttendanceAsync(enrollment, attendance));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task DisenrollAsync_ExistingEnrollment_DisenrollSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var course = new Course() { Id = courseId };
        var student = new User() { Id = userId };
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdAsync(courseId)).ReturnsAsync(course);
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(student);
        _enrollmentsRepositoryMock.Setup(t => t.GetEnrollmentByIdAsync(courseId, userId)).ReturnsAsync(new Enrollment() { User = student, Course = course });
        var enrollment = new EnrollmentManagementModel()
        {
            UserId = userId,
            CourseId = courseId
        };

        // Act
        await _sut.DisenrollAsync(enrollment);

        // Assert
        _enrollmentsRepositoryMock.Verify(t =>
            t.DisenrollAsync(It.Is<Enrollment>(t => t.User == student && t.Course == course)),
            Times.Once
        );
    }

    [Fact]
    public async Task DisenrollAsync_NotExistingEnrollment_EntityNotFoundExceptionThrown()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var message = $"Enrollment with user id {userId} and course id {courseId} was not found.";
        var enrollment = new EnrollmentManagementModel()
        {
            UserId = userId,
            CourseId = courseId
        };

        // Act
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _sut.DisenrollAsync(enrollment));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task GradeStudentAsync_ExistingEnrollmentActiveCourseActiveUser_GradeStudentSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var grade = 5;
        var course = new Course() { Id = courseId };
        var student = new User() { Id = userId };
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdAsync(courseId)).ReturnsAsync(course);
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(student);
        _enrollmentsRepositoryMock.Setup(t => t.GetEnrollmentByIdAsync(courseId, userId)).ReturnsAsync(new Enrollment() { User = student, Course = course });
        var enrollment = new EnrollmentManagementModel()
        {
            UserId = userId,
            CourseId = courseId
        };

        // Act
        await _sut.GradeStudentAsync(enrollment, grade);

        // Assert
        _enrollmentsRepositoryMock.Verify(t =>
            t.GradeStudentAsync(It.Is<Enrollment>(t => t.User == student && t.Course == course), grade),
            Times.Once
        );
    }

    [Fact]
    public async Task GradeStudentAsync_NotExistingEnrollment_EntityNotFoundExceptionThrown()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var grade = 5;
        var message = $"Enrollment with user id {userId} and course id {courseId} was not found.";
        var enrollment = new EnrollmentManagementModel()
        {
            UserId = userId,
            CourseId = courseId
        };

        // Act
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _sut.GradeStudentAsync(enrollment, grade));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task GradeStudentAsync_DeactivatedCourseSent_EntityConflictExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var grade = 5;
        var message = $"Course with id {courseId} is deactivated.";
        var course = new Course() { Id = courseId, IsDeactivated = true };
        var student = new User() { Id = userId };
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdAsync(courseId)).ReturnsAsync(course);
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(student);
        _enrollmentsRepositoryMock.Setup(t => t.GetEnrollmentByIdAsync(courseId, userId)).ReturnsAsync(new Enrollment() { User = student, Course = course });
        var enrollment = new EnrollmentManagementModel()
        {
            UserId = userId,
            CourseId = courseId
        };

        // Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.GradeStudentAsync(enrollment, grade));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task GradeStudentAsync_DeactivatedUserSent_EntityConflictExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var grade = 5;
        var message = $"User with id {userId} is deactivated.";
        var course = new Course() { Id = courseId };
        var student = new User() { Id = userId, IsDeactivated = true };
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdAsync(courseId)).ReturnsAsync(course);
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(student);
        _enrollmentsRepositoryMock.Setup(t => t.GetEnrollmentByIdAsync(courseId, userId)).ReturnsAsync(new Enrollment() { User = student, Course = course });
        var enrollment = new EnrollmentManagementModel()
        {
            UserId = userId,
            CourseId = courseId
        };

        // Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.GradeStudentAsync(enrollment, grade));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task ReviewCourseAsync_ExistingEnrollmentActiveCourseActiveUser_ReviewCourseSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var review = "This is a course review.";
        var course = new Course() { Id = courseId };
        var student = new User() { Id = userId };
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdAsync(courseId)).ReturnsAsync(course);
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(student);
        _enrollmentsRepositoryMock.Setup(t => t.GetEnrollmentByIdAsync(courseId, userId)).ReturnsAsync(new Enrollment() { User = student, Course = course });
        var enrollment = new EnrollmentManagementModel()
        {
            UserId = userId,
            CourseId = courseId
        };

        // Act
        await _sut.ReviewCourseAsync(enrollment, review);

        // Assert
        _enrollmentsRepositoryMock.Verify(t =>
            t.ReviewCourseAsync(It.Is<Enrollment>(t => t.User == student && t.Course == course), review),
            Times.Once
        );
    }

    [Fact]
    public async Task ReviewCourseAsync_NotExistingEnrollment_EntityNotFoundExceptionThrown()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var review = "This is a course review.";
        var message = $"Enrollment with user id {userId} and course id {courseId} was not found.";
        var enrollment = new EnrollmentManagementModel()
        {
            UserId = userId,
            CourseId = courseId
        };

        // Act
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _sut.ReviewCourseAsync(enrollment, review));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task ReviewCourseAsync_DeactivatedUserSent_EntityConflictExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var review = "This is a course review.";
        var message = $"User with id {userId} is deactivated.";
        var course = new Course() { Id = courseId };
        var student = new User() { Id = userId, IsDeactivated = true };
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdAsync(courseId)).ReturnsAsync(course);
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(student);
        _enrollmentsRepositoryMock.Setup(t => t.GetEnrollmentByIdAsync(courseId, userId)).ReturnsAsync(new Enrollment() { User = student, Course = course });
        var enrollment = new EnrollmentManagementModel()
        {
            UserId = userId,
            CourseId = courseId
        };

        // Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.ReviewCourseAsync(enrollment, review));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task ReviewCourseAsync_DeactivatedCourseSent_EntityConflictExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var review = "This is a course review.";
        var message = $"Course with id {courseId} is deactivated.";
        var course = new Course() { Id = courseId, IsDeactivated = true };
        var student = new User() { Id = userId };
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdAsync(courseId)).ReturnsAsync(course);
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(student);
        _enrollmentsRepositoryMock.Setup(t => t.GetEnrollmentByIdAsync(courseId, userId)).ReturnsAsync(new Enrollment() { User = student, Course = course });
        var enrollment = new EnrollmentManagementModel()
        {
            UserId = userId,
            CourseId = courseId
        };

        // Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.ReviewCourseAsync(enrollment, review));

        // Assert
        Assert.Equal(message, exception.Message);
    }
}