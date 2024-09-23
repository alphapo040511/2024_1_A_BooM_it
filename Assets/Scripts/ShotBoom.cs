using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class ShotBoom : MonoBehaviour
{
    public GameObject BoomPrefab;

    public BoomType boomType;

    public Transform firePoint;

    public float rayDistance = 30f;

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
            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                GameObject boom = Instantiate(BoomPrefab, firePoint.position, Quaternion.identity);
                boom.GetComponent<BoomParabola>().BoomInitialize(hit.point, hit.collider.GetComponent<BlockData>().intPosition, firePoint, boomType);
            }
            else
            {
                GameObject boom = Instantiate(BoomPrefab, firePoint.position, Quaternion.identity);
                boom.GetComponent<BoomParabola>().BoomInitialize(transform.forward * rayDistance, new Vector3Int(-1,-1,-1), firePoint, boomType);
            }
        }
    }
}
