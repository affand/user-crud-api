using FluentAssertions;
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

        var created = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var user = created.Value.Should().BeOfType<User>().Subject;

        created.ActionName.Should().Be(nameof(UsersController.GetById));
        user.Id.Should().Be(1);
        user.Name.Should().Be("Ada Lovelace");
        user.Email.Should().Be("ada@example.com");
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

        result.Result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public void GetAll_ReturnsUsers()
    {
        repository.Create("Ada Lovelace", "ada@example.com");
        repository.Create("Grace Hopper", "grace@example.com");

        var result = controller.GetAll();

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var users = ok.Value.Should().BeAssignableTo<IReadOnlyCollection<User>>().Subject;

        users.Should().HaveCount(2);
    }

    [Fact]
    public void GetById_WhenUserExists_ReturnsUser()
    {
        var created = repository.Create("Ada Lovelace", "ada@example.com");

        var result = controller.GetById(created.Id);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var user = ok.Value.Should().BeOfType<User>().Subject;

        user.Id.Should().Be(created.Id);
    }

    [Fact]
    public void GetById_WhenUserDoesNotExist_ReturnsNotFound()
    {
        var result = controller.GetById(999);

        result.Result.Should().BeOfType<NotFoundResult>();
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

        result.Should().BeOfType<NoContentResult>();

        var updated = repository.GetById(created.Id);
        updated.Should().NotBeNull();
        updated!.Name.Should().Be("Ada Byron");
        updated.Email.Should().Be("ada.byron@example.com");
    }

    [Fact]
    public void Update_WhenUserDoesNotExist_ReturnsNotFound()
    {
        var result = controller.Update(999, new UpdateUserRequest
        {
            Name = "Missing User",
            Email = "missing@example.com"
        });

        result.Should().BeOfType<NotFoundResult>();
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

        result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public void Delete_WhenUserExists_RemovesUserAndReturnsNoContent()
    {
        var created = repository.Create("Ada Lovelace", "ada@example.com");

        var result = controller.Delete(created.Id);

        result.Should().BeOfType<NoContentResult>();
        repository.GetById(created.Id).Should().BeNull();
    }

    [Fact]
    public void Delete_WhenUserDoesNotExist_ReturnsNotFound()
    {
        var result = controller.Delete(999);

        result.Should().BeOfType<NotFoundResult>();
    }
}
