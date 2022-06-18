using MetaRipper;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Valve.Newtonsoft.Json;

namespace Gamemodes 
{

    [CustomEditor(typeof(AtlasObjectTable), true)]
    public class AtlasObjectTableEditor : Editor
    {

        public static List<MetadataEntry> entries;
        List<string> generatedObjects = new List<string>();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            AtlasObjectTable objectTable = serializedObject.targetObject as AtlasObjectTable;

            DrawHorizontalLine();

            if (GUILayout.Button("Select Metadata File"))
            {
                string metaFilePath = EditorUtility.OpenFilePanel("Select Metadata File", string.Empty, string.Empty);
                string json = File.ReadAllText(metaFilePath);

                entries = JsonConvert.DeserializeObject<List<MetadataEntry>>(json);
            }

            if(entries != null)
            {
                if (GUILayout.Button("Generate Table"))
                {
                    GenerateFromMetadata(objectTable);
                }

                EditorGUILayout.HelpBox(entries.Count.ToString() + " Entries Loaded!", MessageType.Info);

                DrawHorizontalLine();

                EditorGUILayout.HelpBox(string.Join("\n", generatedObjects.ToArray()), MessageType.None);
            }
        }

        public void GenerateFromMetadata(AtlasObjectTable objectTable)
        {
            generatedObjects = new List<string>();

            foreach (MetadataEntry entry in entries)
            {
                //If this is in the whitelist, add immediately and continue
                if (objectTable.WhitelistedObjectIDs.Contains(entry.ObjectID))
                {
                    generatedObjects.Add(entry.ObjectID);
                    continue;
                }

                if (objectTable.BlacklistedObjectIDs.Contains(entry.ObjectID)) continue;

                if (!entry.SpawnInTable) continue;

                if (objectTable.Category.ToString() != entry.Category) continue;

                if (objectTable.Eras.Count > 0 && !objectTable.Eras.Any(o => o.ToString() == entry.Era)) continue;

                if (objectTable.Sets.Count > 0 && !objectTable.Sets.Any(o => o.ToString() == entry.Set)) continue;

                if (objectTable.CountriesOfOrigin.Count > 0 && !objectTable.CountriesOfOrigin.Any(o => o.ToString() == entry.CountryOfOrigin)) continue;

                if (objectTable.EarliestYear > -1 && entry.FirstYear < objectTable.EarliestYear) continue;

                if (objectTable.LatestYear > -1 && entry.FirstYear > objectTable.LatestYear) continue;

                if (objectTable.Sizes.Count > 0 && !objectTable.Sizes.Any(o => o.ToString() == entry.FirearmSize)) continue;

                if (objectTable.Actions.Count > 0 && !objectTable.Actions.Any(o => o.ToString() == entry.FirearmAction)) continue;

                if (objectTable.Modes.Count > 0 && !objectTable.Modes.Any(o => entry.FirearmFiringModes.Contains(o.ToString()))) continue;

                if (objectTable.ExcludedModes.Count > 0 && objectTable.ExcludedModes.Any(o => entry.FirearmFiringModes.Contains(o.ToString()))) continue;

                if (objectTable.FeedOptions.Count > 0 && !objectTable.FeedOptions.Any(o => entry.FirearmFeedOptions.Contains(o.ToString()))) continue;

                if (objectTable.MountsAvailable.Count > 0 && !objectTable.MountsAvailable.Any(o => entry.FirearmMounts.Contains(o.ToString()))) continue;

                if (objectTable.RoundPowers.Count > 0 && !objectTable.RoundPowers.Any(o => o.ToString() == entry.FirearmRoundPower)) continue;

                if (objectTable.Features.Count > 0 && !objectTable.Features.Any(o => o.ToString() == entry.AttachmentFeature)) continue;

                if (objectTable.MountTypes.Count > 0 && !objectTable.MountTypes.Any(o => o.ToString() == entry.AttachmentMount)) continue;

                if (objectTable.MeleeStyles.Count > 0 && !objectTable.MeleeStyles.Any(o => o.ToString() == entry.MeleeStyle)) continue;

                if (objectTable.MeleeHandedness.Count > 0 && !objectTable.MeleeHandedness.Any(o => o.ToString() == entry.MeleeHandedness)) continue;

                if (objectTable.ThrownTypes.Count > 0 && !objectTable.ThrownTypes.Any(o => o.ToString() == entry.ThrownType)) continue;

                if (objectTable.ThrownDamageTypes.Count > 0 && !objectTable.ThrownDamageTypes.Any(o => o.ToString() == entry.ThrownDamageType)) continue;

                if (objectTable.PowerupTypes.Count > 0 && !objectTable.PowerupTypes.Any(o => o.ToString() == entry.PowerupType)) continue;

                if (objectTable.MinAmmoCapacity > -1 && entry.MaxCapacityRelated < objectTable.MinAmmoCapacity) continue;

                if (objectTable.MaxAmmoCapacity > -1 && entry.MinCapacityRelated > objectTable.MaxAmmoCapacity) continue;

                if (objectTable.OverrideMagType && entry.MagazineType != (int)objectTable.MagTypeOverride) continue;

                if (objectTable.OverrideRoundType && entry.RoundType != (int)objectTable.RoundTypeOverride) continue;

                generatedObjects.Add(entry.ObjectID);

            }
        }

        private void DrawHorizontalLine()
        {
            EditorGUILayout.Space();
            var rect = EditorGUILayout.BeginHorizontal();
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

    }

}


