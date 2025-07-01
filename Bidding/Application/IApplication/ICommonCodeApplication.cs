using GoldBank.Models;

namespace GoldBank.Application.IApplication
{
    public interface ICommonCodeApplication : IBaseApplication<CommonCode>
    {
        Task<int> UploadImage(CommonCode commonCode);
    }
}
