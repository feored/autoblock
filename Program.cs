using GBX.NET;
using GBX.NET.Engines.Game;
using GBX.NET.Engines.GameData;
using GBX.NET.Inputs;
using GBX.NET.LZO;
using CommandLine;


public class Autoblock
{

    static string BASE_LINK = "Stadium\\Media\\Material\\";

    public class Options
    {
        [Option('r', "information", Required = false, HelpText = "Only display information about the block(s), e.g materials used.")]
        public bool Information { get; set; }

        [Option('i', "input", Required = true, HelpText = "Path to the .gbx file to modify or to a folder if multiple .gbx files are to processed")]
        public string Input { get; set; }

        [Option('m', "materialIn", Required = true, HelpText = "Material link that will be replaced e.g StadiumRoad or StadiumGrass")]
        public string MaterialIn { get; set; }

        [Option('o', "materialOut", Required = true, HelpText = "Material link to replace with e.g StadiumRoad or StadiumGrass")]
        public string MaterialOut { get; set; }

        [Option('p', "physics", Required = false, HelpText = "If set, physics surface to set for the material e.g Grass, Ice, Metal, etc")]
        public string Physics { get; set; }

        /*Enum.TryParse("Active", out StatusEnum myStatus);*/

    }



    static void handleBlock(CGameItemModel block, Options opts)
    {
        CGameBlockItem eME = (CGameBlockItem)block.EntityModelEdition!;
        var customizedVariants = eME.CustomizedVariants;
        Console.WriteLine("Found " + customizedVariants.Count + " layers in custom variant.");
        foreach (var customVariant in customizedVariants)
        {
            if (customVariant == null || customVariant.Crystal == null || customVariant.Crystal.Layers.Count == 0)
            {
                Console.WriteLine("Skipping empty layer...");
                break;
            }
            Console.WriteLine("Layers: ", customVariant?.Crystal?.Layers.Count);
            var materials = customVariant?.Crystal?.Materials;
            if (materials == null || materials.Count < 1)
            {
                Console.WriteLine("Skipping empty materials list...");
                break;
            }
            foreach (var mat in materials)
            {
                Console.WriteLine("Material name: " + mat.MaterialUserInst?.MaterialName);
                Console.WriteLine("Link: " + mat.MaterialUserInst?.Link);
                Console.WriteLine("Physics surface: " + mat.MaterialUserInst?.SurfacePhysicId);
                if (opts.Information)
                {
                                       continue;
                }
                if (mat.MaterialUserInst?.Link == BASE_LINK + opts.MaterialIn)
                {
                    mat.MaterialUserInst.Link = BASE_LINK + opts.MaterialOut;
                    Console.WriteLine("Successfully replaced " + opts.MaterialIn + " with " + opts.MaterialOut);
                    if (opts.Physics != null)
                    {
                        Enum.TryParse(opts.Physics, out GBX.NET.Engines.Plug.CPlugSurface.MaterialId physicsID);
                        mat.MaterialUserInst.SurfacePhysicId = physicsID;
                    }
                }
            }
        }
    }
    static void RunOptions(Options opts)
    {
        List<string> blockPaths = new List<string>();
        if (Directory.Exists(opts.Input))
        {
            Console.WriteLine("Processing all .gbx files in the folder...");
            foreach (var file in Directory.GetFiles(opts.Input, "*.gbx"))
            {
                blockPaths.Add(file);
            }
        }
        else
        {
            blockPaths.Add(opts.Input);
        }
        foreach (var blockPath in blockPaths)
        {
            var block = Gbx.ParseNode<CGameItemModel>(blockPath);
            handleBlock(block, opts);
            if (opts.Information)
            {
                continue;
            }
            string modifiers = opts.Physics != null ? opts.MaterialOut + "_" + opts.Physics : opts.MaterialOut;
            string newPath = blockPath.Replace(".Block.Gbx", "_" + modifiers + ".Block.Gbx");
            block.Save(newPath);
            Console.WriteLine("Successfully saved as " + newPath + ".");
        }


        if (opts.Information)
        {
            Console.WriteLine("Displaying information about the block(s)...");
        }
    }
    public static void Main(string[] args)
    {
        Gbx.LZO = new Lzo();
        Parser.Default.ParseArguments<Options>(args)
                 .WithParsed(RunOptions);
    }
}

