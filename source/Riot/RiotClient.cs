using Microsoft.Win32;
using Playnite.SDK;
using Playnite.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Riot
{
    public class RiotChecksClient : LibraryClient
    {
        public override string Icon => RiotChecks.Icon;

        public override bool IsInstalled => RiotChecks.IsInstalled;

        public override void Open()
        {
            ProcessStarter.StartProcess(RiotChecks.ClientExecPath, string.Empty);
        }
    }

    public class RiotChecks
    {

        public static string ClientExecPath
        {
            get
            {
                var path = InstallationPath;
                return string.IsNullOrEmpty(path) ? string.Empty : Path.Combine(path, "RiotClientServices.exe");
            }
        }

        public static bool IsInstalled
        {
            get
            {
                if (string.IsNullOrEmpty(ClientExecPath) || !File.Exists(ClientExecPath))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        // The plugin's Sequence begins here.
        // Our #1 goal is to get the path to launcher.
        // We're using #oddlyspecific approach here. We know the key and the value - but we're not entirely sure of the path -
        // thus, abusing that coopy-paaste from Stackexchange!
        // Even better, since we don't need to launch anything else ever (for now) - it all begins here
        // and here it all ends. The rest of the code just works by example to preserve any possible third-party interactions.
        public static string InstallationPath
        {
            get
            {
            // Now, it's even funnier than with GGG since Riot write to HKEY_USERS only.
            // Fortunately, asking HKEY_CURRENT_USER seem to suffice for 90% of use cases.
            // Do note that we're grabbing proper path to launcher this time. Without executables and other sidesteps.
            // We're forced to, to utilize proper folders without extra code bloat for game install checks.
            RegistryKey uninstallKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall");
            var programs = uninstallKey.GetSubKeyNames();

            foreach (var program in programs)
            {
                RegistryKey subkey = uninstallKey.OpenSubKey(program);
                if (string.Equals("Riot Games, Inc", subkey.GetValue("Publisher", string.Empty).ToString(), StringComparison.CurrentCulture))
                {
                    // Regardless of which of FOUR games we find there, we're stripping everything.
                    // Extra instance of LOL for TFT, which, for some reason, have exclusive ID everywhere.
                    // Might fumble on beta clients and stuff (fresh instances of which, as of early 2022, shouldn't exist).
                    return subkey.GetValue("UninstallString").ToString().Replace("\\RiotClientServices.exe\" --uninstall-product=bacon --uninstall-patchline=live", "").Replace("\\RiotClientServices.exe\" --uninstall-product=valorant --uninstall-patchline=live", "").Replace("\\RiotClientServices.exe\" --uninstall-product=league_of_legends --uninstall-patchline=live", "").Replace("\\RiotClientServices.exe\" --uninstall-product=league_of_legends_game --uninstall-patchline=live", "").Replace("\"", "");
                }
            }

            return string.Empty;   
            }
        }

        public static string Icon => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"icon.png");
    }
}