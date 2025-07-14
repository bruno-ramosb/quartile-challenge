using FluentAssertions;
using NSubstitute;
using Quartile.Application.Dtos;
using Quartile.Application.Services;
using Quartile.Domain.Interfaces.Repositories;
using FluentValidation;
using Quartile.Application.Validators.Store;

namespace Quartile.Tests.Application.Features.Store;

public class StoreServiceIntegrationTests
{
    private readonly IStoreRepository _storeRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly StoreService _storeService;

    public StoreServiceIntegrationTests()
    {
        _storeRepository = Substitute.For<IStoreRepository>();
        _companyRepository = Substitute.For<ICompanyRepository>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        var createValidator = new CreateStoreDtoValidator();
        var updateValidator = new UpdateStoreDtoValidator();
        _storeService = new StoreService(_storeRepository, _companyRepository, _unitOfWork, createValidator, updateValidator);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldPassValidation()
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
            Id = Guid.NewGuid(),
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
        result.Data.Email.Should().Be("test@store.com");
        result.Data.CompanyId.Should().Be(companyId);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldPassValidation()
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

        var company = new Quartile.Domain.Entities.Company
        {
            Id = companyId,
            Name = "Test Company",
            DocumentNumber = "12345678",
            DocumentType = Quartile.Domain.Enums.DocumentType.CNPJ
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
            CompanyId = companyId,
            CreatedAt = DateTime.UtcNow
        };

        _storeRepository.GetByIdAsync(existingStore.Id).Returns(existingStore);
        _companyRepository.GetByIdAsync(companyId).Returns(company);
        _storeRepository.UpdateAsync(Arg.Any<Quartile.Domain.Entities.Store>()).Returns(Task.CompletedTask);
        _unitOfWork.CommitAsync(Arg.Any<CancellationToken>()).Returns(1);

        var result = await _storeService.UpdateAsync(existingStore.Id, updateDto);

        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data.Name.Should().Be("Updated Store");
        result.Data.Email.Should().Be("updated@store.com");
        result.Data.CompanyId.Should().Be(companyId);
    }
} 