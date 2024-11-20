# Counter Style
Yet another Coding Style Checker script.

> [!WARNING]
> To update the docker image, you need to run `cs --update`.
>
> Make sure to run it around once a month.
>
> Auto updates might be coming in a later update (whenever I have time).

> [!WARNING]
> This script is subject to be changed often.
>
> I made this script for me mostly, if you want to contribute, feel free to open a PR!

(yes this is a double warning.)

# Install

```sh
git clone https://github.com/hugoarnal/cs
cd cs
./install.sh
```

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

I also know the [Mango (RIP)](https://github.com/Clement-Z4RM/Mango/) script exists, but it's even longer than any script and the space it takes with the Mango logo is just huge.
