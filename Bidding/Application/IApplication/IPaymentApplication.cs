using GoldBank.Models;
using GoldBank.Models.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace GoldBank.Application.IApplication
{
    public interface IPaymentApplication : IBaseApplication<Payment>
    {
        Task<int> AddPayment(AddPaymentRequest paymentRM);
        Task<int> AddOnlinePayment(AddOnlinePaymentRequest paymentRM);
        Task<bool> VerifyOnlinePayment(VerifyOnlinePaymentRequest verifyOnlinePaymentRequest);
        Task<bool> ConfirmPayment(ConfirmPaymentRequest verifyOnlinePaymentRequest);
        Task<OnlinePaymentSummary> GetOnlinePaymentSummary();
        Task<bool?> CheckOnlinePaymentStatus(int onlinePaymentId);
        void CancelPayment(int paymentId);
        void CancelVendorPayment(int paymentId);
        void CancelOnlinePayment(int onlinePaymentId);
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
        Task<byte[]> GenerateInvoice(Invoice invoice);
    }
}
