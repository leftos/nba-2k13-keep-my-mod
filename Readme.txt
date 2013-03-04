NBA 2K13 Keep My Mod
	by Lefteris "Leftos" Aslanoglou

	Additional credits:
		- solovoy from the NLSC forums


Starting with NBA 2K13, 2K implemented a way to update game art and text 
(actually, any game file) silently, without the need for a patch. However, the 
files 2K updates this way override any mods the user has installed, and there's 
no easy way for the user to make sure his mods override 2K's updates.

This tool tries to solve this problem, by letting the user select which mods to 
keep, and also gives modders a semi-automatic way of making sure their mods 
won't get overridden/replaced by 2K's updates.

===================================================
DISCLAIMER
The tool is still in beta, so use it at your own risk. Make sure to keep 
a full backup of your NBA 2K13 folder inside %appdata%\2K Sports, in 
case something goes wrong. 
 
When you edit the Roster.ROS file, ALWAYS save the edited Roster in a 
file WITH A DIFFERENT NAME! The Roster.ROS gets updated by 2K whenever 
there's an official update, and this tool FORCES the update. 

So, two tips:
1. Always keep a backup of %appdata%\2K Sports\NBA 2K13. Always, always, ALWAYS.
2. When you want to edit the official Roster, always create a copy of it 
first with a different name, and always save to that file. Saving to the 
Roster.ROS file is unreliable even without my tool, but you're 
guaranteed to lose your changes with it, if you don't save to another 
file! 
===================================================

