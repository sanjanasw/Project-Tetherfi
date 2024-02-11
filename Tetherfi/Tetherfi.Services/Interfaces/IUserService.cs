using Tetherfi.Common.Models;

namespace Tetherfi.Service.Interfaces;

public interface IUserService
{
    Task<User> CreateAsync(Register model);
    Task<User> UpdateUserAsync(string userId, UserUpdate model);
}