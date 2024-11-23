using AutoMapper;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.DAL.DTOs;

namespace OnlineLearningPlatform.BLL.Mappings
{
    public class BLLUserMapperProfile: Profile
    {
        public BLLUserMapperProfile()
        {
            CreateMap<UserRegistrationModel, User>();
        }
    }
}
