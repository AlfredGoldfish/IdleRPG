using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleRPG.Data
{
    [CreateAssetMenu(menuName = "IdleRPG/Recipe")]
    public class RecipeDef : ScriptableObject
    {
        [SerializeField] string id;
        public string Id => id;

        public ProfessionType profession;
        public List<Ingredient> inputs = new();
        public ItemDef outputItem;
        public int outputQty = 1;
        public float craftSeconds = 2f;
        public int requiredLevel = 1; // for later gating


#if UNITY_EDITOR
    void OnValidate() {
      if (string.IsNullOrWhiteSpace(id)) id = Guid.NewGuid().ToString("N");
      if (outputQty < 1) outputQty = 1;
    }
#endif
    }

    [Serializable]
    public class Ingredient
    {
        public ItemDef item;
        public int qty = 1;
    }
}
