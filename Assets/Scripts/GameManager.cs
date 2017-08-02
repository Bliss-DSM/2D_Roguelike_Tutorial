using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // 레벨 사이에 게임이 대기하는 시간
    public float levelStartDelay = 2f;
    // 이동 사이에 게임이 대기하는 시간
    public float turnDelay = 0.1f;
    // 어디서나 접근할 수 있는 GameManager 변수 초기화
    public static GameManager instance = null;
    // BoardManager 변수
    public BoardManager boardScript;
    // player의 food 포인트 초기값
    public int playerFoodPoints = 100;
    // public 변수지만 Inspector에서 숨김
    // Player의 턴인지를 검사하는 데 쓰이는 변수
    [HideInInspector] public bool playersTurn;

    // 현재 레벨 숫자를 표시할 텍스트
    private Text levelText;
    // 현재 레벨 숫자를 표시할 때 배경이 되는 검은색 직사각형 이미지
    private GameObject levelImage;
    // level 변수 초기화
    private int level = 1;
    // Board에 존재하는 Enemy들의 List
    private List<Enemy> enemies;
    // Enemy가 움직이고 있는지를 체크하는 변수
    private bool enemiesMoving;
    // Level이 그려지고 있는지를 체크하는 변수
    private bool doingSetup;

    void Awake()
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
        // enemies에 List 대입
        enemies = new List<Enemy>();
        // BoardManager 컴포넌트 할당
        boardScript = GetComponent<BoardManager>();
        // 게임 초기화 메서드 실행
        InitGame();
    }

    private void OnLevelWasLoaded(int index)
    {
        // Scene의 Level을 1 증가시킴
        level++;
        // InitGame 메서드 호출
        InitGame();
    }

    // Board를 초기화시키는 메서드
    void InitGame()
    {
        // Level이 그려지고 있음을 표시
        doingSetup = true;
        // 플레이어를 움직이지 않도록 함
        playersTurn = false;

        // LevelImage를 가져옴
        levelImage = GameObject.Find("LevelImage");
        // LevelText를 가져옴
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        // LevelText를 현재 Level 숫자로 설정
        levelText.text = "Day " + level;
        // LevelImage 활성화
        levelImage.SetActive(true);
        // HideLevelImage 메서드를 호출하고 levelStartDelay만큼 정지함
        Invoke("HideLevelImage", levelStartDelay);

        // enemies에 담긴 모든 요소 제거
        enemies.Clear();
        // level에 따라 Scene을 그림
        boardScript.SetupScene(level);
    }

    // LevelImage를 비활성화하는 메서드
    private void HideLevelImage()
    {
        // LevelImage를 비활성화
        levelImage.SetActive(false);
        // Level을 다 그렸다는 표시
        doingSetup = false;
        // player을 움직일 수 있도록 함
        playersTurn = true;
    }

    // 게임 오버되었을 때 GameManager를 비활성화시키는 메서드
    public void GameOver()
    {
        // 얼마나 많은 날을 살아남았는지를 보여줌
        levelText.text = "After " + level + " days, you starved.";
        // LevelImage 활성화
        levelImage.SetActive(true);
        // GameManager의 Update를 중지
        enabled = false;
    }

    void Update()
    {
        // Player의 턴이거나 enemy가 움직이고 있거나 Level이 그려지고 있다면 반환
        if(playersTurn || enemiesMoving || doingSetup)
        {
            return;
        }

        // 아니라면 MoveEnemies 코루틴 호출
        StartCoroutine(MoveEnemies());
    }

    // Enemy를 enemies List에 추가
    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    // Enemy를 움직이는 코루틴
    IEnumerator MoveEnemies()
    {
        // Enemy가 이동하고 있다는 표시
        enemiesMoving = true;
        // Enemy를 움직이기 전에 turnDelay만큼의 Delay를 줌
        yield return new WaitForSeconds(turnDelay);
        // enemy가 없다면
        if(enemies.Count == 0)
        {
            // turnDelay만큼의 Delay를 줌
            yield return new WaitForSeconds(turnDelay);
        }

        //Enemy의 개수만큼 반복
        for(int i = 0; i < enemies.Count; i++)
        {
            // Enemy를 이동시킴
            enemies[i].MoveEnemy();
            // turnDelay만큼의 Delay를 줌
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        // Player에게 턴을 넘겨줌
        playersTurn = true;
        // Enemy의 이동이 끝났다는 표시
        enemiesMoving = false;
    }
}
