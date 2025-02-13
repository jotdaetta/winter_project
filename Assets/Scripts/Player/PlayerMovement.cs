using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 7f;
    public float dashSpeed = 25f;
    public float dashTime = 0.3f;
    public float dashCool = 3f;
    public bool dashable = true;
    delegate void MyFunc();
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer spriteRenderer;
    bool onDash;
    Vector2 moveVector = new();


    float x = 0, y = 0;
    public void Move()
    {
        if (onDash) return;
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        moveVector = new Vector2(x, y).normalized;
        rb.linearVelocity = moveVector * moveSpeed;
    }

    public void Dash()
    {
        if (!dashable || moveVector == Vector2.zero) return;
        onDash = true;
        dashable = false;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(moveVector * dashSpeed, ForceMode2D.Impulse);
        StartCoroutine(Mujuk(dashTime + 0.1f));
        StartCoroutine(Cooltime(dashTime, dashCool, () => { onDash = false; rb.linearVelocity = Vector2.zero; }, () => dashable = true));
    }
    IEnumerator Mujuk(float cool)
    {
        gameObject.layer = 8;
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        yield return new WaitForSeconds(cool);
        gameObject.layer = 3;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }
    IEnumerator Cooltime(float duration, float cool, MyFunc func0, MyFunc func1)
    {
        yield return new WaitForSeconds(dashTime);
        func0();
        yield return new WaitForSeconds(dashCool);
        func1();
    }
}
