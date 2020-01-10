namespace SIS.MvcFramework.Attributes.Security
{
    using System;
    using SIS.MvcFramework.Identity;

    public class AuthorizeAttribute : Attribute
    {
        private const string DefaultAuthority = "anonymous";
        private const string DefaultRole = "authorized";

        private readonly string authority;

        public AuthorizeAttribute(string authority = DefaultRole)
        {
            this.authority = authority;
        }

        private bool IsLoggedIn(Principal principal)
        {
            return principal != null;
        }

        public bool IsInAuthority(Principal principal)
        {
            if (!this.IsLoggedIn(principal))
            {
                return this.authority == DefaultAuthority;
            }

            return this.authority == DefaultRole
                || principal.Roles.Contains(this.authority.ToLower());
        }
    }
}
