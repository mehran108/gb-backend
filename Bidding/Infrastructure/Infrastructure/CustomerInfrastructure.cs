using GoldBank.Models;
using System.Data.Common;
using System.Data;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Infrastructure.Extension;

namespace GoldBank.Infrastructure.Infrastructure
{
    public class CustomerInfrastructure : BaseInfrastructure, ICustomerInfrastructure
    {
        public CustomerInfrastructure(IConfiguration configuration) : base(configuration)
        {
        }


        //************** INFRASTRUCTURES ****************
        private const string AddStoredProcedureName = "CustomerAdd";
        private const string UpdateStoredProcedureName = "CustomerUpdate";
        private const string ActivateStoredProcedureName = "CustomerActivate";
        private const string GetStoredProcedureName = "CustomerGetById";
        private const string GetAllStoredProcedureName = "CustomerGetAll";
        private const string CustomerIdColumnName = "CustomerId";
        private const string UserNameColumnName = "UserName";
        private const string EmailColumnName = "Email";
        private const string FirstNameColumnName = "FirstName";
        private const string LastNameColumnName = "LastName";
        private const string MobileColumnName = "Mobile";
        private const string AddressColumnName = "Address";
        private const string PasswordHashColumnName = "PasswordHash";
        private const string CreatedDateColumnName = "CreatedDate";
        private const string ModifiedDateColumnName = "ModifiedDate";
        private const string CreatedByIdColumnName = "CreatedById";
        private const string ModifiedByIdColumnName = "ModifiedById";
        private const string ActiveColumnName = "Active";
        private const string CustomerIdParameterName = "@PCustomerId";
        private const string UserNameParameterName = "@PUserName";
        private const string EmailParameterName = "@PEmail";
        private const string FirstNameParameterName = "@PFirstName";
        private const string LastNameParameterName = "@PLastName";
        private const string MobileParameterName = "@PMobile";
        private const string AddressParameterName = "@PAddress";
        private const string PasswordHashParameterName = "@PPasswordHash";
        private const string CreatedDateParameterName = "@PCreatedDate";
        private const string ModifiedDateParameterName = "@PModifiedDate";
        private const string CreatedByIdParameterName = "@PCreatedById";
        private const string ModifiedByIdParameterName = "@PModifiedById";
        private const string ActiveParameterName = "@PActive";

        public async Task<int> Add(Customer Customer)
        {
            var CustomerIdParameter = base.GetParameterOut(CustomerInfrastructure.CustomerIdParameterName, SqlDbType.Int, Customer.CustomerId);
            var parameters = new List<DbParameter>
                {
                         CustomerIdParameter,
                        base.GetParameter(CustomerInfrastructure.UserNameParameterName,Customer.UserName)
                        ,base.GetParameter(CustomerInfrastructure.EmailParameterName,Customer.Email)
                        ,base.GetParameter(CustomerInfrastructure.FirstNameParameterName,Customer.FirstName)
                        ,base.GetParameter(CustomerInfrastructure.LastNameParameterName,Customer.LastName)
                        ,base.GetParameter(CustomerInfrastructure.MobileParameterName,Customer.Mobile)
                        ,base.GetParameter(CustomerInfrastructure.AddressParameterName,Customer.Address)
                        ,base.GetParameter(CustomerInfrastructure.PasswordHashParameterName,Customer.PasswordHash)
                        ,base.GetParameter(CustomerInfrastructure.CreatedDateParameterName,Customer.CreatedDate)
                        ,base.GetParameter(CustomerInfrastructure.CreatedByIdParameterName,Customer.CurrentUserId)
                        ,base.GetParameter(CustomerInfrastructure.ActiveParameterName,Customer.Active)

                };
            await base.ExecuteNonQuery(parameters, CustomerInfrastructure.AddStoredProcedureName, CommandType.StoredProcedure);
            Customer.CustomerId = Convert.ToInt32(CustomerIdParameter.Value);
            return Customer.CustomerId;
        }

