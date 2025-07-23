using Amazon.Runtime.Internal;
using Dapper;
using GoldBank.Infrastructure.Extension;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;
using GoldBank.Models.Product;
using GoldBank.Models.RequestModels;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using MySqlX.XDevAPI.Common;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Utilities.Collections;
using Renci.SshNet.Compression;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Transactions;

namespace GoldBank.Infrastructure.Infrastructure
{
    public class ProductInfrastructure : BaseInfrastructure, IProductInfrastructure
    {
        public ProductInfrastructure(IConfiguration configuration,ICustomerInfrastructure customerInfrastructure) : base(configuration)
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

        public Task<bool> Activate(Product entity)
        {
            throw new NotImplementedException();
        }

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

        private async Task<int> AddOld(Product product)
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
                        p_TotalPrice = product.Jewellery.TotalPrice,
                        p_Title = product.Title
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

                        foreach(var doc in stone.StoneDocuments)
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
        public async Task<int> Add(Product product)
        {
            //return await this.AddOld(product); //old method
            return await this.AddProduct(product,null,null);
        }
        public async Task<bool> Update(Product product)
        {
            //return await this.UpdateOld(product); //old method
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

                if (isOwnConnection)
                    await transaction.CommitAsync();

                return productId;
            }
            catch(Exception ex)
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
                            p_RepairStoneDetailId = doc.RepairStoneDetailId ,
                            p_RepairDetailId = doc.RepairDetailId,
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
                            p_RepairDetailId = doc.RepairDetailId,
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
            base.GetParameter("@p_StoneWeightTypeId", ToDbValue(product.Data.StoneWeightTypeId)),
            base.GetParameter("@p_ReferenceSKU", ToDbValue(product.Data.ReferenceSKU)),
            base.GetParameter("@p_IsSold", ToDbValue(product.Data.IsSold)),
            base.GetParameter("@p_IsReserved", ToDbValue(product.Data.IsReserved))
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
                        item.ProductSource.ProductSourceId = dataReader.GetIntegerValue("productSourceId");
                        item.ProductSource.Description = dataReader.GetStringValue("ProductSourceDescription");
                        item.Vendor.VendorId = dataReader.GetIntegerValue("vendorId");
                        item.Vendor.Description = dataReader.GetStringValue("VendorDescription");
                        item.Title = dataReader.GetStringValue("title");
                        item.ReferenceSKU = dataReader.GetStringValue("referenceSKU");
                        item.IsSold = dataReader.GetBooleanValue("isSold");
                        item.IsReserved = dataReader.GetBooleanValue("isReserved");

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
                            item.LacquerPrice = dataReader.GetDecimalValue("lacquerPrice");
                            item.MakingPrice = dataReader.GetDecimalValue("makingPrice");
                            item.TotalPrice = dataReader.GetDecimalValue("totalPrice");
                            item.IsActive = dataReader.GetBooleanValue("isActive");
                            item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                            item.CreatedAt = dataReader.GetDateTimeValue("createdAt");
                            item.CreatedBy = dataReader.GetIntegerValue("createdBy");
                            item.MinWeight = dataReader.GetDecimalValue("minWeight");
                            item.MaxWeight = dataReader.GetDecimalValue("maxWeight");

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
                        item.ProductSource.ProductSourceId = dataReader.GetIntegerValue("productSourceId");
                        item.Vendor.VendorId = dataReader.GetIntegerValue("vendorId");
                        item.Title = dataReader.GetStringValue("title");
                        item.ReferenceSKU = dataReader.GetStringValue("referenceSKU");
                        item.IsSold = dataReader.GetBooleanValue("isSold");
                        item.IsReserved = dataReader.GetBooleanValue("isReserved");

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
                        item.Title = dataReader.GetStringValue("title");
                        item.ReferenceSKU = dataReader.GetStringValue("referenceSKU");
                        item.IsSold = dataReader.GetBooleanValue("isSold");
                        item.IsReserved = dataReader.GetBooleanValue("isReserved");

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
                            item.LacquerPrice = dataReader.GetDecimalValue("lacquerPrice");
                            item.MakingPrice = dataReader.GetDecimalValue("makingPrice"); 
                            item.TotalPrice = dataReader.GetDecimalValue("totalPrice");
                            item.IsActive = dataReader.GetBooleanValue("isActive");
                            item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                            item.CreatedAt = dataReader.GetDateTimeValue("createdAt");
                            item.CreatedBy = dataReader.GetIntegerValue("createdBy");
                            item.MinWeight = dataReader.GetDecimalValue("minWeight");
                            item.MaxWeight = dataReader.GetDecimalValue("maxWeight");

