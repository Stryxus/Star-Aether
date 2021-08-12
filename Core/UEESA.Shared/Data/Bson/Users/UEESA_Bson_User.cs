using System.Security;

namespace UEESA.Shared.Data.Bson.Users
{
    public class UEESA_Bson_User
    {
        public string User_Username { get; set; }
        public SecureString User_Hash_Email { get; set; }
        public SecureString User_Hash_Password { get; set; }
    }
}
