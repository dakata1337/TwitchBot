using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Twitch.Modules
{
    class Commands
    {
        public string GetOS()
        {
            return $"I'm running on {Environment.OSVersion}";
        }
    }
}
