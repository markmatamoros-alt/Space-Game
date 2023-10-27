using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIReadyButtonReload : MonoBehaviour
{
    public UIPlayerStart uIPlayerStart;
    void OnEnable()
    {
        uIPlayerStart.reload();
        
    }


}
