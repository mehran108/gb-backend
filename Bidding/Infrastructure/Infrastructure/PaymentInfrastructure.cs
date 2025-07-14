using Dapper;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;
using GoldBank.Models.Product;
using GoldBank.Models.RequestModels;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Data;
using System.Transactions;

namespace GoldBank.Infrastructure.Infrastructure
{
    public class PaymentInfrastructure : BaseInfrastructure, IPaymentInfrastructure
    {
        public PaymentInfrastructure(IConfiguration configuration) : base(configuration)
        {

        }

        public Task<bool> Activate(Payment entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> Add(Payment entity)
        {
            throw new NotImplementedException();
        }

        public Task<Payment> Get(Payment entity)
        {
            throw new NotImplementedException();
        }

        public Task<AllResponse<Payment>> GetAll(AllRequest<Payment> entity)
        {
            throw new NotImplementedException();
        }

        public Task<List<Payment>> GetList(Payment entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Payment entity)
        {
            throw new NotImplementedException();
        }
        public async Task<int> AddPayment(AddPaymentRequest paymentRM)
        {
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();

            var parameters = new DynamicParameters();
            parameters.Add("p_CustomerId", paymentRM.CustomerId);
            parameters.Add("p_PaymentTypeId", paymentRM.PaymentTypeId);
            parameters.Add("p_TotalAmount", paymentRM.TotalAmount);
            parameters.Add("p_CreatedBy", paymentRM.CreatedBy);
            parameters.Add("o_PaymentId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var paymentId = await connection.ExecuteAsync("InsertPaymentGb", parameters, transaction, commandType: CommandType.StoredProcedure);
            var newPaymentId = parameters.Get<int>("o_PaymentId");

            if (newPaymentId > 0)
            {
                if (paymentRM.PaymentOrderRM.Count > 0)
                {
                    foreach (var paymentOrder in paymentRM.PaymentOrderRM)
                    {
                        parameters = new DynamicParameters();
                        parameters.Add("p_PaymentId", newPaymentId);
                        parameters.Add("p_OrderId", paymentOrder.OrderId);
                        parameters.Add("p_TotalAmount", paymentOrder.TotalAmount);
                        parameters.Add("p_CreatedBy", paymentRM.CreatedBy);

                        await connection.ExecuteAsync("InsertPaymentOrderGb", parameters, transaction, commandType: CommandType.StoredProcedure);
                    }
                }
            }
            await transaction.CommitAsync();
            return newPaymentId;
        }
        public async Task<int> AddOnlinePayment(OnlinePaymentRM paymentRM)
        {
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();

            var parameters = new DynamicParameters();
            parameters.Add("p_PaymentId", paymentRM.PaymentId);
            parameters.Add("p_Amount", paymentRM.Amount);
            parameters.Add("p_TransactionId", paymentRM.TransactionId);
            parameters.Add("p_CustomerAccountId", paymentRM.CustomerAccountId);
            parameters.Add("p_CompanyAccountId", paymentRM.CompanyAccountId);
            parameters.Add("p_CustomerAccountNumber", paymentRM.CustomerAccountNumber);
            parameters.Add("P_IsVerificationRequested", paymentRM.IsVerificationRequested);
            parameters.Add("P_IsVerificationPassed", paymentRM.IsVerificationPassed);
            parameters.Add("P_IsVerificationFailed", paymentRM.IsVerificationFailed);
            parameters.Add("P_CreatedBy", paymentRM.CreatedBy);
            parameters.Add("o_OnlinePaymentId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("InsertOnlinePaymentGb", parameters, transaction, commandType: CommandType.StoredProcedure);

            var onlinePaymentId = parameters.Get<int>("o_OnlinePaymentId");


            if (onlinePaymentId > 0)
            {
                if (paymentRM.OnlinePaymentDocumentRM.Count > 0)
                {
                    foreach (var paymentOrder in paymentRM.OnlinePaymentDocumentRM)
                    {
                        parameters = new DynamicParameters();
                        parameters.Add("p_OnlinePaymentId", onlinePaymentId);
                        parameters.Add("p_DocumentId", paymentOrder.DocumentId);
                        parameters.Add("p_IsPrimary", paymentOrder.IsPrimary);
                        parameters.Add("p_CreatedBy", paymentRM.CreatedBy);
                        parameters.Add("o_OnlinePaymentDocumentId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                        await connection.ExecuteAsync("InsertUpdateOnlinePaymentDocumentGb", parameters, transaction, commandType: CommandType.StoredProcedure);

                        var onlinePaymentDocumentId = parameters.Get<int>("o_OnlinePaymentDocumentId");
                    }
                }
            }
            await transaction.CommitAsync();
            return onlinePaymentId;
        }
        public async Task<int> AddCardPayment(CardPaymentRM paymentRM)
        {
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();

            var parameters = new DynamicParameters();
            parameters.Add("P_PaymentId",paymentRM.PaymentId);
            parameters.Add("P_Amount",paymentRM.Amount);
            parameters.Add("P_TransactionId", paymentRM.TransactionId);
            parameters.Add("P_ReceiptNo", paymentRM.ReceiptNo);
            parameters.Add("P_TransactionDate", paymentRM.TransactionDate);
            parameters.Add("P_LastFourDigits", paymentRM.LastFourDigits);
            parameters.Add("P_CreatedBy", paymentRM.CreatedBy);
            parameters.Add("o_CardPaymentId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("InsertCardPaymentGb", parameters, transaction, commandType: CommandType.StoredProcedure);

            var onlinePaymentId = parameters.Get<int>("o_CardPaymentId");
            return onlinePaymentId;
        }
    }
}
