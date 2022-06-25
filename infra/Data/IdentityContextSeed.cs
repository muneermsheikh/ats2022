using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using core.Entities;
using core.Entities.Admin;
using core.Entities.EmailandSMS;
using core.Entities.HR;
using core.Entities.Identity;
using core.Entities.MasterEntities;
using core.Entities.Orders;
using core.Entities.Process;
using core.Entities.Tasks;
using core.Entities.Users;
using infra.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace infra.Data
{
    public class IdentityContextSeed
    {
        public static async Task IdentityContextSeedAsync(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            var succeeded = await roleManager.CreateAsync(new AppRole{Name="Candidate"});
            
            if (await userManager.Users.AnyAsync()) return;
            var userdata = await System.IO.File.ReadAllTextAsync("Data/IdentitySeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(userdata);
            if (users == null) return;

            var roles = new List<AppRole> {
                new AppRole{Name="Admin"},
                new AppRole{Name="HR Manager"},
                new AppRole{Name="Process Manager"},
                new AppRole{Name="HR Supervisor"},
                new AppRole{Name="HR Executive"},
                new AppRole{Name="HR Assistant"},
                new AppRole{Name="Document Controller Admin"},
                new AppRole{Name="Document Controller Processing"},
                new AppRole{Name="Candidate"}
            };

            foreach(var role in roles) {
                await roleManager.CreateAsync(role);
            }

            foreach(var user in users) {
                user.UserName = user.UserName.ToLower();
                await userManager.CreateAsync(user, "Pa$$w0rd");
                await userManager.AddToRoleAsync(user, "Candidate");
            }

            var admin = new AppUser {UserName = "admin"};

            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, new[]{"Admin", "HR Manager", "HR Supervisor"});
        }
    }
}
