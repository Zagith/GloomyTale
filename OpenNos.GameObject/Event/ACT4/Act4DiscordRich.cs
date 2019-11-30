using Discord;
using Discord.Webhook;
using OpenNos.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            using (var client = new DiscordWebhookClient("https://discordapp.com/api/webhooks/647350541772521482/y30_Wu0znQUCBkpv-fVTcGVhC8obAiKSMbWDqzcQD_ERTKJTiwyd22u8hRLEij2MQhUN"))
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
                    switch (RaidType)
                    {
                        case MapInstanceType.Act4Morcos:
                            embed = new EmbedBuilder
                            {
                                Color = Color.Red,
                                ImageUrl = "http://wiki.nostale.it/images/b/bc/MaestroMorcos.png",
                                Title = "Morcos Raid",
                                Description = $"Raid Morcos started for {nameFaction} faction"
                            };
                            break;
                        case MapInstanceType.Act4Hatus:
                            embed = new EmbedBuilder
                            {
                                ImageUrl = "http://4.bp.blogspot.com/-rVwumtP1Ivo/U_QeOd78RWI/AAAAAAAABLs/GaeQKq9_b1M/s1600/hatus.jpg",
                                Title = "Hatus Raid",
                                Description = $"Raid Hatus started for {nameFaction} faction"
                            };
                            break;
                        case MapInstanceType.Act4Calvina:
                            embed = new EmbedBuilder
                            {
                                Color = Color.Blue,
                                ImageUrl = "http://wiki.nostale.it/images/2/26/LadyCalvinas.png",
                                Title = "Calvina Raid",
                                Description = $"Raid Calvina started for {nameFaction} faction"
                            };
                            break;
                        case MapInstanceType.Act4Berios:
                            embed = new EmbedBuilder
                            {
                                Color = Color.Gold,
                                ImageUrl = "http://wiki.nostale.it/images/f/f6/BaroneBerios.png",
                                Title = "Berios Raid",
                                Description = $"Raid Berios started for {nameFaction} faction"
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
