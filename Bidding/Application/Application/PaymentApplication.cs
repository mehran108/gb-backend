using GoldBank.Application.IApplication;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;
using GoldBank.Models.Product;
using GoldBank.Models.RequestModels;

namespace GoldBank.Application.Application
{
    public class PaymentApplication : IBaseApplication<Payment>, IPaymentApplication
    {
        public PaymentApplication(IPaymentInfrastructure PaymentInfrastructure, IConfiguration configuration, ILogger<Payment> logger)
        {
            this.PaymentInfrastructure = PaymentInfrastructure;
        }

        public IPaymentInfrastructure PaymentInfrastructure { get; }

        public Task<bool> Activate(Payment entity)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Add(Payment entity)
        {
            return await this.PaymentInfrastructure.Add(entity);
        }

        public async Task<Payment> Get(Payment entity)
        {
            return await this.PaymentInfrastructure.Get(entity);
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
        public async Task<AllResponse<OnlinePaymentVerificationVM>> GetAllOnlinePayments(AllRequest<OnlinePaymentVerificationRM> Payment)
        {
            return await this.PaymentInfrastructure.GetAllOnlinePayments(Payment);
        }
        public async Task<int> AddPayment(AddPaymentRequest paymentRM)
        {
            return await this.PaymentInfrastructure.AddPayment(paymentRM);
        }
        public async Task<int> AddOnlinePayment(AddOnlinePaymentRequest paymentRM)
        {
            return await this.PaymentInfrastructure.AddOnlinePayment(paymentRM);
        }
        public async Task<bool> VerifyOnlinePayment(VerifyOnlinePaymentRequest verifyOnlinePaymentRequest)
        {
            return await this.PaymentInfrastructure.VerifyOnlinePayment(verifyOnlinePaymentRequest);
        }
        public async Task<bool> ConfirmPayment(ConfirmPaymentRequest confirmPaymentRequest)
        {
            return await this.PaymentInfrastructure.ConfirmPayment(confirmPaymentRequest);
        }
        public async Task<bool?> CheckOnlinePaymentStatus(int onlinePaymentId)
        {
            return await this.PaymentInfrastructure.CheckOnlinePaymentStatus(onlinePaymentId);
        }
        public async void CancelPayment(int paymentId)
        {
            this.PaymentInfrastructure.CancelPayment(paymentId);
        }
        public async void CancelVendorPayment(int VendorPaymentId)
        {
            this.PaymentInfrastructure.CancelVendorPayment(VendorPaymentId);
        }
        public async void CancelOnlinePayment(int onlinePaymentId)
        {
            this.PaymentInfrastructure.CancelOnlinePayment(onlinePaymentId);
        }
        public async Task<int> AddECommercePayment(ECommercePayment eCommercePayment)
        {
            return await this.PaymentInfrastructure.AddECommercePayment(eCommercePayment);
        }
        public async Task<int> VerifyECommercePayment(ECommercePayment eCommercePayment)
        {
            return await this.PaymentInfrastructure.VerifyECommercePayment(eCommercePayment);
        }
        public async Task<ECommercePayment> GetECommercePaymentById(string basketId)
        {
            return await this.PaymentInfrastructure.GetECommercePaymentById(basketId);
        }
        public async Task<bool> UpdateECommercePayment(ECommercePayment eCommercePayment)
        { 
            return await this.PaymentInfrastructure.UpdateECommercePayment(eCommercePayment);
        }
        public async Task<int> AddVendorPayment(AddVendorPaymentRequest paymentRM)
        {
            return await this.PaymentInfrastructure.AddVendorPayment(paymentRM);
        }
        public async Task<int> AddVendorOnlinePayment(AddVendorOnlinePaymentRequest paymentRM)
        {
            return await this.PaymentInfrastructure.AddVendorOnlinePayment(paymentRM);
        }
        public async Task<bool> ConfirmVendorPayment(ConfirmVendorPaymentRequest confirmPaymentRequest)
        {
            return await this.PaymentInfrastructure.ConfirmVendorPayment(confirmPaymentRequest);
        }
        public async Task<List<VendorPayment>> GetVendorPaymentsById(int vendorId)
        {
            return await this.PaymentInfrastructure.GetVendorPaymentsById(vendorId);
        }
        public async Task<int> AddCashManagementDetail(CashManagementDetails entity)
        {
            return await this.PaymentInfrastructure.AddCashManagementDetail(entity);
        }
        public async Task<int> CancelCashWidrawAmount(int Id, int UserId)
        {
            return await this.PaymentInfrastructure.CancelCashWidrawAmount(Id,UserId);
        }
        public async Task<CashManagementSummary> GetCashManagementSummary()
        {
            return await this.PaymentInfrastructure.GetCashManagementSummary();
        }
    }
}
