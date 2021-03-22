using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Twitch.Services;

namespace Twitch
{
    class Startup
    {
        private ServiceProvider _serviceProvider;
        private TwitchService _twitchService;
        public Startup()
        {
            //Initialize Logger
            LoggingService.Initialize();

            InitializeServices();
        }

        public async Task InitializeAsync()
        {
            _twitchService.Initialize();

            await Task.Delay(-1);
        }


        private void InitializeServices()
        {
            _serviceProvider = ConfigureServices();
            _twitchService = _serviceProvider.GetRequiredService<TwitchService>();
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<TwitchService>()
                .BuildServiceProvider();
        }
    }
}
