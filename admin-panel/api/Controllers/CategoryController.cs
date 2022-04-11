using Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Api.Controllers
{
    [ApiController]
    [Route("/category/")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _service;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(CategoryService service, ILogger<CategoryController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("all")]
        public List<Category> GetAll()
        {
            _logger.LogDebug("Request: GET /category/list");
            return _service.GetAll();
        }

        [HttpGet("list")]
        public List<Category> GetList([FromQuery] int parent)
        {
            _logger.LogDebug("Request: GET /category/list?parent=" + parent);
            if (parent == 0)
                return _service.GetTop();
            else
                return _service.GetChildren(parent);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {
            _logger.LogDebug("Request: GET /category/" + id);
            var result = _service.GetById(id);
            if (result == null)
                return NotFound();
            else
                return Ok(result);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Category category)
        {
            _logger.LogDebug("Request: POST /category");
            category = _service.Create(category);

            if (category == null)
                return BadRequest("invalid arguments");
            else 
                return Ok(category.Id);
        }

        [HttpPut]
        public IActionResult Update([FromBody] Category category)
        {
            _logger.LogDebug("Request: PUT /category");
            category = _service.Update(category);

            if (category == null)
                return BadRequest("invalid arguments");
            else
                return Ok(category);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete([FromRoute] int id)
        {
            _logger.LogDebug("Request: DELETE /category/" + id);
            _service.Delete(id);

            return Ok();
        }
    }
}
