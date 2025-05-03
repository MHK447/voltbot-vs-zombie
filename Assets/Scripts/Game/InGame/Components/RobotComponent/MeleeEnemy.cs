using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : RobotBase
{
    public override void SetInfo()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        pos.x -= 5f * Time.deltaTime;
        transform.position = pos;
    }
}
