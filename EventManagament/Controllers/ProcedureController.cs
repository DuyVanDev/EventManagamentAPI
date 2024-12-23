using EventManagament.Models;
using EventManagament.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json; // Thêm dòng này nếu chưa có
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventManagament.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoredProcedureController : ControllerBase
    {
        private readonly StoredProcedureService _spService;

        public StoredProcedureController(StoredProcedureService spService)
        {
            _spService = spService;
        }

        [HttpPost("execute")]
        public async Task<IActionResult> ExecuteProcedure([FromBody] StoredProcedureRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.ProcedureName))
            {
                return BadRequest("Invalid request");
            }

            // Call the service method, passing only input parameters
            var result = await _spService.ExecuteStoredProcedureAsync(
                request.ProcedureName,
                request.InputParameters
            );

            // Check if the result is a Dictionary
            if (result is IDictionary<string, object> resultDict && resultDict.ContainsKey("ReturnMess"))
            {
                var returnMess = resultDict["ReturnMess"]?.ToString();

                if (!string.IsNullOrEmpty(returnMess))
                {
                    try
                    {
                        // Try to parse ReturnMess as JSON
                        var parsedReturnMess = JsonConvert.DeserializeObject(returnMess);
                        return Ok(parsedReturnMess);
                    }
                    catch (JsonException)
                    {
                        // If parsing fails, return the raw ReturnMess
                        return Ok(new { ReturnMess = returnMess });
                    }
                }
            }

            // If result is not a dictionary or ReturnMess is missing, return the result as is
            return Ok(result);
        }
    }
}