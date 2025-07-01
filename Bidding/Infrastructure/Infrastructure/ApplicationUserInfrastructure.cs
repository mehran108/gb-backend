using System.Data.Common;
using System.Data;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Infrastructure.Extension;
using GoldBank.Models;

namespace GoldBank.Infrastructure.Infrastructure
{
    public class ApplicationUserInfrastructure : BaseInfrastructure, IApplicationUserInfrastructure
    {
        public ApplicationUserInfrastructure(IConfiguration configuration) : base(configuration)
        {
        }


        #region Constants
        private const string AddStoredProcedureName = "ApplicationUserAdd";
        private const string UpdateStoredProcedureName = "ApplicationUserUpdate";
        private const string ActivateStoredProcedureName = "ApplicationUserActivate";
        private const string GetStoredProcedureName = "ApplicationUserGetById";
        private const string GetAllStoredProcedureName = "ApplicationUserGetAll";
        private const string ApplicationUserIdColumnName = "ApplicationUserId";
        private const string UsernameColumnName = "Username";
        private const string EmailColumnName = "Email";
        private const string FirstnameColumnName = "Firstname";
        private const string LastnameColumnName = "Lastname";
        private const string PasswordHashColumnName = "PasswordHash";
        private const string CreatedDateColumnName = "CreatedDate";
        private const string ModifiedDateColumnName = "ModifiedDate";
        private const string CreatedByIdColumnName = "CreatedById";
        private const string ModifiedByIdColumnName = "ModifiedById";
        private const string ActiveColumnName = "Active";
        private const string ApplicationUserIdParameterName = "@PApplicationUserId";
        private const string UsernameParameterName = "@PUsername";
        private const string EmailParameterName = "@PEmail";
        private const string FirstnameParameterName = "@PFirstname";
        private const string LastnameParameterName = "@PLastname";
        private const string PasswordHashParameterName = "@PPasswordHash";
        private const string CreatedDateParameterName = "@PCreatedDate";
        private const string ModifiedDateParameterName = "@PModifiedDate";
        private const string CreatedByIdParameterName = "@PCreatedById";
        private const string ModifiedByIdParameterName = "@PModifiedById";
        private const string ActiveParameterName = "@PActive";

        #endregion
        public async Task<int> Add(ApplicationUser ApplicationUser)
        {
            var ApplicationUserIdParameter = base.GetParameterOut(ApplicationUserInfrastructure.ApplicationUserIdParameterName, SqlDbType.BigInt, ApplicationUser.ApplicationUserId);
            var parameters = new List<DbParameter>
                {
                         ApplicationUserIdParameter,
                        base.GetParameter(ApplicationUserInfrastructure.UsernameParameterName,ApplicationUser.UserName)
                        ,base.GetParameter(ApplicationUserInfrastructure.EmailParameterName,ApplicationUser.Email)
                        ,base.GetParameter(ApplicationUserInfrastructure.FirstnameParameterName,ApplicationUser.FirstName)
                        ,base.GetParameter(ApplicationUserInfrastructure.LastnameParameterName,ApplicationUser.LastName)
                        ,base.GetParameter(ApplicationUserInfrastructure.PasswordHashParameterName,ApplicationUser.PasswordHash)
                        ,base.GetParameter(ApplicationUserInfrastructure.CreatedDateParameterName,ApplicationUser.CreatedDate)
                        ,base.GetParameter(ApplicationUserInfrastructure.CreatedByIdParameterName,ApplicationUser.CurrentUserId)
                        ,base.GetParameter(ApplicationUserInfrastructure.ActiveParameterName,ApplicationUser.Active)

                };
            await base.ExecuteNonQuery(parameters, ApplicationUserInfrastructure.AddStoredProcedureName, CommandType.StoredProcedure);
            ApplicationUser.ApplicationUserId = Convert.ToInt32(ApplicationUserIdParameter.Value);
            return ApplicationUser.ApplicationUserId;
        }

