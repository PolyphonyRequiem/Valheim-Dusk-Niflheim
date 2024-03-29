﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PatchNotesExtender.Patches
{
    [HarmonyPatch]
    public static class PatchNotesPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ChangeLog), "Start")]
        private static void ChangeLog__Start(ref TextAsset ___m_changeLog)
        {
            var patchNotesUri = PatchNotesExtender.PatchNotesUri.Value;
            if (String.IsNullOrWhiteSpace(patchNotesUri))
            {
                PatchNotesExtender.Log.LogWarning("patchNotesUri is null of whitespace.  No patch notes will be fetched.");
                return;
            }
            try
            {
                using (WebClient client = new WebClient())
                {
                    var patchNotes = client.DownloadString(patchNotesUri.Trim());
                    ___m_changeLog = new TextAsset(patchNotes);
                }
            }
            catch (Exception ex)
            {
                PatchNotesExtender.Log.LogError($"Unable to process data from '{patchNotesUri}'.  An exception occurred. {ex.ToString()}");
            }
        }
    }
}
