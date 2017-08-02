using UnityEngine;

public class Wall : MonoBehaviour
{
    // 데미지를 입으면 바뀌는 Sprite
    public Sprite dmgSprite;
    // Wall의 체력
    public int hp = 4;
    // 벽이 데미지를 입었을 때 재생되는 효과음
    public AudioClip chopSound1;
    public AudioClip chopSound2;

    // SpriteRenderer 컴포넌트를 할당할 변수
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        // SpriteRenderer 컴포넌트 할당
        spriteRenderer = GetComponent<SpriteRenderer>();    
    }

    // Wall이 데미지를 입었을 때
    public void DamageWall(int loss)
    {
        // chopSound 재생
        SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);
        // Wall의 Sprite를 dmgSprite로 변경
        spriteRenderer.sprite = dmgSprite;
        // Wall의 hp가 loss만큼 감소
        hp -= loss;
        
        // Wall의 hp가 다 떨어지면
        if(hp <= 0)
        {
            // Wall 비활성화
            gameObject.SetActive(false);
        }
    }
}