        public async Task<bool> Update(ApplicationUser ApplicationUser)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(ApplicationUserInfrastructure.ApplicationUserIdParameterName,ApplicationUser.ApplicationUserId),
                base.GetParameter(ApplicationUserInfrastructure.UsernameParameterName,ApplicationUser.UserName)
                ,base.GetParameter(ApplicationUserInfrastructure.EmailParameterName,ApplicationUser.Email)
                ,base.GetParameter(ApplicationUserInfrastructure.FirstnameParameterName,ApplicationUser.FirstName)
                ,base.GetParameter(ApplicationUserInfrastructure.LastnameParameterName,ApplicationUser.LastName)
                ,base.GetParameter(ApplicationUserInfrastructure.ModifiedDateParameterName,ApplicationUser.ModifiedDate)
                ,base.GetParameter(ApplicationUserInfrastructure.CurrentUserIdParameterName,ApplicationUser.CurrentUserId)
                ,base.GetParameter(ApplicationUserInfrastructure.ModifiedByIdParameterName,ApplicationUser.ModifiedById)
                ,base.GetParameter(ApplicationUserInfrastructure.ActiveParameterName,ApplicationUser.Active)

            };
            var ReturnValue = await base.ExecuteNonQuery(parameters, ApplicationUserInfrastructure.UpdateStoredProcedureName, CommandType.StoredProcedure);
            return ReturnValue > 0;
        }

        public async Task<ApplicationUser> Get(ApplicationUser ApplicationUser)
        {
            ApplicationUser ApplicationUserItem = new ApplicationUser();
            var parameters = new List<DbParameter>
            {
            base.GetParameter(ApplicationUserInfrastructure.ApplicationUserIdParameterName,ApplicationUser.ApplicationUserId),
            base.GetParameter(ApplicationUserInfrastructure.EmailParameterName,ApplicationUser.Email),
            };
            using (var dataReader = await base.ExecuteReader(parameters, ApplicationUserInfrastructure.GetStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {

                        ApplicationUserItem = new ApplicationUser
                        {
                            ApplicationUserId = dataReader.GetIntegerValue(ApplicationUserInfrastructure.ApplicationUserIdColumnName),
                            UserName = dataReader.GetStringValue(ApplicationUserInfrastructure.UsernameColumnName),
                            Email = dataReader.GetStringValue(ApplicationUserInfrastructure.EmailColumnName),
                            FirstName = dataReader.GetStringValue(ApplicationUserInfrastructure.FirstnameColumnName),
                            LastName = dataReader.GetStringValue(ApplicationUserInfrastructure.LastnameColumnName),
                            CreatedDate = dataReader.GetDateTimeValue(ApplicationUserInfrastructure.CreatedDateColumnName),
                            ModifiedDate = dataReader.GetDateTimeValue(ApplicationUserInfrastructure.ModifiedDateColumnName),
                            CreatedById = dataReader.GetIntegerValue(ApplicationUserInfrastructure.CreatedByIdColumnName),
                            ModifiedById = dataReader.GetIntegerValue(ApplicationUserInfrastructure.ModifiedByIdColumnName),
                            Active = dataReader.GetBooleanValue(ApplicationUserInfrastructure.ActiveColumnName)

                        };
                       
                    }
                    if (!dataReader.IsClosed)
                    {
                        dataReader.Close();
                    }

                }
                return ApplicationUserItem;
            }
        }

        public async Task<List<ApplicationUser>> GetList(ApplicationUser ApplicationUser)
        {
            var ApplicationUserList = new List<ApplicationUser>();
            ApplicationUser ApplicationUserItem = null;
            var parameters = new List<DbParameter>
            {
            };

            using (var dataReader = await base.ExecuteReader(parameters, ApplicationUserInfrastructure.GetAllStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    {
                        while (dataReader.Read())
                        {
                            ApplicationUserItem = new ApplicationUser
                            {
                                ApplicationUserId = dataReader.GetIntegerValue(ApplicationUserInfrastructure.ApplicationUserIdColumnName),
                                UserName = dataReader.GetStringValue(ApplicationUserInfrastructure.UsernameColumnName) ,
                                Email = dataReader.GetStringValue(ApplicationUserInfrastructure.EmailColumnName),
                                FirstName = dataReader.GetStringValue(ApplicationUserInfrastructure.FirstnameColumnName),
                                LastName = dataReader.GetStringValue(ApplicationUserInfrastructure.LastnameColumnName),
                                CreatedDate = dataReader.GetDateTimeValue(ApplicationUserInfrastructure.CreatedDateColumnName),
                                ModifiedDate = dataReader.GetDateTimeValue(ApplicationUserInfrastructure.ModifiedDateColumnName),
                                CreatedById = dataReader.GetIntegerValue(ApplicationUserInfrastructure.CreatedByIdColumnName),
                                ModifiedById = dataReader.GetIntegerValue(ApplicationUserInfrastructure.ModifiedByIdColumnName),
                                Active = dataReader.GetBooleanValue(ApplicationUserInfrastructure.ActiveColumnName)
                            };
                            ApplicationUserList.Add(ApplicationUserItem);
                        }
                        if (!dataReader.IsClosed)
                        {
                            dataReader.Close();
                        }
                    }
                }
                return ApplicationUserList;
            }
        }

        public async Task<bool> Activate(ApplicationUser ApplicationUser)
        {
            var parameters = new List<DbParameter>
                {

                 base.GetParameter(ApplicationUserInfrastructure.ApplicationUserIdParameterName,ApplicationUser.ApplicationUserId)
                 ,base.GetParameter(ApplicationUserInfrastructure.CurrentUserIdParameterName,ApplicationUser.CurrentUserId),
                 base.GetParameter(ApplicationUserInfrastructure.ActiveParameterName,ApplicationUser.Active)

                };
            var returnValue = await base.ExecuteNonQuery(parameters, ApplicationUserInfrastructure.ActivateStoredProcedureName, CommandType.StoredProcedure);
            return returnValue > 0;
        }

    }
}
