using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;

public enum PlayerState
{
    Ready,         // 준비중
    Loading,       // 로딩중
    Standby,       // 대기중
    Playing,       // 플레이중
    Die,           // 사망
    Spectating     // 관전중
}

public enum SkillState
{
    None,           //아무 효과 없음
    Resisting,      //저항중 (쉴드)
    SpeedUp,        //이속증가
}

public class Player : NetworkBehaviour
{
    [Networked] public float angle { get; set; }
    [Networked] public float rotateAngle { get; set; }
    [Networked] public Vector2 mouseInput { get; set; }
    [Networked] public PlayerState state { get; set; }
    [Networked] public SkillState skillState { get; set; }

    private NetworkCharacterController _cc;

    private TickTimer changeDelay { get; set; }
    private TickTimer jumpDelay { get; set; }
    private NetworkButtons _networkButtons { get; set; }

    [SerializeField] private Transform cameraPivot;

    public NetworkMecanimAnimator _animator;
    public Transform FirePoint;
    public LineRenderer lineRenderer;
    public MeshTrail meshTrail;
    public GameObject Shield;
    public AudioSource audioSource;

    public AudioClip fireSound;

    private GameObject thirdPersonCamera;
    private float cameraDistance;

    [Networked] public int currentWeapon { get; set; }

    public Item item;
    public Item[] weapon = new Item[3];

