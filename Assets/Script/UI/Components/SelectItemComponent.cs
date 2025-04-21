using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectItemComponent : MonoBehaviour
{

    private int SelectItemIdx = 0;

    public void Set(int selectitemidx)
    {
        SelectItemIdx = selectitemidx; 

    }
}
