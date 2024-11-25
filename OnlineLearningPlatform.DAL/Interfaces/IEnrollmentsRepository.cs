using OnlineLearningPlatform.DAL.DTOs;

namespace OnlineLearningPlatform.DAL.Interfaces;

public interface IEnrollmentsRepository
{
    Task CreateEnrollmentAsync(Enrollment enrollment);
}