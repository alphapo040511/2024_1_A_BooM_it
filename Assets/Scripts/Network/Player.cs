using Fusion;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PlayerState
{
    Ready,         // 준비 중
    Loading,       // 로딩 중
    Standby,       // 게임 시작 카운트다운
    Playing,       // 게임 중
    Die,           // 게임 종료
    Spectating     // 관전
}

public class Player : NetworkBehaviour
{
    [Networked] public float angle { get; set; }
    [Networked] public float rotateAngle { get; set; }
    [Networked] public Vector2 mouseInput { get; set; }
    [Networked] public PlayerState state { get; set; }

    private NetworkCharacterController _cc;

    private TickTimer fireDelay { get; set; }
    private TickTimer jumpDelay { get; set; }
    private NetworkButtons _networkButtons { get; set; }

    [SerializeField] private NetworkParabola parabola;
    [SerializeField] private NetworkPrefabRef ballPre;

    [SerializeField] private Transform cameraPivot;

    public NetworkMecanimAnimator _animator;
    public Transform FirePoint;
    public LineRenderer lineRenderer;

    private GameObject thirdPersonCamera;
    private float cameraDistance;

    [Networked] public int weaponIndex { get; set; }

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
            ballPre = GameManager.instance.bombPrefabs[0];
            parabola = GameManager.instance.parabolas[0];
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (transform.position.y < -3)
        {
            if (state == PlayerState.Playing)
            {
                UpdataState(PlayerState.Die);
                if (HasInputAuthority)
                {
                    BattleManager.Instance.RPC_PlayerValueChange(Runner.LocalPlayer);
                }
                thirdPersonCamera.transform.SetParent(null);
            }
            if (state == PlayerState.Die && HasInputAuthority)
            {
                thirdPersonCamera.transform.LookAt(cameraPivot);
            }
        }

        if (GetInput(out NetworkInputData data))
        {
            //인풋 데이터 받아오는 부분
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
        if (HasStateAuthority)
        {
            RPC_ChangeMouseMode((int)newState);
        }
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
    public void RPC_ChangeMouseMode(int stateIndex)
    {
        Cursor.lockState = stateIndex >= 1 ? CursorLockMode.Locked : CursorLockMode.None;
        if(stateIndex == 2)
        {
            if (HasInputAuthority)
            {
                thirdPersonCamera.transform.SetParent(cameraPivot.transform);
                thirdPersonCamera.transform.localPosition = new Vector3(0, 0, -5);
                thirdPersonCamera.transform.localRotation = default;
                cameraDistance = thirdPersonCamera.transform.localPosition.z;
                ballPre = GameManager.instance.bombPrefabs[0];
                parabola = GameManager.instance.parabolas[0];
            }
        }
    }

    public void Knockback(Vector3 bombPos)
    {
        if (HasStateAuthority)
        {
            Vector3 knockback = (transform.position - bombPos).normalized * 10;
            if (knockback.y < 0)
            {
                knockback.y = 0;
            }
            _cc.Knockback(knockback);
        }
    }

    //캐릭터 움직임
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
        _cc.Move(moveDirection);
    }

    //카메라 움직임
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
            cameraDistance = Mathf.Clamp(cameraDistance, -2.5f, -1);
            Vector3 targetPos = Vector3.forward * cameraDistance;
            thirdPersonCamera.transform.localPosition = Vector3.Lerp(thirdPersonCamera.transform.localPosition, targetPos, Runner.DeltaTime * 15);
        }
    }

    private void ChangeWeapon(float wheel)
    {
        if(HasInputAuthority)
        {
            if(wheel > 0)
            {
                weaponIndex++;
            }
            else
            {
                weaponIndex--;
            }

            if (weaponIndex >= GameManager.instance.bombPrefabs.Count)
            {
                weaponIndex = GameManager.instance.bombPrefabs.Count - 1;
            }
            else if(weaponIndex < 0)
            {
                weaponIndex = 0;
            }

            ballPre = GameManager.instance.bombPrefabs[weaponIndex];
            parabola = GameManager.instance.parabolas[weaponIndex];
        }
    }


    //점프 확인
    private void CheckAndJump()
    {
        if (state != PlayerState.Playing) return;

        if (jumpDelay.ExpiredOrNotRunning(Runner))
        {
            if (_networkButtons.IsSet(NetworkInputData.KEYBOARDSPACE))
            {
                jumpDelay = TickTimer.CreateFromSeconds(Runner, 0.2f);
                _animator.Animator.SetBool("Jump", true);
                _cc.Jump();
            }
        }
    }

    //발사 확인
    private void CheckAndFireProjectile()
    {
        if (state != PlayerState.Playing) return;

        if (fireDelay.ExpiredOrNotRunning(Runner))
        {
            if (_networkButtons.IsSet(NetworkInputData.MOUSEBUTTON0))        //버튼 선언한 것 가져와서 진행한다.
            {
                fireDelay = TickTimer.CreateFromSeconds(Runner, 0.5f);          //0.5초 간격으로 쏜다.
                _animator.Animator.SetInteger("Fire", 1);
                StartCoroutine(AnimationDelay());
                FirePosition();
            }
        }


        if (HasInputAuthority)
        {
            if (_networkButtons.IsSet(NetworkInputData.MOUSEBUTTON1))
            {
                Vector3[] point = parabola.Trajectory(angle, FirePoint, cameraPivot);
                lineRenderer.positionCount = point.Length;
                for (int i = 0; i < point.Length; i++)
                {
                    lineRenderer.SetPosition(i, point[i]);
                }
                lineRenderer.enabled = true;
            }
            else
            {
                lineRenderer.enabled = false;
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

    private void FirePosition()                                             //발사체 생성 함수
    {
        if (Object.HasStateAuthority)
        {
            Vector3 forward = transform.forward;
            Runner.Spawn(ballPre,
                FirePoint.position,
                Quaternion.LookRotation(forward),
                Object.InputAuthority,
                (runner, o) => o.GetComponent<NetworkParabola>().Init(angle, FirePoint.position, cameraPivot.rotation));
        }
    }
}