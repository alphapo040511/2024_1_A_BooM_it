using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SinglePlayer : MonoBehaviour
{
    [Header("Character Controller Settings")]
    public float gravity = -20;
    private float jumpImpulse = 8;
    public float acceleration = 50;
    public float breaking = 50;
    public float maxSpeed = 4;

    [Header("Camera Settings")]
    private GameObject thirdPersonCamera;
    public Transform cameraPivot;
    public float mouseSensitivity = 2.0f;       //?????? ????

    public float yMinLimit = -75;               //?????? ???? ???? ??????
    public float yMaxLimit = 75;                //?????? ???? ???? ??????

    private float theta = 0.0f;                 //???????? ???? ???? ????
    private float phi = 0.0f;                   //???????? ???? ???? ????
    private float targetVecticalRotation = 0;   //???? ???? ???? ????


    public PlayerState playerState;
    public SkillState skillState;

    public Transform firePoint;
    public Transform groundChecker;

    public CharacterController _cc;
    public Animator animator;
    public LineRenderer lineRenderer;

    public GameObject bomb;
    public GameObject shield;

    public AudioClip fireSound;

    private int currentWeapon;

    public Item weapon;
    public Item item;

    private bool aiming;
    private float angle;

    private Vector3 knockbackVel;

    private Vector3Int spawnPosition;

    public void SetSpawnPoint(Vector3Int Pos)
    {
        spawnPosition = Pos;
    }

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        thirdPersonCamera = Camera.main.gameObject;
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "Tutorial_5")
        {
            weapon = null;
        }

        Cursor.lockState = CursorLockMode.Locked;
        if(thirdPersonCamera != null)
        {
            thirdPersonCamera.transform.SetParent(cameraPivot.transform);
            thirdPersonCamera.transform.localPosition = new Vector3(0, 0, -2.5f);
            thirdPersonCamera.transform.localRotation = default;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Grounded();
        if (Cursor.lockState != CursorLockMode.Locked) return;
        CameraMovement(HandleRotation());
        Fire();
        UseItem();
        Animing();

        if(transform.position.y <= -3)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        _cc.enabled = false;
        transform.position = spawnPosition;
        _cc.enabled = true;
        _cc.Move(Vector3.zero);
    }

    private bool Grounded()
    {
        bool isGounded = false;
        Collider[] hitColliders = Physics.OverlapSphere(groundChecker.position, 0.1f, 1 << 3);
        if (hitColliders.Length > 0)
        {
            isGounded = true;
            animator.SetBool("Jump", false);
        }
        animator.SetBool("Falling", !isGounded);
        return isGounded;
    }

    private void Movement()
    {
        Vector3 newVel = _cc.velocity;

        if(Mathf.Abs(Input.GetAxis("Vertical")) < 0.1f) { animator.SetFloat("HorizontalSpeed", 0); }
        else { animator.SetFloat("HorizontalSpeed", Input.GetAxis("Vertical")); }

        if (Mathf.Abs(Input.GetAxis("Horizontal")) < 0.1f) { animator.SetFloat("VerticalSpeed", 0); }
        else { animator.SetFloat("VerticalSpeed", Input.GetAxis("Horizontal")); }

        if (Mathf.Abs(MoveDirInput().magnitude) < 0.1f) { animator.SetFloat("Speed", 0); }
        else { animator.SetFloat("Speed", Mathf.Abs(MoveDirInput().magnitude)); }

        if (newVel.y > maxSpeed * 2)
        {
            newVel.y = maxSpeed * 2;
        }

        float VelY = newVel.y;
        newVel.y = 0;

        if (playerState == PlayerState.Playing)
        {
            newVel = Vector3.ClampMagnitude(newVel + MoveDirInput() * acceleration * Time.deltaTime, maxSpeed);

            if (Input.GetKeyDown(KeyCode.Space) && Grounded())
            {
                VelY += Jump().y;
                animator.SetBool("Jump", true);
            }
        }

        if (MoveDirInput() == Vector3.zero)
        {
            newVel = Vector3.Lerp(newVel, default, breaking * Time.deltaTime);
        }

        newVel.y = VelY;

        newVel.y += gravity * Time.deltaTime;

        if (knockbackVel != default)
        {
            newVel += knockbackVel;
            knockbackVel = Vector3.Lerp(knockbackVel, default, breaking * 0.25f * Time.deltaTime);
        }


        _cc.Move(newVel * Time.deltaTime);
    }

    private void CameraMovement(Vector2 mouseDirection)
    {
        Vector2 mouseDir = default;
        mouseDir = mouseDirection;

        mouseDir.x = Mathf.Clamp(mouseDir.x, -75, 75);

        transform.rotation = Quaternion.Euler(new Vector3(0f, mouseDir.y, 0f));
        cameraPivot.localRotation = Quaternion.Euler(new Vector3(mouseDir.x, 0f, 0f));
        angle = cameraPivot.localRotation.eulerAngles.x > 180 ? 360 - cameraPivot.localRotation.eulerAngles.x : -cameraPivot.localRotation.eulerAngles.x;

        float cameraDistance = aiming ? -1 : -2.5f;
        cameraDistance = Mathf.Clamp(cameraDistance, -2.5f, -1);
        Vector3 targetPos = Vector3.forward * cameraDistance;
        thirdPersonCamera.transform.localPosition = Vector3.Lerp(thirdPersonCamera.transform.localPosition, targetPos, Time.deltaTime * 15);
    }

    public void KnockBack(Vector3 Pos, float force)
    {
        if (skillState == SkillState.Resisting)
        {
            TutorialManager.instance.AddPoint(TutorialType.UseSkill);
            SkillOff();
            return;
        }

        Vector3 knockback = (transform.position - Pos).normalized * force;
        if (knockback.y < 0)
        {
            knockback.y = 0;
        }

        knockbackVel += knockback;
    }

    public void Fire()
    {
        if (weapon == null) return;

        if (!weapon.isOnCooldown)
        {
            if (Input.GetMouseButtonDown(0))
            {
                weapon.UseItem(this);
                GameObject temp = Instantiate(bomb, firePoint.position, Quaternion.LookRotation(transform.forward));
                temp.GetComponent<TutorialBomb>().Init(angle, firePoint.position, cameraPivot.rotation);
            }
        }
    }

    public void UseItem()
    {
        if(!item.isOnCooldown)
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                item.UseItem(this);
                shield.SetActive(true);
                skillState = SkillState.Resisting;
                WeaponUIManager.instance.UseItem(item.cooldownTime);
                Invoke("SkillOff", item.duration);
            }
        }
    }

    private void SkillOff()
    {
        skillState = SkillState.None;
        shield.SetActive(false);
    }

    public void Animing()
    {
        if (weapon == null) return;

        if (Input.GetMouseButton(1))
        {
            Vector3[] point = bomb.GetComponent<TutorialBomb>().Trajectory(angle, firePoint, cameraPivot);
            lineRenderer.positionCount = point.Length;
            for (int i = 0; i < point.Length; i++)
            {
                lineRenderer.SetPosition(i, point[i]);
            }
            lineRenderer.enabled = true;

            aiming = true;
        }
        else
        {
            lineRenderer.enabled = false;
            aiming = false;
        }
    }

    private Vector3 Jump()
    {
        return new Vector3(0, jumpImpulse, 0);
    }

    Vector3 MoveDirInput()
    {
        Vector3 vertical = Input.GetAxis("Vertical") * transform.forward;
        Vector3 horizontal = Input.GetAxis("Horizontal") * transform.right;

        return (vertical + horizontal).normalized;
    }

    Vector3 HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;         //?????? ???? ????
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;         //?????? ???? ????

        //???? ????(theta ??)
        theta += mouseX;                            //?????? ?????? ????
        theta = Mathf.Repeat(theta, 360.0f);        //???? ???? 360?? ???? ?????? ????

        //???? ???? ????
        targetVecticalRotation -= mouseY;
        //targetVecticalRotation = Mathf.Clamp(targetVecticalRotation, yMinLimit, yMaxLimit);     //???? ???? ????
        phi = targetVecticalRotation;       //?????? ???????? ?????? ???????? ???? ????
        //phi = Mathf.MoveTowards(phi, targetVecticalRotation, verticalRotationSpeed * Time.DeltaTime);

        return new Vector3(phi, theta);
    }
}
