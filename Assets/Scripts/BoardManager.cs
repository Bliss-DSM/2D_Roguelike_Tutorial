using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    // 클래스 직렬화
    [Serializable]
    // 범위를 지정할 때 쓰이는 Class
    public class Count
    {
        // 최솟값
        public int minimum;
        // 최댓값
        public int maximum;

        // 최솟값과 최댓값을 받아오는 생성자
        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    // Board의 행(세로줄) 개수
    public int columns = 8;
    // Board의 열(가로줄) 개수
    public int rows = 8;
    // 생성될 wall 개수 범위 지정
    public Count wallCount = new Count(5, 9);
    // 생성될 food 개수 범위 지정
    public Count foodCount = new Count(1, 5);
    // exit prefab이 담길 GameObject 변수
    public GameObject exit;
    // floor prefab이 담길 GameObject 배열
    public GameObject[] floorTiles;
    // wall prefab이 담길 GameObject 배열
    public GameObject[] wallTiles;
    // food prefab이 담길 GameObject 배열
    public GameObject[] foodTiles;
    // enemy prefab이 담길 GameObject 배열
    public GameObject[] enemyTiles;
    // outerWall prefab이 담길 GameObject 배열
    public GameObject[] outerWallTiles;

    // board를 구성하는 요소들의 부모, Hierarchy를 정리하기 위함
    private Transform boardHolder;
    // wall, food, enemy prefab이 위치할 수 있는 Vector3 값을 저장한 List
    private List<Vector3> gridPositions = new List<Vector3>();

    // gridPositions을 초기화하는 메서드
    void InitializeList()
    {
        // gridPositions에 담긴 모든 요소 제거
        gridPositions.Clear();

        // 중첩 for문을 돌면서 gridPositions에 요소들 추가
        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    // floor, outerWall prefab을 그리는 메서드
    void BoardSetup()
    {
        // Board라는 이름의 GameObject 생성
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                // floorTiles에서 랜덤한 prefab 하나를 가져와 toInstantiate에 대입
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                // 만약 제일 바깥에 위치한다면
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    // outerWallTiles에서 랜덤한 prefab 하나를 가져와 toInstantiate에 대입
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }

                // toInstantiate를 (x, y) 위치에 인스턴스화함
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                //instance의 부모를 boardHolder로 설정
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    // gidPositions에서 랜덤한 위치를 선택하는 메서드
    Vector3 RandomPosition()
    {
        // gridPositions의 랜덤한 index를 randomIndex에 저장
        int randomIndex = Random.Range(0, gridPositions.Count);
        // gridPositions의 randomIndex번째 요소를 randomPosition에 저장
        Vector3 randomPosition = gridPositions[randomIndex];
        // randomPosition에 저장된 요소는 gridPositions에서 제거
        gridPositions.RemoveAt(randomIndex);
        // randomPosition 반환
        return randomPosition;
    }

    // 최소 minimum번에서 최대 maximum번, 랜덤으로 tileArray에서 prefab을 하나 뽑아 인스턴스화하는 메서드
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        // 몇 번 뽑을지 minimum과 maximum 사이에서 랜덤으로 정함
        int objectCount = Random.Range(minimum, maximum + 1);

        // objectCount번 인스턴스화
        for(int i = 0; i < objectCount; i++)
        {
            // RandomPosition 메서드 호출해서 gridPositions에 저장되어 있는 위치를 랜덤으로 뽑아 randomPosition에 저장
            Vector3 randomPosition = RandomPosition();
            // tileArray에서 랜덤으로 prefab 하나를 뽑아 tileChoice에 저장
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            // tileChoice를 randomPosition 위치에 인스턴스화함
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    // GameManager에서 호출하는 public 메서드
    // level에 맞는 Scene을 그리는 메서드
    public void SetupScene(int level)
    {
        // floor과 outerWall을 그림
        BoardSetup();
        // gridPositions 초기화
        InitializeList();
        // 최소 minimum번에서 최대 maximum번, 랜덤으로 wallTiles에서 prefab을 하나 뽑아 인스턴스화함
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        // 최소 minimum번에서 최대 maximum번, 랜덤으로 wallTiles에서 prefab을 하나 뽑아 인스턴스화함
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        // enemy의 숫자를 Log함수에 비례해서 증가하도록 설정
        int enemyCount = (int)Mathf.Log(level, 2f);
        // enemyCount번, 랜덤으로 enemyTiles에서 prefab을 하나 뽑아 인스턴스화함
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        // exit을 오른쪽 위에 인스턴스화함
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }
}
