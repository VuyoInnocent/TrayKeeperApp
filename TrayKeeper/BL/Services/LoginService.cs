using TrayKeeper.BL.Interfaces;
using TrayKeeper.DL.Interfaces;
using TrayKeeper.Models;

namespace TrayKeeper.BL
{
    public class LoginService : ILogInService
    {
        private readonly ILogInRepository _logInRepository;

        public LoginService(ILogInRepository logInRepository)
        {
            _logInRepository = logInRepository;
        }

        public async Task<bool> Login(string userName, string password)
        {

            //var insert = await _logInRepository.InsertAsync(new User { Username = userName, PasswordHash = password });

            var allUser = await _logInRepository.GetAllAsync();
            var user = allUser.Where(i => i.Username == userName && i.PasswordHash == password).FirstOrDefault();

            if (user != null)
            {
                return true;
            }

            return false;
        }

    }
}

