namespace OpenSDK.Security
{
    public class Model
    {
        public string? SecurityKey;
        public int? SecurityPermission;
    
        public Model(string securityKey, int securityPermission)
        {
            SecurityKey = securityKey;
            SecurityPermission = securityPermission;
        }

        public string? Key
        {
            get => SecurityKey;
            set => SecurityKey = value;
        }

        public int? Permission
        {
            get => SecurityPermission;
            set => SecurityPermission = value;
        }
    }
}