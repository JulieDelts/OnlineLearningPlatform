using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Core;
using OnlineLearningPlatform.DAL.DTOs;
using OnlineLearningPlatform.DAL.Interfaces;

namespace OnlineLearningPlatform.DAL
{
    public class UsersRepository : IUsersRepository
    {
        private readonly OnlineLearningPlatformContext _context;

        public UsersRepository(OnlineLearningPlatformContext context)
        {
            _context = context;
        }

        public async Task<Guid> Register(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return user.Id;
        }

        public async Task<User?> GetUserByLogin(string login)
        {
            return await _context.User.Where(u => u.Login == login).SingleOrDefaultAsync();
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _context.User.Where(u => u.IsDeactivated == false).ToListAsync();
        }

        public async Task<User> GetUserByIdWithFullInfo(Guid id)
        {
            var user = await _context.User.Where(s => s.Id == id).Include(u => u.Enrollments).ThenInclude(en => en.Course).Include(u => u.TaughtCourses).FirstOrDefaultAsync();

            if (user != null)
            {
                return user;
            }
            else
            {
                throw new ArgumentException("The entity is not found.");
            }
        }

        public async Task UpdateProfile(Guid id, User user)
        {
            var userToUpdate = await GetUserById(id);

            userToUpdate.FirstName = user.FirstName;
            userToUpdate.LastName = user.LastName;
            userToUpdate.Email = user.Email;
            userToUpdate.Phone = user.Phone;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRole(Guid id, Role role)
        {
            var userToUpdate = await GetUserById(id);

            userToUpdate.Role = role;
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePassword(Guid id, string password)
        {
            var userToUpdate = await GetUserById(id);

            userToUpdate.Password = password;
            await _context.SaveChangesAsync();
        }

        public async Task DeactivateUser(Guid id)
        {
            var userToUpdate = await GetUserById(id);

            userToUpdate.IsDeactivated = true;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUser(Guid id)
        {
            var userToDelete = await GetUserById(id);

            _context.User.Remove(userToDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserById(Guid id)
        {
            var user = await _context.User.Where(s => s.Id == id).FirstOrDefaultAsync();

            if (user != null)
            {
                return user;
            }
            else
            {
                throw new ArgumentException("The entity is not found.");
            }
        }
    }
}
