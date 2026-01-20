using Dapper;
using GoldBank.Infrastructure.Extension;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;
using GoldBank.Models.Product;
using GoldBank.Models.RequestModels;
using System;
using System.Data;
using System.Data.Common;

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
                        parameters.Add("p_CompanyAccountId", cardPay.CompanyAccountId ?? 0);
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
        public async Task<OnlinePaymentSummary> GetOnlinePaymentSummary()
        {
            var res = new OnlinePaymentSummary();
            var parameters = new List<DbParameter>
            {

            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetAllOnlinePaymentSummary_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        res.Rejected = dataReader.GetIntegerValue("rejected");
                        res.Passed = dataReader.GetIntegerValue("passed");
                        res.Pending = dataReader.GetIntegerValue("pending");
                    }
                }
            }
            return res;
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
        public async void CancelVendorPayment(int vendorPaymentId)
        {
            using var connection = base.GetConnection();

            var parameters = new DynamicParameters();
            parameters.Add("p_VendorPaymentId", vendorPaymentId);
            await connection.ExecuteAsync("CancelVendorPaymentGb", parameters, commandType: CommandType.StoredProcedure);
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
                 base.GetParameter("p_SearchText",Payment.SearchText),
                 base.GetParameter("p_CustomerId", Payment.Data.CustomerId),
                 base.GetParameter("p_IsVerificationRequested", Payment.Data.IsVerificationRequested),
                 base.GetParameter("p_IsVerificationFailed", Payment.Data.IsVerificationFailed),
                 base.GetParameter("p_IsVerificationPassed",Payment.Data.IsVerificationPassed)
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
                        paymentItem.CreatedAt = dataReader.GetDateTimeValue("createdAt");
                        paymentItem.CreatedBy = dataReader.GetIntegerValue("createdBy");
                        paymentItem.UserName = dataReader.GetStringValue("UserName");
                        paymentItem.BranchName = dataReader.GetStringValue("branchName");
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
                parameters.Add("p_GoldAmount", paymentRM.GoldAmount);
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
                        parameters.Add("p_VendorPaymentId", onlinePaymentId);
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
                parameters.Add("p_VendorGoldPaymentTypeId", confirmPaymentRequest.VendorGoldPaymentTypeId);
                parameters.Add("p_ProductId", confirmPaymentRequest.ProductId);
                parameters.Add("p_GoldAmount", confirmPaymentRequest.GoldAmount);
                parameters.Add("o_Success", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync("ConfirmVendorPayment_Gb", parameters, transaction, commandType: CommandType.StoredProcedure);
                var succeed = parameters.Get<int>("o_Success");
                response = succeed == 1;
                if (response && confirmPaymentRequest.PaymentDocumentRM?.Count > 0)
                {
                    foreach (var paymentOrder in confirmPaymentRequest.PaymentDocumentRM)
                    {
                        parameters = new DynamicParameters();
                        parameters.Add("p_VendorOnlinePaymentId", paymentOrder.VendorOnlinePaymentId);
                        parameters.Add("p_VendorPaymentId", paymentOrder.VendorPaymentId);
                        parameters.Add("p_DocumentId", paymentOrder.DocumentId);
                        parameters.Add("p_IsPrimary", paymentOrder.IsPrimary);
                        parameters.Add("p_CreatedBy", confirmPaymentRequest.CreatedBy);

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
        public async Task<List<VendorPayment>> GetVendorPaymentsById(int vendorId)
        {
            var result = new List<VendorPayment>();
            var parameters = new List<DbParameter>
            {
                base.GetParameter("p_VendorId", vendorId)
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetAllVendorPayments_Gb", CommandType.StoredProcedure))
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
                        res.GoldAmount = dataReader.GetDecimalValue("goldAmount");
                        res.VendorGoldPaymentTypeId = dataReader.GetIntegerValueNullable("vendorGoldPaymentTypeId");
                        res.CashAmount = dataReader.GetDecimalValue("cashAmount");
                        res.IsConfirmed = dataReader.GetBooleanValue("isConfirmed");
                        res.CreatedBy = dataReader.GetIntegerValue("createdBy");
                        res.CreatedAt = dataReader.GetDateTimeValue("createdAt");
                        res.VendorOnlinePayments = new List<AddVendorOnlinePaymentRequest>();
                        res.PaymentDocument = new List<VendorPaymentDocument>();
                        result.Add(res);
                    }
                }
                if (dataReader.NextResult())
                {
                    while (dataReader.Read())
                    {
                        var row = new AddVendorOnlinePaymentRequest();
                        row.VendorPaymentId = dataReader.GetIntegerValue("vendorPaymentId");
                        row.VendorOnlinePaymentId = dataReader.GetIntegerValue("vendorOnlinePaymentId");
                        row.Amount = dataReader.GetDecimalValue("amount");
                        row.TransactionId = dataReader.GetStringValue("transactionId");
                        row.VendorAccountNumber = dataReader.GetStringValue("vendorAccountNumber");
                        row.VendorAccountId = dataReader.GetIntegerValue("vendorAccountId");
                        row.CompanyAccountId = dataReader.GetIntegerValue("companyAccountId");
                        row.CreatedBy = dataReader.GetIntegerValue("createdBy");
                        var onlinePaymentItem = result.FirstOrDefault(x => x.VendorPaymentId == row.VendorPaymentId);
                        if (onlinePaymentItem != null)
                        {
                            onlinePaymentItem.VendorOnlinePayments.Add(row);
                        }
                    }
                }
            if (dataReader.NextResult())
            {
                while (dataReader.Read())
                {
                    var item = new VendorPaymentDocument();
                    item.VendorPaymentId = dataReader.GetIntegerValue("vendorPaymentId");
                    item.VendorPaymentDocumentId = dataReader.GetIntegerValue("vendorPaymentDocumentId");
                    item.DocumentId = dataReader.GetIntegerValue("documentId");
                    item.Url = dataReader.GetStringValue("url");
                    item.IsPrimary = dataReader.GetBooleanValue("isPrimary");
                    item.CreatedBy = dataReader.GetIntegerValue("createdBy");
                    var onlinePaymentDocumentItem = result.FirstOrDefault(x => x.VendorPaymentId == item.VendorPaymentId);
                    if (onlinePaymentDocumentItem != null)
                    {
                        onlinePaymentDocumentItem.PaymentDocument.Add(item);
                    }
                }
            }
        }
            return result;
        }

        private static string GenerateTransactionId()
        {
            string datePart = DateTime.Now.ToString("yyMMddHH");

            Random rnd = new Random();
            string randomPart = rnd.Next(0, 100).ToString("D2");

            return datePart + randomPart;
        }

        public async Task<int> AddCashManagementDetail(CashManagementDetails entity)
        {
            var response = 0;
            entity.TransactionId = GenerateTransactionId();
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("p_CompanyAccountId", entity.CompanyAccountId);
                parameters.Add("p_StoreId", entity.StoreId);
                parameters.Add("p_Amount", entity.Amount);
                parameters.Add("p_IsWithdraw", entity.IsWithdraw);
                parameters.Add("p_IsAddCash", entity.IsAddCash);
                parameters.Add("p_Notes", entity.Notes);
                parameters.Add("p_CreatedBy", entity.CreatedBy);
                parameters.Add("p_TransactionId", entity.TransactionId);
                parameters.Add("o_CashManagementDetailId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync("AddCashManagementDetailGb", parameters, transaction, commandType: CommandType.StoredProcedure);

                var cashManagementDetailId = parameters.Get<int>("o_CashManagementDetailId");
                response = cashManagementDetailId;

                if (cashManagementDetailId > 0)
                {
                    foreach (var paymentOrder in entity.CashManagementDetailDocuments)
                    {
                        parameters = new DynamicParameters();
                        parameters.Add("p_CashManagementDetailId", cashManagementDetailId);
                        parameters.Add("p_DocumentId", paymentOrder.DocumentId);
                        parameters.Add("p_IsPrimary", paymentOrder.IsPrimary);
                        parameters.Add("p_CreatedBy", entity.CreatedBy);

                        await connection.ExecuteAsync("AddOrUpdateCashManagementDocument", parameters, transaction, commandType: CommandType.StoredProcedure);
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
        private object ToDbValue(object? value)
        {
            if (value == null) return DBNull.Value;

            switch (value)
            {
                case int intVal when intVal <= 0:
                    return DBNull.Value;

                case decimal decVal when decVal <= 0:
                    return DBNull.Value;

                case string strVal when string.IsNullOrWhiteSpace(strVal):
                    return DBNull.Value;

                default:
                    return value;
            }
        }
        public async Task<int> CancelCashWidrawAmount(int Id,int UserId)
        {
            var response = 0;
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("p_CashManagementDetailId", Id);
                parameters.Add("p_UpdatedBy", UserId);
                parameters.Add("o_IsUpdated", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync("DeleteCashManagementDetailGb", parameters, transaction, commandType: CommandType.StoredProcedure);

                var cashManagementDetailId = parameters.Get<int>("o_IsUpdated");
                response = cashManagementDetailId;

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
        public async Task<CashManagementSummary> GetCashManagementSummary()
        {
            var result = new CashManagementSummary();
            var Stores = new List<Store>();
            var Banks = new List<CompanyAccount>();
            var parameters = new List<DbParameter>
            {
                //base.GetParameter("p_VendorId", vendorId)
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetCashManagementSummaryGb", CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        var store = new Store();
                        store.StoreId = dataReader.GetIntegerValue("storeId");
                        store.Description = dataReader.GetStringValue("description");
                        store.AvailableCash = dataReader.GetDecimalValue("AvailableCash");
                        store.InFlows = dataReader.GetDecimalValue("InFlows");
                        store.OutFlows = dataReader.GetDecimalValue("OutFlows");
                        Stores.Add(store);
                    }
                }
                if (dataReader.NextResult())
                {
                    while (dataReader.Read())
                    {
                        var bank = new CompanyAccount();
                        bank.CompanyAccountId = dataReader.GetIntegerValue("companyAccountId");
                        bank.Description = dataReader.GetStringValue("description");
                        bank.CurrentBalance = dataReader.GetDecimalValue("CurrentBalance");
                        bank.InFlows = dataReader.GetDecimalValue("InFlows");
                        bank.OutFlows = dataReader.GetDecimalValue("OutFlows");
                        Banks.Add(bank);
                    }
                }
                if (dataReader.NextResult())
                {
                    while (dataReader.Read())
                    {
                        result.TotalBankBalance = dataReader.GetDecimalValue("TotalBankBalance");
                        result.TotalPhysicalCash = dataReader.GetDecimalValue("TotalPhysicalCash");
                        result.TotalAvailableCash = dataReader.GetDecimalValue("TotalAvailableCash");
                    }
                }           
            }
            result.BankDetails = Banks;
            result.BranchDetails = Stores;
            return result;
        }
        public async Task<List<StoreCashManagementSummary>> GetAllCashManagementSummary(StoreCashManagementRequestVm request)
        {
            var Summary = new List<StoreCashManagementSummary>();
            var parameters = new List<DbParameter>
            {
                base.GetParameter("@p_FromDate", ToDbValue(request.FromDate)),
                base.GetParameter("@p_ToDate", ToDbValue(request.ToDate))
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetAllCashManagementSummary", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        var item = new StoreCashManagementSummary();
                        item.TransactionId = dataReader.GetStringValue("transactionId");
                        item.Date = dataReader.GetDateTimeValue("date");
                        item.Store = dataReader.GetStringValue("store");
                        item.TransferType = dataReader.GetStringValue("transferType");
                        item.Source = dataReader.GetStringValue("source");
                        item.Destination = dataReader.GetStringValue("destination");
                        item.Amount = dataReader.GetDecimalValue("amount");
                        item.IsCredit = dataReader.GetBooleanValue("isCredit");
                        item.IsDebit = dataReader.GetBooleanValue("isDebit");
                        item.StoreId = dataReader.GetIntegerValue("storeId");
                        item.IsVendorPayment = dataReader.GetBooleanValueNullable("isVendorPayment");
                        item.PaymentId = dataReader.GetIntegerValueNullable("paymentId");
                        item.OrderIds = dataReader.GetStringValue("OrderIds");
                        Summary.Add(item);
                    }         
                }
                //if (dataReader.NextResult())
                //{
                //    if (dataReader.Read())
                //    {
                //        Response = dataReader.GetIntegerValue("totalRecords");
                //    }
                //}
            }
            return Summary;
        }       
        public async Task<List<OrderInvoiceTemplate>> GetAlInvoiceTemplates()
        {
            var result = new List<OrderInvoiceTemplate>();
            var parameters = new List<DbParameter>
            {
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetAlInvoiceTemplates", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        var item = new OrderInvoiceTemplate();
                        item.Code = dataReader.GetStringValue("code");
                        item.Template = dataReader.GetStringValue("template");
                        result.Add(item);
                    }
                }
            }

            return result;
        }
        public async Task<bool> UpdatePaymentInvoiceURL(Invoice Invoice)
        {
            using var connection = base.GetConnection();

            var parameters = new DynamicParameters();
            parameters.Add("p_PaymentId", Invoice.PaymentId);
            parameters.Add("p_Url", Invoice.Url);
            parameters.Add("o_Succeed", dbType: DbType.Byte, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("UpdatePaymentInvoiceURLGb", parameters, commandType: CommandType.StoredProcedure);
            byte? succeed = parameters.Get<byte?>("o_Succeed");
            bool? response = succeed == null ? null : (succeed == 1 ? true : false);
            return response ?? false;
        }
    }
}
