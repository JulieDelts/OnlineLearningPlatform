﻿
using AutoMapper;
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.DAL.DTOs;

namespace OnlineLearningPlatform.BLL.Mappings
{
    public class BLLCourseMapperProfile: Profile
    {
        public BLLCourseMapperProfile() 
        {
            CreateMap<Course, CourseModel>();
            CreateMap<Enrollment, CourseEnrollmentModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Course.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Course.Description));
        }
    }
}