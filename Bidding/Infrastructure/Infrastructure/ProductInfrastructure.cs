using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;

namespace GoldBank.Infrastructure.Infrastructure
{
    public class ProductInfrastructure : BaseInfrastructure, IProductInfrastructure
    {
        public ProductInfrastructure(IConfiguration configuration) : base(configuration)
        {
        }

        public Task<bool> Activate(Product entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> Add(Product entity)
        {
            throw new NotImplementedException();
        }

        public Task<Product> Get(Product entity)
        {
            throw new NotImplementedException();
        }

        public Task<List<Product>> GetList(Product entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Product entity)
        {
            throw new NotImplementedException();
        }
    }
}
