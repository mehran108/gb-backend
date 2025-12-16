using GoldBank.Application.IApplication;
using GoldBank.Models;
using GoldBank.Models.Product;
using GoldBank.Models.RequestModels;
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
        [HttpGet("GetAllPrimaryCategories")]
        public async Task<ActionResult<IEnumerable<PrimaryCategory>>> GetPrimaryCategories()
        {
            var result = await this.LookupApplication.GetPrimaryCategories();
            return Ok(result);
        }
        [HttpGet("GetAllCategories")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories([FromQuery] bool isDefault, bool isAppraisal)
        {
            var result = await this.LookupApplication.GetCategories(isDefault, isAppraisal);
            return Ok(result);
        }
        [HttpGet("GetAllSubCategories")]
        public async Task<ActionResult<IEnumerable<SubCategory>>> GetSubCategories()
        {
            var result = await this.LookupApplication.GetSubCategories();
            return Ok(result);
        }
        [HttpGet("GetAllStores")]
        public async Task<ActionResult<IEnumerable<SubCategory>>> GetAllStores()
        {
            var result = await this.LookupApplication.GetAllStores();
            return Ok(result);
        }
        [HttpGet("GetAllOrderTypes")]
        public async Task<ActionResult<IEnumerable<OrderType>>> GetAllOrderTypes()
        {
            var result = await this.LookupApplication.GetAllOrderTypes();
            return Ok(result);
        }
        [HttpGet("GetAllDeliveryMethods")]
        public async Task<ActionResult<IEnumerable<DelieveryMethod>>> GetAllDeliveryMethods()
        {
            var result = await this.LookupApplication.GetAllDeliveryMethods();
            return Ok(result);
        }
        [HttpGet("GetAllOrderStatus")]
        public async Task<ActionResult<IEnumerable<OrderStatus>>> GetAllOrderStatus()
        {
            var result = await this.LookupApplication.GetAllOrderStatus();
            return Ok(result);
        }
        [HttpGet("GetAllPaymentType")]
        public async Task<ActionResult<IEnumerable<PaymentType>>> GetAllPaymentType()
        {
            var result = await this.LookupApplication.GetAllPaymentType();
            return Ok(result);
        }
        [HttpGet("GetAllCustomerAccounts")]
        public async Task<ActionResult<IEnumerable<CustomerAccount>>> GetAllCustomerAccounts()
        {
            var result = await this.LookupApplication.GetAllCustomerAccounts();
            return Ok(result);
        }
        [HttpGet("GetAllCompanyAccounts")]
        public async Task<ActionResult<IEnumerable<CompanyAccount>>> GetAllCompanyAccounts()
        {
            var result = await this.LookupApplication.GetAllCompanyAccounts();
            return Ok(result);
        }
        [HttpGet("GetAllLacquerTypes")]
        public async Task<ActionResult<IEnumerable<LacquerType>>> GetAllLacquerTypes()
        {
            var result = await this.LookupApplication.GetAllLacquerTypes();
            return Ok(result);
        }
        [HttpGet("GetAllRepairDamageAreas")]
        public async Task<ActionResult<IEnumerable<RepairDamageArea>>> GetAllRepairDamageAreas()
        {
            var result = await this.LookupApplication.GetAllRepairDamageAreas();
            return Ok(result);
        }
        [HttpGet("GetAllRepairDamageTypes")]
        public async Task<ActionResult<IEnumerable<RepairDamageType>>> GetAllRepairDamageTypes()
        {
            var result = await this.LookupApplication.GetAllRepairDamageTypes();
            return Ok(result);
        }
        [HttpGet("GetAllRepairPolishing")]
        public async Task<ActionResult<IEnumerable<RepairPolishing>>> GetAllRepairPolishing()
        {
            var result = await this.LookupApplication.GetAllRepairPolishing();
            return Ok(result);
        }
        [HttpGet("GetAllRepairCleaning")]
        public async Task<ActionResult<IEnumerable<RepairCleaning>>> GetAllRepairCleaning()
        {
            var result = await this.LookupApplication.GetAllRepairCleaning();
            return Ok(result);
        }
        [HttpGet("GetAllExpiryDuration")]
        public async Task<ActionResult<IEnumerable<ExpiryDurationType>>> GetAllExpiryDuration()
        {
            var result = await this.LookupApplication.GetAllExpiryDuration();
            return Ok(result);
        }
        [HttpGet("GetAllDiscountType")]
        public async Task<ActionResult<IEnumerable<DiscountType>>> GetAllDiscountType()
        {
            var result = await this.LookupApplication.GetAllDiscountType();
            return Ok(result);
        }
        [HttpGet("GetAllVendorPaymentTypes")]
        public async Task<ActionResult<IEnumerable<VendorPaymentType>>> GetAllVendorPaymentTypes()
        {
            var result = await this.LookupApplication.GetAllVendorPaymentTypes();
            return Ok(result);
        }
        [HttpGet("GetAllCustomerCategories")]
        public async Task<ActionResult<IEnumerable<CustomerCategory>>> GetAllCustomerCategories()
        {
            var result = await this.LookupApplication.GetAllCustomerCategories();
            return Ok(result);
        }
        [HttpGet("GetAllVendorGoldPaymentTypes")]
        public async Task<ActionResult<IEnumerable<VendorGoldPaymentType>>> GetAllVendorGoldPaymentTypes()
        {
            var result = await this.LookupApplication.GetAllVendorGoldPaymentTypes();
            return Ok(result);
        }

        [HttpGet("GetAllLabels")]
        public async Task<ActionResult<IEnumerable<Label>>> GetAllLabels()
        {
            var result = await this.LookupApplication.GetAllLabels();
            return Ok(result);
        }

    }
}
