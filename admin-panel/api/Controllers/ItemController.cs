using Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Api.Controllers
{
    [ApiController]
    [Route("/item/")]
    public class ItemController : ControllerBase
    {
        private readonly ItemService _service;
        private readonly ILogger<ItemController> _logger;

        public ItemController(ItemService service, ILogger<ItemController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("list")]
        public List<Item> GetList([FromQuery] int category)
        {
            _logger.LogDebug("Request: GET /item/list?category=" + category);
            if (category == 0)
                return _service.GetAll();
            else
                return _service.GetByCategory(category);
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById([FromRoute] int id)
        {
            _logger.LogDebug("Request: GET /item/" + id);
            var result = _service.GetById(id);
            if (result == null)
                return NotFound();
            else
                return Ok(result);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Item item)
        {
            _logger.LogDebug("Request: POST /item");
            item = _service.Create(item);

            if (item == null)
                return BadRequest();
            else
                return Ok(item.Id);
        }

        [HttpPut]
        public IActionResult Update([FromBody] Item item)
        {
            _logger.LogDebug("Request: PUT /item");
            item = _service.Update(item);

            if (item == null)
                return BadRequest();
            else
                return Ok(item);
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete([FromRoute] int id)
        {
            _logger.LogDebug("Request: DELETE /item/" + id);
            _service.Delete(id);

            return Ok();
        }
    }
}
