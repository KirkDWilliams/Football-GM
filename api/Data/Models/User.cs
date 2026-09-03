using System.ComponentModel.DataAnnotations;

namespace FootballGm.Api.Data.Models;

public class User
{
    [Key] public string Id { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;
}
