using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    [SerializeField] PlayerController con;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Sword"))
        {
            con.Animation.PlayHit();
            con.Health.PlayerGetDamage(3f);
        }
    }
}
