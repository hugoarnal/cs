#!/usr/bin/bash

if [ -e "/usr/bin/cs" ]; then
    line=$(head -n 1 "/usr/bin/cs")
    if [ "$line" == "#!/usr/bin/env python3" ]; then
        echo "You seem to have an outdated cs version located at /usr/bin/cs."
        echo "It needs to be removed to install the new cs version."
        echo "If the file located at /usr/bin/cs is not the Counter Style script, do not proceed."
        read -p "Would you like to proceed? (y/n) " proceed
        echo ""
        if [ "$proceed" == "y" ]; then
            echo "Proceeding with removing the /usr/bin/cs script."
            sudo rm /usr/bin/cs
        else
            echo "Not proceeding with removing the /usr/bin/cs file."
        fi
    fi
    echo ""
fi

echo "Downloading latest cs version:"
sudo curl -o /usr/local/bin/cs https://raw.githubusercontent.com/hugoarnal/cs/main/cs
sudo chmod +x /usr/local/bin/cs
echo ""

echo "Make sure you have /usr/local/bin in your PATH variable for cs to run."
