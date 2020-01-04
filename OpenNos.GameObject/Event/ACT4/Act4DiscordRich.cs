﻿using Discord;
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
                        case MapInstanceType.Act4Viserion:
                            embed = new EmbedBuilder
                            {
                                Color = Color.Red,
                                ImageUrl = "http://wiki.nostale.it/images/b/bc/MaestroMorcos.png",
                                Title = "Morcos Raid",
                                Description = $"Raid Morcos started for {nameFaction} faction"
                            };
                            break;
                        case MapInstanceType.Act4Orias:
                            embed = new EmbedBuilder
                            {
                                ImageUrl = "http://4.bp.blogspot.com/-rVwumtP1Ivo/U_QeOd78RWI/AAAAAAAABLs/GaeQKq9_b1M/s1600/hatus.jpg",
                                Title = "Hatus Raid",
                                Description = $"Raid Hatus started for {nameFaction} faction"
                            };
                            break;
                        case MapInstanceType.Act4Zanarkand:
                            embed = new EmbedBuilder
                            {
                                Color = Color.Blue,
                                ImageUrl = "http://wiki.nostale.it/images/2/26/LadyCalvinas.png",
                                Title = "Calvina Raid",
                                Description = $"Raid Calvina started for {nameFaction} faction"
                            };
                            break;
                        case MapInstanceType.Act4Demetra:
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
