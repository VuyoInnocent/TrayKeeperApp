namespace TrayKeeper.BL.Interfaces
{
    public interface ILogInService 
    {
        Task<bool> Login(string userName, string password);
    }
}
