using Quartile.Application.Dtos;
using Quartile.Application.Common.Response;
using Quartile.Domain.Entities;
using Quartile.Domain.Interfaces.Repositories;
using System.Net;
using Quartile.Application.Interfaces;
using FluentValidation;
using Quartile.Application.Validators.Store;

namespace Quartile.Application.Services;

public class StoreService : IStoreService
{
    private readonly IStoreRepository _storeRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateStoreDto> _createValidator;
    private readonly IValidator<UpdateStoreDto> _updateValidator;

    public StoreService(
        IStoreRepository storeRepository,
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateStoreDto> createValidator,
        IValidator<UpdateStoreDto> updateValidator)
    {
        _storeRepository = storeRepository;
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<Result<StoreDto>> CreateAsync(CreateStoreDto createStoreDto)
    {
        var validationResult = await _createValidator.ValidateAsync(createStoreDto);
        if (!validationResult.IsValid)
        {
            return Result<StoreDto>.Fail(validationResult.Errors);
        }

        var company = await _companyRepository.GetByIdAsync(createStoreDto.CompanyId);
        if (company == null)
        {
            return Result<StoreDto>.Fail("Company not found", HttpStatusCode.BadRequest);
        }

        var store = new Store
        {
            Id = Guid.NewGuid(),
            Name = createStoreDto.Name,
            Email = createStoreDto.Email,
            Phone = createStoreDto.Phone,
            Address = createStoreDto.Address,
            City = createStoreDto.City,
            State = createStoreDto.State,
            ZipCode = createStoreDto.ZipCode,
            CompanyId = createStoreDto.CompanyId
        };

        await _storeRepository.AddAsync(store);
        await _unitOfWork.CommitAsync(CancellationToken.None);
        return Result<StoreDto>.Successful(MapToDto(store, company), "Store created successfully");
    }

    public async Task<Result<StoreDto>> GetByIdAsync(Guid id)
    {
        var store = await _storeRepository.GetByIdAsync(id);
        if (store == null)
        {
            return Result<StoreDto>.Fail("Store not found", HttpStatusCode.NotFound);
        }

        var company = await _companyRepository.GetByIdAsync(store.CompanyId);
        return Result<StoreDto>.Successful(MapToDto(store, company), "Store retrieved successfully");
    }

    public async Task<Result<IEnumerable<StoreDto>>> GetAllAsync()
    {
        var stores = await _storeRepository.GetAllAsync();
        var storeDtos = new List<StoreDto>();
        
        foreach (var store in stores)
        {
            var company = await _companyRepository.GetByIdAsync(store.CompanyId);
            storeDtos.Add(MapToDto(store, company));
        }
        
        return Result<IEnumerable<StoreDto>>.Successful(storeDtos, "Stores retrieved successfully");
    }

    public async Task<Result<StoreDto>> UpdateAsync(Guid id, UpdateStoreDto updateStoreDto)
    {
        var validationResult = await _updateValidator.ValidateAsync(updateStoreDto);
        if (!validationResult.IsValid)
        {
            return Result<StoreDto>.Fail(validationResult.Errors);
        }

        var existingStore = await _storeRepository.GetByIdAsync(id);
        if (existingStore == null)
        {
            return Result<StoreDto>.Fail("Store not found", HttpStatusCode.NotFound);
        }

        var company = await _companyRepository.GetByIdAsync(updateStoreDto.CompanyId);
        if (company == null)
        {
            return Result<StoreDto>.Fail("Company not found", HttpStatusCode.BadRequest);
        }

        existingStore.Name = updateStoreDto.Name;
        existingStore.Email = updateStoreDto.Email;
        existingStore.Phone = updateStoreDto.Phone;
        existingStore.Address = updateStoreDto.Address;
        existingStore.City = updateStoreDto.City;
        existingStore.State = updateStoreDto.State;
        existingStore.ZipCode = updateStoreDto.ZipCode;
        existingStore.CompanyId = updateStoreDto.CompanyId;

        await _storeRepository.UpdateAsync(existingStore);
        await _unitOfWork.CommitAsync(CancellationToken.None);
        return Result<StoreDto>.Successful(MapToDto(existingStore, company), "Store updated successfully");
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

    private static StoreDto MapToDto(Store store, Company? company = null)
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
            UpdatedAt = store.UpdatedAt,
            CompanyId = store.CompanyId,
            CompanyName = company?.Name ?? string.Empty
        };
    }
} 