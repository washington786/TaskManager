using System;
using Microsoft.AspNetCore.Identity;
using TaskManager.Models;

namespace TaskManager.Extensions;

public class IdentityExtentions
{
    public static async Task<IdentityResult> CreateUserWithRolesAsync(UserManager<AppUser> usermanager, string email, string password, string firstName, string lastName, string roleName = "user")
    {
        var user = new AppUser
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };

        var result = await usermanager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await usermanager.AddToRoleAsync(user, roleName);
        }
        return result;
    }
}
