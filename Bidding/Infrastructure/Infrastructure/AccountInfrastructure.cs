using GoldBank.Infrastructure.Extension;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using static GoldBank.Models.User;

namespace GoldBank.Infrastructure.Infrastructure
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
        private const string GetUserListStoredProcedureName = "GetUserList";
        private const string RegisterUsertoredProcedureName = "RegisterUser";
        private const string UpdateUsertoredProcedureName = "UpdateUser";
        private const string PasswordResetStoredProcedureName = "PasswordReset";
        private const string ActiveNonActivStoredProcedureName = "UpdateActive";
        private const string GetUserpaginationStoredProcedureName = "GetUserpagination";
        private const string GetUserBySearchingStoredProcedureName = "GetUserBySearching";

        private const string UserortingStoredProcedureName = "SortingUser";


        //Column names
        private const string UserIdColumnName = "UserId";
        private const string EmailColumnName = "Email";
        private const string FirstNameColumnName = "FirstName";
        private const string NameColumnName = "Name";
        private const string LastNameColumnName = "LastName";
        private const string PasswordColumnName = "Password";
        private const string PasswordHashColumnName = "Password";
        private const string IdentificationNumberColumnName = "IdentificationNumber";
        private const string Address1NumberColumnName = "Address1";
        private const string PostalCodeNumberColumnName = "PostalCode";





        //Parameter names
        private const string UserIdParameterName = "PUserId";
        private const string NameParameterName = "@PName";
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
        private const string ColumnParameterName = "@PColumn";

        #endregion


        #region methods





        public async Task<User> GetUserByEmail(string email)
        {
            User user = new User();

            var parameters = new List<DbParameter>
            {
                GetParameter(EmailParameterName, email)
            };

            using (var dataReader = await ExecuteReader(parameters, GetUserByEmailStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                        user = new User
                        {
                            UserId = dataReader.GetIntegerValue(UserIdColumnName),
                            Email = dataReader.GetStringValue(EmailColumnName),
                            //FirstName = dataReader.GetStringValue(FirstNameColumnName),
                            Name = dataReader.GetStringValue(NameColumnName),
                            //LastName = dataReader.GetStringValue(LastNameColumnName),
                            PasswordHash = dataReader.GetStringValue(PasswordHashColumnName),
                            Active = dataReader.GetBooleanValue(ActiveColumnName)
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

        public async Task<User> GetUserById(int UserId)
        {
            User user = new User();

            var parameters = new List<DbParameter>
            {
                GetParameter(UserIdParameterName, UserId)
            };

            using (var dataReader = await ExecuteReader(parameters, GetUserByIdStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                        user = new User
                        {
                            UserId = dataReader.GetIntegerValue(UserIdColumnName),
                            Email = dataReader.GetStringValue(EmailColumnName),
                            FirstName = dataReader.GetStringValue(FirstNameColumnName),
                            LastName = dataReader.GetStringValue(LastNameColumnName),
                            PasswordHash = dataReader.GetStringValue(PasswordHashColumnName),
                            Active = dataReader.GetBooleanValue(ActiveColumnName)

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

        public async Task<List<User>> GetUserList()
        {
            List<User> userList = new List<User>();
            User user = new User();

            var parameters = new List<DbParameter>
            {
                // base.GetParameter(AccountInfrastructure.PageNoParameterName, )
            };

            using (var dataReader = await ExecuteReader(parameters, GetUserListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        user = new User
                        {
                            UserId = dataReader.GetIntegerValue(UserIdColumnName),
                            Email = dataReader.GetStringValue(EmailColumnName),
                            FirstName = dataReader.GetStringValue(FirstNameColumnName),
                            LastName = dataReader.GetStringValue(LastNameColumnName),
                            PasswordHash = dataReader.GetStringValue(PasswordHashColumnName),
                            Active = dataReader.GetBooleanValue(ActiveColumnName)
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

        public async Task<int> RegisterUser(User User)
        {
            var applicationUserIdParamter = GetParameterOut(UserIdParameterName, SqlDbType.Int, User.UserId);

            var parameters = new List<DbParameter>
            {
                applicationUserIdParamter,
                GetParameter(EmailParameterName, User.Email),
                GetParameter(NameParameterName, User.FirstName),
                GetParameter(PasswordHashParameterName, User.PasswordHash)
            };
            await ExecuteNonQuery(parameters, RegisterUsertoredProcedureName, CommandType.StoredProcedure);
            User.UserId = Convert.ToInt32(applicationUserIdParamter.Value);

            return User.UserId;

        }

        public async Task<bool> UpdateUser(User User)
        {
            var parameters = new List<DbParameter>
            {
                GetParameter(UserIdParameterName, User.UserId),
                GetParameter(EmailParameterName, User.Email),
                GetParameter(FirstNameParameterName, User.FirstName),
                GetParameter(LastNameParameterName, User.LastName)
            };

            var result = await ExecuteNonQuery(parameters, UpdateUsertoredProcedureName, CommandType.StoredProcedure);


            return true;
        }

        public async Task<bool> PasswordReset(User User)
        {
            var parameters = new List<DbParameter>
            {
                GetParameter(UserIdParameterName, User.UserId),
                GetParameter(PasswordHashParameterName, User.PasswordHash),
            };

            var result = await ExecuteNonQuery(parameters, PasswordResetStoredProcedureName, CommandType.StoredProcedure);


            return true;
        }
        public async Task<bool> ChangePassword(User User)
        {
            var parameters = new List<DbParameter>
            {
                GetParameter(UserIdParameterName, User.UserId),
                GetParameter(PasswordHashParameterName, User.PasswordHash),
            };

            var result = await ExecuteNonQuery(parameters, PasswordResetStoredProcedureName, CommandType.StoredProcedure);


            return true;
        }

        public async Task<bool> ActiveNonActive(User User)
        {
            var parameters = new List<DbParameter>
            {
                 GetParameter(UserIdParameterName, User.UserId),
                GetParameter(ActiveCodeParameterName, User.Active),

            };

            var result = await ExecuteNonQuery(parameters, ActiveNonActivStoredProcedureName, CommandType.StoredProcedure);


            return true;
        }
        public async Task<List<User>> GetUserListPagination()
        {
            List<User> userList = new List<User>();
            User user = new User();

            var parameters = new List<DbParameter>
            {
                //base.GetParameter(AccountInfrastructure.UserIdParameterName, UserId)
            };

            using (var dataReader = await ExecuteReader(parameters, GetUserListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        user = new User
                        {
                            UserId = dataReader.GetIntegerValue(UserIdColumnName),
                            Email = dataReader.GetStringValue(EmailColumnName),
                            FirstName = dataReader.GetStringValue(FirstNameColumnName),
                            LastName = dataReader.GetStringValue(LastNameColumnName),
                            PasswordHash = dataReader.GetStringValue(PasswordHashColumnName)

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



        public async Task<Request<User>> GetUserPagination(Request<User> request)

        {
            Request<User> result = new Request<User>();
            result.ItemList = new List<User>();

            result.TotalCount = 0;
            var TotalCountParamter = GetParameterOut(TotalCountParameterName, SqlDbType.Int, request.TotalCount);
            var parameters = new List<DbParameter>
            {
                TotalCountParamter,
                GetParameter(ColumnParameterName, request.Column),
                GetParameter(TargetTextParameterName, request.Target),
                GetParameter(LimitParameterName, request.Limit),
                GetParameter(PageNoParameterName, request.PageNo),
                GetParameter(OrderParameterName, request.Order),
                // base.GetParameter(ProspectInfrasutructure.TotalCountParameterName,SqlDbType.Int, request.TotalCount),
              
            };
            using (var dataReader = await ExecuteReader(parameters, GetUserpaginationStoredProcedureName, CommandType.StoredProcedure))

            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {

                        var user = new User
                        {
                            UserId = dataReader.GetIntegerValue(UserIdColumnName),
                            Email = dataReader.GetStringValue(EmailColumnName),
                            FirstName = dataReader.GetStringValue(FirstNameColumnName),
                            LastName = dataReader.GetStringValue(LastNameColumnName),
                            PasswordHash = dataReader.GetStringValue(PasswordHashColumnName),
                            Active = dataReader.GetBooleanValue(ActiveColumnName)
                        };


                        result.ItemList.Add(user);

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

        public async Task<List<User>> Userearching(string Target)
        {
            List<User> UserList = new List<User>();
            User user = new User();

            var parameters = new List<DbParameter>
            {
                GetParameter(TargetTextParameterName,Target)
            };

            using (var dataReader = await ExecuteReader(parameters, GetUserBySearchingStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        user = new User
                        {
                            UserId = dataReader.GetIntegerValue(UserIdColumnName),
                            Email = dataReader.GetStringValue(EmailColumnName),
                            FirstName = dataReader.GetStringValue(FirstNameColumnName),
                            LastName = dataReader.GetStringValue(LastNameColumnName),
                            Active = dataReader.GetBooleanValue(ActiveColumnName)
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
        public async Task<Request<User>> Userorting(Request<User> request)

        {
            Request<User> result = new Request<User>();
            result.ItemList = new List<User>();

            result.TotalCount = 0;
            var TotalCountParamter = GetParameterOut(TotalCountParameterName, SqlDbType.Int, request.TotalCount);
            var parameters = new List<DbParameter>
            {
                TotalCountParamter,
                GetParameter(LimitParameterName, request.Limit),
                GetParameter(OrderParameterName, request.Order),
                GetParameter(ColumnParameterName, request.Column),
                GetParameter(PageNoParameterName, request.PageNo)
            };
            using (var dataReader = await ExecuteReader(parameters, UserortingStoredProcedureName, CommandType.StoredProcedure))

            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {

                        var user = new User
                        {
                            UserId = dataReader.GetIntegerValue(UserIdColumnName),
                            Email = dataReader.GetStringValue(EmailColumnName),
                            FirstName = dataReader.GetStringValue(FirstNameColumnName),
                            LastName = dataReader.GetStringValue(LastNameColumnName),
                            Active = dataReader.GetBooleanValue(ActiveColumnName)
                        };


                        result.ItemList.Add(user);

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