using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateTween : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collider){
        DoTweenBasic tween = collider.gameObject.GetComponent<DoTweenBasic>();
        if(tween != null){
            tween.DoTween();
        }
    }
}
