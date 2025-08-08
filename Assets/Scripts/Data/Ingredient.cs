using System;
using UnityEngine;

namespace IdleRPG.Data
{
    [Serializable]
    public class Ingredient
    {
        public ItemDef item;
        [Min(1)] public int qty = 1;
    }
}
