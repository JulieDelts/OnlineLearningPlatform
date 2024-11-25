using AutoMapper;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.DAL.DTOs;

namespace OnlineLearningPlatform.BLL.Mappings;

internal class BLLUserMapperProfile: Profile
{
    public BLLUserMapperProfile()
    {
        CreateMap<UserRegistrationModel, User>();
        CreateMap<User, UserModel>();
        CreateMap<User, ExtendedUserModel>()
             .ForMember(dest => dest.TaughtCourses, opt => opt.Ignore())
             .ForMember(dest => dest.Enrollments, opt => opt.Ignore());
        CreateMap<UpdateUserProfileModel, User>();
    }
}
