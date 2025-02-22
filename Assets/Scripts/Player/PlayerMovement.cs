using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeedRecoverSpeed = 0.5f;
    [SerializeField] float moveSlow = 0.5f;
    [SerializeField] float moveSpeed = 7f;
    [SerializeField] float dashSpeed = 25f;
    [SerializeField] float dashTime = 0.3f;
    [SerializeField] float dashCool = 3f;
    [SerializeField] bool dashable = true;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer spriteRenderer;
    delegate void MyFunc();
    float moveSpeedMul = 1;
    bool onDash;
    Vector2 moveVector = new();


    float x = 0, y = 0;
    bool locked;
    Vector2 dirVector;
    public void Move()
    {
        if (onDash) return;
        x = Input.GetAxisRaw("Horizontal"); // 이거 키보드기준 getaxis면 조작감 구린데 패드는 모르겠음 ==========================================
        y = Input.GetAxisRaw("Vertical");
        moveVector = new Vector2(x, y).normalized;
        rb.linearVelocity = moveVector * moveSpeed * moveSpeedMul;
        if (!(x == 0 && y == 0))
        {
            if (!locked && lastLocked) lastLocked = false;
            dirVector = new Vector2(x, y);
        }
    }

    bool onFighting;
    float exitFightTime;

    public void Slow(bool on = true)
    {
        if (on)
        {
            exitFightTime = moveSpeedRecoverSpeed;
            onFighting = true;
            moveSpeedMul = moveSlow;
            return;
        }
    }

    void Update()
    {
        if (onFighting)
        {
            exitFightTime -= Time.deltaTime;
            if (exitFightTime <= 0)
            {
                onFighting = false;
                moveSpeedMul = 1;
            }
        }
    }

    public bool isMoving { get { return new Vector2(x, y) != Vector2.zero; } }

    bool lastLocked;
    public void Turn(bool locked, Vector2 targetpos)
    {
        this.locked = locked;
        Vector2 direction;
        if (locked)
        {
            lastLocked = true;
            direction = targetpos - (Vector2)transform.position;
        }
        else
        {
            if (lastLocked) return;
            direction = dirVector;
        }
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), Time.deltaTime * 7);
    }

    public void Dash()
    {
        if (!dashable || moveVector == Vector2.zero) return;
        onDash = true;
        dashable = false;
        rb.linearVelocity = Vector2.zero;
        Mujuk();
        rb.AddForce(moveVector * dashSpeed, ForceMode2D.Impulse);
        StartCoroutine(Cooltime(dashTime, dashCool, () =>
        {
            onDash = false;
            Mujuk(true);
        }, () => dashable = true));
    }

    void Mujuk(bool off = false)
    {
        gameObject.layer = off ? 3 : 8;
        spriteRenderer.color = new Color(1, 1, 1, off ? 1 : 0.4f);
    }

    IEnumerator Cooltime(float duration, float cool, MyFunc func0, MyFunc func1 = null)
    {
        yield return new WaitForSeconds(duration);
        func0();
        yield return new WaitForSeconds(cool);
        func1();
    }
}
