using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public bool TakeDamage(int damage, bool isknife = false);
}
