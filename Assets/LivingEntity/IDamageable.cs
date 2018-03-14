using UnityEngine;

public interface IDamageable
{
    void TakeHit(float damagae, Vector3 hitPoint, Vector3 hitDirection);
    void TakeDamage(float damagae);
}
