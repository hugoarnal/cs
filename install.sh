#!/usr/bin/bash

echo "Downloading latest cs version:"
sudo curl -o /usr/bin/cs https://raw.githubusercontent.com/hugoarnal/cs/main/cs
sudo chmod +x /usr/bin/cs
