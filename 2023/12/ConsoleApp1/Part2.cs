public class Part2
{
    public static long Run()
    {
        IEnumerable<(int recordNo, string springs, int[] groups)> conditionRecords = File.ReadAllLines(@"..\..\..\..\day-12.txt").Select((line, idx) =>
        {
            var parts = line.Split(" ");
            return (idx, parts[0], parts[1].Split(',').Select(int.Parse).ToArray());
        });

        var lockObj = new object();
        long sum = 0;
        Parallel.ForEach(conditionRecords.Expand().ToArray(), record =>
        {
            var combinationsPerIteration = new List<(int startIndex, int lastIndex, long combinationCount)[]>(Enumerable.Range(0,5).Select(_ => new (int startIndex, int lastIndex, long combinationCount)[0]));
            for (int i = 0; i < 5; i++)
            {
                var combinations = (i != 0) ? combinationsPerIteration.ElementAt(i-1) : new[] { (startIndex: 0, lastIndex: 0, combinationCount: (long)0) };
                Parallel.ForEach(combinations, combination =>
                {
                    var currentIterationCombinations = new List<(int idx, int groupSize)[]>();

                    var springs = record.springs.AsSpan();
                    var startIdx = (i==0) ? 0 : combination.lastIndex + 2;
                    springs = springs.Slice(startIdx);

                    if (i < 4)
                        springs = springs.Slice(0, springs.FindRightMostPosition(Enumerable.Range(0, 4 - i).SelectMany(_ => record.groups)) - 1);

                    var positions = springs.MoveToStartPositions(record.groups);

                    if (positions.Length != 0)
                    {
                        do
                        {
                            if (springs.IsValidCombination(positions, i != 4))
                            {
                                currentIterationCombinations.Add(positions);
                            }
                        } while ((positions = springs.TryMoveNext(positions)).Length > 0);
                    }
                    lock (lockObj)
                    {
                        var perIteration = combinationsPerIteration.ElementAt(i);
                        combinationsPerIteration[i] = perIteration.Union(currentIterationCombinations.GroupBy(position => position.Last().idx + position.Last().groupSize - 1).Select(group => (firstIndex: startIdx, lastIndex: startIdx + group.Key, combinationCount: (long)group.Count()))).ToArray();
                    }
                });
            }

            var validCombinations = combinationsPerIteration[0]
                                        .Join(combinationsPerIteration[1], c0 => c0.lastIndex + 2, c1 => c1.startIndex, (c0, c1) => (lastIndex: c1.lastIndex, combinationCount: c0.combinationCount * c1.combinationCount))
                                        .Join(combinationsPerIteration[2], c1 => c1.lastIndex + 2, c2 => c2.startIndex, (c1, c2) => (lastIndex: c2.lastIndex, combinationCount: c1.combinationCount * c2.combinationCount))
                                        .Join(combinationsPerIteration[3], c2 => c2.lastIndex + 2, c3 => c3.startIndex, (c2, c3) => (lastIndex: c3.lastIndex, combinationCount: c2.combinationCount * c3.combinationCount))
                                        .Join(combinationsPerIteration[4], c3 => c3.lastIndex + 2, c4 => c4.startIndex, (c3, c4) => (lastIndex: c4.lastIndex, combinationCount: c3.combinationCount * c4.combinationCount))
                                        .Sum(c => c.combinationCount);
            lock (lockObj)
            {
                sum += validCombinations;
            }
        });
        return sum;
    }
}

public static partial class Extensions
{
    public static int FindRightMostPosition(this ReadOnlySpan<char> springs, IEnumerable<int> groupSizes)
    {
        var rightMostBoundary = springs.Length - 1;
        foreach (var groupSize in groupSizes.Reverse())
            rightMostBoundary = springs.FindRightMostPosition(rightMostBoundary, groupSize) - 2;

        return rightMostBoundary + 2;
    }

    public static int FindRightMostPosition(this ReadOnlySpan<char> springs, int rightMostBoundary, int groupSize)
    {
        var startLookingAt = rightMostBoundary - groupSize + 1;
        for (int i = startLookingAt; i >= 0; i--)
        {
            var candidate = springs.Slice(i, groupSize);
            var prevIdx = i - 1;
            if (!candidate.ContainsAnyExcept('#', '?') && (i == 0 || springs[i - 1] != '#'))
                return i;
        }

        throw new InvalidOperationException("Unable to find right-most position");
    }

