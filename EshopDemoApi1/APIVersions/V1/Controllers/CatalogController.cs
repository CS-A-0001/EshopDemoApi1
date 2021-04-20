using AutoMapper;
using CatalogDemo.API.Infrastructure;
using CatalogDemo.API.Versions.V1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace CatalogDemo.API.Versions.V1.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
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

        [HttpGet]
        [Route("items")]        
        [ProducesResponseType(typeof(IEnumerable<ProductDTO>), (int)HttpStatusCode.OK)]
        public virtual async Task<IActionResult> ItemsAsync()
        {
            var items = await _catalogContext.Products.ToListAsync();
            return Ok(_mapper.Map<ProductDTO[]>(items));
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
    }
}
