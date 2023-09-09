﻿using Reptile;
using HarmonyLib;
using System.Collections.Generic;
using System;

namespace BrcCustomCharacters.Patches
{
    [HarmonyPatch(typeof(Reptile.SfxLibrary), "GenerateEnumDictionary")]
    public class InitSfxLibraryPatch
    {
        public static void Postfix(SfxLibrary __instance)
        {
            foreach (KeyValuePair<SfxCollectionID, SfxCollection> collectionPair in __instance.sfxCollectionIDDictionary)
            {
                Characters correspondingCharacter = VoiceUtility.CharacterFromVoiceCollection(collectionPair.Key);
                AssetDatabase.InitializeSfxCollectionsForCharacter(correspondingCharacter, collectionPair.Value);
            }
        }
    }

    [HarmonyPatch(typeof(Reptile.SfxLibrary), nameof(Reptile.SfxLibrary.GetSfxCollectionById))]
    public class GetSfxCollectionIdPatch
    {
        public static void Postfix(SfxCollectionID sfxCollectionId, ref SfxCollection __result, SfxLibrary __instance)
        {
            if (__result == null)
            {
                return;
            }

            Characters correspondingCharacter = VoiceUtility.CharacterFromVoiceCollection(sfxCollectionId);
            if (AssetDatabase.GetCharacterSfxCollection(correspondingCharacter, out SfxCollection collection))
            {
                __result = collection;
            }
        }
    }

    public class GetSfxCollectionStringPatch
    {
        public static void Postfix(string sfxCollectionName, ref SfxCollection __result, SfxLibrary __instance)
        {
            if (__result == null)
            {
                return;
            }

            foreach (KeyValuePair<string, SfxCollection> stringPair in __instance.sfxCollectionDictionary)
            {
                if (!(stringPair.Value == null) && stringPair.Value.collectionName.Equals(sfxCollectionName))
                {
                    if (Enum.TryParse(stringPair.Key, out SfxCollectionID collectionId))
                    {
                        Characters correspondingCharacter = VoiceUtility.CharacterFromVoiceCollection(collectionId);
                        if (AssetDatabase.GetCharacterSfxCollection(correspondingCharacter, out SfxCollection collection))
                        {
                            __result = collection;
                        }
                    }
                }
            }
        }
    }
}
