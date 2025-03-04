using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BsTYCallboard.Data;
using BsTYCallboard.Entity;
using BsTYCallboard.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BsTYCallboard.Tests
{
    public class SpecialityServiceTests
    {
        private readonly SpecialityService _specialityService;
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<DbSet<Speciality>> _mockSet;

        public SpecialityServiceTests()
        {
            _mockContext = new Mock<ApplicationDbContext>();
            _mockSet = new Mock<DbSet<Speciality>>();
            _specialityService = new SpecialityService(_mockContext.Object);
        }

        [Fact]
        public async Task CreateSpecialityAsync_ValidSpeciality_ReturnsSpeciality()
        {
            // Arrange
            var speciality = new Speciality { id = 1, name = "Computer Science", description = "Study of computation" };

            _mockContext.Setup(m => m.Specialities.Add(It.IsAny<Speciality>())).Verifiable();
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _specialityService.CreateSpecialityAsync(speciality);

            // Assert
            Assert.Equal(speciality, result);
            _mockContext.Verify(m => m.Specialities.Add(It.IsAny<Speciality>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task GetAllSpecialitiesAsync_ReturnsAllSpecialities()
        {
            // Arrange
            var specialities = new List<Speciality>
            {
                new Speciality { id = 1, name = "Computer Science", description = "Study of computation" },
                new Speciality { id = 2, name = "Information Technology", description = "Study of IT" }
            };

            _mockSet.As<IQueryable<Speciality>>().Setup(m => m.Provider).Returns(specialities.AsQueryable().Provider);
            _mockSet.As<IQueryable<Speciality>>().Setup(m => m.Expression).Returns(specialities.AsQueryable().Expression);
            _mockSet.As<IQueryable<Speciality>>().Setup(m => m.ElementType).Returns(specialities.AsQueryable().ElementType);
            _mockSet.As<IQueryable<Speciality>>().Setup(m => m.GetEnumerator()).Returns(specialities.GetEnumerator());

            _mockContext.Setup(m => m.Specialities).Returns(_mockSet.Object);

            // Act
            var result = await _specialityService.GetAllSpecialitiesAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Computer Science", result[0].name);
            Assert.Equal("Information Technology", result[1].name);
        }

        [Fact]
        public async Task GetSpecialityByIdAsync_SpecialityExists_ReturnsSpeciality()
        {
            // Arrange
            var speciality = new Speciality { id = 1, name = "Computer Science", description = "Study of computation" };

            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(speciality);
            _mockContext.Setup(m => m.Specialities).Returns(_mockSet.Object);

            // Act
            var result = await _specialityService.GetSpecialityByIdAsync(1);

            // Assert
            Assert.Equal(speciality, result);
        }

        [Fact]
        public async Task GetSpecialityByIdAsync_SpecialityDoesNotExist_ReturnsNull()
        {
            // Arrange
            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync((Speciality)null);
            _mockContext.Setup(m => m.Specialities).Returns(_mockSet.Object);

            // Act
            var result = await _specialityService.GetSpecialityByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateSpecialityAsync_SpecialityExists_ReturnsTrue()
        {
            // Arrange
            var speciality = new Speciality { id = 1, name = "Computer Science", description = "Study of computation" };
            var updatedSpeciality = new Speciality { id = 1, name = "Information Technology", description = "Study of IT" };

            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(speciality);
            _mockContext.Setup(m => m.Specialities).Returns(_mockSet.Object);
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _specialityService.UpdateSpecialityAsync(updatedSpeciality);

            // Assert
            Assert.True(result);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateSpecialityAsync_SpecialityDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var updatedSpeciality = new Speciality { id = 1, name = "Information Technology", description = "Study of IT" };

            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync((Speciality)null);
            _mockContext.Setup(m => m.Specialities).Returns(_mockSet.Object);

            // Act
            var result = await _specialityService.UpdateSpecialityAsync(updatedSpeciality);

            // Assert
            Assert.False(result);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task DeleteSpecialityAsync_SpecialityExists_ReturnsTrue()
        {
            // Arrange
            var speciality = new Speciality { id = 1, name = "Computer Science", description = "Study of computation" };

            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(speciality);
            _mockContext.Setup(m => m.Specialities).Returns(_mockSet.Object);
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _specialityService.DeleteSpecialityAsync(1);

            // Assert
            Assert.True(result);
            _mockContext.Verify(m => m.Specialities.Remove(It.IsAny<Speciality>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteSpecialityAsync_SpecialityDoesNotExist_ReturnsFalse()
        {
            // Arrange
            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync((Speciality)null);
            _mockContext.Setup(m => m.Specialities).Returns(_mockSet.Object);

            // Act
            var result = await _specialityService.DeleteSpecialityAsync(1);

            // Assert
            Assert.False(result);
            _mockContext.Verify(m => m.Specialities.Remove(It.IsAny<Speciality>()), Times.Never);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
        }
    }
}
