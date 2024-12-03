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
        CreateMap<User, ExtendedUserModel>();
        CreateMap<UpdateUserProfileModel, User>();
    }
}
