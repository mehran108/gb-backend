//using GoldBank.Application;
//using GoldBank.Infrastructure;
//using GoldBank.Models;

//namespace GoldBank.BackgroundJobs
//{
//    public class BackgroundJobs : BackgroundService
//   {
//        public IProspectsApplication ProspectsApplication { get; set; }
//        public IEmailApplication EmailApplication { get; set; }

//        public BackgroundJobs(IProspectsApplication prospectsApplication, IEmailApplication emailApplication)
//        {
//            ProspectsApplication = prospectsApplication;
//            EmailApplication = emailApplication;
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//{
//            //List<Prospect> prospetsList = new List<Prospect>();
//            //Prospect prospects = new Prospect();

//            //  var resultuser =  await this.ProspectsApplication.GetProspectsList();
//            //foreach (var item in resultuser)
//            //{

//            //    await this.EmailApplication.SendEmailProspect(item.Email);

//            //}
//            await EmailApplication.EmailSchedualeCampaign();
//        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

           

//}
//}
//}
