using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Player에서 사용하는 모든 MonoBehavior를 모아 PlayerController로 보내줌
    [Header("Init")]
    [SerializeField] InputManager input;
    [SerializeField] PlayerStateMachine stateMachine;
    [SerializeField] InteractionState interaction;
    [SerializeField] GroundCheck groundCheck;
    [SerializeField] CameraFollow3D cam;
    [SerializeField] CharacterController characterController;
    [SerializeField] Animator animator;
    [SerializeField] Parrying parrying;
    [SerializeField] Dodge dodge;
    [SerializeField] Attack attack;
    [SerializeField] AttackState attackState;
    [SerializeField] AnimationEventController animatorEventController;

    [Header("Weapon")]
    [SerializeField] Renderer sword;
    [SerializeField] Renderer bow;

    private PlayerController Controller;

    // 매 Attack마다 ChangeWeaponType를 해줘야
    // 무기 바꿀 때 바꾼지 확인 가능
    // 이거 옮겨줘야할듯
    public WeaponType currentWeaponType { get; private set; }
    private WeaponType previousWeaponType;

    public ItemType currentItemType { get; private set; }
    private ItemType previousItemType;

    public bool IsInvincible { get; private set; }
    
    private int weaponNum = 0;
    private int itemNum = 0;
    private bool isEquip = false;

    private Coroutine defaultCoroutine;
    private Coroutine changeCoroutine;

    public enum WeaponType
    {
        Default,
        Sword,
        Bow
    }
    private void Start()
    {
        sword.enabled = false;
        bow.enabled = false;

        var behaviours = GetComponentsInChildren<PlayerBehaviour>();

        foreach (var b in behaviours)
        {
            b.Init(Controller);
        }
    }

    private void Update()
    {
        // Debug.Log(currentWeaponType);
        if (attackState.isAttacking)
            return;

        int weaponLength = System.Enum.GetValues(typeof(WeaponType)).Length;

        if (Controller.Input.ChangeWeapon != 0 && changeCoroutine == null)
        {
            weaponNum = (weaponNum + Controller.Input.ChangeWeapon + weaponLength) % weaponLength;
            Controller.Input.AckWeaponInput();
            RequestChangeCoroutine();
        }

        ChangeWeaponType((WeaponType)weaponNum);

        int itemLength = System.Enum.GetValues(typeof(WeaponType)).Length;

        if (Controller.Input.ChangeWeapon != 0 && changeCoroutine == null)
        {
            itemNum = (itemNum + Controller.Input.ChangeWeapon + itemLength) % itemLength;
            Controller.Input.AckItemInput();
            RequestChangeCoroutine();
        }

        ChangeItemType((ItemType)itemNum);
    }
    public void ChangeWeaponType(WeaponType type)
    {
        if (currentWeaponType == type)
            return;

        Debug.Log($"WaponType:{type}");

        previousWeaponType = currentWeaponType;
        currentWeaponType = type;

        switch (type)
        {
            case WeaponType.Default:
                Controller.Animation.SetWeaponType(0);
                RequestCoroutine();
                return;

            case WeaponType.Sword:
                isEquip = true;
                Controller.Animation.SetWeaponType(2);
                Controller.Animator.SetLayerWeight(2, 1);
                sword.enabled = true; // 콜라이더 활성화는 다른 코드에서
                bow.enabled = false;
                return;
            case WeaponType.Bow:
                isEquip = true;
                Controller.Animator.SetLayerWeight(2, 1);
                Controller.Animation.SetWeaponType(1);
                sword.enabled = false;
                bow.enabled = true;
                // 활 렌더러 켜기
                return;
        }
    }
    public ItemType CurrentItemType { get; private set; }
    public enum ItemType
    {
        HPPosion,
        Bomb
    }

    public void ChangeItemType(ItemType type)
    {
        if (currentItemType == type)
            return;

        Debug.Log($"WaponType:{type}");

        previousItemType = currentItemType;
        currentItemType = type;

        switch (type)
        {
            case ItemType.HPPosion:
                Controller.Animation.SetWeaponType(0);
                RequestCoroutine();
                return;

            case ItemType.Bomb:
                isEquip = true;
                Controller.Animation.SetWeaponType(2);
                Controller.Animator.SetLayerWeight(2, 1);
                sword.enabled = true; // 콜라이더 활성화는 다른 코드에서
                bow.enabled = false;
                return;
        }
    }
    public void Unequip()
    {
        isEquip = false;
    }
    // 무적
    public void ChangeInvincible(bool value) // 카운터 시간은 따로 만들어야함
    {
        IsInvincible = value;
    }

    private void Awake()
    {
        // Controller 생성
        Controller = new PlayerController(
            input,
            this,
            stateMachine,
            interaction,
            groundCheck,
            cam,
            characterController,
            dodge,
            animator,
            attack,
            attackState,
            animatorEventController
        );
        Controller.Animation.SetWeaponType(0);
    }
    private void RequestCoroutine()
    {
        if(defaultCoroutine == null)
            defaultCoroutine = StartCoroutine(DefaultWeapon());
    }
    IEnumerator DefaultWeapon()
    {
        yield return new WaitUntil(()=>!isEquip);

        Controller.Animator.SetLayerWeight(2, 0);
        sword.enabled = false;
        bow.enabled = false;
    }
    private void RequestChangeCoroutine()
    {
        if (changeCoroutine == null)
            changeCoroutine = StartCoroutine(WaitForChangeInput());
    }
    IEnumerator WaitForChangeInput()
    {
        yield return new WaitForSeconds(0.3f);
        changeCoroutine = null;
    }
}
