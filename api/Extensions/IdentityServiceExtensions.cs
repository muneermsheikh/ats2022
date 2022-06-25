using System.Text;
using core.Entities.Identity;
using infra.Data;
using infra.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace api.Extensions
{
    
    
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            var builder = services.AddIdentityCore<AppUser>(opt => 
                {
                    opt.Password.RequiredLength=8;
                    opt.Password.RequireDigit=true;
                    opt.Password.RequireUppercase=true;
                }
            )
                .AddRoles<AppRole>()
                .AddRoleManager<RoleManager<AppRole>>()
                .AddSignInManager<SignInManager<AppUser>>()
                .AddRoleValidator<RoleValidator<AppRole>>()
                .AddEntityFrameworkStores<AppIdentityDbContext>();

            //builder = new IdentityBuilder(builder.UserType, builder.Services);
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Token:Key"])),
                        ValidIssuer = config["Token:Issuer"],
                        ValidateIssuer =  false,
                        ValidateAudience = false,
                    };
                });
                  
            services.AddAuthorization(options =>
                {
                    options.AddPolicy("AdminRole", policy => policy.RequireRole("Admin"));
                    options.AddPolicy("HRManagerRole", policy => policy.RequireRole("HRManager", "HRSupervisor", "HRExecutive"));
        
                });
            services.AddAuthorization(options => { options.AddPolicy("Admin", new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser().RequireClaim("role", "Admin").Build());});
            services.AddAuthorization(options => { options.AddPolicy("Candidate", new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser().RequireClaim("role", "Candidate").Build());});
            services.AddAuthorization(options => { options.AddPolicy("Employee", new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser().RequireClaim("role", "Employee").Build());});
            services.AddAuthorization(options => { options.AddPolicy("Customer", new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser().RequireClaim("role", "Customer").Build());});
            services.AddAuthorization(options => { options.AddPolicy("Associate", new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser().RequireClaim("role", "Associate").Build());});
        
            return services;
        }
    }
 

}