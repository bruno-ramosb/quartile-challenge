using FluentAssertions;
using NSubstitute;
using Quartile.Application.Dtos;
using Quartile.Application.Services;
using Quartile.Domain.Interfaces.Repositories;

namespace Quartile.Tests.Application.Features.Store;

public class StoreServiceIntegrationTests
{
    private readonly IStoreRepository _storeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly StoreService _storeService;

    public StoreServiceIntegrationTests()
    {
        _storeRepository = Substitute.For<IStoreRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _storeService = new StoreService(_storeRepository, _unitOfWork);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldPassValidation()
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
            Id = Guid.NewGuid(),
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
        result.Data.Email.Should().Be("test@store.com");
    }



    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldPassValidation()
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

        var existingStore = new Quartile.Domain.Entities.Store
        {
            Id = Guid.NewGuid(),
            Name = "Original Store",
            Email = "original@store.com",
            Phone = "1234567890",
            Address = "123 Original St",
            City = "Original City",
            State = "OS",
            ZipCode = "12345",
            CreatedAt = DateTime.UtcNow
        };

        _storeRepository.GetByIdAsync(existingStore.Id).Returns(existingStore);
        _storeRepository.UpdateAsync(Arg.Any<Quartile.Domain.Entities.Store>()).Returns(Task.CompletedTask);
        _unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var result = await _storeService.UpdateAsync(existingStore.Id, updateDto);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Name.Should().Be("Updated Store");
        result.Data.Email.Should().Be("updated@store.com");
    }


} 