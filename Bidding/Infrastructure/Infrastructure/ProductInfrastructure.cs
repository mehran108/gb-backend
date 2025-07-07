using Dapper;
using GoldBank.Infrastructure.Extension;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;
using GoldBank.Models.Product;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
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
                        p_StoreId = product.StoreId,
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
                                p_TotalWeight = stone.TotalWeight,
                                p_StoneShapeId = stone.StoneShapeId,
                                p_CreatedBy = product.CreatedBy
                            },
                            transaction,
                            commandType: CommandType.StoredProcedure
                        );
                        if(product.StoneProducts?.Count > 0)
                        {
                            foreach (var stoneProduct in product.StoneProducts)
                            {
                                foreach(var doc in stoneProduct.StoneDocuments)
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
                }
                else
                {
                    //empty stone product;
                }

                await transaction.CommitAsync();
                return productId;
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

        public async Task<bool> Update(Product product)
        {
            using var connection = base.GetConnection();
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Step 1: Update Product + Jewellery
                var affected = await connection.ExecuteAsync(
                    "UpdateProductWithJewelleryGb",
                    new
                    {
                        ProductId = product.ProductId,
                        JewelleryId = product.Jewellery.JewelleryId,
                        ProductTypeId = product.ProductTypeId,
                        SKU = product.SKU,
                        ProductSourceId = product.ProductSourceId,
                        VendorId = product.VendorId,
                        CreatedBy = product.CreatedBy,

                        PrimaryCategoryIds = product.Jewellery.PrimaryCategoryIds,
                        CategoryId = product.Jewellery.CategoryId,
                        SubCategoryId = product.Jewellery.SubCategoryId,
                        WearingTypeIds = product.Jewellery.WearingTypeIds,
                        CollectionIds = product.Jewellery.CollectionIds,
                        GenderId = product.Jewellery.GenderId,
                        OccasionIds = product.Jewellery.OccasionIds,
                        Description = product.Jewellery.Description,
                        MetalTypeId = product.Jewellery.MetalTypeId,
                        MetalPurityTypeId = product.Jewellery.MetalPurityTypeId,
                        MetalColorTypeId = product.Jewellery.MetalColorTypeId,
                        WeightTypeId = product.Jewellery.WeightTypeId,
                        NetWeight = product.Jewellery.NetWeight,
                        WastageWeight = product.Jewellery.WastageWeight,
                        WastagePct = product.Jewellery.WastagePct,
                        TotalWeight = product.Jewellery.TotalWeight,
                        Width = product.Jewellery.Width,
                        Bandwidth = product.Jewellery.Bandwidth,
                        Thickness = product.Jewellery.Thickness,
                        Size = product.Jewellery.Size,
                        IsEcommerce = product.Jewellery.IsEcommerce,
                        IsEngravingAvailable = product.Jewellery.IsEngravingAvailable,
                        IsSizeAlterationAvailable = product.Jewellery.IsSizeAlterationAvailable,
                        LacquerPrice = product.Jewellery.LacquerPrice,
                        MakingPrice = product.Jewellery.MakingPrice,
                        TotalPrice = product.Jewellery.TotalPrice
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );

                // Optional: Delete old ProductDocuments before re-inserting
                await connection.ExecuteAsync(
                    "DeleteProductDocumentsGb",
                    new { ProductId = product.ProductId },
                    transaction,
                    commandType: CommandType.StoredProcedure
                );

                // Step 2: Insert updated ProductDocuments
                if (product.ProductDocuments?.Count > 0)
                {
                    foreach (var doc in product.ProductDocuments)
                    {
                        await connection.ExecuteAsync(
                            "AddProductDocumentGb",
                            new
                            {
                                ProductId = product.ProductId,
                                DocumentId = doc.DocumentId,
                                CreatedBy = product.CreatedBy
                            },
                            transaction,
                            commandType: CommandType.StoredProcedure
                        );
                    }
                }

                // Optional: Delete old StoneProducts + StoneDocuments
                await connection.ExecuteAsync(
                    "DeleteStoneProductsByProductIdGb",
                    new { ProductId = product.ProductId },
                    transaction,
                    commandType: CommandType.StoredProcedure
                );

                // Step 3: Re-insert StoneProducts and their Documents
                if(product.StoneProducts?.Count>0)
                {
                    foreach (var stone in product.StoneProducts)
                    {
                        int stoneId = await connection.QuerySingleAsync<int>(
                            "AddStoneProductGb",
                            new
                            {
                                ProductId = product.ProductId,
                                StoneTypeId = stone.StoneTypeId,
                                Quantity = stone.Quantity,
                                StoneWeightTypeId = stone.StoneWeightTypeId,
                                TotalPrice = stone.TotalPrice,
                                p_TotalWeight = stone.TotalWeight,
                                StoneShapeId = stone.StoneShapeId,
                                CreatedBy = product.CreatedBy
                            },
                            transaction,
                            commandType: CommandType.StoredProcedure
                        );

                        foreach (var doc in stone.StoneDocuments)
                        {
                            await connection.ExecuteAsync(
                                "AddStoneDocumentGb",
                                new
                                {
                                    StoneId = stoneId,
                                    DocumentId = doc.DocumentId,
                                    CreatedBy = product.CreatedBy,
                                    IsPrimary = doc.IsPrimary
                                },
                                transaction,
                                commandType: CommandType.StoredProcedure
                            );
                        }
                    }
                }

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public Task<AllResponse<Product>> GetAll(AllRequest<Product> entity)
        {
            throw new NotImplementedException();
        }
        public Task<bool> BulkImport(ProductBulkImport productBulkImport)
        {
            throw new NotImplementedException();
        }
        public async Task<AllResponse<Product>> GetAllProducts(AllRequest<ProductRequestVm> product)
        {
            var result = new AllResponse<Product>();
            var parameters = new List<DbParameter>
            {
            base.GetParameter("@p_PageNumber", product.Offset),
            base.GetParameter("@p_PageSize", product.PageSize),
            base.GetParameter("@p_ProductTypeId", product.Data.ProductTypeId),
            base.GetParameter("@p_SKU", product.Data.SKU),
            base.GetParameter("@p_ProductSourceId", product.Data.ProductSourceId),
            base.GetParameter("@p_VendorId", product.Data.VendorId),
            base.GetParameter("@p_PrimaryCategoryIds", product.Data.PrimaryCategoryIds),
            base.GetParameter("@p_CategoryIds", product.Data.CategoryIds),
            base.GetParameter("@p_SubCategoryIds", product.Data.SubCategoryIds),
            base.GetParameter("@p_WearingTypeIds", product.Data.WearingTypeIds),
            base.GetParameter("@p_CollectionIds", product.Data.CollectionIds),
            base.GetParameter("@p_GenderId", product.Data.GenderId),
            base.GetParameter("@p_OccasionIds", product.Data.OccasionIds),
            base.GetParameter("@p_Description", product.Data.Description),
            base.GetParameter("@p_MetalTypeId", product.Data.MetalTypeId),
            base.GetParameter("@p_MetalPurityTypeId", product.Data.MetalPurityTypeId),
            base.GetParameter("@p_MetalColorTypeId", product.Data.MetalColorTypeId),
            base.GetParameter("@p_WeightTypeId", product.Data.WeightTypeId),
            base.GetParameter("@p_NetWeight", product.Data.NetWeight),
            base.GetParameter("@p_WastageWeight", product.Data.WastageWeight),
            base.GetParameter("@p_WastagePct", product.Data.WastagePct),
            base.GetParameter("@p_TotalWeight", product.Data.TotalWeight),
            base.GetParameter("@p_Width", product.Data.Width),
            base.GetParameter("@p_Bandwidth", product.Data.Bandwidth),
            base.GetParameter("@p_Thickness", product.Data.Thickness),
            base.GetParameter("@p_Size", product.Data.Size),
            base.GetParameter("@p_IsEcommerce", product.Data.IsEcommerce),
            base.GetParameter("@p_IsEngravingAvailable", product.Data.IsEngravingAvailable),
            base.GetParameter("@p_IsSizeAlterationAvailable", product.Data.IsSizeAlterationAvailable),
            base.GetParameter("@p_LacquerPrice", product.Data.LacquerPrice),
            base.GetParameter("@p_MakingPrice", product.Data.MakingPrice),
            base.GetParameter("@p_TotalPrice", product.Data.TotalPrice),
            base.GetParameter("@p_StoneTypeId", product.Data.StoneTypeId),
            base.GetParameter("@p_StoneShapeId", product.Data.StoneShapeId),
            base.GetParameter("@p_StoneWeightTypeId", product.Data.StoneWeightTypeId),
                };
            using (var dataReader = await base.ExecuteReader(parameters, "GetAllProductsGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                        var res = new Product();
                        res.ProductTypeId = dataReader.GetIntegerValue(ProductInfrastructure.ProductIdColumnName);
                        result.Data.Add(res);
                    }
                }
            }

            return result;
        }
    }
}
