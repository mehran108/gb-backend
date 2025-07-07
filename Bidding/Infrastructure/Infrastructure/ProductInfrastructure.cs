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
        private object ToDbValue(object? value)
        {
            if (value == null) return DBNull.Value;

            switch (value)
            {
                case int intVal when intVal <= 0:
                    return DBNull.Value;

                case decimal decVal when decVal <= 0:
                    return DBNull.Value;

                case string strVal when string.IsNullOrWhiteSpace(strVal):
                    return DBNull.Value;

                default:
                    return value;
            }
        }

        public async Task<AllResponse<Product>> GetAllProducts(AllRequest<ProductRequestVm> product)
        {
            var Response = new AllResponse<Product>();
            var ProductList = new List<Product>();
            var JewelleryList = new List<Jewellery>();
            var StoneProductList = new List<StoneProduct>();
            var parameters = new List<DbParameter>
            {
            base.GetParameter("@p_PageNumber", product.Offset),
            base.GetParameter("@p_PageSize", ToDbValue(product.PageSize)),
            base.GetParameter("@p_ProductTypeId", ToDbValue(product.Data.ProductTypeId)),
            base.GetParameter("@p_SKU", ToDbValue(product.Data.SKU)),
            base.GetParameter("@p_ProductSourceId", ToDbValue(product.Data.ProductSourceId)),
            base.GetParameter("@p_VendorId", ToDbValue(product.Data.VendorId)),
            base.GetParameter("@p_PrimaryCategoryIds", ToDbValue(product.Data.PrimaryCategoryIds)),
            base.GetParameter("@p_CategoryId", ToDbValue(product.Data.CategoryId)),
            base.GetParameter("@p_SubCategoryId", ToDbValue(product.Data.SubCategoryId)),
            base.GetParameter("@p_WearingTypeIds", ToDbValue(product.Data.WearingTypeIds)),
            base.GetParameter("@p_CollectionIds", ToDbValue(product.Data.CollectionIds)),
            base.GetParameter("@p_GenderId", ToDbValue(product.Data.GenderId)),
            base.GetParameter("@p_OccasionIds", ToDbValue(product.Data.OccasionIds)),
            base.GetParameter("@p_Description", ToDbValue(product.Data.Description)),
            base.GetParameter("@p_MetalTypeId", ToDbValue(product.Data.MetalTypeId)),
            base.GetParameter("@p_MetalPurityTypeId", ToDbValue(product.Data.MetalPurityTypeId)),
            base.GetParameter("@p_MetalColorTypeId", ToDbValue(product.Data.MetalColorTypeId)),
            base.GetParameter("@p_WeightTypeId", ToDbValue(product.Data.WeightTypeId)),
            base.GetParameter("@p_NetWeight", ToDbValue(product.Data.NetWeight)),
            base.GetParameter("@p_WastageWeight", ToDbValue(product.Data.WastageWeight)),
            base.GetParameter("@p_WastagePct", ToDbValue(product.Data.WastagePct)),
            base.GetParameter("@p_TotalWeight", ToDbValue(product.Data.TotalWeight)),
            base.GetParameter("@p_Width", ToDbValue(product.Data.Width)),
            base.GetParameter("@p_Bandwidth", ToDbValue(product.Data.Bandwidth)),
            base.GetParameter("@p_Thickness", ToDbValue(product.Data.Thickness)),
            base.GetParameter("@p_Size", ToDbValue(product.Data.Size)),
            base.GetParameter("@p_IsEcommerce", ToDbValue(product.Data.IsEcommerce) ),
            base.GetParameter("@p_IsEngravingAvailable", ToDbValue(product.Data.IsEngravingAvailable)),
            base.GetParameter("@p_IsSizeAlterationAvailable", ToDbValue(product.Data.IsSizeAlterationAvailable)),
            base.GetParameter("@p_LacquerPrice", ToDbValue(product.Data.LacquerPrice)),
            base.GetParameter("@p_MakingPrice", ToDbValue(product.Data.MakingPrice)),
            base.GetParameter("@p_TotalPrice", ToDbValue(product.Data.TotalPrice)),
            base.GetParameter("@p_StoneTypeId", ToDbValue(product.Data.StoneTypeId)),
            base.GetParameter("@p_StoneShapeId", ToDbValue(product.Data.StoneShapeId)),
            base.GetParameter("@p_StoneWeightTypeId", ToDbValue(product.Data.StoneWeightTypeId))
                };
            using (var dataReader = await base.ExecuteReader(parameters, "GetAllProductsGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        var item = new Product();
                        item.ProductTypeId = dataReader.GetIntegerValue(ProductInfrastructure.ProductIdColumnName);
                        item.ProductId = dataReader.GetIntegerValue("productId");
                        item.SKU = dataReader.GetStringValue("sKU");
                        item.ProductSourceId = dataReader.GetIntegerValue("productSourceId");
                        item.VendorId = dataReader.GetIntegerValue("vendorId");
                        item.IsActive = dataReader.GetBooleanValue("isActive");
                        item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        item.StoreId = dataReader.GetIntegerValue("storeId");
                        item.CreatedAt = dataReader.GetDateTime("createdAt");
                        item.CreatedBy = dataReader.GetIntegerValue("createdBy");

                        ProductList.Add(item);
                    }
                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            var item = new Jewellery();
                            item.JewelleryId = dataReader.GetIntegerValue("jewelleryId");
                            item.ProductId = dataReader.GetIntegerValue("productId");
                            item.ProductType = new ProductType();// TODO 
                            item.PrimaryCategoryIds = dataReader.GetStringValue("primaryCategoryIds");
                            item.CategoryId = dataReader.GetIntegerValue("categoryId");
                            item.SubCategoryId = dataReader.GetIntegerValue("subCategoryId");
                            item.WearingTypeIds = dataReader.GetStringValue("wearingTypeIds");
                            item.CollectionIds = dataReader.GetStringValue("collectionIds");
                            item.GenderId = dataReader.GetIntegerValue("genderId");
                            item.OccasionIds = dataReader.GetStringValue("occasionIds");
                            item.Description = dataReader.GetStringValue("description");
                            item.MetalTypeId = dataReader.GetIntegerValue("metalTypeId");
                            item.MetalPurityTypeId = dataReader.GetIntegerValue("metalPurityTypeId");
                            item.MetalColorTypeId = dataReader.GetIntegerValue("metalColorTypeId");
                            item.WeightTypeId = dataReader.GetIntegerValue("weightTypeId");
                            item.NetWeight = dataReader.GetIntegerValue("netWeight");
                            item.WastagePct = dataReader.GetIntegerValue("wastagePct");
                            item.WastageWeight = dataReader.GetIntegerValue("wastageWeight");
                            item.TotalWeight = dataReader.GetIntegerValue("totalWeight");
                            item.Width = dataReader.GetStringValue("width");
                            item.Bandwidth = dataReader.GetStringValue("bandwidth");
                            item.Thickness = dataReader.GetStringValue("thickness");
                            item.Size = dataReader.GetStringValue("size");
                            item.IsEcommerce = dataReader.GetBooleanValue("isEcommerce");
                            item.IsEngravingAvailable = dataReader.GetBooleanValue("isEngravingAvailable");
                            item.LacquerPrice = dataReader.GetDecimalValue("jewelleryId");
                            item.MakingPrice = dataReader.GetDecimalValue("jewelleryId");
                            item.TotalPrice = dataReader.GetDecimalValue("totalPrice");
                            item.IsActive = dataReader.GetBooleanValue("isActive");
                            item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                            item.CreatedAt = dataReader.GetDateTimeValue("createdAt");
                            item.CreatedBy = dataReader.GetIntegerValue("createdBy");

                            var productItem = ProductList.FirstOrDefault(x => x.ProductId == item.ProductId);
                            if(productItem != null)
                            {
                                productItem.Jewellery = item;
                            }
                        }
                    }
                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            var item = new StoneProduct();
                            item.StoneProductId = dataReader.GetIntegerValue("stoneProductId");
                            item.StoneShapeId = dataReader.GetIntegerValue("stoneShapeId");
                            item.ProductId = dataReader.GetIntegerValue("productId");
                            item.Quantity = dataReader.GetIntegerValue("quantity");
                            item.StoneShape = new StoneShape();
                            item.StoneShapeId = dataReader.GetIntegerValue("stoneShapeId");
                            item.StoneType = new StoneType();
                            item.StoneTypeId = dataReader.GetIntegerValue("stoneTypeId");
                            item.StoneWeightType = new StoneWeightType();
                            item.StoneWeightTypeId = dataReader.GetIntegerValue("stoneWeightTypeId");
                            item.TotalWeight = dataReader.GetDecimalValue("totalWeight");
                            item.TotalPrice = dataReader.GetDecimalValue("totalPrice");
                            item.IsActive = dataReader.GetBooleanValue("isActive");
                            item.IsDeleted = dataReader.GetBooleanValue("isDeleted");

                            var productItem = ProductList.FirstOrDefault(x => x.ProductId == item.ProductId);
                            if (productItem != null)
                            {
                                productItem.StoneProducts.Add(item);
                            }
                        }
                    }
                }
            }

            //foreach(var item in ProductList)
            //{
            //    foreach(var jewellery in JewelleryList)
            //    {
            //        if(jewellery.ProductId == item.ProductId)
            //        {
            //            item.Jewellery = jewellery;
            //        }
            //    }
            //    foreach(var stone in StoneProductList)
            //    {
            //        if(stone.ProductId == item.ProductId)
            //        {
            //            item.StoneProducts.Add(stone);
            //        }
            //    }
            //}

            Response.Data = ProductList;
            Response.TotalRecord = ProductList.Count;

            return Response;
        }
    }
}
