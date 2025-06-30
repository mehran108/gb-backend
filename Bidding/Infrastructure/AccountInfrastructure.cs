using GoldBank.Infrastructure.Extension;
using GoldBank.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using static GoldBank.Models.User;

namespace GoldBank.Infrastructure
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
        private const string ColumnParameterName = "@PColumn";

        #endregion


        #region methods





        public async Task<User> GetUserByEmail(string email)
        {
            User user = new User();

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
                        user = new User
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

        public async Task<User> GetUserById(int UserId)
        {
            User user = new User();

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
                        user = new User
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

        public async Task<List<User>> GetUserList()
        {
            List<User> userList = new List<User>();
            User user = new User();

            var parameters = new List<DbParameter>
            {
                // base.GetParameter(AccountInfrastructure.PageNoParameterName, )
            };

            using (var dataReader = await base.ExecuteReader(parameters, AccountInfrastructure.GetUserListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        user = new User
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

        public async Task<int> RegisterUser(User User)
        {
            var applicationUserIdParamter = base.GetParameterOut(AccountInfrastructure.UserIdParameterName, SqlDbType.Int, User.UserId);

            var parameters = new List<DbParameter>
            {
                applicationUserIdParamter,
                base.GetParameter(AccountInfrastructure.EmailParameterName, User.Email),
                base.GetParameter(AccountInfrastructure.FirstNameParameterName, User.FirstName),
                base.GetParameter(AccountInfrastructure.LastNameParameterName, User.LastName),
                base.GetParameter(AccountInfrastructure.PasswordHashParameterName, User.PasswordHash),
                base.GetParameter(AccountInfrastructure.IdentificationNumberParameterName, User.IdentificationNumber),
                //base.GetParameter(AccountInfrastructure.PhoneNumberParameterName, User.PhoneNumber),
                base.GetParameter(AccountInfrastructure.Address1ParameterName, User.Address1),
                //base.GetParameter(AccountInfrastructure.PostalCodeParameterName, User.PostalCode)
            };
            await base.ExecuteNonQuery(parameters, AccountInfrastructure.RegisterUsertoredProcedureName, CommandType.StoredProcedure);
            User.UserId = Convert.ToInt32(applicationUserIdParamter.Value);

            return User.UserId;

        }

        public async Task<bool> UpdateUser(User User)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(AccountInfrastructure.UserIdParameterName, User.UserId),
                base.GetParameter(AccountInfrastructure.EmailParameterName, User.Email),
                base.GetParameter(AccountInfrastructure.FirstNameParameterName, User.FirstName),
                base.GetParameter(AccountInfrastructure.LastNameParameterName, User.LastName)
            };

            var result = await base.ExecuteNonQuery(parameters, AccountInfrastructure.UpdateUsertoredProcedureName, CommandType.StoredProcedure);


            return true;
        }

        public async Task<bool> PasswordReset(User User)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(AccountInfrastructure.UserIdParameterName, User.UserId),
                base.GetParameter(AccountInfrastructure.PasswordHashParameterName, User.PasswordHash),
            };

            var result = await base.ExecuteNonQuery(parameters, AccountInfrastructure.PasswordResetStoredProcedureName, CommandType.StoredProcedure);


            return true;
        }
        public async Task<bool> ChangePassword(User User)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(AccountInfrastructure.UserIdParameterName, User.UserId),
                base.GetParameter(AccountInfrastructure.PasswordHashParameterName, User.PasswordHash),
            };

            var result = await base.ExecuteNonQuery(parameters, AccountInfrastructure.PasswordResetStoredProcedureName, CommandType.StoredProcedure);


            return true;
        }

        public async Task<bool> ActiveNonActive(User User)
        {
            var parameters = new List<DbParameter>
            {
                 base.GetParameter(AccountInfrastructure.UserIdParameterName, User.UserId),
                base.GetParameter(AccountInfrastructure.ActiveCodeParameterName, User.Active),

            };

            var result = await base.ExecuteNonQuery(parameters, AccountInfrastructure.ActiveNonActivStoredProcedureName, CommandType.StoredProcedure);


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

            using (var dataReader = await base.ExecuteReader(parameters, AccountInfrastructure.GetUserListStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        user = new User
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



        public async Task<Request<User>> GetUserPagination(Request<User> request)

        {
            Request<User> result = new Request<User>();
            result.ItemList = new List<User>();

            result.TotalCount = 0;
            var TotalCountParamter = base.GetParameterOut(AccountInfrastructure.TotalCountParameterName, SqlDbType.Int, request.TotalCount);
            var parameters = new List<DbParameter>
            {
                TotalCountParamter,
                base.GetParameter(AccountInfrastructure.ColumnParameterName, request.Column),
                base.GetParameter(AccountInfrastructure.TargetTextParameterName, request.Target),
                base.GetParameter(AccountInfrastructure.LimitParameterName, request.Limit),
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

                        var user = new User
                        {
                            UserId = dataReader.GetIntegerValue(AccountInfrastructure.UserIdColumnName),
                            Email = dataReader.GetStringValue(AccountInfrastructure.EmailColumnName),
                            FirstName = dataReader.GetStringValue(AccountInfrastructure.FirstNameColumnName),
                            LastName = dataReader.GetStringValue(AccountInfrastructure.LastNameColumnName),
                            PasswordHash = dataReader.GetStringValue(AccountInfrastructure.PasswordHashColumnName),
                            Active = dataReader.GetBooleanValue(AccountInfrastructure.ActiveColumnName)
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
                base.GetParameter(AccountInfrastructure.TargetTextParameterName,Target)
            };

            using (var dataReader = await base.ExecuteReader(parameters, AccountInfrastructure.GetUserBySearchingStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        user = new User
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
        public async Task<Request<User>> Userorting(Request<User> request)

        {
            Request<User> result = new Request<User>();
            result.ItemList = new List<User>();

            result.TotalCount = 0;
            var TotalCountParamter = base.GetParameterOut(AccountInfrastructure.TotalCountParameterName, SqlDbType.Int, request.TotalCount);
            var parameters = new List<DbParameter>
            {
                TotalCountParamter,
                base.GetParameter(AccountInfrastructure.LimitParameterName, request.Limit),
                base.GetParameter(AccountInfrastructure.OrderParameterName, request.Order),
                base.GetParameter(AccountInfrastructure.ColumnParameterName, request.Column),
                base.GetParameter(AccountInfrastructure.PageNoParameterName, request.PageNo)
            };
            using (var dataReader = await base.ExecuteReader(parameters, AccountInfrastructure.UserortingStoredProcedureName, CommandType.StoredProcedure))

            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {

                        var user = new User
                        {
                            UserId = dataReader.GetIntegerValue(AccountInfrastructure.UserIdColumnName),
                            Email = dataReader.GetStringValue(AccountInfrastructure.EmailColumnName),
                            FirstName = dataReader.GetStringValue(AccountInfrastructure.FirstNameColumnName),
                            LastName = dataReader.GetStringValue(AccountInfrastructure.LastNameColumnName),
                            Active = dataReader.GetBooleanValue(AccountInfrastructure.ActiveColumnName)
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