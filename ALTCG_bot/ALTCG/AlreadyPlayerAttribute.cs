using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALTCG_bot
{
    internal class AlreadyPlayerAttribute : PreconditionAttribute
    {
        

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            throw new NotImplementedException();
        }
    }
}
