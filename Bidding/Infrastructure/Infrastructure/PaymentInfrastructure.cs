using Dapper;
using GoldBank.Infrastructure.Extension;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;
using GoldBank.Models.Product;
using GoldBank.Models.RequestModels;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Data;
using System.Data.Common;
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

        public async Task<Payment> Get(Payment entity)
        {
            var res = new Payment();
            res.OnlinePayment = new List<OnlinePayment>();
            res.PaymentOrder = new List<PaymentOrder>();
            res.Customer = new Customer();
            var onlinePaymentDocuments = new List<OnlinePaymentDocument>();
            var parameters = new List<DbParameter>
            {
                base.GetParameter("p_PaymentId", entity.PaymentId)
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetPaymentById_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        res.PaymentId = dataReader.GetIntegerValue("paymentId");
                        res.CustomerId = dataReader.GetIntegerValue("customerId");
                        res.PaymentTypeId = dataReader.GetIntegerValue("paymentTypeId");
                        res.TotalAmount = dataReader.GetDecimalValue("totalAmount");
                        res.CashAmount = dataReader.GetDecimalValue("cashAmount");
                        res.IsConfirmed = dataReader.GetBooleanValue("isConfirmed");
                        res.CreatedBy = dataReader.GetIntegerValue("createdBy");
                        res.PaymentOrder = new List<PaymentOrder>();
                    }
                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            var item = new PaymentOrder();
                            item.PaymentOrderId = dataReader.GetIntegerValue("paymentOrderId");
                            item.PaymentId = dataReader.GetIntegerValue("paymentId");
                            item.OrderId = dataReader.GetIntegerValue("orderId");
                            item.TotalAmount = dataReader.GetDecimalValue("totalAmount");
                            item.PaymentId = dataReader.GetIntegerValue("paymentId");

                            res.PaymentOrder.Add(item);
                        }
                    }
                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            var onlinepayment = new OnlinePayment();
                            onlinepayment.OnlinePaymentId = dataReader.GetIntegerValue("onlinePaymentId");
                            onlinepayment.PaymentId = dataReader.GetIntegerValue("paymentId");
                            onlinepayment.Amount = dataReader.GetDecimalValue("amount");
                            onlinepayment.TransactionId = dataReader.GetStringValue("transactionId");
                            onlinepayment.CustomerAccountId = dataReader.GetIntegerValue("customerAccountId");
                            onlinepayment.CompanyAccountId = dataReader.GetIntegerValue("companyAccountId");
                            onlinepayment.CustomerAccountNumber = dataReader.GetStringValue("customerAccountNumber");
                            onlinepayment.IsVerficationRequested = dataReader.GetBooleanValue("isVerificationRequested");
                            onlinepayment.IsVerficationPassed = dataReader.GetBooleanValue("isVerificationPassed");
                            onlinepayment.CustomerAccount = dataReader.GetStringValue("customerAccount");
                            onlinepayment.CompanyAccount = dataReader.GetStringValue("CompanyAccount");
                            onlinepayment.IsVerficationFailed = dataReader.GetBooleanValue("isVerificationFailed");
                            onlinepayment.OnlinePaymentDocument = new List<OnlinePaymentDocument>();
                            res.OnlinePayment.Add(onlinepayment);
                        }
                    }

                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            var item = new OnlinePaymentDocument();
                            item.OnlinePaymentDocumentId = dataReader.GetIntegerValue("onlinePaymentDocumentId");
                            item.DocumentId = dataReader.GetIntegerValue("documentId");
                            item.OnlinePaymentId = dataReader.GetIntegerValue("OnlinePaymentId");
                            item.IsPrimary = dataReader.GetBooleanValue("isPrimary");
                            item.Url = dataReader.GetStringValue("url");

                            var orderItem = res.OnlinePayment?.FirstOrDefault(o => o.OnlinePaymentId == item.OnlinePaymentId);

                            if (orderItem != null)
                            {
                                if (orderItem.OnlinePaymentDocument == null)
                                {
                                    orderItem.OnlinePaymentDocument = new List<OnlinePaymentDocument>();
                                }
                                orderItem.OnlinePaymentDocument.Add(item);
                            }
                        }
                    }
                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            res.Customer.CustomerId = dataReader.GetIntegerValue("CustomerId");
                            res.Customer.ReferenceCustomerId = dataReader.GetIntegerValue("ReferenceCustomerId");
                            res.Customer.FirstName = dataReader.GetStringValue("FirstName");
                            res.Customer.LastName = dataReader.GetStringValue("LastName");
                            res.Customer.Email = dataReader.GetStringValue("Email");
                            res.Customer.Mobile = dataReader.GetStringValue("Mobile");
                            res.Customer.PostalAddress = dataReader.GetStringValue("PostalAddress");
                            res.Customer.CountryId = dataReader.GetIntegerValue("CountryId");
                            res.Customer.CityId = dataReader.GetIntegerValue("CityId");
                            res.Customer.IsPOS = dataReader.GetBooleanValueNullable("IsPOS");
                            res.Customer.BirthAnniversary = dataReader.GetDateTimeValue("BirthAnniversary");
                            res.Customer.WeddingAnniversary = dataReader.GetDateTimeValue("WeddingAnniversary");
                            res.Customer.IsActive = dataReader.GetBooleanValue("IsActive");
                            res.Customer.CreatedBy = dataReader.GetIntegerValue("CreatedBy");
                            res.Customer.UpdatedBy = dataReader.GetIntegerValue("UpdatedBy");
                            res.Customer.CreatedAt = dataReader.GetDateTimeValue("CreatedAt");
                            res.Customer.UpdatedAt = dataReader.GetDateTimeValue("UpdatedAt");
                            res.Customer.RingSize = dataReader.GetStringValue("ringSize");
                            res.Customer.BangleSize = dataReader.GetStringValue("bangleSize");

                        }
                    }
                }
            }
            return res;
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
                response = newPaymentId;
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
        public async Task<AllResponse<OnlinePaymentVerificationVM>> GetAllOnlinePayments(AllRequest<OnlinePaymentVerificationRM> Payment)
        {
            var res = new AllResponse<OnlinePaymentVerificationVM>();
            var paymentItems = new List<OnlinePaymentVerificationVM>();
            var parameters = new List<DbParameter>
            {
                 base.GetParameter("p_PageNumber", Payment.Offset),
                 base.GetParameter("p_PageSize", Payment.PageSize),
                 base.GetParameter("p_CustomerId", Payment.Data.CustomerId),
                 base.GetParameter("p_IsVerificationRequested", Payment.Data.IsVerficationPassed),
                 base.GetParameter("p_IsVerficationFailed", Payment.Data.IsVerficationFailed),
                 base.GetParameter("p_IsVerficationPassed",Payment.Data.IsVerficationPassed)
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetAllOnlinePayment_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        var paymentItem = new OnlinePaymentVerificationVM();
                        paymentItem.PaymentId = dataReader.GetIntegerValue("paymentId");
                        paymentItem.OnlinePaymentId = dataReader.GetIntegerValue("onlinePaymentId");
                        paymentItem.Amount = dataReader.GetDecimalValue("amount");
                        paymentItem.TransactionId = dataReader.GetStringValue("transactionId");
                        paymentItem.CustomerAccountId = dataReader.GetIntegerValue("customerAccountId");
                        paymentItem.CustomerAccount = dataReader.GetStringValue("customerAccount");
                        paymentItem.CustomerAccountNumber = dataReader.GetStringValue("customerAccountNumber");
                        paymentItem.CompanyAccountId = dataReader.GetIntegerValue("companyAccountId");
                        paymentItem.CompanyAccount = dataReader.GetStringValue("CompanyAccount");
                        paymentItem.IsVerficationRequested = dataReader.GetBooleanValue("isVerificationRequested");
                        paymentItem.IsVerficationPassed = dataReader.GetBooleanValue("isVerificationPassed");
                        paymentItem.IsVerficationFailed = dataReader.GetBooleanValue("isVerificationFailed");
                        paymentItem.CustomerFirstName = dataReader.GetStringValue("firstName");
                        paymentItem.CustomerLastName = dataReader.GetStringValue("lastName");
                        paymentItems.Add(paymentItem);
                    }
                }
                if (dataReader.NextResult())
                {
                    while (dataReader.Read())
                    {
                        res.TotalRecord = dataReader.GetIntegerValue("TotalRecords");
                    }
                }
            }
            res.Data = paymentItems;
            return res;
        }
        public async Task<int> AddECommercePayment(ECommercePayment eCommercePayment)
        {
            using var connection = base.GetConnection();

            var parameters = new DynamicParameters();
            parameters.Add("p_ProductIds", eCommercePayment.ProductIds);
            parameters.Add("p_CustomerId", eCommercePayment.CustomerId);
            parameters.Add("p_BasketId", eCommercePayment.BasketId);
            parameters.Add("p_MerchantId", eCommercePayment.MerchantId);
            parameters.Add("p_Amount", eCommercePayment.Amount);
            parameters.Add("p_Currency", eCommercePayment.Currency);
            parameters.Add("p_DelieveryMethodId", eCommercePayment.DelieveryMethodId);
            parameters.Add("p_EstDelieveryDate", eCommercePayment.EstDelieveryDate);
            parameters.Add("p_ShippingCost", eCommercePayment.ShippingCost);
            parameters.Add("p_DelieveryAddress", eCommercePayment.DelieveryAddress);
            parameters.Add("p_Status", eCommercePayment.Status);
            parameters.Add("p_CreatedBy", eCommercePayment.CreatedBy);
            parameters.Add("o_ECommercePaymentId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("AddECommercePaymentGb", parameters, commandType: CommandType.StoredProcedure);

            var onlinePaymentId = parameters.Get<int>("o_ECommercePaymentId");
            return onlinePaymentId;
        }
        public async Task<bool> UpdateECommercePayment(ECommercePayment eCommercePayment)
        {
            using var connection = base.GetConnection();

            var parameters = new DynamicParameters();
            parameters.Add("p_ProductIds", eCommercePayment.ProductIds);
            parameters.Add("p_ECommercePaymentId", eCommercePayment.ECommercePaymentId);
            parameters.Add("p_CustomerId", eCommercePayment.CustomerId);
            parameters.Add("p_BasketId", eCommercePayment.BasketId);
            parameters.Add("p_MerchantId", eCommercePayment.MerchantId);
            parameters.Add("p_Amount", eCommercePayment.Amount);
            parameters.Add("p_Currency", eCommercePayment.Currency);
            parameters.Add("p_DelieveryMethodId", eCommercePayment.DelieveryMethodId);
            parameters.Add("p_EstDelieveryDate", eCommercePayment.EstDelieveryDate);
            parameters.Add("p_ShippingCost", eCommercePayment.ShippingCost);
            parameters.Add("p_DelieveryAddress", eCommercePayment.DelieveryAddress);
            parameters.Add("p_Status", eCommercePayment.Status);
            parameters.Add("p_UpdatedBy", eCommercePayment.UpdatedBy);
            parameters.Add("o_IsUpdated", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("UpdateECommercePaymentGb", parameters, commandType: CommandType.StoredProcedure);

            var onlinePaymentId = parameters.Get<int>("o_IsUpdated");
            return onlinePaymentId > 0;
        }
        public async Task<int> VerifyECommercePayment(ECommercePayment eCommercePayment)
        {
            using var connection = base.GetConnection();

            var parameters = new DynamicParameters();
            parameters.Add("p_BasketId", eCommercePayment.BasketId);
            parameters.Add("p_transactionDetail", eCommercePayment.TransactionDetail);
            parameters.Add("p_transactionId", eCommercePayment.TransactionId);
            parameters.Add("p_BasketId", eCommercePayment.BasketId);
            parameters.Add("p_UpdatedBy", eCommercePayment.UpdatedBy);
            parameters.Add("o_IsUpdated", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("ConfirmECommercePaymentGb", parameters, commandType: CommandType.StoredProcedure);

            var onlinePaymentId = parameters.Get<int>("o_IsUpdated");
            return onlinePaymentId;
        }
        public async Task<ECommercePayment> GetECommercePaymentById(string basketId)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter("p_BasketId",basketId)
            };
            var result = new ECommercePayment();
            using (var dataReader = await base.ExecuteReader(parameters, "GetPaymentDetailsByBasketIdGb", CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        result.ECommercePaymentId = dataReader.GetIntegerValue("eCommercePaymentId");
                        result.ProductIds = dataReader.GetStringValue("productIds");
                        result.CustomerId = dataReader.GetIntegerValue("customerId");
                        result.BasketId = dataReader.GetStringValue("basketId");
                        result.MerchantId = dataReader.GetStringValue("merchantId");
                        result.Amount = dataReader.GetDecimalValue("amount");
                        result.Currency = dataReader.GetStringValue("currency");
                        result.DelieveryMethodId = dataReader.GetIntegerValue("delieveryMethodId");
                        result.EstDelieveryDate = dataReader.GetDateTimeValue("estDelieveryDate");
                        result.ShippingCost = dataReader.GetDecimalValue("shippingCost");
                        result.DelieveryAddress = dataReader.GetStringValue("delieveryAddress");
                        result.Status = dataReader.GetStringValue("status");
                        result.UpdatedBy = dataReader.GetIntegerValue("updatedBy");
                        result.CreatedAt = dataReader.GetDateTimeValue("createdAt");
                        result.UpdatedAt = dataReader.GetDateTimeValue("updatedAt");
                        result.UpdatedBy = dataReader.GetIntegerValue("updatedBy");
                        result.IsActive = dataReader.GetBooleanValue("isActive");
                        result.TransactionDetail = dataReader.GetStringValue("TransactionDetail");
                        result.TransactionId = dataReader.GetStringValue("TransactionId");
                    }
                }
            }
            return result;
        }
        public async Task<int> AddVendorPayment(AddVendorPaymentRequest paymentRM)
        {
            var response = 0;
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_VendorId", paymentRM.VendorId);
                parameters.Add("p_PaymentTypeId", paymentRM.PaymentTypeId);
                parameters.Add("p_Amount", paymentRM.Amount);
                parameters.Add("p_CreatedBy", paymentRM.CreatedBy);
                parameters.Add("p_VendorPaymentTypeId", paymentRM.VendorPaymentTypeId);
                parameters.Add("o_VendorPaymentId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                response = await connection.ExecuteAsync("InsertVendorPayment_Gb", parameters, transaction, commandType: CommandType.StoredProcedure);
                var vendorPaymentId = parameters.Get<int>("o_VendorPaymentId");

                await transaction.CommitAsync();
                response = vendorPaymentId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await connection.DisposeAsync();

            }
            return response;
        }
        public async Task<int> AddVendorOnlinePayment(AddVendorOnlinePaymentRequest paymentRM)
        {
            var response = 0;
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_VendorPaymentId", paymentRM.VendorPaymentId);
                parameters.Add("p_Amount", paymentRM.Amount);
                parameters.Add("p_TransactionId", paymentRM.TransactionId);
                parameters.Add("p_VendorAccountId", paymentRM.VendorAccountId);
                parameters.Add("p_CompanyAccountId", paymentRM.CompanyAccountId);
                parameters.Add("p_VendorAccountNumber", paymentRM.VendorAccountNumber);
                parameters.Add("P_CreatedBy", paymentRM.CreatedBy);
                parameters.Add("o_VendorOnlinePaymentId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync("InsertVendorOnlinePayment_Gb", parameters, transaction, commandType: CommandType.StoredProcedure);

                var onlinePaymentId = parameters.Get<int>("o_VendorOnlinePaymentId");
                response = onlinePaymentId;

                if (onlinePaymentId > 0)
                {
                    foreach (var paymentOrder in paymentRM.OnlinePaymentDocumentRM)
                    {
                        parameters = new DynamicParameters();
                        parameters.Add("p_VendorOnlinePaymentId", onlinePaymentId);
                        parameters.Add("p_DocumentId", paymentOrder.DocumentId);
                        parameters.Add("p_IsPrimary", paymentOrder.IsPrimary);
                        parameters.Add("p_CreatedBy", paymentRM.CreatedBy);

                        await connection.ExecuteAsync("InsertVendorOnlinePaymentDocument_Gb", parameters, transaction, commandType: CommandType.StoredProcedure);

                    }
                }
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await transaction.DisposeAsync();
            }
            return response;
        }
        public async Task<bool> ConfirmVendorPayment(ConfirmVendorPaymentRequest confirmPaymentRequest)
        {
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();
            bool response = false;
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_VendorPaymentId", confirmPaymentRequest.VendorPaymentId);
                parameters.Add("P_CashAmount", confirmPaymentRequest.CashAmount);
                parameters.Add("p_createdBy", confirmPaymentRequest.CreatedBy);
                parameters.Add("p_Notes", confirmPaymentRequest.Notes);
                parameters.Add("o_Success", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync("ConfirmVendorPayment_Gb", parameters, transaction, commandType: CommandType.StoredProcedure);
                var succeed = parameters.Get<int>("o_Success");
                response = succeed == 1;

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                await transaction.DisposeAsync();
            }
            return response;
        }
        public async Task<List<VendorPayment>> GetVendorPaymentsById(int vendorId)
        {
            var result = new List<VendorPayment>();
            var parameters = new List<DbParameter>
            {
                base.GetParameter("p_VendorId", vendorId)
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetPaymentById_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        var res = new VendorPayment();
                        res.VendorPaymentId = dataReader.GetIntegerValue("vendorPaymentId");
                        res.VendorId = dataReader.GetIntegerValue("vendorId");
                        res.PaymentTypeId = dataReader.GetIntegerValue("paymentTypeId");
                        res.VendorPaymentTypeId = dataReader.GetIntegerValue("vendorPaymentTypeId");
                        res.Amount = dataReader.GetDecimalValue("amount");
                        res.CashAmount = dataReader.GetDecimalValue("cashAmount");
                        res.IsConfirmed = dataReader.GetBooleanValue("isConfirmed");
                        res.CreatedBy = dataReader.GetIntegerValue("createdBy");
                        res.CreatedAt = dataReader.GetDateTimeValue("createdAt");
                    }
                }
            }
            return result;
        }
    }
}
