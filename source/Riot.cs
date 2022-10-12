using Playnite.SDK;
using Playnite.SDK.Data;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Riot
{
    [LoadPlugin]
    public class Riot : LibraryPluginBase<RiotSettingsViewModel>
    {
    // For Stage 2 plugins it's enough to dump game dependant intel here and call it a day. Dumpn'...
    // Do note that since LoL uses same client with TFT, we don't have to ask for paths and statuses twice, for now.
    // TODO: expell this YOBA construct into a dedicated script which would accept the name of the game and spit appropriate location.
    // Can't do it now since we're having severe case of ID vs DIR name mismatch - my skill is too low to handle it yet...
    // Can't scrap it entirely due to some Playnite SDK changes (which necessiate defining all install folders) which I can't quite grasp...
    // See https://github.com/DrinkFromTheCup/Playnite-Library-SpaceStation13/issues/1 (I believe, this one is related).
        string LeagueOfLegendsInstallDirectory
        {
            get
            {
                // Looking for default install location, one level above launcher...
                if (Directory.Exists(Path.Combine(RiotChecks.InstallationPath, "").Replace("Riot Client", "League of Legends")))
                {
                    return Path.Combine(RiotChecks.InstallationPath, "").Replace("Riot Client", "League of Legends");
                }
                else
                // Double IFs is taboo usually - but Riot gives us no other choice under given set of circumstances.
                {
                    // Now it's a trippy part. Riot doesn't save game install locations in Registry. Parsing HDD (whole dem) is an overkill.
                    // We need grabbing C:\ProgramData\Riot Games\Metadata content and praying that permissions would not screw everything up...
                    // Do note that if this check sees wrong data - launcher itself sees wrong data too. So the file below have to be removed to fix BOTH.
                    if (File.Exists("C:\\ProgramData\\Riot Games\\Metadata\\league_of_legends.live\\league_of_legends.live.product_settings.yaml"))
                    {
                        // I hate grabbing WHOLE file for a single string. It's like poaching tigers for their... whiskers. And it's a sign of developer's (my) laziness and/or incompetence.
                        // But it's the only FULLY reliable option here. Everything for the end user.
                        // Might come handy for Paradox integration in the future...
                        string text = File.ReadAllText("C:\\ProgramData\\Riot Games\\Metadata\\league_of_legends.live\\league_of_legends.live.product_settings.yaml");
                        Regex cusRegex = new Regex("product_install_full_path: .*");
                        var result = cusRegex.Match(text).Value.Replace("product_install_full_path: ", string.Empty).Replace("\"", "");
                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
        bool LeagueOfLegendsInstalled
        {
            get
            {
                // TODO: move more double checks like this one into dedicated methods during script unchungifying or something...
                // Humping end user's HDD extra time is taboo.
                if (string.IsNullOrEmpty(LeagueOfLegendsInstallDirectory) || !Directory.Exists(LeagueOfLegendsInstallDirectory))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        // Here LOL/TFT folder checks end and blatant coopy-paaste for other games start. No mishaps expected in advance.

        string LegendsOfRuneterraInstallDirectory
        {
            get
            {
                if (Directory.Exists(Path.Combine(RiotChecks.InstallationPath, "").Replace("Riot Client", "LoR")))
                {
                    return Path.Combine(RiotChecks.InstallationPath, "").Replace("Riot Client", "LoR");
                }
                else
                {
                    if (File.Exists("C:\\ProgramData\\Riot Games\\Metadata\\bacon.live\\bacon.live.product_settings.yaml"))
                    {
                        string text = File.ReadAllText("C:\\ProgramData\\Riot Games\\Metadata\\bacon.live\\bacon.live.product_settings.yaml");
                        Regex cusRegex = new Regex("product_install_full_path: .*");
                        var result = cusRegex.Match(text).Value.Replace("product_install_full_path: ", string.Empty).Replace("\"", "");
                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
        bool LegendsOfRuneterraInstalled
        {
            get
            {
                if (string.IsNullOrEmpty(LegendsOfRuneterraInstallDirectory) || !Directory.Exists(LegendsOfRuneterraInstallDirectory))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        string ValorantInstallDirectory
        {
            get
            {
                if (Directory.Exists(Path.Combine(RiotChecks.InstallationPath, "").Replace("Riot Client", "VALORANT")))
                {
                    return Path.Combine(RiotChecks.InstallationPath, "").Replace("Riot Client", "VALORANT");
                }
                else
                {
                    if (File.Exists("C:\\ProgramData\\Riot Games\\Metadata\\valorant.live\\valorant.live.product_settings.yaml"))
                    {
                        string text = File.ReadAllText("C:\\ProgramData\\Riot Games\\Metadata\\valorant.live\\valorant.live.product_settings.yaml");
                        Regex cusRegex = new Regex("product_install_full_path: .*");
                        var result = cusRegex.Match(text).Value.Replace("product_install_full_path: ", string.Empty).Replace("\"", "");
                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
        bool ValorantInstalled
        {
            get
            {
                if (string.IsNullOrEmpty(ValorantInstallDirectory) || !Directory.Exists(ValorantInstallDirectory))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    // End of game entries dump

        // Start of Riot plugin definitions
        public Riot(IPlayniteAPI api) : base(
            "Riot Launcher",
            Guid.Parse("317a5e2e-eac1-48bc-adb3-fb9e321afd3f"),
            // No need in auto-close. No need in extra settings either.
            new LibraryPluginProperties { CanShutdownClient = false, HasSettings = false },
            new RiotChecksClient(),
            RiotChecks.Icon,
            (_) => new RiotSettingsView(),
            api)
        {
            // No settings - no problem. Looks optional. I believe, API needs this empty entry anyway so keeping it as is.
        }

        public override IEnumerable<GameMetadata> GetGames(LibraryGetGamesArgs args)
        {
            return new List<GameMetadata>()
            {
                // Start of new game entry.
                new GameMetadata()
                {
                    Name = "League of Legends",
                    GameId = "league_of_legends",
                    GameActions = new List<GameAction>
                    {
                        new GameAction()
                        {
                            Name = "Play",
                            Type = GameActionType.File,
                            // Due to... robustness of game client, it might go schizo and refuse to see present installs.
                            // Resorting to launcher + command line options for this case, so it'll be enough
                            // to flip Install flag manually and enjoy the game. Same for other three games.
                            Path = RiotChecks.ClientExecPath,
                            Arguments = "--launch-product=league_of_legends --launch-patchline=live",
                            IsPlayAction = true
                        }
                    },
                    // We FINALLY have adequate install folder detection now...
                    IsInstalled = LeagueOfLegendsInstalled,
                    InstallDirectory = LeagueOfLegendsInstallDirectory,
                    Source = new MetadataNameProperty("Riot Games"),
                    Links = new List<Link>()
                    {
                        new Link("Store", @"https://www.leagueoflegends.com")
                    },
                    Platforms = new HashSet<MetadataProperty> { new MetadataSpecProperty("pc_windows") }
                },
                // End of new game entry. Do note that last entry in the list should not have comma as last symbol.
                new GameMetadata()
                {
                    Name = "Teamfight Tactics",
                    // As seen in Riot Launcher's config, just in case.
                    GameId = "league_of_legends_game",
                    GameActions = new List<GameAction>
                    {
                        new GameAction()
                        {
                            Name = "Play",
                            Type = GameActionType.File,
                            Path = RiotChecks.ClientExecPath,
                            Arguments = "--launch-product=league_of_legends --launch-patchline=live",
                            IsPlayAction = true
                        }
                    },
                    IsInstalled = LeagueOfLegendsInstalled,
                    InstallDirectory = LeagueOfLegendsInstallDirectory,
                    Source = new MetadataNameProperty("Riot Games"),
                    Links = new List<Link>()
                    {
                        new Link("Store", @"https://www.teamfighttactics.com")
                    },
                    Platforms = new HashSet<MetadataProperty> { new MetadataSpecProperty("pc_windows") }
                },
                new GameMetadata()
                {
                    Name = "Legends of Runeterra",
                    // Still Bacon...
                    GameId = "bacon",
                    GameActions = new List<GameAction>
                    {
                        new GameAction()
                        {
                            Name = "Play",
                            Type = GameActionType.File,
                            Path = RiotChecks.ClientExecPath,
                            Arguments = "--launch-product=bacon --launch-patchline=live",
                            IsPlayAction = true
                        }
                    },
                    IsInstalled = LegendsOfRuneterraInstalled,
                    InstallDirectory = LegendsOfRuneterraInstallDirectory,
                    Source = new MetadataNameProperty("Riot Games"),
                    Links = new List<Link>()
                    {
                        new Link("Store", @"https://www.legendsofruneterra.com")
                    },
                    Platforms = new HashSet<MetadataProperty> { new MetadataSpecProperty("pc_windows") }
                },
                new GameMetadata()
                {
                    Name = "Valorant",
                    GameId = "valorant",
                    GameActions = new List<GameAction>
                    {
                        new GameAction()
                        {
                            Name = "Play",
                            Type = GameActionType.File,
                            Path = RiotChecks.ClientExecPath,
                            Arguments = "--launch-product=valorant --launch-patchline=live",
                            IsPlayAction = true
                        }
                    },
                    IsInstalled = ValorantInstalled,
                    InstallDirectory = ValorantInstallDirectory,
                    Source = new MetadataNameProperty("Riot Games"),
                    Links = new List<Link>()
                    {
                        new Link("Store", @"https://playvalorant.com")
                    },
                    Platforms = new HashSet<MetadataProperty> { new MetadataSpecProperty("pc_windows") }
                }
            };
        }


        // Start of blatant install/uninstall links adding.
        // I'd really like to utilize something much more simple since we have only one entry point anyway,
        // in a form of stand-alone launcher, but we're having what we're having for now.
        public override IEnumerable<InstallController> GetInstallActions(GetInstallActionsArgs args)
        {
            if (args.Game.PluginId != Id)
            {
                yield break;
            }

            yield return new RiotInstallController(args.Game);
        }

        public override IEnumerable<UninstallController> GetUninstallActions(GetUninstallActionsArgs args)
        {
            if (args.Game.PluginId != Id)
            {
                yield break;
            }

            yield return new RiotUninstallController(args.Game);
        }
        // End of blatant install/uninstall links adding.

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return new RiotSettingsView();
        }
    }
}