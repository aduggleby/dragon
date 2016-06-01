namespace Dragon.SecurityServer.PermissionSTS.Client
{
    public class AddPermissionModel
    {
        public string UserId { get; set; }
        public string Operation { get; set; }
        public string Object { get; set; }
    }
}
