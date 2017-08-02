using System.Collections;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    // 1만큼 가는 데 걸리는 시간
    public float moveTime = 0.1f;
    // 이동 시 충돌을 검사할 때 쓰이는 레이어 마스크
    public LayerMask blockingLayer;

    // BoxCollider2D 컴포넌트를 할당할 변수
    private BoxCollider2D boxColider;
    // Rigidbody2D 컴포넌트를 할당할 변수
    private Rigidbody2D rb2D;
    // moveTime의 역수를 저장할 변수
    // 나누기 대신 비교적 효율적인 곱하기 연산을 할 수 있게 해 줌
    private float inverseMoveTime;

    // 재정의될 수 있음
    protected virtual void Start()
    {
        // BoxColider2D 컴포넌트 할당
        boxColider = GetComponent<BoxCollider2D>();
        // Rigidbody2D 컴포넌트 할당
        rb2D = GetComponent<Rigidbody2D>();
        // moveTime의 역수를 대입
        inverseMoveTime = 1f / moveTime;
    }

    // 이동할 방향을 가져와서 이동하는 메서드
    // 이동이 가능한지를 bool으로, blockingLayer과 부딪혔는지를 RaycastHit2D로 반환
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        // 시작점을 현재 위치로 지정
        Vector2 start = transform.position;
        // 도착점을 시작점에서 방향을 더한 위치로 지정
        Vector2 end = start + new Vector2(xDir, yDir);

        // hit가 자기 자신의 BoxColider2D에 부딪히지 않도록 BoxColider2D 컴포넌트 비활성화
        boxColider.enabled = false;
        // start에서 end로 Ray를 보냈을 때 blockingLayer에 부딪힌 위치를 hit에 저장
        hit = Physics2D.Linecast(start, end, blockingLayer);
        // 다시 BoxColider2D 컴포넌트 활성화
        boxColider.enabled = true;

        // 부딪히지 않았다면
        if(hit.transform == null)
        {
            // SmoothMovement 코루틴 시작 (이동)
            StartCoroutine(SmoothMovement(end));
            // 이동 가능하다는 의미로 true 반환
            return true;
        }

        // 이동 불가능하다는 의미로 false 반환
        return false;
    }

    // 재정의될 수 있음
    // 이동 방향을 받아와서 이동 (Move 메서드 실행)
    // 이동이 가능하지 않다면 OnCantMove 메서드 실행
    protected virtual void AttemptMove<T>(int xDir, int yDir) where T : Component
    {
        // 목적지까지의 이동경로에 blockingLayer가 있는지 검사하기 위한 변수
        RaycastHit2D hit;
        // Move 메서드 호출하고 반환값을 canMove에 저장
        bool canMove = Move(xDir, yDir, out hit);

        // Move 메서드에서 hit이 부딪히지 않았다면 메서드 종료
        if (hit.transform == null)
        {
            return;
        }

        // hit과 부딪힌 Object를 hitComponent에 할당
        T hitComponent = hit.transform.GetComponent<T>();

        // 움직일 수 없고 hit이 무언가에 부딪혔다면
        if( !canMove && hitComponent != null)
        {
            // OnCantMove 실행 (hitComponent가 무엇이냐에 따라 실행내용이 달라짐)
            OnCantMove(hitComponent);
        }
    }

    // 실제로 MovingObject를 움직이는 코루틴, 도착점을 받아옴
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        // 남은 거리의 제곱 (연산이 빠름)에 현재 Object의 위치에서 도착점을 빼고 제곱한 값을 대입
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        // 남은 거리의 제곱이 Epsilon (0에 수렴하는 값) 보다 크면
        while(sqrRemainingDistance > float.Epsilon)
        {
            // 현재 Object의 위치에서 도착점의 방향으로 inverseMoveTime * Time.deltaTime만큼 이동한 거리를 newPosition에 대입
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            // Object를 newPosition으로 이동
            rb2D.MovePosition(newPosition);
            // 남은 거리의 제곱을 다시 계산
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            // 루프를 갱신하기 전에 다음 프레임을 기다림
            yield return null;
        }
    }

    // 추상 메서드
    // 움직이지 못할 경우에 실행하는 메서드
    protected abstract void OnCantMove<T>(T component) where T : Component;
}
