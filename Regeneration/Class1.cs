using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;

namespace Regeneration
{
    [StaticConstructorOnStartup]
    public static class DeathRestRegenerate
    {
        static DeathRestRegenerate()
        {
            Log.Message("Harmony loaded for Sanguophage Regeneration.");
            new Harmony("rimworld.mod.misinformedOwl.sanguophageregeneration").PatchAll();
        }
    }

    [HarmonyPatch(typeof(Gene_Deathrest))]
    [HarmonyPatch(nameof(Gene_Deathrest.TickDeathresting))]
    class DeathrestPatch
    {
        static void Postfix(Gene_Deathrest __instance)
        {
            Pawn pawn = __instance.pawn;
            Building_Bed building_Bed = pawn.CurrentBed();
            if (building_Bed != null)
            {
                if (__instance.deathrestTicks % 12000 == 11999)
                {
                    HediffSet missingBodyParts = pawn.health.hediffSet;
                    if (missingBodyParts != null)
                    {
                        List<Hediff_MissingPart> missingPartsToRemove = new List<Hediff_MissingPart>();

                        foreach (Hediff part in missingBodyParts.hediffs)
                        {
                            if (part is Hediff_MissingPart missingPart)
                            {
                                missingPartsToRemove.Add(missingPart);
                            }
                        }
                        if (missingPartsToRemove.Count > 0)
                        {
                            pawn.health.RemoveHediff(missingPartsToRemove[0]);
                            if (missingPartsToRemove.Count == 1)
                            {
                                Messages.Message($"{pawn.Name} Has regenerated all limbs.", pawn, MessageTypeDefOf.PositiveEvent);
                            }
                        }
                    }
                }
            }

        }
    }
}