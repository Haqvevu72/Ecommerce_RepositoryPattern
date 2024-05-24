using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities.Concretes;
using ECommerce.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.Json;
using System.Text.Json;

namespace ECommerce.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IReadProductRepository _readProductRepo;
    private readonly IWriteProductRepository _writeProductRepo;
    private readonly IReadCategoryRepository _readCategoryRepo;

    public ProductController(IReadProductRepository readProductRepo, IWriteProductRepository writeProductRepo , IReadCategoryRepository readCategoryRepo)
    {
        _readProductRepo = readProductRepo;
        _writeProductRepo = writeProductRepo;
        _readCategoryRepo = readCategoryRepo;
    }

    // -CRUD Operations
    [HttpGet("AllProducts")]
    public async Task<IActionResult> GetAll([FromQuery]PaginationVM paginationVM)
    {
        var products = await _readProductRepo.GetAllAsync();
        var prodcutForPage = products.ToList().
                    Skip(paginationVM.Page*paginationVM.PageSize).Take(paginationVM.PageSize).ToList();


        var allProductVm = prodcutForPage.Select(p => new AllProductVM()
        {
            Name = p.Name,
            Price = p.Price,
            Description = p.Description,
            CategoryName = p.Category.Name,
            ImageUrl = p.ImageUrl,
            Stock = p.Stock
        }).ToList();

        return Ok(allProductVm);
    }
    
    [HttpPost("AddProduct")]
    public async Task<IActionResult> AddProduct([FromBody] AddProductVM productVM)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        var product = new Product()
        {
            Name = productVM.Name,
            Price = productVM.Price,
            Description = productVM.Description,
            CategoryId = productVM.CategoryId,
        };

        await _writeProductRepo.AddAsync(product);
        await _writeProductRepo.SaveChangeAsync();

        return StatusCode(201);
    }

    [HttpPut("UpdateProduct")]
    public async Task<IActionResult> UpdateProduct([FromQuery]int id,[FromBody] UpdateProductVM productVm)
    {
        if (productVm == null || productVm.Id != id)
        {
            Console.WriteLine("Error id");
            return BadRequest();
        }

        var checkExistance = await _readProductRepo.GetByIdAsync(id);
        if (checkExistance == null)
        {
            Console.WriteLine("Error P");
            return NotFound();
        }
        

        checkExistance.Name = productVm.Name;
        var checkCategoryExistance = await _readCategoryRepo.GetByIdAsync(productVm.CategoryId);
        if (checkCategoryExistance == null)
        {
            Console.WriteLine("Error C");
            return NotFound();
        }

        checkExistance.CategoryId = productVm.CategoryId;

        await _writeProductRepo.UpdateAsync(checkExistance);
        await _writeProductRepo.SaveChangeAsync();

        return Ok();
    }

    [HttpDelete("DeleteProduct/")]
    public async Task<IActionResult> DeleteProduct([FromQuery] int id)
    {
        var checkExistance = await _readProductRepo.GetByIdAsync(id);
        
        if (checkExistance == null)
            return NotFound();

        await _writeProductRepo.DeleteAsync(id);
        await _writeProductRepo.SaveChangeAsync();

        return Ok();
    }
}
