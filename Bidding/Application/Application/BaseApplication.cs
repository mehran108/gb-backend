namespace GoldBank.Application.Application
{
    public abstract class BaseApplication
    {
        #region Constructor
        /// <summary>
        /// BaseApplication initailizes object instance.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public BaseApplication(IConfiguration configuration, ILogger logger)
        {
            this.Configuration = configuration;
            this.Logger = logger;

        }

        public IConfiguration Configuration { get; }
        public ILogger Logger { get; }
        #endregion
    }
}
