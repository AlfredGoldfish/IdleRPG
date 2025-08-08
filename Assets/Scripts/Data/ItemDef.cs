using UnityEngine;

namespace IdleRPG.Data
{
    public enum Rarity { Common, Uncommon, Rare, Epic, Legendary }

    [CreateAssetMenu(menuName = "IdleRPG/Item")]
    public class ItemDef : ScriptableObject
    {
        [SerializeField] string id;           // unique, stable; use for save/load
        public string Id => id;
        public string displayName;
        public Rarity rarity;

#if UNITY_EDITOR
    void OnValidate() {
      // auto-fill ID once; keep it stable afterward
      if (string.IsNullOrWhiteSpace(id)) id = System.Guid.NewGuid().ToString("N");
    }
#endif
    }
}
