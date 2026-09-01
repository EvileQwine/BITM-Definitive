using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float maxHealth;
    public float curHealth;

    public float HealthPercent()
    {
        return curHealth / maxHealth;
    }
}
