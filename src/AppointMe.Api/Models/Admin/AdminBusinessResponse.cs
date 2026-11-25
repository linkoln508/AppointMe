namespace AppointMe.Api.Models.Admin;

public class AdminBusinessResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? OwnerEmail { get; set; }
}