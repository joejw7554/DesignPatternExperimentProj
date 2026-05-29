using System.Collections.Generic;

/// <summary>
/// 싱글톤 패턴 예제 — 키-값(맵) 기반 전역 데이터 레지스트리.
///
/// 핵심 특징:
///   1. private 생성자 → 외부에서 new 로 생성 불가
///   2. 정적(static) Instance 프로퍼티 → 전역 접근점(global access point)
///   3. Double-Checked Locking → 멀티스레드 환경에서도 단일 인스턴스 보장
///   4. 내부에 Dictionary(맵) 를 보유 → 게임 설정·데이터를 키-값으로 관리
/// </summary>
public class DataRegistry
{
    // ─── 싱글톤 구현부 ───────────────────────────────────────────────────────

    private static DataRegistry _instance;
    private static readonly object _lock = new object();

    /// <summary>전역 접근점. 최초 호출 시 인스턴스를 생성(Lazy Init)합니다.</summary>
    public static DataRegistry Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new DataRegistry();
                }
            }
            return _instance;
        }
    }

    // private 생성자: 클래스 외부에서 직접 생성 불가
    private DataRegistry()
    {
        _data = new Dictionary<string, string>();
    }

    // ─── 맵(Dictionary) 기능부 ───────────────────────────────────────────────

    private readonly Dictionary<string, string> _data;

    /// <summary>키에 값을 저장(없으면 추가, 있으면 덮어쓰기)합니다.</summary>
    public void Set(string key, string value) => _data[key] = value;

    /// <summary>키에 해당하는 값을 반환합니다. 키가 없으면 null.</summary>
    public string Get(string key) => _data.TryGetValue(key, out var val) ? val : null;

    /// <summary>키 존재 여부를 확인합니다.</summary>
    public bool ContainsKey(string key) => _data.ContainsKey(key);

    /// <summary>저장된 항목 수를 반환합니다.</summary>
    public int Count => _data.Count;

    /// <summary>모든 데이터를 삭제합니다. 인스턴스 자체는 유지됩니다.</summary>
    public void Clear() => _data.Clear();

    // ─── 테스트 전용 ─────────────────────────────────────────────────────────

#if UNITY_INCLUDE_TESTS
    /// <summary>
    /// [테스트 전용] 싱글톤 인스턴스를 null 로 초기화하여 다음 접근 시 새 인스턴스가
    /// 만들어지게 합니다. 프로덕션 코드에서는 호출하지 마세요.
    /// </summary>
    internal static void ResetForTesting()
    {
        lock (_lock)
        {
            _instance = null;
        }
    }
#endif
}
