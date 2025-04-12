#!/usr/bin/bash

if [ -e "/usr/bin/cs" ]; then
    line=$(head -n 1 "/usr/bin/cs")
    if [ "$line" == "#!/usr/bin/env python3" ]; then
        echo "You seem to have an outdated cs version located at /usr/bin/cs."
        echo "It needs to be removed to install the new cs version."
        exit 1
    fi
fi

echo "Downloading latest cs version:"
sudo curl -o /usr/local/bin/cs https://raw.githubusercontent.com/hugoarnal/cs/main/cs
sudo chmod +x /usr/local/bin/cs
echo ""

echo "Make sure you have /usr/local/bin in your PATH variable for cs to run."
