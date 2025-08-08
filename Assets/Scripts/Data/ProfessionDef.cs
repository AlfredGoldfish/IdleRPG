using System;
using UnityEngine;

namespace IdleRPG.Data
{
    [CreateAssetMenu(menuName = "IdleRPG/Profession")]
    public class ProfessionDef : ScriptableObject
    {
        [SerializeField] string id;
        public string Id => id;

        public string displayName;
        public ProfessionType type;

#if UNITY_EDITOR
    void OnValidate() {
      if (string.IsNullOrWhiteSpace(id)) id = Guid.NewGuid().ToString("N");
    }
#endif
    }
}
