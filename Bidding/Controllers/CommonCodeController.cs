using GoldBank.Application.IApplication;
using GoldBank.Models;
using Microsoft.AspNetCore.Mvc;

namespace GoldBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonCodeController : ControllerBase
    {
        public ICommonCodeApplication CommonCodeApplication { get; }
        public ILogger logger { get; set; }
        public CommonCodeController(IConfiguration configuration, ILogger<CommonCodeController> logger, ICommonCodeApplication CommonCodeApplication)
        {
            this.CommonCodeApplication = CommonCodeApplication;
            this.logger = logger;
        }


        [HttpPost("Add")]
        public async Task<int> Add(CommonCode CommonCode)
        {
            return await CommonCodeApplication.Add(CommonCode);
        }

        [HttpPost("Update")]
        public async Task<bool> Update(CommonCode CommonCode)
        {
            return await CommonCodeApplication.Update(CommonCode);
        }
        [HttpGet("Get")]
        public async Task<CommonCode> GetById([FromQuery] int CommonCodeId)
        {
            var CommonCode = new CommonCode { CommonCodeId = CommonCodeId };
            return await CommonCodeApplication.Get(CommonCode);
        }
        [HttpPost("UploadImage")]
        public async Task<int> UploadImage(CommonCode commonCode)
        {
            return await CommonCodeApplication.UploadImage(commonCode);
        }
    }
}
