
using AutoMapper;
using Moq;
using OnlineLearningPlatform.BLL.Mappings;
using OnlineLearningPlatform.BLL.ServicesUtils;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.BLL.Tests;

public class CoursesServiceTests
{
    private readonly Mock<ICoursesRepository> _coursesRepositoryMock;
    private readonly Mapper _mapper;
    private readonly CoursesService _sut;

    public CoursesServiceTests()
    {
        _coursesRepositoryMock = new();
        var config = new MapperConfiguration(
        cfg =>
        {
            cfg.AddProfile(new BLLUserMapperProfile());
            cfg.AddProfile(new BLLCourseMapperProfile());
        });
        _mapper = new Mapper(config);
        _sut = new(
            _coursesRepositoryMock.Object,
            _mapper,
            new CoursesUtils(_coursesRepositoryMock.Object),
            new UsersUtils(new Mock<IUsersRepository>().Object)
            );
    }


}
