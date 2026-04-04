using UnityEngine;

public class PlayerController
{
    // Components (MonoBehavior)
    public InputManager Input { get; }    
    public CameraFollow3D Cam {  get; }    
    public Player Player { get; }
    public PlayerStateMachine StateMachine { get; }
    public AttackState AttackState { get; }
    public GroundCheck GroundCheck { get; }
    public PlayerTrigger Trigger { get; }
    public CharacterController cc { get; }
    public InteractionState Interaction { get; }
    public LocomotionState Locomotion { get; }
    public Parrying Parrying { get; }
    public Dodge Dodge { get; }

    // System (순수 C#)
    public DeadState Dead { get; }
    public PlayerMovement Movement { get; }
    public KnockbackState Knockback { get; }
    public Health Health { get; }
    public Attack Attack { get; }
    public ActionState ActionState { get; }
    public EventManager Event { get; }
    public AnimationController Animation { get; }
    public Animator Animator { get; }


    // 엄
    public PlayerController(InputManager input, 
        Player player, 
        PlayerStateMachine stateMachine, 
        AttackState attackState,
        InteractionState interactionState,
        GroundCheck groundCheck, 
        CameraFollow3D cam, 
        CharacterController characterController,
        Animator animator,
        Parrying parrying,
        Dodge dodge)
    {
        Input = input;
        Player = player;
        StateMachine = stateMachine;
        AttackState = attackState;
        Interaction = interactionState;
        GroundCheck = groundCheck;
        Cam = cam;
        cc = characterController;
        Animator = animator;
        Parrying = parrying;
        Dodge = dodge;

        // 순수 C# 코드 생성
        Dead = new DeadState(this);
        Knockback = new KnockbackState(this);
        Movement = new PlayerMovement(this);
        Health = new Health(this);
        Attack = new Attack(this);
        Parrying = new Parrying(this);
        ActionState = new ActionState(this);
        Event = new EventManager(this);
        Animation = new AnimationController(this);
    }
}
