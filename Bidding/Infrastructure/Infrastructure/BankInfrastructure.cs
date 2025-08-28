using Dapper;
using GoldBank.Infrastructure.Extension;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;
using GoldBank.Models.Product;
using GoldBank.Models.RequestModels;
using System.Data;
using System.Data.Common;
using System.Transactions;

namespace GoldBank.Infrastructure.Infrastructure
{
    public class BankInfrastructure : BaseInfrastructure, IBankInfrastructure
    {
        public BankInfrastructure(IConfiguration configuration) : base(configuration)
        {

        }

        public async Task<bool> Activate(CompanyAccount entity)
        {
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();
            var response = 0;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_CompanyAccountId", entity.CompanyAccountId);
                parameters.Add("p_UpdatedBy", entity.UpdatedAt);
                parameters.Add("p_Active", entity.IsActive);
                parameters.Add("o_CompanyAccountId", dbType: DbType.Int32, direction: ParameterDirection.Output);


                await connection.ExecuteAsync("ActivateCompanyAccount_gb", parameters, transaction, commandType: CommandType.StoredProcedure);

                response = parameters.Get<int>("o_CompanyAccountId");
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
            }
            finally
            {
                await transaction.DisposeAsync();
            }
            return response > 0;
        }

        public async Task<int> Add(CompanyAccount entity)
        {
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();
            var response = 0;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_Description", entity.Description);
                parameters.Add("p_AccountName", entity.AccountName);
                parameters.Add("p_BranchCode", entity.BranchCode);
                parameters.Add("p_Iban", entity.Iban);
                parameters.Add("p_Currency", entity.Currency);
                parameters.Add("p_BankAccountId", entity.BankAccountId);
                parameters.Add("p_createdBy", entity.CreatedBy);
                parameters.Add("o_CompanyAccountId", dbType: DbType.Int32, direction: ParameterDirection.Output);


                await connection.ExecuteAsync("AddCompanyAccount_gb", parameters, transaction, commandType: CommandType.StoredProcedure);

                response = parameters.Get<int>("o_CompanyAccountId");
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
            }
            finally
            {
                await transaction.DisposeAsync();
            }
            return response;
        }

        public async Task<CompanyAccount> Get(CompanyAccount entity)
        {
            CompanyAccount companyAccount = new CompanyAccount();

            var parameters = new List<DbParameter>
            {
                base.GetParameter("p_CompanyAccountId", entity.CompanyAccountId)
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetCompanyAccountById_Gb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        companyAccount.CompanyAccountId = dataReader.GetIntegerValue("companyAccountId");
                        companyAccount.Description = dataReader.GetStringValue("description");
                        companyAccount.AccountName = dataReader.GetStringValue("accountName");
                        companyAccount.BranchCode = dataReader.GetStringValue("branchCode");
                        companyAccount.Iban = dataReader.GetStringValue("iban");
                        companyAccount.Currency = dataReader.GetStringValue("currency");
                        companyAccount.IsActive = dataReader.GetBooleanValue("isActive");
                        companyAccount.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        companyAccount.CreatedAt = dataReader.GetDateTime("createdAt");
                        companyAccount.CreatedBy = dataReader.GetIntegerValue("createdBy");
                    }
                }
            }
            return companyAccount;
        }

        public async Task<AllResponse<CompanyAccount>> GetAll(AllRequest<CompanyAccount> entity)
        {
            var result = new AllResponse<CompanyAccount>();
            result.Data = new List<CompanyAccount>();
            var parameters = new List<DbParameter>
            {
                base.GetParameter("@p_PageNumber", entity.Offset),
                base.GetParameter("@p_PageSize", entity.PageSize)
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetAllCompanyAccountsGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        CompanyAccount companyAccount = new CompanyAccount();
                        companyAccount.CompanyAccountId = dataReader.GetIntegerValue("companyAccountId");
                        companyAccount.Description = dataReader.GetStringValue("description");
                        companyAccount.AccountName = dataReader.GetStringValue("accountName");
                        companyAccount.BranchCode = dataReader.GetStringValue("branchCode");
                        companyAccount.Iban = dataReader.GetStringValue("iban");
                        companyAccount.Currency = dataReader.GetStringValue("currency");
                        companyAccount.IsActive = dataReader.GetBooleanValue("isActive");
                        companyAccount.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        companyAccount.CreatedAt = dataReader.GetDateTime("createdAt");
                        companyAccount.CreatedBy = dataReader.GetIntegerValue("createdBy");
                        result.Data.Add(companyAccount);
                    }
                }
                if (dataReader != null && dataReader.NextResult())
                {
                    while (dataReader.Read())
                    {
                        result.TotalRecord = dataReader.GetIntegerValue("TotalRecords");
                    }
                }
            }
            return result;
        }

        public Task<List<CompanyAccount>> GetList(CompanyAccount entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Update(CompanyAccount entity)
        {
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();
            var response = 0;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_Description", entity.Description);
                parameters.Add("p_AccountName", entity.AccountName);
                parameters.Add("p_BranchCode", entity.BranchCode);
                parameters.Add("p_Iban", entity.Iban);
                parameters.Add("p_Currency", entity.Currency);
                parameters.Add("p_BankAccountId", entity.BankAccountId);
                parameters.Add("p_UpdatedBy", entity.UpdatedBy);
                parameters.Add("p_CompanyAccountId", entity.CompanyAccountId);
                parameters.Add("o_Updated", dbType: DbType.Int32, direction: ParameterDirection.Output);


                await connection.ExecuteAsync("UpdateCompanyAccount_gb", parameters, transaction, commandType: CommandType.StoredProcedure);

                response = parameters.Get<int>("o_Updated");
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
            }
            finally
            {
                await transaction.DisposeAsync();
            }
            return response > 0;
        }
    }
}
