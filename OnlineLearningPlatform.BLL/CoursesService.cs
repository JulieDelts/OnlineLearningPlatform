using OnlineLearningPlatform.BLL.Interfaces;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.BLL
{
    public class CoursesService: ICoursesService
    {
        private readonly ICoursesRepository _repository;
        public CoursesService(ICoursesRepository repository)
        {
            _repository = repository;
        }
    }
}
