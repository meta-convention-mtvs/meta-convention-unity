using UnityEngine;
using DG.Tweening;  // DOTween 네임스페이스 추가

public class FloatingAnimation : MonoBehaviour
{
    public float moveDistance = 0.5f; // 떠오를 최대 거리
    public float duration = 1f; // 애니메이션 지속 시간

    void Start()
    {
        // 오브젝트가 위아래로 떠다니는 애니메이션 설정
        // 위치를 초기 위치에서 +moveDistance 만큼 위로 가고, 다시 원위치로 돌아오는 애니메이션
        transform.DOMoveY(transform.position.y + moveDistance, duration)
            .SetLoops(-1, LoopType.Yoyo)  // 반복적으로 (Yoyo 타입으로 위아래로 반복)
            .SetEase(Ease.InOutSine);     // 부드러운 Ease 인아웃 효과
    }
}