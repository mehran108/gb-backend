using GoldBank.Application.IApplication;
using GoldBank.Connector;
using GoldBank.Infrastructure;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Infrastructure.Infrastructure;
using GoldBank.Models;

namespace GoldBank.Application.Application
{
    public class ApplicationUserApplication : IApplicationUserApplication
    {
        public ApplicationUserApplication(IConfiguration Configuration, IApplicationUserInfrastructure applicationUserInfrastructure) 
        {
            this.Configuration = Configuration;
            this.ApplicationUserInfrastructure = applicationUserInfrastructure;
               
        }

        public IConfiguration Configuration { get; }
        public IApplicationUserInfrastructure ApplicationUserInfrastructure { get; set; }
        public IServiceConnector ServiceConnector { get; set; }

        public async Task<bool> Activate(ApplicationUser ApplicationUser)
        {
            return await ApplicationUserInfrastructure.Activate(ApplicationUser);
        }

        public async Task<int> Add(ApplicationUser ApplicationUser)
        {
            return await ApplicationUserInfrastructure.Add(ApplicationUser);
        }

        public async Task<ApplicationUser> GetById(ApplicationUser ApplicationUser)
        {
            return await ApplicationUserInfrastructure.GetById(ApplicationUser);
        }

        public async Task<List<ApplicationUser>> GetAll(ApplicationUser ApplicationUser)
        {
            return await ApplicationUserInfrastructure.GetAll(ApplicationUser);
        }

        public async Task<bool> Update(ApplicationUser ApplicationUser)
        {
            return await ApplicationUserInfrastructure.Update(ApplicationUser);
        }
    }
}
