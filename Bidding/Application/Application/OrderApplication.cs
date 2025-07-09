using GoldBank.Application.IApplication;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;
using GoldBank.Models.Product;

namespace GoldBank.Application.Application
{
    public class OrderApplication : IBaseApplication<Product>, IOrderApplication
    {
        public OrderApplication(IOrderInfrastructure orderInfrastructure, IConfiguration configuration, ILogger<Product> logger, IDocumentApplication DocumentApplication)
        {
            this.OrderInfrastructure = orderInfrastructure;
            this.DocumentApplication = DocumentApplication;
        }
        public IOrderInfrastructure OrderInfrastructure { get; }
        public IDocumentApplication DocumentApplication { get; }


        Task<bool> IBaseApplication<Product>.Activate(Product entity)
        {
            throw new NotImplementedException();
        }

        Task<int> IBaseApplication<Product>.Add(Product entity)
        {
            throw new NotImplementedException();
        }

        Task<Product> IBaseApplication<Product>.Get(Product entity)
        {
            throw new NotImplementedException();
        }

        Task<AllResponse<Product>> IBaseApplication<Product>.GetAll(AllRequest<Product> entity)
        {
            throw new NotImplementedException();
        }

        Task<List<Product>> IBaseApplication<Product>.GetList(Product entity)
        {
            throw new NotImplementedException();
        }

        Task<bool> IBaseApplication<Product>.Update(Product entity)
        {
            throw new NotImplementedException();
        }
    }
}
