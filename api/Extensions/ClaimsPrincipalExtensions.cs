using System.Security.Claims;

namespace api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string RetrieveEmailFromPrincipal(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Email);
        }
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Name)?.Value;
        }

    
        public static int GetUserId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }
    
        public static int GetIdentityUserId(this ClaimsPrincipal user)
        {
            //var id = user.FindFirst("Id").Value;
            //var x = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        }

        public static string GetIdentityUserEmailId(this ClaimsPrincipal user)
        {
            return user?.FindFirst(ClaimTypes.Email)?.Value;
        }
         public static int GetUserIdentityUserEmployeeId(this ClaimsPrincipal user)
        {
            return 0;   //user.GetIdentityUserEmployeeId();
        }
 
        public static bool IsUserAuthenticated(this ClaimsPrincipal user)
        {
            return user.Identity.IsAuthenticated;
        }

        //roles
        public static bool UserHasTheRole(this ClaimsPrincipal userClaim, string roleName)
        {
            return userClaim.IsInRole(roleName);
        }

        public static bool AddRoleToUser(this ClaimsPrincipal userClaim, string roleName)
        {
            return userClaim.AddRoleToUser(roleName);
        }

    }
}