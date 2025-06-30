using Bidding.Infrastructure.Extension;
using Bidding.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using static Bidding.Models.Users;

namespace Bidding.Infrastructure
{
    public class AccountInfrastructure : BaseInfrastructure, IAccountInfrastructure
    {
        public AccountInfrastructure(IConfiguration configuration) : base(configuration)
        {
        }

        #region Constants
        //Stored procedure names

        private const string GetUserByEmailStoredProcedureName = "GetUserByEmail";
        private const string GetUserByIdStoredProcedureName = "GetUserById";
        private const string GetUsersListStoredProcedureName = "GetUsersList";
        private const string RegisterUserStoredProcedureName = "RegisterUser";
        private const string UpdateUserStoredProcedureName = "UpdateUser";
        private const string PasswordResetStoredProcedureName = "PasswordReset";
        private const string ActiveNonActivStoredProcedureName = "UpdateActive";
        private const string GetUserpaginationStoredProcedureName = "GetUserpagination";
        private const string GetUserBySearchingStoredProcedureName = "GetUserBySearching";

        private const string UserSortingStoredProcedureName = "SortingUser";


        //Column names
        private const string UserIdColumnName = "UserId";
        private const string EmailColumnName = "Email";
        private const string FirstNameColumnName = "FirstName";
        private const string LastNameColumnName = "LastName";
        private const string PasswordColumnName = "Password";
        private const string PasswordHashColumnName = "PasswordHash";
        private const string IdentificationNumberColumnName = "IdentificationNumber";
        private const string Address1NumberColumnName = "Address1";
        private const string PostalCodeNumberColumnName = "PostalCode";
        




        //Parameter names
        private const string UserIdParameterName = "PUserId";
        private const string EmailParameterName = "PEmail";
        private const string PasswordParameterName = "PPassword";
        private const string FirstNameParameterName = "PFirstName";
        private const string LastNameParameterName = "PLastName";
        private const string PasswordHashParameterName = "PPasswordHash";
        private const string IdentificationNumberParameterName = "PIdentificationNumber";
        private const string PhoneNumberParameterName = "PPhoneNumber";
        private const string Address1ParameterName = "PAddress1";
        private const string PostalCodeParameterName = "PPostalCode";
        private const string ActiveCodeParameterName = "PActive";
        private const string LimitParameterName = "PLimit";
        private const string PageNoParameterName = "PPageNo";
        private const string TotalCountParameterName = "@PTotalCount";
        private const string TargetTextParameterName = "PTarget";
        private const string OrderParameterName = "@POrder";
        private const string ColomnParameterName = "@PColomn";

        #endregion


        #region methods





        public async Task<Users> GetUserByEmail(string email)
        {
            Users user = new Users();

            var parameters = new List<DbParameter>
            {
                base.GetParameter(AccountInfrastructure.EmailParameterName, email)
            };

            using (var dataReader = await base.ExecuteReader(parameters, AccountInfrastructure.GetUserByEmailStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                        user = new Users
                        {
                            UserId = dataReader.GetIntegerValue(AccountInfrastructure.UserIdColumnName),
                            Email = dataReader.GetStringValue(AccountInfrastructure.EmailColumnName),
                            FirstName = dataReader.GetStringValue(AccountInfrastructure.FirstNameColumnName),
                            LastName = dataReader.GetStringValue(AccountInfrastructure.LastNameColumnName),
                            PasswordHash = dataReader.GetStringValue(AccountInfrastructure.PasswordHashColumnName),
                            Active = dataReader.GetBooleanValue(AccountInfrastructure.ActiveColumnName)
                        };
                    }
                    if (!dataReader.IsClosed)
                    {
                        dataReader.Close();
                    }

                }
            }


            return user;
        }

        public async Task<Users> GetUserById(int UserId)
        {
            Users user = new Users();

            var parameters = new List<DbParameter>
            {
                base.GetParameter(AccountInfrastructure.UserIdParameterName, UserId)
            };

            using (var dataReader = await base.ExecuteReader(parameters, AccountInfrastructure.GetUserByIdStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                        user = new Users
                        {
                            UserId = dataReader.GetIntegerValue(AccountInfrastructure.UserIdColumnName),
                            Email = dataReader.GetStringValue(AccountInfrastructure.EmailColumnName),
                            FirstName = dataReader.GetStringValue(AccountInfrastructure.FirstNameColumnName),
                            LastName = dataReader.GetStringValue(AccountInfrastructure.LastNameColumnName),
                            PasswordHash = dataReader.GetStringValue(AccountInfrastructure.PasswordHashColumnName),
                            Active = dataReader.GetBooleanValue(AccountInfrastructure.ActiveColumnName)

                        };
                    }
                    if (!dataReader.IsClosed)
                    {
                        dataReader.Close();
                    }

                }
            }


