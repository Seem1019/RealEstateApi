using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using RealEstate.Application.DTOs;
using RealEstate.Application.Services;
using RealEstate.Domain.Common;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Exceptions;
using RealEstate.Domain.Interfaces;

namespace RealEstate.Tests
{
    [TestFixture]
    public class PropertyServiceTests
    {
        private Mock<IPropertyRepository> _mockRepo;
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<PropertyService>> _mockLogger;
        private PropertyService _service;

        [SetUp]
        public void Setup()
        {
            _mockRepo = new Mock<IPropertyRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<PropertyService>>();
            _service = new PropertyService(_mockRepo.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Test]
        public async Task CreateAsync_ValidDto_CallsAddAndSave()
        {
            // Arrange
            var dto = new CreatePropertyDto { Name = "Test", Address = "Addr", Price = 100, CodeInternal = "Code", Year = 2020, IdOwner = 1 };
            var expectedDto = new PropertyDto { IdProperty = 1, Name = dto.Name, Address = dto.Address, Price = dto.Price, CodeInternal = dto.CodeInternal, Year = dto.Year, IdOwner = dto.IdOwner };
            _mockMapper.Setup(m => m.Map<PropertyDto>(It.IsAny<Property>())).Returns(expectedDto);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            _mockRepo.Verify(r => r.AddAsync(It.Is<Property>(p =>
                p.Name == dto.Name &&
                p.Address == dto.Address &&
                p.Price == dto.Price &&
                p.CodeInternal == dto.CodeInternal &&
                p.Year == dto.Year &&
                p.IdOwner == dto.IdOwner)), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(dto.Name));
            Assert.That(result.IdProperty, Is.EqualTo(1));
        }

        [Test]
        public async Task AddImageAsync_ValidDto_CallsAddImageAndSave()
        {
            // Arrange
            var dto = new AddImageDto { PropertyId = 1, File = "image.jpg" };
            var property = Property.Create("Test", "Addr", 100, "Code", 2020, 1);
            _mockRepo.Setup(r => r.GetByIdAsync(dto.PropertyId)).ReturnsAsync(property);

            // Act
            await _service.AddImageAsync(dto);

            // Assert
            _mockRepo.Verify(r => r.GetByIdAsync(dto.PropertyId), Times.Once);
            _mockRepo.Verify(r => r.AddImageAsync(It.Is<PropertyImage>(i => i.IdProperty == dto.PropertyId && i.File == dto.File)), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void AddImageAsync_PropertyNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var dto = new AddImageDto { PropertyId = 1, File = "image.jpg" };
            _mockRepo.Setup(r => r.GetByIdAsync(dto.PropertyId)).ReturnsAsync((Property?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(() => _service.AddImageAsync(dto));
            Assert.That(ex.Message, Contains.Substring("not found"));
        }

        [Test]
        public async Task ChangePriceAsync_ValidDto_CallsUpdateAndSaveWithTrace()
        {
            // Arrange
            var dto = new ChangePriceDto { Id = 1, NewPrice = 200 };
            var property = Property.Create("Test", "Addr", 100, "Code", 2020, 1);
            _mockRepo.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync(property);

            // Act
            await _service.ChangePriceAsync(dto);

            // Assert
            _mockRepo.Verify(r => r.GetByIdAsync(dto.Id), Times.Once);
            _mockRepo.Verify(r => r.AddTraceAsync(It.IsAny<PropertyTrace>()), Times.Once);
            _mockRepo.Verify(r => r.UpdateAsync(It.Is<Property>(p => p.Price == dto.NewPrice)), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void ChangePriceAsync_PropertyNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var dto = new ChangePriceDto { Id = 1, NewPrice = 200 };
            _mockRepo.Setup(r => r.GetByIdAsync(dto.Id)).ReturnsAsync((Property?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(() => _service.ChangePriceAsync(dto));
            Assert.That(ex.Message, Contains.Substring("not found"));
        }

        [Test]
        public async Task UpdateAsync_ValidDto_CallsUpdateAndSave()
        {
            // Arrange
            var id = 1;
            var dto = new UpdatePropertyDto { Name = "Updated", Address = "New Addr", Price = 150, CodeInternal = "NewCode", Year = 2021 };
            var property = Property.Create("Test", "Addr", 100, "Code", 2020, 1);
            var expectedDto = new PropertyDto { IdProperty = id, Name = dto.Name, Address = dto.Address, Price = dto.Price, CodeInternal = dto.CodeInternal, Year = dto.Year };
            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(property);
            _mockMapper.Setup(m => m.Map<PropertyDto>(It.IsAny<Property>())).Returns(expectedDto);

            // Act
            var result = await _service.UpdateAsync(id, dto);

            // Assert
            _mockRepo.Verify(r => r.GetByIdAsync(id), Times.Once);
            _mockRepo.Verify(r => r.UpdateAsync(It.Is<Property>(p =>
                p.Name == dto.Name &&
                p.Address == dto.Address &&
                p.Price == dto.Price &&
                p.CodeInternal == dto.CodeInternal &&
                p.Year == dto.Year)), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
            Assert.That(result.Name, Is.EqualTo(dto.Name));
        }

        [Test]
        public void UpdateAsync_PropertyNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var id = 1;
            var dto = new UpdatePropertyDto();
            _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((Property?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(id, dto));
            Assert.That(ex.Message, Contains.Substring("not found"));
        }

        [Test]
        public async Task ListAsync_WithFiltersAndPaging_ReturnsPagedResult()
        {
            // Arrange
            var filter = new PropertyFilter { MinPrice = 100m, MaxPrice = 500m, PageNumber = 1, PageSize = 5 };
            var mockPaged = new PagedResult<Property> { Items = new List<Property> { Property.Create("Test", "Addr", 200, "Code", 2020, 1) }, TotalCount = 10 };
            _mockRepo.Setup(r => r.ListPagedAsync(filter)).ReturnsAsync(mockPaged);
            _mockMapper.Setup(m => m.Map<IEnumerable<PropertyDto>>(It.IsAny<IEnumerable<Property>>())).Returns(new List<PropertyDto> { new PropertyDto() });

            // Act
            var result = await _service.ListAsync(filter);

            // Assert
            Assert.That(result.TotalCount, Is.EqualTo(10));
            Assert.That(result.Items.Count(), Is.EqualTo(1));
            _mockRepo.Verify(r => r.ListPagedAsync(filter), Times.Once);
        }

        [Test]
        public void ListAsync_InvalidFilter_ThrowsValidationException()
        {
            // Arrange
            var filter = new PropertyFilter { MinPrice = 500m, MaxPrice = 100m };

            // Act & Assert
            Assert.ThrowsAsync<ValidationException>(() => _service.ListAsync(filter));
        }

        [Test]
        public async Task GetPropertiesByOwnerIdAsync_ReturnsProperties()
        {
            // Arrange
            var ownerId = 1;
            var properties = new List<Property> { Property.Create("Test", "Addr", 100, "Code", 2020, ownerId) };
            _mockRepo.Setup(r => r.GetPropertiesByOwnerIdAsync(ownerId)).ReturnsAsync(properties);
            _mockMapper.Setup(m => m.Map<IEnumerable<PropertyDto>>(properties)).Returns(new List<PropertyDto> { new PropertyDto { IdProperty = 1 } });

            // Act
            var result = await _service.GetPropertiesByOwnerIdAsync(ownerId);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(1));
            _mockRepo.Verify(r => r.GetPropertiesByOwnerIdAsync(ownerId), Times.Once);
        }

        [Test]
        public async Task GetPropertyDetailsAsync_ReturnsDetails()
        {
            // Arrange
            var id = 1;
            var property = Property.Create("Test", "Addr", 100, "Code", 2020, 1);
            var expectedDto = new PropertyDetailsDto { IdProperty = id };
            _mockRepo.Setup(r => r.GetPropertyDetailsAsync(id)).ReturnsAsync(property);
            _mockMapper.Setup(m => m.Map<PropertyDetailsDto>(property)).Returns(expectedDto);

            // Act
            var result = await _service.GetPropertyDetailsAsync(id);

            // Assert
            Assert.That(result.IdProperty, Is.EqualTo(id));
            _mockRepo.Verify(r => r.GetPropertyDetailsAsync(id), Times.Once);
        }

        [Test]
        public async Task GetPropertyDetailsAsync_NotFound_ThrowsNotFoundException()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetPropertyDetailsAsync(1)).ReturnsAsync((Property?)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<NotFoundException>(() => _service.GetPropertyDetailsAsync(1));
            Assert.That(ex.Message, Contains.Substring("not found"));
        }
    }
}