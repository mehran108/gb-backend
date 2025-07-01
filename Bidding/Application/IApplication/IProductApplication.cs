using GoldBank.Models;
using GoldBank.Models.Product;

namespace GoldBank.Application.IApplication
{
    public interface IProductApplication : IBaseApplication<ProductGb>
    {
        Task<int> UploadImage(CommonCode commonCode);

    }
}
