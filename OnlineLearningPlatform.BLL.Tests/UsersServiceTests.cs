using AutoMapper;
using Moq;
using OnlineLearningPlatform.BLL.Mappings;
using OnlineLearningPlatform.BLL.ServicesUtils;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.BLL.Tests;

public class UsersServiceTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly Mapper _mapper;
    private readonly UsersService _sut;

    public UsersServiceTests()
    {
        _usersRepositoryMock = new();
        var config = new MapperConfiguration(
        cfg =>
        {
            cfg.AddProfile(new BLLUserMapperProfile());
            cfg.AddProfile(new BLLCourseMapperProfile());
        });
        _mapper = new Mapper(config);
        _sut = new(
            _usersRepositoryMock.Object,
            _mapper,
            new UsersUtils(_usersRepositoryMock.Object)
            );
    }
}
