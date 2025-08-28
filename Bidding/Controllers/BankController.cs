using GoldBank.Application.IApplication;
using GoldBank.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoldBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : ControllerBase
    {
        public ILogger logger { get; set; }
        public IBankApplication BankApplication { get; set; }
        public BankController(IConfiguration configuration, ILogger<BankController> logger, IBankApplication BankApplication, IDocumentApplication DocumentApplication)
        {
            this.BankApplication = BankApplication;
            this.logger = logger;
        }

        [HttpPost("Add")]
        public async Task<int> Add(CompanyAccount companyAccount)
        {
            return await this.BankApplication.Add(companyAccount);
        }

        [HttpPost("Update")]
        public async Task<bool> Update(CompanyAccount companyAccount)
        {
            return await this.BankApplication.Update(companyAccount);
        }

        [HttpGet("Get")]
        public async Task<CompanyAccount> GetById([FromQuery] int companyAccountId)
        {
            var companyAccount = new CompanyAccount { CompanyAccountId = companyAccountId };
            return await this.BankApplication.Get(companyAccount);
        }

        [HttpPost("GetAll")]
        public async Task<List<CompanyAccount>> GetAll()
        {
            var companyAccount = new CompanyAccount();
            return await this.BankApplication.GetList(companyAccount);
        }
    }
}
