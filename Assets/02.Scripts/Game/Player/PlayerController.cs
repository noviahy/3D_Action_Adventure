using UnityEngine;

public class PlayerController
{
    // Components (MonoBehavior)
    public InputManager Input { get; }
    public CameraFollow3D Cam { get; }
    public Player Player { get; }
    public PlayerStateMachine StateMachine { get; }
    public GroundCheck GroundCheck { get; }
    public PlayerTrigger Trigger { get; }
    public CharacterController cc { get; }
    public LocomotionState Locomotion { get; }
    public Parrying Parrying { get; }
    public Dodge Dodge { get; }
    public Animator Animator { get; }
    public Attack Attack { get; }
    public AttackState AttackState { get; }
    public AnimationEventController AnimationEventController { get; }
    public ActionIdle ActionIdle { get; }
    public BowAttack BowAttack { get; }
    public Climb Climb { get; }
    public InteractionIdle InteractionIdle { get; }
    public Roll Roll { get; }
    public BoxInteractionState BoxInteractionState { get; }
    public RootMotionController RootMotionController { get; }

    // System (순수 C#)
    public DeadState Dead { get; }
    public PlayerMovement Movement { get; }
    public KnockbackState Knockback { get; }
    public Health Health { get; }
    public ActionState ActionState { get; }
    public EventManager Event { get; }
    public AnimationController Animation { get; }
    public InteractionState InteractionState { get; }

    // 모노
    public PlayerController(InputManager input,
        Player player,
        PlayerStateMachine stateMachine,
        GroundCheck groundCheck,
        CameraFollow3D cam,
        CharacterController characterController,
        LocomotionState locomotion,
        Dodge dodge,
        Animator animator,
        Attack attack,
        AttackState attackState,
        AnimationEventController animationEvent,
        ActionIdle actionIdle,
        Parrying parrying,
        BowAttack bowAttack,
        Climb climb,
        InteractionIdle interactionIdle,
        Roll roll,
        BoxInteractionState boxInteractionState,
        RootMotionController rootMotionController)
    {
        Input = input;
        Player = player;
        StateMachine = stateMachine;
        Attack = attack;
        GroundCheck = groundCheck;
        Cam = cam;
        cc = characterController;
        Locomotion = locomotion;
        Dodge = dodge;
        Animator = animator;
        AttackState = attackState;
        AnimationEventController = animationEvent;
        ActionIdle = actionIdle;
        Parrying = parrying;
        BowAttack = bowAttack;
        Climb = climb;
        InteractionIdle = interactionIdle;
        Roll = roll;
        BoxInteractionState = boxInteractionState;
        RootMotionController = rootMotionController;

        // 순수 C# 코드 생성
        Dead = new DeadState(this);
        Knockback = new KnockbackState(this);
        Movement = new PlayerMovement(this);
        Health = new Health(this);
        ActionState = new ActionState(this);
        Event = new EventManager(this);
        Animation = new AnimationController(this);
        InteractionState = new InteractionState(this);

        // Input에 직접 넣어줌
        input.Init(this);
    }
}
