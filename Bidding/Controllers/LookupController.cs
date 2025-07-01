using GoldBank.Application.IApplication;
using GoldBank.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace GoldBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupController : ControllerBase
    {
        public LookupController(ILookupApplication lookupApplication, IConfiguration Configuration, ILogger<LookupController> logger) 
        {
            LookupApplication = lookupApplication;
        }

        #region Properties and Data Members
        public ILookupApplication LookupApplication { get; set; }

        #endregion

        [HttpPost("add")]
        //[AllowAnonymous]
        public async Task<int> Add([FromBody] LookupValue entity)
        {

            var Id = await this.LookupApplication.Add(entity);
            return Id;
        }
        [HttpPost("GetLookupByCode")]
        //[AllowAnonymous]
        public async Task<List<LookupValue>> GetLookupByCode([FromBody] LookupValue entity)
        {
            return await this.LookupApplication.GetLookupByCode(entity);
        }
    }
}
