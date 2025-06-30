using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Bidding.Infrastructure
{
    public abstract class BaseInfrastructure
    {
        #region Constructor
        /// <summary>
        /// BaseInfrastructure initializes class object.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        protected BaseInfrastructure(IConfiguration configuration)
        {
            this.Configuration = configuration;
            this.ConnectionString = this.Configuration["General:DefaultConnection"];            
        }
        #endregion

        #region Properties and Data Members
        protected const string CreatedByIdColumnName = "CreatedById";
        protected const string CreatedByNameColumnName = "CreatedByName";
        protected const string CreatedDateColumnName = "CreatedDate";
        protected const string ModifiedByIdColumnName = "ModifiedById";
        protected const string ModifiedByNameColumnName = "ModifiedByName";
        protected const string ModifiedDateColumnName = "ModifiedDate";
        protected const string ActiveColumnName = "Active";
        protected const string IsDeletedColumnName = "IsDeleted";
        protected const string ActiveForCustomerColumnName = "ActiveForCustomer";
        protected const string UnReadCountColumnName = "UnReadCount";

        protected const string CurrentUserIdParameterName = "@PCurrentUserId";
        protected const string AgentIdParameterName = "PAgentId";

        protected const string IsActiveParameterName = "PActive";
        protected const string OffsetParameterName = "POffset";
        protected const string PageSizeParameterName = "PPageSize";
        protected const string TotalRecordParameterName = "PTotalRecord";
        protected const string SortColumnParameterName = "PSortColumn";
        protected const string SortAscendingParameterName = "PSortAscending";
        protected const string FilterColumnIdParameterName = "PFilterColumnId";
        protected const string ActiveParameterName = "PActive";
        protected const string ActiveForCustomerParameterName = "PActiveForCustomer";
        protected const string NotificationCodeParamaterName = "PNotificationCode";
        protected const string SearchTextParameterName = "PSearchText";
        protected const string CheckSFCFlag = "PCheckSFCFlag";
        protected const string EmailAddress = "Email";

        #endregion

        /// <summary>
        /// ConnectionString holds connection string for dataBaseSQL.
        /// </summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// Configuration holds configurations and settings.
        /// </summary>
        protected IConfiguration Configuration { get; }

        #region Private Methods
        /// <summary>
        /// GetException format exception details.
        /// </summary>
        /// <param name="className"></param>
        /// <param name="methodName"></param>
        /// <param name="exception"></param>
        /// <param name="parameters"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        private DatabaseException GetException(string className, string methodName, Exception exception, List<DbParameter> parameters, string commandText, CommandType commandType)
        {
            var message = $"Failed in {className}.{methodName}. {exception.Message}";
            var dbException = new DatabaseException(message, exception, parameters, commandText, commandType);

            return dbException;
        }
        #endregion

        #region DataBaseSQL Operations
        /// <summary>
        /// GetConnection initializes and return connection fro given connection string.
        /// </summary>
        /// <returns></returns>
        private DbConnection GetConnection()
        {
            
            DbConnection connection = new SqlConnection(this.ConnectionString);
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            return connection;
        }

        /// <summary>
        /// GetCommand initializes command object for given connection.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private DbCommand GetCommand(DbConnection connection, string commandText, CommandType commandType, List<DbParameter> parameters)
        {
            var command = connection.CreateCommand();

            command.CommandText = commandText;
            command.CommandType = commandType;

            if (parameters != null && parameters.Count > 0)
            {
                command.Parameters.AddRange(parameters.ToArray());
            }

            return command;
        }

        /// <summary>
        /// GetParameter initializes parameter for provided data.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        protected DbParameter GetParameter(string parameterName, object parameterValue)
        {
            DbParameter parameterObject = new SqlParameter(parameterName, parameterValue ?? DBNull.Value);

            parameterObject.Direction = ParameterDirection.Input;

            return parameterObject;
        }

        /// <summary>
        /// GetParameter initializes parameter of out type for provided data.
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="type"></param>
        /// <param name="parameterValue"></param>
        /// <param name="parameterDirection"></param>
        /// <returns></returns>
        protected DbParameter GetParameterOut(string parameterName, SqlDbType type, object parameterValue = null, ParameterDirection parameterDirection = ParameterDirection.InputOutput)
        {
            DbParameter parameterObject = new SqlParameter(parameterName, type);

            if (type == SqlDbType.NVarChar ||
                type == SqlDbType.VarChar ||
                type == SqlDbType.NText ||
                type == SqlDbType.Text)
            {
                parameterObject.Size = -1;
            }

            parameterObject.Direction = parameterDirection;

            parameterObject.Value = parameterValue ?? DBNull.Value;

            return parameterObject;
        }
        #endregion

        #region Data Operations
        /// <summary>
        /// Executes a SQL statement against a connection object and returns the number of rows affected.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected async Task<int> ExecuteNonQuery(List<DbParameter> parameters, string commandText, CommandType commandType = CommandType.StoredProcedure)
        {
            var returnValue = -1;

            try
            {
                using (var connection = this.GetConnection())
                {
                    var cmd = this.GetCommand(connection, commandText, commandType, parameters);

                    cmd.CommandTimeout = 0;
                    returnValue = await cmd.ExecuteNonQueryAsync();

                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var dbException = this.GetException(this.GetType().FullName, "ExecuteNonQuery", ex, parameters, commandText, commandType);
                //LogException(dbException);
                throw dbException;
            }

            return returnValue;
        }

        /// <summary>
        /// Executes the query and returns the first column of the first row in the result set returned by the query. All other
        /// columns and rows are ignored. And returns the first column of the first row in the result set.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected async Task<object> ExecuteScalar(List<DbParameter> parameters, string commandText, CommandType commandType = CommandType.StoredProcedure)
        {
            object returnValue = null;

            try
            {
                using (var connection = this.GetConnection())
                {
                    var cmd = this.GetCommand(connection, commandText, commandType, parameters);

                    returnValue = await cmd.ExecuteScalarAsync();
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var dbException = this.GetException(this.GetType().FullName, "ExecuteScalar", ex, parameters, commandText, commandType);
                //LogException(dbException);
                throw dbException;
            }

            return returnValue;
        }

        /// <summary>
        /// Executes the System.Data.Common.DbCommand.CommandText against the System.Data.Common.DbCommand.Connection, and
        /// returns an System.Data.Common.DbDataReader using one of the System.Data.CommandBehavior values.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        protected async Task<DbDataReader> ExecuteReader(List<DbParameter> parameters, string commandText, CommandType commandType = CommandType.StoredProcedure)
        {
            DbDataReader ds;

            try
            {
                //using (var connection = this.GetConnection())
                //{

                var connection = this.GetConnection();
                var cmd = this.GetCommand(connection, commandText, commandType, parameters);
                cmd.CommandTimeout = 0;
                ds = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                //}
            }
            catch (Exception ex)
            {
                var dbException = this.GetException(this.GetType().FullName, "ExecuteReader", ex, parameters, commandText, commandType);
                //LogException(dbException);
                throw dbException;
            }

            return ds;
        }


        #endregion
    }
}
