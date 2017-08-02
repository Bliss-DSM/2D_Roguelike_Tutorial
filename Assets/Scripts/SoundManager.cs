using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // 효과음을 저장하는 변수
    public AudioSource efxSource;
    // 배경음을 저장하는 변수
    public AudioSource musicSource;
    // 어디서나 접근할 수 있는 SoundManager 변수 초기화
    public static SoundManager instance = null;

    // 낮은 피치
    public float lowPitchRange = 0.95f;
    // 높은 피치
    public float highPitchRange = 1.05f;

    private void Awake()
    {
        // GameManager 인스턴스가 할당되지 않았다면
        if (instance == null)
        {
            // 자기 자신을 할당
            instance = this;
        }
        // GameManager 인스턴스가 null도 아니고 자신도 아니라면
        else if (instance != this)
        {
            // 자신이 연결된 gameObject 파괴
            Destroy(gameObject);
        }

        // Scene이 변경되더라도 자신이 연결된 gameObject는 파괴되지 않도록 설정
        DontDestroyOnLoad(gameObject);
    }

    // 매개변수로 받은 AudioClip 재생
    public void PlaySingle(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();
    }

    // 매개변수로 받은 AudioClip들 중에서 하나 랜덤으로 재생
    // 피치도 랜덤으로 재생
    public void RandomizeSfx(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        efxSource.pitch = randomPitch;
        efxSource.clip = clips[randomIndex];
        efxSource.Play();
    }
}
