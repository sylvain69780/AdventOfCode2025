public class CombinationGenerator
{
    public static List<int[]> GenerateCombinations(int sum, int tupleSize)
    {
        var result = new List<int[]>();
        GenerateCombinationsRecursive(sum, tupleSize, new int[tupleSize], 0, result);
        return result;
    }

    private static void GenerateCombinationsRecursive(
        int remainingSum,
        int tupleSize,
        int[] current,
        int index,
        List<int[]> result)
    {
        if (index == tupleSize - 1)
        {
            current[index] = remainingSum;
            result.Add((int[])current.Clone());
            return;
        }

        for (int i = 0; i <= remainingSum; i++)
        {
            current[index] = i;
            GenerateCombinationsRecursive(remainingSum - i, tupleSize, current, index + 1, result);
        }
    }
}