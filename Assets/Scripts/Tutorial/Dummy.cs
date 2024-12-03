using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Dummy : MonoBehaviour
{
    public DummyType dummyType;

    public float angle = 45;

    public GameObject character;
    public Transform firePoint;

    public CharacterController controller;
    public Animator animator;

    private Vector3 knockbackVel;
    private Vector3Int spawnPosition;

    public GameObject BombPrefabs;

    private bool isWorking = false;
    private bool isDead = false;

    public void Spawn(Vector3Int pos)
    {
        character.SetActive(true);
        spawnPosition = pos;
        controller.enabled = false;
        transform.position = spawnPosition;
        controller.enabled = true;
    }

    public void Respawn()
    {
        controller.enabled = false;
        transform.position = spawnPosition;
        controller.enabled = true;
        isDead = false;
        character.SetActive(true);
    }

    private void Update()
    {
        if(transform.position.y <= -3)
        {
            character.SetActive(false);
            if(!isDead)
            {
                isDead = true;
                TutorialManager.instance.AddPoint(TutorialType.KillDummy);
            }

            return;
        }

        if(isWorking == false && dummyType == DummyType.Fire)
        {
            isWorking = true;
            StartCoroutine(FireCount());
        }

        Vector3 vel = controller.velocity;


        float Y = vel.y;

        vel = Vector3.ClampMagnitude(vel, 4);
        vel = Vector3.Lerp(vel, default, 50 * Time.deltaTime);

        if(Y > 8)
        {
            Y = 8;
        }

        vel.y = Y;

        if (knockbackVel != Vector3.zero)
        {
            vel += knockbackVel;
            knockbackVel = Vector3.Lerp(knockbackVel, default, 50 * 0.25f * Time.deltaTime);
        }

        if (!controller.isGrounded)
        {
            vel.y +=  -9.81f * Time.deltaTime;
        }



        controller.Move(vel * Time.deltaTime);
    }

    public void KnockBack(Vector3 Pos, float force)
    {
        if ((int)dummyType >= 1)
        {
            return;
        }

        Vector3 knockback = (transform.position - Pos).normalized * force;
        if (knockback.y < 0)
        {
            knockback.y = 0;
        }

        knockbackVel += knockback;
    }

    private IEnumerator FireCount()
    {
        yield return new WaitForSeconds(5);
        Fire();
        yield return new WaitForSeconds(5);
        isWorking = false;
    }

    public void Fire()
    {
        GameObject temp = Instantiate(BombPrefabs, firePoint.position, firePoint.rotation);
        temp.GetComponent<TutorialBomb>().Init(angle, firePoint.position, transform.rotation);
    }
}
