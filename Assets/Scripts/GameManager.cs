using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 어디서나 접근할 수 있는 GameManager 변수 초기화
    public static GameManager instance = null;
    // BoardManager 변수
    public BoardManager boardScript;
    // player의 food 포인트 초기값
    public int playerFoodPoints = 100;
    // public 변수지만 Inspector에서 숨김
    // Player의 턴인지를 검사하는 데 쓰이는 변수
    [HideInInspector] public bool playersTurn = true;

    // level 변수 초기화
    private int level = 3;

    void Awake()
    {
        // GameManager 인스턴스가 할당되지 않았다면 자신을 할당
        if (instance == null) instance = this;
        // GameManager 인스턴스가 null도 아니고 자신도 아니라면 자신이 연결된 gameObject를 파괴시킴
        else if (instance != this) Destroy(gameObject);

        // Scene이 변경되더라도 자신이 연결된 gameObject는 파괴되지 않도록 설정
        DontDestroyOnLoad(gameObject);
        // BoardManager 컴포넌트 할당
        boardScript = GetComponent<BoardManager>();
        // 게임 초기화 메서드 실행
        InitGame();
    }

    // boardScript의 SetupScene을 호출하는 메서드
    void InitGame()
    {
        // level에 따라 Scene을 그림
        boardScript.SetupScene(level);
    }

    // 게임 오버되었을 때 GameManager를 비활성화시키는 메서드
    public void GameOver()
    {
        // GameManager의 Update를 중지
        enabled = false;
    }
}
