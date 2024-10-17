using Fusion;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public enum PlayerState
{
    Ready,         // �غ� ��
    Standby,       // ���� ���� ī��Ʈ�ٿ�
    Playing,       // ���� ��
    GameOver,      // ���� ����
    Spectating     // ����
}

public class Player : NetworkBehaviour
{
    [Networked] public float angle { get; set; }
    [Networked] public PlayerState state { get; set; }

    private NetworkCharacterController _cc;

    private TickTimer fireDelay { get; set; }
    private TickTimer jumpDelay { get; set; }
    private NetworkButtons _networkButtons { get; set; }

    [SerializeField] private NetworkPrefabRef _prefabBall;

    [SerializeField] private Transform cameraPivot;

    public NetworkMecanimAnimator _animator;
    public Transform FirePoint;

    private GameObject thirdPersonCamera;
    private float cameraDistance;

    [Networked] public float initTheta { get; set; }
    [Networked] public int weaponIndex { get; set; }

    public void Init(float angle)
    {
        initTheta = angle;
        if(HasInputAuthority)
        {
            NetworkLevelManager.instance.battleManager.thisPlayerHash = Runner.LocalPlayer.GetHashCode();
        }
    }

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
    }

    public override void Spawned()
    {
        if (HasInputAuthority)
        {
            thirdPersonCamera = Camera.main.gameObject;
            thirdPersonCamera.transform.SetParent(cameraPivot.transform);
            thirdPersonCamera.transform.localPosition = new Vector3(0, 0, -5);
            thirdPersonCamera.transform.localRotation = default;
            cameraDistance = thirdPersonCamera.transform.localPosition.z;
            _prefabBall = GameManager.instance.bombPrefabs[0];
        }
        UpdataState(PlayerState.Playing);       //�ϴ� �÷��� �� ���·� ����
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {   
            //��ǲ ������ �޾ƿ��� �κ�
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

    public void UpdataState(PlayerState newState)
    {
        state = newState;
        if(newState == PlayerState.Playing || newState == PlayerState.Spectating) 
        {
            Cursor.lockState = CursorLockMode.Locked;           //���콺 Ŀ���� ��װ� �����.
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    //ĳ���� ������
    private void PlayerMovement(Vector3 moveDirection)
    {
        if (state != PlayerState.Playing) return;

        //�ִϸ��̼� ����ȭ
        _animator.Animator.SetFloat("HorizontalSpeed", moveDirection.x);
        _animator.Animator.SetFloat("VerticalSpeed", moveDirection.z);
        _animator.Animator.SetFloat("Speed",Mathf.Abs(moveDirection.magnitude));
        moveDirection = transform.forward * moveDirection.x + transform.right * moveDirection.z;
        _cc.Move(moveDirection);
    }

    //ī�޶� ������
    private void CameraMovement(Vector2 mouseDirection)
    {
        if (state != PlayerState.Standby && state != PlayerState.Playing) return;

        transform.rotation = Quaternion.Euler(new Vector3(0f, mouseDirection.y + initTheta, 0f));
        cameraPivot.localRotation = Quaternion.Euler(new Vector3(mouseDirection.x, 0f, 0f));
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

            _prefabBall = GameManager.instance.bombPrefabs[weaponIndex];
        }
    }


    //���� Ȯ��
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

    //�߻� Ȯ��
    private void CheckAndFireProjectile()
    {
        if (state != PlayerState.Playing) return;

        if (fireDelay.ExpiredOrNotRunning(Runner))
        {
            if (_networkButtons.IsSet(NetworkInputData.MOUSEBUTTON0))        //��ư ������ �� �����ͼ� �����Ѵ�.
            {
                fireDelay = TickTimer.CreateFromSeconds(Runner, 0.5f);          //0.5�� �������� ���.
                _animator.Animator.SetInteger("Fire", 1);
                StartCoroutine(AnimationDelay());
                FirePosition();
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

    private void FirePosition()                                             //�߻�ü ���� �Լ�
    {
        if (Object.HasStateAuthority)
        {
            Vector3 forward = transform.forward;
            Runner.Spawn(_prefabBall,
                FirePoint.position,
                Quaternion.LookRotation(forward),
                Object.InputAuthority,
                (runner, o) => o.GetComponent<NetworkParabola>().Init(angle, FirePoint.position));
        }
    }
}