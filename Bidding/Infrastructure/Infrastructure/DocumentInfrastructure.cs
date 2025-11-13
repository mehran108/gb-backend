using GoldBank.Application.Application;
using GoldBank.Infrastructure.Extension;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;
using System.Data;
using System.Data.Common;

namespace GoldBank.Infrastructure.Infrastructure
{
    public class DocumentInfrastructure : BaseInfrastructure, IDocumentInfrastructure
    {
        public DocumentInfrastructure(IConfiguration configuration) : base(configuration)
        {

        }
        #region Constants
        private const string DocumentIdParameterName = "@PDocumentId";

        public const string DocumentIdColumnName = "DocumentId";

        private const string GetDocumentByIdProcedureName = "GetDocumentById";
        private const string GetDocumentListProcedureName = "GetDocumentList";

        private const string DocumentNameParameterName = "PName";
        private const string DocumentExtensionParameterName = "PExtension";
        private const string DocumentPathParameterName = "PUrl";

        #endregion

        public Task<bool> Activate(Document entity)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Add(Document entity)
        {
            var DocumentIdParamter = base.GetParameterOut(DocumentInfrastructure.DocumentIdParameterName, SqlDbType.Int, entity.DocumentId);
            var parameters = new List<DbParameter>
            {
                DocumentIdParamter,
                base.GetParameter(DocumentInfrastructure.DocumentNameParameterName, entity.Url),
                base.GetParameter(DocumentInfrastructure.DocumentExtensionParameterName, entity.Extension),
                base.GetParameter(DocumentInfrastructure.DocumentPathParameterName, entity.Url),
                base.GetParameter(BaseInfrastructure.ActiveParameterName, entity.IsActive),
                base.GetParameter("PCreatedBy",entity.CreatedBy)

            };

            await base.ExecuteNonQuery(parameters, "AddDocument_gb", CommandType.StoredProcedure);

            entity.DocumentId = Convert.ToInt32(DocumentIdParamter.Value);

            return entity.DocumentId;
        }

        public async Task<Document> Get(Document entity)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(DocumentInfrastructure.DocumentIdParameterName, entity.DocumentId)
            };
            var res = new Document();
            using (var dataReader = await base.ExecuteReader(parameters, DocumentInfrastructure.GetDocumentByIdProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                        res.DocumentId = dataReader.GetIntegerValue(DocumentInfrastructure.DocumentIdColumnName);
                    }
                }
            }
            return res;
        }

        public async Task<List<Document>> GetList(Document entity)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(DocumentInfrastructure.DocumentIdParameterName, entity.DocumentId)
            };
            var result = new List<Document>();
            using (var dataReader = await base.ExecuteReader(parameters, DocumentInfrastructure.GetDocumentListProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                        var res = new Document();
                        res.DocumentId = dataReader.GetIntegerValue(DocumentInfrastructure.DocumentIdColumnName);
                        result.Add(res);
                    }
                }
            }
            return result;
        }
        public async Task<bool> Delete(Document Document)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter("p_DocumentId", Document.DocumentId)
            };
            var res = await this.ExecuteNonQuery(parameters, "DeleteDocument_gb", CommandType.StoredProcedure);
            return res > 0;
        }
        public Task<bool> Update(Document entity)
        {
            throw new NotImplementedException();
        }
        public Task<AllResponse<Document>> GetAll(AllRequest<Document> entity)
        {
            throw new NotImplementedException();
        }
    }
}
