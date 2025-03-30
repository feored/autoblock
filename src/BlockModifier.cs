using GBX.NET;
using GBX.NET.Engines.GameData;
using GBX.NET.LZO;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autoblock.src
{
    internal class BlockModifier
    {
        public class ConvertOptions
        {
            public List<Conversion> Conversions { get; set; }
            public List<BlockData> BlockData { get; set; }

        }
        public class Conversion
        {
            public Material Material { get; set; } = new Material();
            public string NewLink { get; set; } = String.Empty;
            public string NewPhysics { get; set; } = String.Empty;

        }

        public class BlockData
        {
            public string ByteStream { get; set; } = String.Empty;
            public string Filename { get; set; } = String.Empty;
        }

        public class Material
        {
            public string Link { get; set; } = String.Empty;
            public string Physics { get; set; } = String.Empty;

        }

        public class BlockInfo
        {
            public string Filename { get; set; }
            public int CustomVariants { get; set; }

            public List<Material> Materials { get; set; }

            public BlockInfo(string filename)
            {
                Materials = new List<Material>();
                Filename = filename;
            }
        }


        public static List<BlockData> ConvertBlocks(List<BlockData> data, List<Conversion> conversions)
        {
            List<BlockData> convertedBlocks = new List<BlockData>();
            List<CGameItemModel> blocks = GetBlocks(data);

            for (int i = 0; i < blocks.Count; i++)
            {
                CGameItemModel block = blocks[i];
                CGameBlockItem eME = (CGameBlockItem)block.EntityModelEdition!;
                var customizedVariants = eME.CustomizedVariants;
                foreach (var customVariant in customizedVariants)
                {
                    if (customVariant == null || customVariant.Crystal == null || customVariant.Crystal.Layers.Count == 0)
                    {
                        continue;
                    }
                    var materials = customVariant?.Crystal?.Materials;
                    if (materials == null || materials.Count == 0)
                    {
                        continue;
                    }
                    foreach (var mat in materials)
                    {
                        if (mat.MaterialUserInst == null)
                        {
                            continue;
                        }
                        Conversion? relevantConversion = conversions.SingleOrDefault(c => c.Material.Link == mat.MaterialUserInst.Link && c.Material.Physics == mat.MaterialUserInst.SurfacePhysicId.ToString());
                        if (relevantConversion == null)
                        {
                            continue;
                        }
                        Enum.TryParse(relevantConversion.NewPhysics, out GBX.NET.Engines.Plug.CPlugSurface.MaterialId physicsID);
                        mat.MaterialUserInst.Link = relevantConversion.NewLink;
                        mat.MaterialUserInst.SurfacePhysicId = physicsID;
                    }
                }
                BlockData convertedBlock = new BlockData();
                convertedBlock.Filename = data[i].Filename;
                MemoryStream stream = new MemoryStream();
                block.Save(stream);
                convertedBlock.ByteStream = Convert.ToBase64String(stream.ToArray());
                convertedBlocks.Add(convertedBlock);
            }
            return convertedBlocks;
        }


        public static List<CGameItemModel> GetBlocks(List<BlockData> data)
        {
            List<CGameItemModel> blocks = new List<CGameItemModel>();
            foreach (var d in data)
            {
                var stream = new MemoryStream(Convert.FromBase64String(d.ByteStream));
                blocks.Add(Gbx.ParseNode<CGameItemModel>(stream));
            }
            return blocks;
        }



        public static List<BlockInfo> GetBlocksInfo(List<BlockData> data)
        {
            List<BlockInfo> blocks_info = new List<BlockInfo>();
            List<CGameItemModel> blocks = GetBlocks(data);

            static BlockInfo ReadBlockInfo(CGameItemModel block, string filename)
            {
                BlockInfo block_info = new BlockInfo(filename);

                CGameBlockItem eME = (CGameBlockItem)block.EntityModelEdition!;
                var customizedVariants = eME.CustomizedVariants;
                block_info.CustomVariants = customizedVariants.Count;

                foreach (var customVariant in customizedVariants)
                {
                    if (customVariant == null || customVariant.Crystal == null || customVariant.Crystal.Layers.Count == 0)
                    {
                        continue;
                    }
                    var materials = customVariant?.Crystal?.Materials;
                    if (materials == null || materials.Count < 1)
                    {
                        continue;

                    }
                    foreach (var mat in materials)
                    {
                        block_info.Materials.Add(new Material() { Link = mat.MaterialUserInst?.Link ?? "", Physics = mat.MaterialUserInst.SurfacePhysicId.ToString() });
                    }
                }
                return block_info;
            }
            for (int i = 0; i < blocks.Count; i++)
            {
                blocks_info.Add(ReadBlockInfo(blocks[i], data[i].Filename));
            }
            return blocks_info;
        }
    }
}
