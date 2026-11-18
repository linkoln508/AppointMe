namespace AppointMe.Api.Auth;

public static class Roles
{
    public const string Customer = "Customer";
    public const string BusinessOwner = "BusinessOwner";
    public const string Admin = "Admin";
    public const string BusinessOwnerOrAdmin = BusinessOwner + "," + Admin;
}