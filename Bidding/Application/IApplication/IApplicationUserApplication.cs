using GoldBank.Models;

namespace GoldBank.Application.IApplication
{
    public interface IApplicationUserApplication
    {
        public Task<int> Add(ApplicationUser ApplicationUser);
        public Task<bool> Update(ApplicationUser ApplicationUser);
        public Task<ApplicationUser> GetById(ApplicationUser ApplicationUser);
        public Task<List<ApplicationUser>> GetAll(ApplicationUser ApplicationUser);
        public Task<bool> Activate(ApplicationUser ApplicationUser);
    }
}
