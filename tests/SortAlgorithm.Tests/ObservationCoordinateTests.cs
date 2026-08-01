using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

/// <summary>
/// 共有コア（他アルゴリズムが委譲先として呼ぶ SortCore）が、渡されたスパン自身の座標系で
/// 相とロールを報告することを保証するテスト。
///
/// Read/Write/Compare/Swap は SortSpan がインデックスとバッファ ID を導出するので、スライスを
/// 受け取ったコアの要素操作は自動的にそのスライスの座標で報告される。一方、相とロールは呼び出し側が
/// 手で書くため、生のループ変数を渡したりバッファ ID を定数で埋めたりすると、要素操作とは別の要素を
/// 指したまま気付かれない。実際 <see cref="InsertionSort"/> の SortCore がこの状態で、BucketSort が
/// ソートするバケツと Glidesort がソートするスクラッチブロックのすべてが、入力配列の無関係な位置を
/// 指していた。
///
/// ソート結果は正しいままで、イベント列も単体では辻褄が合うので、同一呼び出しの要素操作と突き合わせる
/// このテストでしか検出できない。
/// </summary>
public class ObservationCoordinateTests
{
    private static SortSpan<int, ComparableComparer<int>, VisualizationContext> Wrap(int[] array, VisualizationContext context)
        => new(array.AsSpan(), context, new ComparableComparer<int>(), 0);

    /// <param name="Name">失敗時に出す表示名。</param>
    /// <param name="IndexPhase">パラメータがインデックスである相。</param>
    /// <param name="IndexParamCount">その相の先頭から数えていくつのパラメータがインデックスか。</param>
    /// <param name="CountPhase">パラメータが件数である相（インデックスと取り違えていないことの確認用）。null なら無し。</param>
    /// <param name="Invoke">(配列, スライス開始, スライス長, バッファ ID, コンテキスト) を受けてコアを呼ぶ。</param>
    public sealed record CoreCase(
        string Name,
        SortPhase IndexPhase,
        int IndexParamCount,
        SortPhase? CountPhase,
        Action<int[], int, int, int, VisualizationContext> Invoke)
    {
        public override string ToString() => Name;
    }

    public static IEnumerable<Func<CoreCase>> Cores()
    {
        // BucketSort / Glidesort / SpinSort / FlatStableSort がスライスを渡す。
        yield return () => new CoreCase(
            "InsertionSort.SortCore", SortPhase.InsertionPass, 3, null,
            static (array, start, length, bufferId, context) =>
            {
                var sliced = Wrap(array, context).Slice(start, length, bufferId);
                InsertionSort.SortCore(sliced, 0, length);
            });

        // 現在の呼び出し元は TimSort のみでスライスを渡さないが、契約は同じ。
        yield return () => new CoreCase(
            "BinaryInsertionSort.SortCore", SortPhase.BinaryInsertionPass, 3, null,
            static (array, start, length, bufferId, context) =>
            {
                var sliced = Wrap(array, context).Slice(start, length, bufferId);
                BinaryInsertionSort.SortCore(sliced, 0, length, 0);
            });

        // HeapExtract のパラメータは抽出回数であってインデックスではない。
        yield return () => new CoreCase(
            "HeapSort.SortCore", SortPhase.HeapBuild, 2, SortPhase.HeapExtract,
            static (array, start, length, bufferId, context) =>
            {
                var sliced = Wrap(array, context).Slice(start, length, bufferId);
                HeapSort.SortCore(sliced, 0, length);
            });
    }

