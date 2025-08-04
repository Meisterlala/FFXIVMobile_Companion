<img margin: auto width="880" height="583" alt="logo-index 1" src="https://github.com/user-attachments/assets/38cf535d-a8ab-466d-ba52-64cdb7622779" />


# FFXIVM (FFXIV Mobile) Companion
### This works on android devices (no root required) and Mumu/Bluestacks emulators

## Prerequisites
- A computer
- [Download the exe file here](http://aida.moe/ffxiv_mobile/FFXIVMobile_Companion.exe)

 ### One of the following:
* Your phone connected to the same network as your computer (it's fine for your computer to be wired and your phone wifi)
* Your phone connected to the computer via USB
* Mumu (Rooted, enable root by going to Settings -> Other -> Enable "Root Permissions" and restarting Mumu)
* Bluestacks (Rooted, [you can learn how to root it here](<https://www.youtube.com/watch?v=EVk5vq_0vkE>))

Make sure you've enabled ADB in Bluestacks too (Settings -> Advanced -> turn on Android Debug Bridge)

## Before You Start
- If you do not have the game installed, you can download and install it through this program
- If you have the game installed but not patched, you can patch it through this program to change languages to whatever you want
- If you want to update your translations or fix issues with the game (such as all text being numbers), this program will do that too

A reminder that you should **never** install or open WeChat on your emulator, you *will* get banned. Install WeChat on your actual phone and then scan the QR code on your emulator to log in.

## Usage

1. Open the file you downloaded - If you get a warning from Windows Smart Screen select `Run anyway` - [You can check out a virus scan of the exe here](<https://www.virustotal.com/gui/url/a24ebee2f5881af4e4d8ebdd4a0e233ff56debbf9011964f34e33a96fea3d384?nocache=1>) or copy the URL and test it yourself
2. Select the language you want your game to be in
3. Select how you want to connect to your device
4. Select what you want to do:

- If you don't have the game installed, you can select to install it (option A)
- If you have the game installed but haven't done any modifications to make it into other languages yet, you'll need to do the Initial Setup (option 0)
- If you just want to change the language or update the story patch, you can do that (Option 1)

## Known Issues

* Potions are no longer usable via the quick item bag
* Some translations are wrong (Why is "track quest" translated as "Fiendish Orbs"??), missing (skill descriptions), or just plain weird (Not every NPC is Kan--e--senna game, please). We cannot modify the english translation and therefore cannot fix these. Sorry!

## Having issues?

Make sure to read all directions carefully. If you still can't get it to work, you can [join our discord](http://discord.gg/ffxivmobile) to get help.

## Donate
If you'd like to donate/support what I do, please check out the following:

[Ko-fi](https://ko-fi.com/aidaenna)

[Paypal](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=QXF8EL4737HWJ)

[Patreon](https://www.patreon.com/PSO2)

Thank you very much, every bit helps!

## Credits:
Aida-Enna (Discovering the original translation files and finding the exploit to load them and change the language/creating the original program/creating this new rewritten program)
Thaun (writing the original instructions to do this over ADB)
Deiki (For finding out the game won't touch files that are set to read-only)

## Advanced Usage

Here is a list of arguments that the program accepts. You can combine them in any order, for example:

`-ip=192.168.1.7:39065 -lang=en -wifi -nopair -updatepatch`  
would select English, select wifi, connect to the specified IP without pairing, and then update the patch

Flag: `-adv`  
Mainly for me to test features I'm in the middle of writing, unlocks 'Advanced Mode'

Flag: `-lang`  
Usage: `-lang=language_code_here`  
Example: `-lang=en`  
Used to specify which language you'd like your game to be in, skipping the selection

Valid codes are:  
`en` (English)  
`ja` (Japanese)  
`ko` (Korean)  
`de` (German)  
`fr` (French)  
`zh` (Chinese)  

Flag: `-usb`  
Used to specify that you'll be connecting over USB, skipping the selection

Flag: `-wifi`  
Used to specify that you'll be connecting over WiFi, skipping the selection

Flag: `-mumu`  
Used to specify that you'll be connecting to the MuMu emulator, skipping the selection

Flag: `-bluestacks`  
Used to specify that you'll be connecting to the BlueStacks emulator, skipping the selection  

Flag: `-nopair`  
Used to specify that you want to skip pairing your device for WiFi (useful if it's already paired)

Flag: `-ip`  
Usage: `-ip=IPAddress_and_Port`  
Example: `-ip=192.168.1.5:40850`  
Used to specify the IP you want to connect to for non-USB connections, skipping the input for it

Flag: `-initialsetup`  
Used to specify that you want to do the initial (I have the game installed but need to patch it) setup

Flag: `-updatepatch`  
Used to specify that you want to update the patch and/or change the language to the specified language
