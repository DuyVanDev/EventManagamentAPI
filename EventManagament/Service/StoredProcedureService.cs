using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;

namespace EventManagament.Services
{
    public class StoredProcedureService
    {
        private readonly string _connectionString;

        public StoredProcedureService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<object> ExecuteStoredProcedureAsync(string procedureName, Dictionary<string, object> inputParameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var dynamicParameters = new DynamicParameters();

                // Add input parameters
                if (inputParameters != null)
                {
                    foreach (var param in inputParameters)
                    {
                        // Convert JsonElement to string if needed
                        if (param.Value is System.Text.Json.JsonElement jsonElement)
                        {
                            dynamicParameters.Add(param.Key, jsonElement.ToString());
                        }
                        else
                        {
                            dynamicParameters.Add(param.Key, param.Value);
                        }
                    }
                }

                // Automatically add ReturnMess output parameter
                dynamicParameters.Add("@ReturnMess", dbType: DbType.String, direction: ParameterDirection.Output, size: 4000);

                // Execute the stored procedure
                await connection.ExecuteAsync(procedureName, dynamicParameters, commandType: CommandType.StoredProcedure);

                // Retrieve the ReturnMess output parameter
                var returnMess = dynamicParameters.Get<string>("@ReturnMess");

                // Return the result including ReturnMess
                return new
                {
                    ReturnMess = returnMess
                };
            }
        }
    }
}
