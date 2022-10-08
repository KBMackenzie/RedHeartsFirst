A Cult of the Lamb mod that adds a little 'Heart Order' menu to the game's pause menu, letting you choose what hearts you will take damage to first.

The little menu window is **draggable** and looks like this:

![Heart Order Menu](https://i.imgur.com/09P1TXs.gif)

## Installation
This mod’s only dependency is BepInEx. It's also fully compatible with my 'Weapon Selector' mod.

#### Installation (Mod Manager)
1. Download and install [r2modman](https://thunderstore.io/package/ebkr/r2modman/) or the [Thunderstore Mod Manager](https://www.overwolf.com/app/Thunderstore-Thunderstore_Mod_Manager).
2. Install this mod and all of its dependencies with the help of the mod manager! 

#### Installation (Manual)
1. Download and install BepInEx.
    1. If you're downloading it from [its Github page](https://github.com/BepInEx/BepInEx/releases), follow [this installation guide](https://docs.bepinex.dev/articles/user_guide/installation/index.html#where-to-download-bepinex).
    2. If you're downloading ["BepInExPack CultOfTheLamb" from Thunderstore](https://cult-of-the-lamb.thunderstore.io/package/BepInEx/BepInExPack_CultOfTheLamb/), follow the manual installation guide on the Thunderstore page itself. This one comes with a preconfigured `BepInEx.cfg` file, so it's advised you download this one.
2. Find the `BepInEx/plugins` folder.
3. Place the contents of **"RedHeartsFirst.zip"** in a new folder within the plugins folder.

## How To Use
All you have to do is select a heart order from the menu and start a dungeon. You can also change your heart order mid-dungeon and even mid-combat, but some bugs may ensue from that.

The available options are:
1. Black, Blue, Red -- The game's default.
2. Black, Red, Blue -- The mod's default. Your blue hearts will always be the last to be damaged.
3. Red, Black, Blue -- You will take damage to your red hearts before your black ones and to your black hearts before your blue ones.
4. Blue, Red, Black -- You will take damage to your blue hearts before your red ones, and to your red hearts before your black ones.

Your heart order choice is saved to a text file named `HeartOrder.txt` created inside of the `BepInEx/plugins` folder.

## Known Bugs
All known bugs are purely visual and shouldn't affect gameplay.

1. Black Heart animation always plays when Lamb is hit, regardless of whether they took any damage to their Black Hearts or not. A purely visual bug.

If you find any bugs or issues, you can contact me on Discord! `kelly betty#7936`

## Changelog
1. **2.0.0** -- Added menu window, added more choices, fixed bugs with hearts disappearing after Lamb gets hit.