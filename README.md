# Rottweiler

![Rottweiler](https://i.imgur.com/1E5LDuf.png)

[![Donate](https://img.shields.io/badge/Donate-PayPal-yellowgreen.svg)](https://www.paypal.me/scobalula) ![Downloads](https://img.shields.io/github/downloads/Scobalula/Rottweiler/total.svg) [![license](https://img.shields.io/github/license/Scobalula/Rottweiler.svg)]()

Rottweiler is a vicious Call of Duty sound exporter supporting several titles. It supports exporting sounds that are stored in the Fast Files and PAK (AW/MWR).

## Supported Titles

| Game                                    | Compression | Supported                        |
|-----------------------------------------|-------------|----------------------------------|
| Call of Duty: Modern Warfare            | ZLIB        | Yes                              |
| Call of Duty: World at War              | ZLIB        | Yes                              |
| Call of Duty: Modern Warfare 2          | ZLIB        | Yes                              |
| Call of Duty: Black Ops                 | ZLIB        | No (Use [BassDrop](http://aviacreations.com/wraith/#utilities-view))                |
| Call of Duty: Modern Warfare 3          | ZLIB        | Yes                              |
| Call of Duty: Black Ops 2               | ZLIB        | No (Stored in SAB's, use [Wraith Archon](http://aviacreations.com/wraith/)) |
| Call of Duty: Ghosts                    | ZLIB        | Yes                              |
| Call of Duty: Advanced Warfare          | LZ4         | Yes*                             |
| Call of Duty: Black Ops 3               | LZ4         | No (Stored in SAB's, use [Wraith Archon](http://aviacreations.com/wraith/)) |
| Call of Duty: Infinite Warfare          | LZ4         | No (Stored in SAB's, use [Wraith Archon](http://aviacreations.com/wraith/)) |
| Call of Duty: Modern Warfare Remastered | LZ4         | Yes*                             |
| Call of Duty: World War II              | LZ4         | No (Use [Ac1dRain](http://aviacreations.com/wraith/#utilities-view))                |

\*For these titles, the PAK files and the Fast Files must be in the same directy to support exporting from the PAKs.

## How to Use 

Using Rottweiler is very easy, to use Rottweiler:

1. Choose a Fast File to load (For weapon sounds, you'll usually want to load common/common_mp for any of the games).
2. Drag it onto the list view or click Load Fast File, it will proceed to decompress the fast file and search for audio.
3. Once the sounds are loaded (if there are any), you can now export sounds that are listed.

## Requirements

* Windows 7 64bit or Above (Latest Updates)
* Microsoft .NET Framework 4.5.2
* 2GB+ Disk Space (Decompressed Fast Files hit 1GB+)
* Official copies of the game/s. 

## License / Disclaimers

Rottweiler is licensed under the GPL3.0 license and it and its source code is free to use and modify. Rottweiler comes with NO warranty, any damages caused are solely the responsibility of the user. See the LICENSE file for more information.

Audio exported through Rottweiler is property of Activision and the respective developers, said copyrighted Audio CANNOT be used in a commercial environment or for profit, the intention of Rottweiler is to aid users making mods for other Call of Duty titles using their own SDKs.

All work was done purely on the fast files through a Hex Editor, legally obtained copies of the game, and no game code was touched or used, Rottweiler does not and will never rebuild/recompress fast files.

**Rottweiler is currently in alpha, and with that in mind, bugs, errors, you know, the bad stuff.**

## Download

The latest version can be found on the [Releases Page](https://github.com/Scobalula/Rottweiler/releases).

## Changelog

A changelog can be found [here](https://github.com/Scobalula/Rottweiler/blob/master/Changelog.md).

## Credits

* Milosz Krajewski ([LZ4Net](https://github.com/MiloszKrajewski/lz4net))
* Developers of FFViewer (ZLIB Based FF Information)
* Ray1235 (ZLIB Based FF Information)
* Freepik ([Rottweiler Icon](https://www.freepik.com/free-vector/breeds-of-dogs_780552.htm))
* Amazing people of Stackoverflow
## Support Me

If you use Rottweiler in any of your projects, it would be appreciated if you credit me, a lot of time and work went into developing it (I've been working on FF's on and off since I first released IW6 Extractor in July) and a simple credit isn't too much to ask for.

If you'd like to support me even more, consider donating, I develop a lot of apps including Rottweiler and majority are available free of charge with source code included:

[![Donate](https://img.shields.io/badge/Donate-PayPal-yellowgreen.svg)](https://www.paypal.me/scobalula)
