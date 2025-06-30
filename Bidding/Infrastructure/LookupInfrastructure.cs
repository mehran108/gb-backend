using System.Data.Common;
using System.Data;
using GoldBank.Models;
using MySqlX.XDevAPI;

namespace GoldBank.Infrastructure
{
    public interface LookupInfrastructure
    {
        #region Constructor
        /// <summary>
        ///  Lookupfrastructure initializes class object.
        /// </summary>
        public LookupInfrastructure(IConfiguration configuration, ILogger<LookupInfrastructure> logger) 
        {
        }

        #endregion

        #region StoreProcedures
        private const string AddLookupValueProcedureName = "AddLookupValue";
        #endregion

        #region Constants
        private const string Id = "Id";
        private const string LookupTableId = "LookupTableId";
        private const string LookupValueCode = "LookupValueCode";
        private const string Description = "Description";
        private const string Extra = "Extra";

        private const string LookupTableIdParameterName = "p_lookupTableId";
        private const string LookupValueCodeParameterName = "p_lookupValueCode";
        private const string DescriptionParameterName = "p_description";
        private const string UserIdParameterName = "p_userid";
        private const string ExtraParameterName = "p_extra";

        #endregion

        public async Task<int> AddLookupValue(LookupValue entity)
        {
            var Id = base.GetParameterOut(LookupInfrastructure.Id, SqlDbType.Int, entity.Id);
            var parameters = new List<DbParameter>
            {
                journeyIdParamter,
                base.GetParameter(LookupInfrastructure.LookupTableIdParameterName, entity.LookupTableId),
                base.GetParameter(LookupInfrastructure.LookupValueCodeParameterName, entity.LookupValueCode),
                base.GetParameter(LookupInfrastructure.DescriptionParameterName, entity.Description),
                base.GetParameter(LookupInfrastructure.ExtraParameterName, entity.Extra),
                base.GetParameter(LookupInfrastructure.UserIdParameterName, entity.CurrentUserId)

            };
            //TODO: Add other parameters.

            await base.ExecuteNonQuery(parameters, LookupInfrastructure.AddLookupValueProcedureName, CommandType.StoredProcedure);

            entity.Id = Convert.ToInt32(Id.Value);

            return entity.Id;
        }

    }
}
