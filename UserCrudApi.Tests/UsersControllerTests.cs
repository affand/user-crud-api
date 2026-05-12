using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using UserCrudApi.Api.Controllers;
using UserCrudApi.Api.Dtos;
using UserCrudApi.Api.Models;
using UserCrudApi.Api.Repositories;

namespace UserCrudApi.Tests;

public sealed class UsersControllerTests
{
    private readonly IUserRepository repository = Substitute.For<IUserRepository>();
    private readonly UsersController controller;

    public UsersControllerTests()
    {
        controller = new UsersController(repository);
    }

    [Fact]
    public void Create_ReturnsCreatedUser()
    {
        var request = new CreateUserRequest
        {
            Name = "Ada Lovelace",
            Email = "ADA@example.com"
        };

        var expectedUser = new User
        {
            Id = 1,
            Name = "Ada Lovelace",
            Email = "ada@example.com"
        };

        repository.EmailExists(request.Email).Returns(false);
        repository.Create(request.Name, request.Email).Returns(expectedUser);

        var result = controller.Create(request);

        var created = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var user = created.Value.Should().BeOfType<User>().Subject;

        created.ActionName.Should().Be(nameof(UsersController.GetById));
        created.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(expectedUser.Id);
        user.Should().BeEquivalentTo(expectedUser);
        repository.Received(1).Create(request.Name, request.Email);
    }

    [Fact]
    public void Create_WithDuplicateEmail_ReturnsConflict()
    {
        var request = new CreateUserRequest
        {
            Name = "Another Ada",
            Email = "ADA@example.com"
        };

        repository.EmailExists(request.Email).Returns(true);

        var result = controller.Create(request);

        result.Result.Should().BeOfType<ConflictObjectResult>();
        repository.DidNotReceive().Create(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public void GetAll_ReturnsUsers()
    {
        var expectedUsers = new[]
        {
            new User { Id = 1, Name = "Ada Lovelace", Email = "ada@example.com" },
            new User { Id = 2, Name = "Grace Hopper", Email = "grace@example.com" }
        };

        repository.GetAll().Returns(expectedUsers);

        var result = controller.GetAll();

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var users = ok.Value.Should().BeAssignableTo<IReadOnlyCollection<User>>().Subject;

        users.Should().BeEquivalentTo(expectedUsers);
    }

    [Fact]
    public void GetById_WhenUserExists_ReturnsUser()
    {
        var expectedUser = new User
        {
            Id = 1,
            Name = "Ada Lovelace",
            Email = "ada@example.com"
        };

        repository.GetById(expectedUser.Id).Returns(expectedUser);

        var result = controller.GetById(expectedUser.Id);

        var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var user = ok.Value.Should().BeOfType<User>().Subject;

        user.Should().BeEquivalentTo(expectedUser);
    }

    [Fact]
    public void GetById_WhenUserDoesNotExist_ReturnsNotFound()
    {
        repository.GetById(999).Returns((User?)null);

        var result = controller.GetById(999);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void Update_WhenUserExists_UpdatesAndReturnsNoContent()
    {
        var request = new UpdateUserRequest
        {
            Name = "Ada Byron",
            Email = "ada.byron@example.com"
        };

        repository.EmailExists(request.Email, 1).Returns(false);
        repository.Update(1, request.Name, request.Email).Returns(true);

        var result = controller.Update(1, request);

        result.Should().BeOfType<NoContentResult>();
        repository.Received(1).Update(1, request.Name, request.Email);
    }

    [Fact]
    public void Update_WhenUserDoesNotExist_ReturnsNotFound()
    {
        var request = new UpdateUserRequest
        {
            Name = "Missing User",
            Email = "missing@example.com"
        };

        repository.EmailExists(request.Email, 999).Returns(false);
        repository.Update(999, request.Name, request.Email).Returns(false);

        var result = controller.Update(999, request);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void Update_WithDuplicateEmail_ReturnsConflict()
    {
        var request = new UpdateUserRequest
        {
            Name = "Grace Hopper",
            Email = "ada@example.com"
        };

        repository.EmailExists(request.Email, 2).Returns(true);

        var result = controller.Update(2, request);

        result.Should().BeOfType<ConflictObjectResult>();
        repository.DidNotReceive().Update(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public void Delete_WhenUserExists_RemovesUserAndReturnsNoContent()
    {
        repository.Delete(1).Returns(true);

        var result = controller.Delete(1);

        result.Should().BeOfType<NoContentResult>();
        repository.Received(1).Delete(1);
    }

    [Fact]
    public void Delete_WhenUserDoesNotExist_ReturnsNotFound()
    {
        repository.Delete(999).Returns(false);

        var result = controller.Delete(999);

        result.Should().BeOfType<NotFoundResult>();
    }
}
