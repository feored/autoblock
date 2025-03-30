### Autoblock

Easily swap materials and physics on custom blocks in TM2 Stadium.


__Warning__: You first need to create the custom blocks, including all variants you want (even if they are the same as the starting block) before you can modify them. This program will not create the custom block itself, it needs to already be present.


### Usage

For a quick start, download [Base.zip](/assets/Base.zip) (click on View Raw). This folder contains as many premade custom blocks as I've had the time to make.

Launch autoblock.exe, then open one or several custom block (.Block.Gbx) files.
For each block opened, a table will display the materials that already used within that block.
A material is composed a link (the textures, the visual aspect of that material) and a physics layer (which controls gameplay aspects, like ice, grass, and so on).

![selecting-blocks](/assets/select.png)

In the next section, you can add conversions from an existing material (for example, the center of the normal road block is usually a material composed of the link StadiumRoad, and the physics layer Asphalt). If you want to make the center of road blocks into grass instead, choose StadiumGrass for the New Link and Grass for the New Physics, then hit convert.

![converting-blocks](/assets/convert.png)

Once the files are converted, you can download them and save them in your Maniaplanet/Blocks folder.

You need to restart Trackmania for blocks placed inside the Blocks folder to appear in the editor.

![grass-blocks](/assets/grass.png)

Tip: Because all variations of each block are bundled in the Base blocks, you may notice after finishing your map that the filesize is very large. To make these blocks a bit lighter, you can remove the variations you don't need by editing the custom block then hitting the red cross in front of each unused variation.

![deleting-variations](/assets/delete.png)

#### Creating a custom block

Place the block you want to make into a custom block in the map editor. While still in the block menu (F2), click on the +, then on the specified block.

In the menu that just appeared, click the + on customized variants until all the customized variants are there (if one appears in red, you can delete it and stop). If asked, discard changes. Most blocks have at least one ground and air variant, but some can have less or more.

For each custom variant, click on the folder icon where it says empty, then just click on the back arrow again. Do this for all custom variants, then save your new custom block. This is the .Block.Gbx that you will give as input to the above program using ```-i```.
