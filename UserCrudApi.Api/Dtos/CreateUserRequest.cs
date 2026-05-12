using System.ComponentModel.DataAnnotations;

namespace UserCrudApi.Api.Dtos;

public sealed class CreateUserRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public required string Name { get; init; }

    [Required]
    [EmailAddress]
    public required string Email { get; init; }
}
