using GoldBank.Infrastructure.Extension;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;
using System.Data;
using System.Data.Common;

namespace GoldBank.Infrastructure.Infrastructure
{
    public class CommonCodeInfrastructure : BaseInfrastructure, ICommonCodeInfrastructure
    {
        public CommonCodeInfrastructure(IConfiguration configuration) : base(configuration)
        {

        }
        #region Constants
        private const string CommonCodeIdParameterName = "@PCommonCodeId";
        private const string ImageURLParameterName = "@PImageURL";

        public const string CommonCodeIdColumnName = "CommonCodeId";

        private const string AddCommonCodeProcedureName = "AddCommonCode";
        private const string FileUploadProcedureName = "AddDocument";
        private const string GetCommonCodeByIdProcedureName = "GetCommonCodeById";
        private const string GetCommonCodeListProcedureName = "GetCommonCodeList";

        private const string DocumentIdColumnName = "DocumentId";
        private const string DocumentTypeIdColumnName = "DocumentTypeId";
        private const string DocumentFileColumnName = "DocumentFile";
        private const string DocumentNameColumnName = "DocumentName";
        private const string DocumentExtensionColumnName = "DocumentExtension";
        private const string DocumentPathColumnName = "DocumentPath";
        
        private const string DocumentIdParameterName = "PDocumentId";
        private const string DocumentTypeIdParameterName = "PDocumentTypeId";
        private const string DocumentFileParameterName = "PDocumentFile";
        private const string DocumentNameParameterName = "PDocumentName";
        private const string DocumentExtensionParameterName = "PDocumentExtension";
        private const string DocumentPathParameterName = "PDocumentPath";
        private const string DocumentTypeParameterName = "PDocumentType";

        #endregion

        public Task<bool> Activate(CommonCode entity)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Add(CommonCode entity)
        {
            var DocumentIdParamter = base.GetParameterOut(CommonCodeInfrastructure.DocumentIdParameterName, SqlDbType.Int, entity.DocumentId);
            var parameters = new List<DbParameter>
            {
                DocumentIdParamter,
                base.GetParameter(CommonCodeInfrastructure.DocumentNameParameterName, entity.FileName),
                base.GetParameter(CommonCodeInfrastructure.DocumentExtensionParameterName, entity.DocumentExtension),
                base.GetParameter(CommonCodeInfrastructure.DocumentPathParameterName, entity.DocumentPath),
                base.GetParameter(BaseInfrastructure.CurrentUserIdParameterName,    entity.CreatedBy),
                base.GetParameter(BaseInfrastructure.ActiveParameterName, entity.IsActive),
                base.GetParameter(CommonCodeInfrastructure.DocumentTypeParameterName,entity.DocumentType)

            };

            await base.ExecuteNonQuery(parameters, CommonCodeInfrastructure.FileUploadProcedureName, CommandType.StoredProcedure);

            entity.DocumentId = Convert.ToInt32(DocumentIdParamter.Value);

            return entity.DocumentId;
        }

        public async Task<CommonCode> Get(CommonCode entity)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(CommonCodeInfrastructure.CommonCodeIdParameterName, entity.CommonCodeId)
            };
            var res = new CommonCode();
            using (var dataReader = await base.ExecuteReader(parameters, CommonCodeInfrastructure.GetCommonCodeByIdProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                        res.CommonCodeId = dataReader.GetIntegerValue(CommonCodeInfrastructure.CommonCodeIdColumnName);
                    }
                }
            }
            return res;
        }

        public async Task<List<CommonCode>> GetList(CommonCode entity)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(CommonCodeInfrastructure.CommonCodeIdParameterName, entity.CommonCodeId)
            };
            var result = new List<CommonCode>();
            using (var dataReader = await base.ExecuteReader(parameters, CommonCodeInfrastructure.GetCommonCodeListProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                        var res = new CommonCode();
                        res.CommonCodeId = dataReader.GetIntegerValue(CommonCodeInfrastructure.CommonCodeIdColumnName);
                        result.Add(res);
                    }
                }
            }
            return result;
        }

        public Task<bool> Update(CommonCode entity)
        {
            throw new NotImplementedException();
        }
    }
}
