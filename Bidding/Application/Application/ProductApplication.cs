using GoldBank.Application.IApplication;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;
using GoldBank.Models.Product;

namespace GoldBank.Application.Application
{
    public class ProductApplication : IBaseApplication<Product>, IProductApplication
    {
        public ProductApplication(IProductInfrastructure ProductInfrastructure, IConfiguration configuration, ILogger<Product> logger, IDocumentApplication DocumentApplication)
        {
            this.ProductInfrastructure = ProductInfrastructure;
            DocumentApplication = DocumentApplication;
        }

        public IProductInfrastructure ProductInfrastructure { get;}
        public IDocumentApplication DocumentApplication { get;}
        public async Task<bool> Activate(Product entity)
        {
            return await ProductInfrastructure.Activate(entity);
        }

        public async Task<int> Add(Product entity)
        {
            return await ProductInfrastructure.Add(entity);
        }

        public async Task<Product> Get(Product entity)
        {
            return await ProductInfrastructure.Get(entity);
        }

        public async Task<List<Product>> GetList(Product entity)
        {
            return await ProductInfrastructure.GetList(entity);
        }

        public async Task<bool> Update(Product entity)
        {
            return await ProductInfrastructure.Update(entity);
        }
        public async Task<int> UploadImage(Document Document)
        {
            return await DocumentApplication.UploadImage(Document);            
        }
        public Task<AllResponse<Product>> GetAll(AllRequest<Product> entity)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> BulkImport(string fileContent, ProductBulkImport productBulkImport)
        {
            throw new NotImplementedException();
        }
        public async Task<AllResponse<Product>> GetAllProducts(AllRequest<ProductRequestVm> product)
        {
            return await ProductInfrastructure.GetAllProducts(product);
        }
        public async Task<Product> GetProductById(int productId)
        {
            return await ProductInfrastructure.GetProductById(productId);
        }
    }
}
