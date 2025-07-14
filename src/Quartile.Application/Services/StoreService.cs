using Quartile.Application.Dtos;
using Quartile.Application.Common.Response;
using Quartile.Domain.Entities;
using Quartile.Domain.Interfaces.Repositories;
using System.Net;
using Quartile.Application.Interfaces;

namespace Quartile.Application.Services;

public class StoreService : IStoreService
{
    private readonly IStoreRepository _storeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StoreService(
        IStoreRepository storeRepository,
        IUnitOfWork unitOfWork)
    {
        _storeRepository = storeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<StoreDto>> CreateAsync(CreateStoreDto createStoreDto)
    {
        var store = new Store
        {
            Id = Guid.NewGuid(),
            Name = createStoreDto.Name,
            Email = createStoreDto.Email,
            Phone = createStoreDto.Phone,
            Address = createStoreDto.Address,
            City = createStoreDto.City,
            State = createStoreDto.State,
            ZipCode = createStoreDto.ZipCode
        };

        await _storeRepository.AddAsync(store);
        await _unitOfWork.CommitAsync(CancellationToken.None);
        return Result<StoreDto>.Successful(MapToDto(store), "Store created successfully");
    }

    public async Task<Result<StoreDto>> GetByIdAsync(Guid id)
    {
        var store = await _storeRepository.GetByIdAsync(id);
        if (store == null)
        {
            return Result<StoreDto>.Fail("Store not found", HttpStatusCode.NotFound);
        }

        return Result<StoreDto>.Successful(MapToDto(store), "Store retrieved successfully");
    }

    public async Task<Result<IEnumerable<StoreDto>>> GetAllAsync()
    {
        var stores = await _storeRepository.GetAllAsync();
        var storeDtos = stores.Select(MapToDto);
        return Result<IEnumerable<StoreDto>>.Successful(storeDtos, "Stores retrieved successfully");
    }

    public async Task<Result<StoreDto>> UpdateAsync(Guid id, UpdateStoreDto updateStoreDto)
    {
        var existingStore = await _storeRepository.GetByIdAsync(id);
        if (existingStore == null)
        {
            return Result<StoreDto>.Fail("Store not found", HttpStatusCode.NotFound);
        }

        existingStore.Name = updateStoreDto.Name;
        existingStore.Email = updateStoreDto.Email;
        existingStore.Phone = updateStoreDto.Phone;
        existingStore.Address = updateStoreDto.Address;
        existingStore.City = updateStoreDto.City;
        existingStore.State = updateStoreDto.State;
        existingStore.ZipCode = updateStoreDto.ZipCode;

        await _storeRepository.UpdateAsync(existingStore);
        await _unitOfWork.CommitAsync(CancellationToken.None);
        return Result<StoreDto>.Successful(MapToDto(existingStore), "Store updated successfully");
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var existingStore = await _storeRepository.GetByIdAsync(id);
        if (existingStore == null)
        {
            return Result<bool>.Fail("Store not found", HttpStatusCode.NotFound);
        }

        await _storeRepository.RemoveAsync(existingStore);
        await _unitOfWork.CommitAsync(CancellationToken.None);
        return Result<bool>.Successful(true, "Store deleted successfully");
    }

    private static StoreDto MapToDto(Store store)
    {
        return new StoreDto
        {
            Id = store.Id,
            Name = store.Name,
            Email = store.Email,
            Phone = store.Phone,
            Address = store.Address,
            City = store.City,
            State = store.State,
            ZipCode = store.ZipCode,
            CreatedAt = store.CreatedAt,
            UpdatedAt = store.UpdatedAt
        };
    }
} 