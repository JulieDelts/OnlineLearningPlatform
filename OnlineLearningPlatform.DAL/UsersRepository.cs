using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Core;
using OnlineLearningPlatform.DAL.DTOs;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.DAL;

public class UsersRepository(OnlineLearningPlatformContext context) : IUsersRepository
{
    public async Task<Guid> RegisterAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveChangesAsync();

        return user.Id;
    }

    public async Task<User?> GetUserByLoginAsync(string login)
    {
        return await context.Users.SingleOrDefaultAsync(u => u.Login == login);
    }

    public async Task<List<User>> GetAllActiveUsersAsync()
    {
        return await context.Users.Where(u => u.IsDeactivated == false).ToListAsync();
    }

    public async Task<User?> GetUserByIdWithFullInfoAsync(Guid id) => 
        await context.Users
            .Include(u => u.Enrollments)
                .ThenInclude(en => en.Course)
            .Include(u => u.TaughtCourses)
            .SingleOrDefaultAsync(s => s.Id == id);

    public async Task<User?> GetUserByIdAsync(Guid id) => await context.Users.SingleOrDefaultAsync(s => s.Id == id);

    public async Task UpdateProfileAsync(User user, User userUpdate)
    {
        user.FirstName = userUpdate.FirstName;
        user.LastName = userUpdate.LastName;
        user.Email = userUpdate.Email;
        user.Phone = userUpdate.Phone;
        await context.SaveChangesAsync();
    }

    public async Task UpdateRoleAsync(User user, Role role)
    {
        user.Role = role;
        await context.SaveChangesAsync();
    }

    public async Task UpdatePasswordAsync(User user, string password)
    {
        user.Password = password;
        await context.SaveChangesAsync();
    }

    public async Task DeactivateUserAsync(User user)
    {
        user.IsDeactivated = true;
        await context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(User user)
    {
        context.Users.Remove(user);
        await context.SaveChangesAsync();
    }
}
