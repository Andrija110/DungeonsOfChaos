using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainAreaStartup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Game.Instance.UpdatePortals();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
