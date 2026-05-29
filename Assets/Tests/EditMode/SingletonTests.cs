using NUnit.Framework;

/// <summary>
/// DataRegistry 싱글톤 NUnit 테스트 모음.
///
/// ┌─ 테스트를 통해 알 수 있는 것 ──────────────────────────────────────────┐
/// │ 1. 동일 인스턴스 보장  — 몇 번을 호출해도 ReferenceEquals == true      │
/// │ 2. 전역 상태 공유      — 한 변수에서 저장한 값을 다른 변수에서 읽음     │
/// │ 3. 기본 CRUD           — Set/Get/ContainsKey/Count/Clear 동작 확인     │
/// │ 4. 데이터 영속성       — 인스턴스가 살아있는 한 데이터가 보존됨         │
/// │ 5. Clear 의미          — 데이터는 지워지지만 인스턴스는 그대로 유지     │
/// │ 6. ResetForTesting     — 강제 초기화 후 새 인스턴스가 생성되는지 확인  │
/// └────────────────────────────────────────────────────────────────────────┘
/// </summary>
[TestFixture]
public class SingletonTests
{
    [SetUp]
    public void SetUp()
    {
        // 각 테스트 시작 전 싱글톤을 초기화해 테스트 간 독립성을 확보합니다.
        DataRegistry.ResetForTesting();
    }

    [TearDown]
    public void TearDown()
    {
        DataRegistry.ResetForTesting();
    }

    // =========================================================================
    // [1] 동일 인스턴스 보장
    //     싱글톤의 핵심: 몇 번 호출해도 항상 같은 객체를 돌려준다.
    // =========================================================================

    [Test]
    public void Instance_항상_동일한_객체를_반환한다()
    {
        var a = DataRegistry.Instance;
        var b = DataRegistry.Instance;

        Assert.AreSame(a, b, "두 번 접근해도 같은 인스턴스여야 합니다.");
    }

    [Test]
    public void Instance_여러번_호출해도_동일한_레퍼런스를_가진다()
    {
        var instances = new DataRegistry[10];
        for (int i = 0; i < instances.Length; i++)
            instances[i] = DataRegistry.Instance;

        for (int i = 1; i < instances.Length; i++)
            Assert.AreSame(instances[0], instances[i],
                $"instances[0] 과 instances[{i}] 는 동일한 인스턴스여야 합니다.");
    }

    // =========================================================================
    // [2] 전역 상태 공유
    //     변수 refA 와 refB 는 같은 객체를 가리키므로 refA 에 저장한 값을
    //     refB 에서도 읽을 수 있다.
    // =========================================================================

    [Test]
    public void 한_곳에서_저장한_데이터를_다른_참조에서도_읽을_수_있다()
    {
        DataRegistry refA = DataRegistry.Instance;
        DataRegistry refB = DataRegistry.Instance;

        refA.Set("level", "5");

        Assert.AreEqual("5", refB.Get("level"),
            "동일 인스턴스이므로 refA 에 저장한 값을 refB 에서 읽을 수 있어야 합니다.");
    }

    // =========================================================================
    // [3] 기본 CRUD 동작
    // =========================================================================

    [Test]
    public void Set_후_Get으로_동일한_값을_반환한다()
    {
        DataRegistry.Instance.Set("playerName", "Hero");

        Assert.AreEqual("Hero", DataRegistry.Instance.Get("playerName"));
    }

    [Test]
    public void Set으로_기존_값을_덮어쓸_수_있다()
    {
        DataRegistry.Instance.Set("score", "100");
        DataRegistry.Instance.Set("score", "200");

        Assert.AreEqual("200", DataRegistry.Instance.Get("score"));
    }

    [Test]
    public void 존재하지_않는_키_조회시_null을_반환한다()
    {
        Assert.IsNull(DataRegistry.Instance.Get("nonExistentKey"),
            "없는 키는 null 을 반환해야 합니다.");
    }

    [Test]
    public void ContainsKey_저장된_키는_true_없는키는_false()
    {
        DataRegistry.Instance.Set("hp", "100");

        Assert.IsTrue(DataRegistry.Instance.ContainsKey("hp"));
        Assert.IsFalse(DataRegistry.Instance.ContainsKey("mp"));
    }

    [Test]
    public void Count_저장한_항목수와_일치한다()
    {
        DataRegistry.Instance.Set("a", "1");
        DataRegistry.Instance.Set("b", "2");
        DataRegistry.Instance.Set("c", "3");

        Assert.AreEqual(3, DataRegistry.Instance.Count);
    }

    // =========================================================================
    // [4] 데이터 영속성 — 인스턴스가 유지되는 한 데이터도 보존된다.
    // =========================================================================

    [Test]
    public void 인스턴스가_유지되는_한_데이터는_보존된다()
    {
        DataRegistry.Instance.Set("persistent", "yes");

        // 동일 인스턴스에 다시 접근해도 데이터가 남아있어야 한다.
        Assert.AreEqual("yes", DataRegistry.Instance.Get("persistent"));
    }

    // =========================================================================
    // [5] Clear — 데이터는 삭제되지만 인스턴스 자체는 유지된다.
    // =========================================================================

    [Test]
    public void Clear_후_Count는_0이고_데이터가_삭제된다()
    {
        DataRegistry.Instance.Set("x", "1");
        DataRegistry.Instance.Set("y", "2");
        DataRegistry.Instance.Clear();

        Assert.AreEqual(0, DataRegistry.Instance.Count);
        Assert.IsNull(DataRegistry.Instance.Get("x"));
    }

    [Test]
    public void Clear는_인스턴스_자체는_유지한다()
    {
        var before = DataRegistry.Instance;
        DataRegistry.Instance.Clear();
        var after = DataRegistry.Instance;

        Assert.AreSame(before, after,
            "Clear 는 데이터만 삭제하고 인스턴스는 유지해야 합니다.");
    }

    // =========================================================================
    // [6] ResetForTesting — 강제 초기화 후 새 인스턴스가 생성된다.
    //     이 테스트는 "싱글톤은 인스턴스 하나만 허용한다"는 제약을 검증하는
    //     도구(ResetForTesting) 자체가 올바르게 동작하는지 확인합니다.
    // =========================================================================

    [Test]
    public void ResetForTesting_후_새로운_인스턴스가_생성된다()
    {
        var before = DataRegistry.Instance;
        DataRegistry.ResetForTesting();
        var after = DataRegistry.Instance;

        Assert.AreNotSame(before, after,
            "ResetForTesting 후에는 새 인스턴스가 생성되어야 합니다.");
    }

    [Test]
    public void ResetForTesting_후_이전_데이터는_사라진다()
    {
        DataRegistry.Instance.Set("key", "value");
        DataRegistry.ResetForTesting();

        Assert.IsNull(DataRegistry.Instance.Get("key"),
            "인스턴스가 새로 만들어지면 이전 데이터는 없어야 합니다.");
    }
}
