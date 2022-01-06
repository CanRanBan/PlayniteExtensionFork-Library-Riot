### Riot Games Launcher library integration for Playnite

Yes, yes, all these games became available at Epic Games Store recently - but sometimes all you want is to go native...

Put it in, turn it on, trigger thy lib sync - usual plugin activation routine.

Is expected to coexist well with Steam and Epic Games Store versions of Riot Games games (no idea why you might need this though).

### Caveat Emptor

The development of this plugin was... slightly complicated by Riot Launcher's 'après nous le déluge' approach, due to which the launcher, or my integration, or both might go schizo about what's installed and what's not.

If the plugin starts seeing wrong things - delete that thing's folder from a folder one level above launcher's.

If the extension failed to find truly installed game - flip 'Installed' checkbox in game's properties in Playnite and the plugind should handle the rest just fine :)

Exercise discretion prior to installing.

-- Aliaksandr