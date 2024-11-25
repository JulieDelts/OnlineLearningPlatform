
using OnlineLearningPlatform.DAL.DTOs;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.DAL
{
    public class EnrollmentsRepository : IEnrollmentsRepository
    {

        private readonly OnlineLearningPlatformContext _context;

        public EnrollmentsRepository(OnlineLearningPlatformContext context)
        {
            _context = context;
        }

        public async Task CreateEnrollmentAsync(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
        }
    }
}
