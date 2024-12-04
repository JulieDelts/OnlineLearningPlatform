using AutoMapper;
using OnlineLearningPlatform.Models.Requests;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.Models.Responses;

namespace OnlineLearningPlatform.Mappings;

internal class APIUserMapperProfile: Profile
{
    public APIUserMapperProfile()
    {
        CreateMap<UserModel, UserResponse>();
        CreateMap<ExtendedUserModel, ExtendedUserResponse>();
        CreateMap<RegisterRequest, UserRegistrationModel>();
        CreateMap<UpdateUserPasswordRequest, UpdateUserPasswordModel>();
        CreateMap<UpdateUserProfileRequest, UpdateUserProfileModel>();
    }
}
