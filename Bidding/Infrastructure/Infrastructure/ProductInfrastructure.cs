using Amazon.Runtime.Internal;
using Dapper;
using GoldBank.Infrastructure.Extension;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;
using GoldBank.Models.Product;
using GoldBank.Models.RequestModels;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Utilities.Collections;
using Renci.SshNet.Compression;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Transactions;

namespace GoldBank.Infrastructure.Infrastructure
{
    public class ProductInfrastructure : BaseInfrastructure, IProductInfrastructure
    {
        public ProductInfrastructure(IConfiguration configuration, ICustomerInfrastructure customerInfrastructure) : base(configuration)
        {
            this.CustomerInfrastructure = customerInfrastructure;
        }
        #region Constants
        private const string ProductIdParameterName = "@PProductId";

        public const string ProductIdColumnName = "ProductId";
        public const string ProductTypeIdColumnName = "ProductTypeId";
        public const string SKUColumnName = "SKU";
        public const string ProductSourceIdColumnName = "@PProductSource";
        public const string VendorIdColumnName = "@PVendorId";

        public ICustomerInfrastructure CustomerInfrastructure { get; set; }

        #endregion

        public async Task<bool> BulkImport(Document document)
        {
            using var connection = base.GetConnection();
            //await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var command = @"truncate table producttemp_gb";
                await connection.ExecuteAsync(command, transaction: transaction);
                command = "LOAD DATA INFILE '/var/lib/mysql-files/file.csv' INTO TABLE producttemp_gb FIELDS TERMINATED BY ',' LINES TERMINATED BY '\\n' IGNORE 1 ROWS;";

                await connection.ExecuteAsync(command, transaction: transaction);




                // Step 2: Call stored procedure to insert products
                await connection.ExecuteAsync(
                    "BulkInsertProducts_Gb",
                    param: null,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
            return true;
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

        #region Not Used

        public Task<bool> Activate(Product entity)
        {
            throw new NotImplementedException();
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
        public async Task<bool> UpdateOld(Product product)
        {
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var parameters = new DynamicParameters();

                // Input parameters for product and jewellery
                parameters.Add("p_ProductId", product.ProductId);
                parameters.Add("p_ProductTypeId", product.ProductTypeId);
                parameters.Add("p_SKU", product.SKU);
                parameters.Add("p_ProductSourceId", product.ProductSourceId);
                parameters.Add("p_VendorId", product.VendorId);
                parameters.Add("p_StoreId", product.StoreId);
                parameters.Add("p_CreatedBy", product.CreatedBy);

                parameters.Add("p_PrimaryCategoryIds", product.Jewellery.PrimaryCategoryIds);
                parameters.Add("p_CategoryId", product.Jewellery.CategoryId);
                parameters.Add("p_SubCategoryId", product.Jewellery.SubCategoryId);
                parameters.Add("p_WearingTypeIds", product.Jewellery.WearingTypeIds);
                parameters.Add("p_CollectionIds", product.Jewellery.CollectionIds);
                parameters.Add("p_GenderId", product.Jewellery.GenderId);
                parameters.Add("p_OccasionIds", product.Jewellery.OccasionIds);
                parameters.Add("p_Description", product.Jewellery.Description);
                parameters.Add("p_MetalTypeId", product.Jewellery.MetalTypeId);
                parameters.Add("p_MetalPurityTypeId", product.Jewellery.MetalPurityTypeId);
                parameters.Add("p_MetalColorTypeId", product.Jewellery.MetalColorTypeId);
                parameters.Add("p_WeightTypeId", product.Jewellery.WeightTypeId);
                parameters.Add("p_NetWeight", product.Jewellery.NetWeight);
                parameters.Add("p_WastageWeight", product.Jewellery.WastageWeight);
                parameters.Add("p_WastagePct", product.Jewellery.WastagePct);
                parameters.Add("p_TotalWeight", product.Jewellery.TotalWeight);
                parameters.Add("p_Width", product.Jewellery.Width);
                parameters.Add("p_Bandwidth", product.Jewellery.Bandwidth);
                parameters.Add("p_Thickness", product.Jewellery.Thickness);
                parameters.Add("p_Size", product.Jewellery.Size);
                parameters.Add("p_IsEcommerce", product.Jewellery.IsEcommerce);
                parameters.Add("p_IsEngravingAvailable", product.Jewellery.IsEngravingAvailable);
                parameters.Add("p_IsSizeAlterationAvailable", product.Jewellery.IsSizeAlterationAvailable);
                parameters.Add("p_LacquerPrice", product.Jewellery.LacquerPrice);
                parameters.Add("p_MakingPrice", product.Jewellery.MakingPrice);
                parameters.Add("p_TotalPrice", product.Jewellery.TotalPrice);
                parameters.Add("p_Title", product.Title);
                parameters.Add("p_ReferenceSKU", product.ReferenceSKU);

                // OUT parameters
                parameters.Add("o_ProductId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("o_JewelleryId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                // Execute SP for Product & Jewellery
                await connection.ExecuteAsync(
                    "AddOrUpdateProductWithJewelleryGb",
                    parameters,
                    transaction,
                    commandType: CommandType.StoredProcedure);

                int productId = parameters.Get<int>("o_ProductId");
                int jewelleryId = parameters.Get<int>("o_JewelleryId");

                if (productId <= 0 || jewelleryId <= 0)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                // Product Documents
                if (product.ProductDocuments?.Count > 0)
                {
                    foreach (var doc in product.ProductDocuments)
                    {
                        await connection.ExecuteAsync(
                            "InsertUpdateProductDocumentGb",
                            new
                            {
                                p_ProductId = productId,
                                p_DocumentId = doc.DocumentId,
                                p_IsPrimary = doc.IsPrimary,
                                p_CreatedBy = product.CreatedBy
                            },
                            transaction,
                            commandType: CommandType.StoredProcedure);
                    }
                }

                // Stone Products + Stone Documents
                if (product.StoneProducts?.Count > 0)
                {
                    foreach (var stone in product.StoneProducts)
                    {
                        await connection.ExecuteAsync(
                            "InsertUpdateStoneProductGb",
                            new
                            {
                                p_ProductId = productId,
                                p_StoneTypeId = stone.StoneTypeId,
                                p_StoneShapeId = stone.StoneShapeId,
                                p_StoneWeightTypeId = stone.StoneWeightTypeId,
                                p_Quantity = stone.Quantity,
                                p_TotalWeight = stone.TotalWeight,
                                p_TotalPrice = stone.TotalPrice,
                                p_MinStoneWeight = stone.MinStoneWeight,
                                p_MaxStoneWeight = stone.MaxStoneWeight,
                                p_MinStonePrice = stone.MinStonePrice,
                                p_MaxStonePrice = stone.MaxStonePrice,
                                p_CreatedBy = product.CreatedBy
                            },
                            transaction,
                            commandType: CommandType.StoredProcedure);

                        int stoneId = await connection.QueryFirstOrDefaultAsync<int>(
                            "SELECT stoneProductId FROM stoneProduct_gb WHERE productId = @productId AND stoneTypeId = @stoneTypeId AND stoneShapeId = @stoneShapeId",
                            new
                            {
                                productId,
                                stoneTypeId = stone.StoneTypeId,
                                stoneShapeId = stone.StoneShapeId
                            },
                            transaction);

                        if (stone.StoneDocuments?.Count > 0)
                        {
                            foreach (var doc in stone.StoneDocuments)
                            {
                                await connection.ExecuteAsync(
                                    "InsertUpdateStoneDocumentGb",
                                    new
                                    {
                                        p_StoneId = stoneId,
                                        p_DocumentId = doc.DocumentId,
                                        p_IsPrimary = doc.IsPrimary,
                                        p_CreatedBy = product.CreatedBy
                                    },
                                    transaction,
                                    commandType: CommandType.StoredProcedure);
                            }
                        }
                    }
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw; // Optionally log ex
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

        #endregion

        #region Product Details

        public async Task<int> Add(Product product)
        {
            return await this.AddProduct(product, null, null);
        }
        public async Task<bool> Update(Product product)
        {
            return await this.UpdateProduct(product, null, null);
        }
        private async Task<int> AddProduct(Product product, IDbConnection? externalConnection = null, IDbTransaction? externalTransaction = null)
        {
            var isOwnConnection = externalConnection == null;
            DbConnection connection = externalConnection != null ? (DbConnection)externalConnection : base.GetConnection();
            DbTransaction transaction = externalTransaction != null ? (DbTransaction)externalTransaction : await connection.BeginTransactionAsync();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_ProductTypeId", product.ProductTypeId);
                parameters.Add("p_SKU", product.SKU);
                parameters.Add("p_ProductSourceId", product.ProductSourceId);
                parameters.Add("p_VendorId", product.VendorId);
                parameters.Add("p_StoreId", product.StoreId);
                parameters.Add("p_CreatedBy", product.CreatedBy);
                parameters.Add("p_PrimaryCategoryIds", product.Jewellery.PrimaryCategoryIds);
                parameters.Add("p_CategoryId", product.Jewellery.CategoryId);
                parameters.Add("p_SubCategoryId", product.Jewellery.SubCategoryId);
                parameters.Add("p_WearingTypeIds", product.Jewellery.WearingTypeIds);
                parameters.Add("p_CollectionIds", product.Jewellery.CollectionIds);
                parameters.Add("p_GenderId", product.Jewellery.GenderId);
                parameters.Add("p_OccasionIds", product.Jewellery.OccasionIds);
                parameters.Add("p_Description", product.Jewellery.Description);
                parameters.Add("p_MetalTypeId", product.Jewellery.MetalTypeId);
                parameters.Add("p_MetalPurityTypeId", product.Jewellery.MetalPurityTypeId);
                parameters.Add("p_MetalColorTypeId", product.Jewellery.MetalColorTypeId);
                parameters.Add("p_WeightTypeId", product.Jewellery.WeightTypeId);
                parameters.Add("p_NetWeight", product.Jewellery.NetWeight);
                parameters.Add("p_WastageWeight", product.Jewellery.WastageWeight);
                parameters.Add("p_WastagePct", product.Jewellery.WastagePct);
                parameters.Add("p_TotalWeight", product.Jewellery.TotalWeight);
                parameters.Add("p_Width", product.Jewellery.Width);
                parameters.Add("p_Bandwidth", product.Jewellery.Bandwidth);
                parameters.Add("p_Thickness", product.Jewellery.Thickness);
                parameters.Add("p_Size", product.Jewellery.Size);
                parameters.Add("p_IsEcommerce", product.Jewellery.IsEcommerce);
                parameters.Add("p_IsEngravingAvailable", product.Jewellery.IsEngravingAvailable);
                parameters.Add("p_IsSizeAlterationAvailable", product.Jewellery.IsSizeAlterationAvailable);
                parameters.Add("p_LacquerPrice", product.Jewellery.LacquerPrice);
                parameters.Add("p_MakingPrice", product.Jewellery.MakingPrice);
                parameters.Add("p_TotalPrice", product.Jewellery.TotalPrice);
                parameters.Add("p_Title", product.Title);
                parameters.Add("p_ReferenceSKU", product.ReferenceSKU);
                parameters.Add("p_MinWeight", product.Jewellery.MinWeight);
                parameters.Add("p_MaxWeight", product.Jewellery.MaxWeight);
                parameters.Add("p_IsSold", product.IsSold);
                parameters.Add("p_IsReserved", product.IsReserved);
                parameters.Add("p_ReferenceOrderId", product.ReferenceOrderId);
                parameters.Add("p_SerialNumber", product.Jewellery.SerialNumber);
                parameters.Add("P_KaatCategoryId", product.KaatCategoryId);
                parameters.Add("P_VendorAmount", product.VendorAmount);
                parameters.Add("p_InventoryUploadDate", product.InventoryUploadDate);

                parameters.Add("o_ProductId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("o_JewelleryId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "AddProductWithJewelleryGb",
                    parameters,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );

                int productId = parameters.Get<int>("o_ProductId");
                int jewelleryId = parameters.Get<int>("o_JewelleryId");

                if (productId <= 0 || jewelleryId <= 0)
                    throw new Exception("Product or Jewellery insert failed.");

                // Product Documents
                foreach (var doc in product.ProductDocuments ?? Enumerable.Empty<ProductDocument>())
                {
                    await connection.ExecuteAsync("InsertUpdateProductDocumentGb", new
                    {
                        p_ProductId = productId,
                        p_DocumentId = doc.DocumentId,
                        p_IsPrimary = doc.IsPrimary,
                        p_CreatedBy = product.CreatedBy,
                        p_IsPostManufactured = doc.IsPostManufactured
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure);
                }

                // Stone Products and Documents
                foreach (var stone in product.StoneProducts ?? Enumerable.Empty<StoneProduct>())
                {
                    await connection.ExecuteAsync("InsertUpdateStoneProductGb", new
                    {
                        p_ProductId = productId,
                        p_StoneTypeId = stone.StoneTypeId,
                        p_StoneShapeId = stone.StoneShapeId,
                        p_StoneWeightTypeId = stone.StoneWeightTypeId,
                        p_Quantity = stone.Quantity,
                        p_TotalWeight = stone.TotalWeight,
                        p_TotalPrice = stone.TotalPrice,
                        p_MinStoneWeight = stone.MinStoneWeight,
                        p_MaxStoneWeight = stone.MaxStoneWeight,
                        p_MinStonePrice = stone.MinStonePrice,
                        p_MaxStonePrice = stone.MaxStonePrice,
                        p_CreatedBy = product.CreatedBy
                    },
        transaction: transaction,
                    commandType: CommandType.StoredProcedure);

                    int stoneId = await connection.QueryFirstOrDefaultAsync<int>(
                        "SELECT stoneProductId FROM stoneProduct_gb WHERE productId = @productId AND stoneTypeId = @stoneTypeId AND stoneShapeId = @stoneShapeId",
                        new { productId, stone.StoneTypeId, stone.StoneShapeId },
                        transaction
                    );

                    foreach (var doc in stone.StoneDocuments ?? Enumerable.Empty<StoneDocument>())
                    {
                        await connection.ExecuteAsync("InsertUpdateStoneDocumentGb", new
                        {
                            p_StoneId = stoneId,
                            p_DocumentId = doc.DocumentId,
                            p_IsPrimary = doc.IsPrimary,
                            p_CreatedBy = product.CreatedBy
                        }, transaction: transaction,
                    commandType: CommandType.StoredProcedure);
                    }
                }

                if (product.ProductSourceId == 3)
                {
                    if (product.CustomCharge?.Count > 0)
                    {
                        foreach (var customCharge in product.CustomCharge)
                        {
                            await connection.ExecuteAsync(
                                "InsertCustomChargeGb",
                                new
                                {
                                    p_OrderId = 0,
                                    p_ProductId = productId,
                                    p_Label = customCharge.Label,
                                    p_Value = customCharge.Value,
                                    p_CreatedBy = customCharge.CreatedBy
                                },
                                transaction: transaction,
                                commandType: CommandType.StoredProcedure
                            );
                        }
                    }
                }

                if (isOwnConnection)
                    await transaction.CommitAsync();

                return productId;
            }
            catch (Exception ex)
            {
                if (isOwnConnection)
                    await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                if (isOwnConnection)
                    await connection.DisposeAsync();
            }
        }
        public async Task<bool> UpdateProduct(Product product, IDbConnection? externalConnection = null, IDbTransaction? externalTransaction = null)
        {
            var isOwnConnection = externalConnection == null;
            DbConnection connection = externalConnection != null ? (DbConnection)externalConnection : base.GetConnection();
            DbTransaction transaction = externalTransaction != null ? (DbTransaction)externalTransaction : await connection.BeginTransactionAsync();

            try
            {
                var parameters = new DynamicParameters();

                // Input parameters for product and jewellery
                parameters.Add("p_ProductId", product.ProductId);
                parameters.Add("p_ProductTypeId", product.ProductTypeId);
                parameters.Add("p_SKU", product.SKU);
                parameters.Add("p_ProductSourceId", product.ProductSourceId);
                parameters.Add("p_VendorId", product.VendorId);
                parameters.Add("p_StoreId", product.StoreId);
                parameters.Add("p_CreatedBy", product.CreatedBy);

                parameters.Add("p_PrimaryCategoryIds", product.Jewellery.PrimaryCategoryIds);
                parameters.Add("p_CategoryId", product.Jewellery.CategoryId);
                parameters.Add("p_SubCategoryId", product.Jewellery.SubCategoryId);
                parameters.Add("p_WearingTypeIds", product.Jewellery.WearingTypeIds);
                parameters.Add("p_CollectionIds", product.Jewellery.CollectionIds);
                parameters.Add("p_GenderId", product.Jewellery.GenderId);
                parameters.Add("p_OccasionIds", product.Jewellery.OccasionIds);
                parameters.Add("p_Description", product.Jewellery.Description);
                parameters.Add("p_MetalTypeId", product.Jewellery.MetalTypeId);
                parameters.Add("p_MetalPurityTypeId", product.Jewellery.MetalPurityTypeId);
                parameters.Add("p_MetalColorTypeId", product.Jewellery.MetalColorTypeId);
                parameters.Add("p_WeightTypeId", product.Jewellery.WeightTypeId);
                parameters.Add("p_NetWeight", product.Jewellery.NetWeight);
                parameters.Add("p_WastageWeight", product.Jewellery.WastageWeight);
                parameters.Add("p_WastagePct", product.Jewellery.WastagePct);
                parameters.Add("p_TotalWeight", product.Jewellery.TotalWeight);
                parameters.Add("p_Width", product.Jewellery.Width);
                parameters.Add("p_Bandwidth", product.Jewellery.Bandwidth);
                parameters.Add("p_Thickness", product.Jewellery.Thickness);
                parameters.Add("p_Size", product.Jewellery.Size);
                parameters.Add("p_IsEcommerce", product.Jewellery.IsEcommerce);
                parameters.Add("p_IsEngravingAvailable", product.Jewellery.IsEngravingAvailable);
                parameters.Add("p_IsSizeAlterationAvailable", product.Jewellery.IsSizeAlterationAvailable);
                parameters.Add("p_LacquerPrice", product.Jewellery.LacquerPrice);
                parameters.Add("p_MakingPrice", product.Jewellery.MakingPrice);
                parameters.Add("p_TotalPrice", product.Jewellery.TotalPrice);
                parameters.Add("p_Title", product.Title);
                parameters.Add("p_ReferenceSKU", product.ReferenceSKU);
                parameters.Add("p_MinWeight", product.Jewellery.MinWeight);
                parameters.Add("p_MaxWeight", product.Jewellery.MaxWeight);
                parameters.Add("p_IsSold", product.IsSold);
                parameters.Add("p_IsReserved", product.IsReserved);
                parameters.Add("p_SerialNumber", product.Jewellery.SerialNumber);
                parameters.Add("P_KaatCategoryId", product.KaatCategoryId);
                parameters.Add("P_VendorAmount", product.VendorAmount);
                parameters.Add("p_InventoryUploadDate", product.InventoryUploadDate);

                // OUT parameters
                parameters.Add("o_ProductId", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("o_JewelleryId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                // Execute SP for Product & Jewellery
                await connection.ExecuteAsync(
                    "AddOrUpdateProductWithJewelleryGb",
                    parameters,
                    transaction,
                    commandType: CommandType.StoredProcedure);

                int productId = parameters.Get<int>("o_ProductId");
                int jewelleryId = parameters.Get<int>("o_JewelleryId");

                if (productId <= 0 || jewelleryId <= 0)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                // Product Documents
                if (product.ProductDocuments?.Count > 0)
                {
                    foreach (var doc in product.ProductDocuments)
                    {
                        await connection.ExecuteAsync(
                            "InsertUpdateProductDocumentGb",
                            new
                            {
                                p_ProductId = productId,
                                p_DocumentId = doc.DocumentId,
                                p_IsPrimary = doc.IsPrimary,
                                p_CreatedBy = product.CreatedBy,
                                p_IsPostManufactured = doc.IsPostManufactured
                            },
                            transaction,
                            commandType: CommandType.StoredProcedure);
                    }
                }

                // Stone Products + Stone Documents
                if (product.StoneProducts?.Count > 0)
                {
                    foreach (var stone in product.StoneProducts)
                    {
                        await connection.ExecuteAsync(
                            "InsertUpdateStoneProductGb",
                            new
                            {
                                p_ProductId = productId,
                                p_StoneTypeId = stone.StoneTypeId,
                                p_StoneShapeId = stone.StoneShapeId,
                                p_StoneWeightTypeId = stone.StoneWeightTypeId,
                                p_Quantity = stone.Quantity,
                                p_TotalWeight = stone.TotalWeight,
                                p_TotalPrice = stone.TotalPrice,
                                p_MinStoneWeight = stone.MinStoneWeight,
                                p_MaxStoneWeight = stone.MaxStoneWeight,
                                p_MinStonePrice = stone.MinStonePrice,
                                p_MaxStonePrice = stone.MaxStonePrice,
                                p_CreatedBy = product.CreatedBy
                            },
                            transaction,
                            commandType: CommandType.StoredProcedure);

                        int stoneId = await connection.QueryFirstOrDefaultAsync<int>(
                            "SELECT stoneProductId FROM stoneProduct_gb WHERE productId = @productId AND stoneTypeId = @stoneTypeId AND stoneShapeId = @stoneShapeId",
                            new
                            {
                                productId,
                                stoneTypeId = stone.StoneTypeId,
                                stoneShapeId = stone.StoneShapeId
                            },
                            transaction);

                        if (stone.StoneDocuments?.Count > 0)
                        {
                            foreach (var doc in stone.StoneDocuments)
                            {
                                await connection.ExecuteAsync(
                                    "InsertUpdateStoneDocumentGb",
                                    new
                                    {
                                        p_StoneId = stoneId,
                                        p_DocumentId = doc.DocumentId,
                                        p_IsPrimary = doc.IsPrimary,
                                        p_CreatedBy = product.CreatedBy
                                    },
                                    transaction,
                                    commandType: CommandType.StoredProcedure);
                            }
                        }
                    }
                }
                if (product.ProductSourceId == 3)
                {
                    if (product.CustomCharge?.Count > 0)
                    {
                        foreach (var customCharge in product.CustomCharge)
                        {
                            await connection.ExecuteAsync(
                                "InsertUpdateCustomChargeGb",
                                new
                                {
                                    p_customChargesId = customCharge.CustomChargesId,
                                    p_OrderId = 0,
                                    p_ProductId = product.ProductId,
                                    p_Label = customCharge.Label,
                                    p_Value = customCharge.Value,
                                    p_CreatedBy = customCharge.CreatedBy
                                },
                                transaction: transaction,
                                commandType: CommandType.StoredProcedure
                            );
                        }
                    }
                }
                if (isOwnConnection)
                    await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                if (isOwnConnection)
                    await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                if (isOwnConnection)
                    await connection.DisposeAsync();
            }
        }
        public async Task<AllResponse<Product>> GetAllProducts(AllRequest<ProductRequestVm> product)
        {
            var Response = new AllResponse<Product>();
            var ProductList = new List<Product>();
            var JewelleryList = new List<Jewellery>();
            var StoneProductList = new List<StoneProduct>();
            var ProductLabelsList = new List<ProductLabel>();
            var DiscountsList = new List<Discount>(); // ✅ store all discounts

            if (product.SearchText == null)
                product.SearchText = "";

            var parameters = new List<DbParameter>
    {
        base.GetParameter("@p_PageNumber", product.Offset),
        base.GetParameter("@p_PageSize", ToDbValue(product.PageSize)),
        base.GetParameter("@P_SearchText", product.SearchText),
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
        base.GetParameter("@p_IsEcommerce", ToDbValue(product.Data.IsEcommerce)),
        base.GetParameter("@p_IsEngravingAvailable", ToDbValue(product.Data.IsEngravingAvailable)),
        base.GetParameter("@p_IsSizeAlterationAvailable", ToDbValue(product.Data.IsSizeAlterationAvailable)),
        base.GetParameter("@p_LacquerPrice", ToDbValue(product.Data.LacquerPrice)),
        base.GetParameter("@p_MakingPrice", ToDbValue(product.Data.MakingPrice)),
        base.GetParameter("@p_TotalPrice", ToDbValue(product.Data.TotalPrice)),
        base.GetParameter("@p_StoneTypeId", ToDbValue(product.Data.StoneTypeId)),
        base.GetParameter("@p_StoneShapeId", ToDbValue(product.Data.StoneShapeId)),
        base.GetParameter("@p_StoneWeightTypeId", ToDbValue(product.Data.StoneWeightTypeId)),
        base.GetParameter("@p_ReferenceSKU", ToDbValue(product.Data.ReferenceSKU)),
        base.GetParameter("@p_IsSold", ToDbValue(product.Data.IsSold)),
        base.GetParameter("@p_IsReserved", ToDbValue(product.Data.IsReserved)),
        base.GetParameter("@p_KaatCategoryId", ToDbValue(product.Data.KaatCategoryId)),
        base.GetParameter("@p_MinWeight", ToDbValue(product.Data.MinWeight)),
        base.GetParameter("@p_MaxWeight", ToDbValue(product.Data.MaxWeight)),
        base.GetParameter("@p_StartDate", ToDbValue(product.Data.StartDate)),
        base.GetParameter("@p_EndDate", ToDbValue(product.Data.EndDate)),
        base.GetParameter("@p_LabelIds", ToDbValue(product.Data.LabelIds))
    };

            using (var dataReader = await base.ExecuteReader(parameters, "GetAllProductsGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        var item = new Product();
                        item.ProductSource = new ProductSource();
                        item.Vendor = new Vendor();
                        item.CustomCharge = new List<CustomCharge>();
                        item.ProductLabel = new List<ProductLabel>();
                        item.ProductTypeId = dataReader.GetIntegerValue("productTypeId");
                        item.ProductId = dataReader.GetIntegerValue("productId");
                        item.SKU = dataReader.GetStringValue("sKU");
                        item.ProductSourceId = dataReader.GetIntegerValue("productSourceId");
                        item.VendorId = dataReader.GetIntegerValue("vendorId");
                        item.IsActive = dataReader.GetBooleanValue("isActive");
                        item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        item.StoreId = dataReader.GetIntegerValue("storeId");
                        item.CreatedAt = dataReader.GetDateTime("createdAt");
                        item.CreatedBy = dataReader.GetIntegerValue("createdBy");
                        item.ProductSource.ProductSourceId = dataReader.GetIntegerValue("productSourceId");
                        item.ProductSource.Description = dataReader.GetStringValue("ProductSourceDescription");
                        item.Vendor.VendorId = dataReader.GetIntegerValue("vendorId");
                        item.Vendor.Description = dataReader.GetStringValue("VendorDescription");
                        item.Title = dataReader.GetStringValue("title");
                        item.ReferenceSKU = dataReader.GetStringValue("referenceSKU");
                        item.IsSold = dataReader.GetBooleanValue("isSold");
                        item.IsReserved = dataReader.GetBooleanValue("isReserved");
                        item.ReferenceOrderId = dataReader.GetIntegerValueNullable("referenceOrderId");
                        item.VendorAmount = dataReader.GetDecimalValue("vendorAmount");
                        item.KaatCategoryId = dataReader.GetIntegerValue("kaatCategoryId");
                        item.InventoryUploadDate = dataReader.GetDateTimeValue("inventoryUploadDate");

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
                            item.WastageWeight = dataReader.GetDecimalValue("wastageWeight");
                            item.TotalWeight = dataReader.GetDecimalValue("totalWeight");
                            item.Width = dataReader.GetStringValue("width");
                            item.Bandwidth = dataReader.GetStringValue("bandwidth");
                            item.Thickness = dataReader.GetStringValue("thickness");
                            item.Size = dataReader.GetStringValue("size");
                            item.IsEcommerce = dataReader.GetBooleanValue("isEcommerce");
                            item.IsEngravingAvailable = dataReader.GetBooleanValue("isEngravingAvailable");
                            item.IsSizeAlterationAvailable = dataReader.GetBooleanValue("isSizeAlterationAvailable");
                            item.LacquerPrice = dataReader.GetDecimalValue("lacquerPrice");
                            item.MakingPrice = dataReader.GetDecimalValue("makingPrice");
                            item.TotalPrice = dataReader.GetDecimalValue("totalPrice");
                            item.IsActive = dataReader.GetBooleanValue("isActive");
                            item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                            item.CreatedAt = dataReader.GetDateTimeValue("createdAt");
                            item.CreatedBy = dataReader.GetIntegerValue("createdBy");
                            item.MinWeight = dataReader.GetDecimalValue("minWeight");
                            item.MaxWeight = dataReader.GetDecimalValue("maxWeight");
                            item.SerialNumber = dataReader.GetStringValue("serialNumber");

                            var productItem = ProductList.FirstOrDefault(x => x.ProductId == item.ProductId);
                            if (productItem != null)
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
                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            var item = new StoneDocument();
                            item.StoneDocumentId = dataReader.GetIntegerValue("stoneDocumentId");
                            item.StoneId = dataReader.GetIntegerValue("stoneId");
                            item.DocumentId = dataReader.GetIntegerValue("documentId");
                            item.Url = dataReader.GetStringValue("url");
                            item.IsPrimary = dataReader.GetBooleanValue("isPrimary");

                            var filteredProduct = ProductList.SelectMany(p => p.StoneProducts).FirstOrDefault(sp => sp.StoneProductId == item.StoneId);

                            if (filteredProduct != null)
                            {
                                filteredProduct.StoneDocuments.Add(item);
                            }
                        }
                    }
                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            var item = new ProductDocument();
                            item.ProductDocumentId = dataReader.GetIntegerValue("productDocumentId");
                            item.ProductId = dataReader.GetIntegerValue("productId");
                            item.DocumentId = dataReader.GetIntegerValue("documentId");
                            item.IsPrimary = dataReader.GetBooleanValue("isPrimary");
                            item.Url = dataReader.GetStringValue("url");

                            var productItem = ProductList.FirstOrDefault(x => x.ProductId == item.ProductId);
                            if (productItem != null)
                            {
                                productItem.ProductDocuments.Add(item);
                            }
                        }
                    }
                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            var item = new CustomCharge();
                            item.CustomChargesId = dataReader.GetIntegerValue("customChargesId");
                            item.ProductId = dataReader.GetIntegerValue("productId");
                            item.Label = dataReader.GetStringValue("label");
                            item.Value = dataReader.GetDecimalValue("value");
                            var productItem = ProductList.FirstOrDefault(o => o.ProductId == item.ProductId);
                            if (productItem != null)
                            {
                                productItem?.CustomCharge?.Add(item);
                            }
                        }
                    }
                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            var item = new ProductLabel();
                            item.ProductId = dataReader.GetIntegerValue("productId");
                            item.LabelId = dataReader.GetIntegerValue("labelId");
                            ProductLabelsList.Add(item);
                            var productItem = ProductList.FirstOrDefault(o => o.ProductId == item.ProductId);
                            if (productItem != null)
                            {
                                productItem?.ProductLabel?.Add(item);
                            }
                        }
                    }

                    // ✅ Discount Section (rewritten to apply HIGHEST discount)
                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            var discount = new Discount
                            {
                                DiscountId = dataReader.GetIntegerValue("DiscountId"),
                                DiscountTypeId = dataReader.GetIntegerValue("DiscountTypeId"),
                                Name = dataReader.GetStringValue("Name"),
                                CardDisplayName = dataReader.GetStringValue("CardDisplayName"),
                                Code = dataReader.GetStringValue("Code"),
                                MinInvoiceAmount = dataReader.GetDecimalValue("MinInvoiceAmount"),
                                MaxUsage = dataReader.GetIntegerValue("MaxUsage"),
                                PersonName = dataReader.GetStringValue("PersonName"),
                                Description = dataReader.GetStringValue("Description"),
                                SalesComissionPct = dataReader.GetDecimalValue("SalesComissionPct"),
                                MaxUser = dataReader.GetIntegerValue("MaxUser"),
                                CustomerId = dataReader.GetIntegerValue("CustomerId"),
                                ExpiryDuration = dataReader.GetIntegerValue("ExpiryDuration"),
                                ExpiryDurationType = dataReader.GetIntegerValue("ExpiryDurationType"),
                                LoyaltyCardTypeId = dataReader.GetIntegerValue("LoyaltyCardTypeId"),
                                VoucherTypeId = dataReader.GetIntegerValue("VoucherTypeId"),
                                PrimaryCategories = dataReader.GetStringValue("PrimaryCategories"),
                                CategoryIds = dataReader.GetStringValue("CategoryIds"),
                                SubCategoryIds = dataReader.GetStringValue("SubCategoryIds"),
                                CollectionTypeIds = dataReader.GetStringValue("CollectionTypeIds"),
                                LabelIds = dataReader.GetStringValue("LabelIds"),
                                DiscountAmount = dataReader.GetDecimalValue("DiscountAmount"),
                                DiscountPct = dataReader.GetDecimalValue("DiscountPct"),
                                StartDate = dataReader.GetDateTimeValue("StartDate"),
                                EndDate = dataReader.GetDateTimeValue("EndDate"),
                                IsEcommerce = dataReader.GetBooleanValue("IsEcommerce"),
                                IsInStore = dataReader.GetBooleanValue("IsInStore"),
                                StoreIds = dataReader.GetStringValue("StoreIds")
                            };

                            DiscountsList.Add(discount); // ✅ collect all discounts
                        }
                    }

                    // ✅ Apply highest discount per product
                    var bestDiscountByProduct = new Dictionary<int, Discount>();

                    foreach (var discount in DiscountsList)
                    {
                        var discountPrimaryCategories = new HashSet<string>((discount.PrimaryCategories ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
                        var discountSubCategories = new HashSet<string>((discount.SubCategoryIds ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
                        var discountCategories = new HashSet<string>((discount.CategoryIds ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
                        var discountCollections = new HashSet<string>((discount.CollectionTypeIds ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));
                        var discountLabels = new HashSet<string>((discount.LabelIds ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()));

                        var matchingProductIdsByLabels = ProductLabelsList
                            .Where(pl => discountLabels.Contains(pl.LabelId.ToString()))
                            .Select(pl => pl.ProductId)
                            .ToHashSet();

                        var filteredProducts = ProductList
                            .Where(p =>
                                matchingProductIdsByLabels.Contains(p.ProductId)
                                || (p.Jewellery?.CategoryId != null && discountCategories.Contains(p.Jewellery.CategoryId.ToString()))
                                || (p.Jewellery?.SubCategoryId != null && discountSubCategories.Contains(p.Jewellery.SubCategoryId.ToString()))
                                || ((p.Jewellery?.CollectionIds ?? "")
                                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Any(id => discountCollections.Contains(id.Trim())))
                                || ((p.Jewellery?.PrimaryCategoryIds ?? "")
                                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Any(id => discountPrimaryCategories.Contains(id.Trim())))
                            )
                            .ToList();

                        foreach (var p in filteredProducts)
                        {
                            if (!bestDiscountByProduct.TryGetValue(p.ProductId, out var existing) ||
                                IsHigherDiscount(discount, existing))
                            {
                                bestDiscountByProduct[p.ProductId] = discount;
                            }
                        }
                    }

                    foreach (var p in ProductList)
                    {
                        if (bestDiscountByProduct.TryGetValue(p.ProductId, out var best))
                        {
                            p.DiscountDetails = best;
                        }
                    }

                    if (dataReader.NextResult())
                    {
                        if (dataReader.Read())
                        {
                            Response.TotalRecord = dataReader.GetIntegerValue("TotalRecords");
                        }
                    }
                }
            }

            Response.Data = ProductList;
            return Response;
        }

        // ✅ Helper
        private static bool IsHigherDiscount(Discount newD, Discount oldD)
        {
            if (newD.DiscountPct > oldD.DiscountPct)
                return true;
            if (newD.DiscountPct < oldD.DiscountPct)
                return false;
            return newD.DiscountAmount > oldD.DiscountAmount;
        }

        public async Task<List<Product>> GetAllProductList()
        {
            var Response = new AllResponse<Product>();
            var ProductList = new List<Product>();
            var JewelleryList = new List<Jewellery>();
            var StoneProductList = new List<StoneProduct>();
            var parameters = new List<DbParameter>
            { };
            using (var dataReader = await base.ExecuteReader(parameters, "GetAllProductListGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        var item = new Product();
                        item.ProductSource = new ProductSource();
                        item.Vendor = new Vendor();


                        item.ProductTypeId = dataReader.GetIntegerValue("productTypeId");
                        item.ProductId = dataReader.GetIntegerValue("productId");
                        item.SKU = dataReader.GetStringValue("sKU");
                        item.ProductSourceId = dataReader.GetIntegerValue("productSourceId");
                        item.VendorId = dataReader.GetIntegerValue("vendorId");
                        item.IsActive = dataReader.GetBooleanValue("isActive");
                        item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        item.StoreId = dataReader.GetIntegerValue("storeId");
                        item.CreatedAt = dataReader.GetDateTime("createdAt");
                        item.CreatedBy = dataReader.GetIntegerValue("createdBy");
                        item.ProductSource.ProductSourceId = dataReader.GetIntegerValue("productSourceId");
                        item.Vendor.VendorId = dataReader.GetIntegerValue("vendorId");
                        item.Title = dataReader.GetStringValue("title");
                        item.ReferenceSKU = dataReader.GetStringValue("referenceSKU");
                        item.IsSold = dataReader.GetBooleanValue("isSold");
                        item.IsReserved = dataReader.GetBooleanValue("isReserved");

                        item.VendorAmount = dataReader.GetDecimalValue("vendorAmount");
                        item.KaatCategoryId = dataReader.GetIntegerValue("kaatCategoryId");
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
                            if (productItem != null)
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
                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            var item = new StoneDocument();
                            item.StoneDocumentId = dataReader.GetIntegerValue("stoneDocumentId");
                            item.StoneId = dataReader.GetIntegerValue("stoneId");
                            item.DocumentId = dataReader.GetIntegerValue("documentId");
                            item.Url = dataReader.GetStringValue("url");
                            item.IsPrimary = dataReader.GetBooleanValue("isPrimary");

                            var filteredProduct = ProductList.SelectMany(p => p.StoneProducts).FirstOrDefault(sp => sp.StoneProductId == item.StoneId);

                            if (filteredProduct != null)
                            {
                                filteredProduct.StoneDocuments.Add(item);
                            }
                        }
                    }
                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            var item = new ProductDocument();
                            item.ProductDocumentId = dataReader.GetIntegerValue("productDocumentId");
                            item.ProductId = dataReader.GetIntegerValue("productId");
                            item.DocumentId = dataReader.GetIntegerValue("documentId");
                            item.IsPrimary = dataReader.GetBooleanValue("isPrimary");
                            item.Url = dataReader.GetStringValue("url");
                            item.IsPostManufactured = dataReader.GetBooleanValue("isPostManufactured");

                            var productItem = ProductList.FirstOrDefault(x => x.ProductId == item.ProductId);
                            if (productItem != null)
                            {
                                productItem.ProductDocuments.Add(item);
                            }
                        }
                    }
                }
            }
            return ProductList;
        }
        public async Task<Product> GetProductById(int productId)
        {

            var Product = new Product();
            var JewelleryList = new List<Jewellery>();
            var StoneProductList = new List<StoneProduct>();
            Product.CustomCharge = new List<CustomCharge>();
            var parameters = new List<DbParameter>
            {
                base.GetParameter("@p_ProductId", ToDbValue(productId))
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetProductById", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                        var item = new Product();
                        item.ProductTypeId = dataReader.GetIntegerValue("productTypeId");
                        item.ProductId = dataReader.GetIntegerValue("productId");
                        item.SKU = dataReader.GetStringValue("sKU");
                        item.ProductSourceId = dataReader.GetIntegerValue("productSourceId");
                        item.VendorId = dataReader.GetIntegerValue("vendorId");
                        item.IsActive = dataReader.GetBooleanValue("isActive");
                        item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        item.StoreId = dataReader.GetIntegerValue("storeId");
                        item.CreatedAt = dataReader.GetDateTime("createdAt");
                        item.CreatedBy = dataReader.GetIntegerValue("createdBy");
                        item.UpdatedAt = dataReader.GetDateTimeValue("updatedAt");
                        item.Title = dataReader.GetStringValue("title");
                        item.ReferenceSKU = dataReader.GetStringValue("referenceSKU");
                        item.IsSold = dataReader.GetBooleanValue("isSold");
                        item.IsReserved = dataReader.GetBooleanValue("isReserved");
                        item.ReferenceOrderId = dataReader.GetIntegerValueNullable("referenceOrderId");

                        item.VendorAmount = dataReader.GetDecimalValue("vendorAmount");
                        item.KaatCategoryId = dataReader.GetIntegerValue("kaatCategoryId");
                        item.InventoryUploadDate = dataReader.GetDateTimeValue("inventoryUploadDate");

                        item.CustomCharge = new List<CustomCharge>();
                        Product = item;
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
                            item.NetWeight = dataReader.GetDecimalValue("netWeight");
                            item.WastagePct = dataReader.GetDecimalValue("wastagePct");
                            item.WastageWeight = dataReader.GetDecimalValue("wastageWeight");
                            item.TotalWeight = dataReader.GetDecimalValue("totalWeight");
                            item.Width = dataReader.GetStringValue("width");
                            item.Bandwidth = dataReader.GetStringValue("bandwidth");
                            item.Thickness = dataReader.GetStringValue("thickness");
                            item.Size = dataReader.GetStringValue("size");
                            item.IsEcommerce = dataReader.GetBooleanValue("isEcommerce");
                            item.IsEngravingAvailable = dataReader.GetBooleanValue("isEngravingAvailable");
                            item.IsSizeAlterationAvailable = dataReader.GetBooleanValue("isSizeAlterationAvailable");
                            item.LacquerPrice = dataReader.GetDecimalValue("lacquerPrice");
                            item.MakingPrice = dataReader.GetDecimalValue("makingPrice");
                            item.TotalPrice = dataReader.GetDecimalValue("totalPrice");
                            item.IsActive = dataReader.GetBooleanValue("isActive");
                            item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                            item.CreatedAt = dataReader.GetDateTimeValue("createdAt");
                            item.CreatedBy = dataReader.GetIntegerValue("createdBy");
                            item.MinWeight = dataReader.GetDecimalValue("minWeight");
                            item.MaxWeight = dataReader.GetDecimalValue("maxWeight");
                            item.SerialNumber = dataReader.GetStringValue("serialNumber");


                            if (Product.ProductId == item.ProductId)
                            {
                                Product.Jewellery = item;
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
                            item.MinStoneWeight = dataReader.GetDecimalValue("minStoneWeight");
                            item.MaxStoneWeight = dataReader.GetDecimalValue("maxStoneWeight");
                            item.MinStonePrice = dataReader.GetDecimalValue("minStonePrice");
                            item.MaxStonePrice = dataReader.GetDecimalValue("maxStonePrice");
                            item.IsActive = dataReader.GetBooleanValue("isActive");
                            item.IsDeleted = dataReader.GetBooleanValue("isDeleted");

                            if (Product.ProductId == item.ProductId)
                            {
                                Product.StoneProducts.Add(item);
                            }
                        }
                    }
                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            var item = new StoneDocument();
                            item.StoneDocumentId = dataReader.GetIntegerValue("stoneDocumentId");
                            item.StoneId = dataReader.GetIntegerValue("stoneId");
                            item.DocumentId = dataReader.GetIntegerValue("documentId");
                            item.Url = dataReader.GetStringValue("url");
                            item.IsPrimary = dataReader.GetBooleanValue("isPrimary");

                            var filteredProduct = Product.StoneProducts.FirstOrDefault(sp => sp.StoneProductId == item.StoneId);

                            if (filteredProduct != null)
                            {
                                filteredProduct.StoneDocuments.Add(item);
                            }
                        }
                    }
                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            var item = new ProductDocument();
                            item.ProductDocumentId = dataReader.GetIntegerValue("productDocumentId");
                            item.ProductId = dataReader.GetIntegerValue("productId");
                            item.DocumentId = dataReader.GetIntegerValue("documentId");
                            item.Url = dataReader.GetStringValue("url");
                            item.IsPrimary = dataReader.GetBooleanValue("isPrimary");
                            item.IsPostManufactured = dataReader.GetBooleanValue("isPostManufactured");

