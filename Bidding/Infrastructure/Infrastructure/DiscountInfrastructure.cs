using Dapper;
using GoldBank.Infrastructure.Extension;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;
using GoldBank.Models.RequestModels;
using MySqlX.XDevAPI.Common;
using System.Data;
using System.Data.Common;
using static Org.BouncyCastle.Math.EC.ECCurve;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GoldBank.Infrastructure.Infrastructure
{
    public class DiscountInfrastructure : BaseInfrastructure, IDiscountInfrastructure
    {
        public DiscountInfrastructure(IConfiguration configuration) : base(configuration)
        {

        }


        public Task<bool> Activate(Discount entity)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Add(Discount entity)
        {
            using var connection = base.GetConnection();

            var parameters = new DynamicParameters();
            parameters.Add("p_DiscountTypeId", entity.DiscountTypeId);
            parameters.Add("p_Name", entity.Name);
            parameters.Add("p_CardDisplayName", entity.CardDisplayName);
            parameters.Add("p_Code", entity.Code);
            parameters.Add("p_MinInvoiceAmount", entity.MinInvoiceAmount);
            parameters.Add("p_MaxUsage", entity.MaxUsage);
            parameters.Add("p_PersonName", entity.PersonName);
            parameters.Add("p_Description", entity.Description);
            parameters.Add("p_SalesComissionPct", entity.SalesComissionPct);
            parameters.Add("p_MaxUser", entity.MaxUser);
            parameters.Add("p_CustomerId", entity.CustomerId);
            parameters.Add("p_ExpiryDuration", entity.ExpiryDuration);
            parameters.Add("p_ExpiryDurationType", entity.ExpiryDurationType);
            parameters.Add("p_LoyaltyCardTypeId", entity.LoyaltyCardTypeId);
            parameters.Add("p_VoucherTypeId", entity.VoucherTypeId);
            parameters.Add("p_PrimaryCategories", entity.PrimaryCategories);
            parameters.Add("p_CategoryIds", entity.CategoryIds);
            parameters.Add("p_SubCategoryIds", entity.SubCategoryIds);
            parameters.Add("p_CollectionTypeIds", entity.CollectionTypeIds);
            parameters.Add("p_LabelTypeIds", entity.LabelTypeIds);
            parameters.Add("p_DiscountAmount", entity.DiscountAmount);
            parameters.Add("p_DiscountPct", entity.DiscountPct);
            parameters.Add("p_StartDate", entity.StartDate);
            parameters.Add("p_EndDate", entity.EndDate);
            parameters.Add("p_IsEcommerce", entity.IsEcommerce);
            parameters.Add("p_IsInStore", entity.IsInStore);
            parameters.Add("p_StoreIds", entity.StoreIds);
            parameters.Add("p_CreatedBy", entity.CreatedBy);
            parameters.Add("o_DiscountId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("AddDiscountGb", parameters, commandType: CommandType.StoredProcedure);

            var discountId = parameters.Get<int>("o_DiscountId");
            return discountId;
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
        public async Task<Discount> Get(Discount entity)
        {
            var discount = new Discount();
            var parameters = new List<DbParameter>
            {
                 base.GetParameter("p_DiscountId", entity.DiscountId),
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetDiscountById_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        discount.DiscountId = dataReader.GetIntegerValue("DiscountId");
                        discount.DiscountTypeId = dataReader.GetIntegerValue("DiscountTypeId");
                        discount.Name = dataReader.GetStringValue("Name");
                        discount.CardDisplayName = dataReader.GetStringValue("CardDisplayName");
                        discount.Code = dataReader.GetStringValue("Code");
                        discount.MinInvoiceAmount = dataReader.GetDecimalValue("MinInvoiceAmount");
                        discount.MaxUsage = dataReader.GetIntegerValue("MaxUsage");
                        discount.PersonName = dataReader.GetStringValue("PersonName");
                        discount.Description = dataReader.GetStringValue("Description");
                        discount.SalesComissionPct = dataReader.GetDecimalValue("SalesComissionPct");
                        discount.MaxUser = dataReader.GetIntegerValue("MaxUser");
                        discount.CustomerId = dataReader.GetIntegerValue("CustomerId");
                        discount.ExpiryDuration = dataReader.GetIntegerValue("ExpiryDuration");
                        discount.ExpiryDurationType = dataReader.GetIntegerValue("ExpiryDurationType");
                        discount.LoyaltyCardTypeId = dataReader.GetIntegerValue("LoyaltyCardTypeId");
                        discount.VoucherTypeId = dataReader.GetIntegerValue("VoucherTypeId");
                        discount.PrimaryCategories = dataReader.GetStringValue("PrimaryCategories");
                        discount.CategoryIds = dataReader.GetStringValue("CategoryIds");
                        discount.SubCategoryIds = dataReader.GetStringValue("SubCategoryIds");
                        discount.CollectionTypeIds = dataReader.GetStringValue("CollectionTypeIds");
                        discount.LabelTypeIds = dataReader.GetStringValue("LabelTypeIds");
                        discount.DiscountAmount = dataReader.GetDecimalValue("DiscountAmount");
                        discount.DiscountPct = dataReader.GetDecimalValue("DiscountPct");
                        discount.StartDate = dataReader.GetDateTimeValue("StartDate");
                        discount.EndDate = dataReader.GetDateTimeValue("EndDate");
                        discount.IsEcommerce = dataReader.GetBooleanValue("IsEcommerce");
                        discount.IsInStore = dataReader.GetBooleanValue("IsInStore");
                        discount.StoreIds = dataReader.GetStringValue("StoreIds");
                        discount.IsActive = dataReader.GetBooleanValue("IsActive");
                        discount.UpdatedAt = dataReader.GetDateTimeValue("UpdatedAt");
                        discount.UpdatedBy = dataReader.GetIntegerValue("UpdatedBy");
                        discount.CreatedAt = dataReader.GetDateTimeValue("CreatedAt");
                        discount.CreatedBy = dataReader.GetIntegerValue("CreatedBy");
                    }
                }
            }

            return discount;
        }
        public  Task<AllResponse<Discount>> GetAll(AllRequest<Discount> entity)
        {
            throw new NotImplementedException();
        }
        public async  Task<AllResponse<Discount>> GetAllDiscounts(AllRequest<DiscountRM> entity)
        {
            var result = new AllResponse<Discount>();
            var data = new List<Discount>();
            var voucherType = new VoucherType ();
            var loyaltyCardType = new LoyaltyCardType();

            var parameters = new List<DbParameter>
            {
                base.GetParameter("p_PageNumber", entity.Offset),
                base.GetParameter("p_PageSize", entity.PageSize),
                base.GetParameter("p_DiscountTypeId", entity.Data.DiscountTypeId > 0 ? (object)entity.Data.DiscountTypeId : DBNull.Value)
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetAllDiscount_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        var discount = new Discount();
                        discount.DiscountId = dataReader.GetIntegerValue("DiscountId");
                        discount.DiscountTypeId = dataReader.GetIntegerValue("DiscountTypeId");
                        discount.Name = dataReader.GetStringValue("Name");
                        discount.CardDisplayName = dataReader.GetStringValue("CardDisplayName");
                        discount.Code = dataReader.GetStringValue("Code");
                        discount.MinInvoiceAmount = dataReader.GetDecimalValue("MinInvoiceAmount");
                        discount.MaxUsage = dataReader.GetIntegerValue("MaxUsage");
                        discount.PersonName = dataReader.GetStringValue("PersonName");
                        discount.Description = dataReader.GetStringValue("Description");
                        discount.SalesComissionPct = dataReader.GetDecimalValue("SalesComissionPct");
                        discount.MaxUser = dataReader.GetIntegerValue("MaxUser");
                        discount.CustomerId = dataReader.GetIntegerValue("CustomerId");
                        discount.ExpiryDuration = dataReader.GetIntegerValue("ExpiryDuration");
                        discount.ExpiryDurationType = dataReader.GetIntegerValue("ExpiryDurationType");
                        discount.LoyaltyCardTypeId = dataReader.GetIntegerValue("LoyaltyCardTypeId");
                        discount.VoucherTypeId = dataReader.GetIntegerValue("VoucherTypeId");
                        discount.PrimaryCategories = dataReader.GetStringValue("PrimaryCategories");
                        discount.CategoryIds = dataReader.GetStringValue("CategoryIds");
                        discount.SubCategoryIds = dataReader.GetStringValue("SubCategoryIds");
                        discount.CollectionTypeIds = dataReader.GetStringValue("CollectionTypeIds");
                        discount.LabelTypeIds = dataReader.GetStringValue("LabelTypeIds");
                        discount.CustomerFirstName = dataReader.GetStringValue("firstName");
                        discount.CustomerLastName = dataReader.GetStringValue("lastName");
                        discount.DiscountAmount = dataReader.GetDecimalValue("DiscountAmount");
                        discount.DiscountPct = dataReader.GetDecimalValue("DiscountPct");
                        discount.StartDate = dataReader.GetDateTimeValue("StartDate");
                        discount.EndDate = dataReader.GetDateTimeValue("EndDate");
                        discount.IsEcommerce = dataReader.GetBooleanValue("IsEcommerce");
                        discount.IsInStore = dataReader.GetBooleanValue("IsInStore");
                        discount.StoreIds = dataReader.GetStringValue("StoreIds");
                        discount.IsActive = dataReader.GetBooleanValue("IsActive");
                        discount.UpdatedAt = dataReader.GetDateTimeValue("UpdatedAt");
                        discount.UpdatedBy = dataReader.GetIntegerValue("UpdatedBy");
                        discount.CreatedAt = dataReader.GetDateTimeValue("CreatedAt");
                        discount.CreatedBy = dataReader.GetIntegerValue("CreatedBy");
                        if (discount.DiscountTypeId == 4)
                        {
                            voucherType.VoucherTypeId = discount.VoucherTypeId ?? 0;
                            discount.VoucherType = await this.GetVoucherType(voucherType);
                        }
                        if (discount.DiscountTypeId == 3)
                        {
                            loyaltyCardType.LoyaltyCardTypeId = discount.LoyaltyCardTypeId ?? 0;
                            discount.LoyaltyCardType = await this.GetLoyaltyCardType(loyaltyCardType);
                        }
                        data.Add(discount);
                    }
                    if (dataReader.NextResult())
                    {
                        while (dataReader.Read())
                        {
                            result.TotalRecord = dataReader.GetIntegerValue("TotalRecords");
                        }
                    }
                }
            }
            result.Data = data;
            return result;
        }

        public Task<List<Discount>> GetList(Discount entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Update(Discount entity)
        {
            using var connection = base.GetConnection();

            var parameters = new DynamicParameters();
            parameters.Add("p_DiscountId", entity.DiscountId);
            parameters.Add("p_DiscountTypeId", entity.DiscountTypeId);
            parameters.Add("p_Name", entity.Name);
            parameters.Add("p_CardDisplayName", entity.CardDisplayName);
            parameters.Add("p_Code", entity.Code);
            parameters.Add("p_MinInvoiceAmount", entity.MinInvoiceAmount);
            parameters.Add("p_MaxUsage", entity.MaxUsage);
            parameters.Add("p_PersonName", entity.PersonName);
            parameters.Add("p_Description", entity.Description);
            parameters.Add("p_SalesComissionPct", entity.SalesComissionPct);
            parameters.Add("p_MaxUser", entity.MaxUser);
            parameters.Add("p_CustomerId", entity.CustomerId);
            parameters.Add("p_ExpiryDuration", entity.ExpiryDuration);
            parameters.Add("p_ExpiryDurationType", entity.ExpiryDurationType);
            parameters.Add("p_LoyaltyCardTypeId", entity.LoyaltyCardTypeId);
            parameters.Add("p_VoucherTypeId", entity.VoucherTypeId);
            parameters.Add("p_PrimaryCategories", entity.PrimaryCategories);
            parameters.Add("p_CategoryIds", entity.CategoryIds);
            parameters.Add("p_SubCategoryIds", entity.SubCategoryIds);
            parameters.Add("p_CollectionTypeIds", entity.CollectionTypeIds);
            parameters.Add("p_LabelTypeIds", entity.LabelTypeIds);
            parameters.Add("p_DiscountAmount", entity.DiscountAmount);
            parameters.Add("p_DiscountPct", entity.DiscountPct);
            parameters.Add("p_StartDate", entity.StartDate);
            parameters.Add("p_EndDate", entity.EndDate);
            parameters.Add("p_IsEcommerce", entity.IsEcommerce);
            parameters.Add("p_IsInStore", entity.IsInStore);
            parameters.Add("p_StoreIds", entity.StoreIds);
            parameters.Add("p_UpdatedBy", entity.UpdatedBy);
            parameters.Add("o_IsUpdated", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("UpdateDiscountGb", parameters, commandType: CommandType.StoredProcedure);

            var discountId = parameters.Get<int>("o_IsUpdated");
            return discountId > 0;
        }
        public async Task<bool> UpdateDiscountStatus(DiscountURM discount)
        {
            using var connection = base.GetConnection();

            var parameters = new DynamicParameters();
            parameters.Add("p_DiscountId", discount.DiscountId);
            parameters.Add("p_IsActive", discount.IsActive);
            parameters.Add("p_IsDeleted", discount.IsDeleted);
            parameters.Add("p_UpdatedBy", discount.UpdatedBy);
            parameters.Add("o_IsUpdated", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("UpdateDiscountStatusGb", parameters, commandType: CommandType.StoredProcedure);
            var discountId = parameters.Get<int>("o_IsUpdated");
            return discountId > 0;

        }

        #region Voucher Type
        public async Task<int> AddVoucherType(VoucherType entity)
        {
            using var connection = base.GetConnection();

            var parameters = new DynamicParameters();;
            parameters.Add("p_Name", entity.Name);
            parameters.Add("p_MinInvoiceAmount", entity.MinInvoiceAmount);
            parameters.Add("p_MaxUsage", entity.MaxUsage);
            parameters.Add("p_Description", entity.Description);            
            parameters.Add("p_ExpiryDuration", entity.ExpiryDuration);
            parameters.Add("p_ExpiryDurationType", entity.ExpiryDurationType);
            parameters.Add("p_PrimaryCategories", entity.PrimaryCategories);
            parameters.Add("p_CategoryIds", entity.CategoryIds);
            parameters.Add("p_SubCategoryIds", entity.SubCategoryIds);
            parameters.Add("p_CollectionTypeIds", entity.CollectionTypeIds);
            parameters.Add("p_LabelTypeIds", entity.LabelTypeIds);
            parameters.Add("p_DiscountAmount", entity.DiscountAmount);
            parameters.Add("p_DiscountPct", entity.DiscountPct);
            parameters.Add("p_IsEcommerce", entity.IsEcommerce);
            parameters.Add("p_IsInStore", entity.IsInStore);
            parameters.Add("p_CreatedBy", entity.CreatedBy);
            parameters.Add("o_VoucherTypeId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("AddVoucherTypeGb", parameters, commandType: CommandType.StoredProcedure);

            var discountId = parameters.Get<int>("o_VoucherTypeId");
            return discountId;
        }
        public async Task<bool> UpdateVoucherType(VoucherType entity)
        {
            using var connection = base.GetConnection();

            var parameters = new DynamicParameters(); 
            parameters.Add("p_Name", entity.Name);
            parameters.Add("p_MinInvoiceAmount", entity.MinInvoiceAmount);
            parameters.Add("p_MaxUsage", entity.MaxUsage);
            parameters.Add("p_Description", entity.Description);
            parameters.Add("p_ExpiryDuration", entity.ExpiryDuration);
            parameters.Add("p_ExpiryDurationType", entity.ExpiryDurationType);
            parameters.Add("p_VoucherTypeId", entity.VoucherTypeId);
            parameters.Add("p_PrimaryCategories", entity.PrimaryCategories);
            parameters.Add("p_CategoryIds", entity.CategoryIds);
            parameters.Add("p_SubCategoryIds", entity.SubCategoryIds);
            parameters.Add("p_CollectionTypeIds", entity.CollectionTypeIds);
            parameters.Add("p_LabelTypeIds", entity.LabelTypeIds);
            parameters.Add("p_DiscountAmount", entity.DiscountAmount);
            parameters.Add("p_DiscountPct", entity.DiscountPct);
            parameters.Add("p_IsEcommerce", entity.IsEcommerce);
            parameters.Add("p_IsInStore", entity.IsInStore);
            parameters.Add("p_CreatedBy", entity.UpdatedBy);
            parameters.Add("o_IsUpdated", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("UpdateVoucherTypeGb", parameters, commandType: CommandType.StoredProcedure);

            var updated = parameters.Get<int>("o_IsUpdated");
            return updated > 0;
        }
        public async Task<VoucherType> GetVoucherType(VoucherType voucherType)
        {
            var discount = new VoucherType();
            var parameters = new List<DbParameter>
            {
                 base.GetParameter("p_VoucherTypeId", voucherType.VoucherTypeId),
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetVoucherTypeById_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        discount.Name = dataReader.GetStringValue("Name");
                        discount.MinInvoiceAmount = dataReader.GetDecimalValue("MinInvoiceAmount");
                        discount.MaxUsage = dataReader.GetIntegerValue("MaxUsage");
                        discount.Description = dataReader.GetStringValue("Description");
                        
                        discount.ExpiryDuration = dataReader.GetIntegerValue("ExpiryDuration");
                        discount.ExpiryDurationType = dataReader.GetIntegerValue("ExpiryDurationType");
                        discount.VoucherTypeId = dataReader.GetIntegerValue("VoucherTypeId");
                        discount.PrimaryCategories = dataReader.GetStringValue("PrimaryCategories");
                        discount.CategoryIds = dataReader.GetStringValue("CategoryIds");
                        discount.SubCategoryIds = dataReader.GetStringValue("SubCategoryIds");
                        discount.CollectionTypeIds = dataReader.GetStringValue("CollectionTypeIds");
                        discount.LabelTypeIds = dataReader.GetStringValue("LabelTypeIds");
                        discount.DiscountAmount = dataReader.GetDecimalValue("DiscountAmount");
                        discount.DiscountPct = dataReader.GetDecimalValue("DiscountPct");
                        discount.IsEcommerce = dataReader.GetBooleanValue("IsEcommerce");
                        discount.IsInStore = dataReader.GetBooleanValue("IsInStore");
                        discount.IsActive = dataReader.GetBooleanValue("IsActive");
                        discount.UpdatedAt = dataReader.GetDateTimeValue("UpdatedAt");
                        discount.UpdatedBy = dataReader.GetIntegerValue("UpdatedBy");
                        discount.CreatedAt = dataReader.GetDateTimeValue("CreatedAt");
                        discount.CreatedBy = dataReader.GetIntegerValue("CreatedBy");
                    }
                }
            }

            return discount;
        }

        public async Task<List<VoucherType>> GetAllVoucherType(VoucherType voucherType)
        {
            var result  = new List<VoucherType>();
            var parameters = new List<DbParameter>
            {
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetAllVoucherType_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        var discount = new VoucherType();
                        discount.Name = dataReader.GetStringValue("Name");
                        discount.MinInvoiceAmount = dataReader.GetDecimalValue("MinInvoiceAmount");
                        discount.MaxUsage = dataReader.GetIntegerValue("MaxUsage");
                        discount.Description = dataReader.GetStringValue("Description");

                        discount.ExpiryDuration = dataReader.GetIntegerValue("ExpiryDuration");
                        discount.ExpiryDurationType = dataReader.GetIntegerValue("ExpiryDurationType");
                        discount.VoucherTypeId = dataReader.GetIntegerValue("VoucherTypeId");
                        discount.PrimaryCategories = dataReader.GetStringValue("PrimaryCategories");
                        discount.CategoryIds = dataReader.GetStringValue("CategoryIds");
                        discount.SubCategoryIds = dataReader.GetStringValue("SubCategoryIds");
                        discount.CollectionTypeIds = dataReader.GetStringValue("CollectionTypeIds");
                        discount.LabelTypeIds = dataReader.GetStringValue("LabelTypeIds");
                        discount.DiscountAmount = dataReader.GetDecimalValue("DiscountAmount");
                        discount.DiscountPct = dataReader.GetDecimalValue("DiscountPct");
                        discount.IsEcommerce = dataReader.GetBooleanValue("IsEcommerce");
                        discount.IsInStore = dataReader.GetBooleanValue("IsInStore");
                        discount.IsActive = dataReader.GetBooleanValue("IsActive");
                        discount.UpdatedAt = dataReader.GetDateTimeValue("UpdatedAt");
                        discount.UpdatedBy = dataReader.GetIntegerValue("UpdatedBy");
                        discount.CreatedAt = dataReader.GetDateTimeValue("CreatedAt");
                        discount.CreatedBy = dataReader.GetIntegerValue("CreatedBy");
                        result.Add(discount);
                    }
                }
            }

            return result;
        }
        #endregion
        #region Loyalty Card Type
        public async Task<int> AddLoyaltyCardType(LoyaltyCardType entity)
        {
            using var connection = base.GetConnection();

            var parameters = new DynamicParameters(); ;
            parameters.Add("p_Name", entity.Name);
            parameters.Add("p_ExpiryDuration", entity.ExpiryDuration);
            parameters.Add("p_ExpiryDurationType", entity.ExpiryDurationType);
            parameters.Add("p_PrimaryCategories", entity.PrimaryCategories);
            parameters.Add("p_CategoryIds", entity.CategoryIds);
            parameters.Add("p_SubCategoryIds", entity.SubCategoryIds);
            parameters.Add("p_CollectionTypeIds", entity.CollectionTypeIds);
            parameters.Add("p_LabelTypeIds", entity.LabelTypeIds);
            parameters.Add("p_DiscountAmount", entity.DiscountAmount);
            parameters.Add("p_DiscountPct", entity.DiscountPct);
            parameters.Add("p_CreatedBy", entity.CreatedBy);
            parameters.Add("o_LoyaltyCardTypeId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("AddLoyaltyCardTypeGb", parameters, commandType: CommandType.StoredProcedure);

            var discountId = parameters.Get<int>("o_LoyaltyCardTypeId");
            return discountId;
        }
        public async Task<bool> UpdateLoyaltyCardType(LoyaltyCardType entity)
        {
            using var connection = base.GetConnection();

            var parameters = new DynamicParameters();
            parameters.Add("p_Name", entity.Name);
            parameters.Add("p_ExpiryDuration", entity.ExpiryDuration);
            parameters.Add("p_ExpiryDurationType", entity.ExpiryDurationType);
            parameters.Add("p_LoyaltyCardTypeId", entity.LoyaltyCardTypeId);
            parameters.Add("p_PrimaryCategories", entity.PrimaryCategories);
            parameters.Add("p_CategoryIds", entity.CategoryIds);
            parameters.Add("p_SubCategoryIds", entity.SubCategoryIds);
            parameters.Add("p_CollectionTypeIds", entity.CollectionTypeIds);
            parameters.Add("p_LabelTypeIds", entity.LabelTypeIds);
            parameters.Add("p_DiscountAmount", entity.DiscountAmount);
            parameters.Add("p_DiscountPct", entity.DiscountPct);
            parameters.Add("p_CreatedBy", entity.UpdatedBy);
            parameters.Add("o_IsUpdated", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("UpdateLoyaltyCardTypeGb", parameters, commandType: CommandType.StoredProcedure);

            var updated = parameters.Get<int>("o_IsUpdated");
            return updated > 0;
        }
        public async Task<LoyaltyCardType> GetLoyaltyCardType(LoyaltyCardType LoyaltyCardType)
        {
            var discount = new LoyaltyCardType();
            var parameters = new List<DbParameter>
     {
          base.GetParameter("p_LoyaltyCardTypeId", LoyaltyCardType.LoyaltyCardTypeId),
     };
            using (var dataReader = await base.ExecuteReader(parameters, "GetLoyaltyCardTypeById_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        discount.Name = dataReader.GetStringValue("Name");                        
                        discount.ExpiryDuration = dataReader.GetIntegerValue("ExpiryDuration");
                        discount.ExpiryDurationType = dataReader.GetIntegerValue("ExpiryDurationType");
                        discount.LoyaltyCardTypeId = dataReader.GetIntegerValue("LoyaltyCardTypeId");
                        discount.PrimaryCategories = dataReader.GetStringValue("PrimaryCategories");
                        discount.CategoryIds = dataReader.GetStringValue("CategoryIds");
                        discount.SubCategoryIds = dataReader.GetStringValue("SubCategoryIds");
                        discount.CollectionTypeIds = dataReader.GetStringValue("CollectionTypeIds");
                        discount.LabelTypeIds = dataReader.GetStringValue("LabelTypeIds");
                        discount.DiscountAmount = dataReader.GetDecimalValue("DiscountAmount");
                        discount.DiscountPct = dataReader.GetDecimalValue("DiscountPct");
                        discount.IsActive = dataReader.GetBooleanValue("IsActive");
                        discount.UpdatedAt = dataReader.GetDateTimeValue("UpdatedAt");
                        discount.UpdatedBy = dataReader.GetIntegerValue("UpdatedBy");
                        discount.CreatedAt = dataReader.GetDateTimeValue("CreatedAt");
                        discount.CreatedBy = dataReader.GetIntegerValue("CreatedBy");
                    }
                }
            }

            return discount;
        }

        public async Task<List<LoyaltyCardType>> GetAllLoyaltyCardType(LoyaltyCardType LoyaltyCardType)
        {
            var result = new List<LoyaltyCardType>();
            var parameters = new List<DbParameter>
            {
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetAllLoyaltyCardType_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        var discount = new LoyaltyCardType();
                        discount.Name = dataReader.GetStringValue("Name");
                        discount.ExpiryDuration = dataReader.GetIntegerValue("ExpiryDuration");
                        discount.ExpiryDurationType = dataReader.GetIntegerValue("ExpiryDurationType");
                        discount.LoyaltyCardTypeId = dataReader.GetIntegerValue("LoyaltyCardTypeId");
                        discount.PrimaryCategories = dataReader.GetStringValue("PrimaryCategories");
                        discount.CategoryIds = dataReader.GetStringValue("CategoryIds");
                        discount.SubCategoryIds = dataReader.GetStringValue("SubCategoryIds");
                        discount.CollectionTypeIds = dataReader.GetStringValue("CollectionTypeIds");
                        discount.LabelTypeIds = dataReader.GetStringValue("LabelTypeIds");
                        discount.DiscountAmount = dataReader.GetDecimalValue("DiscountAmount");
                        discount.DiscountPct = dataReader.GetDecimalValue("DiscountPct");
                        discount.IsActive = dataReader.GetBooleanValue("IsActive");
                        discount.UpdatedAt = dataReader.GetDateTimeValue("UpdatedAt");
                        discount.UpdatedBy = dataReader.GetIntegerValue("UpdatedBy");
                        discount.CreatedAt = dataReader.GetDateTimeValue("CreatedAt");
                        discount.CreatedBy = dataReader.GetIntegerValue("CreatedBy");
                        result.Add(discount);
                    }
                }
            }

            return result;
        }
        #endregion

        #region Apply Discount
        public async Task<int> AddOrderDiscount(OrderDiscount entity)
        {
            using var connection = base.GetConnection();

            var parameters = new DynamicParameters();
            parameters.Add("p_OrderId", entity.OrderId, DbType.Int32);
            parameters.Add("p_DiscountId", entity.DiscountId, DbType.Int32);
            parameters.Add("p_CreatedBy", entity.CreatedBy, DbType.Int32);
            parameters.Add("o_OrderDiscountId", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("AddOrderDiscount_gb", parameters, commandType: CommandType.StoredProcedure);

            var OrderDiscountId = parameters.Get<int>("o_OrderDiscountId");
            return OrderDiscountId;
        }
        public async Task<DiscountCodeVerification> GetDiscountValidityByCode(OrderDiscount entity)
        {

            var res = new DiscountCodeVerification();
            var parameters = new List<DbParameter>
            {
                 base.GetParameter("p_DiscountCode", entity.Code),
                 base.GetParameter("p_DiscountTypeId", entity.DiscountTypeId)
            };

            using (var dataReader = await base.ExecuteReader(parameters, "GetDiscountDetailsByCode_gb", CommandType.StoredProcedure))
            {
                var Discount = new Discount();
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        var result = new DiscountCodeVerification();
                        result.Code = dataReader.GetStringValue("code");
                        result.IsValid = dataReader.GetBooleanValue("isValid");
                        result.status = dataReader.GetStringValue("status");
                        result.Description = dataReader.GetStringValue("description");
                        result.DiscountId = dataReader.GetIntegerValue("discountId");
                        Discount.DiscountId = result.DiscountId;
                        if(result.DiscountId > 0)
                        {
                            result.DiscountDetails = await this.Get(Discount);
                        }
                        else
                        {
                            result.DiscountDetails = null;
                        }
                        res = result;
                    }
                }
            }
            return res;
        }
        #endregion

        #region Summary
        public async Task<List<SaleSummary>> GetActiveSalesSummary(DiscountSummary summary)
        {
            var res = new List<SaleSummary>();
            var parameters = new List<DbParameter>
            {
                base.GetParameter("p_DiscountTypeId", summary.DiscountTypeId),
                base.GetParameter("p_DiscountId", summary.DiscountId),
                base.GetParameter("p_StoreIds", summary.StoreIds)
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetSaleSummaryById_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        var result = new SaleSummary();
                        result.DiscountId = dataReader.GetIntegerValue("discountId");
                        result.Name = dataReader.GetStringValue("name");
                        result.StockLevel = dataReader.GetIntegerValue("stockLevel");
                        result.UnitsSold = dataReader.GetIntegerValue("unitsSold");
                        result.Revenue = dataReader.GetDecimalValue("revenue");
                        result.WeightSold = dataReader.GetDecimalValue("weightSold");
                        result.PrimaryCategories = dataReader.GetStringValue("PrimaryCategories");
                        result.CategoryIds = dataReader.GetStringValue("CategoryIds");
                        result.SubCategoryIds = dataReader.GetStringValue("SubCategoryIds");
                        result.StartDate = dataReader.GetDateTimeValue("StartDate");
                        result.EndDate = dataReader.GetDateTimeValue("EndDate");
                        result.DiscountAmount = dataReader.GetDecimalValue("DiscountAmount");
                        result.DiscountPct = dataReader.GetDecimalValue("DiscountPct");

                        result.TotalUsage = dataReader.GetIntegerValue("totalUsage");
                        result.DiscountGiven = dataReader.GetDecimalValue("discountGiven");
                        result.RedeemedCount = dataReader.GetIntegerValue("RedeemedCount");
                        result.ComissionEarned = dataReader.GetDecimalValue("comissionEarned");
                        res.Add(result);
                    }
                }
            }
            return res;
        }
        public async Task<List<VoucherSummary>> GetVoucherSummary(DiscountSummary summary)
        {
            var res = new List<VoucherSummary>();
            var parameters = new List<DbParameter>
            {
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetVoucherSummary_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        var result = new VoucherSummary();
                        result.VoucherTypeId = dataReader.GetIntegerValue("VoucherTypeId");
                        result.Name = dataReader.GetStringValue("name");
                        result.RedeemedCount = dataReader.GetIntegerValue("Redeemed");
                        result.ActiveCount = dataReader.GetIntegerValue("active");
                        result.ExpiredCount = dataReader.GetIntegerValue("expired");
                        result.DeactivedCount = dataReader.GetIntegerValue("deactived");
                        res.Add(result);
                    }
                }
            }
            return res;
        }
        public async Task<List<LoyaltyCardSummary>> GetLoyaltyCardSummary(DiscountSummary summary)
        {
            var res = new List<LoyaltyCardSummary>();
            var parameters = new List<DbParameter>
            {
            };
            using (var dataReader = await base.ExecuteReader(parameters, "GetLoyaltyCardSummary_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null)
                {
                    while (dataReader.Read())
                    {
                        var result = new LoyaltyCardSummary();
                        result.LoyaltyCardTypeId = dataReader.GetIntegerValue("loyaltyCardTypeId");
                        result.Revenue = dataReader.GetDecimalValue("revenue");
                        result.WeightSold = dataReader.GetDecimalValue("weightSold");
                        result.ActiveCount = dataReader.GetIntegerValue("activeCount");
                        result.ExpiredCount = dataReader.GetIntegerValue("ExpiredCount");
                        result.UsedByCount = dataReader.GetIntegerValue("UsedByCount");
                        result.Name = dataReader.GetStringValue("name");
                        res.Add(result);
                    }
                }
            }
            return res;

        }
        #endregion
    }
}
