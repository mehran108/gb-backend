using GoldBank.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using static GoldBank.Models.User;

namespace GoldBank.Application
{
    public interface IAccountApplication
    {
        Task<bool> ForgotPassword(string email);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserById(int UserId);
        Task<List<User>> GetUserList();
        Task<int> RegisterUser(User User);
        Task<bool> UpdateUser(User User);
        Task<bool> PasswordReset(User User);
        Task<bool> ChangePassword(User User);
        Task<bool> ActiveNonActive(User User);
        Task<Request<User>> GetUserPagination(Request<User> request);
        Task<List<User>> Userearching(string Target);
        Task<Request<User>> Userorting(Request<User> request);
    }
}
