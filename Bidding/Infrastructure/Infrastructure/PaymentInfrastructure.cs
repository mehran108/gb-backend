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
            var response = 0;
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_CustomerId", paymentRM.CustomerId);
                parameters.Add("p_PaymentTypeId", paymentRM.PaymentTypeId);
                parameters.Add("p_TotalAmount", paymentRM.TotalAmount);
                parameters.Add("p_CreatedBy", paymentRM.CreatedBy);
                parameters.Add("o_PaymentId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                response = await connection.ExecuteAsync("InsertPaymentGb", parameters, transaction, commandType: CommandType.StoredProcedure);
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
                response =  newPaymentId;
            }
            catch
            {
                await transaction.RollbackAsync();
            }
            finally
            {
                await connection.DisposeAsync();

            }  
            return response;
        }
        public async Task<int> AddOnlinePayment(AddOnlinePaymentRequest paymentRM)
        {
            var response = 0;
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_PaymentId", paymentRM.PaymentId);
                parameters.Add("p_Amount", paymentRM.Amount);
                parameters.Add("p_TransactionId", paymentRM.TransactionId);
                parameters.Add("p_CustomerAccountId", paymentRM.CustomerAccountId);
                parameters.Add("p_CompanyAccountId", paymentRM.CompanyAccountId);
                parameters.Add("p_CustomerAccountNumber", paymentRM.CustomerAccountNumber);
                parameters.Add("P_CreatedBy", paymentRM.CreatedBy);
                parameters.Add("o_OnlinePaymentId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync("InsertOnlinePaymentGb", parameters, transaction, commandType: CommandType.StoredProcedure);

                var onlinePaymentId = parameters.Get<int>("o_OnlinePaymentId");
                response = onlinePaymentId;

                if (onlinePaymentId > 0)
                {
                    foreach (var paymentOrder in paymentRM.OnlinePaymentDocumentRM)
                    {
                        parameters = new DynamicParameters();
                        parameters.Add("p_OnlinePaymentId", onlinePaymentId);
                        parameters.Add("p_DocumentId", paymentOrder.DocumentId);
                        parameters.Add("p_IsPrimary", paymentOrder.IsPrimary);
                        parameters.Add("p_CreatedBy", paymentRM.CreatedBy);

                        await connection.ExecuteAsync("InsertOnlinePaymentDocumentGb", parameters, transaction, commandType: CommandType.StoredProcedure);

                    }
                }
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
        public async Task<bool> VerifyOnlinePayment(VerifyOnlinePaymentRequest verifyOnlinePaymentRequest)
        {
            using var connection = base.GetConnection();

            var parameters = new DynamicParameters();
            parameters.Add("p_OnlinePaymentId", verifyOnlinePaymentRequest.OnlinePaymentId);
            parameters.Add("p_IsApproved", verifyOnlinePaymentRequest.IsApproved == true ? 1 : 0, DbType.Byte);
            parameters.Add("p_Notes", verifyOnlinePaymentRequest.Notes);
            parameters.Add("o_Success", dbType: DbType.Byte, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "VerifyOnlinePaymentGb",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var tinyIntValue = parameters.Get<byte>("o_Success");
            var isSucceed = tinyIntValue == 1;
            return isSucceed;
        }
        public async Task<bool> ConfirmPayment(ConfirmPaymentRequest confirmPaymentRequest)
        {
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();
            bool response = false;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_PaymentId", confirmPaymentRequest.PaymentId);
                parameters.Add("P_CashAmount", confirmPaymentRequest.CashAmount);
                parameters.Add("p_createdBy", confirmPaymentRequest.CreatedBy);
                parameters.Add("o_Success", dbType: DbType.Byte, direction: ParameterDirection.Output);

                await connection.ExecuteAsync("ConfirmPaymentGb", parameters, transaction, commandType: CommandType.StoredProcedure);
                var succeed = parameters.Get<Byte>("o_Success");
                response = succeed == 1;
                if (response)
                {
                    foreach (var cardPay in confirmPaymentRequest.CardPayment)
                    {
                        parameters = new DynamicParameters();
                        parameters.Add("p_TransactionId", cardPay.TransactionId);
                        parameters.Add("p_PaymentId", confirmPaymentRequest.PaymentId);
                        parameters.Add("p_TransactionDate", cardPay.TransactionDate);
                        parameters.Add("p_ReceiptNo", cardPay.ReceiptNo);
                        parameters.Add("p_Amount", cardPay.Amount);
                        parameters.Add("p_LastFourDigit", cardPay.LastFourDigit);
                        parameters.Add("p_CreatedBy", confirmPaymentRequest.CreatedBy);

                        await connection.ExecuteAsync("AddCardPaymentGb", parameters, transaction, commandType: CommandType.StoredProcedure);
                    }
                }
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
        public async Task<bool?> CheckOnlinePaymentStatus(int onlinePaymentId)
        {
            using var connection = base.GetConnection();

            var parameters = new DynamicParameters();
            parameters.Add("p_onlinePaymentId", onlinePaymentId);
            parameters.Add("o_PaymentStatus", dbType: DbType.Byte, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("CheckOnlinePaymentStatusGb", parameters, commandType: CommandType.StoredProcedure);
            byte? succeed = parameters.Get<byte?>("o_PaymentStatus");
            bool? response = succeed == null ? null : (succeed == 1 ? true : false);
            return response;

        }
        public async void CancelPayment(int paymentId)
        {
            using var connection = base.GetConnection();

            var parameters = new DynamicParameters();
            parameters.Add("p_PaymentId", paymentId);
            await connection.ExecuteAsync("CancelPaymentGb", parameters, commandType: CommandType.StoredProcedure);
        }
        public async void CancelOnlinePayment(int onlinePaymentId)
        {
            using var connection = base.GetConnection();

            var parameters = new DynamicParameters();
            parameters.Add("p_OnlinePaymentId", onlinePaymentId);
            await connection.ExecuteAsync("CancelOnlinePaymentGb", parameters, commandType: CommandType.StoredProcedure);
        }
    }
}
