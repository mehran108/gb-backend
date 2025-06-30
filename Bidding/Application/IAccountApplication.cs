using Bidding.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Bidding.Models.Users;

namespace Bidding.Application
{
    public interface IAccountApplication
    {
        Task<bool> ForgotPassword(string email);
        Task<Users> GetUserByEmail(string email);
        Task<Users> GetUserById(int UserId);
        Task<List<Users>> GetUsersList();
        Task<int> RegisterUser(Users User);
        Task<bool> UpdateUser(Users User);
        Task<bool> PasswordReset(Users users);
        Task<bool> ChangePassword(Users users);
        Task<bool> ActiveNonActive(Users users);
        Task<Request<Users>> GetUserPagination(Request<Users> request);
        Task<List<Users>> UserSearching(string Target);
        Task<Request<Users>> UserSorting(Request<Users> request);
    }
}
