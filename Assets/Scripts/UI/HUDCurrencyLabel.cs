using UnityEngine;
using IdleRPG.Core;
using IdleRPG.Data;

#if TMP_PRESENT || UNITY_EDITOR
using TMPro;
#endif
using UnityEngine.UI;

namespace IdleRPG.UI
{
    public class HUDCurrencyLabel : MonoBehaviour
    {
        [SerializeField] private CurrencyDef currency;
#if TMP_PRESENT || UNITY_EDITOR
        [SerializeField] private TMP_Text tmpText;
#endif
        [SerializeField] private Text uiText;

        private EconomyService eco;

        private void Reset()
        {
#if TMP_PRESENT || UNITY_EDITOR
            if (!tmpText) tmpText = GetComponent<TMP_Text>();
#endif
            if (!uiText) uiText = GetComponent<Text>();
        }

        private void OnEnable()
        {
            eco = EconomyService.Instance ?? Object.FindAnyObjectByType<EconomyService>();
            if (eco == null || currency == null) return;

            Refresh();
            eco.OnCurrencyChanged += HandleChanged;
        }

        private void OnDisable()
        {
            if (eco != null) eco.OnCurrencyChanged -= HandleChanged;
        }

        private void HandleChanged(CurrencyDef def, ulong newTotal)
        {
            if (def == currency) Refresh();
        }

        private void Refresh()
        {
            if (eco == null || currency == null) return;
            var value = eco.Get(currency).ToString();
#if TMP_PRESENT || UNITY_EDITOR
            if (tmpText) { tmpText.text = value; return; }
#endif
            if (uiText) { uiText.text = value; return; }
        }
    }
}
