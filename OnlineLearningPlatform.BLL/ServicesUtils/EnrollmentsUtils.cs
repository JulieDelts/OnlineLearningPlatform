using OnlineLearningPlatform.BLL.Exceptions;
using OnlineLearningPlatform.DAL.DTOs;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.BLL.ServicesUtils;

public class EnrollmentsUtils(IEnrollmentsRepository enrollmentsRepository)
{
    public async Task<Enrollment> GetEnrollmentAsync(Guid courseId, Guid userId)
    {
        var enrollmentDTO = await enrollmentsRepository.GetEnrollmentByIdAsync(courseId, userId);

        if (enrollmentDTO == null)
            throw new EntityNotFoundException($"Enrollment with user id {userId} and course id {courseId} was not found.");

        return enrollmentDTO;
    }
}
