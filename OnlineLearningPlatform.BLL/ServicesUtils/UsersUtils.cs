using OnlineLearningPlatform.BLL.Exceptions;
using OnlineLearningPlatform.DAL.DTOs;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.BLL.ServicesUtils;

public class UsersUtils(IUsersRepository usersRepository)
{
    public async Task<User> GetUserByIdAsync(Guid id)
    {
        var userDTO = await usersRepository.GetUserByIdAsync(id);

        if (userDTO == null)
            throw new EntityNotFoundException($"User with id {id} was not found.");

        return userDTO;
    }

    public async Task<User> GetUserFullInfoByIdAsync(Guid id)
    {
        var userDTO = await usersRepository.GetUserByIdWithFullInfoAsync(id);

        if (userDTO == null)
            throw new EntityNotFoundException($"User with id {id} was not found.");

        return userDTO;
    }
}
