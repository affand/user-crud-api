using Microsoft.AspNetCore.Mvc;
using UserCrudApi.Api.Controllers;
using UserCrudApi.Api.Dtos;
using UserCrudApi.Api.Models;
using UserCrudApi.Api.Repositories;

namespace UserCrudApi.Tests;

public sealed class UsersControllerTests
{
    private readonly InMemoryUserRepository repository = new();
    private readonly UsersController controller;

    public UsersControllerTests()
    {
        controller = new UsersController(repository);
    }

    [Fact]
    public void Create_ReturnsCreatedUser()
    {
        var result = controller.Create(new CreateUserRequest
        {
            Name = "Ada Lovelace",
            Email = "ADA@example.com"
        });

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var user = Assert.IsType<User>(created.Value);

        Assert.Equal(nameof(UsersController.GetById), created.ActionName);
        Assert.Equal(1, user.Id);
        Assert.Equal("Ada Lovelace", user.Name);
        Assert.Equal("ada@example.com", user.Email);
    }

    [Fact]
    public void Create_WithDuplicateEmail_ReturnsConflict()
    {
        repository.Create("Ada Lovelace", "ada@example.com");

        var result = controller.Create(new CreateUserRequest
        {
            Name = "Another Ada",
            Email = "ADA@example.com"
        });

        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public void GetAll_ReturnsUsers()
    {
        repository.Create("Ada Lovelace", "ada@example.com");
        repository.Create("Grace Hopper", "grace@example.com");

        var result = controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var users = Assert.IsAssignableFrom<IReadOnlyCollection<User>>(ok.Value);

        Assert.Equal(2, users.Count);
    }

    [Fact]
    public void GetById_WhenUserExists_ReturnsUser()
    {
        var created = repository.Create("Ada Lovelace", "ada@example.com");

        var result = controller.GetById(created.Id);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var user = Assert.IsType<User>(ok.Value);

        Assert.Equal(created.Id, user.Id);
    }

    [Fact]
    public void GetById_WhenUserDoesNotExist_ReturnsNotFound()
    {
        var result = controller.GetById(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public void Update_WhenUserExists_UpdatesAndReturnsNoContent()
    {
        var created = repository.Create("Ada Lovelace", "ada@example.com");

        var result = controller.Update(created.Id, new UpdateUserRequest
        {
            Name = "Ada Byron",
            Email = "ada.byron@example.com"
        });

        Assert.IsType<NoContentResult>(result);

        var updated = repository.GetById(created.Id);
        Assert.NotNull(updated);
        Assert.Equal("Ada Byron", updated.Name);
        Assert.Equal("ada.byron@example.com", updated.Email);
    }

    [Fact]
    public void Update_WhenUserDoesNotExist_ReturnsNotFound()
    {
        var result = controller.Update(999, new UpdateUserRequest
        {
            Name = "Missing User",
            Email = "missing@example.com"
        });

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Update_WithDuplicateEmail_ReturnsConflict()
    {
        repository.Create("Ada Lovelace", "ada@example.com");
        var grace = repository.Create("Grace Hopper", "grace@example.com");

        var result = controller.Update(grace.Id, new UpdateUserRequest
        {
            Name = "Grace Hopper",
            Email = "ada@example.com"
        });

        Assert.IsType<ConflictObjectResult>(result);
    }

    [Fact]
    public void Delete_WhenUserExists_RemovesUserAndReturnsNoContent()
    {
        var created = repository.Create("Ada Lovelace", "ada@example.com");

        var result = controller.Delete(created.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.Null(repository.GetById(created.Id));
    }

    [Fact]
    public void Delete_WhenUserDoesNotExist_ReturnsNotFound()
    {
        var result = controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
