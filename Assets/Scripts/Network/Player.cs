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

    private GameObject mainCamera;
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
            mainCamera = Camera.main.gameObject;
            mainCamera.transform.SetParent(cameraPivot.transform);
            mainCamera.transform.localPosition = new Vector3(0, 0, -5);
            mainCamera.transform.localRotation = default;
            cameraDistance = mainCamera.transform.localPosition.z;
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
                mainCamera.transform.localPosition = Vector3.Lerp(mainCamera.transform.localPosition, targetPos, Runner.DeltaTime * 2);
            }
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