Instructions
	For users (short version):
		Just run the tool, and click on Start Game. You should be all set. The 
		tool is fully-automatic in what it does, and shouldn't require further 
		assistance from you. Just keep it running while you're playing the game.
		Make sure you read the rest of the readme though, for more things you 
		can do.
		
		Quick note though, to avoid having the game resync its updates when
		you quit a game and return to the Main Menu (which takes about 10-15
		minutes during which you don't have the Online Data updates, such as the
		new shoes), remember this:
		- When you're about to enter a game, Alt-Tab into the tool and click
		   on Force Keep My Mods.
		- When you're about to quit a game and return to the Main Menu, Alt-Tab
		   into the tool, and click on Restore Online Data Backup.
		If you remember to do these two things, you'll always have both your
		mods and the rest of 2K's updates, without having to wait through the
		lengthy update.

		If you want to temporarily disable the tool and only keep 2K's updates,
		for example to synchronize NBA Today or to go Online, click on Restore
		Online Data Backup. After you do that, the tool won't watch for re-syncs
		and won't force your mods, until you use the "Force Keep My Mods"
		option, which will re-enable the automatic features of the tool.

	For users (detailed version):
		When the tool starts, it makes sure to hide any rosters currently 
		available. This way it forces the game to download the latest roster and
		updates. After the game downloads the updates and saves the latest
		roster, the tool forces the mods over 2K's conflicting updates. It then
		unhides any custom rosters you may have, and you can load them via
		Options > Load/Save > Load. As long as the tool is running, it keeps
		watching for the game's silent re-syncs and updates from the 2K servers,
		and forces the mods again over 2K's updates whenever it needs to, 
		keeping your game both fresh and modded all the time.
	
			NOTE 1: This hack is the only way this tool will know when the 
			update's over, and when it can replace 2K's updates with the mods 
			you've selected to keep. If you don't allow it to hide the rosters, 
			you'll have to tell it when to replace 2K's updates yourself. Read 
			below.

			NOTE 2: The game won't let you override any of 2K's updates with mods 
			if you try to play against others Online by using the Online option.
			It will resync all updates from the server, and again override any
			mods it has updated. However, once you're done playing online, you 
			can restore your mods by going to the Home screen in-game, and using
			the "Force Keep My Mods" option, described below.

			NOTE 3: Make sure you always run the tool BEFORE you start the game.
			The tool goes through some preparations when it starts to make sure
			the game updates and that everything goes well; its behaviour if
			started while NBA 2K13 is running hasn't been tested and may cause
			the game to crash.

			NOTE 3.1: Since v0.1.5, the tool will start even while the game's
			running, and will not hide the rosters or restore 2K's online data,
			but will force your mods. It will automatically start checking for 
			re-syncs and forcing your mods when needed, as always.

			NOTE 4: If you're trying to use NBA Today or Online and the game 
			complains about the synchronization having failed, use the Restore 
			Online Data Backup feature; it's explained in detail below. See also
			notes 6 and 6.1.

			NOTE 5: You can keep the tool open between closing and restarting
			the game, but it's recommended if you restart the tool each time
			you exit the game and want to start it again. You can still get the
			same effect by using
				1. Restore Online Data Backup
				2. Hide Rosters (Forces Update)
			the one after the other in that order, and then starting the game.

			NOTE 6: If you want to temporarily disable the tool and keep 2K's 
			updates, for example to synchronize NBA Today or to go Online, click
			on Restore Online Data Backup. After you do that, the tool won't 
			watch for re-syncs and won't force your mods, until you use the 
			"Force Keep My Mods" option, which will re-enable the automatic 
			features of the tool.

			NOTE 6.1: To be able to enter NBA Today or Online without any 
			delays, wait until the "Re-syncing..." prompt has disappeared from 
			below the "Online Data Browser" button, and then use "Restore Online
			Data Backup".

		From the main screen, you can do the following:
		- Start Game
			Starts the game, and watches for any updates. Once the bootup 
			sequence is complete, the game should have created a new roster with
			the latest updates. That's when the tool replaces any of 2K's 
			updates	that conflict with your mods, with the actual mods. Also, it
			unhides any custom rosters you had, other than 2K's official one, so
			that you can load them using Options > Load/Save.
		- Restore Custom Rosters
			If for some reason you can't find your custom rosters in the 
			Load/Save screen, use this button. Normally	you shouldn't have to 
			use this button at all. Note that this feature will temporarily
			disable the tool's automatic capabilities, if used before the game
			starts.
		- Restore Original Roster Backup
			If you don't want the game to update its roster, or if the roster 
			the game created has older data than the one you last had (this 
			could happen if an error occurs while the game downloads the latest 
			roster), you can restore the backup this tool keeps of your original
			roster and replace the current one. Note that this feature will 
			temporarily	disable the tool's automatic capabilities, if used 
			before the game	starts.
		- Hide Rosters (Forces Update)
			Use this button to hide the rosters manually. Only useful if you 
			restored the original and/or the custom	rosters using one of the 
			options above. This is run automatically when the tool starts, so 
			usually you	don't have to do this yourself. This will re-enable
			the tool's automatic capabilities if disabled.
		- Force Keep My Mods
			If for some reason you feel your mods were replaced by 2K's updates,
			you can force-replace 2K's updates	with your mods using this 
			option. Only use this if the automatic replacement didn't work, or 
			if you didn't allow the tool to automatically replace 2K's updates 
			(e.g. if you restored the original and/or custom rosters before 
			starting the game). Also useful after being done playing online, and
			wanting to get your mods back. This will re-enable the tool's 
			automatic capabilities if disabled.
		- Restore Online Data Backup
			Use this option to restore the Online Data backup the tool keeps
			whenever it replaces 2K's updates with your mods (either automa-
			tically or manually using the "Force Keep My Mods" option). This
			feature exists because if the game finds any mods during an NBA
			Today update (which is essentially an Online Data sync) inside the
			Online Data folder, it freaks out and deletes the whole folder,
			and forces you to wait while it downloads everything again. Using
			this option will make sure that the game finds everything exactly
			the way it expects it to be, and keep calm. You'll have to use
			Force Keep My Mods however if you want to get your mods back, or
			restart the tool, as its automatic capabilities will be disabled.

				NOTE: This feature is useful if you want to use NBA Today or
				Online, since those features require the Online Data to be
				pure 2K updates, without any mods. When you use this feature,
				the tool's automatic features are temporarily disabled, until
				you re-enable them using "Force Keep My Mods". You can still
				combine NBA Today and your mods, by first restoring the
				Online Data backup, letting the game synchronize (you'll know
				this has happened if the NBA Today game in the Home screen has
				the words "Featured Game" in the middle), and then using the
				"Force Keep My Mods" feature. This way you can play an NBA
				Today game, and still keep your mods.

		- Online Data Browser (Select Mods To Keep)
			This tool allows mod developers to automatically have their mods 
			recognized by it and kept over 2K's	updates. However, if you've 
			installed a mod from a developer who is unaware of "Keep My Mod", 
			and doesn't give you such an option, you can manually select which 
			files that 2K has updated you want to keep modded by using this 
			screen.

				NOTE: Not all files included in modlists will be shown in this
				window. Only those that 2K has updated can be switched on and
				off. If 2K hasn't updated the files, there's no reason for the
				tool to try to "protect" them. You shouldn't delete the 
				modlists containing extra files though, as you never know when
				2K will decide to update them; then those modlists will be
				used again.

		- Create Modlist
			This feature is for modders only		
		- Save Logs
			If you're asked to provide the tool's logs for troubleshooting, 
			click on this button. You'll find 2	text files waiting on your 
			desktop, named "KeepMyMods_log.txt" and "KeepMyMods_mods.txt". 
			You'll be told what to do with the contents. No information that can
			personally identify you is kept, other than	maybe your Windows 
			username (which you can blank out by Search/Replacing for it in 
			"KeepMyMods_log.txt") and the mods you have told the tool to keep.

	For modders:
		To make sure the tool automatically keeps your mod over 2K's updates on 
		files you have modded, include a "YourModNameHere.modlist" file in a 
		"KeepMyMods" subdirectory. Inside it, include all files your mod
		replaces, separating each one with a new line. You can do this in a
		more automatic way using the "Create Modlist" feature inside the tool,
		which creates the modlist file for you after you select the files,
		instead of having you type them one-by-one.

			Example:
			Filename: Pistons Court Update by The Greatest Modder.modlist

			Contents:
			f021.iff
			s021.iff
			team_021.iff

		Each time the tool starts and whenever the Online Data Browser is 
		opened, the tool looks for any files with a .modlist extension in the 
		KeepMyMods subfolder, inside the game's installation folder.

		So a proper way to pack your mod would be...
			Example:
			Filename: Pistons Court Update.zip

			Contents:
			KeepMyMods\Pistons Court Update by The Greatest Modder.modlist
			f021.iff
			s021.iff
			team_021.iff
		This way, when the user extracts all the files from the archive into the
		game's installation folder,	the modlist file goes inside the KeepMyMods 
		subfolder, inside the game's installation folder.

		You can verify this by going into the Online Data Browser after having 
		created the proper modlist file inside the KeepMyMods subfolder. If your
		mod's files are in the Keep My Mods column, you've done	it correctly.

Troubleshooting
	You can ask me for help whether you're a user or a modder on the tool's 
	thread in the NBA Live Series Center forums.

	URL: http://forums.nba-live.com/viewtopic.php?f=149&t=88654