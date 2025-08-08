using System.Collections.Generic;
using UnityEngine;

namespace IdleRPG.Data
{
    [CreateAssetMenu(menuName = "IdleRPG/Databases/Recipe Database")]
    public class RecipeDatabase : ScriptableObject
    {
        public List<RecipeDef> recipes = new();

        Dictionary<string, RecipeDef> _byId;
        Dictionary<ProfessionType, List<RecipeDef>> _byProfession;

        static readonly List<RecipeDef> _empty = new();

        public void Build()
        {
            _byId = new Dictionary<string, RecipeDef>(recipes.Count);
            _byProfession = new Dictionary<ProfessionType, List<RecipeDef>>();
            foreach (var r in recipes)
            {
                if (!r) continue;
                if (!_byId.ContainsKey(r.Id)) _byId.Add(r.Id, r);
                if (!_byProfession.TryGetValue(r.profession, out var list))
                {
                    list = new List<RecipeDef>();
                    _byProfession.Add(r.profession, list);
                }
                list.Add(r);
            }
        }

        public RecipeDef GetById(string id)
        {
            if (_byId == null) Build();
            return id != null && _byId.TryGetValue(id, out var def) ? def : null;
        }

        public List<RecipeDef> GetByProfession(ProfessionType type)
        {
            if (_byProfession == null) Build();
            return _byProfession.TryGetValue(type, out var list) ? list : _empty;
        }

        public static RecipeDatabase Load()
          => Resources.Load<RecipeDatabase>("Databases/RecipeDatabase");
    }
}
