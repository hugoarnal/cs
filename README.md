# Counter Style
Yet another Coding Style Checker script.

> [!WARNING]
> This script is subject to be changed VERY often.
>
> I made this script for my usage.
>
> If you still wish to contribute, feel free to open a PR!

# Install

```sh
curl -s https://raw.githubusercontent.com/hugoarnal/cs/main/install.sh | sh
```

# Updates
To update the docker image, you need to run `cs --update`.

Make sure to run it around once a month or whenever the official docker image gets updated.

# Usage

```
cs -h
```

# Credits:
- [Mainly BananaSplit](https://github.com/Ardorax/BananaSplit/)
- [Official coding-style-checker](https://github.com/Epitech/coding-style-checker)

#### Main reason behind this script
I was using [BananaSplit](https://github.com/Ardorax/BananaSplit/) before I made this script, but I wanted to make mine for a few reasons:

- On rare occasions, it could take quite long because everytime you ran the script it tries pulling a new docker image (which is also default behavior for the vanilla official coding-style checker script).
- BananaSplit doesn't specify the amount of errors once it ran.
- You cannot choose to keep the .log.
- You have to specify the path everytime you run it.

I also know the [Mango](https://github.com/Clement-Z4RM/Mango/) script exists, but it's even longer than any script and the space it takes with the Mango logo is just huge.
