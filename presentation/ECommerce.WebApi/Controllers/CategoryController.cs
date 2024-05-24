using ECommerce.Application.Repositories;
using ECommerce.Domain.Entities.Concretes;
using ECommerce.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly IWriteCategoryRepository _writeCategoryRepo;
    private readonly IReadCategoryRepository _readCategoryRepo;

    public CategoryController(IWriteCategoryRepository writeCategoryRepo , IReadCategoryRepository readCategoryRepo)
    {
        _writeCategoryRepo = writeCategoryRepo;
        _readCategoryRepo = readCategoryRepo;
    }
    
    // -CRUD Operations

    [HttpGet("AllCategories")]
    public async Task<IActionResult> GetAll([FromQuery]PaginationVM paginationVM)
    {
        var categories = await _readCategoryRepo.GetAllAsync();
        var categoryForPage = categories.ToList()
            .Skip(paginationVM.PageSize * paginationVM.Page)
            .Take(paginationVM.PageSize).ToList();

        var allCategoriesVM = categoryForPage.Select(c => new AllCategoriesVM()
        {
            Name = c.Name,
            Description = c.Description
        }).ToList();

        return Ok(allCategoriesVM);
    }

    [HttpPost("AddCategory")]
    public async Task<IActionResult> AddCategory([FromBody] AddCategoryVM categoryVM)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var category = new Category()
        {
            Name = categoryVM.Name,
            Description = categoryVM.Description,
        };

        await _writeCategoryRepo.AddAsync(category);
        await _writeCategoryRepo.SaveChangeAsync();

        return StatusCode(201);
    }

    [HttpPut("UpdateCategory")]
    public async Task<IActionResult> UpdateCategory(int id,[FromBody] UpdateCategoryVM categoryVm)
    {
        var category = await _readCategoryRepo.GetByIdAsync(id);

        if (category == null)
            return NotFound();

        category.Name = categoryVm.Name;
        category.Description = categoryVm.Description;
        category.ImageUrl = categoryVm.ImageUrl;

        await _writeCategoryRepo.UpdateAsync(category);
        await _writeCategoryRepo.SaveChangeAsync();

        return Ok();
    }

    [HttpDelete("DeleteCategory")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _readCategoryRepo.GetByIdAsync(id);

        if (category == null)
            return NotFound("Category Not Found !");

        await _writeCategoryRepo.DeleteAsync(category.Id);
        await _writeCategoryRepo.SaveChangeAsync();

        return Ok();
    }

    // -Additional Functionalities

    [HttpGet("GetProductsByCategory")]
    public async Task<IActionResult> GetProducts(int id)
    {
        var products = await _readCategoryRepo.GetProductsByCategory(id);
        if (products == null)
            return NotFound("Category Not Found");

        var allProductVm = products.Select(p => new GetProductVM()
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Description = p.Description,
            CategoryName = p.Category.Name,
            ImageUrl = p.ImageUrl,
            Stock = p.Stock
        }).ToList();

        return Ok(allProductVm);
    }
}
