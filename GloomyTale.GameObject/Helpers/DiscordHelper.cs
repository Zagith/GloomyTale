using Discord.Webhook;
using System.Threading.Tasks;

namespace GloomyTale.GameObject.Helpers
{
    public class DiscordHelper
    {
        public DiscordHelper()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            // The webhook url follows the format https://discordapp.com/api/webhooks/{id}/{token}
            // Because anyone with the webhook URL can use your webhook
            // you should NOT hard code the URL or ID + token into your application.
            string message = "@everyone **Server Online**";
            using (var client = new DiscordWebhookClient("https://discordapp.com/api/webhooks/661214889875996672/b-fY_M3_X9EOfQWLLNs5fNmvV3rkuao7PjJz6Ni1w1Dh0pEHTe6NeJNpeshc73yoke7b"))
            {
                // Webhooks are able to send multiple embeds per message
                // As such, your embeds must be passed as a collection. 
                await client.SendMessageAsync(text: message);
            }
        }
    }
}
