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
                            UnitPrice = dataReader.GetDecimalValue("unitPrice")
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
