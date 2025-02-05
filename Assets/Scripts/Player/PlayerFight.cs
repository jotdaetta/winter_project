using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerFight : MonoBehaviour
{
    [SerializeField] LayerMask WallLayer;
    List<GameObject> enemies = null;
    SpriteRenderer targetRenderer = null;
    int index;
    bool LockedOn;
    private void Update()
    {
        if (LockedOn && targetRenderer == null && enemies != null)
            ChangeLockOn();
        CheckWallBang();
    }
    void CheckWallBang()
    {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, targetRenderer.transform.position, WallLayer);
        if (hit.collider != null)
        {
            ChangeLockOn();
        }
    }
    public void LockOn()
    {
        if (LockedOn)
        {
            LockedOn = false;
            SetRenderer(true);
            return;
        }
        LockedOn = true;
        enemies = GameObject.FindGameObjectsWithTag("enemy").ToList();
        index = FindMinDistEnemy();
        SetRenderer();
    }
    public void ChangeLockOn()
    {
        if (enemies == null || !LockedOn) return;
        enemies.RemoveAt(index);
        if (enemies.Count == 0)
            enemies = GameObject.FindGameObjectsWithTag("enemy").ToList();
        index = FindMinDistEnemy();
        SetRenderer();
    }
    void SetRenderer(bool off = false)
    {
        if (targetRenderer != null)
        {
            targetRenderer.color = Color.black;
            if (off) return;
        }
        targetRenderer = enemies[index].transform.GetChild(0).GetComponent<SpriteRenderer>();
        targetRenderer.color = Color.white;
    }
    int FindMinDistEnemy()
    {
        float min = 0;
        int index = 0;

        for (int i = 0; i < enemies.Count; i++)
        {
            float dist = Vector2.Distance(transform.position, enemies[i].transform.position);
            if (i == 0) min = dist;
            else if (min > dist)
            {
                min = dist;
                index = i;
            }
        }
        return index;
    }
}
