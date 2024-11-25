using AutoMapper;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.Models.Requests;
using OnlineLearningPlatform.Models.Responses;

namespace OnlineLearningPlatform.Mappings
{
    public class APICourseMapperProfile: Profile
    {
        public APICourseMapperProfile()
        {
            CreateMap<CourseModel, CourseResponse>();
            CreateMap<CourseEnrollmentModel, CourseEnrollmentResponse>();
            CreateMap<CreateCourseRequest, CreateCourseModel>();
            CreateMap<UpdateCourseRequest, UpdateCourseModel>();
            CreateMap<ExtendedCourseModel, ExtendedCourseResponse>();
        }
    }
}