        public async Task<bool> Update(Customer Customer)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(CustomerInfrastructure.CustomerIdParameterName,Customer.CustomerId),
                base.GetParameter(CustomerInfrastructure.UserNameParameterName,Customer.UserName)
                ,base.GetParameter(CustomerInfrastructure.EmailParameterName,Customer.Email)
                ,base.GetParameter(CustomerInfrastructure.FirstNameParameterName,Customer.FirstName)
                ,base.GetParameter(CustomerInfrastructure.LastNameParameterName,Customer.LastName)
                ,base.GetParameter(CustomerInfrastructure.MobileParameterName,Customer.Mobile)
                ,base.GetParameter(CustomerInfrastructure.AddressParameterName,Customer.Address)
                ,base.GetParameter(CustomerInfrastructure.ModifiedDateParameterName,Customer.ModifiedDate)
                ,base.GetParameter(CustomerInfrastructure.CurrentUserIdParameterName,Customer.CurrentUserId)
                ,base.GetParameter(CustomerInfrastructure.ModifiedByIdParameterName,Customer.ModifiedById)
                ,base.GetParameter(CustomerInfrastructure.ActiveParameterName,Customer.Active)

            };
            var ReturnValue = await base.ExecuteNonQuery(parameters, CustomerInfrastructure.UpdateStoredProcedureName, CommandType.StoredProcedure);
            return ReturnValue > 0;
        }

        public async Task<Customer> GetById(Customer Customer)
        {
            Customer CustomerItem = new Customer();
            var parameters = new List<DbParameter>
            {
            base.GetParameter(CustomerInfrastructure.CustomerIdParameterName,Customer.CustomerId),
            base.GetParameter(CustomerInfrastructure.EmailParameterName,Customer.Email),
            };
            using (var dataReader = await base.ExecuteReader(parameters, CustomerInfrastructure.GetStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {

                        CustomerItem = new Customer
                        {
                            CustomerId = dataReader.GetIntegerValue(CustomerInfrastructure.CustomerIdColumnName),
                            UserName = dataReader.GetStringValue(CustomerInfrastructure.UserNameColumnName)
                        ,
                            Email = dataReader.GetStringValue(CustomerInfrastructure.EmailColumnName)
                        ,
                            FirstName = dataReader.GetStringValue(CustomerInfrastructure.FirstNameColumnName)
                        ,
                            LastName = dataReader.GetStringValue(CustomerInfrastructure.LastNameColumnName),
                            PasswordHash = dataReader.GetStringValue(CustomerInfrastructure.PasswordHashColumnName)
                        ,Mobile = dataReader.GetStringValue(CustomerInfrastructure.MobileColumnName)
                        ,Address = dataReader.GetStringValue(CustomerInfrastructure.AddressColumnName)
                        ,
                            CreatedDate = dataReader.GetDateTimeValue(CustomerInfrastructure.CreatedDateColumnName)
                        ,
                            ModifiedDate = dataReader.GetDateTimeValue(CustomerInfrastructure.ModifiedDateColumnName)
                        ,
                            CreatedById = dataReader.GetIntegerValue(CustomerInfrastructure.CreatedByIdColumnName)
                        ,
                            ModifiedById = dataReader.GetIntegerValue(CustomerInfrastructure.ModifiedByIdColumnName)
                        ,
                            Active = dataReader.GetBooleanValue(CustomerInfrastructure.ActiveColumnName)

                        };
                        
                    }
                    if (!dataReader.IsClosed)
                    {
                        dataReader.Close();
                    }

                }
                return CustomerItem;
            }
        }

        public async Task<List<Customer>> GetAll(Customer Customer)
        {
            var CustomerList = new List<Customer>();
            Customer CustomerItem = null;
            var parameters = new List<DbParameter>
            {
            };

            using (var dataReader = await base.ExecuteReader(parameters, CustomerInfrastructure.GetAllStoredProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    {
                        while (dataReader.Read())
                        {
                            CustomerItem = new Customer
                            {
                                CustomerId = dataReader.GetIntegerValue(CustomerInfrastructure.CustomerIdColumnName),
                                UserName = dataReader.GetStringValue(CustomerInfrastructure.UserNameColumnName)
                            ,
                                Email = dataReader.GetStringValue(CustomerInfrastructure.EmailColumnName)
                            ,
                                FirstName = dataReader.GetStringValue(CustomerInfrastructure.FirstNameColumnName)
                            ,
                                LastName = dataReader.GetStringValue(CustomerInfrastructure.LastNameColumnName)
                            ,    
                                Mobile = dataReader.GetStringValue(CustomerInfrastructure.MobileColumnName)
                        ,
                                Address = dataReader.GetStringValue(CustomerInfrastructure.AddressColumnName)
                                ,

                                CreatedDate = dataReader.GetDateTimeValue(CustomerInfrastructure.CreatedDateColumnName)
                            ,
                                ModifiedDate = dataReader.GetDateTimeValue(CustomerInfrastructure.ModifiedDateColumnName)
                            ,
                                CreatedById = dataReader.GetIntegerValue(CustomerInfrastructure.CreatedByIdColumnName)
                            ,
                                ModifiedById = dataReader.GetIntegerValue(CustomerInfrastructure.ModifiedByIdColumnName)
                            ,
                                Active = dataReader.GetBooleanValue(CustomerInfrastructure.ActiveColumnName)

                            };
                            CustomerList.Add(CustomerItem);
                        }
                        if (!dataReader.IsClosed)
                        {
                            dataReader.Close();
                        }
                    }
                }
                return CustomerList;
            }
        }

        public async Task<bool> Activate(Customer Customer)
        {
            var parameters = new List<DbParameter>
                {

                 base.GetParameter(CustomerInfrastructure.CustomerIdParameterName,Customer.CustomerId)
                 ,base.GetParameter(CustomerInfrastructure.CurrentUserIdParameterName,Customer.CurrentUserId),
                 base.GetParameter(CustomerInfrastructure.ActiveParameterName,Customer.Active)

                };
            var returnValue = await base.ExecuteNonQuery(parameters, CustomerInfrastructure.ActivateStoredProcedureName, CommandType.StoredProcedure);
            return returnValue > 0;
        }





    }
}
