using AppointMe.Api.Auth;
using AppointMe.Api.Models;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

public class BusinessOwnerHandler : AuthorizationHandler<BusinessOwnerRequirement, Business>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,     // пользователь и ресурс
        BusinessOwnerRequirement requirement,    // требование
        Business resource)                       // конкретный бизнес
    {
        if (context.User.IsInRole(Roles.Admin))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

         var userId = Guid.Parse(context.User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

        if (resource.OwnerId == userId)
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}