using Discord;
using Discord.Webhook;
using OpenNos.Domain;
using System.Threading.Tasks;

namespace OpenNos.GameObject.Event.ACT4
{
    public class Act4DiscordRich
    {
        public Act4DiscordRich(byte faction, MapInstanceType raidType, bool isMukraju)
        {
            Faction = faction;
            RaidType = raidType;
            IsMukraju = isMukraju;
            MainAsync().GetAwaiter().GetResult();
        }

        public MapInstanceType RaidType { get; set; }

        public byte Faction { get; set; }

        public bool IsMukraju { get; set; }


        public async Task MainAsync()
        {
            // The webhook url follows the format https://discordapp.com/api/webhooks/{id}/{token}
            // Because anyone with the webhook URL can use your webhook
            // you should NOT hard code the URL or ID + token into your application.
            EmbedBuilder embed;
            string nameFaction;
            string message = "";
            using (var client = new DiscordWebhookClient("https://discordapp.com/api/webhooks/662019836267986954/HhI3-iyFEWbyvfndxZYWkWC8mWlBoSRyNdA3wkeKUj3QyHORvtrTA3fiFPtlpwQG7fCZ"))
            {
                switch (Faction)
                {
                    case 1:
                        nameFaction = "Angel";
                        break;
                    case 2:
                        nameFaction = "Demon";
                        break;
                    default:
                        return;

                }
                if (IsMukraju == true)
                {
                    message = "A new raid can appear to Frozen Crown... Get ready for the battle!";
                    string description = $"Raid started for {nameFaction} faction";
                    embed = new EmbedBuilder
                    {
                        Color = Color.DarkBlue,
                        ImageUrl = "http://wiki.nostale.it/images/4/45/LordMukraju.png",
                        Title = "Lord Mukraju Appear",
                        Description = $"Faction {nameFaction}"
                    };
                }
                else
                {
                    message = "A new raid appeared to Frozen Crown... Get ready for the battle!";
                    string description = $"Raid started for {nameFaction} faction";
                    switch (RaidType)
                    {
                        case MapInstanceType.Act4Viserion:
                            embed = new EmbedBuilder
                            {
                                Color = Color.Red,
                                ImageUrl = "https://cdn.discordapp.com/attachments/534507467128963092/571088928816496650/8563.png",
                                Title = "God Viserion Raid",
                                Description = description
                            };
                            break;
                        case MapInstanceType.Act4Orias:
                            embed = new EmbedBuilder
                            {
                                ImageUrl = "https://cdn.discordapp.com/attachments/534507467128963092/571088937871998977/8577.png",
                                Title = "God Orias Raid",
                                Description = description
                            };
                            break;
                        case MapInstanceType.Act4Zanarkand:
                            embed = new EmbedBuilder
                            {
                                Color = Color.Blue,
                                ImageUrl = "https://cdn.discordapp.com/attachments/534507467128963092/571088954476986394/8629.png",
                                Title = "God Zanarkand Raid",
                                Description = description
                            };
                            break;
                        case MapInstanceType.Act4Demetra:
                            embed = new EmbedBuilder
                            {
                                Color = Color.Gold,
                                ImageUrl = "https://cdn.discordapp.com/attachments/534507467128963092/571088946608603137/8624.png",
                                Title = "God Demetra Raid",
                                Description = description
                            };
                            break;
                        default:
                            return;
                    }
                }
                // Webhooks are able to send multiple embeds per message
                // As such, your embeds must be passed as a collection. 
                await client.SendMessageAsync(text: message, embeds: new[] { embed.Build() });
            }
        }
    }
}