                            if (Product.ProductId == item.ProductId)
                            {
                                Product.ProductDocuments.Add(item);
                            }
                        }
                    }
                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            var res = new CustomCharge();
                            res.ProductId = dataReader.GetIntegerValue("productId");
                            res.CustomChargesId = dataReader.GetIntegerValue("customChargesId");
                            res.Label = dataReader.GetStringValue("label");
                            res.Value = dataReader.GetDecimalValue("Value");
                            res.IsActive = dataReader.GetBooleanValue("isActive");
                            res.IsDeleted = dataReader.GetBooleanValue("IsDeleted");
                            res.CreatedBy = dataReader.GetIntegerValue("createdBy");
                            res.UpdatedBy = dataReader.GetIntegerValue("updatedBy");
                            res.UpdatedAt = dataReader.GetDateTimeValue("updatedAt");
                            res.CreatedAt = dataReader.GetDateTimeValue("createdAt");
                            if (Product.ProductId == res.ProductId)
                            {
                                Product.CustomCharge?.Add(res);
                            }
                        }
                    }
                }
            }
            return Product;
        }
        public async Task<bool> DeleteProduct(Product product)
        {
            using var connection = base.GetConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_ProductId", product.ProductId);
            parameters.Add("P_UpdatedBy", product.UpdatedBy);
            parameters.Add("o_updated", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("DeleteProductById_gb", parameters, commandType: CommandType.StoredProcedure);
            var succeed = parameters.Get<int>("o_updated");
            return succeed == 1;

        }
        #endregion

        #region Order Details
        public async Task<int> AddOrder(Order order)
        {
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                if (order.OrderTypeId == 2 || order.OrderTypeId == 5)// bespoke order
                {
                    order.Product.IsSold = true;
                    order.Product.IsReserved = true;
                    // Pass connection and transaction to reuse Add logic
                    order.ProductId = await this.AddProduct(order.Product, connection, transaction);

                    if (order.ProductId <= 0)
                        throw new Exception("Failed to insert Product inside AddOrder.");
                }
                else if (order.OrderTypeId == 1 || order.OrderTypeId == 3 || order.OrderTypeId == 4) // direct Sale orders + rserved + alteration
                {
                    order.Product.IsReserved = true;
                    order.Product.IsSold = false;
                    bool isUpdated = await this.UpdateProduct(order.Product, connection, transaction);
                }
                if (order.AlterationDetails != null && order.OrderTypeId == 4) // direct order alteration 
                {
                    order.AlterationDetailsId = await this.AddAlterationDetails(order.AlterationDetails, connection, transaction);
                }
                if (order.RepairDetails != null && order.OrderTypeId == 5) // repair order
                {
                    order.RepairDetailsId = await this.AddRepairDetails(order.RepairDetails, connection, transaction);
                }
                if (order.AppraisalDetails != null && order.OrderTypeId == 6) // appraisal
                {
                    order.Product.IsReserved = true;
                    order.Product.IsSold = false;
                    order.ProductId = await this.AddProduct(order.Product, connection, transaction);
                    if (order.ProductId <= 0)
                        throw new Exception("Failed to insert Product inside AddOrder.");
                    order.AppraisalDetailsId = await this.AddAppraisalDetail(order.AppraisalDetails, connection, transaction);
                }
                if (order.ExchangeDetails != null && order.OrderTypeId == 7) // exchange
                {
                    order.Product.IsReserved = true;
                    order.Product.IsSold = false;
                    order.ProductId = await this.AddProduct(order.Product, connection, transaction);
                    if (order.ProductId <= 0)
                        throw new Exception("Failed to insert Product inside AddOrder.");
                    order.ExchangeDetailsId = await this.AddExchangeDetail(order.ExchangeDetails, connection, transaction);
                }
                if (order.GoldBookingDetails != null && order.OrderTypeId == 8) // gold booking 
                {
                    order.GoldBookingDetailsId = await this.AddGoldBookingDetail(order.GoldBookingDetails, connection, transaction);
                }
                if (order.GiftCardDetails != null && order.OrderTypeId == 9) // gift card  
                {
                    order.GiftCardDetailsId = await this.AddGiftCardDetail(order.GiftCardDetails, connection, transaction);
                }

                // Prepare parameters
                var parameters = new DynamicParameters();
                parameters.Add("p_CustomerId", order.CustomerId);
                parameters.Add("p_ProductId", order.ProductId);
                parameters.Add("p_StoreId", order.StoreId);
                parameters.Add("p_OrderTypeId", order.OrderTypeId);
                parameters.Add("p_EstStartingPrice", order.EstStartingPrice);
                parameters.Add("p_EstMaxPrice", order.EstMaxPrice);
                parameters.Add("p_Rate", order.Rate);
                parameters.Add("p_IsRateLocked", order.IsRateLocked);
                parameters.Add("p_AdvancePayment", order.AdvancePayment);
                parameters.Add("p_PendingPayment", order.PendingPayment);
                parameters.Add("p_PaymentReceived", order.PaymentReceived);
                parameters.Add("p_OrderStatusId", order.OrderStatusId);
                parameters.Add("p_CreatedBy", order.CreatedBy);
                parameters.Add("p_DelieveryMethodId", order.OrderDelievery?.DelieveryMethodId);
                parameters.Add("p_EstDelieveryDate", order.OrderDelievery?.EstDelieveryDate);
                parameters.Add("p_ShippingCost", order.OrderDelievery?.ShippingCost);
                parameters.Add("p_DelieveryAddress", order.OrderDelievery?.DelieveryAddress);
                parameters.Add("p_AdvancePaymentPct", order.AdvancePaymentPct);
                parameters.Add("p_TotalPayment", order.TotalPayment);
                parameters.Add("p_AlterationDetailsId", order.AlterationDetailsId);
                parameters.Add("p_RepairDetailId", order.RepairDetailsId);
                parameters.Add("p_AppraisalDetailId", order.AppraisalDetailsId);
                parameters.Add("p_ExchangeDetailId", order.ExchangeDetailsId);
                parameters.Add("p_GoldBookingDetailId", order.GoldBookingDetailsId);
                parameters.Add("p_GiftCardDetailId", order.GiftCardDetailsId);
                parameters.Add("p_IsEcommerceOrder", order.IsEcommerceOrder);
                parameters.Add("p_IsOnlinePosOrder", order.IsOnlinePosOrder);
                parameters.Add("o_OrderId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                // Insert Order
                await connection.ExecuteAsync(
                    "InsertOrderGb",
                    parameters,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );

                order.OrderId = parameters.Get<int>("o_OrderId");

                if (order.OrderId <= 0)
                    throw new Exception("Failed to insert Order.");

                // Insert Custom Charges
                if (order.CustomCharge?.Count > 0)
                {
                    foreach (var customCharge in order.CustomCharge)
                    {
                        await connection.ExecuteAsync(
                            "InsertCustomChargeGb",
                            new
                            {
                                p_OrderId = order.OrderId,
                                p_ProductId = 0,
                                p_Label = customCharge.Label,
                                p_Value = customCharge.Value,
                                p_CreatedBy = customCharge.CreatedBy
                            },
                            transaction: transaction,
                            commandType: CommandType.StoredProcedure
                        );
                    }
                }

                // Commit transaction only if everything succeeded
                await transaction.CommitAsync();
                return order.OrderId;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine("Error in AddOrder(): " + ex.Message);
                return 0;
            }
        }
        public async Task<AllResponse<Order>> GetAllOrders(AllRequest<OrderRequestVm> product)
        {
            //var AllProducts = await this.GetAllProductList();
            var Response = new AllResponse<Order>();
            var OrderList = new List<Order>();
            var parameters = new List<DbParameter>
            {
                base.GetParameter("@p_PageNumber", product.Offset),
                base.GetParameter("@p_PageSize", ToDbValue(product.PageSize)),
                base.GetParameter("@p_SearchText", ToDbValue(product.SearchText)),
                base.GetParameter("@p_OrderId", ToDbValue(product.Data.OrderId)),
                base.GetParameter("@p_CustomerId", ToDbValue(product.Data.CustomerId)),
                base.GetParameter("@p_ProductId", ToDbValue(product.Data.ProductId)),
                base.GetParameter("@p_StoreId", ToDbValue(product.Data.StoreId)),
                base.GetParameter("@p_OrderTypeId", ToDbValue(product.Data.OrderTypeId)),
                base.GetParameter("@p_EstStartingPrice", ToDbValue(product.Data.EstStartingPrice)),
                base.GetParameter("@p_EstMaxPrice", ToDbValue(product.Data.EstMaxPrice)),
                base.GetParameter("@p_Rate", ToDbValue(product.Data.Rate)),
                base.GetParameter("@p_IsRateLocked", ToDbValue(product.Data.IsRateLocked)),
                base.GetParameter("@p_AdvancePayment", ToDbValue(product.Data.AdvancePayment)),
                base.GetParameter("@p_PendingPayment", ToDbValue(product.Data.PendingPayment)),
                base.GetParameter("@p_PaymentReceived", ToDbValue(product.Data.PaymentReceived)),
                base.GetParameter("@p_OrderStatusId", ToDbValue(product.Data.OrderStatusId)),
                base.GetParameter("@p_IsFromInventory", ToDbValue(product.Data.IsFromInventory)),
                base.GetParameter("@p_MetalTypeId", ToDbValue(product.Data.MetalTypeId)),
                base.GetParameter("@p_StoneTypeId", ToDbValue(product.Data.StoneTypeId)),
                base.GetParameter("@p_CategoryId", ToDbValue(product.Data.CategoryId)),
                base.GetParameter("@p_StartDate", ToDbValue(product.Data.StartDate)),
                base.GetParameter("@p_EndDate", ToDbValue(product.Data.EndDate)),
                base.GetParameter("@p_MinWeight", ToDbValue(product.Data.MinWeight)),
                base.GetParameter("@p_MaxWeight", ToDbValue(product.Data.MaxWeight)),
                base.GetParameter("@p_DeliveryMethodId", ToDbValue(product.Data.DeliveryMethodId)),
                base.GetParameter("@p_CreatedBy", ToDbValue(product.Data.CreatedBy)),
            };

            using (var dataReader = await base.ExecuteReader(parameters, "GetAllOrdersGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        var item = new Order();
                        item.OrderType = new OrderType();
                        item.Customer = new Customer();
                        item.CustomCharge = new List<CustomCharge>();
                        var Customer = new Customer();
                        item.OrderDelievery = new OrderDelievery();

                        item.ProductId = dataReader.GetIntegerValue("productId");
                        item.OrderId = dataReader.GetIntegerValue("orderId");
                        item.CustomerId = dataReader.GetIntegerValue("customerId");
                        item.StoreId = dataReader.GetIntegerValue("storeId");
                        item.OrderTypeId = dataReader.GetIntegerValue("orderTypeId");
                        item.EstMaxPrice = dataReader.GetDecimalValue("estMaxPrice");
                        item.EstStartingPrice = dataReader.GetDecimalValue("estStartingPrice");
                        item.Rate = dataReader.GetDecimalValue("rate");
                        item.CreatedBy = dataReader.GetIntegerValue("createdBy");
                        item.UpdatedBy = dataReader.GetIntegerValue("updatedBy");
                        item.CreatedAt = dataReader.GetDateTimeValue("createdAt");
                        item.UpdatedAt = dataReader.GetDateTimeValue("updatedAt");
                        item.IsRateLocked = dataReader.GetBooleanValue("IsRateLocked");
                        item.IsOnlinePosOrder = dataReader.GetBooleanValue("IsOnlinePosOrder");
                        item.IsEcommerceOrder = dataReader.GetBooleanValue("IsEcommerceOrder");
                        item.PaymentReceived = dataReader.GetDecimalValue("paymentReceived");
                        item.AdvancePayment = dataReader.GetDecimalValue("advancePayment");
                        item.PendingPayment = dataReader.GetDecimalValue("pendingPayment");
                        item.AdvancePaymentPct = dataReader.GetDecimalValue("advancePaymentPct");
                        item.TotalPayment = dataReader.GetDecimalValue("totalPayment");
                        item.OrderStatusId = dataReader.GetIntegerValue("OrderStatusId");
                        item.RepairDetailsId = dataReader.GetIntegerValue("repairDetailId");
                        item.AlterationDetailsId = dataReader.GetIntegerValue("alterationDetailsId");
                        item.OrderDelievery.DelieveryMethodId = dataReader.GetIntegerValue("delieveryMethodId");
                        item.OrderDelievery.EstDelieveryDate = dataReader.GetDateTimeValue("estDelieveryDate");
                        item.OrderDelievery.ShippingCost = dataReader.GetIntegerValue("shippingCost");
                        item.OrderDelievery.DelieveryAddress = dataReader.GetStringValue("delieveryAddress");
                        item.AppraisalDetailsId = dataReader.GetIntegerValue("appraisalDetailId");
                        item.ExchangeDetailsId = dataReader.GetIntegerValue("exchangeDetailId");
                        item.GoldBookingDetailsId = dataReader.GetIntegerValue("goldBookingDetailId");
                        item.GiftCardDetailsId = dataReader.GetIntegerValue("giftCardDetailId");
                        Customer.FirstName = dataReader.GetStringValue("customerName");
                        Customer.Mobile = dataReader.GetStringValue("Mobile");
                        Customer.CustomerId = item.CustomerId;
                        item.Customer = Customer;
                        // item.Customer = await this.CustomerInfrastructure.Get(Customer);

                        item.Product = await this.GetProductById(item.ProductId);
                        //if (item.OrderTypeId == 4) // alteration
                        //{
                        //    item.AlterationDetails = await this.GetAlterationDetailsById((int)item.OrderId);
                        //}
                        //if (item.OrderTypeId == 5) // repair
                        //{
                         //   item.RepairDetails = await this.GetRepairDetailsById((int)item.OrderId);
                        //}
                        //if (item.OrderTypeId == 6) // appraisal
                        //{
                           // item.AppraisalDetails = await this.GetAppraisalDetailsById((int)item.OrderId);
                        //}
                        //if (item.OrderTypeId == 7) // exchange 
                        //{
                        //    item.ExchangeDetails = await this.GetExchangeDetailsById((int)item.OrderId);
                        //}
                        //if (item.OrderTypeId == 8) // gold booking
                        //{
                        //    item.GoldBookingDetails = await this.GetGoldBookingDetailsById((int)item.OrderId);
                        //}
                        //if (item.OrderTypeId == 9) // gift card
                        //{
                        //    item.GiftCardDetails = await this.GetGiftCardDetailsById((int)item.OrderId);
                        //}
                        OrderList.Add(item);
                    }
                }

                if (dataReader.NextResult())
                {
                    while (dataReader.Read())
                    {
                        var item = new CustomCharge();
                        item.CustomChargesId = dataReader.GetIntegerValue("customChargesId");
                        item.OrderId = dataReader.GetIntegerValue("orderId");
                        item.Label = dataReader.GetStringValue("label");
                        item.Value = dataReader.GetDecimalValue("value");
                        var orderItem = OrderList.FirstOrDefault(o => o.OrderId == item.OrderId);
                        if (orderItem != null)
                        {
                            orderItem?.CustomCharge?.Add(item);
                        }
                    }
                }
                if (dataReader.NextResult())
                {
                    if (dataReader.Read())
                    {
                        Response.TotalRecord = Convert.ToInt32(dataReader.GetDoubleValue("totalRecords"));
                    }
                }
                //AlterationDetails
                if (dataReader.NextResult())
                {
                    while (dataReader.Read())
                    {
                        var item = new AlterationDetails();
                        item.AlterationDetailsId = dataReader.GetIntegerValue("alterationDetailsId");
                        item.CurrentJewellerySize = dataReader.GetStringValue("currentJewellerySize");
                        item.DesiredJewellerySize = dataReader.GetStringValue("desiredJewellerySize");
                        item.SizeNote = dataReader.GetStringValue("sizeNote");
                        item.ResizingPrice = dataReader.GetDecimalValue("resizingPrice");
                        item.LacquerTypeId = dataReader.GetIntegerValue("lacquerTypeId");
                        item.LacquerNote = dataReader.GetStringValue("lacquerNote");
                        item.LacquerReferenceSKU = dataReader.GetStringValue("lacquerReferenceSKU");
                        item.OtherDescription = dataReader.GetStringValue("otherDescription");
                        item.LacquerPrice = dataReader.GetDecimalValue("lacquerPrice");
                        item.WeightChangePrice = dataReader.GetDecimalValue("weightChangePrice");
                        item.WeightChange = dataReader.GetDecimalValue("weightChange");
                        item.ProductTotalPrice = dataReader.GetDecimalValue("productTotalPrice");
                        item.OtherPrice = dataReader.GetDecimalValue("otherPrice");
                        item.IsActive = dataReader.GetBooleanValue("isActive");
                        item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        item.CreatedAt = dataReader.GetDateTime("createdAt");
                        item.CreatedBy = dataReader.GetIntegerValue("createdBy");

                        var OrderItem = OrderList.Find(o => o.AlterationDetailsId == item.AlterationDetailsId);
                        if(OrderItem != null)
                        {
                            OrderItem.AlterationDetails = item;
                        }
                    }
                }
                //Alteration Stone
                if (dataReader.NextResult())
                {
                    while (dataReader.Read())
                    {
                        var stone = new StoneAlteration();
                        stone.StoneAlterationId = dataReader.GetIntegerValue("stoneAlterationId");
                        stone.AlterationDetailsId = dataReader.GetIntegerValue("alterationDetailsId");
                        stone.CurrentStoneTypeId = dataReader.GetIntegerValue("currentStoneTypeId");
                        stone.DesiredStoneTypeId = dataReader.GetIntegerValue("desiredStoneTypeId");
                        stone.AdditionalNote = dataReader.GetStringValue("additionalNote");
                        stone.ReferenceSKU = dataReader.GetStringValue("referenceSKU");
                        stone.WeightTypeId = dataReader.GetIntegerValue("weightTypeId");
                        stone.Weight = dataReader.GetDecimalValue("weight");
                        stone.Price = dataReader.GetDecimalValue("price");
                        stone.ActualWeight = dataReader.GetDecimalValue("actualWeight");
                        stone.ActualPrice = dataReader.GetDecimalValue("actualPrice");
                        stone.IsActive = dataReader.GetBooleanValue("isActive");
                        stone.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        stone.CreatedAt = dataReader.GetDateTime("createdAt");

                        var OrderItem = OrderList.Find(o => o.AlterationDetailsId == stone.AlterationDetailsId);
                        if (OrderItem != null && OrderItem.AlterationDetails != null)
                        {
                            if (OrderItem.AlterationDetails.Stones == null)
                                OrderItem.AlterationDetails.Stones = new List<StoneAlteration> { stone };

                            OrderItem.AlterationDetails.Stones.Add(stone);
                        }
                    }
                }

                //RepairDetails
                if (dataReader.NextResult())
                {
                    while (dataReader.Read())
                    {
                        var item = new RepairDetails();
                        item.RepairDetailId = dataReader.GetIntegerValue("repairDetailId");
                        item.ProductTypeId = dataReader.GetIntegerValue("productTypeId");
                        item.MetalTypeId = dataReader.GetIntegerValue("metalTypeId");
                        item.WeightBeforeRepair = dataReader.GetDecimalValue("weightBeforeRepair");
                        item.RepairCleaningId = dataReader.GetIntegerValue("repairCleaningId");
                        item.CleaningNotes = dataReader.GetStringValue("cleaningNotes");
                        item.CleaningPrice = dataReader.GetDecimalValue("cleaningPrice");
                        item.RepairPolishingId = dataReader.GetIntegerValue("repairPolishingId");
                        item.PolishingNotes = dataReader.GetStringValue("polishingNotes");
                        item.PolishingPrice = dataReader.GetDecimalValue("polishingPrice");
                        item.CurrentJewellerySize = dataReader.GetStringValue("currentJewellerySize");
                        item.DesiredJewellerySize = dataReader.GetStringValue("desiredJewellerySize");
                        item.ResizingNotes = dataReader.GetStringValue("resizingNotes");
                        item.ResizingPrice = dataReader.GetDecimalValue("resizingPrice");
                        item.RepairDamageTypeIds = dataReader.GetStringValue("repairDamageTypeIds");
                        item.RepairDamageAreaIds = dataReader.GetStringValue("repairDamageAreaIds");
                        item.RepairingNotes = dataReader.GetStringValue("repairingNotes");
                        item.RepairingPrice = dataReader.GetDecimalValue("repairingPrice");

                        item.EstRepairingCost = dataReader.GetDecimalValue("estRepairCost");
                        item.WeightChange = dataReader.GetDecimalValue("weightChange");
                        item.WeightChangePrice = dataReader.GetDecimalValue("weightChangePrice");
                        item.ActualWeight = dataReader.GetDecimalValue("actualWeight");
                        item.TotalRepairCost = dataReader.GetDecimalValue("totalRepairCost");
                        item.EstDeliveryDate = dataReader.GetDateTimeValue("estDeliveryDate");
                        item.WeightTypeId = dataReader.GetIntegerValue("weightTypeId");
                        //item.WeightAfterRepair = dataReader.GetIntegerValue("weightAfterRepair");

                        item.IsActive = dataReader.GetBooleanValue("isActive");
                        item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        item.CreatedAt = dataReader.GetDateTimeValue("createdAt");
                        item.CreatedBy = dataReader.GetIntegerValue("createdBy");

                        var OrderItem = OrderList.Find(o => o.RepairDetailsId == item.RepairDetailId);
                        if (OrderItem != null)
                        {
                            OrderItem.RepairDetails = item;
                        }
                    }
                }
                //Repair Stones
                if (dataReader.NextResult())
                {
                    while (dataReader.Read())
                    {
                        var stone = new RepairStoneDetails();
                        stone.RepairStoneDetailId = dataReader.GetIntegerValue("repairStoneDetailId");
                        stone.RepairDetailId = dataReader.GetIntegerValue("repairDetailId");
                        stone.CurrentStoneId = dataReader.GetIntegerValue("currentStoneId");
                        stone.DesiredStoneId = dataReader.GetIntegerValue("desiredStoneId");
                        stone.IsFixed = dataReader.GetBooleanValue("isFixed");
                        stone.StoneTypeIds = dataReader.GetStringValue("stoneTypeIds");
                        stone.IsReplacement = dataReader.GetBooleanValue("isReplacement");
                        stone.Notes = dataReader.GetStringValue("notes");
                        stone.Price = dataReader.GetDecimalValue("price");
                        stone.ActualWeight = dataReader.GetDecimalValue("actualWeight");
                        stone.ActualPrice = dataReader.GetDecimalValue("actualPrice");
                        stone.IsActive = dataReader.GetBooleanValue("isActive");
                        stone.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        stone.CreatedAt = dataReader.GetDateTime("createdAt");
                        stone.CreatedBy = dataReader.GetIntegerValue("createdBy");

                        var OrderItem = OrderList.Find(o => o.RepairDetailsId == stone.RepairDetailId);
                        if (OrderItem != null && OrderItem.RepairDetails != null)
                        {
                            if (OrderItem.RepairDetails.RepairStoneDetails == null)
                                OrderItem.RepairDetails.RepairStoneDetails = new List<RepairStoneDetails> { stone };

                            OrderItem.RepairDetails.RepairStoneDetails.Add(stone);
                        }
                    }
                }
                if (dataReader.NextResult())
                {
                    while (dataReader.Read())
                    {

                        var item = new RepairDocument();
                        item.RepairDocumentId = dataReader.GetIntegerValue("repairDocumentId");
                        item.RepairDetailId = dataReader.GetIntegerValue("repairDetailId");
                        item.DocumentId = dataReader.GetIntegerValue("documentId");
                        item.Url = dataReader.GetStringValue("url");
                        item.IsPrimary = dataReader.GetBooleanValue("isPrimary");
                        item.IsPostRepair = dataReader.GetBooleanValue("isPostRepair");
                        item.IsActive = dataReader.GetBooleanValue("isActive");
                        item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        item.CreatedAt = dataReader.GetDateTimeValue("createdAt");
                        item.CreatedBy = dataReader.GetIntegerValue("createdBy");
                        var OrderItem = OrderList.Find(o => o.RepairDetailsId == item.RepairDetailId);
                        if (OrderItem != null && OrderItem.RepairDetails != null)
                        {
                            if (OrderItem.RepairDetails.RepairDocuments == null)
                                OrderItem.RepairDetails.RepairDocuments = new List<RepairDocument> { item };

                            OrderItem.RepairDetails.RepairDocuments.Add(item);
                        }
                    }
                }
                //AppraisalDetail
                if (dataReader.NextResult())
                {
                    while (dataReader.Read())
                    {
                        var item = new AppraisalDetail();
                        item.AppraisalDetailId = dataReader.GetIntegerValue("appraisalDetailId");
                        item.TotalProductWeight = dataReader.GetDecimalValue("totalProductWeight");
                        item.NetGoldWeight = dataReader.GetIntegerValue("netGoldWeight");
                        item.PureGoldWeight = dataReader.GetDecimalValue("pureGoldWeight");
                        item.DeductionPercentage = dataReader.GetDecimalValue("deductionPercentage");
                        item.AppraisalPrice = dataReader.GetDecimalValue("appraisalPrice");
                        item.Notes = dataReader.GetStringValue("notes");
                        item.WeightTypeId = dataReader.GetIntegerValue("weightTypeId");

                        item.IsActive = dataReader.GetBooleanValue("isActive");
                        item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        item.CreatedAt = dataReader.GetDateTimeValue("createdAt");
                        item.CreatedBy = dataReader.GetIntegerValue("createdBy");

                        var OrderItem = OrderList.Find(o => o.AppraisalDetailsId == item.AppraisalDetailId);
                        if (OrderItem != null)
                        {
                            OrderItem.AppraisalDetails = item;
                        }
                    }
                }
                //Appraisal Stones
                if (dataReader.NextResult())
                {
                    while (dataReader.Read())
                    {
                        var stone = new AppraisalStoneDetail();

                        stone.AppraisalStoneDetailId = dataReader.GetIntegerValue("appraisalStoneDetailId");
                        stone.AppraisalDetailId = dataReader.GetIntegerValue("appraisalDetailId");
                        stone.StoneTypeId = dataReader.GetIntegerValue("stoneTypeId");
                        stone.StoneQuantity = dataReader.GetIntegerValue("stoneQuantity");
                        stone.StoneWeight = dataReader.GetDecimalValue("stoneWeight");
                        stone.StonePrice = dataReader.GetDecimalValue("stonePrice");
                        stone.StoneWeightTypeId = dataReader.GetIntegerValue("stoneWeightTypeId");

                        var OrderItem = OrderList.Find(o => o.AppraisalDetailsId == stone.AppraisalDetailId);
                        if (OrderItem != null && OrderItem.AppraisalDetails != null)
                        {
                            if (OrderItem.AppraisalDetails.AppraisalStoneDetails == null)
                                OrderItem.AppraisalDetails.AppraisalStoneDetails = new List<AppraisalStoneDetail> { stone };

                            OrderItem.AppraisalDetails.AppraisalStoneDetails.Add(stone);
                        }
                    }
                }
                if (dataReader.NextResult())
                {
                    while (dataReader.Read())
                    {
                        var item = new AppraisalDocument();
                        item.AppraisalDetailId = dataReader.GetIntegerValue("appraisalDetailId");
                        item.AppraisalDocumentId = dataReader.GetIntegerValue("appraisalDocumentId");
                        item.DocumentId = dataReader.GetIntegerValue("documentId");
                        item.Url = dataReader.GetStringValue("url");
                        item.IsPrimary = dataReader.GetBooleanValue("isPrimary");
                        var OrderItem = OrderList.Find(o => o.AppraisalDetailsId == item.AppraisalDetailId);
                        if (OrderItem != null && OrderItem.AppraisalDetails != null)
                        {
                            if (OrderItem.AppraisalDetails.AppraisalDocuments == null)
                                OrderItem.AppraisalDetails.AppraisalDocuments = new List<AppraisalDocument> { item };

                            OrderItem.AppraisalDetails.AppraisalDocuments.Add(item);
                        }
                    }
                }
                //ExchangeDetail
                if (dataReader.NextResult())
                {
                    while (dataReader.Read())
                    {
                        var item = new ExchangeDetail();
                        item.DeductionPercentage = dataReader.GetDecimalValue("deductionPercentage");
                        item.DeductionValue = dataReader.GetDecimalValue("deductionValue");
                        item.ExchangeDetailId = dataReader.GetIntegerValue("exchangeDetailId");
                        item.OriginalPrice = dataReader.GetDecimalValue("originalPrice");
                        item.ExchangePrice = dataReader.GetDecimalValue("exchangePrice");
                        item.Notes = dataReader.GetStringValue("notes");

                        item.IsActive = dataReader.GetBooleanValue("isActive");
                        item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        item.CreatedAt = dataReader.GetDateTimeValue("createdAt");
                        item.CreatedBy = dataReader.GetIntegerValue("createdBy");

                        var OrderItem = OrderList.Find(o => o.ExchangeDetailsId == item.ExchangeDetailId);
                        if (OrderItem != null)
                        {
                            OrderItem.ExchangeDetails = item;
                        }
                    }
                }
                //GoldBookingDetail
                if (dataReader.NextResult())
                {
                    while (dataReader.Read())
                    {
                        var item = new GoldBookingDetail();
                        item.GoldBookingDetailId = dataReader.GetIntegerValue("goldBookingDetailId");
                        item.WeightTypeId = dataReader.GetIntegerValue("weightTypeId");
                        item.BookingWeight = dataReader.GetDecimalValue("bookingWeight");
                        item.BookingPrice = dataReader.GetDecimalValue("bookingPrice");
                        item.ReservationDate = dataReader.GetDateTimeValue("reservationDate");
                        item.Notes = dataReader.GetStringValue("notes");
                        item.IsActive = dataReader.GetBooleanValue("isActive");
                        item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        item.CreatedAt = dataReader.GetDateTimeValue("createdAt");
                        item.CreatedBy = dataReader.GetIntegerValue("createdBy");
                        item.Sku = dataReader.GetStringValue("sku");

                        var OrderItem = OrderList.Find(o => o.GoldBookingDetailsId == item.GoldBookingDetailId);
                        if (OrderItem != null)
                        {
                            OrderItem.GoldBookingDetails = item;
                        }
                    }
                }
                //GoldBookingDetail
                if (dataReader.NextResult())
                {
                    while (dataReader.Read())
                    {
                        var Item = new GiftCardDetail();
                        Item.GiftCardDetailId = dataReader.GetIntegerValue("giftCardDetailId");
                        Item.RecipientName = dataReader.GetStringValue("recipientName");
                        Item.RecipientMobileNumber = dataReader.GetStringValue("recipientMobileNumber");
                        Item.RecipientCnic = dataReader.GetStringValue("recipientCnic");
                        Item.Amount = dataReader.GetDecimalValue("amount");
                        Item.DepositorName = dataReader.GetStringValue("depositorName");
                        Item.DepositorMobileNumber = dataReader.GetStringValue("depositorMobileNumber");
                        Item.Code = dataReader.GetStringValue("code");
                        Item.Sku = dataReader.GetStringValue("sku");
                        Item.RedeemDate = dataReader.GetDateTimeValue("redeemDate");
                        Item.IsActive = dataReader.GetBooleanValue("isActive");
                        Item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        Item.CreatedAt = dataReader.GetDateTimeValue("createdAt");
                        Item.UpdatedAt = dataReader.GetDateTimeValue("updatedAt");
                        Item.CreatedBy = dataReader.GetIntegerValue("createdBy");
                        Item.UpdatedBy = dataReader.GetIntegerValue("updatedBy");

                        var OrderItem = OrderList.Find(o => o.GiftCardDetailsId == Item.GiftCardDetailId);
                        if (OrderItem != null)
                        {
                            OrderItem.GiftCardDetails = Item;
                        }
                    }
                }
            }
            Response.Data = OrderList;
            return Response;
        }
        public async Task<bool> UpdateOrder(Order order)
        {
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                int alterationDetailsId = order.AlterationDetails?.AlterationDetailsId == null ? 0 : order.AlterationDetails.AlterationDetailsId;
                if (order.Product != null && order.OrderTypeId != 5 && order.OrderTypeId != 9 && order.OrderTypeId != 8)
                {
                    // Pass connection and transaction to reuse Add logic
                    bool isUpdate = await this.UpdateProduct(order.Product, connection, transaction);

                    if (!isUpdate)
                        throw new Exception("Failed to update Product inside Order.");
                }
                if (order.AlterationDetails != null && order.OrderTypeId == 4)
                {
                    if (order.AlterationDetails.AlterationDetailsId > 0)
                    {
                        bool res = await this.UpdateAlterationDetails(order.AlterationDetails, connection, transaction);
                        if (!res)
                            throw new Exception("Failed to update Alteration.");
                    }
                    else
                    {
                        alterationDetailsId = await this.AddAlterationDetails(order.AlterationDetails, connection, transaction);
                        if (alterationDetailsId == 0)
                            throw new Exception("Failed to Add Alteration Details.");
                    }
                }
                if (order.RepairDetails != null && order.OrderTypeId == 5) // Repair
                {
                    var res = await this.UpdateRepairDetails(order.RepairDetails, connection, transaction);
                    if (res < 1)
                        throw new Exception("Failed to update repair.");
                }
                if (order.AppraisalDetails != null && order.OrderTypeId == 6) // Appraisal
                {
                    var res = await this.UpdateAppraisalDetail(order.AppraisalDetails, connection, transaction);
                    if (!res)
                        throw new Exception("Failed to update appraisal.");
                }
                if (order.ExchangeDetails != null && order.OrderTypeId == 7) // Exchange
                {
                    var res = await this.UpdateExchangeDetail(order.ExchangeDetails, connection, transaction);
                    if (!res)
                        throw new Exception("Failed to update exchange.");
                }
                if (order.GoldBookingDetails != null && order.OrderTypeId == 8) // Gold Booking
                {
                    var res = await this.UpdateGoldBookingDetail(order.GoldBookingDetails, connection, transaction);
                    if (!res)
                        throw new Exception("Failed to update gold booking.");
                }
                if (order.GiftCardDetails != null && order.OrderTypeId == 9) // Gift Card 
                {
                    var res = await this.UpdateGiftCardDetail(order.GiftCardDetails, connection, transaction);
                    if (!res)
                        throw new Exception("Failed to update gift card details.");
                }

                // Prepare parameters
                var parameters = new DynamicParameters();
                parameters.Add("p_EstStartingPrice", order.EstStartingPrice);
                parameters.Add("p_EstMaxPrice", order.EstMaxPrice);
                parameters.Add("p_Rate", order.Rate);
                parameters.Add("p_IsRateLocked", order.IsRateLocked);
                parameters.Add("p_AdvancePayment", order.AdvancePayment);
                parameters.Add("p_PendingPayment", order.PendingPayment);
                parameters.Add("p_PaymentReceived", order.PaymentReceived);
                parameters.Add("p_OrderStatusId", order.OrderStatusId);
                parameters.Add("p_CreatedBy", order.CreatedBy);
                parameters.Add("p_OrderId", order.OrderId);
                parameters.Add("p_DelieveryMethodId", order.OrderDelievery?.DelieveryMethodId);
                parameters.Add("p_EstDelieveryDate", order.OrderDelievery?.EstDelieveryDate);
                parameters.Add("p_ShippingCost", order.OrderDelievery?.ShippingCost);
                parameters.Add("p_DelieveryAddress", order.OrderDelievery?.DelieveryAddress);
                parameters.Add("p_OrderTypeId", order.OrderTypeId);
                parameters.Add("p_AdvancePaymentPct", order.AdvancePaymentPct);
                parameters.Add("p_TotalPayment", order.TotalPayment);
                parameters.Add("p_AlterationDetailsId", alterationDetailsId); 
                parameters.Add("p_IsEcommerceOrder", order.IsEcommerceOrder);
                parameters.Add("p_IsOnlinePosOrder", order.IsOnlinePosOrder);
                parameters.Add("o_OrderId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                // Insert Order
                await connection.ExecuteAsync(
                    "UpdateOrderGb",
                    parameters,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );

                order.OrderId = parameters.Get<int>("o_OrderId");

                if (order.OrderId <= 0)
                    throw new Exception("Failed to update Order.");

                // Insert Custom Charges
                if (order.CustomCharge?.Count > 0)
                {
                    foreach (var customCharge in order.CustomCharge)
                    {
                        await connection.ExecuteAsync(
                            "InsertUpdateCustomChargeGb",
                            new
                            {
                                p_customChargesId = customCharge.CustomChargesId,
                                p_OrderId = order.OrderId,
                                p_ProductId = 0,
                                p_Label = customCharge.Label,
                                p_Value = customCharge.Value,
                                p_CreatedBy = customCharge.CreatedBy
                            },
                            transaction: transaction,
                            commandType: CommandType.StoredProcedure
                        );
                    }
                }

                // Commit transaction only if everything succeeded
                await transaction.CommitAsync();
                return order.OrderId > 0;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine("Error in AddOrder(): " + ex.Message);
                return false;
            }
        }
        public async Task<Order> GetOrderById(int orderId)
        {
            var item = new Order();
            item.CustomCharge = new List<CustomCharge>();

            var parameters = new List<DbParameter>
            {
                base.GetParameter("@p_OrderId", orderId)
            };

            using (var dataReader = await base.ExecuteReader(parameters, "GetOrderByIdGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        item.OrderType = new OrderType();
                        item.OrderStatus = new OrderStatus();
                        item.Customer = new Customer();
                        item.OrderDelievery = new OrderDelievery();
                        var Customer = new Customer();

                        item.ProductId = dataReader.GetIntegerValue("productId");
                        item.OrderId = dataReader.GetIntegerValue("orderId");
                        item.CustomerId = dataReader.GetIntegerValue("customerId");
                        item.StoreId = dataReader.GetIntegerValue("storeId");
                        item.OrderTypeId = dataReader.GetIntegerValue("orderTypeId");
                        item.EstMaxPrice = dataReader.GetDecimalValue("estMaxPrice");
                        item.EstStartingPrice = dataReader.GetDecimalValue("estStartingPrice");
                        item.Rate = dataReader.GetDecimalValue("rate");
                        item.CreatedBy = dataReader.GetIntegerValue("createdBy");
                        item.UpdatedBy = dataReader.GetIntegerValue("updatedBy");
                        item.CreatedAt = dataReader.GetDateTimeValue("createdAt");
                        item.UpdatedAt = dataReader.GetDateTimeValue("updatedAt");
                        item.IsRateLocked = dataReader.GetBooleanValue("IsRateLocked");
                        item.PaymentReceived = dataReader.GetDecimalValue("paymentReceived");
                        item.AdvancePayment = dataReader.GetDecimalValue("advancePayment");
                        item.PendingPayment = dataReader.GetDecimalValue("pendingPayment");
                        item.OrderStatusId = dataReader.GetIntegerValue("OrderStatusId");
                        item.AdvancePaymentPct = dataReader.GetDecimalValue("advancePaymentPct");
                        item.TotalPayment = dataReader.GetDecimalValue("totalPayment");

                        item.IsOnlinePosOrder = dataReader.GetBooleanValue("IsOnlinePosOrder");
                        item.IsEcommerceOrder = dataReader.GetBooleanValue("IsEcommerceOrder");
                        item.OrderDelievery.DelieveryMethodId = dataReader.GetIntegerValue("delieveryMethodId");
                        item.OrderDelievery.EstDelieveryDate = dataReader.GetDateTimeValue("estDelieveryDate");
                        item.OrderDelievery.ShippingCost = dataReader.GetIntegerValue("shippingCost");
                        item.OrderDelievery.DelieveryAddress = dataReader.GetStringValue("delieveryAddress");


                        Customer.CustomerId = item.CustomerId;
                        item.Customer = await this.CustomerInfrastructure.Get(Customer);
                        item.Product = await this.GetProductById(item.ProductId);
                        if (item.OrderTypeId == 4) //Alteration
                        {
                            item.AlterationDetails = await this.GetAlterationDetailsById(orderId);
                        }
                        else if (item.OrderTypeId == 5) //repair
                        {
                            item.RepairDetails = await this.GetRepairDetailsById(orderId);
                        }
                        else if (item.OrderTypeId == 6) //appraisal
                        {
                            item.AppraisalDetails = await this.GetAppraisalDetailsById(orderId);
                        }
                        else if (item.OrderTypeId == 7) //exchange
                        {
                            item.ExchangeDetails = await this.GetExchangeDetailsById(orderId);
                        }
                        else if (item.OrderTypeId == 8) //gold booking
                        {
                            item.GoldBookingDetails = await this.GetGoldBookingDetailsById(orderId);
                        }
                        else if (item.OrderTypeId == 9) //gift card 
                        {
                            item.GiftCardDetails = await this.GetGiftCardDetailsById(orderId);
                        }
                    }
                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            var res = new CustomCharge();
                            res.OrderId = dataReader.GetIntegerValue("orderId");
                            res.CustomChargesId = dataReader.GetIntegerValue("customChargesId");
                            res.Label = dataReader.GetStringValue("label");
                            res.Value = dataReader.GetDecimalValue("Value");
                            res.IsActive = dataReader.GetBooleanValue("isActive");
                            res.IsDeleted = dataReader.GetBooleanValue("IsDeleted");
                            res.CreatedBy = dataReader.GetIntegerValue("createdBy");
                            res.UpdatedBy = dataReader.GetIntegerValue("updatedBy");
                            res.UpdatedAt = dataReader.GetDateTimeValue("updatedAt");
                            res.CreatedAt = dataReader.GetDateTimeValue("createdAt");
                            item.CustomCharge.Add(res);
                        }
                    }
                }
            }
            return item;
        }
        public async Task<bool> UpdateOrderById(OrderStatusReqVm order)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter("p_OrderId",order.OrderId),
                base.GetParameter("p_OrderStatusId", order.OrderStatusId),
                base.GetParameter("p_ReservationDate", order.ReservationDate),
                base.GetParameter("p_UpdatedBy", order.UpdatedBy)
            };
            var ReturnValue = await base.ExecuteNonQuery(parameters, "UpdateOrderById_gb", CommandType.StoredProcedure);
            return ReturnValue > 0;
        }
        public async Task<bool> DeleteOrder(Order order)
        {

            using var connection = base.GetConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_OrderId", order.OrderId);
            parameters.Add("P_UpdatedBy", order.UpdatedBy);
            parameters.Add("o_updated", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("DeleteOrderById_gb", parameters, commandType: CommandType.StoredProcedure);
            var succeed = parameters.Get<int>("o_updated");
            return succeed == 1;
        }
        public async Task<List<OrderStatusCount>> GetOrderStatusByTypeId(int orderTypeId)
        {
            var result = new List<OrderStatusCount>();
            var parameters = new List<DbParameter>
            {
                base.GetParameter("p_OrderTypeId", orderTypeId)
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetOrderStatusSummary_gb", CommandType.StoredProcedure))
            {
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        var item = new OrderStatusCount();
                        item.OrderStatusId = dataReader.GetIntegerValue("orderStatusId");
                        item.TotalOrders = dataReader.GetIntegerValue("totalOrders");
                        item.Description = dataReader.GetStringValue("description");
                        result.Add(item);
                    }
                }
            }
                return result;
        }
        #endregion

        #region Alteration Details
        public async Task<int> AddAlterationDetails(AlterationDetails alterationDetails)
        {
            return await AddAlterationDetails(alterationDetails, null, null);
        }
        public async Task<bool> UpdateAlterationDetails(AlterationDetails alterationDetails)
        {
            return await UpdateAlterationDetails(alterationDetails, null, null);
        }
        private async Task<bool> UpdateAlterationDetails(AlterationDetails alterationDetails, IDbConnection? externalConnection = null, IDbTransaction? externalTransaction = null)
        {
            var isOwnConnection = externalConnection == null;
            DbConnection connection = externalConnection != null ? (DbConnection)externalConnection : base.GetConnection();
            DbTransaction transaction = externalTransaction != null ? (DbTransaction)externalTransaction : await connection.BeginTransactionAsync();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_CurrentJewellerySize", alterationDetails.CurrentJewellerySize);
                parameters.Add("p_DesiredJewellerySize", alterationDetails.DesiredJewellerySize);
                parameters.Add("p_SizeNote", alterationDetails.SizeNote);
                parameters.Add("p_ResizingPrice", alterationDetails.ResizingPrice);
                parameters.Add("p_LacquerTypeId", alterationDetails.LacquerTypeId);
                parameters.Add("p_LacquerNote", alterationDetails.LacquerNote);
                parameters.Add("p_LacquerReferenceSKU", alterationDetails.LacquerReferenceSKU);
                parameters.Add("p_LacquerPrice", alterationDetails.LacquerPrice);
                parameters.Add("p_OtherDescription", alterationDetails.OtherDescription);
                parameters.Add("p_OtherPrice", alterationDetails.OtherPrice);
                parameters.Add("p_ProductTotalPrice", alterationDetails.ProductTotalPrice);
                parameters.Add("p_WeightChange", alterationDetails.WeightChange);
                parameters.Add("p_WeightChangePrice", alterationDetails.WeightChangePrice);
                parameters.Add("p_UpdatedBy", alterationDetails.UpdatedBy);
                parameters.Add("p_AlterationDetailsId", alterationDetails.AlterationDetailsId);
                parameters.Add("o_AlterationDetailsId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "UpdateAlterationDetailsGb",
                    parameters,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );

                int alterationDetailsId = parameters.Get<int>("o_AlterationDetailsId");

                if (alterationDetailsId <= 0)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                // Product Documents
                foreach (var doc in alterationDetails.Documents ?? Enumerable.Empty<AlterationDetailsDocument>())
                {
                    await connection.ExecuteAsync("InsertUpdateAlterationDetailsDocumentGb", new
                    {
                        p_AlterationDetailsId = alterationDetailsId,
                        p_DocumentId = doc.DocumentId,
                        p_CreatedBy = alterationDetails.CreatedBy,
                        p_IsPostAlteration = doc.IsPostAlteration,
                        p_IsPrimary = doc.IsPrimary,
                        p_IsLacquer = doc.IsLacquer,
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure);
                }

                // Stone Products and Documents
                foreach (var stone in alterationDetails.Stones ?? Enumerable.Empty<StoneAlteration>())
                {
                    await connection.ExecuteAsync("InsertUpdateAlterationStoneGb", new
                    {
                        p_AlterationDetailsId = alterationDetailsId,
                        p_CurrentStoneTypeId = stone.CurrentStoneTypeId,
                        p_DesiredStoneTypeId = stone.DesiredStoneTypeId,
                        p_AdditionalNote = stone.AdditionalNote,
                        p_ReferenceSKU = stone.ReferenceSKU,
                        p_WeightTypeId = stone.WeightTypeId,
                        p_Weight = stone.Weight,
                        p_Price = stone.Price,
                        p_CreatedBy = alterationDetails.CreatedBy,
                        p_StoneAlterationId = stone.StoneAlterationId,
                        p_ActualPrice = stone.ActualPrice,
                        p_ActualWeight = stone.ActualWeight
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure);

                }

                if (isOwnConnection)
                    await transaction.CommitAsync();
                return alterationDetailsId > 0;
            }
            catch (Exception ex)
            {
                if (isOwnConnection)
                    await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                if (isOwnConnection)
                    await connection.DisposeAsync();
            }
        }
        private async Task<int> AddAlterationDetails(AlterationDetails alterationDetails, IDbConnection? externalConnection = null, IDbTransaction? externalTransaction = null)
        {
            var isOwnConnection = externalConnection == null;
            DbConnection connection = externalConnection != null ? (DbConnection)externalConnection : base.GetConnection();
            DbTransaction transaction = externalTransaction != null ? (DbTransaction)externalTransaction : await connection.BeginTransactionAsync();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_CurrentJewellerySize", alterationDetails.CurrentJewellerySize);
                parameters.Add("p_DesiredJewellerySize", alterationDetails.DesiredJewellerySize);
                parameters.Add("p_SizeNote", alterationDetails.SizeNote);
                parameters.Add("p_ResizingPrice", alterationDetails.ResizingPrice);
                parameters.Add("p_LacquerTypeId", alterationDetails.LacquerTypeId);
                parameters.Add("p_LacquerNote", alterationDetails.LacquerNote);
                parameters.Add("p_LacquerReferenceSKU", alterationDetails.LacquerReferenceSKU);
                parameters.Add("p_LacquerPrice", alterationDetails.LacquerPrice);
                parameters.Add("p_OtherDescription", alterationDetails.OtherDescription);
                parameters.Add("p_OtherPrice", alterationDetails.OtherPrice);
                parameters.Add("p_ProductTotalPrice", alterationDetails.ProductTotalPrice);
                parameters.Add("p_WeightChange", alterationDetails.WeightChange);
                parameters.Add("p_WeightChangePrice", alterationDetails.WeightChangePrice);
                parameters.Add("p_CreatedBy", alterationDetails.CreatedBy);
                parameters.Add("o_AlterationDetailsId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "AddAlterationDetailsGb",
                    parameters,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );

                int alterationDetailsId = parameters.Get<int>("o_AlterationDetailsId");

                if (alterationDetailsId <= 0)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Product  insertion failed.");
                }

                // Product Documents
                foreach (var doc in alterationDetails.Documents ?? Enumerable.Empty<AlterationDetailsDocument>())
                {
                    await connection.ExecuteAsync("InsertUpdateAlterationDetailsDocumentGb", new
                    {
                        p_AlterationDetailsId = alterationDetailsId,
                        p_DocumentId = doc.DocumentId,
                        p_CreatedBy = alterationDetails.CreatedBy,
                        p_IsPostAlteration = doc.IsPostAlteration,
                        p_IsPrimary = doc.IsPrimary,
                        p_IsLacquer = doc.IsLacquer,
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure);
                }

                // Stone Products and Documents
                foreach (var stone in alterationDetails.Stones ?? Enumerable.Empty<StoneAlteration>())
                {
                    await connection.ExecuteAsync("InsertUpdateAlterationStoneGb", new
                    {
                        p_AlterationDetailsId = alterationDetailsId,
                        p_CurrentStoneTypeId = stone.CurrentStoneTypeId,
                        p_DesiredStoneTypeId = stone.DesiredStoneTypeId,
                        p_AdditionalNote = stone.AdditionalNote,
                        p_ReferenceSKU = stone.ReferenceSKU,
                        p_WeightTypeId = stone.WeightTypeId,
                        p_Weight = stone.Weight,
                        p_Price = stone.Price,
                        p_CreatedBy = alterationDetails.CreatedBy,
                        p_StoneAlterationId = stone.StoneAlterationId,
                        p_ActualPrice = stone.ActualPrice,
                        p_ActualWeight = stone.ActualWeight
                    },
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure);

                }

                if (isOwnConnection)
                    await transaction.CommitAsync();
                return alterationDetailsId;
            }
            catch (Exception ex)
            {
                if (isOwnConnection)
                    await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                if (isOwnConnection)
                    await connection.DisposeAsync();
            }
        }
        public async Task<AlterationDetails> GetAlterationDetailsById(int orderId)
        {

            var alteration = new AlterationDetails();
            var documents = new List<AlterationDetailsDocument>();
            var stones = new List<StoneAlteration>();

            var parameters = new List<DbParameter>
            {
                base.GetParameter("p_OrderId", ToDbValue(orderId))
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetAlterationDetailsByOrderId_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                        var item = new AlterationDetails();
                        item.AlterationDetailsId = dataReader.GetIntegerValue("alterationDetailsId");
                        item.CurrentJewellerySize = dataReader.GetStringValue("currentJewellerySize");
                        item.DesiredJewellerySize = dataReader.GetStringValue("desiredJewellerySize");
                        item.SizeNote = dataReader.GetStringValue("sizeNote");
                        item.ResizingPrice = dataReader.GetDecimalValue("resizingPrice");
                        item.LacquerTypeId = dataReader.GetIntegerValue("lacquerTypeId");
                        item.LacquerNote = dataReader.GetStringValue("lacquerNote");
                        item.LacquerReferenceSKU = dataReader.GetStringValue("lacquerReferenceSKU");
                        item.OtherDescription = dataReader.GetStringValue("otherDescription");
                        item.LacquerPrice = dataReader.GetDecimalValue("lacquerPrice");
                        item.WeightChangePrice = dataReader.GetDecimalValue("weightChangePrice");
                        item.WeightChange = dataReader.GetDecimalValue("weightChange");
                        item.ProductTotalPrice = dataReader.GetDecimalValue("productTotalPrice");
                        item.OtherPrice = dataReader.GetDecimalValue("otherPrice");
                        item.IsActive = dataReader.GetBooleanValue("isActive");
                        item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        item.CreatedAt = dataReader.GetDateTime("createdAt");
                        item.CreatedBy = dataReader.GetIntegerValue("createdBy");

                        alteration = item;
                    }
                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            var item = new AlterationDetailsDocument();
                            item.AlterationDetailsDocumentId = dataReader.GetIntegerValue("alterationDetailsDocumentId");
                            item.AlterationDetailsId = dataReader.GetIntegerValue("alterationDetailsId");
                            item.DocumentId = dataReader.GetIntegerValue("documentId");
                            item.IsPrimary = dataReader.GetBooleanValue("IsPrimary");
                            item.IsPostAlteration = dataReader.GetBooleanValue("isPostAlteration");
                            item.Url = dataReader.GetStringValue("url");
                            item.IsLacquer = dataReader.GetBooleanValue("isLacquer");
                            item.IsActive = dataReader.GetBooleanValue("isActive");
                            item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                            item.CreatedAt = dataReader.GetDateTime("createdAt");
                            item.CreatedBy = dataReader.GetIntegerValue("createdBy");
                            documents.Add(item);
                        }
                    }
                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            var item = new StoneAlteration();

                            item.StoneAlterationId = dataReader.GetIntegerValue("stoneAlterationId");
                            item.AlterationDetailsId = dataReader.GetIntegerValue("alterationDetailsId");
                            item.CurrentStoneTypeId = dataReader.GetIntegerValue("currentStoneTypeId");
                            item.DesiredStoneTypeId = dataReader.GetIntegerValue("desiredStoneTypeId");
                            item.AdditionalNote = dataReader.GetStringValue("additionalNote");
                            item.ReferenceSKU = dataReader.GetStringValue("referenceSKU");
                            item.WeightTypeId = dataReader.GetIntegerValue("weightTypeId");
                            item.Weight = dataReader.GetDecimalValue("weight");
                            item.Price = dataReader.GetDecimalValue("price");
                            item.ActualWeight = dataReader.GetDecimalValue("actualWeight");
                            item.ActualPrice = dataReader.GetDecimalValue("actualPrice");
                            item.IsActive = dataReader.GetBooleanValue("isActive");
                            item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                            item.CreatedAt = dataReader.GetDateTime("createdAt");
                            item.CreatedBy = dataReader.GetIntegerValue("createdBy");
                            stones.Add(item);
                        }
                    }

                }
            }
            alteration.Documents = documents;
            alteration.Stones = stones;
            return alteration;
        }
        #endregion

        #region Repair Details

        public async Task<RepairDetails> GetRepairDetailsById(int orderId)
        {

            var repair = new RepairDetails();
            var documents = new List<RepairDocument>();
            var stones = new List<RepairStoneDetails>();

            var parameters = new List<DbParameter>
            {
                base.GetParameter("p_OrderId", ToDbValue(orderId))
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetRepairDetailsByOrderId_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                        var item = new RepairDetails();
                        item.RepairDetailId = dataReader.GetIntegerValue("repairDetailId");
                        item.ProductTypeId = dataReader.GetIntegerValue("productTypeId");
                        item.MetalTypeId = dataReader.GetIntegerValue("metalTypeId");
                        item.WeightBeforeRepair = dataReader.GetDecimalValue("weightBeforeRepair");
                        item.RepairCleaningId = dataReader.GetIntegerValue("repairCleaningId");
                        item.CleaningNotes = dataReader.GetStringValue("cleaningNotes");
                        item.CleaningPrice = dataReader.GetDecimalValue("cleaningPrice");
                        item.RepairPolishingId = dataReader.GetIntegerValue("repairPolishingId");
                        item.PolishingNotes = dataReader.GetStringValue("polishingNotes");
                        item.PolishingPrice = dataReader.GetDecimalValue("polishingPrice");
                        item.CurrentJewellerySize = dataReader.GetStringValue("currentJewellerySize");
                        item.DesiredJewellerySize = dataReader.GetStringValue("desiredJewellerySize");
                        item.ResizingNotes = dataReader.GetStringValue("resizingNotes");
                        item.ResizingPrice = dataReader.GetDecimalValue("resizingPrice");
                        item.RepairDamageTypeIds = dataReader.GetStringValue("repairDamageTypeIds");
                        item.RepairDamageAreaIds = dataReader.GetStringValue("repairDamageAreaIds");
                        item.RepairingNotes = dataReader.GetStringValue("repairingNotes");
                        item.RepairingPrice = dataReader.GetDecimalValue("repairingPrice");

                        item.EstRepairingCost = dataReader.GetDecimalValue("estRepairCost");
                        item.WeightChange = dataReader.GetDecimalValue("weightChange");
                        item.WeightChangePrice = dataReader.GetDecimalValue("weightChangePrice");
                        item.ActualWeight = dataReader.GetDecimalValue("actualWeight");
                        item.TotalRepairCost = dataReader.GetDecimalValue("totalRepairCost");
                        item.EstDeliveryDate = dataReader.GetDateTimeValue("estDeliveryDate");
                        item.WeightTypeId = dataReader.GetIntegerValue("weightTypeId");
                        //item.WeightAfterRepair = dataReader.GetIntegerValue("weightAfterRepair");

                        item.IsActive = dataReader.GetBooleanValue("isActive");
                        item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        item.CreatedAt = dataReader.GetDateTimeValue("createdAt");
                        item.CreatedBy = dataReader.GetIntegerValue("createdBy");
                        repair = item;
                    }
                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            var item = new RepairDocument();
                            item.RepairDocumentId = dataReader.GetIntegerValue("repairDocumentId");
                            item.RepairDetailId = dataReader.GetIntegerValue("repairDetailId");
                            item.DocumentId = dataReader.GetIntegerValue("documentId");
                            item.Url = dataReader.GetStringValue("url");
                            item.IsPrimary = dataReader.GetBooleanValue("isPrimary");
                            item.IsPostRepair = dataReader.GetBooleanValue("isPostRepair");
                            item.IsActive = dataReader.GetBooleanValue("isActive");
                            item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                            item.CreatedAt = dataReader.GetDateTimeValue("createdAt");
                            item.CreatedBy = dataReader.GetIntegerValue("createdBy");
                            documents.Add(item);
                        }
                    }
                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            var item = new RepairStoneDetails();

                            item.RepairStoneDetailId = dataReader.GetIntegerValue("repairStoneDetailId");
                            item.RepairDetailId = dataReader.GetIntegerValue("repairDetailId");
                            item.CurrentStoneId = dataReader.GetIntegerValue("currentStoneId");
                            item.DesiredStoneId = dataReader.GetIntegerValue("desiredStoneId");
                            item.IsFixed = dataReader.GetBooleanValue("isFixed");
                            item.StoneTypeIds = dataReader.GetStringValue("stoneTypeIds");
                            item.IsReplacement = dataReader.GetBooleanValue("isReplacement");
                            item.Notes = dataReader.GetStringValue("notes");
                            item.Price = dataReader.GetDecimalValue("price");
                            item.ActualWeight = dataReader.GetDecimalValue("actualWeight");
                            item.ActualPrice = dataReader.GetDecimalValue("actualPrice");
                            item.IsActive = dataReader.GetBooleanValue("isActive");
                            item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                            item.CreatedAt = dataReader.GetDateTime("createdAt");
                            item.CreatedBy = dataReader.GetIntegerValue("createdBy");
                            stones.Add(item);
                        }
                    }

                }
            }
            repair.RepairDocuments = documents;
            repair.RepairStoneDetails = stones;
            return repair;
        }
        private async Task<int> AddRepairDetails(RepairDetails repairDetails, IDbConnection? externalConnection = null, IDbTransaction? externalTransaction = null)
        {
            var isOwnConnection = externalConnection == null;
            DbConnection connection = externalConnection != null ? (DbConnection)externalConnection : base.GetConnection();
            DbTransaction transaction = externalTransaction != null ? (DbTransaction)externalTransaction : await connection.BeginTransactionAsync();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_ProductTypeId", repairDetails.ProductTypeId);
                parameters.Add("p_MetalTypeId", repairDetails.MetalTypeId);
                parameters.Add("p_WeightBeforeRepair", repairDetails.WeightBeforeRepair);
                parameters.Add("p_RepairCleaningId", repairDetails.RepairCleaningId);
                parameters.Add("p_CleaningNotes", repairDetails.CleaningNotes);
                parameters.Add("p_CleaningPrice", repairDetails.CleaningPrice);
                parameters.Add("p_RepairPolishingId", repairDetails.RepairPolishingId);
                parameters.Add("p_PolishingNotes", repairDetails.PolishingNotes);
                parameters.Add("p_PolishingPrice", repairDetails.PolishingPrice);
                parameters.Add("p_CurrentJewellerySize", repairDetails.CurrentJewellerySize);
                parameters.Add("p_DesiredJewellerySize", repairDetails.DesiredJewellerySize);
                parameters.Add("p_ResizingNotes", repairDetails.ResizingNotes);
                parameters.Add("p_ResizingPrice", repairDetails.ResizingPrice);
                parameters.Add("p_RepairDamageTypeIds", repairDetails.RepairDamageTypeIds);
                parameters.Add("p_RepairDamageAreaIds", repairDetails.RepairDamageAreaIds);
                parameters.Add("p_RepairingNotes", repairDetails.RepairingNotes);
                parameters.Add("p_RepairingPrice", repairDetails.RepairingPrice);
                parameters.Add("p_EstRepairingCost", repairDetails.EstRepairingCost);
                parameters.Add("p_EstDeliveryDate", repairDetails.EstDeliveryDate);
                parameters.Add("p_WeightChange", repairDetails.WeightChange);
                parameters.Add("p_WeightChangePrice", repairDetails.WeightChangePrice);
                parameters.Add("p_ActualWeight", repairDetails.ActualWeight);
                parameters.Add("p_TotalRepairCost", repairDetails.TotalRepairCost);
                parameters.Add("p_WeightTypeId", repairDetails.WeightTypeId);
                parameters.Add("p_CreatedBy", repairDetails.CreatedBy);

                parameters.Add("o_RepairDetailId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "AddRepairDetailsGb",
                    parameters,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );

                int RepairDetailId = parameters.Get<int>("o_RepairDetailId");

                if (RepairDetailId > 0)
                {
                    // Repair Documents
                    foreach (var doc in repairDetails.RepairDocuments ?? Enumerable.Empty<RepairDocument>())
                    {
                        await connection.ExecuteAsync("AddUpdateRepairDocumentGb", new
                        {
                            p_RepairDocumentId = doc.RepairDocumentId,
                            p_DocumentId = doc.DocumentId,
                            p_RepairDetailId = RepairDetailId,
                            p_IsPrimary = doc.IsPrimary,
                            p_IsPostRepair = doc.IsPostRepair,
                            p_CreatedBy = doc.CreatedBy,
                            p_UpdatedBy = repairDetails.UpdatedBy
                        },
                        transaction: transaction,
                        commandType: CommandType.StoredProcedure);
                    }

                    foreach (var doc in repairDetails.RepairStoneDetails ?? Enumerable.Empty<RepairStoneDetails>())
                    {
                        await connection.ExecuteAsync("AddUpdateRepairStoneDetailGb", new
                        {
                            p_RepairStoneDetailId = doc.RepairStoneDetailId,
                            p_RepairDetailId = RepairDetailId,
                            p_CurrentStoneId = doc.CurrentStoneId,
                            p_DesiredStoneId = doc.DesiredStoneId,
                            p_StoneTypeIds = doc.StoneTypeIds,
                            p_IsFixed = doc.IsFixed,
                            p_IsReplacement = doc.IsReplacement,
                            p_Notes = doc.Notes,
                            p_Price = doc.Price,
                            p_ActualWeight = doc.ActualWeight,
                            p_ActualPrice = doc.ActualPrice,
                            p_CreatedBy = repairDetails.CreatedBy,
                            p_UpdatedBy = repairDetails.UpdatedBy
                        },
                        transaction: transaction,
                        commandType: CommandType.StoredProcedure);
                    }
                }
                else
                {
                    throw new Exception("Repair details insertion failed");
                }

                if (isOwnConnection)
                    await transaction.CommitAsync();

                return RepairDetailId;
            }
            catch (Exception ex)
            {
                if (isOwnConnection)
                    await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                if (isOwnConnection)
                    await connection.DisposeAsync();
            }
        }
        private async Task<int> UpdateRepairDetails(RepairDetails repairDetails, IDbConnection? externalConnection = null, IDbTransaction? externalTransaction = null)
        {
            var isOwnConnection = externalConnection == null;
            DbConnection connection = externalConnection != null ? (DbConnection)externalConnection : base.GetConnection();
            DbTransaction transaction = externalTransaction != null ? (DbTransaction)externalTransaction : await connection.BeginTransactionAsync();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_RepairDetailId", repairDetails.RepairDetailId);
                parameters.Add("p_ProductTypeId", repairDetails.ProductTypeId);
                parameters.Add("p_MetalTypeId", repairDetails.MetalTypeId);
                parameters.Add("p_WeightBeforeRepair", repairDetails.WeightBeforeRepair);
                parameters.Add("p_RepairCleaningId", repairDetails.RepairCleaningId);
                parameters.Add("p_CleaningNotes", repairDetails.CleaningNotes);
                parameters.Add("p_CleaningPrice", repairDetails.CleaningPrice);
                parameters.Add("p_RepairPolishingId", repairDetails.RepairPolishingId);
                parameters.Add("p_PolishingNotes", repairDetails.PolishingNotes);
                parameters.Add("p_PolishingPrice", repairDetails.PolishingPrice);
                parameters.Add("p_CurrentJewellerySize", repairDetails.CurrentJewellerySize);
                parameters.Add("p_DesiredJewellerySize", repairDetails.DesiredJewellerySize);
                parameters.Add("p_ResizingNotes", repairDetails.ResizingNotes);
                parameters.Add("p_ResizingPrice", repairDetails.ResizingPrice);
                parameters.Add("p_RepairDamageTypeIds", repairDetails.RepairDamageTypeIds);
                parameters.Add("p_RepairDamageAreaIds", repairDetails.RepairDamageAreaIds);
                parameters.Add("p_RepairingNotes", repairDetails.RepairingNotes);
                parameters.Add("p_RepairingPrice", repairDetails.RepairingPrice);
                parameters.Add("p_EstRepairingCost", repairDetails.EstRepairingCost);
                parameters.Add("p_EstDeliveryDate", repairDetails.EstDeliveryDate);
                parameters.Add("p_WeightChange", repairDetails.WeightChange);
                parameters.Add("p_WeightChangePrice", repairDetails.WeightChangePrice);
                parameters.Add("p_ActualWeight", repairDetails.ActualWeight);
                parameters.Add("p_TotalRepairCost", repairDetails.TotalRepairCost);
                parameters.Add("p_WeightTypeId", repairDetails.WeightTypeId);
                parameters.Add("p_UpdatedBy", repairDetails.UpdatedBy);
                parameters.Add("o_RepairDetailId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "UpdateRepairDetailsGb",
                    parameters,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );

                int RepairDetailId = parameters.Get<int>("o_RepairDetailId");

                if (RepairDetailId > 0) // updated successfully
                {
                    // Repair Documents
                    foreach (var doc in repairDetails.RepairDocuments ?? Enumerable.Empty<RepairDocument>())
                    {
                        await connection.ExecuteAsync("AddUpdateRepairDocumentGb", new
                        {
                            p_RepairDocumentId = doc.RepairDocumentId,
                            p_DocumentId = doc.DocumentId,
                            p_RepairDetailId = RepairDetailId,
                            p_IsPrimary = doc.IsPrimary,
                            p_IsPostRepair = doc.IsPostRepair,
                            p_CreatedBy = repairDetails.CreatedBy,
                            p_UpdatedBy = repairDetails.UpdatedBy
                        },
                        transaction: transaction,
                        commandType: CommandType.StoredProcedure);
                    }

                    foreach (var doc in repairDetails.RepairStoneDetails ?? Enumerable.Empty<RepairStoneDetails>())
                    {
                        await connection.ExecuteAsync("AddUpdateRepairStoneDetailGb", new
                        {
                            p_RepairStoneDetailId = doc.RepairStoneDetailId,
                            p_RepairDetailId = RepairDetailId,
                            p_CurrentStoneId = doc.CurrentStoneId,
                            p_DesiredStoneId = doc.DesiredStoneId,
                            p_IsFixed = doc.IsFixed,
                            p_StoneTypeIds = doc.StoneTypeIds,
                            p_IsReplacement = doc.IsReplacement,
                            p_Notes = doc.Notes,
                            p_Price = doc.Price,
                            p_ActualWeight = doc.ActualWeight,
                            p_ActualPrice = doc.ActualPrice,
                            p_CreatedBy = repairDetails.CreatedBy,
                            p_UpdatedBy = repairDetails.UpdatedBy
                        },
                        transaction: transaction,
                        commandType: CommandType.StoredProcedure);
                    }
                }
                else
                {
                    throw new Exception("Repair details insertion failed");
                }

                if (isOwnConnection)
                    await transaction.CommitAsync();

                return RepairDetailId;
            }
            catch (Exception ex)
            {
                if (isOwnConnection)
                    await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                if (isOwnConnection)
                    await connection.DisposeAsync();
            }
        }
        #endregion

        #region Appraisal Details

        private async Task<int> AddAppraisalDetail(AppraisalDetail appraisalDetails, IDbConnection? externalConnection = null, IDbTransaction? externalTransaction = null)
        {
            var isOwnConnection = externalConnection == null;
            DbConnection connection = externalConnection != null ? (DbConnection)externalConnection : base.GetConnection();
            DbTransaction transaction = externalTransaction != null ? (DbTransaction)externalTransaction : await connection.BeginTransactionAsync();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_TotalProductWeight", appraisalDetails.TotalProductWeight, DbType.Decimal);
                parameters.Add("p_NetGoldWeight", appraisalDetails.NetGoldWeight, DbType.Decimal);
                parameters.Add("p_PureGoldWeight", appraisalDetails.PureGoldWeight, DbType.Decimal);
                parameters.Add("p_DeductionPercentage", appraisalDetails.DeductionPercentage, DbType.Decimal);
                parameters.Add("p_AppraisalPrice", appraisalDetails.AppraisalPrice, DbType.Decimal);
                parameters.Add("p_Notes", appraisalDetails.Notes, DbType.String, size: 1000);
                parameters.Add("p_WeightTypeId", appraisalDetails.WeightTypeId, DbType.Int32);
                parameters.Add("o_AppraisalDetailId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "AddAppraisalDetailGb",
                    parameters,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );

                int AppraisalDetailId = parameters.Get<int>("o_AppraisalDetailId");

                if (AppraisalDetailId > 0)
                {
                    // Repair Documents
                    foreach (var doc in appraisalDetails.AppraisalDocuments ?? Enumerable.Empty<AppraisalDocument>())
                    {
                        await connection.ExecuteAsync("AddUpdateAppraisalDocumentGb", new
                        {
                            p_AppraisalDocumentId = doc.AppraisalDocumentId,
                            p_DocumentId = doc.DocumentId,
                            p_AppraisalDetailId = AppraisalDetailId,
                            p_IsPrimary = doc.IsPrimary,
                            p_CreatedBy = appraisalDetails.UpdatedBy
                        },
                        transaction: transaction,
                        commandType: CommandType.StoredProcedure);
                    }

                    foreach (var doc in appraisalDetails.AppraisalStoneDetails ?? Enumerable.Empty<AppraisalStoneDetail>())
                    {
                        await connection.ExecuteAsync(
                        "AddUpdateAppraisalStoneDetailGb",
                        new
                        {
                            p_AppraisalStoneDetailId = doc.AppraisalStoneDetailId,
                            p_AppraisalDetailId = AppraisalDetailId,
                            p_StoneTypeId = doc.StoneTypeId,
                            p_StoneQuantity = doc.StoneQuantity,
                            p_StoneWeight = doc.StoneWeight,
                            p_StonePrice = doc.StonePrice,
                            p_UpdatedBy = appraisalDetails.UpdatedBy,
                            p_StoneWeightTypeId = doc.StoneWeightTypeId
                        },
                        transaction: transaction,
                        commandType: CommandType.StoredProcedure);
                    }
                }
                else
                {
                    throw new Exception("Repair details insertion failed");
                }

                if (isOwnConnection)
                    await transaction.CommitAsync();

                return AppraisalDetailId;
            }
            catch (Exception ex)
            {
                if (isOwnConnection)
                    await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                if (isOwnConnection)
                    await connection.DisposeAsync();
            }
        }
        private async Task<bool> UpdateAppraisalDetail(AppraisalDetail appraisalDetails, IDbConnection? externalConnection = null, IDbTransaction? externalTransaction = null)
        {
            var isOwnConnection = externalConnection == null;
            DbConnection connection = externalConnection != null ? (DbConnection)externalConnection : base.GetConnection();
            DbTransaction transaction = externalTransaction != null ? (DbTransaction)externalTransaction : await connection.BeginTransactionAsync();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_AppraisalDetailId", appraisalDetails.AppraisalDetailId, dbType: DbType.Int32);
                parameters.Add("p_TotalProductWeight", appraisalDetails.TotalProductWeight, DbType.Decimal);
                parameters.Add("p_NetGoldWeight", appraisalDetails.NetGoldWeight, DbType.Decimal);
                parameters.Add("p_PureGoldWeight", appraisalDetails.PureGoldWeight, DbType.Decimal);
                parameters.Add("p_DeductionPercentage", appraisalDetails.DeductionPercentage, DbType.Decimal);
                parameters.Add("p_AppraisalPrice", appraisalDetails.AppraisalPrice, DbType.Decimal);
                parameters.Add("p_Notes", appraisalDetails.Notes, DbType.String, size: 1000);
                parameters.Add("p_WeightTypeId", appraisalDetails.WeightTypeId, DbType.Int32);
                parameters.Add("o_IsUpdated", DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "UpdateAppraisalDetailGb",
                    parameters,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );

                int o_IsUpdated = parameters.Get<int>("o_IsUpdated");

                if (o_IsUpdated > 0)
                {
                    // Repair Documents
                    foreach (var doc in appraisalDetails.AppraisalDocuments ?? Enumerable.Empty<AppraisalDocument>())
                    {
                        await connection.ExecuteAsync("AddUpdateAppraisalDocumentGb", new
                        {
                            p_AppraisalDocumentId = doc.AppraisalDocumentId,
                            p_DocumentId = doc.DocumentId,
                            p_AppraisalDetailId = appraisalDetails.AppraisalDetailId,
                            p_IsPrimary = doc.IsPrimary,
                            p_CreatedBy = appraisalDetails.UpdatedBy
                        },
                        transaction: transaction,
                        commandType: CommandType.StoredProcedure);
                    }

                    foreach (var doc in appraisalDetails.AppraisalStoneDetails ?? Enumerable.Empty<AppraisalStoneDetail>())
                    {
                        await connection.ExecuteAsync(
                        "AddUpdateAppraisalStoneDetailGb",
                        new
                        {
                            p_AppraisalStoneDetailId = doc.AppraisalStoneDetailId,
                            p_AppraisalDetailId = appraisalDetails.AppraisalDetailId,
                            p_StoneTypeId = doc.StoneTypeId,
                            p_StoneQuantity = doc.StoneQuantity,
                            p_StoneWeight = doc.StoneWeight,
                            p_StonePrice = doc.StonePrice,
                            p_UpdatedBy = appraisalDetails.UpdatedBy,
                            p_StoneWeightTypeId = doc.StoneWeightTypeId
                        },
                        transaction: transaction,
                        commandType: CommandType.StoredProcedure);
                    }
                }
                else
                {
                    throw new Exception("Appraisal details insertion failed");
                }

                if (isOwnConnection)
                    await transaction.CommitAsync();

                return o_IsUpdated > 0;
            }
            catch (Exception ex)
            {
                if (isOwnConnection)
                    await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                if (isOwnConnection)
                    await connection.DisposeAsync();
            }
        }
        public async Task<AppraisalDetail> GetAppraisalDetailsById(int orderId)
        {

            var appraisal = new AppraisalDetail();
            var documents = new List<AppraisalDocument>();
            var stones = new List<AppraisalStoneDetail>();

            var parameters = new List<DbParameter>
            {
                base.GetParameter("p_OrderId", ToDbValue(orderId))
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetAppraisalDetailsByOrderIdGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                        var item = new AppraisalDetail();
                        item.AppraisalDetailId = dataReader.GetIntegerValue("appraisalDetailId");
                        item.TotalProductWeight = dataReader.GetDecimalValue("totalProductWeight");
                        item.NetGoldWeight = dataReader.GetIntegerValue("netGoldWeight");
                        item.PureGoldWeight = dataReader.GetDecimalValue("pureGoldWeight");
                        item.DeductionPercentage = dataReader.GetDecimalValue("deductionPercentage");
                        item.AppraisalPrice = dataReader.GetDecimalValue("appraisalPrice");
                        item.Notes = dataReader.GetStringValue("notes");
                        item.WeightTypeId = dataReader.GetIntegerValue("weightTypeId");

                        item.IsActive = dataReader.GetBooleanValue("isActive");
                        item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        item.CreatedAt = dataReader.GetDateTimeValue("createdAt");
                        item.CreatedBy = dataReader.GetIntegerValue("createdBy");
                        appraisal = item;
                    }
                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            var item = new AppraisalDocument();
                            item.AppraisalDetailId = dataReader.GetIntegerValue("appraisalDetailId");
                            item.AppraisalDocumentId = dataReader.GetIntegerValue("appraisalDocumentId");
                            item.DocumentId = dataReader.GetIntegerValue("documentId");
                            item.Url = dataReader.GetStringValue("url");
                            item.IsPrimary = dataReader.GetBooleanValue("isPrimary");
                            documents.Add(item);
                        }
                    }
                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            var item = new AppraisalStoneDetail();

                            item.AppraisalStoneDetailId = dataReader.GetIntegerValue("appraisalStoneDetailId");
                            item.AppraisalDetailId = dataReader.GetIntegerValue("appraisalDetailId");
                            item.StoneTypeId = dataReader.GetIntegerValue("stoneTypeId");
                            item.StoneQuantity = dataReader.GetIntegerValue("stoneQuantity");
                            item.StoneWeight = dataReader.GetDecimalValue("stoneWeight");
                            item.StonePrice = dataReader.GetDecimalValue("stonePrice");
                            item.StoneWeightTypeId = dataReader.GetIntegerValue("stoneWeightTypeId");

                            stones.Add(item);
                        }
                    }
                }
            }
            appraisal.AppraisalDocuments = documents;
            appraisal.AppraisalStoneDetails = stones;
            return appraisal;
        }

        #endregion

        #region Exchange Details
        private async Task<int> AddExchangeDetail(ExchangeDetail exchangeDetails, IDbConnection? externalConnection = null, IDbTransaction? externalTransaction = null)
        {
            var isOwnConnection = externalConnection == null;
            DbConnection connection = externalConnection != null ? (DbConnection)externalConnection : base.GetConnection();
            DbTransaction transaction = externalTransaction != null ? (DbTransaction)externalTransaction : await connection.BeginTransactionAsync();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_DeductionPercentage", exchangeDetails.DeductionPercentage, DbType.Decimal);
                parameters.Add("p_DeductionValue", exchangeDetails.DeductionValue, DbType.Decimal);
                parameters.Add("p_OriginalPrice", exchangeDetails.OriginalPrice, DbType.Decimal);
                parameters.Add("p_ExchangePrice", exchangeDetails.ExchangePrice, DbType.Decimal);
                parameters.Add("p_Notes", exchangeDetails.Notes, DbType.String, size: 1000);
                parameters.Add("p_CreatedBy", exchangeDetails.CreatedBy, DbType.String, size: 1000);
                parameters.Add("o_ExchangeDetailId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "AddExchangeDetailGb",
                    parameters,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );

                int ExchangeDetailId = parameters.Get<int>("o_ExchangeDetailId");

                if (ExchangeDetailId > 0)
                {
                    // Repair Documents
                    foreach (var doc in exchangeDetails.ExchangeDocuments ?? Enumerable.Empty<ExchangeDocument>())
                    {
                        await connection.ExecuteAsync("AddUpdateExchangeDocumentGb", new
                        {
                            p_ExchangeDocumentId = doc.ExchangeDocumentId,
                            p_DocumentId = doc.DocumentId,
                            p_ExchangeDetailId = ExchangeDetailId,
                            p_IsPrimary = doc.IsPrimary,
                            p_CreatedBy = exchangeDetails.CreatedBy
                        },
                        transaction: transaction,
                        commandType: CommandType.StoredProcedure);
                    }
                }
                else
                {
                    throw new Exception("Exchange details insertion failed");
                }

                if (isOwnConnection)
                    await transaction.CommitAsync();

                return ExchangeDetailId;
            }
            catch (Exception ex)
            {
                if (isOwnConnection)
                    await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                if (isOwnConnection)
                    await connection.DisposeAsync();
            }
        }
        private async Task<bool> UpdateExchangeDetail(ExchangeDetail exchangeDetails, IDbConnection? externalConnection = null, IDbTransaction? externalTransaction = null)
        {
            var isOwnConnection = externalConnection == null;
            DbConnection connection = externalConnection != null ? (DbConnection)externalConnection : base.GetConnection();
            DbTransaction transaction = externalTransaction != null ? (DbTransaction)externalTransaction : await connection.BeginTransactionAsync();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_DeductionPercentage", exchangeDetails.DeductionPercentage, DbType.Decimal);
                parameters.Add("p_DeductionValue", exchangeDetails.DeductionValue, DbType.Decimal);
                parameters.Add("p_OriginalPrice", exchangeDetails.OriginalPrice, DbType.Decimal);
                parameters.Add("p_ExchangePrice", exchangeDetails.ExchangePrice, DbType.Decimal);
                parameters.Add("p_Notes", exchangeDetails.Notes, DbType.String, size: 1000);
                parameters.Add("p_UpdatedBy", exchangeDetails.UpdatedBy, DbType.Int32, size: 1000);
                parameters.Add("p_ExchangeDetailId", exchangeDetails.ExchangeDetailId, dbType: DbType.Int32);
                parameters.Add("o_IsUpdated", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "UpdateExchangeDetailGb",
                    parameters,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );

                int o_IsUpdated = parameters.Get<int>("o_IsUpdated");

                if (o_IsUpdated > 0)
                {
                    // Repair Documents
                    foreach (var doc in exchangeDetails.ExchangeDocuments ?? Enumerable.Empty<ExchangeDocument>())
                    {
                        await connection.ExecuteAsync("AddUpdateExchangeDocumentGb", new
                        {
                            p_ExchangeDocumentId = doc.ExchangeDocumentId,
                            p_DocumentId = doc.DocumentId,
                            p_ExchangeDetailId = exchangeDetails.ExchangeDetailId,
                            p_IsPrimary = doc.IsPrimary,
                            p_CreatedBy = exchangeDetails.UpdatedBy
                        },
                        transaction: transaction,
                        commandType: CommandType.StoredProcedure);
                    }
                }
                else
                {
                    throw new Exception("Exchange details updation failed");
                }

                if (isOwnConnection)
                    await transaction.CommitAsync();

                return o_IsUpdated > 0;
            }
            catch (Exception ex)
            {
                if (isOwnConnection)
                    await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                if (isOwnConnection)
                    await connection.DisposeAsync();
            }
        }
        public async Task<ExchangeDetail> GetExchangeDetailsById(int orderId)
        {
            var exchange = new ExchangeDetail();
            var documents = new List<ExchangeDocument>();

            var parameters = new List<DbParameter>
            {
                base.GetParameter("p_OrderId", ToDbValue(orderId))
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetExchangeDetailsByOrderIdGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                        var item = new ExchangeDetail();
                        item.DeductionPercentage = dataReader.GetDecimalValue("deductionPercentage");
                        item.DeductionValue = dataReader.GetDecimalValue("deductionValue");
                        item.ExchangeDetailId = dataReader.GetIntegerValue("exchangeDetailId");
                        item.OriginalPrice = dataReader.GetDecimalValue("originalPrice");
                        item.ExchangePrice = dataReader.GetDecimalValue("exchangePrice");
                        item.Notes = dataReader.GetStringValue("notes");

                        item.IsActive = dataReader.GetBooleanValue("isActive");
                        item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        item.CreatedAt = dataReader.GetDateTimeValue("createdAt");
                        item.CreatedBy = dataReader.GetIntegerValue("createdBy");
                        exchange = item;
                    }
                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            var item = new ExchangeDocument();
                            item.ExchangeDetailId = dataReader.GetIntegerValue("exchangeDetailId");
                            item.ExchangeDocumentId = dataReader.GetIntegerValue("exchangeDocumentId");
                            item.DocumentId = dataReader.GetIntegerValue("documentId");
                            item.Url = dataReader.GetStringValue("url");
                            item.IsPrimary = dataReader.GetBooleanValue("isPrimary");
                            documents.Add(item);
                        }
                    }
                }
            }
            exchange.ExchangeDocuments = documents;
            return exchange;
        }

        #endregion

        #region Gold Booking Details
        private async Task<int> AddGoldBookingDetail(GoldBookingDetail goldBookingDetails, IDbConnection? externalConnection = null, IDbTransaction? externalTransaction = null)
        {
            var isOwnConnection = externalConnection == null;
            DbConnection connection = externalConnection != null ? (DbConnection)externalConnection : base.GetConnection();
            DbTransaction transaction = externalTransaction != null ? (DbTransaction)externalTransaction : await connection.BeginTransactionAsync();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_WeightTypeId", goldBookingDetails.WeightTypeId, DbType.Int32);
                parameters.Add("p_BookingWeight", goldBookingDetails.BookingWeight, DbType.Decimal);
                parameters.Add("p_BookingPrice", goldBookingDetails.BookingPrice, DbType.Decimal);
                parameters.Add("p_Notes", goldBookingDetails.Notes, DbType.String, size: 1000);
                parameters.Add("p_Sku", goldBookingDetails.Sku, DbType.String, size: 200);
                parameters.Add("p_CreatedBy", goldBookingDetails.CreatedBy, DbType.Int32, size: 1000);
                parameters.Add("o_GoldBookingDetailId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "AddGoldBookingDetailGb",
                    parameters,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );

                int GoldBookingDetailId = parameters.Get<int>("o_GoldBookingDetailId");

                if (GoldBookingDetailId < 0)
                {
                    throw new Exception("GoldBooking details insertion failed");
                }
                if (isOwnConnection)
                    await transaction.CommitAsync();

                return GoldBookingDetailId;
            }
            catch (Exception ex)
            {
                if (isOwnConnection)
                    await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                if (isOwnConnection)
                    await connection.DisposeAsync();
            }
        }
        private async Task<bool> UpdateGoldBookingDetail(GoldBookingDetail goldBookingDetails, IDbConnection? externalConnection = null, IDbTransaction? externalTransaction = null)
        {
            var isOwnConnection = externalConnection == null;
            DbConnection connection = externalConnection != null ? (DbConnection)externalConnection : base.GetConnection();
            DbTransaction transaction = externalTransaction != null ? (DbTransaction)externalTransaction : await connection.BeginTransactionAsync();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_GoldBookingDetailId", goldBookingDetails.GoldBookingDetailId, DbType.Int32);
                parameters.Add("p_WeightTypeId", goldBookingDetails.WeightTypeId, DbType.Int32);
                parameters.Add("p_BookingWeight", goldBookingDetails.BookingWeight, DbType.Decimal);
                parameters.Add("p_BookingPrice", goldBookingDetails.BookingPrice, DbType.Decimal);
                parameters.Add("p_Notes", goldBookingDetails.Notes, DbType.String, size: 1000);
                parameters.Add("p_UpdatedBy", goldBookingDetails.UpdatedBy, DbType.Int32);
                parameters.Add("o_IsUpdated", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "UpdateGoldBookingDetailGb",
                    parameters,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );

                int IsUpdated = parameters.Get<int>("o_IsUpdated");

                if (IsUpdated < 0)
                {
                    throw new Exception("GoldBooking details updation failed");
                }
                if (isOwnConnection)
                    await transaction.CommitAsync();

                return IsUpdated > 0;
            }
            catch (Exception ex)
            {
                if (isOwnConnection)
                    await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                if (isOwnConnection)
                    await connection.DisposeAsync();
            }
        }
        public async Task<GoldBookingDetail> GetGoldBookingDetailsById(int orderId)
        {
            var exchange = new GoldBookingDetail();

            var parameters = new List<DbParameter>
            {
                base.GetParameter("p_OrderId", ToDbValue(orderId))
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetGoldBookingDetailsByOrderIdGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                        var item = new GoldBookingDetail();
                        item.GoldBookingDetailId = dataReader.GetIntegerValue("goldBookingDetailId");
                        item.WeightTypeId = dataReader.GetIntegerValue("weightTypeId");
                        item.BookingWeight = dataReader.GetDecimalValue("bookingWeight");
                        item.BookingPrice = dataReader.GetDecimalValue("bookingPrice");
                        item.ReservationDate = dataReader.GetDateTimeValue("reservationDate");
                        item.Notes = dataReader.GetStringValue("notes");
                        item.IsActive = dataReader.GetBooleanValue("isActive");
                        item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        item.CreatedAt = dataReader.GetDateTimeValue("createdAt");
                        item.CreatedBy = dataReader.GetIntegerValue("createdBy");
                        exchange = item;
                    }
                }
            }
            return exchange;
        }

        #endregion

        #region Gift Card Details
        private async Task<int> AddGiftCardDetail(GiftCardDetail giftCardDetails, IDbConnection? externalConnection = null, IDbTransaction? externalTransaction = null)
        {
            var isOwnConnection = externalConnection == null;
            DbConnection connection = externalConnection != null ? (DbConnection)externalConnection : base.GetConnection();
            DbTransaction transaction = externalTransaction != null ? (DbTransaction)externalTransaction : await connection.BeginTransactionAsync();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_RecipientName", giftCardDetails.RecipientName, DbType.String, size:500);
                parameters.Add("p_RecipientMobileNumber", giftCardDetails.RecipientMobileNumber, DbType.String, size: 500);
                parameters.Add("p_RecipientCnic", giftCardDetails.RecipientCnic, DbType.String, size: 500);
                parameters.Add("p_Amount", giftCardDetails.Amount, DbType.Decimal);
                parameters.Add("p_DepositorName", giftCardDetails.DepositorName, DbType.String, size: 500);
                parameters.Add("p_DepositorMobileNumber", giftCardDetails.DepositorMobileNumber, DbType.String, size: 1000);
                parameters.Add("p_Code", giftCardDetails.Code, DbType.String, size: 1000);
                parameters.Add("p_Sku", giftCardDetails.Sku, DbType.String);
                parameters.Add("p_RedeemDate", giftCardDetails.RedeemDate, DbType.DateTime);
                parameters.Add("p_CreatedBy", giftCardDetails.CreatedBy, DbType.Int32);
                parameters.Add("o_GiftCardDetailId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "AddGiftCardDetailGb",
                    parameters,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );

                int GiftCardDetailId = parameters.Get<int>("o_GiftCardDetailId");

                if (GiftCardDetailId < 0)
                {
                    throw new Exception("Gift Card insertion failed");
                }
                if (isOwnConnection)
                    await transaction.CommitAsync();

                return GiftCardDetailId;
            }
            catch (Exception ex)
            {
                if (isOwnConnection)
                    await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                if (isOwnConnection)
                    await connection.DisposeAsync();
            }
        }
        private async Task<bool> UpdateGiftCardDetail(GiftCardDetail giftCardDetails, IDbConnection? externalConnection = null, IDbTransaction? externalTransaction = null)
        {
            var isOwnConnection = externalConnection == null;
            DbConnection connection = externalConnection != null ? (DbConnection)externalConnection : base.GetConnection();
            DbTransaction transaction = externalTransaction != null ? (DbTransaction)externalTransaction : await connection.BeginTransactionAsync();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_GiftCardDetailId", giftCardDetails.GiftCardDetailId, dbType: DbType.Int32);
                parameters.Add("p_RecipientName", giftCardDetails.RecipientName, DbType.String, size: 500);
                parameters.Add("p_RecipientMobileNumber", giftCardDetails.RecipientMobileNumber, DbType.String, size: 500);
                parameters.Add("p_RecipientCnic", giftCardDetails.RecipientCnic, DbType.String, size: 500);
                parameters.Add("p_Amount", giftCardDetails.Amount, DbType.Decimal);
                parameters.Add("p_DepositorName", giftCardDetails.DepositorName, DbType.String, size: 500);
                parameters.Add("p_DepositorMobileNumber", giftCardDetails.DepositorMobileNumber, DbType.String, size: 1000);
                parameters.Add("p_Code", giftCardDetails.Code, DbType.String, size: 1000);
                parameters.Add("p_Sku", giftCardDetails.Sku, DbType.String);
                parameters.Add("p_RedeemDate", giftCardDetails.RedeemDate, DbType.DateTime);
                parameters.Add("p_UpdatedBy", giftCardDetails.UpdatedBy, DbType.Int32);
                parameters.Add("o_IsUpdated", giftCardDetails.GiftCardDetailId, dbType: DbType.Int32 , direction: ParameterDirection.Output);

                await connection.ExecuteAsync(
                    "UpdateGiftCardDetailGb",
                    parameters,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );

                int IsUpdated = parameters.Get<int>("o_IsUpdated");

                if (IsUpdated < 0)
                {
                    throw new Exception("Gift Card updation failed");
                }
                if (isOwnConnection)
                    await transaction.CommitAsync();

                return IsUpdated > 0;
            }
            catch (Exception ex)
            {
                if (isOwnConnection)
                    await transaction.RollbackAsync();
                throw;
            }
            finally
            {
                if (isOwnConnection)
                    await connection.DisposeAsync();
            }
        }
        public async Task<GiftCardDetail> GetGiftCardDetailsById(int orderId)
        {
            var giftCard = new GiftCardDetail();

            var parameters = new List<DbParameter>
            {
                base.GetParameter("p_OrderId", ToDbValue(orderId))
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetGiftCardDetailsByOrderIdGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                        giftCard.GiftCardDetailId = dataReader.GetIntegerValue("giftCardDetailId");
                        giftCard.RecipientName = dataReader.GetStringValue("recipientName");
                        giftCard.RecipientMobileNumber = dataReader.GetStringValue("recipientMobileNumber");
                        giftCard.RecipientCnic = dataReader.GetStringValue("recipientCnic");
                        giftCard.Amount = dataReader.GetDecimalValue("amount");
                        giftCard.DepositorName = dataReader.GetStringValue("depositorName");
                        giftCard.DepositorMobileNumber = dataReader.GetStringValue("depositorMobileNumber");
                        giftCard.Code = dataReader.GetStringValue("code");
                        giftCard.Sku = dataReader.GetStringValue("sku");
                        giftCard.RedeemDate = dataReader.GetDateTimeValue("redeemDate");
                        giftCard.IsActive = dataReader.GetBooleanValue("isActive");
                        giftCard.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        giftCard.CreatedAt = dataReader.GetDateTimeValue("createdAt");
                        giftCard.UpdatedAt = dataReader.GetDateTimeValue("updatedAt");
                        giftCard.CreatedBy = dataReader.GetIntegerValue("createdBy");
                        giftCard.UpdatedBy = dataReader.GetIntegerValue("updatedBy");
                    }
                }
            }
            return giftCard;
        }

        #endregion

        #region Vendor
        public async Task<int> AddVendor(Vendor vendor)
        {
            var response = 0;
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_Description", vendor.Description);
                parameters.Add("p_SerialNumber", vendor.SerialNumber);
                parameters.Add("p_Contact", vendor.Contact);
                parameters.Add("p_CreatedBy", vendor.CreatedBy);
                parameters.Add("o_VendorId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                response = await connection.ExecuteAsync("AddVendor_gb", parameters, transaction, commandType: CommandType.StoredProcedure);
                var vendorId = parameters.Get<int>("o_VendorId");

                if (vendorId > 0)
                {
                    if (vendor.KaatCategory?.Count > 0)
                    {
                        foreach (var kaat in vendor.KaatCategory)
                        {
                            await connection.ExecuteAsync(
                                "InsertorUpdateKaatCategory_Gb",
                                new
                                {
                                    P_KaatCategoryId = kaat.KaatCategoryId,
                                    p_VendorId = vendorId,
                                    p_Label = kaat.Label,
                                    p_Value = kaat.Value,
                                    p_UpdatedBy = vendor.CreatedBy
                                },
                                transaction: transaction,
                                commandType: CommandType.StoredProcedure
                            );
                        }
                    }
                }
                await transaction.CommitAsync();
                response = vendorId;
            }
            catch
            {
                await transaction.RollbackAsync();
            }
            finally
            {
                await connection.DisposeAsync();

            }
            return response;
        }
        public async Task<bool> UpdateVendor(Vendor vendor)
        {
            var response = 0;
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_VendorId", vendor.VendorId); 
                parameters.Add("p_Description", vendor.Description);
                parameters.Add("p_SerialNumber", vendor.SerialNumber);
                parameters.Add("p_Contact", vendor.Contact);
                parameters.Add("p_UpdatedBy", vendor.UpdatedBy); 
                parameters.Add("o_Response", dbType: DbType.Int32, direction: ParameterDirection.Output); 

                // Execute the update stored procedure
                response = await connection.ExecuteAsync("UpdateVendor_gb", parameters, transaction, commandType: CommandType.StoredProcedure);
                var updatedVendorId = parameters.Get<int>("o_Response");

                if (updatedVendorId > 0)
                {

                    foreach (var kaat in vendor.KaatCategory)
                    {
                        await connection.ExecuteAsync(
                            "InsertorUpdateKaatCategory_Gb",
                            new
                            {
                                P_KaatCategoryId = kaat.KaatCategoryId,
                                p_VendorId = updatedVendorId,
                                p_Label = kaat.Label,
                                p_Value = kaat.Value,
                                p_UpdatedBy = vendor.UpdatedBy // Assuming UpdatedBy field exists
                            },
                            transaction: transaction,
                            commandType: CommandType.StoredProcedure
                        );

                    }
                }

                await transaction.CommitAsync();
                response = updatedVendorId;
            }
            catch
            {
                await transaction.RollbackAsync();
            }
            finally
            {
                await connection.DisposeAsync();
            }

            return response > 0;
        }
        public async Task<Vendor> GetVendorById(int vendorId)
        {
            var result = new Vendor();
            result.KaatCategory = new List<KaatCategory>();
            var parameters = new List<DbParameter>
            {
                base.GetParameter("p_VendorId",vendorId)
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetVendorById_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        result.VendorId = dataReader.GetIntegerValue("vendorId");
                        result.Description = dataReader.GetStringValue("description");
                        result.SerialNumber = dataReader.GetStringValue("serialNumber");
                        result.Contact = dataReader.GetStringValue("contact");
                        result.TotalAddedStock = dataReader.GetDecimalValue("totalAddedStock");
                        result.TotalAvailableStock = dataReader.GetDecimalValue("totalAvailableStock");
                        result.CashDue = dataReader.GetDecimalValue("cashDue");
                        result.GoldDue = dataReader.GetDecimalValue("goldDue");
                        result.IsActive = dataReader.GetBooleanValue("isActive");
                        result.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        result.CreatedAt = dataReader.GetDateTime("createdAt");
                        result.CreatedBy = dataReader.GetIntegerValue("createdBy");
                    }
                }
                if (dataReader.NextResult())
                {
                    while (dataReader.Read())
                    {
                        var item = new KaatCategory();
                        item.KaatCategoryId = dataReader.GetIntegerValue("kaatCategoryId");
                        item.VendorId = dataReader.GetIntegerValue("vendorId");
                        item.Label = dataReader.GetStringValue("label");
                        item.Value = dataReader.GetDecimalValue("value");
                        item.KaatStock = dataReader.GetDecimalValue("KaatStock");
                        result.KaatCategory.Add(item);
                    }
                }
            }
            return result;
        }
        public async Task<List<Vendor>> GetAllVendors()
        {
            var Response = new List<Vendor>();
            var parameters = new List<DbParameter>
            {
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetAllVendors_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        var result = new Vendor();
                        result.VendorId = dataReader.GetIntegerValue("vendorId");
                        result.Description = dataReader.GetStringValue("description");
                        result.SerialNumber = dataReader.GetStringValue("serialNumber");
                        result.Contact = dataReader.GetStringValue("contact");
                        result.IsActive = dataReader.GetBooleanValue("isActive");
                        result.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        result.CreatedAt = dataReader.GetDateTime("createdAt");
                        result.CreatedBy = dataReader.GetIntegerValue("createdBy");
                        result.KaatCategory = new List<KaatCategory>();
                        Response.Add(result);
                    }
                }
                if (dataReader.NextResult())
                {
                    while (dataReader.Read())
                    {
                        var item = new KaatCategory();
                        item.KaatCategoryId = dataReader.GetIntegerValue("kaatCategoryId");
                        item.VendorId = dataReader.GetIntegerValue("vendorId");
                        item.Label = dataReader.GetStringValue("label");
                        item.Value = dataReader.GetDecimalValue("value");
                        var kaatItem = Response.FirstOrDefault(o => o.VendorId == item.VendorId);
                        if (kaatItem != null)
                        {
                            kaatItem?.KaatCategory?.Add(item);
                        }
                    }
                }
            }
            return Response;
        }
        #endregion

        #region Live Gold Rate
        public async Task<bool> AddUpdateMetalPurity(List<MetalPurity> metalPurities)
        {
            var response = 0;
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                foreach (var metalPurity in metalPurities)
                {
                    var affectedRows = await connection.ExecuteAsync(
                                 "AddUpdateMetalPurity_Gb",
                                 new
                                 {
                                     P_MetalPurityId = metalPurity.MetalPurityId,
                                     p_Description = metalPurity.Description,
                                     p_UnitPrice = metalPurity.UnitPrice,
                                     p_MetalTypeId = metalPurity.MetalTypeId,
                                     p_StoreId = metalPurity.StoreId,
                                     p_CreatedBy = metalPurity.CreatedBy,
                                     p_PurityPercentage = metalPurity.PurityPercentage
                                 },
                                 transaction: transaction,
                                 commandType: CommandType.StoredProcedure
                             );

                    response += affectedRows;
                }
                await transaction.CommitAsync();

            }
            catch
            {
                await transaction.RollbackAsync();
            }
            finally
            {
                await connection.DisposeAsync();

            }
            return response > 0;
        }
        public async Task<IEnumerable<MetalPurity>> GetMetalPurityHistory(MetalPurityVm entity)
        {
            var list = new List<MetalPurity>();
            var parameters = new List<DbParameter>
            {
                base.GetParameter("p_MetalTypeId", entity.MetalTypeId),
                base.GetParameter("p_MetalPurityIds", entity.MetalPurityIds),
                base.GetParameter("p_StoreIds", entity.StoreIds),
                base.GetParameter("p_StartDate", entity.StartDate),
                base.GetParameter("p_EndDate", entity.EndDate),
                base.GetParameter("p_AggregationType", entity.AggregationType)
        
            };
            using (var dataReader = await ExecuteReader(parameters, "GetMetalPurityHistory_Gb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new MetalPurity
                        {
                            MetalPurityId = dataReader.GetIntegerValue("MetalPurityId"),
                            Description = dataReader.GetStringValue("description"),
                            UnitPrice = dataReader.GetDecimalValue("unitPrice"),
                            MetalTypeId = dataReader.GetIntegerValue("metalTypeId"),
                            PurityPercentage = dataReader.GetDecimalValue("purityPercentage"),
                            StoreId = dataReader.GetIntegerValue("storeId"),
                            UpdatedAt = dataReader.GetDateTimeValue("updatedAt")
                        });
                    }
                }
            }
            return list;
        }

        #endregion

        #region Raw Gold
        public async Task<int> AddRawGold(RawGold RawGold)
        {
            var response = 0;
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_Description", RawGold.Description);
                parameters.Add("p_Value", RawGold.Value);
                parameters.Add("p_CreatedBy", RawGold.CreatedBy);
                parameters.Add("o_RawGoldId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                response = await connection.ExecuteAsync("AddRawGold_gb", parameters, transaction, commandType: CommandType.StoredProcedure);
                var rawGoldId = parameters.Get<int>("o_RawGoldId");
                await transaction.CommitAsync();

                response = rawGoldId;
            }
            catch
            {
                await transaction.RollbackAsync();
            }
            finally
            {
                await connection.DisposeAsync();
            }

            return response;
        }
        public async Task<List<RawGold>> GetAllRawGolds()
        {
            var result = new List<RawGold>();
            var parameters = new List<DbParameter>
            {
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetAllRawGold_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    { 
                        var rawGold = new RawGold();
                        rawGold.Value = dataReader.GetDecimalValue("value");
                        rawGold.RawGoldId = dataReader.GetIntegerValue("rawGoldId");
                        rawGold.Description = dataReader.GetStringValue("description");
                        rawGold.IsActive = dataReader.GetBooleanValue("isActive");
                        rawGold.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        rawGold.CreatedAt = dataReader.GetDateTime("createdAt");
                        rawGold.CreatedBy = dataReader.GetIntegerValue("createdBy");
                        result.Add(rawGold);
                    }
                }
            }
            return result;
        }
        public async Task<bool> RemoveRawGold(RawGold RawGold)
        {
            var response = 0;
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_Description", RawGold.Description);
                parameters.Add("p_Value", RawGold.Value);
                parameters.Add("p_CreatedBy", RawGold.CreatedBy);
                parameters.Add("o_RawGoldId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                response = await connection.ExecuteAsync("RemoveRawGold_gb", parameters, transaction, commandType: CommandType.StoredProcedure);
                var rawGoldId = parameters.Get<int>("o_RawGoldId");
                await transaction.CommitAsync();

                response = rawGoldId;
            }
            catch
            {
                await transaction.RollbackAsync();
            }
            finally
            {
                await connection.DisposeAsync();
            }

            return response > 0;
        }
        public async Task<RawGold> GetRawGoldById(int rawGoldId)
        {
            var rawGold = new RawGold();
            var parameters = new List<DbParameter>
            {
                base.GetParameter("p_RawGoldId", rawGoldId)
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetRawGoldById_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        rawGold.Value = dataReader.GetDecimalValue("value");
                        rawGold.RawGoldId = dataReader.GetIntegerValue("rawGoldId");
                        rawGold.Description = dataReader.GetStringValue("description");
                        rawGold.IsActive = dataReader.GetBooleanValue("isActive");
                        rawGold.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        rawGold.CreatedAt = dataReader.GetDateTime("createdAt");
                        rawGold.CreatedBy = dataReader.GetIntegerValue("createdBy");
                    }
                }
            }
            return rawGold;
        }
        public async Task<AssetSummary> GetAssetSummary()
        {
            var assetSummary = new AssetSummary();
            var parameters = new List<DbParameter>
            {
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetAssetsSummary_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        assetSummary.TotalGold = dataReader.GetDecimalValue("totalGold");
                        assetSummary.TotalInventory = dataReader.GetDecimalValue("totalInventory");
                        assetSummary.RawGold = dataReader.GetDecimalValue("RawGold");
                    }
                }
            }
            return assetSummary;
        }
        public async Task<int> AddUpdateLabel(Label label)
        {
            var response = 0;
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_Description", label.Description);
                parameters.Add("p_CreatedBy", label.CreatedBy);
                parameters.Add("o_labelId", dbType: DbType.Int32, direction: ParameterDirection.Output);

                response = await connection.ExecuteAsync("AddOrUpdateLabel_gb", parameters, transaction, commandType: CommandType.StoredProcedure);
                var labelId = parameters.Get<int>("o_labelId");
                await transaction.CommitAsync();

                response = labelId;
            }
            catch
            {
                await transaction.RollbackAsync();
            }
            finally
            {
                await connection.DisposeAsync();
            }
            return response;
        }
        public async Task<int> AddProductsLabel(ProductLabel productLabel)
        {
            var response = 0;
            using var connection = base.GetConnection();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("p_LabelId", productLabel.LabelId);
                parameters.Add("p_ProductIds", productLabel.ProductIds);
                parameters.Add("p_CreatedBy", productLabel.CreatedBy);

                response = await connection.ExecuteAsync("UpsertProductLabel_gb", parameters, transaction, commandType: CommandType.StoredProcedure);
                //var labelId = parameters.Get<int>("o_labelId");
                await transaction.CommitAsync();
                
            }
            catch
            {
                await transaction.RollbackAsync();
            }
            finally
            {
                await connection.DisposeAsync();
            }
            return response;
        }
        #endregion
    }
}
