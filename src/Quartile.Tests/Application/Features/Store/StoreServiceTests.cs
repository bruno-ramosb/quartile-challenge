using FluentAssertions;
using NSubstitute;
using Quartile.Application.Dtos;
using Quartile.Application.Services;
using Quartile.Domain.Interfaces.Repositories;

namespace Quartile.Tests.Application.Features.Store;

public class StoreServiceTests
{
    private readonly IStoreRepository _storeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly StoreService _storeService;

    public StoreServiceTests()
    {
        _storeRepository = Substitute.For<IStoreRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _storeService = new StoreService(_storeRepository, _unitOfWork);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldReturnSuccess()
    {
        var createDto = new CreateStoreDto
        {
            Name = "Test Store",
            Email = "test@store.com",
            Phone = "1234567890",
            Address = "123 Test St",
            City = "Test City",
            State = "TS",
            ZipCode = "12345"
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
            CreatedAt = DateTime.UtcNow
        };

        _storeRepository.AddAsync(Arg.Any<Quartile.Domain.Entities.Store>()).Returns(Task.CompletedTask);
        _unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var result = await _storeService.CreateAsync(createDto);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Name.Should().Be("Test Store");
    }



    [Fact]
    public async Task GetByIdAsync_WithExistingStore_ShouldReturnStore()
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
            CreatedAt = DateTime.UtcNow
        };

        _storeRepository.GetByIdAsync(storeId).Returns(store);

        var result = await _storeService.GetByIdAsync(storeId);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Id.Should().Be(storeId);
        result.Data.Name.Should().Be("Test Store");
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
        var stores = new List<Quartile.Domain.Entities.Store>
        {
            new Quartile.Domain.Entities.Store { Id = Guid.NewGuid(), Name = "Store 1", Email = "store1@test.com", Phone = "1234567890", Address = "123 St", City = "City", State = "ST", ZipCode = "12345", CreatedAt = DateTime.UtcNow },
            new Quartile.Domain.Entities.Store { Id = Guid.NewGuid(), Name = "Store 2", Email = "store2@test.com", Phone = "0987654321", Address = "456 St", City = "City2", State = "ST2", ZipCode = "54321", CreatedAt = DateTime.UtcNow }
        };

        _storeRepository.GetAllAsync().Returns(stores);

        var result = await _storeService.GetAllAsync();

        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data.First().Name.Should().Be("Store 1");
        result.Data.Last().Name.Should().Be("Store 2");
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldReturnSuccess()
    {
        var updateDto = new UpdateStoreDto
        {
            Name = "Updated Store",
            Email = "updated@store.com",
            Phone = "0987654321",
            Address = "456 Updated St",
            City = "Updated City",
            State = "US",
            ZipCode = "54321"
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
            CreatedAt = DateTime.UtcNow
        };

        _storeRepository.GetByIdAsync(storeId).Returns(existingStore);
        _storeRepository.UpdateAsync(Arg.Any<Quartile.Domain.Entities.Store>()).Returns(Task.CompletedTask);
        _unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var result = await _storeService.UpdateAsync(storeId, updateDto);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Name.Should().Be("Updated Store");
        result.Data.Email.Should().Be("updated@store.com");
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingStore_ShouldReturnFailure()
    {
        var updateDto = new UpdateStoreDto
        {
            Name = "Updated Store",
            Email = "updated@store.com",
            Phone = "0987654321",
            Address = "456 Updated St",
            City = "Updated City",
            State = "US",
            ZipCode = "54321"
        };

        var storeId = Guid.NewGuid();
        _storeRepository.GetByIdAsync(storeId).Returns((Quartile.Domain.Entities.Store?)null);

        var result = await _storeService.UpdateAsync(storeId, updateDto);

        result.Success.Should().BeFalse();
        result.Notifications.Should().Contain("Store not found");
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
            CreatedAt = DateTime.UtcNow
        };

        _storeRepository.GetByIdAsync(storeId).Returns(store);
        _unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var result = await _storeService.DeleteAsync(storeId);

        result.Success.Should().BeTrue();
        await _storeRepository.Received(1).RemoveAsync(store);
        await _unitOfWork.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingStore_ShouldReturnFailure()
    {
        var storeId = Guid.NewGuid();
        _storeRepository.GetByIdAsync(storeId).Returns((Quartile.Domain.Entities.Store?)null);

        var result = await _storeService.DeleteAsync(storeId);

        result.Success.Should().BeFalse();
        result.Notifications.Should().Contain("Store not found");
        await _storeRepository.DidNotReceive().RemoveAsync(Arg.Any<Quartile.Domain.Entities.Store>());
        await _unitOfWork.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
    }
} 