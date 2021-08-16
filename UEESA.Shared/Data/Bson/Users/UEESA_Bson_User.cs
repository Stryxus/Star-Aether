using System.Security;

namespace UEESA.Data.Bson.Users
{
    public class UEESA_Bson_User
    {
        public string User_Username { get; set; }
        public string User_Hash_Email { get; set; }
        public string User_Hash_Password { get; set; }
    }
}
