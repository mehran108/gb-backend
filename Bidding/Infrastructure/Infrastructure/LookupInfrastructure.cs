using System.Data.Common;
using System.Data;
using GoldBank.Models;
using MySqlX.XDevAPI;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Infrastructure.Extension;
using Amazon.Runtime.Documents;
using GoldBank.Models.Product;
using Collection = GoldBank.Models.Product.Collection;
using Microsoft.AspNetCore.Mvc;
using GoldBank.Models.RequestModels;

namespace GoldBank.Infrastructure.Infrastructure
{
    public class LookupInfrastructure : BaseInfrastructure, ILookupInfrastructure
    {
        #region Constructor
        /// <summary>
        ///  Lookupfrastructure initializes class object.
        /// </summary>
        public LookupInfrastructure(IConfiguration configuration) : base(configuration)
        {
        }

        #endregion

        #region StoreProcedures
        private const string AddLookupValueProcedureName = "AddLookupValue";
        private const string GetLookupValueByCodeProcedureName = "GetLookupValueByCode";
        #endregion

        #region Constants
        private const string IdColumnName = "Id";
        private const string LookupTableIdColumnName = "LookupTableId";
        private const string LookupValueCodeColumnName = "LookupValueCode";
        private const string DescriptionColumnName = "Description";
        private const string ExtraColumnName = "Extra";

        private const string LookupTableIdParameterName = "p_lookupTableId";
        private const string LookupValueCodeParameterName = "p_lookupValueCode";
        private const string DescriptionParameterName = "p_description";
        private const string UserIdParameterName = "p_userid";
        private const string ExtraParameterName = "p_extra";
        private const string IdParameterName = "p_id";

        #endregion

        public async Task<int> Add(LookupValue entity)
        {
            var Id = GetParameterOut(LookupInfrastructure.IdParameterName, SqlDbType.Int, entity.Id);
            var parameters = new List<DbParameter>
            {
                Id,
                GetParameter(LookupTableIdParameterName, entity.LookupTableId),
                GetParameter(LookupValueCodeParameterName, entity.LookupValueCode),
                GetParameter(DescriptionParameterName, entity.Description),
                GetParameter(ExtraParameterName, entity.Extra),
                GetParameter(UserIdParameterName, entity.CurrentUserId)

            };
            //TODO: Add other parameters.

            await ExecuteNonQuery(parameters, AddLookupValueProcedureName, CommandType.StoredProcedure);

            entity.Id = Convert.ToInt32(Id.Value);

            return entity.Id;
        }
        public async Task<List<LookupValue>> GetLookupByCode(LookupValue entity)
        {
            var parameters = new List<DbParameter>
            {
                GetParameter(LookupValueCodeParameterName, entity.LookupValueCode)
            };
            List<LookupValue> lookupList = new List<LookupValue>();
            var item = new LookupValue();
            using (var dataReader = await ExecuteReader(parameters, GetLookupValueByCodeProcedureName, CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    if (dataReader.Read())
                    {
                        item = new LookupValue
                        {
                            Id = dataReader.GetIntegerValue(LookupInfrastructure.IdColumnName),
                            LookupTableId = dataReader.GetIntegerValue(LookupInfrastructure.LookupTableIdColumnName),
                            LookupValueCode = dataReader.GetStringValue(LookupInfrastructure.LookupValueCodeColumnName),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName),
                            Extra = dataReader.GetStringValue(LookupInfrastructure.ExtraColumnName)
                        };

                        lookupList.Add(item);
                    }
                }
            }
            return lookupList;
        }

