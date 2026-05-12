namespace UserCrudApi.Api.Models;

public sealed class User
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Email { get; set; }
}