                            if(Product.ProductId == item.ProductId)
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
                }
            }
            return Product;
        }
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
                else if(order.OrderTypeId == 1 || order.OrderTypeId == 3 || order.OrderTypeId == 4) // direct Sale orders + rserved + alteration
                {
                    order.Product.IsReserved = true;
                    order.Product.IsSold = false;
                    bool isUpdated = await this.UpdateProduct(order.Product, connection, transaction);                    
                }
                if (order.AlterationDetails != null && order.OrderTypeId == 4) // direct order alteration 
                {
                    order.AlterationDetailsId = await this.AddAlterationDetails(order.AlterationDetails, connection, transaction);
                }
                if(order.RepairDetails != null && order.OrderTypeId == 5) // repair order
                {
                    order.RepairDetailsId = await this.AddRepairDetails(order.RepairDetails, connection, transaction);
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
                base.GetParameter("@p_OrderStatusId", ToDbValue(product.Data.OrderStatusId))
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
                        item.PaymentReceived = dataReader.GetDecimalValue("paymentReceived");
                        item.AdvancePayment = dataReader.GetDecimalValue("advancePayment");
                        item.PendingPayment = dataReader.GetDecimalValue("pendingPayment");
                        item.AdvancePaymentPct = dataReader.GetDecimalValue("advancePaymentPct");
                        item.TotalPayment = dataReader.GetDecimalValue("totalPayment");
                        item.OrderStatusId = dataReader.GetIntegerValue("OrderStatusId");
                        item.OrderDelievery.DelieveryMethodId = dataReader.GetIntegerValue("delieveryMethodId");
                        item.OrderDelievery.EstDelieveryDate = dataReader.GetDateTimeValue("estDelieveryDate");
                        item.OrderDelievery.ShippingCost = dataReader.GetIntegerValue("shippingCost");
                        item.OrderDelievery.DelieveryAddress = dataReader.GetStringValue("delieveryAddress");

                        Customer.CustomerId = item.CustomerId;
                        item.Customer = await this.CustomerInfrastructure.Get(Customer);
                        //var productItem = AllProducts.FirstOrDefault(p => p.ProductId == item.ProductId);
                        //if (productItem != null)
                        //{
                        //    item.Product = productItem;
                        //}
                        item.Product = await this.GetProductById(item.ProductId);
                        OrderList.Add(item);
                    }
                }

                if (dataReader.NextResult())
                {
                    while(dataReader.Read())
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
                if (order.Product != null)
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
                if (order.RepairDetails != null && order.OrderTypeId == 5)
                {
                    var res = await this.UpdateRepairDetails(order.RepairDetails, connection, transaction);
                    if (res <1)
                        throw new Exception("Failed to update repair.");
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

                        item.OrderDelievery.DelieveryMethodId = dataReader.GetIntegerValue("delieveryMethodId") ;
                        item.OrderDelievery.EstDelieveryDate = dataReader.GetDateTimeValue("estDelieveryDate");
                        item.OrderDelievery.ShippingCost = dataReader.GetIntegerValue("shippingCost");
                        item.OrderDelievery.DelieveryAddress = dataReader.GetStringValue("delieveryAddress");


                        Customer.CustomerId = item.CustomerId;
                        item.Customer = await this.CustomerInfrastructure.Get(Customer);
                        item.Product = await this.GetProductById(item.ProductId);
                        if (item.OrderTypeId == 4)
                        {
                            item.AlterationDetails = await this.GetAlterationDetailsById(orderId);
                        }
                        else if (item.OrderTypeId == 5)
                        {
                            item.RepairDetails = await this.GetRepairDetailsById(orderId);
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
        public async Task<bool> UpdateOrderById(Order order)
        {
            var parameters = new List<DbParameter>
            {
                base.GetParameter("p_OrderId",order.OrderId),
                base.GetParameter("p_OrderStatusId", order.OrderStatusId),
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
                        p_StoneAlterationId = stone.StoneAlterationId
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
                        p_StoneAlterationId = stone.StoneAlterationId
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

                        item.IsActive = dataReader.GetBooleanValue("isActive");
                        item.IsDeleted = dataReader.GetBooleanValue("isDeleted");
                        item.CreatedAt = dataReader.GetDateTime("createdAt");
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
                            item.IsPrimary = dataReader.GetBooleanValue("isPrimary");
                            item.IsPostRepair = dataReader.GetBooleanValue("isPostRepair");
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
                            var item = new RepairStoneDetails();

                            item.RepairStoneDetailId = dataReader.GetIntegerValue("repairStoneDetailId");
                            item.RepairDetailId = dataReader.GetIntegerValue("repairDetailId");
                            item.CurrentStoneId = dataReader.GetIntegerValue("currentStoneId");
                            item.DesiredStoneId = dataReader.GetIntegerValue("desiredStoneId");
                            item.IsFixed = dataReader.GetBooleanValue("isFixed");
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


    }
}
