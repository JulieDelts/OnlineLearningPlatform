using AutoMapper;
using OnlineLearningPlatform.Models.Requests;
using OnlineLearningPlatform.BLL.BusinessModels;

namespace OnlineLearningPlatform.Mappings
{
    public class APIUserMapperProfile: Profile
    {
        public APIUserMapperProfile()
        {
            CreateMap<RegisterRequest, UserRegistrationModel>();
        }
    }
}
