using UnityEngine;

public class PlayerTrigger : PlayerBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Sword"))
        {
            con.Animation.PlayHit();
            con.Health.PlayerGetDamage(3f);
        }
    }
}
