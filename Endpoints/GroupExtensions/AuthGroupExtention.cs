using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.DTO;
using MinimalApi.Models;
using MinimalApi.Repository;
using MinimalApi.Services;
using System.Security.Claims;

namespace MinimalApi.Endpoints.GroupExtensions;

public static class AuthGroupExtention
{
    public static RouteGroupBuilder MapAuthGroup(this RouteGroupBuilder group)
    {
        group.MapPost("/register", async (UserRegistrationModel model, UserManager<ApplicationUser> userManager) =>
        {

            try
            {
                var user = new ApplicationUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    CreatedDate = DateTime.UtcNow,

                };

                var result = await userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                    return Results.BadRequest(result.Errors);

                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
                throw;
            }

        });

        group.MapPost("/login", async (UserLoginModel model, JwtTokenService jwtTokenService, UserManager<ApplicationUser> userManager) =>
        {
            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
                return Results.Unauthorized();
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim("FullName", user.FullName)
    };

            var accessToken = jwtTokenService.GenerateAccessToken(claims);

            return Results.Ok(new RefreshTokenModel
            {
                AccessToken = accessToken,
                // Include refresh token in the response if needed
            });
        });

        return group;
    }
}
