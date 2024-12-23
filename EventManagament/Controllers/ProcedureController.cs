using EventManagament.Models;
using EventManagament.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json; // Thêm dòng này nếu chưa có
using System.Collections.Generic;
using System.Text;
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
        public async Task<IActionResult> ExecuteProcedureStream([FromBody] StoredProcedureRequest request)
        {
            var result = await _spService.ExecuteStoredProcedureAsync(request.ProcedureName, request.InputParameters);

            if (result is string json)
            {
                var jsonBytes = Encoding.UTF8.GetBytes(json);
                return new FileContentResult(jsonBytes, "application/json");
            }

            return Ok(result);
        }
    }
}
