
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

public class CoursesServiceTests
{
    private readonly Mock<ICoursesRepository> _coursesRepositoryMock;
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly Mapper _mapper;
    private readonly CoursesService _sut;

    public CoursesServiceTests()
    {
        _coursesRepositoryMock = new();
        _usersRepositoryMock = new();
        var config = new MapperConfiguration(
        cfg =>
        {
            cfg.AddProfile(new BLLUserMapperProfile());
            cfg.AddProfile(new BLLCourseMapperProfile());
        });
        _mapper = new Mapper(config);
        _sut = new(
            _coursesRepositoryMock.Object,
            _mapper,
            new CoursesUtils(_coursesRepositoryMock.Object),
            new UsersUtils(_usersRepositoryMock.Object)
            );
    }

    [Fact]
    public async Task CreateCourseAsync_ValidMappingActiveTeacher_CreateCourseSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseModel = new CreateCourseModel { TeacherId = userId };
        var user = new User { Id = courseModel.TeacherId, Role = Role.Teacher, IsDeactivated = false };
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _sut.CreateCourseAsync(courseModel);

        // Assert
        _coursesRepositoryMock.Verify(t =>
            t.CreateCourseAsync(It.Is<Course>(t => t.TeacherId == userId && t.Teacher == user)),
            Times.Once
        );
    }

    [Fact]
    public void CreateCourseAsync_ValidModel_MappingSuccess()
    {
        // Arrange
        var name = "TestCourse";
        var description = "This is a test course.";
        var numberOfLessons = 10;
        var teacherId = Guid.NewGuid();

        var courseModel = new CreateCourseModel()
        {
            Name = name,
            Description = description,
            NumberOfLessons = numberOfLessons,
            TeacherId = teacherId
        };

        //Act 
        var course = _mapper.Map<Course>(courseModel);

        //Assert
        Assert.Equal(course.Id, Guid.Empty);
        Assert.Equal(courseModel.Name, course.Name);
        Assert.Equal(courseModel.Description, course.Description);
        Assert.Equal(courseModel.TeacherId, course.TeacherId);
        Assert.Equal(courseModel.NumberOfLessons, course.NumberOfLessons);
        Assert.False(course.IsDeactivated);
        Assert.Null(course.Teacher);
        Assert.Empty(course.Enrollments);
    }

    [Fact]
    public async Task CreateCourseAsync_WrongUserRole_EntityConflictExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var message = "The role of the user is not correct.";
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(new User() { Id = userId, Role = Role.Student });
        var courseModel = new CreateCourseModel { TeacherId = userId };

        // Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.CreateCourseAsync(courseModel));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task CreateCourseAsync_UserDeactivated_EntityConflictExceptionThrown()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var message = $"User with id {userId} is deactivated.";
        _usersRepositoryMock.Setup(t => t.GetUserByIdAsync(userId)).ReturnsAsync(
            new User() { Id = userId, Role = Role.Teacher, IsDeactivated = true });
        var courseModel = new CreateCourseModel { TeacherId = userId };

        // Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.CreateCourseAsync(courseModel));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task GetAllActiveCoursesAsync_GetAllActiveCoursesSuccess()
    {
        // Act
        var result = await _sut.GetAllActiveCoursesAsync();

        // Assert
        _coursesRepositoryMock.Verify(t =>
            t.GetAllActiveCoursesAsync(),
            Times.Once
        );
    }

    [Fact]
    public void GetAllActiveCoursesAsync_ValidModel_MappingSuccess()
    {
        // Arrange
        var courseDTOs = new List<Course>()
        {
            new Course { Name = "FirstTestCourse", Description = "This is the first test course." },
            new Course { Name = "SecondTestCourse", Description = "This is the second test course." },
            new Course { Name = "ThirdTestCourse", Description = "This is the third test course." },
            new Course { Name = "FourthTestCourse", Description = "This is the fourth test course." }
        };

        //Act
        var courses = _mapper.Map<List<CourseModel>>(courseDTOs);

        //Assert
        courses.Should().BeEquivalentTo(courseDTOs, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GetEnrollmentsByCourseIdAsync_ExistingCourseValidMapping_GetEnrollmentsSuccess()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdWithFullInfoAsync(courseId)).ReturnsAsync(
            new Course() { Id = courseId });

        //Act
        await _sut.GetEnrollmentsByCourseIdAsync(courseId);

        //Assert
        _coursesRepositoryMock.Verify(t =>
           t.GetCourseByIdWithFullInfoAsync(courseId),
           Times.Once
        );
    }

    [Fact]
    public void GetEnrollmentsByCourseIdAsync_ValidModel_MappingSuccess()
    {
        // Arrange
        var enrollmentDTOs = new List<Enrollment>()
        {
            new Enrollment()
            {
                User = new User()
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "test1@gmail.com"
                },
                Attendance = 1,
                Grade = null
            },
            new Enrollment()
            {
                User = new User()
                {
                    FirstName = "Kate",
                    LastName = "Beck",
                    Email = "test2@gmail.com"
                },
                Attendance = null,
                Grade = 5
            }
        };

        //Act
        var userEnrollments = _mapper.Map<List<UserEnrollmentModel>>(enrollmentDTOs);

        //Assert
        userEnrollments.Should().BeEquivalentTo(enrollmentDTOs, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GetEnrollmentsByCourseIdAsync_NotExistingCourseSent_EntityNotFoundExceptionThrown()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var message = $"Course with id {courseId} was not found.";

        // Act
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _sut.GetEnrollmentsByCourseIdAsync(courseId));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task GetFullCourseByIdAsync_ExistingCourseValidMapping_GetCourseSuccess()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course() { Id = courseId };
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdWithFullInfoAsync(courseId)).ReturnsAsync(course);

        // Act
        await _sut.GetFullCourseByIdAsync(courseId);

        // Assert
        _coursesRepositoryMock.Verify(t =>
            t.GetCourseByIdWithFullInfoAsync(courseId),
            Times.Once
        );
    }

    [Fact]
    public void GetFullCourseByIdAsync_ValidModel_MappingSuccess()
    {
        // Arrange
        var name = "TestCourse";
        var description = "This is a test course.";
        var numberOfLessons = 20;
        var teacherId = Guid.NewGuid();

        var courseDTO = new Course()
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            NumberOfLessons = numberOfLessons,
            TeacherId = teacherId,
            IsDeactivated = false,
            Teacher = new User()
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "test1@gmail.com"
            }
        };

        //Act 
        var course = _mapper.Map<ExtendedCourseModel>(courseDTO);

        //Assert
        course.Should().BeEquivalentTo(courseDTO, options => options.ExcludingMissingMembers());
    }

    [Fact]
    public async Task GetFullCourseByIdAsync_NotExistingCourseSent_EntityNotFoundExceptionThrown()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var message = $"Course with id {courseId} was not found.";

        // Act
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(async () => await _sut.GetFullCourseByIdAsync(courseId));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task UpdateCourseAsync_ActiveCourseValidMapping_UpdateCourseSuccess()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course() { Id = courseId };
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdAsync(courseId)).ReturnsAsync(course);
        var courseUpdateModel = new UpdateCourseModel() { Name = "NewName", Description = "NewDescription" };
        var courseUpdateDTO = new Course() { Name = courseUpdateModel.Name, Description = courseUpdateModel.Description };

        // Act
        await _sut.UpdateCourseAsync(courseId, courseUpdateModel);

        // Assert
        _coursesRepositoryMock.Verify(t =>
            t.UpdateCourseAsync(It.Is<Course>(t => t.Id == courseId),
            It.Is<Course>(t => t.Name == courseUpdateDTO.Name && t.Description == courseUpdateDTO.Description)),
            Times.Once
        );
    }

    [Fact]
    public void UpdateCourseAsync_ValidModel_MappingSuccess()
    {
        // Arrange
        var name = "NewCourseName";
        var description = "This is a new course description.";
        var numberOfLessons = 10;

        var courseModel = new UpdateCourseModel()
        {
            Name = name,
            Description = description,
            NumberOfLessons = numberOfLessons
        };

        //Act 
        var course = _mapper.Map<Course>(courseModel);

        //Assert
        Assert.Equal(course.Id, Guid.Empty);
        Assert.Equal(courseModel.Name, course.Name);
        Assert.Equal(courseModel.Description, course.Description);
        Assert.Equal(course.Id, Guid.Empty);
        Assert.Equal(courseModel.NumberOfLessons, course.NumberOfLessons);
        Assert.False(course.IsDeactivated);
        Assert.Null(course.Teacher);
        Assert.Empty(course.Enrollments);
    }

    [Fact]
    public async Task UpdateCourseAsync_CourseDeactivated_EntityConflictExceptionThrown()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var message = $"Course with id {courseId} is deactivated.";
        var course = new Course() { Id = courseId, IsDeactivated = true };
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdAsync(courseId)).ReturnsAsync(course);
        var courseUpdateModel = new UpdateCourseModel();

        // Act
        var exception = await Assert.ThrowsAsync<EntityConflictException>(async () => await _sut.UpdateCourseAsync(courseId, courseUpdateModel));

        // Assert
        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public async Task DeactivateCourseAsync_DeactivateCourseSuccess()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course() { Id = courseId };
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdAsync(courseId)).ReturnsAsync(course);

        // Act
        await _sut.DeactivateCourseAsync(courseId);

        // Assert
        _coursesRepositoryMock.Verify(t =>
            t.DeactivateCourseAsync(It.Is<Course>(t => t.Id == courseId)),
            Times.Once
        );
    }

    [Fact]
    public async Task DeleteCourseAsync_DeleteCourseSuccess()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course() { Id = courseId };
        _coursesRepositoryMock.Setup(t => t.GetCourseByIdAsync(courseId)).ReturnsAsync(course);

        // Act
        await _sut.DeleteCourseAsync(courseId);

        // Assert
        _coursesRepositoryMock.Verify(t =>
            t.DeleteCourseAsync(It.Is<Course>(t => t.Id == courseId)),
            Times.Once
        );
    }
}
