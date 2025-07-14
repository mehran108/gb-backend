using Dapper;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;
using GoldBank.Models.Product;
using GoldBank.Models.RequestModels;
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
        public async Task<int> AddPayment(PaymentRM paymentRM)
        {
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();

            var parameters = new DynamicParameters();
            parameters.Add("p_CustomerId", paymentRM.CustomerId);
            parameters.Add("p_PaymentTypeId", paymentRM.PaymentTypeId);
            parameters.Add("p_TotalAmount", paymentRM.TotalAmount);
            parameters.Add("p_CashPayment", paymentRM.CashPayment);
            parameters.Add("p_IsConfirmed", paymentRM.IsConfirmed);
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
                        parameters.Add("p_PaymentId", paymentId);
                        parameters.Add("p_OrderId", paymentOrder.OrderId);
                        parameters.Add("p_TotalAmount", paymentOrder.TotalAmount);
                        parameters.Add("p_CreatedBy", paymentRM.CreatedBy);
                        parameters.Add("o_PaymentOrderId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                        await connection.ExecuteAsync("InsertPaymentOrderGb", parameters, transaction, commandType: CommandType.StoredProcedure);
                    }
                }
            }
            await transaction.CommitAsync();
            return newPaymentId;
        }
    }
}
