using Dapper;
using GoldBank.Infrastructure.Extension;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;
using GoldBank.Models.Product;
using MySql.Data.MySqlClient;
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

        public const string ProductIdColumnName = "ProductId";
        public const string ProductTypeIdColumnName = "ProductTypeId";
        public const string SKUColumnName = "SKU";
        public const string ProductSourceIdColumnName = "@PProductSource";
        public const string VendorIdColumnName = "@PVendorId";


        #endregion

        public Task<bool> Activate(Product entity)
        {
            throw new NotImplementedException();
        }
        public async Task<int> Add(Product product)
        {
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Step 1: Insert Product + Jewellery and get new IDs
                var insertResult = await connection.QueryFirstOrDefaultAsync<(int ProductId, int JewelleryId)>(
                    "AddProductWithJewelleryGb",
                    new
                    {
                        p_ProductTypeId = product.ProductTypeId,
                        p_SKU = product.SKU,
                        p_ProductSourceId = product.ProductSourceId,
                        p_VendorId = product.VendorId,
                        p_CreatedBy = product.CreatedBy,

                        p_PrimaryCategoryIds = product.Jewellery.PrimaryCategoryIds,
                        p_CategoryId = product.Jewellery.CategoryId,
                        p_SubCategoryId = product.Jewellery.SubCategoryId,
                        p_WearingTypeIds = product.Jewellery.WearingTypeIds,
                        p_CollectionIds = product.Jewellery.CollectionIds,
                        p_GenderId = product.Jewellery.GenderId,
                        p_OccasionIds = product.Jewellery.OccasionIds,
                        p_Description = product.Jewellery.Description,
                        p_MetalTypeId = product.Jewellery.MetalTypeId,
                        p_MetalPurityTypeId = product.Jewellery.MetalPurityTypeId,
                        p_MetalColorTypeId = product.Jewellery.MetalColorTypeId,
                        p_WeightTypeId = product.Jewellery.WeightTypeId,
                        p_NetWeight = product.Jewellery.NetWeight,
                        p_WastageWeight = product.Jewellery.WastageWeight,
                        p_WastagePct = product.Jewellery.WastagePct,
                        p_TotalWeight = product.Jewellery.TotalWeight,
                        p_Width = product.Jewellery.Width,
                        p_Bandwidth = product.Jewellery.Bandwidth,
                        p_Thickness = product.Jewellery.Thickness,
                        p_Size = product.Jewellery.Size,
                        p_IsEcommerce = product.Jewellery.IsEcommerce,
                        p_IsEngravingAvailable = product.Jewellery.IsEngravingAvailable,
                        p_IsSizeAlterationAvailable = product.Jewellery.IsSizeAlterationAvailable,
                        p_LacquerPrice = product.Jewellery.LacquerPrice,
                        p_MakingPrice = product.Jewellery.MakingPrice,
                        p_TotalPrice = product.Jewellery.TotalPrice
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );

                int productId = insertResult.ProductId;
                int jewelleryId = insertResult.JewelleryId;

                if (productId <= 0 || jewelleryId <= 0)
                {
                    await transaction.RollbackAsync();
                    return 0;
                }

                // Step 2: Add product documents
                if (product.ProductDocuments?.Count > 0)
                {
                    foreach (var doc in product.ProductDocuments)
                    {
                        await connection.ExecuteAsync(
                            "AddProductDocumentGb",
                            new
                            {
                                p_ProductId = productId,
                                p_DocumentId = doc.DocumentId,
                                p_CreatedBy = product.CreatedBy,
                                p_IsPrimary = doc.IsPrimary
                            },
                            transaction,
                            commandType: CommandType.StoredProcedure
                        );
                    }
                }
                if(product.StoneProducts?.Count > 0)
                {
                    // Step 3: Add stone products and their documents
                    foreach (var stone in product.StoneProducts)
                    {
                        var stoneId = await connection.QuerySingleAsync<int>(
                            "AddStoneProductGb",
                            new
                            {
                                p_ProductId = productId,
                                p_StoneTypeId = stone.StoneTypeId,
                                p_Quantity = stone.Quantity,
                                p_StoneWeightTypeId = stone.StoneWeightTypeId,
                                p_TotalPrice = stone.TotalPrice,
                                p_StoneShapeId = stone.StoneShapeId,
                                p_CreatedBy = product.CreatedBy
                            },
                            transaction,
                            commandType: CommandType.StoredProcedure
                        );
                        if(product.StoneDocuments?.Count > 0)
                        {
                            foreach (var doc in product.StoneDocuments)
                            {
                                await connection.ExecuteAsync(
                                    "AddStoneDocumentGb",
                                    new
                                    {
                                        p_StoneId = stoneId,
                                        p_DocumentId = doc.DocumentId,
                                        p_CreatedBy = product.CreatedBy,
                                        p_IsPrimary = doc.IsPrimary
                                    },
                                    transaction,
                                    commandType: CommandType.StoredProcedure
                                );
                            }
                        }
                    }
                }
                else
                {
                    //empty stone product;
                }

                await transaction.CommitAsync();
                return 1;
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
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
            using (var dataReader = await base.ExecuteReader(parameters, "GetProductById", CommandType.StoredProcedure))
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
            using (var dataReader = await base.ExecuteReader(parameters, "ProductGetList", CommandType.StoredProcedure))
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
        public Task<AllResponse<Product>> GetAll(AllRequest<Product> entity)
        {
            throw new NotImplementedException();
        }
    }
}
