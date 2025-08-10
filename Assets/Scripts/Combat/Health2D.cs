using System;
using UnityEngine;

/// Generic health component for 2D/3D. No Update, no UI, no currency.
/// Other scripts call TakeDamage/Heal; listeners use the events.
public class Health2D : MonoBehaviour
{
    [SerializeField] int maxHP = 10;
    public int Max => Mathf.Max(1, maxHP);
    public int Current { get; private set; }
    public bool IsDead => Current <= 0;

    public event Action<int> OnDamaged; // amount
    public event Action<int> OnHealed;  // amount
    public event Action OnDied;

    void Awake()
    {
        Current = Max;
    }

    public void SetMax(int newMax, bool fill = true)
    {
        maxHP = Mathf.Max(1, newMax);
        if (fill) Current = Max;
        else Current = Mathf.Min(Current, Max);
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || IsDead) return;
        int before = Current;
        Current = Mathf.Max(0, Current - amount);
        OnDamaged?.Invoke(before - Current);
        if (Current == 0) OnDied?.Invoke();
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || IsDead) return;
        int before = Current;
        Current = Mathf.Min(Max, Current + amount);
        OnHealed?.Invoke(Current - before);
    }

    public void Kill()
    {
        if (IsDead) return;
        Current = 0;
        OnDamaged?.Invoke(0); // optional, amount 0 just to signal
        OnDied?.Invoke();
    }

    public void ResetHealth() => Current = Max;
}
