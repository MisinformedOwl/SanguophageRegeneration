using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Regeneration
{
    public class DeathRestRegenerate : Gene_Deathrest
    {
        public void TickDeathresting(bool paused)
        {
            base.TickDeathresting(paused);
            Log.Message("This is a test");
        }
    }
}
