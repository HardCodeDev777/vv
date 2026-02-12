[![Test](https://github.com/HardCodeDev777/vv/actions/workflows/test.yml/badge.svg)](https://github.com/HardCodeDev777/vv/actions/workflows/test.yml)
[![Release](https://github.com/HardCodeDev777/vv/actions/workflows/release.yml/badge.svg)](https://github.com/HardCodeDev777/vv/actions/workflows/release.yml)

# vv

Tool that gives you a clear, terminal-friendly view of your repositories — file trees, language stats, and more, right from the CLI.

## Supported Platforms
- Windows
- Linux
- macOs

## Commands

| Name                           | Description                                 |
|--------------------------------|---------------------------------------------|
| `setup`                         | Setups **vv**                              |
| `fs`                            | Shows repository filesystem info.          |
| `langs`                         | Shows detailed(tokei) languages statistic. |
| `tree`                          | Shows filesystem tree of repository.       |
| `git`                           | Shows detailed git info.                   |

## Setup

After you downloaded binary from **Releases**, it is highly recommended to add directory that contains it to PATH.

Before general using, run:
```shell
vv setup
```

> [!IMPORTANT]
> If you don't want to manually write path to repository everytime, configure repositories folder (`Do you want to locate folder for repositories?`).
>  After configuring, instead of `vv langs --path long/path/to/my/repo` you can write `vv langs` and it will suggest selection prompt with found repositories.

---

# Usage

Discover available commands and options:

```shell
vv -h
```

Show more info about a command:

```shell
vv langs -h
```