        public async Task<IEnumerable<ProductType>> GetAllProductTypeGbAsync()
        {
            var list = new List<ProductType>();
            using (var dataReader = await ExecuteReader(null, "GetAllProductTypeGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new ProductType
                        {
                            ProductTypeId = dataReader.GetIntegerValue("ProductTypeId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName)
                        });
                    }
                }
            }
            return list;
        }

        public async Task<IEnumerable<ProductSource>> GetAllProductSourceGbAsync()
        {
            var list = new List<ProductSource>();
            using (var dataReader = await ExecuteReader(null, "GetAllProductSourceGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new ProductSource
                        {
                            ProductSourceId = dataReader.GetIntegerValue("ProductSourceId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName)
                        });
                    }
                }
            }
            return list;
        }

        public async Task<IEnumerable<Vendor>> GetAllVendorGbAsync()
        {
            var list = new List<Vendor>();
            using (var dataReader = await ExecuteReader(null, "GetAllVendorGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new Vendor
                        {
                            VendorId = dataReader.GetIntegerValue("VendorId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName)
                        });
                    }
                }
            }
            return list;
        }

        public async Task<IEnumerable<MetalType>> GetAllMetalTypeGbAsync()
        {
            var list = new List<MetalType>();
            using (var dataReader = await ExecuteReader(null, "GetAllMetalTypeGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new MetalType
                        {
                            MetalTypeId = dataReader.GetIntegerValue("MetalTypeId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName)
                        });
                    }
                }
            }
            return list;
        }

        public async Task<IEnumerable<MetalPurity>> GetAllMetalPurityGbAsync()
        {
            var list = new List<MetalPurity>();
            using (var dataReader = await ExecuteReader(null, "GetAllMetalPurityGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new MetalPurity
                        {
                            MetalPurityId = dataReader.GetIntegerValue("MetalPurityId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName),
                            UnitPrice = dataReader.GetDecimalValue("unitPrice"),
                            MetalTypeId = dataReader.GetIntegerValue("metalTypeId"),
                            PurityPercentage = dataReader.GetDecimalValue("purityPercentage"),
                            StoreId = dataReader.GetIntegerValue("storeId"),
                            CreatedAt = dataReader.GetDateTimeValue("createdAt"),
                            UpdatedAt = dataReader.GetDateTimeValue("updatedAt")
                        });
                    }
                }
            }
            return list;
        }

        public async Task<IEnumerable<MetalColor>> GetAllMetalColorGbAsync()
        {
            var list = new List<MetalColor>();
            using (var dataReader = await ExecuteReader(null, "GetAllMetalColorGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new MetalColor
                        {
                            MetalColorId = dataReader.GetIntegerValue("MetalColorId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName)
                        });
                    }
                }
            }
            return list;
        }

        public async Task<IEnumerable<WeightType>> GetAllWeightTypeGbAsync()
        {
            var list = new List<WeightType>();
            using (var dataReader = await ExecuteReader(null, "GetAllWeightTypeGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new WeightType
                        {
                            WeightTypeId = dataReader.GetIntegerValue("WeightTypeId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName)
                        });
                    }
                }
            }
            return list;
        }

        public async Task<IEnumerable<StoneType>> GetAllStoneTypeGbAsync()
        {
            var list = new List<StoneType>();
            using (var dataReader = await ExecuteReader(null, "GetAllStoneTypeGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new StoneType
                        {
                            StoneTypeId = dataReader.GetIntegerValue("StoneTypeId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName)
                        });
                    }
                }
            }
            return list;
        }

        public async Task<IEnumerable<StoneWeightType>> GetAllStoneWeightTypeGbAsync()
        {
            var list = new List<StoneWeightType>();
            using (var dataReader = await ExecuteReader(null, "GetAllStoneWeightTypeGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new StoneWeightType
                        {
                            StoneWeightTypeId = dataReader.GetIntegerValue("StoneWeightTypeId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName)
                        });
                    }
                }
            }
            return list;
        }

        public async Task<IEnumerable<StoneShape>> GetAllStoneShapeGbAsync()
        {
            var list = new List<StoneShape>();
            using (var dataReader = await ExecuteReader(null, "GetAllStoneShapeGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new StoneShape
                        {
                            StoneShapeId = dataReader.GetIntegerValue("StoneShapeId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName)
                        });
                    }
                }
            }
            return list;
        }

        public async Task<IEnumerable<WearingType>> GetAllWearingTypeGbAsync()
        {
            var list = new List<WearingType>();
            using (var dataReader = await ExecuteReader(null, "GetAllWearingTypeGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new WearingType
                        {
                            WearingTypeId = dataReader.GetIntegerValue("WearingTypeId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName)
                        });
                    }
                }
            }
            return list;
        }

        public async Task<IEnumerable<Collection>> GetAllCollectionGbAsync()
        {
            var list = new List<Collection>();
            using (var dataReader = await ExecuteReader(null, "GetAllCollectionGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new Collection
                        {
                            CollectionId = dataReader.GetIntegerValue("CollectionId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName)
                        });
                    }
                }
            }
            return list;
        }

        public async Task<IEnumerable<Occasion>> GetAllOccasionGbAsync()
        {
            var list = new List<Occasion>();
            using (var dataReader = await ExecuteReader(null, "GetAllOccasionGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new Occasion
                        {
                            OccasionId = dataReader.GetIntegerValue("OccasionId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName)
                        });
                    }
                }
            }
            return list;
        }

        public async Task<IEnumerable<GenderType>> GetAllGenderTypeGbAsync()
        {
            var list = new List<GenderType>();
            using (var dataReader = await ExecuteReader(null, "GetAllGenderTypeGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new GenderType
                        {
                            GenderTypeId = dataReader.GetIntegerValue("GenderTypeId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName)
                        });
                    }
                }
            }
            return list;
        }
        public async Task<IEnumerable<PrimaryCategory>> GetPrimaryCategories()
        {
            var list = new List<PrimaryCategory>();
            using (var dataReader = await ExecuteReader(null, "GetAllPrimaryCategoryGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new PrimaryCategory
                        {
                            PrimarycategoryId = dataReader.GetIntegerValue("PrimarycategoryId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName)
                        });
                    }
                }
            }
            return list;
        }
        public async Task<IEnumerable<Category>> GetCategories()
        {
            var list = new List<Category>();
            using (var dataReader = await ExecuteReader(null, "GetAllCategoryGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new Category
                        {
                            CategoryId = dataReader.GetIntegerValue("CategoryId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName),
                            ImageUrl = dataReader.GetStringValue("Url")
                        });
                    }
                }
            }
            return list;
        }
        public async Task<IEnumerable<SubCategory>> GetSubCategories()
        {
            var list = new List<SubCategory>();
            using (var dataReader = await ExecuteReader(null, "GetAllSubCategoryGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new SubCategory
                        {
                            SubCategoryId = dataReader.GetIntegerValue("SubCategoryId"),
                            CategoryId = dataReader.GetIntegerValue("CategoryId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName)
                        });
                    }
                }
            }
            return list;
        }
        public async Task<IEnumerable<Store>> GetAllStores()
        {
            var list = new List<Store>();
            using (var dataReader = await ExecuteReader(null, "GetAllStoresGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new Store
                        {
                            StoreId = dataReader.GetIntegerValue("StoreId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName)
                        });
                    }
                }
            }
            return list;
        }
        public async Task<IEnumerable<OrderType>> GetAllOrderTypes()
        {
            var list = new List<OrderType>();
            using (var dataReader = await ExecuteReader(null, "GetAllOrderTypesGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new OrderType
                        {
                            OrderTypeId = dataReader.GetIntegerValue("orderTypeId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName)
                        });
                    }
                }
            }
            return list;
        }
        public async Task<IEnumerable<DelieveryMethod>> GetAllDeliveryMethods()
        {
            var list = new List<DelieveryMethod>();
            using (var dataReader = await ExecuteReader(null, "GetAllDelieveryMethodGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new DelieveryMethod
                        {
                            DelieveryMethodId = dataReader.GetIntegerValue("deliveryMethodId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName)
                        });
                    }
                }
            }
            return list;
        }
        public async Task<IEnumerable<PaymentType>> GetAllPaymentType()
        {
            var list = new List<PaymentType>();
            using (var dataReader = await ExecuteReader(null, "GetAllPaymentTypeGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new PaymentType
                        {
                            PaymentTypeId = dataReader.GetIntegerValue("paymentTypeId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName)
                        });
                    }
                }
            }
            return list;
        }
        public async Task<IEnumerable<OrderStatus>> GetAllOrderStatus()
        {
            var list = new List<OrderStatus>();
            using (var dataReader = await ExecuteReader(null, "GetAllOrderStatus", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new OrderStatus
                        {
                            OrderStatusId = dataReader.GetIntegerValue("orderStatusId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName),
                            PublicDescription = dataReader.GetStringValue("publicDescription"),
                            OrderTypeId = dataReader.GetIntegerValue("orderTypeId"),
                            StatusClass = dataReader.GetStringValue("statusClass")
                        });
                    }
                }
            }
            return list;
        }
        public async Task<IEnumerable<CustomerAccount>> GetAllCustomerAccounts()
        {
            var list = new List<CustomerAccount>();
            using (var dataReader = await ExecuteReader(null, "GetAllCustomerAccounts_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new CustomerAccount
                        {
                            CustomerAccountId = dataReader.GetIntegerValue("CustomerAccountId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName),
                        });
                    }
                }
            }
            return list;
        }
        public async Task<IEnumerable<CompanyAccount>> GetAllCompanyAccounts()
        {
            var list = new List<CompanyAccount>();
            using (var dataReader = await ExecuteReader(null, "GetAllCompanyAccounts_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new CompanyAccount
                        {
                            CompanyAccountId = dataReader.GetIntegerValue("CompanyAccountId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName),
                        });
                    }
                }
            }
            return list;
        }

        public async Task<IEnumerable<LacquerType>> GetAllLacquerTypes()
        {
            var list = new List<LacquerType>();
            using (var dataReader = await ExecuteReader(null, "GetAllLacquerTypes_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new LacquerType
                        {
                            LacquerTypeId = dataReader.GetIntegerValue("lacquerTypeId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName),
                        });
                    }
                }
            }
            return list;
        }

        public async Task<IEnumerable<RepairDamageArea>> GetAllRepairDamageAreas()
        {
            var list = new List<RepairDamageArea>();
            using (var dataReader = await ExecuteReader(null, "GetAllRepairDamageAreaGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new RepairDamageArea
                        {
                            RepairDamageAreaId = dataReader.GetIntegerValue("repairDamageAreaId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName),
                        });
                    }
                }
            }
            return list;
        }
        public async Task<IEnumerable<RepairDamageType>> GetAllRepairDamageTypes()
        {
            var list = new List<RepairDamageType>();
            using (var dataReader = await ExecuteReader(null, "GetAllRepairDamageTypesGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new RepairDamageType
                        {
                            RepairDamageTypeId = dataReader.GetIntegerValue("repairDamageTypeId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName),
                        });
                    }
                }
            }
            return list;
        }
        public async Task<IEnumerable<RepairPolishing>> GetAllRepairPolishing()
        {
            var list = new List<RepairPolishing>();
            using (var dataReader = await ExecuteReader(null, "GetAllRepairPolishingGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new RepairPolishing
                        {
                            RepairPolishingId = dataReader.GetIntegerValue("repairPolishingId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName),
                        });
                    }
                }
            }
            return list;
        }
        public async Task<IEnumerable<RepairCleaning>> GetAllRepairCleaning()
        {
            var list = new List<RepairCleaning>();
            using (var dataReader = await ExecuteReader(null, "GetAllRepairCleaningGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new RepairCleaning
                        {
                            RepairCleaningId = dataReader.GetIntegerValue("repairCleaningId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName),
                        });
                    }
                }
            }
            return list;
        }
        public async Task<IEnumerable<DiscountType>> GetAllDiscountType()
        {
            var list = new List<DiscountType>();
            using (var dataReader = await ExecuteReader(null, "GetAllDiscountTypeGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new DiscountType
                        {
                            DiscountTypeId = dataReader.GetIntegerValue("discountTypeId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName),
                        });
                    }
                }
            }
            return list;
        }
        public async Task<IEnumerable<ExpiryDurationType>> GetAllExpiryDuration()
        {
            var list = new List<ExpiryDurationType>();
            using (var dataReader = await ExecuteReader(null, "GetAllExpiryDurationGb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new ExpiryDurationType
                        {
                            ExpiryDurationTypeId = dataReader.GetIntegerValue("expiryDurationTypeId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName),
                        });
                    }
                }
            }
            return list;
        }
        public async Task<IEnumerable<VendorPaymentType>> GetAllVendorPaymentTypes()
        {
            var list = new List<VendorPaymentType>();
            using (var dataReader = await ExecuteReader(null, "GetAllVendorPaymentType_Gb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new VendorPaymentType
                        {
                            VendorPaymentTypeId = dataReader.GetIntegerValue("vendorPaymentTypeId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName),
                        });
                    }
                }
            }
            return list;
        }
        public async Task<IEnumerable<VendorGoldPaymentType>> GetAllVendorGoldPaymentTypes()
        {
            var list = new List<VendorGoldPaymentType>();
            using (var dataReader = await ExecuteReader(null, "GetAllVendorGoldPaymentType_gb", CommandType.StoredProcedure))
            {
                if (dataReader != null && dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        list.Add(new VendorGoldPaymentType
                        {
                            VendorGoldPaymentTypeId = dataReader.GetIntegerValue("vendorGoldPaymentTypeId"),
                            Description = dataReader.GetStringValue(LookupInfrastructure.DescriptionColumnName),
                        });
                    }
                }
            }
            return list;
        }

        public Task<bool> Update(LookupValue lookupValue)
        {
            throw new NotImplementedException();
        }
        public Task<bool> Activate(LookupValue lookupValue)
        {
            throw new NotImplementedException();
        }
        public Task<List<LookupValue>> GetList(LookupValue entity)
        {
            throw new NotImplementedException();
        }
        public Task<LookupValue> Get(LookupValue entity)
        {
            throw new NotImplementedException();
        }
    }
}
