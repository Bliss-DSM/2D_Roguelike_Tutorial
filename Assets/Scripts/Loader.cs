using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    // GameManager를 담을 GameObject 변수
    public GameObject gameManager;

    void Awake()
    {
        // GameManager의 인스턴스가 할당되지 않았다면
        if(GameManager.instance == null)
        {
            // gameManager prefab을 인스턴스화함
            Instantiate(gameManager);
        }
    }
}