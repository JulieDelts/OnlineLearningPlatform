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
        CreateMap<Enrollment, UserEnrollmentModel>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email));
        CreateMap<CreateCourseModel, Course>();
        CreateMap<UpdateCourseModel, Course>();
    }
}
