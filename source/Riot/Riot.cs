using Playnite.SDK;
using Playnite.SDK.Data;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
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
        string LeagueOfLegendsInstallDirectory = Path.Combine(RiotChecks.InstallationPath, "").Replace("Riot Client", "League of Legends");
        bool LeagueOfLegendsInstalled
        {
            get
            {
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

        string LegendsOfRuneterraInstallDirectory = Path.Combine(RiotChecks.InstallationPath, "").Replace("Riot Client", "LoR");
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

        string ValorantInstallDirectory = Path.Combine(RiotChecks.InstallationPath, "").Replace("Riot Client", "VALORANT");
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