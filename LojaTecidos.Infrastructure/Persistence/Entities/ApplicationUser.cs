using Microsoft.AspNetCore.Identity;

namespace LojaTecidos.Infrastructure.Persistence.Entities;

public class ApplicationUser : IdentityUser
{
    public string Nome { get; set; } = string.Empty;
}
