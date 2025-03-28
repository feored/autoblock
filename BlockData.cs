using GBX.NET.Engines.GameData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockswap
{
   class BlockData
    {
        public string Archetype { get; set; }
        public int CustomizedVariants { get; set; }

        public BlockData(CGameBlockItem blockItem)
        {
            Archetype = blockItem.ArchetypeBlockInfoId;
            CustomizedVariants = blockItem.CustomizedVariants.Count;
        }

        public BlockData(string archetype, int customizedVariants)
        {
            Archetype = archetype;
            CustomizedVariants = customizedVariants;
        }
    }
}
