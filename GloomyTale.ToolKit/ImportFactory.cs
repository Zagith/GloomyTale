using GloomyTale.Core;
using GloomyTale.DAL;
using GloomyTale.Data;
using GloomyTale.Domain;
using GloomyTale.ToolKit.Importers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GloomyTale.ToolKit
{
    public class ImportFactory
    {
        #region Instantiation

        public ImportFactory(ImportConfiguration configuration) => _configuration = configuration;

        #endregion

        #region Members

        private List<MapDTO> _maps;
        private readonly ImportConfiguration _configuration;

        #endregion

        /*public static void ImportAccounts()
        {
            if (DAOFactory.Instance.AccountDAO.ContainsAccounts())
            {
                // If there are accounts in the database, there is no need to re add these 2 accounts.
                // We definately don't want people to access the admin account.
                return;
            }

            var acc1 = new AccountDTO
            {
                Authority = AuthorityType.Administrator,
                Name = "admin",
                Password = "test".ToSha512()
            };
            DAOFactory.Instance.AccountDAO.InsertOrUpdate(ref acc1);

            var acc2 = new AccountDTO
            {
                Authority = AuthorityType.User,
                Name = "test",
                Password = "test".ToSha512()
            };
            DAOFactory.Instance.AccountDAO.InsertOrUpdate(ref acc2);
        }*/

        public void ImportSkills()
        {
            string fileSkillId = $"{_configuration.Folder}\\Skill.dat";
            string fileSkillLang = $"{_configuration.Folder}\\_code_{_configuration.Lang}_Skill.txt";
            List<SkillDTO> skills = new List<SkillDTO>();

            Dictionary<string, string> dictionaryIdLang = new Dictionary<string, string>();
            var skill = new SkillDTO();
            List<ComboDTO> combo = new List<ComboDTO>();
            List<BCardDTO> skillCards = new List<BCardDTO>();
            string line;
            int counter = 0;
            using (var skillIdLangStream = new StreamReader(fileSkillLang, Encoding.GetEncoding(1252)))
            {
                while ((line = skillIdLangStream.ReadLine()) != null)
                {
                    string[] linesave = line.Split('\t');
                    if (linesave.Length > 1 && !dictionaryIdLang.ContainsKey(linesave[0]))
                    {
                        dictionaryIdLang.Add(linesave[0], linesave[1]);
                    }
                }
            }

            using (var skillIdStream = new StreamReader(fileSkillId, Encoding.GetEncoding(1252)))
            {
                while ((line = skillIdStream.ReadLine()) != null)
                {
                    string[] currentLine = line.Split('\t');

                    if (currentLine.Length > 2 && currentLine[1] == "VNUM")
                    {
                        skill = new SkillDTO
                        {
                            SkillVNum = short.Parse(currentLine[2])
                        };
                    }
                    else if (currentLine.Length > 2 && currentLine[1] == "NAME")
                    {
                        skill.NameI18NKey = dictionaryIdLang.TryGetValue(currentLine[2], out string name) ? name : string.Empty;
                    }
                    else if (currentLine.Length > 2 && currentLine[1] == "TYPE")
                    {
                        skill.SkillType = byte.Parse(currentLine[2]);
                        skill.CastId = short.Parse(currentLine[3]);
                        skill.Class = byte.Parse(currentLine[4]);
                        skill.Type = byte.Parse(currentLine[5]);
                        skill.Element = byte.Parse(currentLine[7]);
                    }
                    else if (currentLine.Length > 2 && currentLine[1] == "FCOMBO")
                    {
                        for (int i = 3; i < currentLine.Length - 4; i += 3)
                        {
                            var comb = new ComboDTO
                            {
                                SkillVNum = skill.SkillVNum,
                                Hit = short.Parse(currentLine[i]),
                                Animation = short.Parse(currentLine[i + 1]),
                                Effect = short.Parse(currentLine[i + 2])
                            };

                            if (comb.Hit == 0 && comb.Animation == 0 && comb.Effect == 0)
                            {
                                continue;
                            }

                            if (!DAOFactory.Instance.ComboDAO.LoadByVNumHitAndEffect(comb.SkillVNum, comb.Hit, comb.Effect).Any())
                            {
                                combo.Add(comb);
                            }
                        }
                    }
                    else if (currentLine.Length > 3 && currentLine[1] == "COST")
                    {
                        skill.CPCost = currentLine[2] == "-1" ? (byte)0 : byte.Parse(currentLine[2]);
                        skill.Price = int.Parse(currentLine[3]);
                    }
                    else if (currentLine.Length > 2 && currentLine[1] == "LEVEL")
                    {
                        skill.LevelMinimum = currentLine[2] != "-1" ? byte.Parse(currentLine[2]) : (byte)0;
                        if (skill.Class > 31)
                        {
                            SkillDTO firstskill = skills.FirstOrDefault(s => s.Class == skill.Class);
                            if (firstskill == null || skill.SkillVNum <= firstskill.SkillVNum + 10)
                            {
                                switch (skill.Class)
                                {
                                    case 8:
                                        switch (skills.Count(s => s.Class == skill.Class))
                                        {
                                            case 3:
                                                skill.LevelMinimum = 20;
                                                break;

                                            case 2:
                                                skill.LevelMinimum = 10;
                                                break;

                                            default:
                                                skill.LevelMinimum = 0;
                                                break;
                                        }

                                        break;

                                    case 9:
                                        switch (skills.Count(s => s.Class == skill.Class))
                                        {
                                            case 9:
                                                skill.LevelMinimum = 20;
                                                break;

                                            case 8:
                                                skill.LevelMinimum = 16;
                                                break;

                                            case 7:
                                                skill.LevelMinimum = 12;
                                                break;

                                            case 6:
                                                skill.LevelMinimum = 8;
                                                break;

                                            case 5:
                                                skill.LevelMinimum = 4;
                                                break;

                                            default:
                                                skill.LevelMinimum = 0;
                                                break;
                                        }

                                        break;

                                    case 16:
                                        switch (skills.Count(s => s.Class == skill.Class))
                                        {
                                            case 6:
                                                skill.LevelMinimum = 20;
                                                break;

                                            case 5:
                                                skill.LevelMinimum = 15;
                                                break;

                                            case 4:
                                                skill.LevelMinimum = 10;
                                                break;

                                            case 3:
                                                skill.LevelMinimum = 5;
                                                break;

                                            case 2:
                                                skill.LevelMinimum = 3;
                                                break;

                                            default:
                                                skill.LevelMinimum = 0;
                                                break;
                                        }

                                        break;

                                    default:
                                        switch (skills.Count(s => s.Class == skill.Class))
                                        {
                                            case 10:
                                                skill.LevelMinimum = 20;
                                                break;

                                            case 9:
                                                skill.LevelMinimum = 16;
                                                break;

                                            case 8:
                                                skill.LevelMinimum = 12;
                                                break;

                                            case 7:
                                                skill.LevelMinimum = 8;
                                                break;

                                            case 6:
                                                skill.LevelMinimum = 4;
                                                break;

                                            default:
                                                skill.LevelMinimum = 0;
                                                break;
                                        }

                                        break;
                                }
                            }
                        }

                        skill.MinimumAdventurerLevel = currentLine[3] != "-1" ? byte.Parse(currentLine[3]) : (byte)0;
                        skill.MinimumSwordmanLevel = currentLine[4] != "-1" ? byte.Parse(currentLine[4]) : (byte)0;
                        skill.MinimumArcherLevel = currentLine[5] != "-1" ? byte.Parse(currentLine[5]) : (byte)0;
                        skill.MinimumMagicianLevel = currentLine[6] != "-1" ? byte.Parse(currentLine[6]) : (byte)0;
                    }
                    else if (currentLine.Length > 2 && currentLine[1] == "EFFECT")
                    {
                        skill.CastEffect = short.Parse(currentLine[3]);
                        skill.CastAnimation = short.Parse(currentLine[4]);
                        skill.Effect = short.Parse(currentLine[5]);
                        skill.AttackAnimation = short.Parse(currentLine[6]);
                    }
                    else if (currentLine.Length > 2 && currentLine[1] == "TARGET")
                    {
                        skill.TargetType = byte.Parse(currentLine[2]);
                        skill.HitType = byte.Parse(currentLine[3]);
                        skill.Range = byte.Parse(currentLine[4]);
                        skill.TargetRange = byte.Parse(currentLine[5]);
                    }
                    else if (currentLine.Length > 2 && currentLine[1] == "DATA")
                    {
                        skill.UpgradeSkill = short.Parse(currentLine[2]);
                        skill.UpgradeType = short.Parse(currentLine[3]);
                        skill.CastTime = short.Parse(currentLine[6]);
                        skill.Cooldown = short.Parse(currentLine[7]);
                        skill.MpCost = short.Parse(currentLine[10]);
                        skill.ItemVNum = short.Parse(currentLine[12]);
                    }
                    else if (currentLine.Length > 2 && currentLine[1] == "BASIC")
                    {
                        byte type = (byte)int.Parse(currentLine[3]);
                        if (type == 0 || type == 255)
                        {
                            continue;
                        }

                        int first = int.Parse(currentLine[5]);
                        var itemCard = new BCardDTO
                        {
                            SkillVNum = skill.SkillVNum,
                            Type = type,
                            SubType = (byte)((int.Parse(currentLine[4]) + 1) * 10 + 1 + (first < 0 ? 1 : 0)),
                            IsLevelScaled = Convert.ToBoolean(first % 4),
                            IsLevelDivided = (first % 4) == 2,
                            FirstData = (short)((first > 0 ? first : -first) / 4),
                            SecondData = (short)(int.Parse(currentLine[6]) / 4),
                            ThirdData = (short)(int.Parse(currentLine[7]) / 4)
                        };
                        skillCards.Add(itemCard);
                    }
                    else if (currentLine.Length > 2 && currentLine[1] == "FCOMBO")
                    {
                        // investigate
                        /*
                        if (currentLine[2] == "1")
                        {
                            combo.FirstActivationHit = byte.Parse(currentLine[3]);
                            combo.FirstComboAttackAnimation = short.Parse(currentLine[4]);
                            combo.FirstComboEffect = short.Parse(currentLine[5]);
                            combo.SecondActivationHit = byte.Parse(currentLine[3]);
                            combo.SecondComboAttackAnimation = short.Parse(currentLine[4]);
                            combo.SecondComboEffect = short.Parse(currentLine[5]);
                            combo.ThirdActivationHit = byte.Parse(currentLine[3]);
                            combo.ThirdComboAttackAnimation = short.Parse(currentLine[4]);
                            combo.ThirdComboEffect = short.Parse(currentLine[5]);
                            combo.FourthActivationHit = byte.Parse(currentLine[3]);
                            combo.FourthComboAttackAnimation = short.Parse(currentLine[4]);
                            combo.FourthComboEffect = short.Parse(currentLine[5]);
                            combo.FifthActivationHit = byte.Parse(currentLine[3]);
                            combo.FifthComboAttackAnimation = short.Parse(currentLine[4]);
                            combo.FifthComboEffect = short.Parse(currentLine[5]);
                        }
                        */
                    }
                    else if (currentLine.Length > 2 && currentLine[1] == "CELL")
                    {
                        // investigate
                    }
                    else if (currentLine.Length > 1 && currentLine[1] == "Z_DESC")
                    {
                        // investigate
                        if (DAOFactory.Instance.SkillDAO.LoadById(skill.SkillVNum) != null)
                        {
                            continue;
                        }

                        skills.Add(skill);
                        counter++;
                    }
                }

                DAOFactory.Instance.SkillDAO.Insert(skills);
                DAOFactory.Instance.ComboDAO.Insert(combo);
                DAOFactory.Instance.BCardDAO.Insert(skillCards);

                Logger.Log.Info(string.Format(Language.Instance.GetMessageFromKey("SKILLS_PARSED"), counter));
            }
        }

        public void LoadMaps()
        {
            _maps = DAOFactory.Instance.MapDAO.LoadAll().ToList();
        }

        public void ImportMapType()
        {
            List<MapTypeDTO> list = DAOFactory.Instance.MapTypeDAO.LoadAll().ToList();
            var mt1 = new MapTypeDTO
            {
                MapTypeId = (short)MapTypeEnum.Act1,
                MapTypeName = "Act1",
                PotionDelay = 300,
                RespawnMapTypeId = (long)RespawnType.DefaultAct1,
                ReturnMapTypeId = (long)RespawnType.ReturnAct1
            };
            if (list.All(s => s.MapTypeId != mt1.MapTypeId))
            {
                DAOFactory.Instance.MapTypeDAO.Insert(ref mt1);
            }

            var mt2 = new MapTypeDTO
            {
                MapTypeId = (short)MapTypeEnum.Act2,
                MapTypeName = "Act2",
                PotionDelay = 300,
                RespawnMapTypeId = (long)RespawnType.DefaultAct1,
                ReturnMapTypeId = (long)RespawnType.ReturnAct1
            };
            if (list.All(s => s.MapTypeId != mt2.MapTypeId))
            {
                DAOFactory.Instance.MapTypeDAO.Insert(ref mt2);
            }

            var mt3 = new MapTypeDTO
            {
                MapTypeId = (short)MapTypeEnum.Act3,
                MapTypeName = "Act3",
                PotionDelay = 300,
                RespawnMapTypeId = (long)RespawnType.DefaultAct1,
                ReturnMapTypeId = (long)RespawnType.ReturnAct1
            };
            if (list.All(s => s.MapTypeId != mt3.MapTypeId))
            {
                DAOFactory.Instance.MapTypeDAO.Insert(ref mt3);
            }

            var mt4 = new MapTypeDTO
            {
                MapTypeId = (short)MapTypeEnum.Act4,
                MapTypeName = "Act4",
                PotionDelay = 5000
            };
            if (list.All(s => s.MapTypeId != mt4.MapTypeId))
            {
                DAOFactory.Instance.MapTypeDAO.Insert(ref mt4);
            }

            var mt5 = new MapTypeDTO
            {
                MapTypeId = (short)MapTypeEnum.Act51,
                MapTypeName = "Act5.1",
                PotionDelay = 300,
                RespawnMapTypeId = (long)RespawnType.DefaultAct5,
                ReturnMapTypeId = (long)RespawnType.ReturnAct5
            };
            if (list.All(s => s.MapTypeId != mt5.MapTypeId))
            {
                DAOFactory.Instance.MapTypeDAO.Insert(ref mt5);
            }

            var mt6 = new MapTypeDTO
            {
                MapTypeId = (short)MapTypeEnum.Act52,
                MapTypeName = "Act5.2",
                PotionDelay = 300,
                RespawnMapTypeId = (long)RespawnType.DefaultAct5,
                ReturnMapTypeId = (long)RespawnType.ReturnAct5
            };
            if (list.All(s => s.MapTypeId != mt6.MapTypeId))
            {
                DAOFactory.Instance.MapTypeDAO.Insert(ref mt6);
            }

            var mt7 = new MapTypeDTO
            {
                MapTypeId = (short)MapTypeEnum.Act61,
                MapTypeName = "Act6.1",
                PotionDelay = 300,
                RespawnMapTypeId = (long)RespawnType.DefaultAct6,
                ReturnMapTypeId = (long)RespawnType.ReturnAct1
            };
            if (list.All(s => s.MapTypeId != mt7.MapTypeId))
            {
                DAOFactory.Instance.MapTypeDAO.Insert(ref mt7);
            }

            var mt8 = new MapTypeDTO
            {
                MapTypeId = (short)MapTypeEnum.Act62,
                MapTypeName = "Act6.2",
                PotionDelay = 300,
                RespawnMapTypeId = (long)RespawnType.DefaultAct6,
                ReturnMapTypeId = (long)RespawnType.ReturnAct1
            };
            if (list.All(s => s.MapTypeId != mt8.MapTypeId))
            {
                DAOFactory.Instance.MapTypeDAO.Insert(ref mt8);
            }

            var mt9 = new MapTypeDTO
            {
                MapTypeId = (short)MapTypeEnum.Act61A,
                MapTypeName = "Act6.1a", // angel camp
                PotionDelay = 300,
                RespawnMapTypeId = (long)RespawnType.DefaultAct6,
                ReturnMapTypeId = (long)RespawnType.ReturnAct1
            };
            if (list.All(s => s.MapTypeId != mt9.MapTypeId))
            {
                DAOFactory.Instance.MapTypeDAO.Insert(ref mt9);
            }

            var mt10 = new MapTypeDTO
            {
                MapTypeId = (short)MapTypeEnum.Act61D,
                MapTypeName = "Act6.1d", // demon camp
                PotionDelay = 300,
                RespawnMapTypeId = (long)RespawnType.DefaultAct6,
                ReturnMapTypeId = (long)RespawnType.ReturnAct1
            };
            if (list.All(s => s.MapTypeId != mt10.MapTypeId))
            {
                DAOFactory.Instance.MapTypeDAO.Insert(ref mt10);
            }

            var mt11 = new MapTypeDTO
            {
                MapTypeId = (short)MapTypeEnum.CometPlain,
                MapTypeName = "CometPlain",
                PotionDelay = 300,
                RespawnMapTypeId = (long)RespawnType.DefaultAct1,
                ReturnMapTypeId = (long)RespawnType.ReturnAct1
            };
            if (list.All(s => s.MapTypeId != mt11.MapTypeId))
            {
                DAOFactory.Instance.MapTypeDAO.Insert(ref mt11);
            }

            var mt12 = new MapTypeDTO
            {
                MapTypeId = (short)MapTypeEnum.Mine1,
                MapTypeName = "Mine1",
                PotionDelay = 300,
                RespawnMapTypeId = (long)RespawnType.DefaultAct1,
                ReturnMapTypeId = (long)RespawnType.ReturnAct1
            };
            if (list.All(s => s.MapTypeId != mt12.MapTypeId))
            {
                DAOFactory.Instance.MapTypeDAO.Insert(ref mt12);
            }

            var mt13 = new MapTypeDTO
            {
                MapTypeId = (short)MapTypeEnum.Mine2,
                MapTypeName = "Mine2",
                PotionDelay = 300,
                RespawnMapTypeId = (long)RespawnType.DefaultAct1,
                ReturnMapTypeId = (long)RespawnType.ReturnAct1
            };
            if (list.All(s => s.MapTypeId != mt13.MapTypeId))
            {
                DAOFactory.Instance.MapTypeDAO.Insert(ref mt13);
            }

            var mt14 = new MapTypeDTO
            {
                MapTypeId = (short)MapTypeEnum.MeadowOfMine,
                MapTypeName = "MeadownOfPlain",
                PotionDelay = 300,
                RespawnMapTypeId = (long)RespawnType.DefaultAct1,
                ReturnMapTypeId = (long)RespawnType.ReturnAct1
            };
            if (list.All(s => s.MapTypeId != mt14.MapTypeId))
            {
                DAOFactory.Instance.MapTypeDAO.Insert(ref mt14);
            }

            var mt15 = new MapTypeDTO
            {
                MapTypeId = (short)MapTypeEnum.SunnyPlain,
                MapTypeName = "SunnyPlain",
                PotionDelay = 300,
                RespawnMapTypeId = (long)RespawnType.DefaultAct1,
                ReturnMapTypeId = (long)RespawnType.ReturnAct1
            };
            if (list.All(s => s.MapTypeId != mt15.MapTypeId))
            {
                DAOFactory.Instance.MapTypeDAO.Insert(ref mt15);
            }

            var mt16 = new MapTypeDTO
            {
                MapTypeId = (short)MapTypeEnum.Fernon,
                MapTypeName = "Fernon",
                PotionDelay = 300,
                RespawnMapTypeId = (long)RespawnType.DefaultAct1,
                ReturnMapTypeId = (long)RespawnType.ReturnAct1
            };
            if (list.All(s => s.MapTypeId != mt16.MapTypeId))
            {
                DAOFactory.Instance.MapTypeDAO.Insert(ref mt16);
            }

            var mt17 = new MapTypeDTO
            {
                MapTypeId = (short)MapTypeEnum.FernonF,
                MapTypeName = "FernonF",
                PotionDelay = 300,
                RespawnMapTypeId = (long)RespawnType.DefaultAct1,
                ReturnMapTypeId = (long)RespawnType.ReturnAct1
            };
            if (list.All(s => s.MapTypeId != mt17.MapTypeId))
            {
                DAOFactory.Instance.MapTypeDAO.Insert(ref mt17);
            }

            var mt18 = new MapTypeDTO
            {
                MapTypeId = (short)MapTypeEnum.Cliff,
                MapTypeName = "Cliff",
                PotionDelay = 300,
                RespawnMapTypeId = (long)RespawnType.DefaultAct1,
                ReturnMapTypeId = (long)RespawnType.ReturnAct1
            };
            if (list.All(s => s.MapTypeId != mt18.MapTypeId))
            {
                DAOFactory.Instance.MapTypeDAO.Insert(ref mt18);
            }

            var mt19 = new MapTypeDTO
            {
                MapTypeId = (short)MapTypeEnum.LandOfTheDead,
                MapTypeName = "LandOfTheDead",
                PotionDelay = 300
            };
            if (list.All(s => s.MapTypeId != mt19.MapTypeId))
            {
                DAOFactory.Instance.MapTypeDAO.Insert(ref mt19);
            }

            var mt20 = new MapTypeDTO
            {
                MapTypeId = (short)MapTypeEnum.Act32,
                MapTypeName = "Act 3.2",
                PotionDelay = 300
            };
            if (list.All(s => s.MapTypeId != mt20.MapTypeId))
            {
                DAOFactory.Instance.MapTypeDAO.Insert(ref mt20);
            }

            var mt21 = new MapTypeDTO
            {
                MapTypeId = (short)MapTypeEnum.CleftOfDarkness,
                MapTypeName = "Cleft of Darkness",
                PotionDelay = 300
            };
            if (list.All(s => s.MapTypeId != mt21.MapTypeId))
            {
                DAOFactory.Instance.MapTypeDAO.Insert(ref mt21);
            }

            var mt23 = new MapTypeDTO
            {
                MapTypeId = (short)MapTypeEnum.CitadelAngel,
                MapTypeName = "AngelCitadel",
                PotionDelay = 300
            };
            if (list.All(s => s.MapTypeId != mt23.MapTypeId))
            {
                DAOFactory.Instance.MapTypeDAO.Insert(ref mt23);
            }

            var mt24 = new MapTypeDTO
            {
                MapTypeId = (short)MapTypeEnum.CitadelDemon,
                MapTypeName = "DemonCitadel",
                PotionDelay = 300
            };
            if (list.All(s => s.MapTypeId != mt24.MapTypeId))
            {
                DAOFactory.Instance.MapTypeDAO.Insert(ref mt24);
            }

            var mt25 = new MapTypeDTO
            {
                MapTypeId = (short)MapTypeEnum.Oasis,
                MapTypeName = "Oasis",
                PotionDelay = 300,
                RespawnMapTypeId = (long)RespawnType.DefaultOasis,
                ReturnMapTypeId = (long)RespawnType.DefaultOasis
            };
            if (list.All(s => s.MapTypeId != mt25.MapTypeId))
            {
                DAOFactory.Instance.MapTypeDAO.Insert(ref mt25);
            }

            var mt26 = new MapTypeDTO
            {
                MapTypeId = (short)MapTypeEnum.Act42,
                MapTypeName = "Act42",
                PotionDelay = 5000
            };
            if (list.All(s => s.MapTypeId != mt26.MapTypeId))
            {
                DAOFactory.Instance.MapTypeDAO.Insert(ref mt26);
            }

            Logger.Log.Info(Language.Instance.GetMessageFromKey("MAPTYPES_PARSED"));
        }

        public void ImportMapTypeMap()
        {
            List<MapTypeMapDTO> maptypemaps = new List<MapTypeMapDTO>();
            short mapTypeId = 1;
            for (int i = 1; i < 300; i++)
            {
                bool objectset = false;
                if (i < 3 || i > 48 && i < 53 || i > 67 && i < 76 || i == 102 || i > 103 && i < 105 || i > 144 && i < 149)
                {
                    // "act1"
                    mapTypeId = (short)MapTypeEnum.Act1;
                    objectset = true;
                }
                else if (i > 19 && i < 34 || i > 52 && i < 68 || i > 84 && i < 101)
                {
                    // "act2"
                    mapTypeId = (short)MapTypeEnum.Act2;
                    objectset = true;
                }
                else if (i > 40 && i < 45 || i > 45 && i < 48 || i > 99 && i < 102 || i > 104 && i < 128)
                {
                    // "act3"
                    mapTypeId = (short)MapTypeEnum.Act3;
                    objectset = true;
                }
                else if (i == 260)
                {
                    // "act3.2"
                    mapTypeId = (short)MapTypeEnum.Act32;
                    objectset = true;
                }
                else if (i > 129 && i <= 134 || i == 135 || i == 137 || i == 139 || i == 141 || i > 150 && i < 153)
                {
                    // "act4"
                    mapTypeId = (short)MapTypeEnum.Act4;
                    objectset = true;
                }
                else if (i == 153)
                {
                    // "act4.2"
                    mapTypeId = (short)MapTypeEnum.Act42;
                    objectset = true;
                }
                else if (i > 169 && i < 205)
                {
                    // "act5.1"
                    mapTypeId = (short)MapTypeEnum.Act51;
                    objectset = true;
                }
                else if (i > 204 && i < 221)
                {
                    // "act5.2"
                    mapTypeId = (short)MapTypeEnum.Act52;
                    objectset = true;
                }
                else if (i > 228 && i < 233)
                {
                    // "act6.1a"
                    mapTypeId = (short)MapTypeEnum.Act61;
                    objectset = true;
                }
                else if (i > 232 && i < 238)
                {
                    // "act6.1d"
                    mapTypeId = (short)MapTypeEnum.Act61;
                    objectset = true;
                }
                else if (i > 239 && i < 251 || i == 299)
                {
                    // "act6.2"
                    mapTypeId = (short)MapTypeEnum.Act62;
                    objectset = true;
                }
                else if (i > 260 && i < 264 || i > 2614 && i < 2621)
                {
                    // "Oasis"
                    mapTypeId = (short)MapTypeEnum.Oasis;
                    objectset = true;
                }
                else if (i == 103)
                {
                    // "Comet plain"
                    mapTypeId = (short)MapTypeEnum.CometPlain;
                    objectset = true;
                }
                else if (i == 6)
                {
                    // "Mine1"
                    mapTypeId = (short)MapTypeEnum.Mine1;
                    objectset = true;
                }
                else if (i > 6 && i < 9)
                {
                    // "Mine2"
                    mapTypeId = (short)MapTypeEnum.Mine2;
                    objectset = true;
                }
                else if (i == 3)
                {
                    // "Meadown of mine"
                    mapTypeId = (short)MapTypeEnum.MeadowOfMine;
                    objectset = true;
                }
                else if (i == 4)
                {
                    // "Sunny plain"
                    mapTypeId = (short)MapTypeEnum.SunnyPlain;
                    objectset = true;
                }
                else if (i == 5)
                {
                    // "Fernon"
                    mapTypeId = (short)MapTypeEnum.Fernon;
                    objectset = true;
                }
                else if (i > 9 && i < 19 || i > 79 && i < 85)
                {
                    // "FernonF"
                    mapTypeId = (short)MapTypeEnum.FernonF;
                    objectset = true;
                }
                else if (i > 75 && i < 79)
                {
                    // "Cliff"
                    mapTypeId = (short)MapTypeEnum.Cliff;
                    objectset = true;
                }
                else if (i == 150)
                {
                    // "Land of the dead"
                    mapTypeId = (short)MapTypeEnum.LandOfTheDead;
                    objectset = true;
                }
                else if (i == 138)
                {
                    // "Cleft of Darkness"
                    mapTypeId = (short)MapTypeEnum.CleftOfDarkness;
                    objectset = true;
                }
                else if (i == 130)
                {
                    // "Citadel"
                    mapTypeId = (short)MapTypeEnum.CitadelAngel;
                    objectset = true;
                }
                else if (i == 131)
                {
                    mapTypeId = (short)MapTypeEnum.CitadelDemon;
                    objectset = true;
                }

                // add "act6.1a" and "act6.1d" when ids found
                if (objectset && DAOFactory.Instance.MapDAO.LoadById((short)i) != null && DAOFactory.Instance.MapTypeMapDAO.LoadByMapAndMapType((short)i, mapTypeId) == null)
                {
                    maptypemaps.Add(new MapTypeMapDTO { MapId = (short)i, MapTypeId = mapTypeId });
                }
            }

            DAOFactory.Instance.MapTypeMapDAO.Insert(maptypemaps);
        }

        //Need fix
        public void ImportRespawnMapType()
        {
            List<RespawnMapTypeDTO> respawnmaptypemaps = new List<RespawnMapTypeDTO>
            {
                new RespawnMapTypeDTO
                {
                    RespawnMapTypeId = (long)RespawnType.DefaultAct1,
                    DefaultMapId = 1,
                    DefaultX = 80,
                    DefaultY = 116,
                    Name = "Default"
                },
                new RespawnMapTypeDTO
                {
                    RespawnMapTypeId = (long)RespawnType.ReturnAct1,
                    DefaultMapId = 0,
                    DefaultX = 0,
                    DefaultY = 0,
                    Name = "Return"
                },
                new RespawnMapTypeDTO
                {
                    RespawnMapTypeId = (long)RespawnType.DefaultAct5,
                    DefaultMapId = 170,
                    DefaultX = 86,
                    DefaultY = 48,
                    Name = "DefaultAct5"
                },
                new RespawnMapTypeDTO
                {
                    RespawnMapTypeId = (long)RespawnType.ReturnAct5,
                    DefaultMapId = 0,
                    DefaultX = 0,
                    DefaultY = 0,
                    Name = "ReturnAct5"
                },
                new RespawnMapTypeDTO
                {
                    RespawnMapTypeId = (long)RespawnType.DefaultAct6,
                    DefaultMapId = 228,
                    DefaultX = 72,
                    DefaultY = 102,
                    Name = "DefaultAct6"
                },
                new RespawnMapTypeDTO
                {
                    RespawnMapTypeId = (long)RespawnType.DefaultAct62,
                    DefaultMapId = 228,
                    DefaultX = 72,
                    DefaultY = 102,
                    Name = "DefaultAct62"
                },
                new RespawnMapTypeDTO
                {
                    RespawnMapTypeId = (long)RespawnType.DefaultOasis,
                    DefaultMapId = 261,
                    DefaultX = 66,
                    DefaultY = 70,
                    Name = "DefaultOasis"
                }
            };
            DAOFactory.Instance.RespawnMapTypeDAO.Insert(respawnmaptypemaps);
            Logger.Log.Info(Language.Instance.GetMessageFromKey("RESPAWNTYPE_PARSED"));
        }
    }
}