    /// <summary>
    /// スライスを渡したとき、ロールと相のインデックスパラメータは要素操作と同じ座標系
    /// （スライスの Offset を加えた絶対インデックスと、スライスのバッファ ID）でなければならない。
    /// </summary>
    [Test]
    [MethodDataSource(nameof(Cores))]
    public async Task SharedCoreReportsPhaseAndRoleInSliceCoordinates(CoreCase testCase)
    {
        const int sliceStart = 5;
        const int sliceLength = 8;
        const int bufferId = 113;

        var roles = new List<(int Index, int BufferId)>();
        var elementOps = new List<(int Index, int BufferId)>();
        var indexPhaseParams = new List<int>();
        var countPhaseParams = new List<int>();

        var context = new VisualizationContext(
            onCompare: (i, j, _, bi, bj) =>
            {
                if (i >= 0) elementOps.Add((i, bi));
                if (j >= 0) elementOps.Add((j, bj));
            },
            onSwap: (i, j, b) => { elementOps.Add((i, b)); elementOps.Add((j, b)); },
            onIndexRead: (i, b) => elementOps.Add((i, b)),
            onIndexWrite: (i, b, _) => elementOps.Add((i, b)),
            onPhase: (phase, p1, p2, p3) =>
            {
                var all = new[] { p1, p2, p3 };
                if (phase == testCase.IndexPhase) indexPhaseParams.AddRange(all.Take(testCase.IndexParamCount));
                else if (phase == testCase.CountPhase) countPhaseParams.AddRange(all.Take(2));
            },
            onRole: (i, b, _) => roles.Add((i, b)));

        // スライス外は 0、スライス内は降順（必ず要素が動く）。
        var array = new int[sliceStart + sliceLength + 4];
        for (var i = 0; i < sliceLength; i++) array[sliceStart + i] = sliceLength - i;
        var expectedSlice = Enumerable.Range(1, sliceLength).ToArray();

        testCase.Invoke(array, sliceStart, sliceLength, bufferId, context);

        await Assert.That(array.Skip(sliceStart).Take(sliceLength).ToArray()).IsEquivalentTo(expectedSlice)
            .Because($"{testCase.Name}: 観測を変えてもソート結果は変わらないこと");
        await Assert.That(array.Take(sliceStart).Concat(array.Skip(sliceStart + sliceLength)).All(x => x == 0)).IsTrue()
            .Because($"{testCase.Name}: スライス外に書き込んではならない");

        var min = sliceStart;
        var max = sliceStart + sliceLength - 1;

        // 前提: 要素操作はスライスの座標系で報告されている（SortSpan がそうするため）。
        await Assert.That(elementOps).IsNotEmpty();
        await Assert.That(elementOps.All(o => o.BufferId == bufferId && o.Index >= min && o.Index <= max)).IsTrue()
            .Because($"{testCase.Name}: 要素操作 = [{string.Join(",", elementOps.Select(o => $"{o.Index}@{o.BufferId}").Distinct())}]");

        // ロールが同じ座標系であること。
        await Assert.That(roles).IsNotEmpty()
            .Because($"{testCase.Name}: ロールが 1 度も報告されていません");
        await Assert.That(roles.All(r => r.BufferId == bufferId)).IsTrue()
            .Because($"{testCase.Name}: ロールが要素操作と違うバッファを指しています = [{string.Join(",", roles.Select(r => r.BufferId).Distinct())}]（期待 {bufferId}）");
        await Assert.That(roles.All(r => r.Index >= min && r.Index <= max)).IsTrue()
            .Because($"{testCase.Name}: ロールのインデックスが範囲 [{min},{max}] の外です = [{string.Join(",", roles.Select(r => r.Index).Distinct().Order())}]");

        // 相のインデックスパラメータも同じ座標系であること。
        await Assert.That(indexPhaseParams).IsNotEmpty()
            .Because($"{testCase.Name}: {testCase.IndexPhase} が 1 度も報告されていません");
        await Assert.That(indexPhaseParams.All(p => p >= min && p <= max)).IsTrue()
            .Because($"{testCase.Name}: {testCase.IndexPhase} のインデックスパラメータが範囲 [{min},{max}] の外です = [{string.Join(",", indexPhaseParams.Distinct().Order())}]");

        // 件数パラメータは逆に Offset を足してはならない。
        if (testCase.CountPhase is not null)
        {
            await Assert.That(countPhaseParams).IsNotEmpty()
                .Because($"{testCase.Name}: {testCase.CountPhase} が 1 度も報告されていません");
            await Assert.That(countPhaseParams.All(p => p >= 0 && p <= sliceLength)).IsTrue()
                .Because($"{testCase.Name}: {testCase.CountPhase} のパラメータは件数であり、Offset を加えてはなりません = [{string.Join(",", countPhaseParams.Distinct().Order())}]");
        }
    }

