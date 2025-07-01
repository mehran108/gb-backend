using GoldBank.Infrastructure.Extension;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;
using System.Data;
using System.Data.Common;

namespace GoldBank.Infrastructure.Infrastructure
{
    public class ProductInfrastructure : BaseInfrastructure, IProductInfrastructure
    {
        public ProductInfrastructure(IConfiguration configuration) : base(configuration)
        {

        }
        #region Constants
        private const string ProductIdParameterName = "@PProductId";
        private const string ImageURLParameterName = "@PImageURL";

        public const string ProductIdColumnName = "ProductId";

        private const string AddProductProcedureName = "AddProduct";
        private const string GetProductByIdProcedureName = "GetProductById";
        private const string GetProductListProcedureName = "GetProductList";
        #endregion

        public Task<bool> Activate(Product entity)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Add(Product entity)
        {
            var IdParamter = base.GetParameterOut(ProductInfrastructure.ProductIdParameterName, SqlDbType.Int, entity.ProductId);
            var parameters = new List<DbParameter>
            {
                IdParamter
                //base.GetParameter(ProductInfrastructure.ImageURLParameterName, entity.ImageURL)
            };
            await base.ExecuteNonQuery(parameters, ProductInfrastructure.AddProductProcedureName, CommandType.StoredProcedure);
            entity.ProductId = Convert.ToInt32(IdParamter.Value);

            return entity.ProductId;

        }

        public async Task<Product> Get(Product entity)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(ProductInfrastructure.ProductIdParameterName, entity.ProductId)
            };
            var res = new Product();
            using (var dataReader = await base.ExecuteReader(parameters, ProductInfrastructure.GetProductByIdProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                        res.ProductId = dataReader.GetIntegerValue(ProductInfrastructure.ProductIdColumnName);
                    }
                }
            }
            return res;
        }

        public async Task<List<Product>> GetList(Product entity)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter(ProductInfrastructure.ProductIdParameterName, entity.ProductId)
            };
            var result = new List<Product>();
            using (var dataReader = await base.ExecuteReader(parameters, ProductInfrastructure.GetProductListProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                        var res = new Product();
                        res.ProductId = dataReader.GetIntegerValue(ProductInfrastructure.ProductIdColumnName);
                        result.Add(res);
                    }
                }
            }
            return result;
        }

        public Task<bool> Update(Product entity)
        {
            throw new NotImplementedException();
        }
    }
}
