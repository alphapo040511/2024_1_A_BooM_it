using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [Networked] public float angle { get; set; }

    private NetworkCharacterController _cc;
    public float moveSpeed = 5.0f;

    [SerializeField] private NetworkPrefabRef _prefabBall;
    [SerializeField] private TickTimer delay { get; set; }
    [SerializeField] private NetworkButtons _networkButtons { get; set; }

    [SerializeField] private Transform cameraPivot;


    private GameObject thirdPersonCamera;
    private float cameraDistance;

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

            moveDirection = transform.forward * moveDirection.x + transform.right * moveDirection.z;

            if (moveDirection.magnitude > 1)
            {
                moveDirection.Normalize();
            }

            _cc.Move(moveDirection * moveSpeed * Runner.DeltaTime);

            transform.rotation = Quaternion.Euler(new Vector3(0f, mouseDirection.y, 0f));
            cameraPivot.localRotation = Quaternion.Euler(new Vector3(mouseDirection.x, 0f, 0f));
            if (HasInputAuthority)
            { 
                cameraDistance += wheel;
                cameraDistance = Mathf.Clamp(cameraDistance, -8, -3);
                Vector3 targetPos = Vector3.forward * cameraDistance;
                thirdPersonCamera.transform.localPosition = Vector3.Lerp(thirdPersonCamera.transform.localPosition, targetPos, Runner.DeltaTime * 2);
            }
            angle = cameraPivot.localRotation.eulerAngles.x > 180 ? 360 - cameraPivot.localRotation.eulerAngles.x : -cameraPivot.localRotation.eulerAngles.x;
        }
        CheckAndFireProjectile();
    }

    private void CheckAndFireProjectile()                   //üũ�ϰ� ��� �Լ�
    {
        if (delay.ExpiredOrNotRunning(Runner))
        {
            if (_networkButtons.IsSet(NetworkInputData.MOUSEBUTTON0))        //��ư ������ �� �����ͼ� �����Ѵ�.
            {
                delay = TickTimer.CreateFromSeconds(Runner, 0.5f);          //0.5�� �������� ���.
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
                transform.position + forward,
                Quaternion.LookRotation(forward),
                Object.InputAuthority,
                (runner, o) => o.GetComponent<NetworkParabola>().Init(angle, transform.position + forward));
        }
    }

}