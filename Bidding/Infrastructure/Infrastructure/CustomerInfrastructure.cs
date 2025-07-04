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
       
       
        private const string ReferenceCustomerIdParameterName = "@PReferenceCustomerId";
        private const string EmailParameterName = "@PEmail";
        private const string FirstNameParameterName = "@PFirstName";
        private const string LastNameParameterName = "@PLastName";
        private const string MobileParameterName = "@PMobile";
        private const string PostalAddressParameterName = "@PPostalAddress";


        private const string CreatedByIdParameterName = "@PCreatedBy";
        private const string ModifiedByIdParameterName = "@PUpdatedBy";
        private const string ActiveParameterName = "@PActive";
        private const string CountryIdParameterName = "@PCountryId";
        private const string CityIdParameterName = "@PCityId";
        private const string IsPOSParameterName = "@PIsPOS";
        
        private const string TitleParameterName = "@PTitle";
        private const string BirthAnniversaryParameterName = "@PBirthAnniversary";
        private const string WeddingAnniversaryParameterName = "@PWeddingAnniversary";
        private const string IsNewSubscribeParameterName = "@PIsNewSubscribe";


        public async Task<int> Add(Customer Customer)
        {
            var CustomerIdParameter = base.GetParameterOut(CustomerInfrastructure.ReferenceCustomerIdParameterName, SqlDbType.Int, Customer.ReferenceCustomerId);
            var parameters = new List<DbParameter>
                {
                      CustomerIdParameter,
                     base.GetParameter(CustomerInfrastructure.EmailParameterName,Customer.Email),
                     base.GetParameter(CustomerInfrastructure.FirstNameParameterName,Customer.FirstName),
                     base.GetParameter(CustomerInfrastructure.LastNameParameterName,Customer.LastName),
                     base.GetParameter(CustomerInfrastructure.MobileParameterName,Customer.Mobile),
                     base.GetParameter(CustomerInfrastructure.PostalAddressParameterName,Customer.PostalAddress),
                     base.GetParameter(CustomerInfrastructure.CreatedByIdParameterName,Customer.CreatedBy),
                     base.GetParameter(CustomerInfrastructure.CountryIdParameterName,Customer.CountryId),
                     base.GetParameter(CustomerInfrastructure.CityIdParameterName,Customer.CityId),
                     base.GetParameter(CustomerInfrastructure.IsPOSParameterName,Customer.IsPOS),
                     base.GetParameter(CustomerInfrastructure.TitleParameterName,Customer.Title),
                     base.GetParameter(CustomerInfrastructure.BirthAnniversaryParameterName,Customer.BirthAnniversary),
                     base.GetParameter(CustomerInfrastructure.WeddingAnniversaryParameterName,Customer.WeddingAnniversary),
                     base.GetParameter(CustomerInfrastructure.IsNewSubscribeParameterName,Customer.IsNewSubscribe),

            };
            await base.ExecuteNonQuery(parameters, "AddCustomer_gb", CommandType.StoredProcedure);
            Customer.ReferenceCustomerId = Convert.ToInt32(CustomerIdParameter.Value);
            return Customer.ReferenceCustomerId;
        }

        public async Task<bool> Update(Customer Customer)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(CustomerInfrastructure.ReferenceCustomerIdParameterName,Customer.ReferenceCustomerId),
                base.GetParameter(CustomerInfrastructure.FirstNameParameterName,Customer.FirstName),
                base.GetParameter(CustomerInfrastructure.LastNameParameterName,Customer.LastName),
                base.GetParameter(CustomerInfrastructure.MobileParameterName,Customer.Mobile),
                base.GetParameter(CustomerInfrastructure.PostalAddressParameterName,Customer.PostalAddress),
                base.GetParameter(CustomerInfrastructure.ModifiedByIdParameterName,Customer.UpdatedBy),
                base.GetParameter(CustomerInfrastructure.CountryIdParameterName,Customer.CountryId),
                base.GetParameter(CustomerInfrastructure.CityIdParameterName,Customer.CityId),
                base.GetParameter(CustomerInfrastructure.IsPOSParameterName,Customer.IsPOS),
                base.GetParameter(CustomerInfrastructure.TitleParameterName,Customer.Title),
                base.GetParameter(CustomerInfrastructure.BirthAnniversaryParameterName,Customer.BirthAnniversary),
                base.GetParameter(CustomerInfrastructure.WeddingAnniversaryParameterName,Customer.WeddingAnniversary),
                base.GetParameter(CustomerInfrastructure.IsNewSubscribeParameterName,Customer.IsNewSubscribe),
                base.GetParameter(CustomerInfrastructure.ActiveParameterName,Customer.IsActive)

            };
            var ReturnValue = await base.ExecuteNonQuery(parameters, "UpdateCustomer_gb", CommandType.StoredProcedure);
            return ReturnValue > 0;
        }

        public async Task<Customer> Get(Customer Customer)
        {
            Customer customerItem = new Customer();
            var parameters = new List<DbParameter>
            {
            base.GetParameter(CustomerInfrastructure.ReferenceCustomerIdParameterName,Customer.ReferenceCustomerId),
            base.GetParameter(CustomerInfrastructure.EmailParameterName,Customer.Email),
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetCustomerById_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        customerItem.ReferenceCustomerId = dataReader.GetIntegerValue("ReferenceCustomerId");
                        customerItem.FirstName = dataReader.GetStringValue("FirstName");
                        customerItem.LastName = dataReader.GetStringValue("LastName");
                        customerItem.Email = dataReader.GetStringValue("Email");
                        customerItem.Mobile = dataReader.GetStringValue("Mobile");
                        customerItem.PostalAddress = dataReader.GetStringValue("PostalAddress");
                        customerItem.CountryId = dataReader.GetIntegerValue("CountryId");
                        customerItem.CityId = dataReader.GetIntegerValue("CityId");
                        customerItem.IsPOS = dataReader.GetBooleanValueNullable("IsPOS");
                        customerItem.BirthAnniversary = dataReader.GetDateTimeValue("BirthAnniversary");
                        customerItem.WeddingAnniversary = dataReader.GetDateTimeValue("WeddingAnniversary");
                        customerItem.IsActive = dataReader.GetBooleanValue("IsActive");
                        customerItem.CreatedBy = dataReader.GetIntegerValue("CreatedBy");
                        customerItem.UpdatedBy = dataReader.GetIntegerValue("UpdatedBy");
                        customerItem.CreatedAt = dataReader.GetDateTimeValue("CreatedAt");
                        customerItem.UpdatedAt = dataReader.GetDateTimeValue("UpdatedAt");
                        
                    }
                    if (!dataReader.IsClosed)
                    {
                        dataReader.Close();
                    }

                }
                return customerItem;
            }
        }

        public async Task<List<Customer>> GetList(Customer Customer)
        {
            var CustomerList = new List<Customer>();
            var parameters = new List<DbParameter>
            {
            };

            using (var dataReader = await base.ExecuteReader(parameters, "GetCustomerList_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    {
                        while (dataReader.Read())
                        {
                            var customerItem = new Customer();
                            customerItem.ReferenceCustomerId = dataReader.GetIntegerValue("ReferenceCustomerId");
                            customerItem.FirstName = dataReader.GetStringValue("FirstName");
                            customerItem.LastName = dataReader.GetStringValue("LastName");
                            customerItem.Email = dataReader.GetStringValue("Email");
                            customerItem.Mobile = dataReader.GetStringValue("Mobile");
                            customerItem.PostalAddress = dataReader.GetStringValue("PostalAddress");
                            customerItem.CountryId = dataReader.GetIntegerValue("CountryId");
                            customerItem.CityId = dataReader.GetIntegerValue("CityId");
                            customerItem.IsPOS = dataReader.GetBooleanValueNullable("IsPOS");
                            customerItem.BirthAnniversary = dataReader.GetDateTimeValueNullable("BirthAnniversary");
                            customerItem.WeddingAnniversary = dataReader.GetDateTimeValueNullable("WeddingAnniversary");
                            customerItem.IsActive = dataReader.GetBooleanValue("IsActive");
                            customerItem.CreatedBy = dataReader.GetIntegerValue("CreatedBy");
                            customerItem.UpdatedBy = dataReader.GetIntegerValue("UpdatedBy");
                            customerItem.CreatedAt = dataReader.GetDateTimeValue("CreatedAt");
                            customerItem.UpdatedAt = dataReader.GetDateTimeValue("UpdatedAt");

                            CustomerList.Add(customerItem);
                        }
                        if (!dataReader.IsClosed)
                        {
                            dataReader.Close();
                        }
                    }
                }
            }
            return CustomerList;
        }

        public async Task<bool> Activate(Customer Customer)
        {
           throw new NotImplementedException();
        }
        public async Task<bool> Delete(Customer customer)
        {
            var parameters = new List<DbParameter>
            {
            base.GetParameter(CustomerInfrastructure.ReferenceCustomerIdParameterName,customer.ReferenceCustomerId),
            base.GetParameter(CustomerInfrastructure.EmailParameterName,customer.Email),
            };
            await base.ExecuteNonQuery(parameters, "DeleteCustomer_gb", CommandType.StoredProcedure);
            return true;
        }

        public async Task<AllResponse<Customer>> GetAll(AllRequest<Customer> entity)
        {
            if (entity.SearchText == null)
            {
                entity.SearchText = "";
            }
            var result = new AllResponse<Customer>
            {
                Data = new List<Customer>(),
                Offset = entity.Offset,
                PageSize = entity.PageSize,
                SortColumn = entity.SortColumn,
                SortAscending = entity.SortAscending
            };

            var totalRecordParamter = base.GetParameterOut(BaseInfrastructure.TotalRecordParameterName, SqlDbType.Int, result.TotalRecord);

            var parameters = new List<DbParameter>
            {
                base.GetParameter(BaseInfrastructure.OffsetParameterName, entity.Offset),
                base.GetParameter(BaseInfrastructure.PageSizeParameterName, entity.PageSize),
                base.GetParameter(BaseInfrastructure.SortColumnParameterName, entity.SortColumn),
                base.GetParameter(BaseInfrastructure.SortAscendingParameterName, entity.SortAscending),
                base.GetParameter(BaseInfrastructure.SearchTextParameterName, entity.SearchText),
                totalRecordParamter
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetAllCustomer_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    {
                        while (dataReader.Read())
                        {
                            var customerItem = new Customer();
                            customerItem.ReferenceCustomerId = dataReader.GetIntegerValue("ReferenceCustomerId");
                            customerItem.FirstName = dataReader.GetStringValue("FirstName");
                            customerItem.LastName = dataReader.GetStringValue("LastName");
                            customerItem.Email = dataReader.GetStringValue("Email");
                            customerItem.Mobile = dataReader.GetStringValue("Mobile");
                            customerItem.PostalAddress = dataReader.GetStringValue("PostalAddress");
                            customerItem.CountryId = dataReader.GetIntegerValue("CountryId");
                            customerItem.CityId = dataReader.GetIntegerValue("CityId");
                            customerItem.IsPOS = dataReader.GetBooleanValueNullable("IsPOS");
                            customerItem.BirthAnniversary = dataReader.GetDateTimeValue("BirthAnniversary");
                            customerItem.WeddingAnniversary = dataReader.GetDateTimeValue("WeddingAnniversary");
                            customerItem.IsActive = dataReader.GetBooleanValue("IsActive");
                            customerItem.CreatedBy = dataReader.GetIntegerValue("CreatedBy");
                            customerItem.UpdatedBy = dataReader.GetIntegerValue("UpdatedBy");
                            customerItem.CreatedAt = dataReader.GetDateTimeValue("CreatedAt");
                            customerItem.UpdatedAt = dataReader.GetDateTimeValue("UpdatedAt");

                            result.Data.Add(customerItem);
                        }
                        if (!dataReader.IsClosed)
                        {
                            dataReader.Close();
                        }
                    }
                }
                
            }
            return result;
        }
    }
}
