using FluentAssertions;
using Quartile.Application.Dtos;

namespace Quartile.Tests.Application.Dtos;

public class StoreDtoTests
{
    [Fact]
    public void StoreDto_ShouldHaveCorrectProperties()
    {
        var id = Guid.NewGuid();
        var storeDto = new StoreDto
        {
            Id = id,
            Name = "Test Store",
            Email = "test@store.com",
            Phone = "1234567890",
            Address = "123 Test St",
            City = "Test City",
            State = "TS",
            ZipCode = "12345",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        storeDto.Id.Should().Be(id);
        storeDto.Name.Should().Be("Test Store");
        storeDto.Email.Should().Be("test@store.com");
        storeDto.Phone.Should().Be("1234567890");
        storeDto.Address.Should().Be("123 Test St");
        storeDto.City.Should().Be("Test City");
        storeDto.State.Should().Be("TS");
        storeDto.ZipCode.Should().Be("12345");
        storeDto.CreatedAt.Should().NotBe(default);
        storeDto.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void CreateStoreDto_ShouldHaveCorrectProperties()
    {
        var createStoreDto = new CreateStoreDto
        {
            Name = "Test Store",
            Email = "test@store.com",
            Phone = "1234567890",
            Address = "123 Test St",
            City = "Test City",
            State = "TS",
            ZipCode = "12345"
        };

        createStoreDto.Name.Should().Be("Test Store");
        createStoreDto.Email.Should().Be("test@store.com");
        createStoreDto.Phone.Should().Be("1234567890");
        createStoreDto.Address.Should().Be("123 Test St");
        createStoreDto.City.Should().Be("Test City");
        createStoreDto.State.Should().Be("TS");
        createStoreDto.ZipCode.Should().Be("12345");
    }

    [Fact]
    public void UpdateStoreDto_ShouldHaveCorrectProperties()
    {
        var updateStoreDto = new UpdateStoreDto
        {
            Name = "Updated Store",
            Email = "updated@store.com",
            Phone = "0987654321",
            Address = "456 Updated St",
            City = "Updated City",
            State = "US",
            ZipCode = "54321"
        };

        updateStoreDto.Name.Should().Be("Updated Store");
        updateStoreDto.Email.Should().Be("updated@store.com");
        updateStoreDto.Phone.Should().Be("0987654321");
        updateStoreDto.Address.Should().Be("456 Updated St");
        updateStoreDto.City.Should().Be("Updated City");
        updateStoreDto.State.Should().Be("US");
        updateStoreDto.ZipCode.Should().Be("54321");
    }
} 