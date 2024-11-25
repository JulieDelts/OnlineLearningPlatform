
namespace OnlineLearningPlatform.DAL
{
    public class EnrollmentsRepository
    {

        private readonly OnlineLearningPlatformContext _context;

        public EnrollmentsRepository(OnlineLearningPlatformContext context)
        {
            _context = context;
        }
    }
}
