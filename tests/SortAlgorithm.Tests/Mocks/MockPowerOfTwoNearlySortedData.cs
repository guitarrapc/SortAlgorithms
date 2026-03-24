namespace SortAlgorithm.Tests;

public static class MockPowerOfTwoNearlySortedData
{
    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = CreateNearlySorted(16)
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = CreateNearlySorted(64)
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = CreateNearlySorted(256)
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = CreateNearlySorted(512)
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = CreateNearlySorted(1024)
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = CreateNearlySorted(2048)
        };

        static int[] CreateNearlySorted(int size)
        {
            var array = Enumerable.Range(0, size).ToArray();
            var random = new Random(42);
            // Swap a few elements to make it "nearly sorted"
            for (int i = 0; i < size / 10; i++)
            {
                int idx1 = random.Next(size);
                int idx2 = random.Next(size);
                (array[idx1], array[idx2]) = (array[idx2], array[idx1]);
            }
            return array;
        }
    }
}
