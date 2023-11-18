using Microsoft.AspNetCore.Identity;

namespace webapi.Auth.Entities
{
    public class User:IdentityUser
    {
        public bool ForceRelogin {  get; set; }
    }
}