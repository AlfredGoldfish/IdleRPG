using System.Collections.Generic;
using UnityEngine;

namespace IdleRPG.Data
{
    [CreateAssetMenu(menuName = "IdleRPG/Databases/Enemy Database")]
    public class EnemyDatabase : ScriptableObject
    {
        public List<EnemyDef> enemies = new();
        Dictionary<string, EnemyDef> _byId;

        public void Build()
        {
            _byId = new Dictionary<string, EnemyDef>(enemies.Count);
            foreach (var e in enemies)
            {
                if (e && !_byId.ContainsKey(e.Id)) _byId.Add(e.Id, e);
            }
        }

        public EnemyDef GetById(string id)
        {
            if (_byId == null) Build();
            return id != null && _byId.TryGetValue(id, out var def) ? def : null;
        }

        public static EnemyDatabase Load() =>
          Resources.Load<EnemyDatabase>("Databases/EnemyDatabase");
    }
}
