using GoldBank.Infrastructure.Extension;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models.Product;
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
        private const string VendorIdParameterName = "@PVendorId";
        private const string ProductTypeIdParameterName = "@PProductTypeId";
        private const string ProductSourceIdParameterName = "@PProductSourceId";
        private const string ImageURLParameterName = "@PImageURL";
        private const string SKUParameterName = "@PSKU";

        public const string ProductIdColumnName = "ProductId";
        public const string ProductTypeIdColumnName = "ProductTypeId";
        public const string SKUColumnName = "SKU";
        public const string ProductSourceIdColumnName = "@PProductSource";
        public const string VendorIdColumnName = "@PVendorId";

        private const string JewelleryIdParameterName = "@PJewelleryId";
        private const string PrimaryCategoryIdsParameterName = "@PPrimaryCategoryIds";
        private const string CategoryIdsParameterName = "@PCategoryIds";
        private const string SubCategoryIdsParameterName = "@PSubCategoryIds";
        private const string WearingTypeIdsParameterName = "@PWearingTypeIds";
        private const string CollectionNameParameterName = "@PCollectionName";
        private const string GenderIdParameterName = "@PGenderId";
        private const string OccasionParameterName = "@POccasion";
        private const string DescriptionParameterName = "@PDescription";
        private const string MetalTypeIdParameterName = "@PMetalTypeId";
        private const string MetalPurityTypeIdParameterName = "@PMetalPurityTypeId";
        private const string MetalColorTypeIdParameterName = "@PMetalColorTypeId";
        private const string WeightTypeIdParameterName = "@PWeightTypeId";
        private const string NetWeightParameterName = "@PNetWeight";
        private const string WastageWeightParameterName = "@PWastageWeight";
        private const string WastagePctParameterName = "@PWastagePct";
        private const string TotalWeightParameterName = "@PTotalWeight";
        private const string WidthParameterName = "@PWidth";
        private const string BandwidthParameterName = "@PBandwidth";
        private const string ThicknessParameterName = "@PThickness";
        private const string SizeParameterName = "@PSize";
        private const string IsEcommerceParameterName = "@PIsEcommerce";
        private const string IsEngravingAvailableParameterName = "@PIsEngravingAvailable";
        private const string IsSizeAlterationAvailableParameterName = "@PIsSizeAlterationAvailable";
        private const string LacquerPriceParameterName = "@PLacquerPrice";
        private const string MakingPriceParameterName = "@PMakingPrice";
        private const string TotalPriceParameterName = "@PTotalPrice";


        public const string JewelleryIdColumnName = "JewelleryId";
        public const string PrimaryCategoryIdsColumnName = "PrimaryCategoryIds";
        public const string CategoryIdsColumnName = "CategoryIds";
        public const string SubCategoryIdsColumnName = "SubCategoryIds";
        public const string WearingTypeIdsColumnName = "WearingTypeIds";
        public const string CollectionNameColumnName = "CollectionName";
        public const string GenderIdColumnName = "GenderId";
        public const string OccasionColumnName = "Occasion";
        public const string DescriptionColumnName = "Description";
        public const string MetalTypeIdColumnName = "MetalTypeId";
        public const string MetalPurityTypeIdColumnName = "MetalPurityTypeId";
        public const string MetalColorTypeIdColumnName = "MetalColorTypeId";
        public const string WeightTypeIdColumnName = "WeightTypeId";
        public const string NetWeightColumnName = "NetWeight";
        public const string WastageWeightColumnName = "WastageWeight";
        public const string WastagePctColumnName = "WastagePct";
        public const string TotalWeightColumnName = "TotalWeight";
        public const string WidthColumnName = "Width";
        public const string BandwidthColumnName = "Bandwidth";
        public const string ThicknessColumnName = "Thickness";
        public const string SizeColumnName = "Size";
        public const string IsEcommerceColumnName = "IsEcommerce";
        public const string IsEngravingAvailableColumnName = "IsEngravingAvailable";
        public const string IsSizeAlterationAvailableColumnName = "IsSizeAlterationAvailable";
        public const string LacquerPriceColumnName = "LacquerPrice";
        public const string MakingPriceColumnName = "MakingPrice";
        public const string TotalPriceColumnName = "TotalPrice";

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
                IdParamter,
                base.GetParameter(ProductInfrastructure.ProductTypeIdParameterName, entity.ProductTypeId),
                base.GetParameter(ProductInfrastructure.SKUParameterName, entity.SKU),
                base.GetParameter(ProductInfrastructure.ProductSourceIdParameterName, entity.ProductSourceId),
                base.GetParameter(ProductInfrastructure.VendorIdParameterName, entity.VendorId),
                base.GetParameter(ProductInfrastructure.JewelleryIdParameterName, entity.Jewellery.JewelleryId),
                base.GetParameter(ProductInfrastructure.PrimaryCategoryIdsParameterName, entity.Jewellery.PrimaryCategoryIds),
                base.GetParameter(ProductInfrastructure.CategoryIdsParameterName, entity.Jewellery.CategoryId),
                base.GetParameter(ProductInfrastructure.SubCategoryIdsParameterName, entity.Jewellery.SubCategoryId),
                base.GetParameter(ProductInfrastructure.WearingTypeIdsParameterName, entity.Jewellery.WearingTypeIds),
                base.GetParameter(ProductInfrastructure.CollectionNameParameterName, entity.Jewellery.CollectionName),
                base.GetParameter(ProductInfrastructure.GenderIdParameterName, entity.Jewellery.GenderId),
                base.GetParameter(ProductInfrastructure.OccasionParameterName, entity.Jewellery.Occasion),
                base.GetParameter(ProductInfrastructure.DescriptionParameterName, entity.Jewellery.Description),
                base.GetParameter(ProductInfrastructure.MetalTypeIdParameterName, entity.Jewellery.MetalTypeId),
                base.GetParameter(ProductInfrastructure.MetalPurityTypeIdParameterName, entity.Jewellery.MetalPurityTypeId),
                base.GetParameter(ProductInfrastructure.MetalColorTypeIdParameterName, entity.Jewellery.MetalColorTypeId),
                base.GetParameter(ProductInfrastructure.WeightTypeIdParameterName, entity.Jewellery.WeightTypeId),
                base.GetParameter(ProductInfrastructure.NetWeightParameterName, entity.Jewellery.NetWeight),
                base.GetParameter(ProductInfrastructure.WastageWeightParameterName, entity.Jewellery.WastageWeight),
                base.GetParameter(ProductInfrastructure.WastagePctParameterName, entity.Jewellery.WastagePct),
                base.GetParameter(ProductInfrastructure.TotalWeightParameterName, entity.Jewellery.TotalWeight),
                base.GetParameter(ProductInfrastructure.WidthParameterName, entity.Jewellery.Width),
                base.GetParameter(ProductInfrastructure.BandwidthParameterName, entity.Jewellery.Bandwidth),
                base.GetParameter(ProductInfrastructure.ThicknessParameterName, entity.Jewellery.Thickness),
                base.GetParameter(ProductInfrastructure.SizeParameterName, entity.Jewellery.Size),
                base.GetParameter(ProductInfrastructure.IsEcommerceParameterName, entity.Jewellery.IsEcommerce),
                base.GetParameter(ProductInfrastructure.IsEngravingAvailableParameterName, entity.Jewellery.IsEngravingAvailable),
                base.GetParameter(ProductInfrastructure.IsSizeAlterationAvailableParameterName, entity.Jewellery.IsSizeAlterationAvailable),
                base.GetParameter(ProductInfrastructure.LacquerPriceParameterName, entity.Jewellery.LacquerPrice),
                base.GetParameter(ProductInfrastructure.MakingPriceParameterName, entity.Jewellery.MakingPrice),
                base.GetParameter(ProductInfrastructure.TotalPriceParameterName, entity.Jewellery.TotalPrice),

            };
            await base.ExecuteNonQuery(parameters, ProductInfrastructure.AddProductProcedureName, CommandType.StoredProcedure);
            entity.ProductId = Convert.ToInt32(IdParamter.Value);

            return entity.ProductId;

        }
        public async Task<int> AddProductWithJewelleryAsync(Product product)
        {
            using var connection = base.GetConnection(); // Your method to get DbConnection
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                var parameters = new List<DbParameter>        
                {
                    GetParameter("@ProductTypeId", product.ProductTypeId),
                    GetParameter("@SKU", product.SKU),
                    GetParameter("@ProductSourceId", product.ProductSourceId),
                    GetParameter("@VendorId", product.VendorId),
                    GetParameter("@CreatedBy", product.CreatedBy),
                    GetParameter("@PrimaryCategoryIds", product.Jewellery.PrimaryCategoryIds),
                    GetParameter("@CategoryId", product.Jewellery.CategoryId),
                    GetParameter("@SubCategoryId", product.Jewellery.SubCategoryId),
                    GetParameter("@WearingTypeIds", product.Jewellery.WearingTypeIds),
                    GetParameter("@CollectionIds", product.Jewellery.CollectionName),
                    GetParameter("@GenderId", product.Jewellery.GenderId),
                    GetParameter("@OccasionIds", product.Jewellery.Occasion),
                    GetParameter("@Description", product.Jewellery.Description),
                    GetParameter("@MetalTypeId", product.Jewellery.MetalTypeId),
                    GetParameter("@MetalPurityTypeId", product.Jewellery.MetalPurityTypeId),
                    GetParameter("@MetalColorTypeId", product.Jewellery.MetalColorTypeId),
                    GetParameter("@WeightTypeId", product.Jewellery.WeightTypeId),
                    GetParameter("@NetWeight", product.Jewellery.NetWeight),
                    GetParameter("@WastageWeight", product.Jewellery.WastageWeight),
                    GetParameter("@WastagePct", product.Jewellery.WastagePct),
                    GetParameter("@TotalWeight", product.Jewellery.TotalWeight),
                    GetParameter("@Width", product.Jewellery.Width),
                    GetParameter("@Bandwidth", product.Jewellery.Bandwidth),
                    GetParameter("@Thickness", product.Jewellery.Thickness),
                    GetParameter("@Size", product.Jewellery.Size),
                    GetParameter("@IsEcommerce", product.Jewellery.IsEcommerce),
                    GetParameter("@IsEngravingAvailable", product.Jewellery.IsEngravingAvailable),
                    GetParameter("@IsSizeAlterationAvailable", product.Jewellery.IsSizeAlterationAvailable),
                    GetParameter("@LacquerPrice", product.Jewellery.LacquerPrice),
                    GetParameter("@MakingPrice", product.Jewellery.MakingPrice),
                    GetParameter("@TotalPrice", product.Jewellery.TotalPrice)
                };

                int newProductId = 0, newJewelleryId = 0;

                using (var reader = await base.ExecuteReader(parameters, "AddProductWithJewelleryGb", CommandType.StoredProcedure, transaction, connection))
                {
                    if (reader != null && reader.Read())
                    {
                        newProductId = reader.GetIntegerValue("NewProductId");
                        newJewelleryId = reader.GetIntegerValue("NewJewelleryId");
                    }
                }

                foreach (var doc in product.Documents)
                {
                    var docParams = new List<DbParameter>
            {
                GetParameter("@ProductId", newProductId),
                GetParameter("@DocumentId", doc.DocumentId),
                GetParameter("@CreatedBy", product.CreatedBy)
            };

                    await base.ExecuteNonQuery(docParams, "AddProductDocumentGb", CommandType.StoredProcedure, transaction, connection);
                }

                foreach (var stone in product.StoneProducts)
                {
                    var stoneParams = new List<DbParameter>
            {
                GetParameter("@ProductId", newProductId),
                GetParameter("@StoneTypeId", stone.StoneTypeId),
                GetParameter("@Quantity", stone.Quantity),
                GetParameter("@StoneWeightTypeId", stone.StoneWeightTypeId),
                GetParameter("@TotalPrice", stone.TotalPrice),
                GetParameter("@StoneShapeId", stone.StoneShapeId),
                GetParameter("@CreatedBy", product.CreatedBy)
            };

                    int stoneId = 0;

                    using (var reader = await ExecuteReader(stoneParams, "AddStoneProductGb", CommandType.StoredProcedure, transaction, connection))
                    {
                        if (reader != null && reader.Read())
                        {
                            stoneId = reader.GetIntegerValue("NewStoneId"); // Assumes this column name
                        }
                    }

                    foreach (var doc in stone.Documents)
                    {
                        var docParams = new List<DbParameter>
                {
                    GetParameter("@StoneId", stoneId),
                    GetParameter("@DocumentId", doc.DocumentId),
                    GetParameter("@CreatedBy", product.CreatedBy)
                };

                        await base.ExecuteNonQuery(docParams, "AddStoneDocumentGb", CommandType.StoredProcedure, transaction, connection);
                    }
                }

                transaction.Commit();

                return (2); //TODO dynamic value return
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
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
