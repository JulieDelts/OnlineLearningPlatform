using AutoMapper;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.Models.Responses;

namespace OnlineLearningPlatform.Mappings
{
    public class APICourseMapperProfile: Profile
    {
        public APICourseMapperProfile()
        {
            CreateMap<CourseModel, CourseResponse>();
            CreateMap<CourseEnrollmentModel, CourseEnrollmentResponse>();
        }
    }
}
