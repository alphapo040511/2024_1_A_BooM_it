using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class ShotBoom : MonoBehaviour
{
    public GameObject BoomPrefab;

    public BoomType boomType;

    public Transform firePoint;

    private Ray ray;
    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

        if (Input.GetMouseButtonDown(0))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.Log("น฿ป็");
            if (Physics.Raycast(ray, out hit, 30f))
            {
                GameObject boom = Instantiate(BoomPrefab, firePoint.position, Quaternion.identity);
                boom.GetComponent<BoomParabola>().BoomInitialize(hit, firePoint, boomType);
            }
        }
    }
}
