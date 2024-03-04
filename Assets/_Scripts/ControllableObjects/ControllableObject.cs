using System;
using UnityEngine;

public abstract class ControllableObject : MonoBehaviour{
    [Header("Controllable Object Variables")]
    [SerializeField] protected bool isActivated;

    [Header("Events")]
    public EventHandler OnActivate;
    public EventHandler OnDeactivate;

    public abstract void Activate();
    public abstract void Deactivate();
}
