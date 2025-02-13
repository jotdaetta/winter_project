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
    IEnumerator Cooltime(float duration, float cool, MyFunc func0, MyFunc func1)
    {
        yield return new WaitForSeconds(duration);
        func0();
        yield return new WaitForSeconds(cool);
        func1();
    }
}
