using UnityEngine;
using TMPro;

public class DamagePopup3D : MonoBehaviour
{
    /* ───────── CONFIG ───────── */
    [Header("Refs")]
    public TMP_Text text;                       // drag TextMeshPro (3-D) here

    [Header("Motion")]
    public float riseHeight = 1.0f;
    public float duration = 0.6f;
    public float xJitter = 0.25f;

    [Header("Crit Style")]
    public float critScale = 1.4f;
    public Color critColor = Color.yellow;

    /* ───────── RUNTIME ───────── */
    Color startColor;
    Vector3 startPos, endPos;
    float time;

    /* ─────────────────────────── */
    void Awake()
    {
        // Auto-assign if the Inspector slot was left empty
        if (text == null) text = GetComponentInChildren<TMP_Text>(true);
    }

    void Start()
    {
        if (text == null)
        {
            Debug.LogError("DamagePopup3D → TMP_Text reference missing!");
            Destroy(gameObject);
            return;
        }

        startColor = text.color;
        startPos = transform.position;

        // slight random X offset to avoid perfect stacking
        float xOff = Random.Range(-xJitter, xJitter);
        endPos = startPos + new Vector3(xOff, riseHeight, 0f);
    }

    /** Called immediately after Instantiate by Enemy.cs */
    public void Init(int dmg, bool isCrit = false)
    {
        text.text = dmg.ToString();

        if (isCrit)
        {
            text.color = critColor;
            transform.localScale *= critScale;
        }
    }

    void Update()
    {
        time += Time.deltaTime;
        float t = time / duration;

        // smooth upward motion
        transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));

        // fade-out
        Color c = text.color;
        c.a = Mathf.Lerp(1f, 0f, t);
        text.color = c;

        if (t >= 1f) Destroy(gameObject);
    }

    // keep text facing the camera
    void LateUpdate()
    {
        if (Camera.main) transform.forward = Camera.main.transform.forward;
    }
}
