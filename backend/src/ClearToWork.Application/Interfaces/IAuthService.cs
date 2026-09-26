using System.Threading.Tasks;
using ClearToWork.Application.DTOs;

namespace ClearToWork.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
}
