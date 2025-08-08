using UnityEngine;

namespace IdleRPG.Data
{
    [CreateAssetMenu(menuName = "IdleRPG/Equipment Item")]
    public class EquipmentItemDef : ItemDef
    {
        public EquipmentSlot slot = EquipmentSlot.Weapon;

        // Keep it minimal for MVP; expand later (flat vs % mods, etc.)
        public int attackBonus;
        public int defenseBonus;
        public int speedBonus;
    }
}
