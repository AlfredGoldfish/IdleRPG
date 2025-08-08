using System.Collections.Generic;
using UnityEngine;

namespace IdleRPG.Data
{
    [CreateAssetMenu(menuName = "IdleRPG/Databases/Item Database")]
    public class ItemDatabase : ScriptableObject
    {
        public List<ItemDef> items = new();
        Dictionary<string, ItemDef> _byId;

        public void Build()
        {
            _byId = new Dictionary<string, ItemDef>(items.Count);
            foreach (var i in items)
            {
                if (i && !_byId.ContainsKey(i.Id)) _byId.Add(i.Id, i);
            }
        }

        public ItemDef GetById(string id)
        {
            if (_byId == null) Build();
            return id != null && _byId.TryGetValue(id, out var def) ? def : null;
        }

        public static ItemDatabase Load() =>
          Resources.Load<ItemDatabase>("Databases/ItemDatabase");
    }
}
