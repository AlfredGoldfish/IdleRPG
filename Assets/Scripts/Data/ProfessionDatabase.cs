using System.Collections.Generic;
using UnityEngine;

namespace IdleRPG.Data
{
    [CreateAssetMenu(menuName = "IdleRPG/Databases/Profession Database")]
    public class ProfessionDatabase : ScriptableObject
    {
        public List<ProfessionDef> professions = new();

        Dictionary<string, ProfessionDef> _byId;
        Dictionary<ProfessionType, ProfessionDef> _byType;

        public void Build()
        {
            _byId = new Dictionary<string, ProfessionDef>(professions.Count);
            _byType = new Dictionary<ProfessionType, ProfessionDef>();
            foreach (var p in professions)
            {
                if (!p) continue;
                if (!_byId.ContainsKey(p.Id)) _byId.Add(p.Id, p);
                if (!_byType.ContainsKey(p.type)) _byType.Add(p.type, p);
            }
        }

        public ProfessionDef GetById(string id)
        {
            if (_byId == null) Build();
            return id != null && _byId.TryGetValue(id, out var def) ? def : null;
        }

        public ProfessionDef GetByType(ProfessionType type)
        {
            if (_byType == null) Build();
            return _byType.TryGetValue(type, out var def) ? def : null;
        }

        public static ProfessionDatabase Load()
          => Resources.Load<ProfessionDatabase>("Databases/ProfessionDatabase");
    }
}
