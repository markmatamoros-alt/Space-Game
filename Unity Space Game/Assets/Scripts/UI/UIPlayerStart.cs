using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerStart : MonoBehaviour
{
    public Animator startBtReady;
    public PlayerSceneController playerSceneController;
    bool ready;

    public void reload()
    {
        if(playerSceneController.thisDevice!=null)
        ready = playerSceneController.thisDevice.ready;
        Debug.Log("reload ready: " + ready);

        startBtReady.SetBool("ready", ready);

    }
    public void ChangeReady()
    {

        /*
        if (!ready)
        {
            startBtReady.SetTrigger(ready,true);
            ready = true;
        }
        else
        {
            startBtReady.SetTrigger("unready");
            ready = false;
        }
        */
        if (ready == false)
        {
            ready = !ready;
            Debug.Log("ChangeReady to " + ready);
            startBtReady.SetBool("ready", ready);

            playerSceneController.SetReady(ready);
        }
    }

    public void ResetReady()
    {

        ready = false;
        Debug.Log("ResetReady to " + ready);

        startBtReady.SetBool("ready", false);
    }
}