    public static IEnumerable<(int recordNo, string springs, int[] groups)> Expand(this IEnumerable<(int recordNo, string springs, int[] groups)> source)
    {
        foreach (var record in source)
        {
            var str = new string[5];
            Array.Fill(str, record.springs);
            var springs = string.Join('?', str);

            yield return (record.recordNo, springs, record.groups);
        }
    }

    public static int FindFirstLocation(this ReadOnlySpan<char> springs, int offset, int groupSize)
    {
        for (int i = offset; i < springs.Length - groupSize + 1; i++)
        {
            var candidate = springs.Slice(i, groupSize);
            var nextIdx = i + groupSize;
            if (!candidate.ContainsAnyExcept('#', '?') && (i == 0 || springs[i-1] != '#') && (nextIdx == springs.Length || springs[nextIdx] != '#'))
                return i;
        }

        return -1;
    }

    public static bool IsValidCombination(this ReadOnlySpan<char> springs, (int idx, int groupSize)[] positions, bool allowTrailingHashes)
    {
        var tmp = springs.ToArray();
        foreach (var position in positions.Where(p => p.groupSize != 0))
            Array.Fill(tmp, '@', position.idx, position.groupSize);

        var firstHash = tmp.AsSpan().IndexOf('#');
        var highestIdx = positions.Max(p => p.idx + p.groupSize - 1);
        return firstHash == -1 || (allowTrailingHashes && highestIdx < firstHash);
    }

    public static (int idx, int groupSize)[] MoveToStartPositions(this ReadOnlySpan<char> springs, int[] groupSizes)
    {
        var positions = new (int idx, int groupSize)[groupSizes.Length];
        int offset = 0;
        int i = 0;
        foreach (var groupSize in groupSizes)
        {
            var idx = springs.FindFirstLocation(offset, groupSize);
            if (idx == -1)
                return new (int idx, int groupSize)[0];

            positions[i] = (idx, groupSize);
            offset = idx + groupSize + 1;
            i++;
        }
        return positions;
    }

    public static (int idx, int groupSize)[] TryMoveNext(this ReadOnlySpan<char> springs, (int idx, int groupSize)[] positions)
    {
        var currentPosition = -1;
        (int idx, int groupSize)[]? newPositions = null;
        while (++currentPosition < positions.Count() && (newPositions = springs.TryAdvance(currentPosition, positions)) == null);

        //All elements were already at their final positions
        if (newPositions == null)
            return new (int idx, int groupSize)[0];

        if (currentPosition > 0)
        {
            //Reset all elements before the one that got moved
            var resetPositions = springs.MoveToStartPositions(newPositions.Take(currentPosition).Select(p => p.groupSize).ToArray());
            if (resetPositions.Length == 0)
                throw new InvalidOperationException("Unable to reset positions");
            Array.Copy(resetPositions.ToArray(), newPositions, currentPosition); //newPositions was created by TryAdvance(), so it's safe not to copy
        }
        
        return newPositions;
    }

    public static (int idx, int groupSize)[]? TryAdvance(this ReadOnlySpan<char> springs, int positionToAdvance, (int idx, int groupSize)[] allPositions) 
    {
        var current = allPositions[positionToAdvance];
        var next = (positionToAdvance < allPositions.Length - 1) ? allPositions[positionToAdvance + 1] : default;
        
        for (int i = current.idx + 1; i < ((next == default || next.groupSize == 0) ? springs.Length - current.groupSize + 1 : next.idx - current.groupSize); i++)
        {
            var candidate = springs.Slice(i, current.groupSize);
            var nextIdx = i + current.groupSize;
            if (!candidate.ContainsAnyExcept('#', '?') && (i == 0 || springs[i - 1] != '#') && (nextIdx == springs.Length || springs[nextIdx] != '#'))
            {
                var res = new (int idx, int groupSize)[allPositions.Length]; 
                Array.Copy(allPositions, res, allPositions.Length);
                res[positionToAdvance] = (i, current.groupSize);
                return res;
            }
        }

        return null;
    }
}