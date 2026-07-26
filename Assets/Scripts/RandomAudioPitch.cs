using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomAudioPitch : MonoBehaviour
{
    public float range = 0.01f;
    private void Awake()
    {
        // this helps with the volume when the same clip plays multiple times and the volume is increased massively
        GetComponent<AudioSource>().pitch *= Random.Range(1f - range, 1 + range);
    }
}
