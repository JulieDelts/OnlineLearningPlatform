using AutoMapper;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.DAL.DTOs;

namespace OnlineLearningPlatform.BLL.Mappings;

internal class BLLUserMapperProfile: Profile
{
    public BLLUserMapperProfile()
    {
        CreateMap<User, UserModel>();
        CreateMap<User, ExtendedUserModel>();
        CreateMap<UserRegistrationModel, User>();
        CreateMap<UpdateUserProfileModel, User>();
    }
}
