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
    public class UserServiceTests
    {
        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();

            // Добавьте необходимые данные в In-Memory базу данных, если требуется
            if (!await context.Roles.AnyAsync())
            {
                context.Roles.AddRange(new List<Role>
                {
                    new Role { id = 1, name = "Admin" },
                    new Role { id = 2, name = "User" }
                });

                await context.SaveChangesAsync();
            }

            if (!await context.Faculties.AnyAsync())
            {
                context.Faculties.Add(new Faculty { id = 1, name = "Faculty of IT", description = "Description" });
                await context.SaveChangesAsync();
            }

            if (!await context.Specialities.AnyAsync())
            {
                context.Specialities.Add(new Speciality { id = 1, name = "Computer Science", faculty = await context.Faculties.FirstAsync() });
                await context.SaveChangesAsync();
            }

            return context;
        }

        [Fact]
        public async Task CreateUserAsync_ShouldCreateUser()
        {
            // Arrange
            var context = await GetDbContext();
            var userService = new UserService(context);

            var newUser = new User
            {
                name = "Ivan Ivanov",
                email = "ivan.ivanov@example.com",
                roleId = 1,
                specialityId = 1,
                facultyId = 1
            };
            newUser.SetPassword("password123");

            // Act
            var createdUser = await userService.CreateUserAsync(newUser);

            // Assert
            Assert.NotNull(createdUser);
            Assert.Equal(newUser.name, createdUser.name);
            Assert.Equal(newUser.email, createdUser.email);
            Assert.Equal(newUser.roleId, createdUser.roleId);
            Assert.Equal(newUser.specialityId, createdUser.specialityId);
            Assert.Equal(newUser.facultyId, createdUser.facultyId);
            Assert.True(createdUser.VerifyPassword("password123"));
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var context = await GetDbContext();
            var userService = new UserService(context);

            var users = new List<User>
            {
                new User { name = "User1", email = "user1@example.com", roleId = 1, specialityId = 1, facultyId = 1 },
                new User { name = "User2", email = "user2@example.com", roleId = 2, specialityId = 1, facultyId = 1 }
            };

            users.ForEach(u => u.SetPassword("password123"));
            context.Users.AddRange(users);
            await context.SaveChangesAsync();

            // Act
            var allUsers = await userService.GetAllUsersAsync();

            // Assert
            Assert.Equal(2, allUsers.Count);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser()
        {
            // Arrange
            var context = await GetDbContext();
            var userService = new UserService(context);

            var user = new User
            {
                name = "Ivan Ivanov",
                email = "ivan.ivanov@example.com",
                roleId = 1,
                specialityId = 1,
                facultyId = 1
            };
            user.SetPassword("password123");
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            var retrievedUser = await userService.GetUserByIdAsync(user.id);

            // Assert
            Assert.NotNull(retrievedUser);
            Assert.Equal(user.name, retrievedUser.name);
            Assert.Equal(user.email, retrievedUser.email);
            Assert.Equal(user.roleId, retrievedUser.roleId);
            Assert.Equal(user.specialityId, retrievedUser.specialityId);
            Assert.Equal(user.facultyId, retrievedUser.facultyId);
            Assert.True(retrievedUser.VerifyPassword("password123"));
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateUser()
        {
            // Arrange
            var context = await GetDbContext();
            var userService = new UserService(context);

            var user = new User
            {
                name = "Ivan Ivanov",
                email = "ivan.ivanov@example.com",
                roleId = 1,
                specialityId = 1,
                facultyId = 1
            };
            user.SetPassword("password123");
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            user.name = "Ivan Petrov";
            user.email = "ivan.petrov@example.com";
            var updateResult = await userService.UpdateUserAsync(user);
            var updatedUser = await userService.GetUserByIdAsync(user.id);

            // Assert
            Assert.True(updateResult);
            Assert.Equal("Ivan Petrov", updatedUser.name);
            Assert.Equal("ivan.petrov@example.com", updatedUser.email);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldDeleteUser()
        {
            // Arrange
            var context = await GetDbContext();
            var userService = new UserService(context);

            var user = new User
            {
                name = "Ivan Ivanov",
                email = "ivan.ivanov@example.com",
                roleId = 1,
                specialityId = 1,
                facultyId = 1
            };
            user.SetPassword("password123");
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            var deleteResult = await userService.DeleteUserAsync(user.id);
            var deletedUser = await userService.GetUserByIdAsync(user.id);

            // Assert
            Assert.True(deleteResult);
            Assert.Null(deletedUser);
        }
    }
}
