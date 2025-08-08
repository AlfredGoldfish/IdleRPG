using UnityEngine;

namespace IdleRPG.Data
{
    [CreateAssetMenu(menuName = "IdleRPG/Enemy")]
    public class EnemyDef : ScriptableObject
    {
        [SerializeField] string id;
        public string Id => id;
        public string displayName;
        public int maxHP = 10;
        public int attack = 1;
        public LootTable lootTable;

#if UNITY_EDITOR
    void OnValidate() {
      if (string.IsNullOrWhiteSpace(id)) id = System.Guid.NewGuid().ToString("N");
    }
#endif
    }
}
