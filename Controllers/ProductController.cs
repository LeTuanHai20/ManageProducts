using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
using NG_Core_Auth.Data;
using NG_Core_Auth.Models;

namespace NG_Core_Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            this._db = db;
        }
        // GET: api/Product
        [HttpGet("[action]")]
        [Authorize(Policy = "RequireLoggedIn")]
        public IActionResult GetProducts()
        {
            return Ok(_db.Products.ToList());
        }

       

        // POST: api/Product
        [Authorize(Policy = "AdministratorRole")]
        [HttpPost("[action]")]
        public async Task<IActionResult> AddProduct([FromBody] Product formdata)
        {
            Product product = new Product()
            {
                Name = formdata.Name,
                Description = formdata.Description,
                OutofStock = formdata.OutofStock,
                Price = formdata.Price
            };
            await _db.AddAsync(product);
            await _db.SaveChangesAsync();
            return Ok();
        }
        //POST:api/Product/UpdateProduct/5
        [Authorize(Policy = "AdministratorRole")]
        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateProduct([FromRoute]int id, [FromBody] Product formdata)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var product = await _db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            product.Name = formdata.Name;
            product.Description = formdata.Description;
            product.OutofStock = formdata.OutofStock;
            product.Price = formdata.Price;
            _db.Products.Update(product);
            await _db.SaveChangesAsync();
            return Ok(new JsonResult("product with id : " + id + "is updated success"));
        }

        // DELETE: api/Product/Delete/5
        [Authorize(Policy = "AdministratorRole")]
        [HttpDelete("[action]")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var product = await _db.Products.FindAsync(id);
            if(product == null)
            {
                return NotFound();
            }
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
            return Ok(new JsonResult("the product has id : " + id + " is deleted"));
        }
    }
}
