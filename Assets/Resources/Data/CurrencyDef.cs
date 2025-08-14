using UnityEngine;

namespace IdleRPG.Data
{
    [CreateAssetMenu(menuName = "IdleRPG/Currency", fileName = "Currency_New")]
    public class CurrencyDef : ScriptableObject
    {
        public string id;
        public string displayName = "New Currency";
        public Sprite icon;
        public Color color = Color.white;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
                id = System.Guid.NewGuid().ToString("N");
        }
#endif
    }
}
