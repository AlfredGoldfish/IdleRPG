// FILE: Assets/Scripts/Data/RecipeDef.cs
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleRPG.Data
{
    [CreateAssetMenu(fileName = "Recipe", menuName = "IdleRPG/Recipe")]
    public class RecipeDef : ScriptableObject
    {
        // --- Stable ID for indexing/saves ---
        [SerializeField] private string id;
        public string Id => id;

        // Optional display name for UI
        public string displayName;

        // --- Classification used by your RecipeDatabase ---
        public Profession profession;

        // --- Result + ingredients ---
        public EquipmentItemDef result;
        public List<Ingredient> ingredients = new();

#if UNITY_EDITOR
        private void OnValidate()
        {
            // Auto-generate once, then keep stable
            if (string.IsNullOrWhiteSpace(id))
                id = System.Guid.NewGuid().ToString("N");
        }
#endif
    }

    [Serializable]
    public struct Ingredient
    {
        public EquipmentItemDef item;
        public int amount;
    }
}
