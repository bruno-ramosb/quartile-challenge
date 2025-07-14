using Quartile.Application.Dtos;
using Quartile.Application.Common.Response;

namespace Quartile.Application.Interfaces;

public interface IStoreService
{
    Task<Result<StoreDto>> CreateAsync(CreateStoreDto createStoreDto);
    Task<Result<StoreDto>> GetByIdAsync(Guid id);
    Task<Result<IEnumerable<StoreDto>>> GetAllAsync();
    Task<Result<StoreDto>> UpdateAsync(Guid id, UpdateStoreDto updateStoreDto);
    Task<Result<bool>> DeleteAsync(Guid id);
} 