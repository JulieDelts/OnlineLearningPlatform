using OnlineLearningPlatform.BLL.Exceptions;
using OnlineLearningPlatform.DAL.DTOs;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.BLL.ServicesUtils;

public class CoursesUtils(ICoursesRepository coursesRepository)
{
    public async Task<Course> GetCourseByIdAsync(Guid id)
    {
        var courseDTO = await coursesRepository.GetCourseByIdAsync(id);

        if (courseDTO == null)
            throw new EntityNotFoundException($"Course with id {id} was not found.");

        return courseDTO;
    }
}
