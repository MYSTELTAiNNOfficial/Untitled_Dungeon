using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShootingAction : MonoBehaviour
{
    public UnityEvent action;

    public void Action()
    {
        action?.Invoke();
    }
}
