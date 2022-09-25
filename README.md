## Description
A small Cult of the Lamb mod that lets you take damage to your red hearts before your blue/black hearts. 

The mod has a few known bugs I plan on fixing soon when I have the time, but all of them are visual bugs and shouldn't impact gameplay.

This mod's only requirement is BepInEx.

## Installation
1. Follow the installation guide for BepInEx. (I suggest downloading [BepInExPack CultOfTheLamb](https://cult-of-the-lamb.thunderstore.io/package/BepInEx/BepInExPack_CultOfTheLamb/) from Thunderstore, as that one comes with a preconfigured `BepInEx.cfg` file.)
2. Place the `RedHeartsFirst.dll` file inside of the `BepInEx/plugins` folder.

## Known Bugs
1. Black Heart will pulse every time Lamb takes damage. Purely visual bug.
2. When taking double damage to your red hearts, Black Heart may... 'shrink', because it gets stuck in the middle of an animation. I assume this is because the game expects Black Hearts to go away after double damage, so the animation is pretty intense. This will happen constantly if you're wearing the Golden Fleece (due to the Golden Fleece causing you to always take double damage). Purely visual bug.
