// FILE: Assets/Scripts/Data/EquipmentItemDef.cs
using UnityEngine;

namespace IdleRPG.Data
{
    [CreateAssetMenu(fileName = "Equipment", menuName = "IdleRPG/Equipment")]
    public class EquipmentItemDef : ScriptableObject
    {
        [SerializeField] private string displayName;
        [SerializeField] private EquipmentSlot slot;
        [SerializeField] private Rarity rarity;

        public string DisplayName => displayName;
        public EquipmentSlot Slot => slot;
        public Rarity Rarity => rarity;
    }
}
