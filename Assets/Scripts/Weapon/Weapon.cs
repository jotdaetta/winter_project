using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected int weaponDamage;
    [SerializeField] protected float maxDistance;
    [SerializeField] protected LayerMask detectLayer;
}