    /// <summary>決定的なシャッフル配列（値 0..n-1）。</summary>
    private static int[] MakeArray(int n)
    {
        var arr = Enumerable.Range(0, n).ToArray();
        var rng = new Random(42);
        for (var i = n - 1; i > 0; i--)
        {
            var j = rng.Next(i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
        return arr;
    }

    public sealed record AlgorithmCase(string Name, Action<int[], VisualizationContext> Sort)
    {
        public override string ToString() => Name;
    }

    /// <summary>
    /// 全比較で少なくとも一方のオペランドがバッファ上の要素として報告されるアルゴリズム。
    ///
    /// 「読んだ値を後でも使う」箇所は、span 自身が読んで値を返す out オーバーロードで解消できる。
    ///
    /// 外してあるのは、1 回の読み取りを複数の比較で使い回すアルゴリズム:
    /// - StdSort: 分岐なしネットワーク PartiallySortedSwap（2 つ目は cmov が選んだ中間値どうしの比較で、
    ///   そもそもバッファ上に無い）
    /// - DualPivotQuickSort: ak を読んで 2 つのピボット値と比較する
    ///
    /// ここに位置を載せるには、読み直して実際には行わない読み取りを報告するか、呼び出し側に
    /// 「この値はこのインデックスから読んだ」と申告させるしかない。後者は任意の T に対して検証不能な
    /// 主張になり、手書きのロールインデックスがずれていたのと同じ壊れ方を作る。両オペランド -1 のまま
    /// 残す方が、嘘の位置を載せるより正確。
    /// </summary>
    public static IEnumerable<Func<AlgorithmCase>> LocatableComparisonAlgorithms()
    {
        yield return () => new AlgorithmCase("InsertionSort", static (a, c) => InsertionSort.Sort(a.AsSpan(), c));
        yield return () => new AlgorithmCase("BinaryInsertionSort", static (a, c) => BinaryInsertionSort.Sort(a.AsSpan(), c));
        yield return () => new AlgorithmCase("PairInsertionSort", static (a, c) => PairInsertionSort.Sort(a.AsSpan(), c));
        yield return () => new AlgorithmCase("TimSort", static (a, c) => TimSort.Sort(a.AsSpan(), c));
        yield return () => new AlgorithmCase("QuickSort", static (a, c) => QuickSort.Sort(a.AsSpan(), c));
        yield return () => new AlgorithmCase("QuickSort3way", static (a, c) => QuickSort3way.Sort(a.AsSpan(), c));
        yield return () => new AlgorithmCase("QuickSortMedian3", static (a, c) => QuickSortMedian3.Sort(a.AsSpan(), c));
        yield return () => new AlgorithmCase("IntroSort", static (a, c) => IntroSort.Sort(a.AsSpan(), c));
        yield return () => new AlgorithmCase("PDQSort", static (a, c) => PDQSort.Sort(a.AsSpan(), c));
        yield return () => new AlgorithmCase("PDQSortBranchless", static (a, c) => PDQSortBranchless.Sort(a.AsSpan(), c));
        yield return () => new AlgorithmCase("HeapSort", static (a, c) => HeapSort.Sort(a.AsSpan(), c));
        yield return () => new AlgorithmCase("SymMergeSort", static (a, c) => SymMergeSort.Sort(a.AsSpan(), c));
        yield return () => new AlgorithmCase("CycleSort", static (a, c) => CycleSort.Sort(a.AsSpan(), c));
    }

    /// <summary>
    /// 比較の両オペランドが -1 で報告されると、消費側はその比較をどの配列のどこにも置けない。
    /// 片方でもバッファ上の要素なら、そのインデックスとバッファ ID を報告しなければならない。
    ///
    /// 値ベースのオーバーロードにインライン Read を渡す書き方（<c>IsGreaterThan(s.Read(j), tmp)</c>）は
    /// 読み取りをインデックス付きで報告しておきながら、直後の比較で両オペランドの位置を捨てる。
    /// SortVivo の HowItWorks ではこれが「Compare temp (0) and temp (0)」＝ハイライト無し・値もプレース
    /// ホルダ、として描画されていた。
    /// </summary>
    [Test]
    [MethodDataSource(nameof(LocatableComparisonAlgorithms))]
    public async Task ComparisonsCarryTheLocationOfEveryOperandThatHasOne(AlgorithmCase testCase)
    {
        var unplaceable = 0;
        var total = 0;
        var context = new VisualizationContext(onCompare: (i, j, _, _, _) =>
        {
            total++;
            if (i < 0 && j < 0) unplaceable++;
        });

        var array = MakeArray(120);
        var expected = array.ToArray();
        Array.Sort(expected);

        testCase.Sort(array, context);

        await Assert.That(array).IsEquivalentTo(expected)
            .Because($"{testCase.Name}: 観測を変えてもソート結果は変わらないこと");
        await Assert.That(total).IsGreaterThan(0);
        await Assert.That(unplaceable).IsEqualTo(0)
            .Because($"{testCase.Name}: {unplaceable}/{total} 件の比較が両オペランド -1 で、消費側が位置を特定できません");
    }

    /// <summary>
    /// スライスしない通常経路（Offset 0 / バッファ 0）では、従来どおりの絶対インデックスであること。
    /// スライス側の修正が非スライス経路の報告を変えていないことを固定する。
    /// </summary>
    [Test]
    public async Task SharedCoreKeepsMainBufferCoordinatesForUnslicedSpan()
    {
        var insertionRoles = new List<(int Index, int BufferId)>();
        var heapRoles = new List<(int Index, int BufferId)>();

        var insertionArray = new[] { 4, 3, 2, 1 };
        InsertionSort.Sort(insertionArray.AsSpan(), new VisualizationContext(onRole: (i, b, _) => insertionRoles.Add((i, b))));

        var heapArray = new[] { 4, 3, 2, 1 };
        HeapSort.Sort(heapArray.AsSpan(), new VisualizationContext(onRole: (i, b, _) => heapRoles.Add((i, b))));

        await Assert.That(insertionRoles.Select(r => r.Index).Distinct().Order().ToList())
            .IsEquivalentTo(new List<int> { 1, 2, 3 });
        await Assert.That(insertionRoles.All(r => r.BufferId == 0)).IsTrue();

        // HeapSort は常にヒープ根（= first）にロールを付ける。
        await Assert.That(heapRoles).IsNotEmpty();
        await Assert.That(heapRoles.All(r => r.Index == 0 && r.BufferId == 0)).IsTrue()
            .Because($"[{string.Join(",", heapRoles.Select(r => $"{r.Index}@{r.BufferId}").Distinct())}]");
    }
}
