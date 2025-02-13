using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerFight : MonoBehaviour
{
    [SerializeField] LayerMask WallLayer;
    [SerializeField] List<GameObject> enemies = new();
    SpriteRenderer targetRenderer = null;
    int index, renderStack = 0;
    bool lockedOn;
    float findingInterval;
    private void Update()
    {
        CheckTargetChangeCondition();
    }
    public void LockOn()
    {
        if (lockedOn)
        {
            lockedOn = false;
            SetRenderer(true);
            return;
        }
        enemies = new();
        renderStack = 3;
        lockedOn = true;
        SetLockOn();
    }
    public void ChangeLockOn()
    {
        if (index < enemies.Count)
            enemies[index] = null;
        SetLockOn();
    }
    bool CheckWallBang(Vector2 start_pos, Vector2 target_pos)
    {
        return Physics2D.Linecast(start_pos, target_pos, WallLayer).collider != null;
    }
    List<GameObject> DeleteBeyondWallEnemy(GameObject[] enemies)
    {
        for (int i = enemies.Length - 1; i >= 0; i--)
        {
            if (CheckWallBang(transform.position, enemies[i].transform.position))
                enemies[i] = null;
        }
        return enemies.Where(e => e != null).ToList();
    }
    void SetLockOn()
    {
        enemies = enemies.Where(e => e != null).ToList();
        if (enemies.Count == 0)
            enemies = DeleteBeyondWallEnemy(GameObject.FindGameObjectsWithTag("enemy"));
        if (enemies.Count == 0) return;
        index = FindMinDistEnemy();
        if (index == -1)
        {
            SetRenderer(true);
            return;
        }
        SetRenderer();
    }
    void SetRenderer(bool off = false)
    {
        if (targetRenderer != null)
        {
            targetRenderer.color = Color.black;
            if (off) return;
        }
        if (index >= 0 && index < enemies.Count)
        {
            SpriteRenderer renderer = enemies[index].transform.GetChild(0).GetComponent<SpriteRenderer>();
            print(renderStack);
            if (targetRenderer != null && renderer == targetRenderer && renderStack < 3)
            {
                renderStack++;
                ChangeLockOn();
                return;
            }
            renderStack = 0;
            targetRenderer = renderer;
            targetRenderer.color = Color.white;
        }
    }
    int FindMinDistEnemy()
    {
        float min = float.MaxValue;
        int index = 0;
        for (int i = 0; i < enemies.Count; i++)
        {
            float dist = Vector2.Distance(transform.position, enemies[i].transform.position);
            if (min > dist)
            {
                min = dist;
                index = i;
            }
        }
        return index;
    }
    void CheckTargetChangeCondition()
    {
        if (!lockedOn) return;
        // enemy zero
        if (enemies.Count == 0)
        {
            SetRenderer(true);
            EnemyFindingCondition();
        }
        //enemy dead
        if (targetRenderer == null)
        {
            SetLockOn();
            return;
        }
        // wallbang
        if (CheckWallBang(transform.position, targetRenderer.transform.position))
        {
            ChangeLockOn();
        }
    }
    void EnemyFindingCondition()
    {
        findingInterval += Time.deltaTime;
        if (findingInterval > 0.2f)
        {
            findingInterval = 0;
            SetLockOn();
        }
    }
}
