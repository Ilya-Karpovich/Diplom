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
    public class FacultyServiceTests
    {
        private readonly FacultyService _facultyService;
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<DbSet<Faculty>> _mockSet;

        public FacultyServiceTests()
        {
            _mockContext = new Mock<ApplicationDbContext>();
            _mockSet = new Mock<DbSet<Faculty>>();
            _facultyService = new FacultyService(_mockContext.Object);
        }

        [Fact]
        public async Task CreateFacultyAsync_ValidFaculty_ReturnsFaculty()
        {
            // Arrange
            var faculty = new Faculty { id = 1, name = "Faculty of Computer Science", description = "Description of faculty" };

            _mockContext.Setup(m => m.Faculties.Add(It.IsAny<Faculty>())).Verifiable();
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _facultyService.CreateFacultyAsync(faculty);

            // Assert
            Assert.Equal(faculty, result);
            _mockContext.Verify(m => m.Faculties.Add(It.IsAny<Faculty>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task GetAllFacultiesAsync_ReturnsAllFaculties()
        {
            // Arrange
            var faculties = new List<Faculty>
            {
                new Faculty { id = 1, name = "Faculty of Computer Science", description = "Description of faculty" },
                new Faculty { id = 2, name = "Faculty of Information Technology", description = "Description of IT faculty" }
            };

            _mockSet.As<IQueryable<Faculty>>().Setup(m => m.Provider).Returns(faculties.AsQueryable().Provider);
            _mockSet.As<IQueryable<Faculty>>().Setup(m => m.Expression).Returns(faculties.AsQueryable().Expression);
            _mockSet.As<IQueryable<Faculty>>().Setup(m => m.ElementType).Returns(faculties.AsQueryable().ElementType);
            _mockSet.As<IQueryable<Faculty>>().Setup(m => m.GetEnumerator()).Returns(faculties.GetEnumerator());

            _mockContext.Setup(m => m.Faculties).Returns(_mockSet.Object);

            // Act
            var result = await _facultyService.GetAllFacultiesAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Faculty of Computer Science", result[0].name);
            Assert.Equal("Faculty of Information Technology", result[1].name);
        }

        [Fact]
        public async Task GetFacultyByIdAsync_FacultyExists_ReturnsFaculty()
        {
            // Arrange
            var faculty = new Faculty { id = 1, name = "Faculty of Computer Science", description = "Description of faculty" };

            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(faculty);
            _mockContext.Setup(m => m.Faculties).Returns(_mockSet.Object);

            // Act
            var result = await _facultyService.GetFacultyByIdAsync(1);

            // Assert
            Assert.Equal(faculty, result);
        }

        [Fact]
        public async Task GetFacultyByIdAsync_FacultyDoesNotExist_ReturnsNull()
        {
            // Arrange
            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync((Faculty)null);
            _mockContext.Setup(m => m.Faculties).Returns(_mockSet.Object);

            // Act
            var result = await _facultyService.GetFacultyByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateFacultyAsync_FacultyExists_ReturnsTrue()
        {
            // Arrange
            var faculty = new Faculty { id = 1, name = "Faculty of Computer Science", description = "Description of faculty" };
            var updatedFaculty = new Faculty { id = 1, name = "Faculty of Information Technology", description = "Updated description of IT faculty" };

            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(faculty);
            _mockContext.Setup(m => m.Faculties).Returns(_mockSet.Object);
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _facultyService.UpdateFacultyAsync(updatedFaculty);

            // Assert
            Assert.True(result);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateFacultyAsync_FacultyDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var updatedFaculty = new Faculty { id = 1, name = "Faculty of Information Technology", description = "Updated description of IT faculty" };

            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync((Faculty)null);
            _mockContext.Setup(m => m.Faculties).Returns(_mockSet.Object);

            // Act
            var result = await _facultyService.UpdateFacultyAsync(updatedFaculty);

            // Assert
            Assert.False(result);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task DeleteFacultyAsync_FacultyExists_ReturnsTrue()
        {
            // Arrange
            var faculty = new Faculty { id = 1, name = "Faculty of Computer Science", description = "Description of faculty" };

            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(faculty);
            _mockContext.Setup(m => m.Faculties).Returns(_mockSet.Object);
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _facultyService.DeleteFacultyAsync(1);

            // Assert
            Assert.True(result);
            _mockContext.Verify(m => m.Faculties.Remove(It.IsAny<Faculty>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteFacultyAsync_FacultyDoesNotExist_ReturnsFalse()
        {
            // Arrange
            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync((Faculty)null);
            _mockContext.Setup(m => m.Faculties).Returns(_mockSet.Object);

            // Act
            var result = await _facultyService.DeleteFacultyAsync(1);

            // Assert
            Assert.False(result);
            _mockContext.Verify(m => m.Faculties.Remove(It.IsAny<Faculty>()), Times.Never);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
        }
    }
}
