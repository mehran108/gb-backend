using GoldBank.Application.IApplication;
using GoldBank.Models;
using GoldBank.Models.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace GoldBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
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
        [HttpGet("GetAllProductTypes")]
        public async Task<ActionResult<IEnumerable<ProductType>>> GetProductTypes()
        {
            var result = await this.LookupApplication.GetAllProductTypeGbAsync();
            return Ok(result);
        }

        [HttpGet("GetAllGenderTypes")]
        public async Task<ActionResult<IEnumerable<GenderType>>> GetGenderTypes()
        {
            var result = await this.LookupApplication.GetAllGenderTypeGbAsync();
            return Ok(result);
        }

        [HttpGet("GetAllProductSources")]
        public async Task<ActionResult<IEnumerable<ProductSource>>> GetProductSources()
        {
            var result = await this.LookupApplication.GetAllProductSourceGbAsync();
            return Ok(result);
        }

        [HttpGet("GetAllVendors")]
        public async Task<ActionResult<IEnumerable<Vendor>>> GetVendors()
        {
            var result = await this.LookupApplication.GetAllVendorGbAsync();
            return Ok(result);
        }

        [HttpGet("GetAllMetalTypes")]
        public async Task<ActionResult<IEnumerable<MetalType>>> GetMetalTypes()
        {
            var result = await this.LookupApplication.GetAllMetalTypeGbAsync();
            return Ok(result);
        }

        [HttpGet("GetAllMetalPurities")]
        public async Task<ActionResult<IEnumerable<MetalPurity>>> GetMetalPurities()
        {
            var result = await this.LookupApplication.GetAllMetalPurityGbAsync();
            return Ok(result);
        }

        [HttpGet("GetAllMetalColors")]
        public async Task<ActionResult<IEnumerable<MetalColor>>> GetMetalColors()
        {
            var result = await this.LookupApplication.GetAllMetalColorGbAsync();
            return Ok(result);
        }

        [HttpGet("GetAllWeightTypes")]
        public async Task<ActionResult<IEnumerable<WeightType>>> GetWeightTypes()
        {
            var result = await this.LookupApplication.GetAllWeightTypeGbAsync();
            return Ok(result);
        }

        [HttpGet("GetAllStoneTypes")]
        public async Task<ActionResult<IEnumerable<StoneType>>> GetStoneTypes()
        {
            var result = await this.LookupApplication.GetAllStoneTypeGbAsync();
            return Ok(result);
        }

        [HttpGet("GetAllStoneWeightTypes")]
        public async Task<ActionResult<IEnumerable<StoneWeightType>>> GetStoneWeightTypes()
        {
            var result = await this.LookupApplication.GetAllStoneWeightTypeGbAsync();
            return Ok(result);
        }

        [HttpGet("GetAllStoneShapes")]
        public async Task<ActionResult<IEnumerable<StoneShape>>> GetStoneShapes()
        {
            var result = await this.LookupApplication.GetAllStoneShapeGbAsync();
            return Ok(result);
        }

        [HttpGet("GetAllWearingTypes")]
        public async Task<ActionResult<IEnumerable<WearingType>>> GetWearingTypes()
        {
            var result = await this.LookupApplication.GetAllWearingTypeGbAsync();
            return Ok(result);
        }

        [HttpGet("GetAllCollections")]
        public async Task<ActionResult<IEnumerable<Collection>>> GetCollections()
        {
            var result = await this.LookupApplication.GetAllCollectionGbAsync();
            return Ok(result);
        }

        [HttpGet("GetAllOccasions")]
        public async Task<ActionResult<IEnumerable<Occasion>>> GetOccasions()
        {
            var result = await this.LookupApplication.GetAllOccasionGbAsync();
            return Ok(result);
        }
    }
}
