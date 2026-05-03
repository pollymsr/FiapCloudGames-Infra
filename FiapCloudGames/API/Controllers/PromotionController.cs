using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FiapCloudGames.Application.DTOs;
using FiapCloudGames.Application.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace FiapCloudGames.API.Controllers;

[ApiController]
[Route("api/promotions")]
[Authorize(Roles = "Admin")]
[Tags("Promoções")]
public class PromotionController : ControllerBase
{
    private readonly IPromotionService _promotionService;

    public PromotionController(IPromotionService promotionService)
    {
        _promotionService = promotionService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get All Promotions")]
    public async Task<IActionResult> ListAllPromotions()
    {
        var promotions = await _promotionService.GetAllAsync();
        return Ok(promotions.Select(p => new PromotionResponseDto
        {
            Id = p.Id,
            Code = p.Code,
            Description = p.Description,
            DiscountPercentage = p.DiscountPercentage,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            IsActive = p.IsActive,
            MaxUses = p.MaxUses,
            CurrentUses = p.CurrentUses
        }));
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get Promotion By Id")]
    public async Task<IActionResult> GetPromotionById(Guid id)
    {
        var promotion = await _promotionService.GetByIdAsync(id);
        if (promotion == null)
            return NotFound("Promoção não encontrada");

        return Ok(new PromotionResponseDto
        {
            Id = promotion.Id,
            Code = promotion.Code,
            Description = promotion.Description,
            DiscountPercentage = promotion.DiscountPercentage,
            StartDate = promotion.StartDate,
            EndDate = promotion.EndDate,
            IsActive = promotion.IsActive,
            MaxUses = promotion.MaxUses,
            CurrentUses = promotion.CurrentUses
        });
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create Promotion")]
    public async Task<IActionResult> CreatePromotion([FromBody] CreatePromotionDto dto)
    {
        try
        {
            var promotion = await _promotionService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetPromotionById), new { id = promotion.Id }, promotion);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update Promotion By Id")]
    public async Task<IActionResult> UpdatePromotionById(Guid id, [FromBody] UpdatePromotionDto dto)
    {
        try
        {
            var promotion = await _promotionService.UpdateAsync(id, dto);
            if (promotion == null)
                return NotFound("Promoção não encontrada");

            return Ok(new PromotionResponseDto
            {
                Id = promotion.Id,
                Code = promotion.Code,
                Description = promotion.Description,
                DiscountPercentage = promotion.DiscountPercentage,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                IsActive = promotion.IsActive,
                MaxUses = promotion.MaxUses,
                CurrentUses = promotion.CurrentUses
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete Promotion By Id")]
    public async Task<IActionResult> DeletePromotionById(Guid id)
    {
        if (!await _promotionService.DeleteAsync(id))
            return NotFound("Promoção não encontrada");

        return NoContent();
    }
}