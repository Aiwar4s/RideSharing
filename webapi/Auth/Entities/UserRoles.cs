namespace webapi.Auth.Entities
{
    public static class UserRoles
    {
        public const string Admin=nameof(Admin);
        public const string BasicUser=nameof(BasicUser);
        public const string Driver = nameof(Driver);
        public static readonly IReadOnlyCollection<string> All = new[] { Admin, BasicUser, Driver };
    }
}
