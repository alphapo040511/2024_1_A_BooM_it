using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Player : NetworkBehaviour
{
    [Networked] public float angle { get; set; }

    private NetworkCharacterController _cc;

    [SerializeField] private NetworkPrefabRef _prefabBall;
    [SerializeField] private TickTimer fireDelay { get; set; }
    [SerializeField] private TickTimer jumpDelay { get; set; }
    [SerializeField] private NetworkButtons _networkButtons { get; set; }

    [SerializeField] private Transform cameraPivot;

    public NetworkMecanimAnimator _animator;
    public Transform FirePoint;

    private GameObject thirdPersonCamera;
    private float cameraDistance;

    [Networked] public float initTheta { get; set; }

    public void Init(float angle)
    {
        initTheta = angle;
    }

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
        Cursor.lockState = CursorLockMode.Locked;           //���콺 Ŀ���� ��װ� �����.
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
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            _networkButtons = data.buttons;

            Vector3 moveDirection = data.direction;
            Vector2 mouseDirection = data.lookDirection;
            float wheel = data.wheel;


            _animator.Animator.SetFloat("HorizontalSpeed", moveDirection.x);
            _animator.Animator.SetFloat("VerticalSpeed", moveDirection.z);


            moveDirection = transform.forward * moveDirection.x + transform.right * moveDirection.z;

            _cc.Move(moveDirection);
            

            transform.rotation = Quaternion.Euler(new Vector3(0f, mouseDirection.y + initTheta, 0f));
            cameraPivot.localRotation = Quaternion.Euler(new Vector3(mouseDirection.x, 0f, 0f));
            if (HasInputAuthority)
            { 
                cameraDistance += wheel;
                cameraDistance = Mathf.Clamp(cameraDistance, -2.5f, -1);
                Vector3 targetPos = Vector3.forward * cameraDistance;
                thirdPersonCamera.transform.localPosition = Vector3.Lerp(thirdPersonCamera.transform.localPosition, targetPos, Runner.DeltaTime * 15);
            }
            angle = cameraPivot.localRotation.eulerAngles.x > 180 ? 360 - cameraPivot.localRotation.eulerAngles.x : -cameraPivot.localRotation.eulerAngles.x;
        }
        CheckAndFireProjectile();
        CheckAndJump();
    }


    private void CheckAndJump()
    {
        if (jumpDelay.ExpiredOrNotRunning(Runner))
        {
            if (_networkButtons.IsSet(NetworkInputData.KEYBOARDSPACE))
            {
                jumpDelay = TickTimer.CreateFromSeconds(Runner, 0.2f);
                _cc.Jump();
            }
        }
    }

    private void CheckAndFireProjectile()                   //üũ�ϰ� ��� �Լ�
    {
        if (fireDelay.ExpiredOrNotRunning(Runner))
        {
            if (_networkButtons.IsSet(NetworkInputData.MOUSEBUTTON0))        //��ư ������ �� �����ͼ� �����Ѵ�.
            {
                fireDelay = TickTimer.CreateFromSeconds(Runner, 0.5f);          //0.5�� �������� ���.
                if(HasInputAuthority) RPC_SendMessage_Fire();
                FirePosition();
            }
        }
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

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_SendMessage_Fire()
    {
        RPC_RelayMessage_Fire();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_RelayMessage_Fire()
    {
        _animator.Animator.SetTrigger("Fire");
    }
}