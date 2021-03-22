using System;
using System.Threading.Tasks;

namespace Twitch
{
    class Program
    {
        static async Task Main(string[] args)
            => await new Startup().InitializeAsync();
    }
}