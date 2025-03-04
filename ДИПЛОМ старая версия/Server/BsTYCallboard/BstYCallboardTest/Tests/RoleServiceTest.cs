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
    public class RoleServiceTests
    {
        private readonly RoleService _roleService;
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<DbSet<Role>> _mockSet;

        public RoleServiceTests()
        {
            _mockContext = new Mock<ApplicationDbContext>();
            _mockSet = new Mock<DbSet<Role>>();
            _roleService = new RoleService(_mockContext.Object);
        }

        [Fact]
        public async Task CreateRoleAsync_ValidRole_ReturnsRole()
        {
            // Arrange
            var role = new Role { id = 1, name = "Administrator" };

            _mockContext.Setup(m => m.Roles.Add(It.IsAny<Role>())).Verifiable();
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _roleService.CreateRoleAsync(role);

            // Assert
            Assert.Equal(role, result);
            _mockContext.Verify(m => m.Roles.Add(It.IsAny<Role>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task GetAllRolesAsync_ReturnsAllRoles()
        {
            // Arrange
            var roles = new List<Role>
            {
                new Role { id = 1, name = "Administrator" },
                new Role { id = 2, name = "User" }
            };

            _mockSet.As<IQueryable<Role>>().Setup(m => m.Provider).Returns(roles.AsQueryable().Provider);
            _mockSet.As<IQueryable<Role>>().Setup(m => m.Expression).Returns(roles.AsQueryable().Expression);
            _mockSet.As<IQueryable<Role>>().Setup(m => m.ElementType).Returns(roles.AsQueryable().ElementType);
            _mockSet.As<IQueryable<Role>>().Setup(m => m.GetEnumerator()).Returns(roles.GetEnumerator());

            _mockContext.Setup(m => m.Roles).Returns(_mockSet.Object);

            // Act
            var result = await _roleService.GetAllRolesAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Administrator", result[0].name);
            Assert.Equal("User", result[1].name);
        }

        [Fact]
        public async Task GetRoleByIdAsync_RoleExists_ReturnsRole()
        {
            // Arrange
            var role = new Role { id = 1, name = "Administrator" };

            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(role);
            _mockContext.Setup(m => m.Roles).Returns(_mockSet.Object);

            // Act
            var result = await _roleService.GetRoleByIdAsync(1);

            // Assert
            Assert.Equal(role, result);
        }

        [Fact]
        public async Task GetRoleByIdAsync_RoleDoesNotExist_ReturnsNull()
        {
            // Arrange
            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync((Role)null);
            _mockContext.Setup(m => m.Roles).Returns(_mockSet.Object);

            // Act
            var result = await _roleService.GetRoleByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateRoleAsync_RoleExists_ReturnsTrue()
        {
            // Arrange
            var role = new Role { id = 1, name = "Administrator" };
            var updatedRole = new Role { id = 1, name = "Moderator" };

            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(role);
            _mockContext.Setup(m => m.Roles).Returns(_mockSet.Object);
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _roleService.UpdateRoleAsync(updatedRole);

            // Assert
            Assert.True(result);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateRoleAsync_RoleDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var updatedRole = new Role { id = 1, name = "Moderator" };

            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync((Role)null);
            _mockContext.Setup(m => m.Roles).Returns(_mockSet.Object);

            // Act
            var result = await _roleService.UpdateRoleAsync(updatedRole);

            // Assert
            Assert.False(result);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task DeleteRoleAsync_RoleExists_ReturnsTrue()
        {
            // Arrange
            var role = new Role { id = 1, name = "Administrator" };

            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(role);
            _mockContext.Setup(m => m.Roles).Returns(_mockSet.Object);
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _roleService.DeleteRoleAsync(1);

            // Assert
            Assert.True(result);
            _mockContext.Verify(m => m.Roles.Remove(It.IsAny<Role>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteRoleAsync_RoleDoesNotExist_ReturnsFalse()
        {
            // Arrange
            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync((Role)null);
            _mockContext.Setup(m => m.Roles).Returns(_mockSet.Object);

            // Act
            var result = await _roleService.DeleteRoleAsync(1);

            // Assert
            Assert.False(result);
            _mockContext.Verify(m => m.Roles.Remove(It.IsAny<Role>()), Times.Never);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
        }
    }
}
