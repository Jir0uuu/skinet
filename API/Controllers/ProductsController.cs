using System;
using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


public class ProductsController(IGenericRepository<Product> repository) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts([FromQuery]ProductSpecParams specParams)
    {
        //return Ok(await repository.GetProductAsync(brand, type, sort));

        var spec = new ProductSpecification(specParams);

        return await CreatePagedResult(repository, spec, specParams.PageIndex, specParams.PageSize);
    }
    
    [HttpGet("{id:int}")] // api/products/2
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        // var product = await repository.GetProductByIdAsync(id);
        var product = await repository.GetByIdAsync(id);

        if(product == null)
        {
            return NotFound();
        }

        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        // repository.AddProdcut(product);

        repository.Add(product);
        
        if(await repository.SaveAllAsync())
        {
            //Go to the headers in postman and see location this is the value of CreatedAtAction
            return CreatedAtAction("GetProduct", new {id = product.Id}, product);
        }

        return BadRequest("Problem creating a product");
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> UpdateProduct(int id, Product product)
    {
        if(product.Id != id || !ProductExists(id)) 
            return BadRequest("Cannot Update this product");
        
        repository.Update(product);

        if(await repository.SaveAllAsync())
        {
            return NoContent();
        }

        return BadRequest("Problem updating a product");

    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var product = await repository.GetByIdAsync(id);
        
        if(product == null)
        {
            return NotFound();
        }

        repository.Remove(product);

        if(await repository.SaveAllAsync())
        {
            return NoContent();
        }

        return BadRequest("Problem deleting a product");
    }

    [HttpGet("brands")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
    {
        var spec = new BrandListSpecification();

        return Ok(await repository.ListAsync(spec));
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
    {
        var spec = new TypeListSpecification();

        return Ok(await repository.ListAsync(spec));
    }

    private bool ProductExists(int id)
    {
        return repository.Exists(id);
    }
}
