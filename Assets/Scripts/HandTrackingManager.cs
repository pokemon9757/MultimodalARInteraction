using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.MagicLeap;

namespace MMI
{
    public class HandTrackingManager : MonoBehaviour
    {
        [SerializeField, Tooltip("Default: True. Set to false to not have a pre render Handtracking update. Not recommended for handtracking with visuals as this can affect smoothness.")]
        private bool preRenderHandUpdate = true;

        /// <summary>
        /// Validates fields.
        /// </summary>
        void Start()
        {
            // HAND_TRACKING is a normal permission, so we don't request it at runtime. It is auto-granted if included in the app manifest.
            // If it's missing from the manifest, the permission is not available.
            if (!MLPermissions.CheckPermission(MLPermission.HandTracking).IsOk)
            {
                Debug.LogError($"You must include the {MLPermission.HandTracking} permission in the AndroidManifest.xml to run this example.");
                enabled = false;
                return;
            }

            InputSubsystem.Extensions.MLHandTracking.StartTracking();
            InputSubsystem.Extensions.MLHandTracking.SetPreRenderHandUpdate(preRenderHandUpdate);
        }
    }
}
