using UnityEngine;

namespace IdleRPG.UI
{
    /// <summary>
    /// Helpers for HUD debugging. Wire these to UI Buttons.
    /// </summary>
    public class HUDDebugTools : MonoBehaviour
    {
        [ContextMenu("Reset HUD Counters (Session Only)")]
        public void ResetHudCounters()
        {
            var list = HUDCurrencyCounterSession.Registry;
            for (int i = 0; i < list.Count; i++)
                list[i].ResetSession();
#if UNITY_EDITOR
            Debug.Log("[HUDDebugTools] HUD counters reset (session-only).");
#endif
        }

        [ContextMenu("Reset Save + HUD")]
        public void ResetPersistentAndHud()
        {
            // Reset persistent file + in-memory wallet via SystemsBootstrap
            var bootstrap = FindAnyObjectByType<SystemsBootstrap>();
            if (bootstrap != null)
                bootstrap.ResetSave();

            // Then re-anchor session HUDs to current wallet (which is now zero)
            ResetHudCounters();
#if UNITY_EDITOR
            Debug.Log("[HUDDebugTools] Persistent save cleared and HUD counters reset.");
#endif
        }
    }
}
