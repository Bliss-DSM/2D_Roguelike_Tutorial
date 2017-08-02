using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    // 플레이어가 벽을 때리는 데미지
    public int wallDamage = 1;
    // Food를 먹을 때 오르는 포인트
    public int pointsPerFood = 10;
    // Soda를 먹을 때 오르는 포인트
    public int pointsPerSoda = 20;
    // 레벨을 다시 시작할 때 주는 딜레이
    public float restartLevelDelay = 1f;
    // Player의 food 포인트를 표시할 텍스트
    public Text foodText;
    // 움직일 때 재생되는 효과음
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    // food를 습득했을 때 재생되는 효과음
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    // soda를 습득했을 때 재생되는 효과음
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    // 게임오버가 되었을 때 재생되는 효과음
    public AudioClip gameOverSound;

    // Animator 컴포넌트를 할당할 변수
    private Animator animator;
    // food 포인트를 저장할 변수
    private int food;
    // 터치가 되었는지를 검사하는 변수
    private Vector2 touchOrigin = -Vector2.one;

    // MovingObject의 Start를 재정의
    protected override void Start()
    {
        // Animator 컴포넌트 할당
        animator = GetComponent<Animator>();

        // GameManager에서 Food 포인트 값을 가져옴
        food = GameManager.instance.playerFoodPoints;

        // food 포인트 갱신
        foodText.text = "Food: " + food;

        // MovingObject의 Start 실행
        base.Start();
    }

    // Player가 비활성화되었을 때 호출
    private void OnDisable()
    {
        // 현재 food 포인트를 GameManager의 playerFoodPoints 변수에 전달
        GameManager.instance.playerFoodPoints = food;
    }

    // Player의 턴이면 사용자로부터 입력을 받아서 움직임
    void Update()
    {
        // playersTurn이 false면 Update 종료
        if (GameManager.instance.playersTurn == false)
        {
            return;
        }

        // 수평 방향의 입력을 저장하는 변수
        int horizontal = 0;
        // 수직 방향의 입력을 저장하는 변수
        int vertical = 0;
        
#if UNITY_STANDALONE || UNITY_WEBPLAYER

        // 키보드의 왼쪽, 오른쪽 방향키가 눌렸는지를 검사해서 horizontal에 대입
        horizontal = (int)Input.GetAxisRaw("Horizontal");
        // 키보드의 위, 아래 방향키가 눌렸는지를 검사해서 vertical에 대입
        vertical = (int)Input.GetAxisRaw("Vertical");

        // 대각선으로 이동하지 못하도록 설정
        if (horizontal != 0)
        {
            vertical = 0;
        }

#else
        // 터치가 일어났을 때
        if(Input.touchCount > 0)
        {
            // 터치 지점을 저장
            Touch myTouch = Input.touches[0];

            // 터치가 시작되면
            if(myTouch.phase == TouchPhase.Began)
            {
                // touchOrigin에 위치 저장
                touchOrigin = myTouch.position;
            }

            // 터치가 끝나면
            else if(myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                // touchEnd에 위치 저장
                Vector2 touchEnd = myTouch.position;
                // 어느 방향으로 스와이프했는지 계산
                float x = touchEnd.x - touchOrigin.x;
                float y = touchEnd.y - touchOrigin.y;
                // touchOrigin 초기화
                touchOrigin.x = -1;
                // x축 방향으로 스와이프했다면
                if(Mathf.Abs(x) > Mathf.Abs(y))
                {
                    // 오른쪽으로 스와이프했다면
                    if(x > 0)
                    {
                        // 오른쪽 방향으로 움직이도록 설정
                        horizontal = 1;
                    }
                    // 왼쪽으로 스와이프했다면
                    else
                    {
                        // 왼쪽 방향으로 움직이도록 설정
                        horizontal = -1;
                    }
                }
                // y축 방향으로 스와이프했다면
                else
                {
                    // 위쪽으로 스와이프했다면
                    if(y > 0)
                    {
                        // 위쪽 방향으로 움직이도록 설정
                        vertical = 1;
                    }
                    // 아래쪽으로 스와이프했다면
                    else
                    {
                        // 아래쪽 방향으로 움직이도록 설정
                        vertical = -1;
                    }
                }
            }
        }

#endif

        // 입력이 있다면
        if (horizontal != 0 || vertical != 0)
        {
            // 입력된 방향으로 이동 - Wall과 충돌할 가능성 있음
            AttemptMove<Wall>(horizontal, vertical);
        }
    }

    // MovingObject의 AttemptMove를 재정의
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        // 이동하면 food가 1 줄어듦
        food--;
        // food 포인트 갱신
        foodText.text = "Food: " + food;

        // MovingObject의 AttemptMove 실행
        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;
        //만약 움직이는 데 성공했다면
        if(Move(xDir, yDir, out hit))
        {
            // moveSound 재생
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        // food 포인트가 줄어들었으므로 게임 오버가 되었는지 체크
        CheckIfGameOver();

        // Player가 움직였으므로 Player의 턴 종료
        GameManager.instance.playersTurn = false;
    }

    // Player가 Trigger에 들어왔을 때 호출되는 메서드
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 출구로 들어왔을 때
        if (other.tag == "Exit")
        {
            // restartLevelDelay만큼 정지한 후에 Restart 메서드 호출
            Invoke("Restart", restartLevelDelay);
            // Player 비활성화
            enabled = false;
        }

        // Food를 먹었을 때
        else if (other.tag == "Food")
        {
            // pointsPerFood만큼 food 포인트 증가
            food += pointsPerFood;
            // food 포인트가 증가되었음을 알림
            foodText.text = "+" + pointsPerFood + " Food: " + food;
            // eatSound 재생
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            // Player가 먹은 Food 비활성화
            other.gameObject.SetActive(false);
        }

        //Soda를 먹었을 때
        else if (other.tag == "Soda")
        {
            // pointsPerSoda만큼 food 포인트 증가
            food += pointsPerSoda;
            // food 포인트가 증가되었음을 알림
            foodText.text = "+" + pointsPerSoda + " Food: " + food;
            // drinkSound 재생
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            // Player가 먹은 Soda 비활성화
            other.gameObject.SetActive(false);
        }
    }

    // MovingObject의 OnCantMove를 재정의
    protected override void OnCantMove<T>(T component)
    {
        // 부딪힌 벽을 hitWall에 대입
        Wall hitWall = component as Wall;
        // Wall의 DamageWall 메서드 호출
        hitWall.DamageWall(wallDamage);
        // Player의 애니메이션 상태를 playerChop으로 변경
        animator.SetTrigger("playerChop");
    }

    // Scene 다시 시작  
    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    // Enemy와 부딪혔을 때 실행되는 메서드
    public void LoseFood(int loss)
    {
        // Player의 애니메이션 상태를 playerHit으로 변경
        animator.SetTrigger("playerHit");
        // loss만큼 food 포인트 감소
        food -= loss;
        // food 포인트가 감소되었음을 알림
        foodText.text = "-" + loss + " Food: " + food;
        // food 포인트가 줄어들었으므로 게임 오버가 되었는지 체크
        CheckIfGameOver();
    }

    // 게임오버되었는지 체크하는 메서드
    private void CheckIfGameOver()
    {
        // food 포인트가 0 이하이면
        if(food <= 0)
        {
            // gameOverSound 재생
            SoundManager.instance.PlaySingle(gameOverSound);
            // 배경음악 재생 정지
            SoundManager.instance.musicSource.Stop();
            // GameManager의 GameOver 메서드 호출
            GameManager.instance.GameOver();
        }
    }
}
