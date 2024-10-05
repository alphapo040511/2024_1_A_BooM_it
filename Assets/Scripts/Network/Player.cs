using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [Range(5, 85)] public float angle = 15;

    private NetworkCharacterController _cc;
    public float moveSpeed = 5.0f;

    [SerializeField] private NetworkPrefabRef _prefabBall;
    [SerializeField] private TickTimer delay { get; set; }
    [SerializeField] private NetworkButtons _networkButtons { get; set; }

    [SerializeField] private Transform cameraPivot;
    private NetworkCameraFollow mainCamera;

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
    }

    public override void Spawned()
    {
        if(HasInputAuthority)
        {
            mainCamera = NetworkCameraFollow.instanse;
            mainCamera.SetPlayerTransform(transform, cameraPivot);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            _networkButtons = data.buttons;

            Vector3 moveDirection = data.direction;
            Vector2 mouseDirection = data.lookDirection;

            moveDirection = transform.forward * moveDirection.x + transform.right * moveDirection.z;

            if (moveDirection.magnitude > 1)
            {
                moveDirection.Normalize();
            }

            _cc.Move(moveDirection * moveSpeed * Runner.DeltaTime);

            mainCamera.CameraRotate(mouseDirection.x, mouseDirection.y, Runner.DeltaTime);
        }
        CheckAndFireProjectile();
    }

    public override void Render()
    {
        if(mainCamera != null)
        {
            mainCamera.UpdateCameraPosition();
        }
    }

    private void CheckAndFireProjectile()                   //체크하고 쏘는 함수
    {
        if (delay.ExpiredOrNotRunning(Runner))
        {
            if (_networkButtons.IsSet(NetworkInputData.MOUSEBUTTON0))        //버튼 선언한 것 가져와서 진행한다.
            {
                delay = TickTimer.CreateFromSeconds(Runner, 0.5f);          //0.5초 간격으로 쏜다.
                FirePosition();
            }
        }
    }

    private void FirePosition()                                             //발사체 생성 함수
    {
        if (Object.HasStateAuthority)
        {
            Vector3 forward = transform.forward;
            Runner.Spawn(_prefabBall,
                transform.position + forward,
                Quaternion.LookRotation(forward),
                Object.InputAuthority,
                (runner, o) => o.GetComponent<NetworkParabola>().Init(angle));
        }
    }

}