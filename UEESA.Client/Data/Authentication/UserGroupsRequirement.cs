using Microsoft.AspNetCore.Authorization;

namespace UEESA.Client.Data.Authentication
{
    public class UserGroupsRequirement : IAuthorizationRequirement
    {
        public string[] Groups { get; }

        public UserGroupsRequirement(string[] groups)
        {
            Groups = groups;
        }
    }
}
