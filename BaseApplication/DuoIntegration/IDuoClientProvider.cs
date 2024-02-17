using DuoUniversal;

namespace BaseApplication.DuoIntegration
{
    public interface IDuoClientProvider
    {
        public Client GetDuoClient();
    }

    public class DuoClientProvider : IDuoClientProvider
    {
        private string ClientId { get; }
        private string ClientSecret { get; }
        private string ApiHost { get; }
        private string RedirectUri { get; }

        public DuoClientProvider(IConfiguration config)
        {
            ClientId = config.GetValue<string>("DuoSettings:ClientID");
            ClientSecret = config.GetValue<string>("DuoSettings:ClientSecret");
            ApiHost = config.GetValue<string>("DuoSettings:APIHost");
            RedirectUri = config.GetValue<string>("DuoSettings:RedirectURI");
        }

        public Client GetDuoClient()
        {
            if (string.IsNullOrWhiteSpace(ClientId))
            {
                throw new DuoException("A 'Client ID' configuration value is required in the appsettings file.");
            }
            if (string.IsNullOrWhiteSpace(ClientSecret))
            {
                throw new DuoException("A 'Client Secret' configuration value is required in the appsettings file.");
            }
            if (string.IsNullOrWhiteSpace(ApiHost))
            {
                throw new DuoException("An 'Api Host' configuration value is required in the appsettings file.");
            }
            if (string.IsNullOrWhiteSpace(RedirectUri))
            {
                throw new DuoException("A 'Redirect URI' configuration value is required in the appsettings file.");
            }

            return new ClientBuilder(ClientId, ClientSecret, ApiHost, RedirectUri).Build();
        }
    }
}
