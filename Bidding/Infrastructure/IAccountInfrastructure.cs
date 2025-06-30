using Bidding.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Bidding.Models.Users;

namespace Bidding.Infrastructure
{
    public interface IAccountInfrastructure
    {
        Task<Users> GetUserByEmail(string email);
        Task<bool> PasswordReset(Users users);
        Task<Users> GetUserById(int UserId);
        Task<List<Users>> GetUsersList();
        Task<int> RegisterUser(Users User);
        Task<bool> UpdateUser(Users User);
        Task<bool> ChangePassword(Users User);
        Task<bool> ActiveNonActive(Users User);
        Task<Request<Users>> GetUserPagination(Request<Users> request);

        Task<List<Users>> UserSearching(string Target);

        Task<Request<Users>> UserSorting(Request<Users> request);

        //Task<List<Users>> GetUsersListPagination(Users users);
        //Task<GetAll<Users>> getAllList(GetAll<Users> request);

    }
}
