# PS4OfflineAccountActivator

Activates PSN account on jailbroken PS4 allowing you to export save data to USB among other things. The offsets are for 5.05 fw version.

Requires [ps4debug](https://github.com/jogolden/ps4debug) to compile

## Notes & Warnings

It's better to use this program on a new account. If you use it on an old account (with saves and trophies) you'll encounter these problems:
1. You won't be able to use your old save files easily (the ones created before activation). They'll show as broken. Maybe you can recover them with [Playstation-4-Save-Mounter](https://github.com/ChendoChap/Playstation-4-Save-Mounter). 
2. You'll have to delete your trophies (via FTP) because they will be signed with the unactivated account and all the games you try to launch will error out.

I repeat, I recommend to use a fresh console account for the activation, but do as you wish...


## How to use

Change the source code to:
1. Put your console IP address
2. Put the user number of the account you want to activate (1 if you only have one account in the console)
3. Put your psn account id (in the code you have two methods to find it)

Then:
1. Launch ps4debug on your PS4
2. Launch this program on your computer
3. Power off your PS4
4. Power on your PS4 and enjoy the export capabilities.
4.1. Of if you used an old account fix the problems in the Notes & Warnings section

## Credits

Made by barthen

Thanks to jogolden for the great ps4debug and to all the PS4 scene for making this possible.
