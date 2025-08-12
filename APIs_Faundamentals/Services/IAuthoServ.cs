using APIs_Faundamentals.DTO;
using APIs_Faundamentals.Models;

namespace APIs_Faundamentals.Services
{
    public interface IAuthoServ
    {
        Task<AuthModel> RegisterAsync(UserDataDTO userData);

        Task<AuthModel> GetTokenAsync(TokenReqModel tokenReq);

        Task<string> AssignRoleAsync(RoleModel roleModel);
     }
}
