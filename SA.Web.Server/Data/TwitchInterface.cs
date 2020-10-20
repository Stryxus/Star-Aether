using System;
using System.Threading.Tasks;

using TwitchLib.Api;

using SA.Web.Server.Data.Json;

namespace SA.Web.Server.Data
{
    internal class TwitchInterface
    {
        private TwitchAPI api;

        internal void Connect()
        {
            api = new TwitchAPI();
            api.Settings.ClientId = PrivateVariables.Instance.TwitchAPIID;
        }

        internal async Task<string> GetLogoURL(string username) => (await api.V5.Users.GetUserByNameAsync(username)).Matches[0].Logo;
    }
}
