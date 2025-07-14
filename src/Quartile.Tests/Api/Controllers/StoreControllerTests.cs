using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Quartile.Application.Dtos;
using Quartile.Application.Common.Response;
using Quartile.Api.Controllers.v1;
using Quartile.Application.Interfaces;

namespace Quartile.Tests.Api.Controllers;

public class StoreControllerTests
{
    private readonly IStoreService _storeService;
    private readonly StoreController _controller;

    public StoreControllerTests()
    {
        _storeService = Substitute.For<IStoreService>();
        _controller = new StoreController(_storeService);
    }

    [Fact]
    public async Task Create_WithValidData_ShouldReturnOkResult()
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

        var storeDto = new StoreDto
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

        var result = Result<StoreDto>.Successful(storeDto, "Store created successfully");
        _storeService.CreateAsync(createDto).Returns(result);

        var actionResult = await _controller.Create(createDto);

        actionResult.Should().BeOfType<ActionResult<StoreDto>>();
        var objectResult = actionResult.Result as ObjectResult;
        objectResult!.Value.Should().BeEquivalentTo(storeDto);
        objectResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Create_WithInvalidData_ShouldReturnBadRequest()
    {
        var createDto = new CreateStoreDto
        {
            Name = "",
            Email = "invalid-email",
            Phone = "",
            Address = "",
            City = "",
            State = "",
            ZipCode = ""
        };

        var result = Result<StoreDto>.Fail("Name is required");
        _storeService.CreateAsync(createDto).Returns(result);

        var actionResult = await _controller.Create(createDto);

        actionResult.Should().BeOfType<ActionResult<StoreDto>>();
        var objectResult = actionResult.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(400);
        objectResult.Value.Should().BeEquivalentTo(new { Notifications = new[] { "Name is required" } });
    }

    [Fact]
    public async Task GetById_WithExistingStore_ShouldReturnOkResult()
    {
        var storeId = Guid.NewGuid();
        var storeDto = new StoreDto
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

        var result = Result<StoreDto>.Successful(storeDto, "Store retrieved successfully");
        _storeService.GetByIdAsync(storeId).Returns(result);

        var actionResult = await _controller.GetById(storeId);

        actionResult.Should().BeOfType<ActionResult<StoreDto>>();
        var objectResult = actionResult.Result as ObjectResult;
        objectResult!.Value.Should().BeEquivalentTo(storeDto);
        objectResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetById_WithNonExistingStore_ShouldReturnNotFound()
    {
        var storeId = Guid.NewGuid();
        var result = Result<StoreDto>.Fail("Store not found", System.Net.HttpStatusCode.NotFound);
        _storeService.GetByIdAsync(storeId).Returns(result);

        var actionResult = await _controller.GetById(storeId);

        actionResult.Should().BeOfType<ActionResult<StoreDto>>();
        var objectResult = actionResult.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(404);
        objectResult.Value.Should().BeEquivalentTo(new { Notifications = new[] { "Store not found" } });
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkResult()
    {
        var storeDtos = new List<StoreDto>
        {
            new StoreDto { Id = Guid.NewGuid(), Name = "Store 1", Email = "store1@test.com", Phone = "1234567890", Address = "123 St", City = "City", State = "ST", ZipCode = "12345", CreatedAt = DateTime.UtcNow },
            new StoreDto { Id = Guid.NewGuid(), Name = "Store 2", Email = "store2@test.com", Phone = "0987654321", Address = "456 St", City = "City2", State = "ST2", ZipCode = "54321", CreatedAt = DateTime.UtcNow }
        };

        var result = Result<IEnumerable<StoreDto>>.Successful(storeDtos, "Stores retrieved successfully");
        _storeService.GetAllAsync().Returns(result);

        var actionResult = await _controller.GetAll();

        actionResult.Should().BeOfType<ActionResult<IEnumerable<StoreDto>>>();
        var objectResult = actionResult.Result as ObjectResult;
        objectResult!.Value.Should().BeEquivalentTo(storeDtos);
        objectResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Update_WithValidData_ShouldReturnOkResult()
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
        var storeDto = new StoreDto
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

        var result = Result<StoreDto>.Successful(storeDto);
        _storeService.UpdateAsync(storeId, updateDto).Returns(result);

        var actionResult = await _controller.Update(storeId, updateDto);

        actionResult.Should().BeOfType<ActionResult<StoreDto>>();
        var objectResult = actionResult.Result as ObjectResult;
        objectResult!.Value.Should().BeEquivalentTo(storeDto);
        objectResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Update_WithNonExistingStore_ShouldReturnNotFound()
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
        var result = Result<StoreDto>.Fail("Store not found", System.Net.HttpStatusCode.NotFound);
        _storeService.UpdateAsync(storeId, updateDto).Returns(result);

        var actionResult = await _controller.Update(storeId, updateDto);

        actionResult.Should().BeOfType<ActionResult<StoreDto>>();
        var objectResult = actionResult.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(404);
        objectResult.Value.Should().BeEquivalentTo(new { Notifications = new[] { "Store not found" } });
    }

    [Fact]
    public async Task Delete_WithExistingStore_ShouldReturnOkResult()
    {
        var storeId = Guid.NewGuid();
        var result = Result<bool>.Successful(true, "Store deleted successfully");
        _storeService.DeleteAsync(storeId).Returns(result);

        var actionResult = await _controller.Delete(storeId);

        actionResult.Should().BeOfType<ActionResult<bool>>();
        var objectResult = actionResult.Result as ObjectResult;
        objectResult!.Value.Should().Be(true);
        objectResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Delete_WithNonExistingStore_ShouldReturnNotFound()
    {
        var storeId = Guid.NewGuid();
        var result = Result<bool>.Fail("Store not found", System.Net.HttpStatusCode.NotFound);
        _storeService.DeleteAsync(storeId).Returns(result);

        var actionResult = await _controller.Delete(storeId);

        actionResult.Should().BeOfType<ActionResult<bool>>();
        var objectResult = actionResult.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(404);
        objectResult.Value.Should().BeEquivalentTo(new { Notifications = new[] { "Store not found" } });
    }
} 