// FILE: Assets/Scripts/Data/ItemDef.cs
using UnityEngine;

namespace IdleRPG.Data
{
    [CreateAssetMenu(menuName = "IdleRPG/Item")]
    public class ItemDef : ScriptableObject
    {
        [SerializeField] private string id; // unique, stable; use for save/load
        public string Id => id;

        public string displayName;
        public Rarity rarity; // now refers to the enum in Rarity.cs

#if UNITY_EDITOR
        private void OnValidate()
        {
            // auto-fill ID once; keep it stable afterward
            if (string.IsNullOrWhiteSpace(id))
                id = System.Guid.NewGuid().ToString("N");
        }
#endif
    }
}
