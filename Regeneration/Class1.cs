using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Regeneration
{
    public class CompSanguophageRegeneration : ThingComp
    {
        public override void CompTick()
        {
            base.CompTick();
            Log.Message("This is a test");
        }
    }
}
