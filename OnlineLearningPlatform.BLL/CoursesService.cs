using AutoMapper;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.BLL.Exceptions;
using OnlineLearningPlatform.BLL.Interfaces;
using OnlineLearningPlatform.Core;
using OnlineLearningPlatform.DAL.DTOs;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.BLL;

public class CoursesService(
    IUsersRepository usersRepository,
    ICoursesRepository coursesRepository,
    IMapper mapper
    ) : ICoursesService
{
    public async Task<Guid> CreateCourseAsync(CreateCourseModel course)
    {
        var courseDTO = mapper.Map<Course>(course);

        var userDTO = await usersRepository.GetUserByIdAsync(course.TeacherId);

        if (userDTO.Role != Role.Teacher)
            throw new EntityConflictException("The role of the user is not correct.");

        if (userDTO.IsDeactivated)
            throw new EntityConflictException($"User with id {userDTO.Id} is deactivated.");

        courseDTO.Teacher = userDTO;

        var id = await coursesRepository.CreateCourseAsync(courseDTO);

        return id;
    }

    public async Task<List<CourseModel>> GetAllActiveCoursesAsync()
    {
        var courseDTOs = await coursesRepository.GetAllActiveCoursesAsync();

        var courses = mapper.Map<List<CourseModel>>(courseDTOs);

        return courses;
    }

    //TODO
    public async Task<List<UserEnrollmentModel>> GetEnrollmentsByCourseIdAsync(Guid id)
    {
        var courseDTO = await coursesRepository.GetCourseByIdWithFullInfoAsync(id);

        if (courseDTO == null)
            throw new EntityNotFoundException($"Course with id {id} was not found");

        var enrollmentDTOs = courseDTO.Enrollments.ToList();

        var userEnrollments = mapper.Map<List<UserEnrollmentModel>>(enrollmentDTOs);

        return userEnrollments;
    }

    public async Task<ExtendedCourseModel> GetCourseByIdAsync(Guid id)
    {
        var courseDTO = await coursesRepository.GetCourseByIdWithFullInfoAsync(id);

        if (courseDTO == null)
            throw new EntityNotFoundException($"Course with id {id} was not found.");

        var course = mapper.Map<ExtendedCourseModel>(courseDTO);

        return course;
    }

    public async Task UpdateCourseAsync(Guid id, UpdateCourseModel course)
    {
        var courseDTO = await coursesRepository.GetCourseByIdAsync(id);

        if (courseDTO == null)
            throw new EntityNotFoundException($"Course with id {id} was not found.");

        if (courseDTO.IsDeactivated)
            throw new EntityConflictException($"Course with id {id} is deactivated.");

        var courseUpdate = mapper.Map<Course>(course);

        await coursesRepository.UpdateCourseAsync(courseDTO, courseUpdate);
    }

    public async Task DeactivateCourseAsync(Guid id)
    {
        var courseDTO = await coursesRepository.GetCourseByIdAsync(id);

        if (courseDTO == null)
            throw new EntityNotFoundException($"Course with id {id} was not found.");

        await coursesRepository.DeactivateCourseAsync(courseDTO);
    }

    public async Task DeleteCourseAsync(Guid id)
    {
        var courseDTO = await coursesRepository.GetCourseByIdAsync(id);

        if (courseDTO == null)
            throw new EntityNotFoundException($"Course with id {id} was not found.");

        await coursesRepository.DeleteCourseAsync(courseDTO);
    }
}
