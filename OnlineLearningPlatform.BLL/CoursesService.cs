using AutoMapper;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.BLL.Exceptions;
using OnlineLearningPlatform.BLL.Interfaces;
using OnlineLearningPlatform.BLL.ServicesUtils;
using OnlineLearningPlatform.Core;
using OnlineLearningPlatform.DAL.DTOs;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.BLL;

public class CoursesService(
    ICoursesRepository coursesRepository,
    IMapper mapper,
    CoursesUtils coursesUtils,
    UsersUtils usersUtils
    ) : ICoursesService
{
    public async Task<Guid> CreateCourseAsync(CreateCourseModel course)
    {
        var courseDTO = mapper.Map<Course>(course);

        var userDTO = await usersUtils.GetUserByIdAsync(course.TeacherId);

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
            throw new EntityNotFoundException($"Course with id {id} was not found.");

        var enrollmentDTOs = courseDTO.Enrollments.ToList();

        var userEnrollments = mapper.Map<List<UserEnrollmentModel>>(enrollmentDTOs);

        return userEnrollments;
    }

    public async Task<ExtendedCourseModel> GetFullCourseByIdAsync(Guid id)
    {
        var courseDTO = await coursesRepository.GetCourseByIdWithFullInfoAsync(id);

        if (courseDTO == null)
            throw new EntityNotFoundException($"Course with id {id} was not found.");

        var course = mapper.Map<ExtendedCourseModel>(courseDTO);

        return course;
    }

    public async Task UpdateCourseAsync(Guid id, UpdateCourseModel course, Guid teacherId)
    {
        var courseDTO = await coursesUtils.GetCourseByIdAsync(id);

        if (teacherId != courseDTO.TeacherId)
            throw new AuthorizationFailedException("Users are only allowed to update their own course info.");

        if (courseDTO.IsDeactivated)
            throw new EntityConflictException($"Course with id {id} is deactivated.");

        var courseUpdate = mapper.Map<Course>(course);

        await coursesRepository.UpdateCourseAsync(courseDTO, courseUpdate);
    }

    public async Task DeactivateCourseAsync(Guid id, Guid teacherId)
    {
        var courseDTO = await coursesUtils.GetCourseByIdAsync(id);

        if (teacherId != courseDTO.TeacherId)
            throw new AuthorizationFailedException("Users are only allowed to deactivate their own course.");

        await coursesRepository.DeactivateCourseAsync(courseDTO);
    }

    public async Task DeleteCourseAsync(Guid id)
    {
        var courseDTO = await coursesUtils.GetCourseByIdAsync(id);

        await coursesRepository.DeleteCourseAsync(courseDTO);
    }
}
