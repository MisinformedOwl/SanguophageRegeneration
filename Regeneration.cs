using RimWorld;
using Verse;

namespace Regenerate
{
    public class ThingComp_SanguophageRegeneration : ThingComp
    {
        private float geneActivation = 1000;
        private int elapsedTimeTicks = 0;

        public override void CompTick()
        {
            base.CompTick();

            // Increase elapsed time in ticks
            elapsedTimeTicks++;

            // Log elapsed ticks
            Log.Message($"Elapsed ticks: {elapsedTimeTicks}");

            // Check if elapsed time exceeds gene activation threshold
            if (elapsedTimeTicks > geneActivation)
            {
                beginRegeneration();
                elapsedTimeTicks = 0; // Reset elapsed time
            }
        }

        private void beginRegeneration()
        {
            // Iterate through all pawns in the current map
            foreach (Pawn pawn in Find.CurrentMap.mapPawns.AllPawns)
            {
                if (pawn.Deathresting)
                {
                    RegenerateLimb(pawn);
                }
            }
        }

        private void RegenerateLimb(Pawn pawn)
        {
            // Find the missing body part hediff
            HediffDef missingBodyPartDef = HediffDefOf.MissingBodyPart;
            if (missingBodyPartDef != null)
            {
                Hediff missingBodyPartHediff = pawn.health.hediffSet.GetFirstHediffOfDef(missingBodyPartDef);

                if (missingBodyPartHediff != null)
                {
                    // Remove the missing body part hediff
                    pawn.health.RemoveHediff(missingBodyPartHediff);
                    Log.Message($"Removed missing part");

                    // Add a new hediff representing the regenerated limb
                    Hediff_AddedPart hediffRegeneratedLimb = HediffMaker.MakeHediff(HediffDef.Named("MyRegeneratedLimbHediff"), pawn) as Hediff_AddedPart;

                    if (hediffRegeneratedLimb != null)
                    {
                        // Set properties of the regenerated limb hediff as needed
                        hediffRegeneratedLimb.Part = missingBodyPartHediff.Part; // Set the same body part as the missing one
                        hediffRegeneratedLimb.Severity = 1.0f; // Set the severity of the new limb

                        // Add the regenerated limb hediff to the pawn
                        pawn.health.AddHediff(hediffRegeneratedLimb, null, null);
                        Log.Message($"Added regenerated limb");
                    }
                }
            }
        }

    }
}
