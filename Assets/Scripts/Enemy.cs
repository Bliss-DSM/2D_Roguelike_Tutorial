using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    // Player와 충돌했을 때 Player가 잃을 food 포인트 값
    public int playerDamage;

    // Animator 컴포넌트를 할당할 변수
    private Animator animator;
    // Player의 위치를 저장할 변수
    private Transform target;
    // Enemy가 두 턴 당 한 번씩 이동하도록 함
    private bool skipMove;

    // MovingObject의 Start 재정의
    protected override void Start()
    {
        // Animator 컴포넌트 할당
        animator = GetComponent<Animator>();
        // target 변수에 Player의 위치 저장
        target = GameObject.FindGameObjectWithTag("Player").transform;
        // MovingObject의 Start 실행
        base.Start();
    }

    // MovingObject의 AttemptMove 재정의
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        // Enemy가 두 턴 당 한 번씩 이동하도록 하는 코드
        if(skipMove)
        {
            skipMove = false;
            return;
        }

        // MovingObject의 AttemptMove 실행
        base.AttemptMove<T>(xDir, yDir);

        skipMove = true;
    }

    // Enemy를 움직이는 코드
    public void MoveEnemy()
    {
        // 수평 방향
        int xDir = 0;
        // 수직 방향
        int yDir = 0;

        // Player쪽으로 움직이도록 하는 코드
        // Player와 Enemy의 x값이 같으면
        if(Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            // Player의 y값이 Enemy의 y값보다 크면
            if(target.position.y > transform.position.y)
            {
                // 위쪽 방향으로 움직이도록 설정
                yDir = 1;
            }
            // 아니면
            else
            {
                // 아래쪽 방향으로 움직이도록 설정
                yDir = -1;
            }
        }
        // 아니면
        else
        {
            // Player의 y값이 Enemy의 y값보다 크면
            if(target.position.x > transform.position.x)
            {
                // 오른쪽 방향으로 움직이도록 설정
                xDir = 1;
            }
            // 아니면
            else
            {
                // 왼쪽 방향으로 움직이도록 설정
                yDir = -1;
            }
        }

        // Player의 방향으로 이동 - Player와 충돌할 가능성 있음
        AttemptMove<Player>(xDir, yDir);
    }

    // MovingObject의 OnCantMove를 재정의
    protected override void OnCantMove<T>(T component)
    {
        // 부딪힌 Player을 hitPlayer에 대입
        Player hitPlayer = component as Player;

        // Player의 LoseFood 메서드 호출
        hitPlayer.LoseFood(playerDamage);
    }
}
