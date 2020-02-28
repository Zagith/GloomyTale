using Autofac;
using AutoMapper;
using GloomyTale.Core;
using GloomyTale.DAL;
using GloomyTale.DAL.EF.Helpers;
using GloomyTale.Plugins;
using GloomyTale.Plugins.Exceptions;
using GloomyTale.Plugins.Logging;
using GloomyTale.SqlServer;
using GloomyTale.SqlServer.Mapping;
using GloomyTale.ToolKit.Importers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GloomyTale.ToolKit
{
    public static class Program
    {
        #region Methods

        private static IContainer BuildCoreContainer()
        {
            var pluginBuilder = new ContainerBuilder();
            pluginBuilder.RegisterType<SerilogLogger>().AsImplementedInterfaces();
            // pluginBuilder.RegisterType<LoggingPlugin>().AsImplementedInterfaces().AsSelf();
            pluginBuilder.RegisterType<DatabasePlugin>().AsImplementedInterfaces().AsSelf();
            IContainer container = pluginBuilder.Build();

            var coreBuilder = new ContainerBuilder();
            foreach (ICorePlugin plugin in container.Resolve<IEnumerable<ICorePlugin>>())
            {
                try
                {
                    plugin.OnLoad(coreBuilder);
                }
                catch (PluginException e)
                {
                }
            }

            coreBuilder.Register(_ => new ToolkitMapper()).As<IMapper>().SingleInstance();
            return coreBuilder.Build();
        }

        public static void Main(string[] args)
        {
            // initialize logger
            Logger.InitializeLogger(new SerilogLogger());

            using (IContainer coreContainer = BuildCoreContainer())
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                var config = new ImportConfiguration
                {
                    Folder = "",
                    Lang = "es",
                    Packets = new List<string[]>()
                };
                Console.Title = "GloomyTale - Parser";
                if (!DataAccessHelper.Initialize(coreContainer.Resolve<IOpenNosContextFactory>()))
                {
                    return;
                }

                DAOFactory.Initialize(coreContainer.Resolve<DAOFactory>());

                var key = new ConsoleKeyInfo();
                Logger.Log.Warn(Language.Instance.GetMessageFromKey("NEED_TREE"));
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.WriteLine("Root");
                Console.ResetColor();
                Console.WriteLine("-----_code_en_Card.txt");
                Console.WriteLine("-----_code_en_Item.txt");
                Console.WriteLine("-----_code_en_MapIDData.txt");
                Console.WriteLine("-----_code_en_monster.txt");
                Console.WriteLine("-----_code_en_Skill.txt");
                Console.WriteLine("-----packet.txt");
                Console.WriteLine("-----Card.dat");
                Console.WriteLine("-----Item.dat");
                Console.WriteLine("-----MapIDData.dat");
                Console.WriteLine("-----monster.dat");
                Console.WriteLine("-----Skill.dat");
                Console.WriteLine("-----quest.dat");
                Console.WriteLine("-----qstprize.dat");
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.WriteLine("-----map");
                Console.ResetColor();
                Console.WriteLine("----------0");
                Console.WriteLine("----------1");
                Console.WriteLine("----------...");

                try
                {
                    Logger.Log.Warn(Language.Instance.GetMessageFromKey("ENTER_PATH"));

                    string folder = string.Empty;
                    if (args.Length == 0)
                    {
                        folder = Console.ReadLine();
                        Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_ALL")} [Y/n]");
                        key = Console.ReadKey(true);
                    }
                    else
                    {
                        folder = args.Aggregate(folder, (current, str) => current + str + " ");
                    }

                    config.Folder = folder;
                    var factory = new ImportFactory(config);
                    var packetImporter = new PacketImporter(config);
                    packetImporter.Import();

                    var mapImporter = new MapImporter(config);
                    if (key.KeyChar != 'n')
                    {
                        mapImporter.Import();
                        factory.LoadMaps();
                        factory.ImportRespawnMapType();
                        factory.ImportMapType();
                        factory.ImportMapTypeMap();
                        //factory.ImportAccounts();
                        /*factory.ImportPortals();
                        factory.ImportScriptedInstances();
                        factory.InsertI18NCard();
                        factory.InsertI18NItem();
                        factory.InsertI18NNpcMonster();
                        factory.InsertI18NSkill();
                        factory.InsertI18NMap();
                        factory.ImportItems();*/
                        factory.ImportSkills();
                        /*factory.ImportCards();
                        factory.ImportNpcMonsters();
                        factory.ImportNpcMonsterData();
                        factory.ImportMapNpcs();
                        factory.ImportMonsters();
                        factory.ImportShops();
                        factory.ImportTeleporters();
                        factory.ImportShopItems();
                        factory.ImportShopSkills();
                        factory.ImportRecipe();
                        factory.ImportHardcodedItemRecipes();
                        factory.ImportQuests();*/
                    }
                    else
                    {
                        Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_MAPS")} [Y/n]");
                        key = Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            mapImporter.Import();
                            factory.LoadMaps();
                        }

                        /*Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_MAPTYPES")} [Y/n]");
                        key = Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportMapType();
                            factory.ImportMapTypeMap();
                            factory.ImportRespawnMapType();
                        }*/

                        /*Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_ACCOUNTS")} [Y/n]");
                        key = Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            ImportFactory.ImportAccounts();
                        }

                        System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_PORTALS")} [Y/n]");
                        key = System.Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportPortals();
                        }

                        System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_TIMESPACES")} [Y/n]");
                        key = System.Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportScriptedInstances();
                        }

                        System.Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_I18NCARDS")} [Y/n]");
                        key = System.Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.InsertI18NCard();
                        }

                        System.Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_I18NITEMS")} [Y/n]");
                        key = System.Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.InsertI18NItem();
                        }

                        System.Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_I18NNPCMONSTER")} [Y/n]");
                        key = System.Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.InsertI18NNpcMonster();
                        }

                        System.Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_I18NSKILLS")} [Y/n]");
                        key = System.Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.InsertI18NSkill();
                        }

                        System.Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_I18NMAPS")} [Y/n]");
                        key = System.Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.InsertI18NMap();
                        }

                        System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_ITEMS")} [Y/n]");
                        key = System.Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportItems();
                        }

                        System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_SKILLS")} [Y/n]");
                        key = System.Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportSkills();
                        }

                        System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_MONSTERS")} [Y/n]");
                        key = System.Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportNpcMonsters();
                        }

                        System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_NPCMONSTERDATA")} [Y/n]");
                        key = System.Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportNpcMonsterData();
                        }

                        System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_CARDS")} [Y/n]");
                        key = System.Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportCards();
                        }

                        System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_MAPNPCS")} [Y/n]");
                        key = System.Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportMapNpcs();
                        }

                        System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_MAPMONSTERS")} [Y/n]");
                        key = System.Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportMonsters();
                        }

                        System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_SHOPS")} [Y/n]");
                        key = System.Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportShops();
                        }

                        System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_TELEPORTERS")} [Y/n]");
                        key = System.Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportTeleporters();
                        }

                        System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_SHOPITEMS")} [Y/n]");
                        key = System.Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportShopItems();
                        }

                        System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_SHOPSKILLS")} [Y/n]");
                        key = System.Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportShopSkills();
                        }

                        System.Console.WriteLine($"{Language.Instance.GetMessageFromKey("PARSE_RECIPES")} [Y/n]");
                        key = System.Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportRecipe();
                            factory.ImportHardcodedItemRecipes();
                        }
                        System.Console.WriteLine($@"{Language.Instance.GetMessageFromKey("PARSE_QUESTS")} [Y/n]");
                        key = System.Console.ReadKey(true);
                        if (key.KeyChar != 'n')
                        {
                            factory.ImportQuests();
                        }*/
                    }
                    System.Console.WriteLine(Language.Instance.GetMessageFromKey("DONE"));
                    System.Console.ReadKey();
                }
                catch (FileNotFoundException ex)
                {
                    Logger.Log.Error(Language.Instance.GetMessageFromKey("AT_LEAST_ONE_FILE_MISSING"), ex);
                    System.Console.ReadKey();
                }
            }
        }

        #endregion
    }
}
