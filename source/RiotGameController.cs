using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Playnite;
using Playnite.Common;
using Playnite.SDK;
using Playnite.SDK.Models;
using Playnite.SDK.Plugins;

namespace Riot
{
    public class RiotInstallController : InstallController
    {
        private CancellationTokenSource watcherToken;

        public RiotInstallController(Game game) : base(game)
        {
            if (RiotChecks.IsInstalled)
            {
                Name = "Install with Riot Client";
            }
            else
            {
                Name = "Download Client";
            }
        }

        public override void Dispose()
        {
            watcherToken?.Cancel();
        }

        public override void Install(InstallActionArgs args)
        {
            if (RiotChecks.IsInstalled)
            {
                ProcessStarter.StartProcess(RiotChecks.ClientExecPath);
            }
            else
            // Picking Legends of Runeterra client as launcher of a choice here.
            // Riots didn't bothered themselves with unified launcher installer.
            // LoR's one is region-independant (at least under usual usage flow).
            {
                ProcessStarter.StartUrl(@"https://bacon.secure.dyn.riotcdn.net/channels/public/x/installer/current/live.live.americas.exe");
            }

            StartInstallWatcher();
        }

        public async void StartInstallWatcher()
        {
            watcherToken = new CancellationTokenSource();
            await Task.Run(async () =>
            {
                while (true)
                {
                    if (watcherToken.IsCancellationRequested)
                    {
                        return;
                    }
                    if (File.Exists(RiotChecks.InstallationPath))
                    {
                        InvokeOnInstalled(new GameInstalledEventArgs());
                        return;
                    }

                    await Task.Delay(10000);
                }
            });
        }
    }

    public class RiotUninstallController : UninstallController
    {
        private CancellationTokenSource watcherToken;

        public RiotUninstallController(Game game) : base(game)
        {
            Name = "Uninstall";
        }

        public override void Dispose()
        {
            watcherToken?.Cancel();
        }

        public override void Uninstall(UninstallActionArgs args)
        {
            Dispose();
            if (!File.Exists(RiotChecks.ClientExecPath))
            {
                throw new FileNotFoundException("Unable to find Riot Client.");
            }
            // Since Riots didn't bothered themselves to include proper uninstaller into launcher,
            // we have to either direct end user to ALL folders where everything needs to be removed
            // or open useless (for games which were NOT bundled with the installer) Windows Uninstaller
            // then play possum.
            ProcessStarter.StartProcess("appwiz.cpl");
            StartUninstallWatcher();
        }

        public async void StartUninstallWatcher()
        {
            watcherToken = new CancellationTokenSource();
            while (true)
            {
                if (watcherToken.IsCancellationRequested)
                {
                    return;
                }

                if (!File.Exists(RiotChecks.InstallationPath))
                {
                    InvokeOnUninstalled(new GameUninstalledEventArgs());
                    return;
                }

                await Task.Delay(5000);
            }
        }
    }
}
