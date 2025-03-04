using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CamManager : MonoBehaviour
{
    public static CamManager main;
    public CinemachineCamera cam;
    public CinemachineBasicMultiChannelPerlin noise;
    public CinemachineCameraOffset camOffset;
    float orSize_d;
    float dutch_d;
    Vector3 spectatorCam = new Vector3(-55.28f, -17.2f, -2f);

    IEnumerator dutchRoutine = null;
    IEnumerator offRoutine = null;

    Coroutine shaking = null;

    float frame = 60;

    public bool autoSpector;

    private void Awake()
    {
        cam = GetComponent<CinemachineCamera>();
        camOffset = GetComponent<CinemachineCameraOffset>();
        noise = GetComponent<CinemachineBasicMultiChannelPerlin>();

        main = this;

        orSize_d = cam.Lens.OrthographicSize;
        dutch_d = cam.Lens.Dutch;
    }

    void ClearRoutine(ref IEnumerator routine)
    {
        if (routine != null)
        {
            StopCoroutine(routine);

            routine = null;
        }
    }

    public void CloseUp(float orSize, float dutch, float dur = 0)
    {
        ClearRoutine(ref dutchRoutine);
        dutchRoutine = _closeUp(orSize, dutch, dur);

        StartCoroutine(dutchRoutine);
    }
    public void CloseOut(float dur = 0)
    {
        ClearRoutine(ref dutchRoutine);
        dutchRoutine = _closeOut(dur);

        StartCoroutine(dutchRoutine);
    }
    public void Offset(Vector2 off, float dur = 0)
    {
        ClearRoutine(ref offRoutine);

        offRoutine = _offset(off, dur);

        StartCoroutine(offRoutine);
    }

    public void Shake(float strength = 1, float dur = 0.05f)
    {
        shaking = StartCoroutine(_shake(strength, dur));
    }

    public void StopShake()
    {
        if (shaking == null) return;

        StopCoroutine(shaking);

        noise.AmplitudeGain = 0;
        noise.FrequencyGain = 0;
    }

    IEnumerator _closeUp(float orSize, float dutch, float dur)
    {
        if (dur > 0)
        {
            float dSize = cam.Lens.OrthographicSize, dDutch = cam.Lens.Dutch;

            for (int i = 1; i <= frame; i++)
            {
                cam.Lens.OrthographicSize = dSize - (dSize - orSize) / frame * i;
                cam.Lens.Dutch = dDutch - (dDutch - dutch) / frame * i;

                yield return new WaitForSeconds(dur / frame);
            }
        }

        cam.Lens.OrthographicSize = orSize;
        cam.Lens.Dutch = dutch;

        dutchRoutine = null;
    }

    IEnumerator _closeOut(float dur)
    {
        if (dur > 0)
        {
            float dSize = cam.Lens.OrthographicSize, dDutch = cam.Lens.Dutch;

            for (int i = 1; i <= frame; i++)
            {
                cam.Lens.OrthographicSize = dSize + (orSize_d - dSize) / frame * i;
                cam.Lens.Dutch = dDutch + (dutch_d - dDutch) / frame * i;

                yield return new WaitForSeconds(dur / frame);
            }
        }

        cam.Lens.OrthographicSize = orSize_d;
        cam.Lens.Dutch = dutch_d;

        dutchRoutine = null;
    }

    IEnumerator _offset(Vector3 off, float dur = 0)
    {
        if (dur > 0)
        {
            Vector2 beforeOff = camOffset.Offset;

            for (int i = 1; i <= frame; i++)
            {
                camOffset.Offset = new Vector3(
                    beforeOff.x - (beforeOff.x - off.x) / frame * i,
                    beforeOff.y - (beforeOff.y - off.y) / frame * i
                );

                yield return new WaitForSeconds(dur / frame);
            }
        }

        camOffset.Offset = off;

        offRoutine = null;
    }

    IEnumerator _shake(float strength, float dur)
    {
        noise.AmplitudeGain = strength;
        noise.FrequencyGain = strength;

        yield return new WaitForSeconds(dur);

        noise.AmplitudeGain = 0;
        noise.FrequencyGain = 0;
    }
}
