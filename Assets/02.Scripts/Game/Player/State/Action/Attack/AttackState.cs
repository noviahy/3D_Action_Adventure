using UnityEngine;

public class AttackState : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    public AttackStyle currentAttackStyle { get; private set; }
    public enum AttackStyle
    {
        Light,
        Heavy
    }
    public void ChangeAttackStyle(AttackStyle style)
    {
        if (currentAttackStyle == style) return;
        currentAttackStyle = style;
    }
    private void Update()
    {

    }
}
