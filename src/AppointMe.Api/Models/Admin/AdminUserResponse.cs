namespace AppointMe.Api.Models.Admin;

public class AdminUserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new List<string>();
    public bool IsBlocked { get; set; }
}