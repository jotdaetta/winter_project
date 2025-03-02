using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Glass : MonoBehaviour, IDamageable
{
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer myRenderer;
    [SerializeField] float glassDisappearTime = 0.5f;
    Transform playerTransform;
    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
    }
    public bool TakeDamage(int damage, bool isknife = false)
    {
        print("A");
        GlassBreak();
        return false;
    }
    void GlassBreak()
    {
        if (playerTransform.position.x > transform.position.x)
            myRenderer.flipY = true;
        gameObject.layer = 0;
        animator.SetTrigger("break");
        StartCoroutine(Disappearing());
    }

    IEnumerator Disappearing()
    {
        float elapsed = 0f;

        while (elapsed < glassDisappearTime)
        {
            elapsed += Time.deltaTime;
            myRenderer.color = new Color(1, 1, 1, Mathf.Lerp(1, 0f, elapsed / glassDisappearTime));
            yield return null;
        }

        Destroy(gameObject);
    }
}