            return user;
        }

        public async Task<List<Users>> GetUsersList()
        {
            List<Users> userList = new List<Users>();
            Users user = new Users();

            var parameters = new List<DbParameter>
            {
                // base.GetParameter(AccountInfrastructure.PageNoParameterName, )
            };

            using (var dataReader = await base.ExecuteReader(parameters, AccountInfrastructure.GetUsersListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        user = new Users
                        {
                            UserId = dataReader.GetIntegerValue(AccountInfrastructure.UserIdColumnName),
                            Email = dataReader.GetStringValue(AccountInfrastructure.EmailColumnName),
                            FirstName = dataReader.GetStringValue(AccountInfrastructure.FirstNameColumnName),
                            LastName = dataReader.GetStringValue(AccountInfrastructure.LastNameColumnName),
                            PasswordHash = dataReader.GetStringValue(AccountInfrastructure.PasswordHashColumnName),
                            Active = dataReader.GetBooleanValue(AccountInfrastructure.ActiveColumnName)
                        };

                        userList.Add(user);

                    }
                    if (!dataReader.IsClosed)
                    {
                        dataReader.Close();
                    }

                }
            }


            return userList;
        }

        public async Task<int> RegisterUser(Users users)
        {
            var applicationUserIdParamter = base.GetParameterOut(AccountInfrastructure.UserIdParameterName, SqlDbType.Int, users.UserId);

            var parameters = new List<DbParameter>
            {
                applicationUserIdParamter,
                base.GetParameter(AccountInfrastructure.EmailParameterName, users.Email),
                base.GetParameter(AccountInfrastructure.FirstNameParameterName, users.FirstName),
                base.GetParameter(AccountInfrastructure.LastNameParameterName, users.LastName),
                base.GetParameter(AccountInfrastructure.PasswordHashParameterName, users.PasswordHash),
                base.GetParameter(AccountInfrastructure.IdentificationNumberParameterName, users.IdentificationNumber),
                //base.GetParameter(AccountInfrastructure.PhoneNumberParameterName, users.PhoneNumber),
                base.GetParameter(AccountInfrastructure.Address1ParameterName, users.Address1),
                //base.GetParameter(AccountInfrastructure.PostalCodeParameterName, users.PostalCode)
            };
            await base.ExecuteNonQuery(parameters, AccountInfrastructure.RegisterUserStoredProcedureName, CommandType.StoredProcedure);
            users.UserId = Convert.ToInt32(applicationUserIdParamter.Value);

            return users.UserId;

        }

        public async Task<bool> UpdateUser(Users users)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(AccountInfrastructure.UserIdParameterName, users.UserId),
                base.GetParameter(AccountInfrastructure.EmailParameterName, users.Email),
                base.GetParameter(AccountInfrastructure.FirstNameParameterName, users.FirstName),
                base.GetParameter(AccountInfrastructure.LastNameParameterName, users.LastName)
            };

            var result = await base.ExecuteNonQuery(parameters, AccountInfrastructure.UpdateUserStoredProcedureName, CommandType.StoredProcedure);


            return true;
        }

        public async Task<bool> PasswordReset(Users users)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(AccountInfrastructure.UserIdParameterName, users.UserId),
                base.GetParameter(AccountInfrastructure.PasswordHashParameterName, users.PasswordHash),
            };

            var result = await base.ExecuteNonQuery(parameters, AccountInfrastructure.PasswordResetStoredProcedureName, CommandType.StoredProcedure);


            return true;
        }
        public async Task<bool> ChangePassword(Users users)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(AccountInfrastructure.UserIdParameterName, users.UserId),
                base.GetParameter(AccountInfrastructure.PasswordHashParameterName, users.PasswordHash),
            };

            var result = await base.ExecuteNonQuery(parameters, AccountInfrastructure.PasswordResetStoredProcedureName, CommandType.StoredProcedure);


            return true;
        }

        public async Task<bool> ActiveNonActive(Users users)
        {
            var parameters = new List<DbParameter>
            {
                 base.GetParameter(AccountInfrastructure.UserIdParameterName, users.UserId),
                base.GetParameter(AccountInfrastructure.ActiveCodeParameterName, users.Active),

            };

            var result = await base.ExecuteNonQuery(parameters, AccountInfrastructure.ActiveNonActivStoredProcedureName, CommandType.StoredProcedure);


            return true;
        }
        public async Task<List<Users>> GetUsersListPagination()
        {
            List<Users> userList = new List<Users>();
            Users user = new Users();

            var parameters = new List<DbParameter>
            {
                //base.GetParameter(AccountInfrastructure.UserIdParameterName, UserId)
            };

            using (var dataReader = await base.ExecuteReader(parameters, AccountInfrastructure.GetUsersListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        user = new Users
                        {
                            UserId = dataReader.GetIntegerValue(AccountInfrastructure.UserIdColumnName),
                            Email = dataReader.GetStringValue(AccountInfrastructure.EmailColumnName),
                            FirstName = dataReader.GetStringValue(AccountInfrastructure.FirstNameColumnName),
                            LastName = dataReader.GetStringValue(AccountInfrastructure.LastNameColumnName),
                            PasswordHash = dataReader.GetStringValue(AccountInfrastructure.PasswordHashColumnName)

                        };

                        userList.Add(user);

                    }
                    if (!dataReader.IsClosed)
                    {
                        dataReader.Close();
                    }

                }
            }


            return userList;
        }



        public async Task<Request<Users>> GetUserPagination(Request<Users> request)

        {
            Request<Users> result = new Request<Users>();
            result.itemList = new List<Users>();

            result.TotalCount = 0;
            var TotalCountParamter = base.GetParameterOut(AccountInfrastructure.TotalCountParameterName, SqlDbType.Int, request.TotalCount);
            var parameters = new List<DbParameter>
            {
                TotalCountParamter,
                base.GetParameter(AccountInfrastructure.ColomnParameterName, request.Colomn),
                base.GetParameter(AccountInfrastructure.TargetTextParameterName, request.Target),
                base.GetParameter(AccountInfrastructure.LimitParameterName, request.limit),
                base.GetParameter(AccountInfrastructure.PageNoParameterName, request.PageNo),
                base.GetParameter(AccountInfrastructure.OrderParameterName, request.Order),
                // base.GetParameter(ProspectInfrasutructure.TotalCountParameterName,SqlDbType.Int, request.TotalCount),
              
            };
            using (var dataReader = await base.ExecuteReader(parameters, AccountInfrastructure.GetUserpaginationStoredProcedureName, CommandType.StoredProcedure))

            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {

                        var user = new Users
                        {
                            UserId = dataReader.GetIntegerValue(AccountInfrastructure.UserIdColumnName),
                            Email = dataReader.GetStringValue(AccountInfrastructure.EmailColumnName),
                            FirstName = dataReader.GetStringValue(AccountInfrastructure.FirstNameColumnName),
                            LastName = dataReader.GetStringValue(AccountInfrastructure.LastNameColumnName),
                            PasswordHash = dataReader.GetStringValue(AccountInfrastructure.PasswordHashColumnName),
                            Active = dataReader.GetBooleanValue(AccountInfrastructure.ActiveColumnName)
                        };


                        result.itemList.Add(user);

                    }
                    if (!dataReader.IsClosed)
                    {
                        dataReader.Close();
                    }
                    result.TotalCount = Convert.ToInt32(TotalCountParamter.Value);
                }

                return result;

            }

            
        }

        public async Task<List<Users>> UserSearching(string Target)
        {
            List<Users> UserList = new List<Users>();
            Users user = new Users();

            var parameters = new List<DbParameter>
            {
                base.GetParameter(AccountInfrastructure.TargetTextParameterName,Target)
            };

            using (var dataReader = await base.ExecuteReader(parameters, AccountInfrastructure.GetUserBySearchingStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        user = new Users
                        {
                            UserId = dataReader.GetIntegerValue(AccountInfrastructure.UserIdColumnName),
                            Email = dataReader.GetStringValue(AccountInfrastructure.EmailColumnName),
                            FirstName = dataReader.GetStringValue(AccountInfrastructure.FirstNameColumnName),
                            LastName = dataReader.GetStringValue(AccountInfrastructure.LastNameColumnName),
                            Active = dataReader.GetBooleanValue(AccountInfrastructure.ActiveColumnName)
                        };

                        UserList.Add(user);

                    }
                    if (!dataReader.IsClosed)
                    {
                        dataReader.Close();
                    }

                }
            }


            return UserList;
        }
        public async Task<Request<Users>> UserSorting(Request<Users> request)

        {
            Request<Users> result = new Request<Users>();
            result.itemList = new List<Users>();

            result.TotalCount = 0;
            var TotalCountParamter = base.GetParameterOut(AccountInfrastructure.TotalCountParameterName, SqlDbType.Int, request.TotalCount);
            var parameters = new List<DbParameter>
            {
                TotalCountParamter,
                base.GetParameter(AccountInfrastructure.LimitParameterName, request.limit),
                base.GetParameter(AccountInfrastructure.OrderParameterName, request.Order),
                base.GetParameter(AccountInfrastructure.ColomnParameterName, request.Colomn),
                base.GetParameter(AccountInfrastructure.PageNoParameterName, request.PageNo)
            };
            using (var dataReader = await base.ExecuteReader(parameters, AccountInfrastructure.UserSortingStoredProcedureName, CommandType.StoredProcedure))

            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {

                        var user = new Users
                        {
                            UserId = dataReader.GetIntegerValue(AccountInfrastructure.UserIdColumnName),
                            Email = dataReader.GetStringValue(AccountInfrastructure.EmailColumnName),
                            FirstName = dataReader.GetStringValue(AccountInfrastructure.FirstNameColumnName),
                            LastName = dataReader.GetStringValue(AccountInfrastructure.LastNameColumnName),
                            Active = dataReader.GetBooleanValue(AccountInfrastructure.ActiveColumnName)
                        };


                        result.itemList.Add(user);

                    }
                    if (!dataReader.IsClosed)
                    {
                        dataReader.Close();
                    }
                    result.TotalCount = Convert.ToInt32(TotalCountParamter.Value);
                }

                return result;

            }
        }
    }
}
#endregion