using UnityEngine;

public interface IDamageable
{
    public void ReceiveDamage();
    public void ReceiveCriticalDamage();
    public Transform GetPartialPoint();
    public Transform GetCriticalPoint();
}
