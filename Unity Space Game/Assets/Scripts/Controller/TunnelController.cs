using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TunnelEffect;
public class TunnelController : MonoBehaviour
{
    public TunnelFX2 tunnel;
    public float targetSpeed, targetAmplitude, targetHyperspace, targetFallOffEnd;
    public Color targetTint, targetBackColor, tintGame,tintVictory, backGame,backVictory;
    float velspeed, velamplitude, velhyperspace, velFalloff;

    void Start(){
        StartSequence();
    }
    void Update()
    {
        tunnel.animationAmplitude = Mathf.SmoothDamp(tunnel.animationAmplitude, targetAmplitude, ref velamplitude, 0.1f);
        tunnel.hyperSpeed=Mathf.SmoothDamp(tunnel.hyperSpeed,targetHyperspace,ref velhyperspace, 0.1f);
        // tunnel.SetTravelSpeed(0,smoothSpeed);
        //tunnel.SetTravelSpeed(1,smoothSpeed);
        // tunnel.SetTravelSpeed(2,smoothSpeed);
        // tunnel.SetTravelSpeed(3,smoothSpeed);
        tunnel.layersSpeed = Mathf.SmoothDamp(tunnel.layersSpeed, targetSpeed, ref velspeed, 0.1f);
        tunnel.fallOff = Mathf.SmoothDamp(tunnel.fallOff, targetFallOffEnd, ref velFalloff, 1.6f);
        float step=0.05f;
        tunnel.tintColor=Color.Lerp(tunnel.tintColor,targetTint,step);
        tunnel.backgroundColor=Color.Lerp(tunnel.backgroundColor,targetBackColor,step);
    }

    public void EndSequence(){
        targetTint=tintVictory;
        targetBackColor=backVictory;
        targetFallOffEnd=0.001f;
        targetHyperspace=1;
    }

    public void StartSequence(){
        targetTint=tintGame;
        targetBackColor=backGame;
        targetFallOffEnd=1;
        targetHyperspace=0.8f;

    }

    public void SetSpeed(float value)
    {
        targetSpeed = Tools.map(value, 0, 1, 0.1f, 10f);
        //targetHyperspace=Tools.map(value,0,1,0.4f,0.85f);
        //targetAmplitude=Tools.map(value,0,1,0.10f,0.001f);

    }
}