    [Networked] public bool aiming { get; set; }

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
    }

    public void Init()
    {
        
    }

    public override void Spawned()
    {
        if (HasInputAuthority)
        {
            if (thirdPersonCamera == null)
            {
                thirdPersonCamera = Camera.main.gameObject;
            }
            thirdPersonCamera.transform.SetParent(cameraPivot.transform);
            thirdPersonCamera.transform.localPosition = new Vector3(0, 0, -5);
            thirdPersonCamera.transform.localRotation = default;
            cameraDistance = thirdPersonCamera.transform.localPosition.z;
            RPC_ItemSpawn(GameManager.instance.weaponIndex, GameManager.instance.itemIndex);
        }

        string[] mapName = GameManager.instance.mapIndex.ToString().Split('_');
        if (mapName[0] == "Ice")
        {
            _cc.braking = 5;
            _cc.acceleration = 5;
        }
    }

    [Rpc(RpcSources.All , RpcTargets.All)]
    public void RPC_ItemSpawn(string[] weaponIndex, string itemIndex)
    {
        for (int i = 0; i < weapon.Length; i++)
        {
            string weaponPath = Path.Combine("Weapons", weaponIndex[i]);
            if (weaponIndex[i] == "None")
            {
                weaponPath = Path.Combine("Weapons", "BasicBomb");
            }
            GameObject bomb = Instantiate(Resources.Load<GameObject>(weaponPath), transform);
            weapon[i] = bomb.GetComponent<Item>();
        }

        string path = Path.Combine("Items", itemIndex);
        if(itemIndex == "None")
        {
            path = Path.Combine("Items", "Shield");
        }
        GameObject temp = Instantiate(Resources.Load<GameObject>(path), transform);
        item = temp.GetComponent<Item>();


        if(HasInputAuthority)
        {
            WeaponUIManager.instance.SetWeapons(weapon[0].itemImage, weapon[2].itemImage, weapon[1].itemImage, item.itemImage);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (transform.position.y < -5)
        {
            if (state == PlayerState.Playing)
            {
                UpdataState(PlayerState.Die);
                if (HasInputAuthority)
                {
                    BattleManager.Instance.RPC_PlayerValueChange(Runner.LocalPlayer);
                    thirdPersonCamera.transform.SetParent(null);
                } 
            }
            if (state == PlayerState.Die && HasInputAuthority)
            {
                thirdPersonCamera.transform.LookAt(cameraPivot);
            }
        }

        if (GetInput(out NetworkInputData data))
        {
            _networkButtons = data.buttons;
            Vector3 moveDirection = data.direction;
            Vector2 mouseDirection = data.lookDirection;
            float wheel = data.wheel;

            PlayerMovement(moveDirection);

            if (mouseDirection != Vector2.zero) CameraMovement(mouseDirection);

            if (wheel != 0) ChangeWeapon(wheel);

            if (Grounded())
            {
                CheckAndJump();
            }
            CheckAndFireProjectile();
            UseItem();
        }
    }

    public void Respawn(Vector3 position, Quaternion lookDir)
    {
        _cc.Teleport(position, Quaternion.identity);
        cameraPivot.localRotation = Quaternion.identity;
        rotateAngle = lookDir.eulerAngles.y;
        angle = 0;
    }

    public void UpdataState(PlayerState newState)
    {
        state = newState;
        ResetParameters();
        RPC_UpdateSkillState();
        if (HasStateAuthority)
        {
            RPC_Reset((int)newState);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_UpdateSkillState(SkillState state = SkillState.None)
    {
        skillState = state;
        Shield.SetActive(false);
        if (HasInputAuthority && state != SkillState.None)
        {
            WeaponUIManager.instance.UseItem(item.cooldownTime);
        }

        switch (skillState)
        {
            case SkillState.SpeedUp:
                meshTrail.Use();
                if (HasStateAuthority) Invoke("OffSkill", item.duration);
                break;
            case SkillState.Resisting:
                Shield.SetActive(true);
                if (HasStateAuthority) Invoke("OffSkill", item.duration);
                break;
        }
    }

    private void OffSkill()
    {
        RPC_UpdateSkillState(SkillState.None);
    }

    private void ResetParameters()
    {
        _animator.Animator.SetFloat("HorizontalSpeed", 0);
        _animator.Animator.SetFloat("VerticalSpeed", 0);
        _animator.Animator.SetFloat("Speed", 0);
        _animator.Animator.SetBool("Jump", false);
        _animator.Animator.SetBool("Falling", false);
        _animator.Animator.SetInteger("Fire", 0);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_Reset(int stateIndex)
    {
        Cursor.lockState = stateIndex >= 1 ? CursorLockMode.Locked : CursorLockMode.None;
        if(stateIndex == 2)
        {
            if (HasInputAuthority)
            {
                thirdPersonCamera.transform.SetParent(cameraPivot.transform);
                thirdPersonCamera.transform.localPosition = new Vector3(0, 0, -2.5f);
                thirdPersonCamera.transform.localRotation = default;
                cameraDistance = thirdPersonCamera.transform.localPosition.z;
            }
        }
        else if(stateIndex == 3)
        {
            foreach(Item weaponData in weapon)
            {
                weaponData.Reset();
            }

            item.Reset();
            if(HasInputAuthority)
            {
                WeaponUIManager.instance.ResetItem();
            }
        }
    }

    public void Knockback(Vector3 bombPos, float knockbackForce = 10)
    {
        if (HasStateAuthority)
        {
            if(skillState == SkillState.Resisting)
            {
                RPC_UpdateSkillState(SkillState.None);
                return;
            }

            Vector3 knockback = (transform.position - bombPos).normalized * knockbackForce;
            if (knockback.y < 0)
            {
                knockback.y = 0;
            }
            _cc.Knockback(knockback);
        }
    }

    //움직임 관련 로직
    private void PlayerMovement(Vector3 moveDirection)
    {
        if (state == PlayerState.Die)
        {
            _animator.Animator.SetFloat("Speed", Mathf.Abs(moveDirection.magnitude));
            _cc.Move(default);
        }

        if (state != PlayerState.Playing)
        {
            return;
        }

        //애니메이션 동기화
        _animator.Animator.SetFloat("HorizontalSpeed", moveDirection.x);
        _animator.Animator.SetFloat("VerticalSpeed", moveDirection.z);
        _animator.Animator.SetFloat("Speed",Mathf.Abs(moveDirection.magnitude));
        moveDirection = transform.forward * moveDirection.x + transform.right * moveDirection.z * 0.5f;
        _cc.Move(moveDirection, skillState == SkillState.SpeedUp);
    }

    //카메라 움직임 관련 로직
    private void CameraMovement(Vector2 mouseDirection)
    {
        Vector2 mouseDir = default;
        if (state == PlayerState.Playing)
        {
            mouseDir = mouseDirection - mouseInput;
        }
        else
        {
            mouseInput = mouseDirection;
            if(state == PlayerState.Die)
            {
                return;
            }
        }

        mouseDir.x = Mathf.Clamp(mouseDir.x, -75, 75);

        transform.rotation = Quaternion.Euler(new Vector3(0f, mouseDir.y + rotateAngle, 0f));
        cameraPivot.localRotation = Quaternion.Euler(new Vector3(mouseDir.x, 0f, 0f));
        angle = cameraPivot.localRotation.eulerAngles.x > 180 ? 360 - cameraPivot.localRotation.eulerAngles.x : -cameraPivot.localRotation.eulerAngles.x;
        if (HasInputAuthority)
        {
            cameraDistance = aiming ? -1 : -2.5f;
            cameraDistance = Mathf.Clamp(cameraDistance, -2.5f, -1);
            Vector3 targetPos = Vector3.forward * cameraDistance;
            thirdPersonCamera.transform.localPosition = Vector3.Lerp(thirdPersonCamera.transform.localPosition, targetPos, Runner.DeltaTime * 15);
        }
    }

    private void ChangeWeapon(float wheel)
    {
        if (state != PlayerState.Playing) return;

        if (changeDelay.ExpiredOrNotRunning(Runner))
        {
            currentWeapon += wheel > 0 ? 1 : -1;
            currentWeapon = (int)Mathf.Repeat(currentWeapon, weapon.Length);
            changeDelay = TickTimer.CreateFromSeconds(Runner, 0.2f);
            if(HasInputAuthority) 
            { 
                int previous = (int)Mathf.Repeat(currentWeapon - 1, weapon.Length);
                int next = (int)Mathf.Repeat(currentWeapon +1, weapon.Length);
                WeaponUIManager.instance.ChangeWeapons(weapon[currentWeapon].itemImage, weapon[previous].itemImage, weapon[next].itemImage);
            }
        }
    }


    //점프 체크
    private void CheckAndJump()
    {
        if (state != PlayerState.Playing) return;

        if (jumpDelay.ExpiredOrNotRunning(Runner))
        {
            if (_networkButtons.IsSet(NetworkInputData.KEYCODESPACE))
            {
                jumpDelay = TickTimer.CreateFromSeconds(Runner, 0.2f);
                _animator.Animator.SetBool("Jump", true);
                _cc.Jump();
            }
        }
    }

    //발사 체크
    private void CheckAndFireProjectile()
    {
        if (state != PlayerState.Playing) return;

        if (_networkButtons.IsSet(NetworkInputData.MOUSEBUTTON0))
        {
            if (weapon[currentWeapon].isUsable)        //발사 가능 상태일때
            {
                weapon[currentWeapon].UseItem(this);
            }
        }

        if (_networkButtons.IsSet(NetworkInputData.MOUSEBUTTON1))
        {
            if (HasInputAuthority)
            {
                Vector3[] point = weapon[0].bombParabola.Trajectory(angle, FirePoint, cameraPivot);
                lineRenderer.positionCount = point.Length;
                for (int i = 0; i < point.Length; i++)
                {
                    lineRenderer.SetPosition(i, point[i]);
                }
                lineRenderer.enabled = true;

                aiming = true;
            }
        }
        else
        {
            lineRenderer.enabled = false;
            aiming = false;
        }
    }

    private void UseItem()
    {
        if (state != PlayerState.Playing) return;

        if (_networkButtons.IsSet(NetworkInputData.KEYCODER))
        {
            if (item.isUsable)
            {
                if (HasStateAuthority)
                {
                    item.UseItem(this);
                }
            }
        }
    }

    private bool Grounded()
    {
        _animator.Animator.SetBool("Falling", !_cc.Grounded);
        if(_cc.Grounded)
        {
            _animator.Animator.SetBool("Jump", false);
        }

        return _cc.Grounded;
    }

    private IEnumerator AnimationDelay()
    {
        yield return new WaitForSeconds(0.1f);
        _animator.Animator.SetInteger("Fire", 0);
    }

    public void FirePosition(NetworkPrefabRef bomb)                                             //?????? ???? ????
    {
        audioSource.PlayOneShot(fireSound);
        if (Object.HasStateAuthority)
        {
            _animator.Animator.SetInteger("Fire", 1);
            StartCoroutine(AnimationDelay());


            Vector3 forward = transform.forward;
            Runner.Spawn(bomb,
                FirePoint.position,
                Quaternion.LookRotation(forward),
                Object.InputAuthority,
                (runner, o) => o.GetComponent<NetworkParabola>().Init(angle, FirePoint.position, cameraPivot.rotation));
        }
    }
}