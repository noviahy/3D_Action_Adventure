using UnityEngine;

public class AnimationEventController : MonoBehaviour
{
    [SerializeField] InputManager inputManager;

    public void RequestFinishAttacking()
    {
        inputManager.FinishAttacking();
    }
}
