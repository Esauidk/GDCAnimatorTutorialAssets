using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoTweenBasic : MonoBehaviour
{

    public virtual void InitTween(){
        Debug.Log("Tween Inited");
    }
    public virtual void DoTween(){
        Debug.Log("Tween Performed");
    }

    // Start is called before the first frame update
    void Start()
    {
        InitTween();
    }

    
}
