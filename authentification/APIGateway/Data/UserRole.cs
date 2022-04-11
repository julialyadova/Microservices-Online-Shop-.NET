namespace APIGateway.Data
{
    public class RoleNames
    {
        public const string ADMIN = "Admin";
        public const string USER = "User";
    }

    public class UserRole
    {
        public int Id { get; set; }
        public User User { get; set; }
        public string Name { get; set; }

        public UserRole() { }

        public UserRole(string name)
        {
            Name = name;
        }
    }
}
