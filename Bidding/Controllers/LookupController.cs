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

    }
}
