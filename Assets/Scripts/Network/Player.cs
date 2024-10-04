using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [Range(5, 85)] public float angle = 15;

    private NetworkCharacterController _cc;
    public float moveSpeed = 5.0f;

    [SerializeField] private NetworkPrefabRef _prefabBall;
    [SerializeField] private TickTimer delay { get; set; }
    [SerializeField] private NetworkButtons _networkButtons { get; set; }

    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            _networkButtons = data.buttons;

            Vector3 moveDirection = data.direction;
            _cc.Move(moveDirection * moveSpeed * Runner.DeltaTime);

            if (moveDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(moveDirection);
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
                (runner, o) => o.GetComponent<NetworkParabola>().Init(angle));
        }
    }

}