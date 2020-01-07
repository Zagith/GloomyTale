using Discord.Commands;

namespace GloomyTale.DiscordBot.Services
{
    public class ResultService : RuntimeResult
    {
        public ResultService(CommandError? error, string reason) : base(error, reason)
        {
        }

        public static ResultService FromError(string reason) =>
            new ResultService(CommandError.Unsuccessful, reason);
        public static ResultService FromStrangeError(string strangeReason) =>
            new ResultService(CommandError.Unsuccessful, $"A strange error has occured, please send this to the support channel in our Discord server (~support).\n`{strangeReason}`");
        public static ResultService FromSuccess(string reason = null) =>
            new ResultService(null, reason);
    }
}
