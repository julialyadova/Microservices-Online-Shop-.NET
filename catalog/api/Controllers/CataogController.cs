using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [ApiController]
    [Route("/")]
    public class CataogController : ControllerBase
    {
        private readonly CatalogService _service;
        private readonly ILogger<CataogController> _logger;

        public CataogController(CatalogService service, ILogger<CataogController> logger)
        {
            _service = service;
            _logger = logger;
        }

        public IActionResult GetById([FromQuery] int category)
        {
            _logger.LogDebug("Request: GET category = " + category);
            var result = _service.GetCategoryWithItems(category);
            if (result == null)
                return NotFound();
            else
                return Ok(result);
        }
    }
}
