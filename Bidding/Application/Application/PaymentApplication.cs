using Amazon.Runtime.Internal.Transform;
using DinkToPdf;
using DinkToPdf.Contracts;
using GoldBank.Application.IApplication;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;
using GoldBank.Models.RequestModels;

namespace GoldBank.Application.Application
{
    public class PaymentApplication : IBaseApplication<Payment>, IPaymentApplication
    {
        private readonly string LogoURL;
        private readonly IConverter PdfConverter;

        public PaymentApplication(IPaymentInfrastructure PaymentInfrastructure, IDocumentApplication DocumentApplication, IConverter PdfConverter, IConfiguration configuration, ILookupInfrastructure LookupInfrastructure, ILogger<Payment> logger)
        {
            this.PaymentInfrastructure = PaymentInfrastructure;
            this.LookupInfrastructure = LookupInfrastructure;
            this.PdfConverter = PdfConverter;
            this.DocumentApplication = DocumentApplication;
            LogoURL = configuration["LogoURL"];
        }

        public IPaymentInfrastructure PaymentInfrastructure { get; }
        public ILookupInfrastructure LookupInfrastructure { get; }
        public IDocumentApplication DocumentApplication { get; }

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
        public async Task<OnlinePaymentSummary> GetOnlinePaymentSummary()
        {
            return await this.PaymentInfrastructure.GetOnlinePaymentSummary();
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
        public async Task<List<StoreCashManagementSummary>> GetAllCashManagementSummary(StoreCashManagementRequestVm request)
        {
            return await this.PaymentInfrastructure.GetAllCashManagementSummary(request);
        }
        public async Task<byte[]> GenerateInvoice(Invoice invoice)
        {
            var intemplates = await this.PaymentInfrastructure.GetAlInvoiceTemplates();
            string invoiceTemplate = intemplates.Where(x=> x.Code == "INVTMPL").Select(x=> x.Template).FirstOrDefault()??"";
            

            var tokens = BuildInvoiceHeaders(invoice, LogoURL);

            string finalHtml = PopulateTemplate(invoiceTemplate, tokens);

            var templates = await this.LookupInfrastructure.GetAllOrderTypes();

            string ordersSection = "";
            foreach (var order in invoice.Orders)
            {
                var template = templates.Where(x=> x.OrderTypeId == order.OrderTypeId).Select(x=> x.Template).FirstOrDefault();
                if (template != null)
                {
                    var orderTokens = BuildInvoiceTokens(order, LogoURL);
                    ordersSection += PopulateTemplate(template, orderTokens);
                }
            }

            finalHtml  = finalHtml.Replace("{{orderItems}}", ordersSection );

            var pdf = GeneratePdf(finalHtml);
            using var stream = new MemoryStream(pdf);

            IFormFile pdfFile = new FormFile(
                baseStream: stream,
                baseStreamOffset: 0,
                length: pdf.Length,
                name: "file",
                fileName: $"Invoice_{invoice.PaymentId}.pdf"
            )
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/pdf"
            };

            var document = new Document
            {
                File = pdfFile,
                Name = $"Invoice-for-{invoice.PaymentId}"
            };

            var documentId = await DocumentApplication.UploadImage(document);
            await this.PaymentInfrastructure.UpdatePaymentInvoiceURL(invoice.PaymentId, document.DocumentId);
            return pdf;

        }
        public byte[] GeneratePdf(string finalHtml)
        {
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings
                    {
                        Top = 0,
                        Bottom = 0,
                        Left = 0,
                        Right = 0
                    }
                },

                Objects =
        {
            new DinkToPdf.ObjectSettings
            {
                HtmlContent = finalHtml,
                WebSettings = new WebSettings
                {
                    DefaultEncoding = "utf-8"
                }
            }
        }
            };

            return PdfConverter.Convert(doc);
        }

        public static string PopulateTemplate(string template, Dictionary<string, string> values)
        {
            foreach (var item in values)
                template = template.Replace($"{{{{{item.Key}}}}}", item.Value ?? "");

            return template;
        }
        public static Dictionary<string, string> BuildInvoiceHeaders(Invoice order, string logoURL)
        {
            return new Dictionary<string, string>
        {
        // Header
        { "orderId", order.PaymentId.ToString() },
        { "time", DateTime.Now.ToString("hh:mm tt") },
        { "date", DateTime.Now.ToString("MMM dd yyyy ") },
        { "CustomerName", order.CustomerName },
        { "CustomerPhone", order.Phone },
        { "salesperson", order.CreatedBy },
        { "logoURL", logoURL },
        { "totalPayable", order.TransactionSummary?.GrandTotalPayable ?? ""},
        { "cashPaid", order.TransactionSummary?.CashPaid ?? ""},

        // Online Transfer
        { "companyBank", order.TransactionSummary?.OnlineTransfer?.CompanyBank ?? "" },
        { "customerBank", order.TransactionSummary?.OnlineTransfer?.CustomerBank ?? ""},
        { "customerAccount", order.TransactionSummary ?.OnlineTransfer ?.CustomerAccount ?? "" },
        { "transactionId", order.TransactionSummary ?.OnlineTransfer ?.TransactionId ?? "" },
        { "onlineAmount", order.TransactionSummary ?.OnlineTransfer ?.Amount ?? "" },
        
        { "cardCompanyAccount", order.TransactionSummary?.CardMachine?.CompanyBank ?? "" },
        { "last4Digit", order.TransactionSummary?.CardMachine?.Last4Digits ?? ""},
        { "cardTrxId", order.TransactionSummary ?.CardMachine ?.TransactionId ?? "" },
        { "cardAmount", order.TransactionSummary ?.CardMachine ?.Amount ?? "" }
            };
        }
        public static Dictionary<string, string> BuildInvoiceTokens(InvoiceOrder order, string logoURL)
        {
            return new Dictionary<string, string>
            {
                { "orderType", order.OrderType },

                { "ItemName", order.ItemDetails ?.ItemName ?? "" },
                { "sku", order.ItemDetails ?.SKU ?? "" },
                { "quantity", order.ItemDetails?.Quantity?.ToString()?? "" },
                { "productImage", order.ItemDetails?.Image?.ToString()?? "" },
                { "metalType", order.MetalDetails?.MetalType ?? ""},
                { "metalPurity", order.MetalDetails?.MetalPurity ?? ""},
                { "grossWeight", order.MetalDetails?.Weight?? "" },
                { "ratePerGram", order.MetalDetails?.RatePerGram ?? "" },
                { "metalValue", order.MetalDetails?.MetailValue ?? ""},
                { "itemMetalValue", order.ItemPricing?.MetalValue ?? ""},
                { "makingCharges", order.ItemPricing?.MakingCharges ?? ""},
                { "lacquerCharges", order.ItemPricing?.LacquerCharges ?? ""},
                { "gemstoneValue", order.ItemPricing?.GemStoneValue ?? ""},
                { "subTotalPrice", order.ItemPricing?.SubTotalPrice ?? ""},
                { "discountType", order.ItemPricing?.DiscountType ?? ""},
                { "discountAmount", order.ItemPricing?.DiscountAmount ?? ""},
                { "totalPrice", order.ItemPricing?.TotalPrice ?? ""},
                { "amountPayable", order.ItemPricing?.AmountPayable ?? ""},
                { "reservationId", order.ReservationDetails?.ReservationId ?? ""},
                { "reservationDate", order.ReservationDetails?.ReservationDate ?? ""},
                { "collectionDate", order.ReservationDetails?.CollectionDate ?? ""}
            };
        }

    }
}
