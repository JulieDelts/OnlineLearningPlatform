using AutoMapper;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.DAL.DTOs;

namespace OnlineLearningPlatform.BLL.Mappings;

internal class BLLCourseMapperProfile: Profile
{
    public BLLCourseMapperProfile() 
    {
        CreateMap<Course, CourseModel>();
        CreateMap<Course, ExtendedCourseModel>();
        CreateMap<Enrollment, CourseEnrollmentModel>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Course.Name))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Course.Description));
        CreateMap<CreateCourseModel, Course>();
        CreateMap<UpdateCourseModel, Course>();
    }
}
