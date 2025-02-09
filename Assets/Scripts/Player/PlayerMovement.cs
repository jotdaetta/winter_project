using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 5f;
    public float dashTime = 0.1f;
    public float dashCool = 3f;
    delegate void MyFunc();
    Rigidbody2D rb;
    bool onDash;
    public bool dashable = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    float x = 0, y = 0;
    public void Move()
    {
        if (!onDash)
        {
            x = Input.GetAxisRaw("Horizontal");
            y = Input.GetAxisRaw("Vertical");
        }
        Vector2 moveVector = new Vector2(x, y).normalized;
        float dash = onDash ? dashSpeed : 1;
        rb.linearVelocity = moveVector * moveSpeed * dash;
    }

    public void Dash()
    {
        if (!dashable || x == 0 && y == 0) return;
        onDash = true;
        dashable = false;
        StartCoroutine(Cooltime(dashTime, dashCool, () => onDash = false, () => dashable = true));
    }
    IEnumerator Cooltime(float duration, float cool, MyFunc func0, MyFunc func1)
    {
        yield return new WaitForSeconds(dashTime);
        func0();
        yield return new WaitForSeconds(dashCool);
        func1();
    }
}
