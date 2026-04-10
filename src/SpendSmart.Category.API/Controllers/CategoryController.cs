using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendSmart.Category.API.Application.DTOs;
using SpendSmart.Category.API.Application.Services;
using SpendSmart.Category.API.Domain.Entities;

namespace SpendSmart.Category.API.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryItemService _svc;

    public CategoryController(ICategoryItemService svc) => _svc = svc;

    [HttpGet("defaults")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<CategoryItem>), 200)]
    public async Task<IActionResult> GetDefaults()
    {
        var defaults = await _svc.GetDefaultsAsync();
        return Ok(defaults);
    }

    [HttpGet("all-for-user/{userId}")]
    [Authorize]
    [ProducesResponseType(typeof(List<CategoryItem>), 200)]
    public async Task<IActionResult> GetAllForUser(int userId)
    {
        var categories = await _svc.GetAllForUserAsync(userId);
        return Ok(categories);
    }

    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(CategoryItem), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _svc.GetByIdAsync(id);
        return category == null ? NotFound() : Ok(category);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(CategoryItem), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryItemDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var category = await _svc.CreateCustomCategoryAsync(dto);
            return StatusCode(201, category);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}/deactivate")]
    [Authorize]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Deactivate(int id)
    {
        var category = await _svc.GetByIdAsync(id);
        if (category == null)
            return NotFound();

        await _svc.DeactivateCategoryAsync(id);
        return NoContent();
    }

    [HttpPost("seed/{userId}")]
    [AllowAnonymous]  // Allows Auth.API to call without JWT token
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> SeedDefaults(int userId)
    {
        try
        {
            await _svc.SeedDefaultCategoriesAsync(userId);
            return Ok(new { message = "Default categories seeded successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}