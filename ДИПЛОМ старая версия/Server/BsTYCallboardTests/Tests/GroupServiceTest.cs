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
    public class GroupServiceTests
    {
        private readonly GroupService _groupService;
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<DbSet<Group>> _mockSet;

        public GroupServiceTests()
        {
            _mockContext = new Mock<ApplicationDbContext>();
            _mockSet = new Mock<DbSet<Group>>();
            _groupService = new GroupService(_mockContext.Object);
        }

        [Fact]
        public async Task CreateGroupAsync_ValidGroup_ReturnsGroup()
        {
            // Arrange
            var group = new Group { id = 1, name = "IT Group", facultyId = 1, specialityId = 2, teacherId = 3 };

            _mockContext.Setup(m => m.Groups.Add(It.IsAny<Group>())).Verifiable();
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _groupService.CreateGroupAsync(group);

            // Assert
            Assert.Equal(group, result);
            _mockContext.Verify(m => m.Groups.Add(It.IsAny<Group>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task GetAllGroupsAsync_ReturnsAllGroups()
        {
            // Arrange
            var groups = new List<Group>
            {
                new Group { id = 1, name = "IT Group", facultyId = 1, specialityId = 2, teacherId = 3 },
                new Group { id = 2, name = "CS Group", facultyId = 1, specialityId = 3, teacherId = 4 }
            };

            _mockSet.As<IQueryable<Group>>().Setup(m => m.Provider).Returns(groups.AsQueryable().Provider);
            _mockSet.As<IQueryable<Group>>().Setup(m => m.Expression).Returns(groups.AsQueryable().Expression);
            _mockSet.As<IQueryable<Group>>().Setup(m => m.ElementType).Returns(groups.AsQueryable().ElementType);
            _mockSet.As<IQueryable<Group>>().Setup(m => m.GetEnumerator()).Returns(groups.GetEnumerator());

            _mockContext.Setup(m => m.Groups).Returns(_mockSet.Object);

            // Act
            var result = await _groupService.GetAllGroupsAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("IT Group", result[0].name);
            Assert.Equal("CS Group", result[1].name);
        }

        [Fact]
        public async Task GetGroupByIdAsync_GroupExists_ReturnsGroup()
        {
            // Arrange
            var group = new Group { id = 1, name = "IT Group", facultyId = 1, specialityId = 2, teacherId = 3 };

            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(group);
            _mockContext.Setup(m => m.Groups).Returns(_mockSet.Object);

            // Act
            var result = await _groupService.GetGroupByIdAsync(1);

            // Assert
            Assert.Equal(group, result);
        }

        [Fact]
        public async Task GetGroupByIdAsync_GroupDoesNotExist_ReturnsNull()
        {
            // Arrange
            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync((Group)null);
            _mockContext.Setup(m => m.Groups).Returns(_mockSet.Object);

            // Act
            var result = await _groupService.GetGroupByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateGroupAsync_GroupExists_ReturnsTrue()
        {
            // Arrange
            var group = new Group { id = 1, name = "IT Group", facultyId = 1, specialityId = 2, teacherId = 3 };
            var updatedGroup = new Group { id = 1, name = "New IT Group", facultyId = 1, specialityId = 2, teacherId = 3 };

            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(group);
            _mockContext.Setup(m => m.Groups).Returns(_mockSet.Object);
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _groupService.UpdateGroupAsync(updatedGroup);

            // Assert
            Assert.True(result);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateGroupAsync_GroupDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var updatedGroup = new Group { id = 1, name = "New IT Group", facultyId = 1, specialityId = 2, teacherId = 3 };

            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync((Group)null);
            _mockContext.Setup(m => m.Groups).Returns(_mockSet.Object);

            // Act
            var result = await _groupService.UpdateGroupAsync(updatedGroup);

            // Assert
            Assert.False(result);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task DeleteGroupAsync_GroupExists_ReturnsTrue()
        {
            // Arrange
            var group = new Group { id = 1, name = "IT Group", facultyId = 1, specialityId = 2, teacherId = 3 };

            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(group);
            _mockContext.Setup(m => m.Groups).Returns(_mockSet.Object);
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _groupService.DeleteGroupAsync(1);

            // Assert
            Assert.True(result);
            _mockContext.Verify(m => m.Groups.Remove(It.IsAny<Group>()), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteGroupAsync_GroupDoesNotExist_ReturnsFalse()
        {
            // Arrange
            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync((Group)null);
            _mockContext.Setup(m => m.Groups).Returns(_mockSet.Object);

            // Act
            var result = await _groupService.DeleteGroupAsync(1);

            // Assert
            Assert.False(result);
            _mockContext.Verify(m => m.Groups.Remove(It.IsAny<Group>()), Times.Never);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
        }
    }
}
