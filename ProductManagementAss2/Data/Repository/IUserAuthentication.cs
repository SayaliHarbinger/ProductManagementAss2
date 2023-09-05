using ProductManagementAss2.Models.DTO;

namespace ProductManagementAss2.Data.Repository
{
    public interface IUserAuthentication
    {
        Task<Status> LoginAsync(LoginModel model);
        Task LogoutAsync();
        Task<Status> RegisterAsync(RegistrationModel model);
    }
}
