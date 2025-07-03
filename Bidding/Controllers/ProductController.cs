using ExcelDataReader;
using GoldBank.Application.Application;
using GoldBank.Application.IApplication;
using GoldBank.Models;
using GoldBank.Models.Product;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text;

namespace GoldBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        public IProductApplication ProductApplication { get; }
        public IDocumentApplication DocumentApplication { get; }
        public ILogger logger { get; set; }
        public ProductController(IConfiguration configuration, ILogger<ProductController> logger, IProductApplication productApplication, IDocumentApplication DocumentApplication)
        {
            this.ProductApplication = productApplication;
            this.logger = logger;
            this.DocumentApplication = DocumentApplication;
        }


        [HttpPost("Add")]
        public async Task<int> Add(Product product)
        {
            return await ProductApplication.Add(product);
        }

        [HttpPost("Update")]
        public async Task<bool> Update(Product product)
        {
            return await ProductApplication.Update(product);
        }
        [HttpGet("Get")]
        public async Task<Product> GetById([FromQuery] int productId)
        {
            var product = new Product { ProductId = productId };
            return await ProductApplication.Get(product);
        }
        [HttpPost("GetAll")]
        public async Task<AllResponse<Product>> GetList([FromBody] AllRequest<Product> product)
        {
            return await ProductApplication.GetAll(product);
        }
        [HttpPost("UploadImage")]
        public async Task<int> UploadImage(Document Document)
        {
            return await ProductApplication.UploadImage(Document);
        }
        [HttpPost("BulkImport")]
        public async Task<bool> BulkImport(ProductBulkImport productBulkImport)
        {
            using (var streamReader = new StreamReader(productBulkImport.File.OpenReadStream()))
            {
                var fileContent = "";

                if (productBulkImport.File.FileName.EndsWith(".csv"))
                {
                    fileContent = await streamReader.ReadToEndAsync();
                }
                if (productBulkImport.File.FileName.EndsWith(".xlsx") || productBulkImport.File.FileName.EndsWith(".xls"))
                {
                    var reader = ExcelReaderFactory.CreateReader(productBulkImport.File.OpenReadStream());
                    var spreadsheet = reader.AsDataSet();
                    var table = spreadsheet.Tables[0];
                    fileContent = await this.ToCSV(table);
                    //var bytes = Encoding.UTF8.GetBytes(csv);
                }
                return await ProductApplication.BulkImport(fileContent, productBulkImport);
            }
        }
        private async Task<string> ToCSV(DataTable dtDataTable)
        {
            var builder = new StringBuilder();
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(","))
                        {
                            value = string.Format("\"{0}\"", value);
                            builder.Append(value);
                        }
                        else
                        {
                            builder.Append(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        builder.Append(",");
                    }
                }
                builder.Append(Environment.NewLine);
            }

            var csv = builder.ToString();
            return csv;
        }
    }
}
