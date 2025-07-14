using Microsoft.AspNetCore.Mvc;
using Quartile.Application.Dtos;
using Quartile.Api.Helper;
using Quartile.Application.Interfaces;

namespace Quartile.Api.Controllers.v1;

[ApiController]
[Route("api/[controller]")]
public class StoreController : ControllerBase
{
    private readonly IStoreService _storeService;

    public StoreController(IStoreService storeService)
    {
        _storeService = storeService;
    }

    [HttpPost]
    public async Task<ActionResult<StoreDto>> Create([FromBody] CreateStoreDto createStoreDto)
    {
        var result = await _storeService.CreateAsync(createStoreDto);
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StoreDto>> GetById(Guid id)
    {
        var result = await _storeService.GetByIdAsync(id);
        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StoreDto>>> GetAll()
    {
        var result = await _storeService.GetAllAsync();
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<StoreDto>> Update(Guid id, [FromBody] UpdateStoreDto updateStoreDto)
    {
        var result = await _storeService.UpdateAsync(id, updateStoreDto);
        return result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<bool>> Delete(Guid id)
    {
        var result = await _storeService.DeleteAsync(id);
        return result.ToActionResult();
    }
} 