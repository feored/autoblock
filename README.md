### autoblock

Easily swap materials and physics on custom blocks in TM2 Stadium.

__Warning__: You first need to create the custom blocks, including all variants you want (even if they are the same as the starting block) before you can modify them. This program will not create the custom block itself, it needs to already be present.


### Usage

    -r, --information    Only display information about the block(s), e.g materials used, without saving a new version.

    -i, --input          Required. Path to the .gbx file to modify or to a folder if multiple .gbx files are to processed

    -m, --materialIn     Required. Material link that will be replaced e.g StadiumRoad or StadiumGrass

    -o, --materialOut    Required. Material link to replace with e.g StadiumRoad or StadiumGrass

    -p, --physics        If set, physics surface to set for the material e.g Grass, Ice, Metal, etc

    --help               Display this help screen.

    --version            Display version information.

Files will be saved under the same path + new material and physics. Eg. ```/path/to/your/block.Block.Gbx -> /path/to/your/block_StadiumGrass_Grass.Block.Gbx```

Example to change all road textures into grass textures with grass physics for all Block files in a given folder:

    $ ./Autoblock.exe -i "C:\\Users\\You\\Documents\\Maniaplanet\\Blocks\\Base\\1-1" -m "StadiumRoad" -o "StadiumGrass" -p "Grass"

#### Creating a custom block

Place the block you want to make into a custom block in the map editor. While still in the block menu (F2), click on the +, then on the specified block.

In the menu that just appeared, click the + on customized variants until all the customized variants are there (if one appears in red, you can delete it and stop).

For each custom variant, click on the folder icon where it says empty. Then just click on the back arrow and select 'Keep Changes'. Do this for all custom variants, then save your new custom block. This is the .Block.Gbx that you will give as input to the above program using ```-i```.


### Resources

[Materials List](/NadeoImporterMaterialLib.txt) [Where the string in parentheses in DMaterial is the value you should enter in MaterialOut]

[Physics ID List](/PhysicsID.txt)