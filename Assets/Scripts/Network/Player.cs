using Fusion;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public enum PlayerState
{
    NotReady,      // 준비 전
    Ready,         // 준비 완료
    Standby,       // 게임 시작 카운트다운
    Playing,       // 게임 중
    GameOver,      // 게임 종료
    Spectating     // 관전
}

public class Player : NetworkBehaviour
{
    [Networked] public float angle { get; set; }
    [Networked] public PlayerState state { get; set; }

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
        Cursor.lockState = CursorLockMode.Locked;           //마우스 커서를 잠그고 숨긴다.
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
            //인풋 데이터 받아오는 부분
            _networkButtons = data.buttons;
            Vector3 moveDirection = data.direction;
            Vector2 mouseDirection = data.lookDirection;
            float wheel = data.wheel;

            //애니메이션 동기화
            _animator.Animator.SetFloat("HorizontalSpeed", moveDirection.x);
            _animator.Animator.SetFloat("VerticalSpeed", moveDirection.z);


            moveDirection = transform.forward * moveDirection.x + transform.right * moveDirection.z;

            _cc.Move(moveDirection);

            CameraMovement(mouseDirection, wheel);

            //발사체, 점프 확인
            CheckAndFireProjectile();
            CheckAndJump();
        }
    }

    //카메라 움직임
    private void CameraMovement(Vector3 mouseDirection, float wheel)
    {
        transform.rotation = Quaternion.Euler(new Vector3(0f, mouseDirection.y + initTheta, 0f));
        cameraPivot.localRotation = Quaternion.Euler(new Vector3(mouseDirection.x, 0f, 0f));
        angle = cameraPivot.localRotation.eulerAngles.x > 180 ? 360 - cameraPivot.localRotation.eulerAngles.x : -cameraPivot.localRotation.eulerAngles.x;
        if (HasInputAuthority)
        {
            cameraDistance += wheel;
            cameraDistance = Mathf.Clamp(cameraDistance, -2.5f, -1);
            Vector3 targetPos = Vector3.forward * cameraDistance;
            thirdPersonCamera.transform.localPosition = Vector3.Lerp(thirdPersonCamera.transform.localPosition, targetPos, Runner.DeltaTime * 15);
        }
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

    private void CheckAndFireProjectile()                   //체크하고 쏘는 함수
    {
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
            Runner.Spawn(_prefabBall,
                FirePoint.position,
                Quaternion.LookRotation(forward),
                Object.InputAuthority,
                (runner, o) => o.GetComponent<NetworkParabola>().Init(angle, FirePoint.position));
        }
    }
}