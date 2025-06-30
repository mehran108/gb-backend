using GoldBank.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using static GoldBank.Models.User;

namespace GoldBank.Infrastructure
{
    public interface IAccountInfrastructure
    {
        Task<User> GetUserByEmail(string email);
        Task<bool> PasswordReset(User User);
        Task<User> GetUserById(int UserId);
        Task<List<User>> GetUserList();
        Task<int> RegisterUser(User User);
        Task<bool> UpdateUser(User User);
        Task<bool> ChangePassword(User User);
        Task<bool> ActiveNonActive(User User);
        Task<Request<User>> GetUserPagination(Request<User> request);

        Task<List<User>> Userearching(string Target);

        Task<Request<User>> Userorting(Request<User> request);

        //Task<List<User>> GetUserListPagination(User User);
        //Task<GetAll<User>> getAllList(GetAll<User> request);

    }
}
