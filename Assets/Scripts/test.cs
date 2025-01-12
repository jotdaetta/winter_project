using System.Collections;
using UnityEngine;

public class test : MonoBehaviour
{
    void Start()
    {
        CamManager.main.Offset(new(10, 0), 0.4f);
    }

    IEnumerator st() {
        CamManager.main.CloseUp(3, 10, 0.3f);

        yield return new WaitForSeconds(0.4f);

        CamManager.main.Shake(5);

        yield return new WaitForSeconds(0.4f);

        CamManager.main.CloseOut(0.4f);
    }

    void Update()
    {
        
    }
}
