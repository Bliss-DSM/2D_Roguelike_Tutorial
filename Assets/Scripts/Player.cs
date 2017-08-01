using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    // Animator 컴포넌트를 할당할 변수
    private Animator animator;
    // food 포인트를 저장할 변수
    private int food;

    // MovingObject의 Start를 재정의
    protected override void Start()
    {
        // Animator 컴포넌트 할당
        animator = GetComponent<Animator>();

        // GameManager에서 Food 포인트 값을 가져옴
        food = GameManager.instance.playerFoodPoints;

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
        if (!GameManager.instance.playersTurn)
        {
            return;
        }

        // 수평 방향의 입력을 저장하는 변수
        int horizontal = 0;
        // 수직 방향의 입력을 저장하는 변수
        int vertical = 0;

        // 키보드의 왼쪽, 오른쪽 방향키가 눌렸는지를 검사해서 horizontal에 대입
        horizontal = (int)Input.GetAxisRaw("Horizontal");
        // 키보드의 위, 아래 방향키가 눌렸는지를 검사해서 vertical에 대입
        vertical = (int)Input.GetAxisRaw("Vertical");

        // 대각선으로 이동하지 못하도록 설정
        if (horizontal != 0)
        {
            vertical = 0;
        }

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

        // MovingObject의 AttemptMove 실행
        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;

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
        if (other.tag == "Food")
        {
            // pointsPerFood만큼 food 포인트 증가
            food += pointsPerFood;
            // Player가 먹은 Food 비활성화
            other.gameObject.SetActive(false);
        }

        //Soda를 먹었을 때
        if (other.tag == "Soda")
        {
            // pointsPerSoda만큼 food 포인트 증가
            food += pointsPerSoda;
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
        // Player의 애니메이션 상태를 PlayerChop으로 변경
        animator.SetTrigger("PlayerChop");
    }

    // Scene 다시 시작  
    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Enemy와 부딪혔을 때 실행되는 메서드
    public void LoseFood(int loss)
    {
        // Player의 애니메이션 상태를 PlayerHit으로 변경
        animator.SetTrigger("playerHit");
        // loss만큼 food 포인트 감소
        food -= loss;
        // food 포인트가 줄어들었으므로 게임 오버가 되었는지 체크
        CheckIfGameOver();
    }

    // 게임오버되었는지 체크하는 메서드
    private void CheckIfGameOver()
    {
        // food 포인트가 0 이하이면
        if(food <= 0)
        {
            // GameManager의 GameOver 메서드 호출
            GameManager.instance.GameOver();
        }
    }
}
