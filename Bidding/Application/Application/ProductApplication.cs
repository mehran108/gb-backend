using GoldBank.Application.IApplication;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;

namespace GoldBank.Application.Application
{
    public class ProductApplication : IBaseApplication<Product>, IProductApplication
    {
        public ProductApplication(IProductInfrastructure ProductInfrastructure, IConfiguration configuration, ILogger<Product> logger)
        {
            this.ProductInfrastructure = ProductInfrastructure;
        }

        public IProductInfrastructure ProductInfrastructure { get;}
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
    }
}
