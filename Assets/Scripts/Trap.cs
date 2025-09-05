using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{

    public GameObject EnemyPrefab;
    public Vector3 EnemyPosition;

    public bool Active { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateTrap()
    {
        if (EnemyPrefab != null && EnemyPosition != null && Active)
        {
            Instantiate(EnemyPrefab, EnemyPosition, Quaternion.identity );
            Active = false;
        }
    }
}
