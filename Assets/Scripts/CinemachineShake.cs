using System.Collections;
using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CinemachineShake : MonoBehaviour
{
    private CinemachineBasicMultiChannelPerlin _cinemachine;

    public void Shake() => StartCoroutine(ShakeAnimation());

    private void Start() => _cinemachine =
        GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

    private IEnumerator ShakeAnimation()
    {
        _cinemachine.m_AmplitudeGain = 1f;

        yield return new WaitForSeconds(.2f);

        _cinemachine.m_AmplitudeGain = 0;
    }
}