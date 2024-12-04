using AutoMapper;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.Models.Requests;
using OnlineLearningPlatform.Models.Responses;

namespace OnlineLearningPlatform.Mappings;

internal class APICourseMapperProfile: Profile
{
    public APICourseMapperProfile()
    {
        CreateMap<CourseModel, CourseResponse>();
        CreateMap<CourseEnrollmentModel, CourseEnrollmentResponse>();
        CreateMap<UserEnrollmentModel, UserEnrollmentResponse>();
        CreateMap<ExtendedCourseModel, ExtendedCourseResponse>();
        CreateMap<CreateCourseRequest, CreateCourseModel>();
        CreateMap<UpdateCourseRequest, UpdateCourseModel>();
    }
}
