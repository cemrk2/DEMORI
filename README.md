# DEMORI
A tool made to play OMORI after it has been de-listed from Xbox (on PC)

https://github.com/cemrk2/DEMORI/assets/130415487/db45435a-c9af-4e3e-8448-e45f8bc86d33

### Dumping the game

1. Download [DEMORI](https://github.com/cemrk2/DEMORI/releases)
2. Click on "Select OMORI Directory" and select where you installed OMORI, the default location is `C:\XboxGames\OMORI\Content`
3. Click on "Select Output Directory" and select where the tool should dump the game's files
4. Click on "Dump and Decrypt" and wait
5. Launch "Chowdren.exe" inside the output directory

### A note about the program

This tool doesn't "decrypt" the game's exe that's in the XboxGames folder
since the game's exe is encrypted on disk and the only way to dump it
is to launch it first then to inject a dll into the game **while its running** and copy all the files out.
The original dump of the game was made using Wunkolo's UWPDumper, and has been uploaded to [VirusTotal](https://www.virustotal.com/gui/file/31f1260026cf671db729059e896d2e7d9c801d240cdf2fa13e2582930e086562) 3 months ago

Instead this tool has a bundle of the game's 
original exe encrypted with a few of the game's files
and decrypts that exe with the game's files.

Also, please don't redistribute the decrypted game's exe / assets, 
this tool has only been made due to OMORI being de-listed and being made impossible to buy.
If OMORI gets re-listed on Xbox, I will take my tool down @cemrk on discord
