using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;
using LudeonTK;
using System.Xml;
using System.IO;

namespace Regeneration
{
    [StaticConstructorOnStartup]
    public static class DeathRestRegenerate
    {
        public static int requiredTicks;

        static DeathRestRegenerate()
        {
            try
            {
                Log.Message("Harmony loaded for Sanguophage Regeneration.");
                XmlDocument doc = new XmlDocument();

                // Define paths
                string localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mods/Regeneration/CHANGE REGEN HERE.xml");
                string steamWorkshopPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "steamapps/workshop/content/294100/YourModID/CHANGE REGEN HERE.xml");

                // Check which path exists
                if (File.Exists(localPath))
                {
                    Log.Error("Loading Local.");
                    doc.Load(localPath);
                }
                else if (File.Exists(steamWorkshopPath))
                {
                    Log.Error("Loading workshop.");
                    doc.Load(steamWorkshopPath);
                }
                else
                {
                    Log.Error("XML configuration file not found in either local or Steam Workshop paths.");
                    return;
                }

                XmlNode node = doc.SelectSingleNode("/Config/requiredTicks");
                if (node != null)
                {
                    requiredTicks = int.Parse(node.InnerText);
                    Log.Message($"requiredTicks set to: {requiredTicks}");
                }
                else
                {
                    Log.Error("Node /Config/requiredTicks not found in the XML.");
                }

                new Harmony("rimworld.mod.misinformedOwl.sanguophageregeneration").PatchAll();
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred: {ex.Message}");
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
                    if (__instance.deathrestTicks % DeathRestRegenerate.requiredTicks == DeathRestRegenerate.requiredTicks - 1)
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
                                foreach (Hediff_MissingPart part in missingPartsToRemove)
                                {
                                    string partName = part.Part.Label;
                                    if (partName != "left lung" && partName != "right lung" && partName != "left kidney" && partName != "right kidney")
                                    {
                                        pawn.health.RemoveHediff(part);
                                        break;
                                    }
                                }
                                if (missingPartsToRemove.Count == 1)
                                {
                                    Messages.Message($"{pawn.Name} has regenerated all limbs.", pawn, MessageTypeDefOf.PositiveEvent);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
