#!/bin/bash

# This script will find the first folder in the directory it was executed in
# and archive everything needed for a mod in that folder into .rmod file

# This script can also automatically install the mod if you replace "false" with "true" in "install=false" below.
# !!!WARNING!!! You also have to set path to your Raft directory in "raftdir", you can find it using Steam.
install=true
raftdir=~/.local/share/Steam/steamapps/common/Raft/
# Defaults are:
# Flatpak Steam = ~/.var/app/com.valvesoftware.Steam/.steam/steam/steamapps/common/Raft
# Native Steam = ~/.steam/steam/steamapps/common/Raft/


# =========== The Script ==============
# Check if zip is installed
if ! type "zip" > /dev/null; then
    echo "zip isn't available"
    exit
    fi

# Define name of the first folder in the current directory
foldername=$(basename $(ls -d */ | head -n 1))
if [ -e "$foldername.rmod" ]; then rm "$foldername.rmod"; echo "Removing old $foldername.rmod"; fi

# Zip all files in the sub directory named as the current directory except for everything after "-x"
cd $foldername && zip -r -o "../$foldername.rmod" "." -x "obj/*" "bin/*" "*.csproj" "*.rmod" "*.sln" "build.sh" "build.bat" && cd ..

# Install if enabled
if $install; then
    echo "Installing the mod"
    # Create mods dir if doesn't exist
    if [ ! -e "$raftdir/mods" ]; then mkdir "$raftdir/mods"; fi
    # Copy the mod into the mods dir
    cp $foldername.rmod $raftdir/mods/$foldername.rmod; fi
echo "Finished"
exit