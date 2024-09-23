using UnityEngine;

public interface IDamageble
{
    public void TakeDamage(Vector3 point, Vector3 direction, float damage);
    //public void TakeDamage(Vector3 point, Vector3 direction, float distance, float damage, type of smth);
}
