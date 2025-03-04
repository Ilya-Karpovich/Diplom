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
    public class TeacherServiceTests
    {
        private readonly TeacherService _teacherService;
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<DbSet<Teacher>> _mockSet;

        public TeacherServiceTests()
        {
            _mockContext = new Mock<ApplicationDbContext>();
            _mockSet = new Mock<DbSet<Teacher>>();
            _teacherService = new TeacherService(_mockContext.Object);
        }

        [Fact]
        public async Task CreateTeacherAsync_ValidTeacher_ReturnsTeacher()
        {
            // Arrange
            var teacher = new Teacher { id = 1, name = "John Doe", email = "john.doe@example.com", roleId = 1, facultyId = 1 };

            _mockContext.Setup(m => m.Teachers.Add(It.IsAny<Teacher>())).Verifiable();
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _teacherService.CreateTeacherAsync(teacher);

            // Assert
            Assert.Equal(teacher, result);
            _mockContext.Verify(m => m.Teachers.Add(It.IsAny<Teacher>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task GetAllTeachersAsync_ReturnsAllTeachers()
        {
            // Arrange
            var teachers = new List<Teacher>
            {
                new Teacher { id = 1, name = "John Doe", email = "john.doe@example.com", roleId = 1, facultyId = 1 },
                new Teacher { id = 2, name = "Jane Doe", email = "jane.doe@example.com", roleId = 2, facultyId = 2 }
            };

            _mockSet.As<IQueryable<Teacher>>().Setup(m => m.Provider).Returns(teachers.AsQueryable().Provider);
            _mockSet.As<IQueryable<Teacher>>().Setup(m => m.Expression).Returns(teachers.AsQueryable().Expression);
            _mockSet.As<IQueryable<Teacher>>().Setup(m => m.ElementType).Returns(teachers.AsQueryable().ElementType);
            _mockSet.As<IQueryable<Teacher>>().Setup(m => m.GetEnumerator()).Returns(teachers.GetEnumerator());

            _mockContext.Setup(m => m.Teachers).Returns(_mockSet.Object);

            // Act
            var result = await _teacherService.GetAllTeachersAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("John Doe", result[0].name);
            Assert.Equal("Jane Doe", result[1].name);
        }

        [Fact]
        public async Task GetTeacherByIdAsync_TeacherExists_ReturnsTeacher()
        {
            // Arrange
            var teacher = new Teacher { id = 1, name = "John Doe", email = "john.doe@example.com", roleId = 1, facultyId = 1 };

            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(teacher);
            _mockContext.Setup(m => m.Teachers).Returns(_mockSet.Object);

            // Act
            var result = await _teacherService.GetTeacherByIdAsync(1);

            // Assert
            Assert.Equal(teacher, result);
        }

        [Fact]
        public async Task GetTeacherByIdAsync_TeacherDoesNotExist_ReturnsNull()
        {
            // Arrange
            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync((Teacher)null);
            _mockContext.Setup(m => m.Teachers).Returns(_mockSet.Object);

            // Act
            var result = await _teacherService.GetTeacherByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateTeacherAsync_TeacherExists_ReturnsTrue()
        {
            // Arrange
            var teacher = new Teacher { id = 1, name = "John Doe", email = "john.doe@example.com", roleId = 1, facultyId = 1 };
            var updatedTeacher = new Teacher { id = 1, name = "Jane Doe", email = "jane.doe@example.com", roleId = 1, facultyId = 1 };

            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(teacher);
            _mockContext.Setup(m => m.Teachers).Returns(_mockSet.Object);
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _teacherService.UpdateTeacherAsync(updatedTeacher);

            // Assert
            Assert.True(result);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateTeacherAsync_TeacherDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var updatedTeacher = new Teacher { id = 1, name = "Jane Doe", email = "jane.doe@example.com", roleId = 1, facultyId = 1 };

            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync((Teacher)null);
            _mockContext.Setup(m => m.Teachers).Returns(_mockSet.Object);

            // Act
            var result = await _teacherService.UpdateTeacherAsync(updatedTeacher);

            // Assert
            Assert.False(result);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task DeleteTeacherAsync_TeacherExists_ReturnsTrue()
        {
            // Arrange
            var teacher = new Teacher { id = 1, name = "John Doe", email = "john.doe@example.com", roleId = 1, facultyId = 1 };

            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(teacher);
            _mockContext.Setup(m => m.Teachers).Returns(_mockSet.Object);
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _teacherService.DeleteTeacherAsync(1);

            // Assert
            Assert.True(result);
            _mockContext.Verify(m => m.Teachers.Remove(It.IsAny<Teacher>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteTeacherAsync_TeacherDoesNotExist_ReturnsFalse()
        {
            // Arrange
            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync((Teacher)null);
            _mockContext.Setup(m => m.Teachers).Returns(_mockSet.Object);

            // Act
            var result = await _teacherService.DeleteTeacherAsync(1);

            // Assert
            Assert.False(result);
            _mockContext.Verify(m => m.Teachers.Remove(It.IsAny<Teacher>()), Times.Never);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
        }
    }
}
