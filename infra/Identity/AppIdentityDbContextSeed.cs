using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace infra.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUsersAsync(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            
            if (!roleManager.Roles.Any())
            {
                var roles = new List<AppRole>()
                {
                    new AppRole{Name="Admin"}, new AppRole{Name="HRExecutive"}, 
                    new AppRole{Name="HRTrainee"}, new AppRole{Name="HRSupervisor"}, 
                    new AppRole{Name="HRManager"}, new AppRole{Name="AdminManager"}, 
                    new AppRole{Name="DocumentControllerAdmin"}, new AppRole{Name="ProcessExecutive"}, 
                    new AppRole{Name="VisaExecutiveKSA"}, 
                    new AppRole{Name="VisaExecutiveSharjah"}, new AppRole{Name="VisaExecutiveDubai"}, 
                    new AppRole{Name="VisaExecutiveQatar"}, new AppRole{Name="VisaExecutiveOman"}, 
                    new AppRole{Name="MedicalExecutiveGAMMCA"}, new AppRole{Name="MedicalExecutive"}, 
                    new AppRole{Name="EmigrationExecutive"}, new AppRole{Name="AirlineExecutive"}
                };
                foreach(var role in roles)
                {
                    //if (!await roleManager.RoleExistsAsync(role.Name)) 
                    await roleManager.CreateAsync(role);
                }
            }

            if (!userManager.Users.Any())  {
                var user = new AppUser
                {
                    DisplayName = "Farooque",
                    Email = "farooque@afreenintl.in",
                    UserName = "farooque@afreenintl.in",
                    UserType = "Candidate",
                    KnownAs = "Farooque",
                    Gender = "M",
                    /* Address = new Address
                    {   FirstName = "Farooque", SecondName = "Jigar", Add = "12 BDD Chawls", StreetAdd = "Worli",
                        City = "Mumbai", State = "MH", Pin = "400018" }
                    */
                };

                IdentityResult identityResult = await userManager.CreateAsync(user, "Pa$$w0rd");
                if (identityResult.Succeeded) await userManager.AddToRolesAsync(user, new[] {"HRExecutive", "HRTrainee"});

                user = new AppUser
                {
                    DisplayName = "Munir Sheikh",
                    Email = "munir@afreenintl.in",
                    UserName = "munir@afreenintl.in",
                    UserType = "Candidate",
                    KnownAs = "Munir",
                    Gender = "M",
                    /* Address = new Address
                    {   FirstName = "Munir", SecondName = "Sheikh", Add = "2/A6-002, Shanti Nagar", StreetAdd = "Opp Kotak Bank",
                        City = "Mira Road", State = "MH", Pin = "401107" }
                    */
                };
                identityResult = await userManager.CreateAsync(user, "Pa$$w0rd");
                if(identityResult.Succeeded)  await userManager.AddToRolesAsync(user, new[] {"Admin", "HRManager", "DocumentControllerAdmin"});
                
                user = new AppUser
                {
                    DisplayName = "Mushahid Khan",
                    Email = "mushahid@afreenintl.in",
                    UserName = "mushahid@afreenintl.in",
                    UserType = "Candidate",
                    KnownAs = "Mushahid",
                    Gender = "M",
                    /* Address = new Address
                    {   FirstName = "Mushahid", SecondName = "Khan", Add = "G12, Kohinoor Mall", StreetAdd = "Kurla west",
                        City = "Mumbai", State = "MH", Pin = "400070" }
                */
                };
                identityResult = await userManager.CreateAsync(user, "Pa$$w0rd");
                if(identityResult.Succeeded)  await userManager.AddToRolesAsync(user, new[] {"HRTrainee", "EmigrationExecutive"});

                user = new AppUser
                {
                    DisplayName = "Zahid Shaikh",
                    Email = "zahid@afreenintl.in",
                    UserName = "zahid@afreenintl.in",
                    UserType = "Candidate",
                    KnownAs = "Zahid",
                    Gender = "M",
                    /* Address = new Address
                    {   FirstName = "Zahid", SecondName = "Shaikh", Add = "sector 2, bldg A6, Flat 002", StreetAdd = "Opp Kotak Bank",
                        City = "Mira Road", State = "MH", Pin = "401107" }
                    */
                };
                identityResult = await userManager.CreateAsync(user, "Pa$$w0rd");
                await userManager.AddToRolesAsync(user, new[] {"HRTrainee", "EmigrationExecutive"});

                /*
                if (identityResult.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Admin").Wait();
                }
                */
            }

            
        }


        private static async Task<bool> CreateRole(string rolename, RoleManager<AppRole> roleManager)
        {
            var roleExist = await roleManager.RoleExistsAsync(rolename);
            if (!roleExist) {
                IdentityResult roleResult = await roleManager.CreateAsync(new AppRole{Name = rolename});
                return roleResult==IdentityResult.Success;
            }
            
            return false;
            
        }
    
    }
}