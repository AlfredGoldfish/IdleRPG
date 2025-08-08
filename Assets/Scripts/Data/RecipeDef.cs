// FILE: Assets/Scripts/Data/RecipeDef.cs
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IdleRPG.Data
{
    [CreateAssetMenu(fileName = "Recipe", menuName = "IdleRPG/Recipe")]
    public class RecipeDef : ScriptableObject
    {
        public EquipmentItemDef result;

        // Keep this list public/serialized for inspector editing
        public List<Ingredient> ingredients = new();
    }

    [Serializable] // ← Only once
    public struct Ingredient
    {
        public EquipmentItemDef item;
        public int amount;
    }
}
