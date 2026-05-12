using Microsoft.AspNetCore.Mvc;
using UserCrudApi.Api.Dtos;
using UserCrudApi.Api.Models;
using UserCrudApi.Api.Repositories;

namespace UserCrudApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class UsersController(IUserRepository users) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyCollection<User>> GetAll()
    {
        return Ok(users.GetAll());
    }

    [HttpGet("{id:int}")]
    public ActionResult<User> GetById(int id)
    {
        var user = users.GetById(id);

        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public ActionResult<User> Create(CreateUserRequest request)
    {
        if (users.EmailExists(request.Email))
        {
            return Conflict(new { message = "Email already exists." });
        }

        var user = users.Create(request.Name, request.Email);

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, UpdateUserRequest request)
    {
        if (users.EmailExists(request.Email, id))
        {
            return Conflict(new { message = "Email already exists." });
        }

        return users.Update(id, request.Name, request.Email) ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return users.Delete(id) ? NoContent() : NotFound();
    }
}
