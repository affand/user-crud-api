using UserCrudApi.Api.Models;

namespace UserCrudApi.Api.Repositories;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> users = [];
    private int nextId = 1;

    public IReadOnlyCollection<User> GetAll() => users.AsReadOnly();

    public User? GetById(int id) => users.SingleOrDefault(user => user.Id == id);

    public User Create(string name, string email)
    {
        var user = new User
        {
            Id = nextId++,
            Name = name.Trim(),
            Email = email.Trim().ToLowerInvariant()
        };

        users.Add(user);

        return user;
    }

    public bool EmailExists(string email, int? excludingUserId = null)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        return users.Any(user =>
            user.Email.Equals(normalizedEmail, StringComparison.OrdinalIgnoreCase)
            && user.Id != excludingUserId);
    }

    public bool Update(int id, string name, string email)
    {
        var user = GetById(id);

        if (user is null)
        {
            return false;
        }

        user.Name = name.Trim();
        user.Email = email.Trim().ToLowerInvariant();

        return true;
    }

    public bool Delete(int id)
    {
        var user = GetById(id);

        return user is not null && users.Remove(user);
    }
}
