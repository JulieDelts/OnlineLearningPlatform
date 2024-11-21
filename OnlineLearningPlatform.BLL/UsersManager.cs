using OnlineLearningPlatform.DAL;
using OnlineLearningPlatform.DAL.DTOs;

namespace OnlineLearningPlatform.BLL
{
    public class UsersManager : IUsersManager
    {
        private IUsersRepository _repository;

        public UsersManager()
        {
            _repository = new UsersRepository();
        }

        public async Task<User?> CheckCredentials(string login, string password)
        {
            return await _repository.CheckCredentials(login, password);
        }
    }
}
