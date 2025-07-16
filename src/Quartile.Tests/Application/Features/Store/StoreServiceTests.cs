using FluentAssertions;
using NSubstitute;
using Quartile.Application.Dtos;
using Quartile.Application.Services;
using Quartile.Domain.Interfaces.Repositories;
using FluentValidation;
using Quartile.Application.Validators.Store;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace Quartile.Tests.Application.Features.Store;

public class StoreServiceTests
{
    private readonly IStoreRepository _storeRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StoreService> _logger;
    private readonly StoreService _storeService;

    public StoreServiceTests()
    {
        _storeRepository = Substitute.For<IStoreRepository>();
        _companyRepository = Substitute.For<ICompanyRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _logger = Substitute.For<ILogger<StoreService>>();
        var createValidator = new CreateStoreDtoValidator();
        var updateValidator = new UpdateStoreDtoValidator();
        _storeService = new StoreService(_storeRepository, _companyRepository, _unitOfWork, createValidator, updateValidator, _logger);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldReturnSuccess()
    {
        var companyId = Guid.NewGuid();
        var createDto = new CreateStoreDto
        {
            Name = "Test Store",
            Email = "test@store.com",
            Phone = "1234567890",
            Address = "123 Test St",
            City = "Test City",
            State = "TS",
            ZipCode = "12345",
            CompanyId = companyId
        };

        var company = new Quartile.Domain.Entities.Company
        {
            Id = companyId,
            Name = "Test Company",
            DocumentNumber = "12345678",
            DocumentType = Quartile.Domain.Enums.DocumentType.CNPJ
        };

        var store = new Quartile.Domain.Entities.Store
        {
            Name = createDto.Name,
            Email = createDto.Email,
            Phone = createDto.Phone,
            Address = createDto.Address,
            City = createDto.City,
            State = createDto.State,
            ZipCode = createDto.ZipCode,
            CompanyId = companyId,
            CreatedAt = DateTime.UtcNow
        };

        _companyRepository.GetByIdAsync(companyId).Returns(company);
        _storeRepository.AddAsync(Arg.Any<Quartile.Domain.Entities.Store>()).Returns(Task.CompletedTask);
        _unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var result = await _storeService.CreateAsync(createDto);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Name.Should().Be("Test Store");
        result.Data.CompanyId.Should().Be(companyId);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidCompany_ShouldReturnFailure()
    {
        var companyId = Guid.NewGuid();
        var createDto = new CreateStoreDto
        {
            Name = "Test Store",
            Email = "test@store.com",
            Phone = "1234567890",
            Address = "123 Test St",
            City = "Test City",
            State = "TS",
            ZipCode = "12345",
            CompanyId = companyId
        };

        _companyRepository.GetByIdAsync(companyId).Returns((Quartile.Domain.Entities.Company?)null);

        var result = await _storeService.CreateAsync(createDto);

        result.Success.Should().BeFalse();
        result.Notifications.Should().Contain("Company not found");
    }

    [Fact]
    public async Task CreateAsync_WithInvalidData_ShouldReturnValidationFailure()
    {
        var createDto = new CreateStoreDto
        {
            Name = "",
            Email = "invalid-email",
            Phone = "",
            Address = "",
            City = "",
            State = "",
            ZipCode = "",
            CompanyId = Guid.NewGuid()
        };

        var result = await _storeService.CreateAsync(createDto);

        result.Success.Should().BeFalse();
        result.Notifications.Should().Contain("Name is required");
        result.Notifications.Should().Contain("Email must be a valid email address");
        result.Notifications.Should().Contain("Phone is required");
        result.Notifications.Should().Contain("Address is required");
        result.Notifications.Should().Contain("City is required");
        result.Notifications.Should().Contain("State is required");
        result.Notifications.Should().Contain("ZipCode is required");
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingStore_ShouldReturnStore()
    {
        var storeId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var store = new Quartile.Domain.Entities.Store
        {
            Id = storeId,
            Name = "Test Store",
            Email = "test@store.com",
            Phone = "1234567890",
            Address = "123 Test St",
            City = "Test City",
            State = "TS",
            ZipCode = "12345",
            CompanyId = companyId,
            CreatedAt = DateTime.UtcNow
        };

        var company = new Quartile.Domain.Entities.Company
        {
            Id = companyId,
            Name = "Test Company",
            DocumentNumber = "12345678",
            DocumentType = Quartile.Domain.Enums.DocumentType.CNPJ
        };

        _storeRepository.GetByIdAsync(storeId).Returns(store);
        _companyRepository.GetByIdAsync(companyId).Returns(company);

        var result = await _storeService.GetByIdAsync(storeId);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(storeId);
        result.Data.Name.Should().Be("Test Store");
        result.Data.CompanyId.Should().Be(companyId);
        result.Data.CompanyName.Should().Be("Test Company");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingStore_ShouldReturnFailure()
    {
        var storeId = Guid.NewGuid();
        _storeRepository.GetByIdAsync(storeId).Returns((Quartile.Domain.Entities.Store?)null);

        var result = await _storeService.GetByIdAsync(storeId);

        result.Success.Should().BeFalse();
        result.Notifications.Should().Contain("Store not found");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllStores()
    {
        var companyId = Guid.NewGuid();
        var stores = new List<Quartile.Domain.Entities.Store>
        {
            new Quartile.Domain.Entities.Store { Id = Guid.NewGuid(), Name = "Store 1", Email = "store1@test.com", Phone = "1234567890", Address = "123 St", City = "City", State = "ST", ZipCode = "12345", CompanyId = companyId, CreatedAt = DateTime.UtcNow },
            new Quartile.Domain.Entities.Store { Id = Guid.NewGuid(), Name = "Store 2", Email = "store2@test.com", Phone = "0987654321", Address = "456 St", City = "City2", State = "ST2", ZipCode = "54321", CompanyId = companyId, CreatedAt = DateTime.UtcNow }
        };

        var company = new Quartile.Domain.Entities.Company
        {
            Id = companyId,
            Name = "Test Company",
            DocumentNumber = "12345678",
            DocumentType = Quartile.Domain.Enums.DocumentType.CNPJ
        };

        _storeRepository.GetAllAsync().Returns(stores);
        _companyRepository.GetByIdAsync(companyId).Returns(company);

        var result = await _storeService.GetAllAsync();

        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data.First().Name.Should().Be("Store 1");
        result.Data.Last().Name.Should().Be("Store 2");
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldReturnSuccess()
    {
        var companyId = Guid.NewGuid();
        var updateDto = new UpdateStoreDto
        {
            Name = "Updated Store",
            Email = "updated@store.com",
            Phone = "0987654321",
            Address = "456 Updated St",
            City = "Updated City",
            State = "US",
            ZipCode = "54321",
            CompanyId = companyId
        };
        var storeId = Guid.NewGuid();

        var updatedStore = new Quartile.Domain.Entities.Store
        {
            Id = storeId,
            Name = updateDto.Name,
            Email = updateDto.Email,
            Phone = updateDto.Phone,
            Address = updateDto.Address,
            City = updateDto.City,
            State = updateDto.State,
            ZipCode = updateDto.ZipCode,
            CompanyId = companyId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var existingStore = new Quartile.Domain.Entities.Store
        {
            Id = storeId,
            Name = "Original Store",
            Email = "original@store.com",
            Phone = "1234567890",
            Address = "123 Original St",
            City = "Original City",
            State = "OS",
            ZipCode = "12345",
            CompanyId = companyId,
            CreatedAt = DateTime.UtcNow
        };

        var company = new Quartile.Domain.Entities.Company
        {
            Id = companyId,
            Name = "Test Company",
            DocumentNumber = "12345678",
            DocumentType = Quartile.Domain.Enums.DocumentType.CNPJ
        };

        _storeRepository.GetByIdAsync(storeId).Returns(existingStore);
        _companyRepository.GetByIdAsync(companyId).Returns(company);
        _storeRepository.UpdateAsync(Arg.Any<Quartile.Domain.Entities.Store>()).Returns(Task.CompletedTask);
        _unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var result = await _storeService.UpdateAsync(storeId, updateDto);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Name.Should().Be("Updated Store");
        result.Data.Email.Should().Be("updated@store.com");
        result.Data.CompanyId.Should().Be(companyId);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingStore_ShouldReturnFailure()
    {
        var companyId = Guid.NewGuid();
        var updateDto = new UpdateStoreDto
        {
            Name = "Updated Store",
            Email = "updated@store.com",
            Phone = "0987654321",
            Address = "456 Updated St",
            City = "Updated City",
            State = "US",
            ZipCode = "54321",
            CompanyId = companyId
        };

        var storeId = Guid.NewGuid();
        _storeRepository.GetByIdAsync(storeId).Returns((Quartile.Domain.Entities.Store?)null);

        var result = await _storeService.UpdateAsync(storeId, updateDto);

        result.Success.Should().BeFalse();
        result.Notifications.Should().Contain("Store not found");
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidCompany_ShouldReturnFailure()
    {
        var companyId = Guid.NewGuid();
        var updateDto = new UpdateStoreDto
        {
            Name = "Updated Store",
            Email = "updated@store.com",
            Phone = "0987654321",
            Address = "456 Updated St",
            City = "Updated City",
            State = "US",
            ZipCode = "54321",
            CompanyId = companyId
        };

        var storeId = Guid.NewGuid();
        var existingStore = new Quartile.Domain.Entities.Store
        {
            Id = storeId,
            Name = "Original Store",
            Email = "original@store.com",
            Phone = "1234567890",
            Address = "123 Original St",
            City = "Original City",
            State = "OS",
            ZipCode = "12345",
            CompanyId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        _storeRepository.GetByIdAsync(storeId).Returns(existingStore);
        _companyRepository.GetByIdAsync(companyId).Returns((Quartile.Domain.Entities.Company?)null);

        var result = await _storeService.UpdateAsync(storeId, updateDto);

        result.Success.Should().BeFalse();
        result.Notifications.Should().Contain("Company not found");
    }

    [Fact]
    public async Task DeleteAsync_WithExistingStore_ShouldReturnSuccess()
    {
        var storeId = Guid.NewGuid();
        var store = new Quartile.Domain.Entities.Store
        {
            Id = storeId,
            Name = "Test Store",
            Email = "test@store.com",
            Phone = "1234567890",
            Address = "123 Test St",
            City = "Test City",
            State = "TS",
            ZipCode = "12345",
            CompanyId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        _storeRepository.GetByIdAsync(storeId).Returns(store);
        _unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var result = await _storeService.DeleteAsync(storeId);

        result.Success.Should().BeTrue();
        result.Data.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingStore_ShouldReturnFailure()
    {
        var storeId = Guid.NewGuid();
        _storeRepository.GetByIdAsync(storeId).Returns((Quartile.Domain.Entities.Store?)null);

        var result = await _storeService.DeleteAsync(storeId);

        result.Success.Should().BeFalse();
        result.Notifications.Should().Contain("Store not found");
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidData_ShouldReturnValidationFailure()
    {
        var updateDto = new UpdateStoreDto
        {
            Name = "",
            Email = "invalid-email",
            Phone = "",
            Address = "",
            City = "",
            State = "",
            ZipCode = "",
            CompanyId = Guid.NewGuid()
        };
        var storeId = Guid.NewGuid();

        var result = await _storeService.UpdateAsync(storeId, updateDto);

        result.Success.Should().BeFalse();
        result.Notifications.Should().Contain("Name is required");
        result.Notifications.Should().Contain("Email must be a valid email address");
        result.Notifications.Should().Contain("Phone is required");
        result.Notifications.Should().Contain("Address is required");
        result.Notifications.Should().Contain("City is required");
        result.Notifications.Should().Contain("State is required");
        result.Notifications.Should().Contain("ZipCode is required");
    }
} 