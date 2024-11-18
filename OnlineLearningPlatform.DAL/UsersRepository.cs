using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Core.DTOs;

namespace OnlineLearningPlatform.DAL
{
    public class UsersRepository
    {
        private OnlineLearningPlatformContext _context;

        public UsersRepository()
        {
            _context = new OnlineLearningPlatformContext();
        }

        public async Task<Guid> Register(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return user.Id;
        }

        public async Task<User?> CheckCredentials(string login, string password)
        {
            var user = await _context.User.Where(u => u.Login == login && u.Password == password).FirstOrDefaultAsync();

            return user;
        }

        public async Task<List<User>> GetAll()
        {
            var users = await _context.User.Where(u => u.IsDeactivated == false).ToListAsync();

            if (users != null)
            {
                return users;
            }
            else
            {
                return new List<User>();
            }
        }

        public async Task<User> GetById(Guid id)
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
            var userToUpdate = await _context.User.Where(s => s.Id == id).FirstOrDefaultAsync();

            if (userToUpdate != null)
            {
                userToUpdate.FirstName = user.FirstName;
                userToUpdate.LastName = user.LastName;
                userToUpdate.Email = user.Email;
                userToUpdate.Phone = user.Phone;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("The entity is not found.");
            }
        }

        public async Task UpdateRole(Guid id, User user)
        {
            var userToUpdate = await _context.User.Where(s => s.Id == id).FirstOrDefaultAsync();

            if (userToUpdate != null)
            {
                userToUpdate.Role = user.Role;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("The entity is not found.");
            }
        }

        public async Task UpdatePassword(Guid id, User user)
        {
            var userToUpdate = await _context.User.Where(s => s.Id == id).FirstOrDefaultAsync();

            if (userToUpdate != null)
            {
                userToUpdate.Password = user.Password;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("The entity is not found.");
            }
        }

        public async Task Deactivate(Guid id)
        {
            var userToUpdate = await _context.User.Where(s => s.Id == id).FirstOrDefaultAsync();

            if (userToUpdate != null)
            {
                userToUpdate.IsDeactivated = true;
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("The entity is not found.");
            }
        }

        public async Task Delete(Guid id)
        {
            var userToDelete = await _context.User.Where(s => s.Id == id).FirstOrDefaultAsync();

            if (userToDelete != null)
            {
                _context.User.Remove(userToDelete);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("The entity is not found.");
            }
        }
    }
}
