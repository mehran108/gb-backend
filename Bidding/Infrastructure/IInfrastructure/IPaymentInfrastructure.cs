using GoldBank.Models;
using GoldBank.Models.RequestModels;

namespace GoldBank.Infrastructure.IInfrastructure
{
    public interface IPaymentInfrastructure : IBaseInfrastructure<Payment>
    {
        Task<int> AddPayment(AddPaymentRequest paymentRM);
        Task<int> AddOnlinePayment(AddOnlinePaymentRequest paymentRM);
        Task<bool> VerifyOnlinePayment(VerifyOnlinePaymentRequest verifyOnlinePaymentRequest);
        Task<bool> ConfirmPayment(ConfirmPaymentRequest confirmPaymentRequest);
        Task<OnlinePaymentSummary> GetOnlinePaymentSummary();
        Task<bool?> CheckOnlinePaymentStatus(int onlinePaymentId);
        void CancelOnlinePayment(int onlinePaymentId);
        void CancelPayment(int paymentId);
        void CancelVendorPayment(int VendorPaymentId);
        Task<AllResponse<OnlinePaymentVerificationVM>> GetAllOnlinePayments(AllRequest<OnlinePaymentVerificationRM> Payment);
        Task<int> AddECommercePayment(ECommercePayment eCommercePayment);
        Task<int> VerifyECommercePayment(ECommercePayment eCommercePayment);
        Task<ECommercePayment> GetECommercePaymentById(string basketId);
        Task<bool> UpdateECommercePayment(ECommercePayment eCommercePayment);
        Task<int> AddVendorPayment(AddVendorPaymentRequest paymentRM);
        Task<int> AddVendorOnlinePayment(AddVendorOnlinePaymentRequest paymentRM);
        Task<bool> ConfirmVendorPayment(ConfirmVendorPaymentRequest confirmPaymentRequest);
        Task<List<VendorPayment>> GetVendorPaymentsById(int vendorId);
        Task<int> AddCashManagementDetail(CashManagementDetails entity);
        Task<int> CancelCashWidrawAmount(int Id, int UserId);
        Task<CashManagementSummary> GetCashManagementSummary();
        Task<List<StoreCashManagementSummary>> GetAllCashManagementSummary(StoreCashManagementRequestVm request);
    }
}
