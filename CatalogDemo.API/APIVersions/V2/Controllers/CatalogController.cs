using AutoMapper;
using CatalogDemo.API.Infrastructure;
using CatalogDemo.API.Versions.V2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CatalogDemo.API.Versions.V2.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    public class CatalogController : ControllerBase
    {
        readonly CatalogContext _catalogContext;
        readonly IMapper _mapper; 

        public CatalogController(CatalogContext catalogContext, IMapper mapper)
        {
            _catalogContext = catalogContext ?? throw new ArgumentNullException(nameof(catalogContext));
            _mapper = mapper;
        }

        [HttpGet]
        [Route("items/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ProductDTO), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ProductDTO>> ItemByIdAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var item = await _catalogContext.Products.SingleOrDefaultAsync(ci => ci.Id == id);

            if (item != null)
            {
                return _mapper.Map<ProductDTO>(item);
            }

            return NotFound();
        }       

        [HttpPut]
        [Route("items/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ActionResult> UpdateProductAsync(int id, [FromBody] string description)
        {
            var product = await _catalogContext.Products.SingleOrDefaultAsync(i => i.Id == id);

            if (product == null)
            {
                return NotFound(new { Message = $"Item with id {id} not found." });
            }

            product.Description = description;
            _catalogContext.Products.Update(product);

            await _catalogContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet]
        [Route("items")]        
        [ProducesResponseType(typeof(PaginatedItems<ProductDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<ProductDTO>), (int)HttpStatusCode.OK)]      
        public async Task<IActionResult> ItemsAsync([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {          
            var totalItems = await _catalogContext.Products
                .LongCountAsync();

            var itemsOnPage = await _catalogContext.Products
                .OrderBy(c => c.Name)
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToListAsync();
                        
            var itemDtos = _mapper.Map<ProductDTO[]>(itemsOnPage);
            var model = new PaginatedItems<ProductDTO>(pageIndex, pageSize, totalItems, itemDtos);

            return Ok(model);
        }    
    }
}
