using AutoMapper;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.DAL.DTOs;

namespace OnlineLearningPlatform.BLL.Mappings
{
    public class BLLUserMapperProfile: Profile
    {
        public BLLUserMapperProfile()
        {
            CreateMap<UserRegistrationModel, User>()
                 .ForMember(dest => dest.Id, opt => opt.Ignore()) 
                 .ForMember(dest => dest.Role, opt => opt.Ignore()); 
        }
    }
}
