using GoldBank.Application.IApplication;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;
using GoldBank.Models.Product;

namespace GoldBank.Application.Application
{
    public class ProductApplication : IBaseApplication<ProductGb>, IProductApplication
    {
        public ProductApplication(IProductInfrastructure ProductInfrastructure, IConfiguration configuration, ILogger<ProductGb> logger, ICommonCodeApplication commonCodeApplication)
        {
            this.ProductInfrastructure = ProductInfrastructure;
            CommonCodeApplication = commonCodeApplication;
        }

        public IProductInfrastructure ProductInfrastructure { get;}
        public ICommonCodeApplication CommonCodeApplication { get;}
        public async Task<bool> Activate(ProductGb entity)
        {
            return await ProductInfrastructure.Activate(entity);
        }

        public async Task<int> Add(ProductGb entity)
        {
            return await ProductInfrastructure.Add(entity);
        }

        public async Task<ProductGb> Get(ProductGb entity)
        {
            return await ProductInfrastructure.Get(entity);
        }

        public async Task<List<ProductGb>> GetList(ProductGb entity)
        {
            return await ProductInfrastructure.GetList(entity);
        }

        public async Task<bool> Update(ProductGb entity)
        {
            return await ProductInfrastructure.Update(entity);
        }
        public async Task<int> UploadImage(CommonCode commonCode)
        {
            return await CommonCodeApplication.UploadImage(commonCode);            
        }

    }
}
