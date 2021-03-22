using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Twitch.Modules;
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

            //Initialize Services
            InitializeServices();
        }
        private void InitializeServices()
        {
            //Configure Services
            _serviceProvider = ConfigureServices();
            _twitchService = _serviceProvider.GetRequiredService<TwitchService>();
        }

        public async Task InitializeAsync()
        {
            //Initialize Twitch Service
            _twitchService.Initialize();

            await Task.Delay(-1);
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<TwitchService>()
                .AddSingleton<Commands>()
                .BuildServiceProvider();
        }
    }
}
