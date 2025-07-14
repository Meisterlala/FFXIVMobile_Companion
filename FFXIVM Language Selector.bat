@echo off
set arg1=%1
chcp 65001 >NUL

REM Add LDPlayer support
REM Add checking for ADB and downloading an ADB zip from Github if not found

ECHO [96m███████╗███████╗██╗  ██╗██╗██╗   ██╗███╗   ███╗    ██╗     ███████╗
ECHO ██╔════╝██╔════╝╚██╗██╔╝██║██║   ██║████╗ ████║    ██║     ██╔════╝
ECHO █████╗  █████╗   ╚███╔╝ ██║██║   ██║██╔████╔██║    ██║     ███████╗
ECHO ██╔══╝  ██╔══╝   ██╔██╗ ██║╚██╗ ██╔╝██║╚██╔╝██║    ██║     ╚════██║
ECHO ██║     ██║     ██╔╝ ██╗██║ ╚████╔╝ ██║ ╚═╝ ██║    ███████╗███████║
ECHO ╚═╝     ╚═╝     ╚═╝  ╚═╝╚═╝  ╚═══╝  ╚═╝     ╚═╝    ╚══════╝╚══════╝[0m
ECHO [Final Fantasy XIV Mobile Language Selector, created by Aida Enna/Thaun]
ECHO Source code: [94mhttps://github.com/Aida-Enna/FFXIVM_Language_Selector[0m
ECHO [93m[Updated on 7/12/2025 at 3:12 AM (Fine, I'll do it myself)][0m
echo:

if not exist "adb\adb.exe" (
	ECHO [31mADB Folder/file does not exist! Please make sure you extract the entire zip folder! [0m
	PAUSE
	EXIT
)

if "%arg1%"=="full" (
echo [31m[Advanced mode enabled - Here be dragons][0m
)

:FIRSTSELECT
ECHO [32mWhat would you like to do?[0m
ECHO A. Download the latest non-UI translations
ECHO:
ECHO Or you can select to change the UI languages to one of the following:
ECHO:
ECHO 1. English (en)
ECHO 2. Japanese (ja)
ECHO 3. Korean (ko)
ECHO 4. German (de)
ECHO 5. French (fr)
ECHO 6. Chinese (zh)

if "%arg1%"=="full" (
echo [31m7. [ADV] Change language without wiping data[0m
echo [31m8. [ADV] Rename PAK Files without wiping data[0m
)

echo:
set /p langsel= Type your choice then press enter^> 

if "%langsel%" EQU "A" set lang=update_loc_db
if "%langsel%" EQU "a" set lang=update_loc_db
if "%langsel%" EQU "1" set lang=en
if "%langsel%" EQU "2" set lang=ja
if "%langsel%" EQU "3" set lang=ko
if "%langsel%" EQU "4" set lang=de
if "%langsel%" EQU "5" set lang=fr
if "%langsel%" EQU "6" set lang=zh

if "%arg1%"=="full" (
	if "%langsel%" EQU "7" set lang=adv1
	if "%langsel%" EQU "8" set lang=adv2
)


if not "%lang%"=="update_loc_db" if not "%lang%"=="en" if not "%lang%"=="ja" if not "%lang%"=="ko" if not "%lang%"=="de" if not "%lang%"=="fr" if not "%lang%"=="zh" if not "%lang%"=="adv1" if not "%lang%"=="adv2" (
    ECHO [31mOne or more of your selections were invalid choices. Please try again.[0m
	echo:
	GOTO FIRSTSELECT
)

:SECONDSELECT
echo:
ECHO [32mWhat device are you using?[0m
ECHO 1. An actual phone ([36mover USB[0m)
ECHO 2. An actual phone ([36mover Wifi[0m)
ECHO 3. Mumu
ECHO 4. Bluestacks

echo:
set /p methodsel= Type your choice then press enter^> 

echo:

if "%methodsel%" EQU "1" GOTO USB
if "%methodsel%" EQU "2" GOTO WIFI
if "%methodsel%" EQU "3" GOTO MUMU
if "%methodsel%" EQU "4" GOTO BLUESTACKS

ECHO [31mOne or more of your selections were invalid choices. Please try again.[0m
echo:
GOTO SECONDSELECT


:USB
ECHO 1. Go to Settings → About Phone → Software Information
ECHO 2. Tap 'Build number' 7 times until you see 'Developer mode enabled'
ECHO 3. Return to Settings, go into Developer Options
ECHO 4. Enable USB Debugging
ECHO 5. Plug in your phone and hit 'OK' if an 'Allow USB debugging?' menu pops up
ECHO 5. Press any key once plugged in and ready
ECHO (If you have a Xiaomi device, you may need to enable "USB debugging (Security Settings)" and "Install via USB")
PAUSE
ECHO Attempting to complete task via USB...

if "%lang%"=="update_loc_db" (
	REM Set up a constant ADB Push to fix the Localization DB
	if not exist "FDataBaseLoc.db" (
		ECHO [93mThe FDataBaseLoc.db file does not exist! Attempting to download... [0m
		curl -L "https://github.com/Aida-Enna/FFXIVM_Language_Selector/blob/main/other/FDataBaseLoc.db?raw=true" --output "FDataBaseLoc.db"
		if not exist "FDataBaseLoc.db" (
			ECHO [31mADB The FDataBaseLoc.db file STILL does not exist! Please manually download the file from the repo and place it here. [0m
			EXIT /B
		)
		ECHO [93mDownload complete! [0m
	)
	adb\adb.exe -d push "FDataBaseLoc.db" "/storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/Database" > NUL 2>&1
	adb\adb.exe -d shell chmod -w "/storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/Database/FDataBaseLoc.db"
	GOTO END
)

echo [0mClearing local game data if it exists (an error here is OK!)[36m
adb\adb.exe -d shell pm clear com.tencent.tmgp.fmgame

echo [0mCreating folder for language change[36m
adb\adb.exe -d shell mkdir -p /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Config/Android/
if "%errorlevel%"=="1" (
ECHO [31mFailed to create the folder, make sure your phone is connected and you followed all the steps![0m
ECHO [31mThe script will now exit. Please try again after fixing the above issue.[0m
PAUSE
EXIT /b
)

echo [0mCreating folder for language fix[36m
adb\adb.exe -d shell mkdir -p /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Downloader/1.0.2.0/Dolphin/Paks/
if "%errorlevel%"=="1" (
ECHO [31mFailed to create the folder, make sure your phone is connected and you followed all the steps![0m
ECHO [31mThe script will now exit. Please try again after fixing the above issue.[0m
PAUSE
EXIT /b
)

echo [0mDeleting file incase it exists (an error here is OK!)[36m
adb\adb.exe -d shell rm /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Config/Android/GameUserSettings.ini

echo [0mCreating INI file with new language[36m
adb\adb.exe -d shell echo "[Internationalization]\\nCulture=%lang%" ">>" /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Config/Android/GameUserSettings.ini
if "%errorlevel%"=="1" (
ECHO [31mFailed to create the INI file, make sure your phone is connected and you followed all the steps![0m
ECHO [31mThe script will now exit. Please try again after fixing the above issue.[0m
PAUSE
EXIT /b
)

echo [31mPlease open the FFXIV Mobile app and let it update/patch now. Do not unplug your device from the PC.[0m
echo [31mOnce it is complete and you are at the login screen, close the game and hit enter to continue.[0m
pause 

echo [0mRenaming PAK files to fix language issues[36m
adb\adb.exe -d shell mv /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Downloader/1.0.2.0/Dolphin/Paks/1.0.2.12_Android_ASTC_12_P.pak /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Downloader/1.0.2.0/Dolphin/Paks/1.0.2.12_Android_ASTC_12_P.pak.bak
if "%errorlevel%"=="1" (
ECHO [31mFailed to rename the PAK files make sure you followed the directions and updated before hitting enter![0m
ECHO [31mThe script will now exit. Please try again after fixing the above issue.[0m
PAUSE
EXIT /b
)

GOTO End

:WIFI
ECHO 1. Go to Settings → About Phone → Software Information
ECHO 2. Tap 'Build number' 7 times until you see 'Developer mode enabled'
ECHO 3. Return to Settings, go into Developer Options
ECHO 4. Go into the 'Wireless debugging' menu and enable Wireless debugging
ECHO 5. Select 'Pair device with pairing code'
ECHO 6. Enter the information that shows up below
set /p "ip=Enter the IP address & Port: "
ECHO [0mPairing... (hit enter if you are already paired)[36m
adb\adb.exe pair %ip%
adb\adb.exe disconnect
ECHO [0m
set /p "ip=Enter the IP address & Port (not the pairing one, the one in the wireless debugging settings): "
echo [0mAttempting to complete task via Wifi...[36m
GOTO PART2NOUSB

:BLUESTACKS
set ip=127.0.0.1:5555
ECHO [0mAttempting to complete task via BlueStacks...[36m
GOTO PART2NOUSB

:MUMU
set ip=127.0.0.1:7555
ECHO [0mAttempting to complete task via MuMu...[36m
GOTO PART2NOUSB

:PART2NOUSB
ECHO [0mConnecting to ADB...[36m
adb\adb connect %ip%

if "%lang%"=="update_loc_db" (
	REM Set up a constant ADB Push to fix the Localization DB
	if not exist "FDataBaseLoc.db" (
		ECHO [93mThe FDataBaseLoc.db file does not exist! Attempting to download... [0m
		curl -L "https://github.com/Aida-Enna/FFXIVM_Language_Selector/blob/main/other/FDataBaseLoc.db?raw=true" --output "FDataBaseLoc.db"
		if not exist "FDataBaseLoc.db" (
			ECHO [31mADB The FDataBaseLoc.db file STILL does not exist! Please manually download the file from the repo and place it here. [0m
			EXIT /B
		)
		ECHO [93mDownload complete! [0m
	)
	ECHO [31mFixing folders, an error here might be OK![0m
	adb\adb.exe -s %ip% shell mv /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/Database /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/Database_orig
	adb\adb.exe -s %ip% shell mkdir /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/Database
	echo [0mUpdating dialog translations[36m
	adb\adb.exe -s %ip% push "FDataBaseLoc.db" "/storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/Database"

	echo [0mLocking the file to prevent the game from overwriting it[36m
	adb\adb.exe -s %ip% shell chmod -w "/storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/Database/FDataBaseLoc.db"
	EXIT /b
)

if "%lang%"=="adv1" (
	REM Change language without resetting data
	adb\adb.exe -s %ip% shell rm /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Config/Android/GameUserSettings.ini
	adb\adb.exe -s %ip% shell echo "[Internationalization]\\nCulture=%lang%" ">>" /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Config/Android/GameUserSettings.ini
	GOTO End
)

if "%lang%"=="adv2" (
	REM Rename PAK Files without resetting data
	adb\adb.exe -s %ip% shell mv /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Downloader/1.0.2.0/Dolphin/Paks/1.0.2.12_Android_ASTC_12_P.pak /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Downloader/1.0.2.0/Dolphin/Paks/1.0.2.12_Android_ASTC_12_P.pak.bak
	GOTO End
)

echo [0mClearing local game data if it exists (an error here is OK!)[36m
adb\adb.exe -s %ip% shell pm clear com.tencent.tmgp.fmgame

echo [0mCreating folder for language change[36m
adb\adb.exe -s %ip% shell mkdir -p /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Config/Android/
if "%errorlevel%"=="1" (
ECHO [31mFailed to create the folder, make sure your phone is connected and you followed all the steps![0m
ECHO [31mThe script will now exit. Please try again after fixing the above issue.[0m
PAUSE
EXIT /b
)

echo [0mCreating folder for language fix[36m
adb\adb.exe -s %ip% shell mkdir -p /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Downloader/1.0.2.0/Dolphin/Paks/
if "%errorlevel%"=="1" (
ECHO [31mFailed to create the folder, make sure your phone is connected and you followed all the steps![0m
ECHO [31mThe script will now exit. Please try again after fixing the above issue.[0m
PAUSE
EXIT /b
)

echo [0mDeleting INI file incase it exists (an error here is OK!)[36m
adb\adb.exe -s %ip% shell rm /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Config/Android/GameUserSettings.ini

echo [0mCreating INI file with new language[36m
adb\adb.exe -s %ip% shell echo "[Internationalization]\\nCulture=%lang%" ">>" /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Config/Android/GameUserSettings.ini
if "%errorlevel%"=="1" (
ECHO [31mFailed to create the INI file, make sure your phone is connected and you followed all the steps![0m
ECHO [31mThe script will now exit. Please try again after fixing the above issue.[0m
PAUSE
EXIT /b
)

echo [31mPlease open the FFXIV Mobile app and let it update/patch now.[0m
echo [31mOnce it is complete and you are at the login screen, close the game and hit enter to continue.[0m
pause 

echo [0mRenaming PAK files to fix language issues[36m
adb\adb.exe -s %ip% shell mv /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Downloader/1.0.2.0/Dolphin/Paks/1.0.2.12_Android_ASTC_12_P.pak /storage/emulated/0/Android/data/com.tencent.tmgp.fmgame/files/UE4Game/FGame/FGame/Saved/Downloader/1.0.2.0/Dolphin/Paks/1.0.2.12_Android_ASTC_12_P.pak.bak
if "%errorlevel%"=="1" (
ECHO [31mFailed to rename the PAK files, make sure you followed the directions and updated -before- hitting enter![0m
ECHO [31mThe script will now exit. Please try again after fixing the above issue.[0m
PAUSE
EXIT /b
)

GOTO End

:End
if not "%lang%"=="adv1" if not "%lang%"=="adv2"(
echo [32mAll done - Start the game and the requested changes should be applied![0m
PAUSE
EXIT /b
)

echo [31m[ADV] Task complete.[0m
PAUSE
EXIT /b