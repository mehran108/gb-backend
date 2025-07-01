using System.Data.Common;
using System.Data;
using GoldBank.Models;
using MySqlX.XDevAPI;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Infrastructure.Extension;

namespace GoldBank.Infrastructure.Infrastructure
{
    public class LookupInfrastructure : BaseInfrastructure, ILookupInfrastructure
    {
        #region Constructor
        /// <summary>
        ///  Lookupfrastructure initializes class object.
        /// </summary>
        public LookupInfrastructure(IConfiguration configuration) : base(configuration)
        {
        }

        #endregion

        #region StoreProcedures
        private const string AddLookupValueProcedureName = "AddLookupValue";
        private const string GetLookupValueByCodeProcedureName = "GetLookupValueByCode";
        #endregion

        #region Constants
        private const string IdColumnName = "Id";
        private const string LookupTableIdColumnName = "LookupTableId";
        private const string LookupValueCodeColumnName = "LookupValueCode";
        private const string DescriptionColumnName = "Description";
        private const string ExtraColumnName = "Extra";

        private const string LookupTableIdParameterName = "p_lookupTableId";
        private const string LookupValueCodeParameterName = "p_lookupValueCode";
        private const string DescriptionParameterName = "p_description";
        private const string UserIdParameterName = "p_userid";
        private const string ExtraParameterName = "p_extra";
        private const string IdParameterName = "p_id";

        #endregion

        public async Task<int> Add(LookupValue entity)
        {
            var Id = GetParameterOut(LookupInfrastructure.IdParameterName, SqlDbType.Int, entity.Id);
            var parameters = new List<DbParameter>
            {
                Id,
                GetParameter(LookupTableIdParameterName, entity.LookupTableId),
                GetParameter(LookupValueCodeParameterName, entity.LookupValueCode),
                GetParameter(DescriptionParameterName, entity.Description),
                GetParameter(ExtraParameterName, entity.Extra),
                GetParameter(UserIdParameterName, entity.CurrentUserId)

            };
            //TODO: Add other parameters.

            await ExecuteNonQuery(parameters, AddLookupValueProcedureName, CommandType.StoredProcedure);

            entity.Id = Convert.ToInt32(Id.Value);

            return entity.Id;
        }
        public async Task<List<LookupValue>> GetLookupByCode(LookupValue entity)
        {
            var parameters = new List<DbParameter>
            {
                GetParameter(LookupValueCodeParameterName, entity.LookupValueCode)
            };
            List<LookupValue> lookupList = new List<LookupValue>();
            var item = new LookupValue();
            using (var dataReader = await ExecuteReader(parameters, GetLookupValueByCodeProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                        item = new LookupValue
                        {
                            Id = dataReader.GetIntegerValue(LookupInfrastructure.IdColumnName),
                            LookupTableId = dataReader.GetIntegerValue(LookupInfrastructure.LookupTableIdColumnName),
                            LookupValueCode = dataReader.GetStringValue(LookupInfrastructure.LookupValueCodeColumnName),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName),
                            Extra = dataReader.GetStringValue(LookupInfrastructure.ExtraColumnName)
                        };

                        lookupList.Add(item);
                    }
                }
            }
            return lookupList;
        }
        public Task<bool> Update(LookupValue lookupValue)
        {
            throw new NotImplementedException();
        }
        public Task<bool> Activate(LookupValue lookupValue)
        {
            throw new NotImplementedException();
        }
        public Task<List<LookupValue>> GetList(LookupValue entity)
        {
            throw new NotImplementedException();
        }
        public Task<LookupValue> Get(LookupValue entity)
        {
            throw new NotImplementedException();
        }
    }
}
