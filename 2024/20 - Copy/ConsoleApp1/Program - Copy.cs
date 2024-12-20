//using System.Collections.Generic;

//var test = new[]
//{
//"###############",
//"#...#...#.....#",
//"#.#.#.#.#.###.#",
//"#S#...#.#.#...#",
//"#######.#.#.###",
//"#######.#.#...#",
//"#######.#.###.#",
//"###..E#...#...#",
//"###.#######.###",
//"#...###...#...#",
//"#.#####.#.###.#",
//"#.#...#.#.#...#",
//"#.#.#.#.#.#.###",
//"#...#...#...###",
//"###############",
//};
//var map = File.ReadAllLines(@"..\..\..\..\day-20.txt")
//    .Select(row => row.ToArray())
//    .ToArray();

//var start = map.Find('S');
//var goal = map.Find('E');

//var visited = new Dictionary<Coordinate, int>();
//var jobs = new PriorityQueue<(Coordinate pos, List<Coordinate> path, int steps), int>();
//jobs.Enqueue((start, [start], 0), 0);
//var bestTime = 0;
//var bestPath = Array.Empty<Coordinate>();
//while (jobs.Count > 0)
//{
//    var (pos, path, steps) = jobs.Dequeue();

//    if (pos == goal)
//    {
//        bestTime = steps;
//        bestPath = path.ToArray();
//        break;
//    }

//    if (!visited.TryGetValue(pos, out var prevSteps) || steps < prevSteps)
//    {
//        visited[pos] = steps;

//        foreach (var candidate in pos.Neighbours().Where(x => map.Valid(x)))
//        {
//            var newPath = new List<Coordinate>(path);
//            newPath.Add(candidate);
//            jobs.Enqueue((candidate, newPath, steps + 1), steps + 1 + candidate.Distance(goal));
//        }
//    }
//}

////map.Print(bestPath, start, goal);

//var cheatDistance = 50;
//var cheats = new List<(Coordinate source, Coordinate dest, int saved)>();
//var wallRouteCache = new Dictionary<(Coordinate from, Coordinate to), int>();
//for (int i = 0; i < bestPath.Length - cheatDistance; i++)
//{
//    if (i + cheatDistance >= bestPath.Length)
//        continue;

//    var source = bestPath[i];
//    var candidates = bestPath[(i + cheatDistance)..].Where(dest =>
//    {
//        var distance = source.Distance(dest);
//        //var wallDistance = map.WallDistance(source, dest, wallRouteCache);
//        //var saved = Array.IndexOf(bestPath, dest) - i - wallDistance;
//        return /*bestPath[i + distance] != dest && */  distance > 1 && distance <= 20 /*&& map.HasWallRoute(source, dest)*//*map.HasWallOnlyRoute(source, dest)*/ /*map.WallDistance(source, dest, wallRouteCache) <= 20*/ /*&& saved >= 50*//*map.HasWallOnlyRoute(source, dest, wallRouteCache) <= 20*/ /*map.InBetweens(source, dest).Any(x => x == '#')*/;
//    }).ToArray();
//    //    cheats.AddRange(candidates.Select(x => (source, x, Array.IndexOf(bestPath, x) - i - map.WallDistance(source, x, wallRouteCache)/*source.Distance(x)*/)));
//    //cheats.AddRange(candidates.Select(x => (source, x, Array.IndexOf(bestPath, x) - i - map.WallDistance(source, x, wallRouteCache) /*source.Distance(x)*/)));
//    cheats.AddRange(candidates.Select(x => (source, x, Array.IndexOf(bestPath, x) - i - /*map.WallDistance(source, x, wallRouteCache)*/ source.Distance(x))));
//}

//var grouped = cheats.GroupBy(x => x.saved).OrderBy(x => x.Key).ToList();

////var cheatGoal = bestTime - 50;
////var cheats2 = cheats.Select(x => (x.source, x.dest)).Distinct().Select(hack => (hack.source, hack.dest, saved: bestTime - (Array.IndexOf(bestPath, hack.source) + map.WallDistance(hack.source, hack.dest, wallRouteCache) + RunTrack2(hack.dest, goal)))).ToList();
////var grouped2 = cheats2.GroupBy(x => x.saved).OrderBy(x => x.Key).ToList();
////var result2 = cheats2.Count(x => x.saved >= cheatDistance);

//var result = cheats.Count(x => x.saved >= cheatDistance);

//Console.WriteLine($"Part 1 - {result}");
//Console.WriteLine($"Part 2 - ");
//Console.ReadLine();

////var cheatDistance = 100;
////var cheats = new List<(Coordinate source, Coordinate dest, int saved)>();
////for (int i = 0; i < bestPath.Length; i++)
////{
////    //if (i + cheatDistance >= bestPath.Length)
////    //    continue;

////    var source = bestPath[i];
////    var candidates = bestPath[/*(i + cheatDistance)*/i..].Where(dest =>
////    {
////        var distance = source.Distance(dest);
////        return /*bestPath[i + distance] != dest && */  distance > 1 && distance <= 20 && map.HasWallOnlyRoute(source, dest) /*map.InBetweens(source, dest).Any(x => x == '#')*/;
////    });
////    cheats.AddRange(candidates.Select(x => (source, x, Array.IndexOf(bestPath, x) - i - source.Distance(x))));
////}

////var grouped = cheats.GroupBy(x => x.saved).OrderBy(x => x.Key).ToList();

////var result = cheats.Distinct().Count(x => x.saved >= 50);

////var cheatGoal = bestTime - cheatDistance;
////var cheats = new List<(Coordinate source, Coordinate dest)>();
////for (int i = 0; i < bestPath.Length; i++)
////{
////    var source = bestPath[i];
////    var candidates = bestPath[(i+1)..].Where(dest =>
////    {
////        var distance = source.Distance(dest);
////        return bestPath[i + distance] != dest && distance <= 20 /*&& map.InBetweens(source, dest).Any(x => x == '#')*/;
////    });
////    cheats.AddRange(candidates.Select(x => (source, x)));
////}

////var uniqueHacks = cheats.Distinct().ToList();

////var cheatGoal = bestTime - 50;
////var result = uniqueHacks.Count(hack => RunTrack(hack) <= cheatGoal);
////var result = 0;
////foreach (var hack in uniqueHacks)
////{
////    var time = RunTrack(hack);
////    if (time <= cheatGoal)
////        result++;
////}


//int RunTrack((Coordinate source, Coordinate dest) hack)
//{
//    var cheatMap = map.CloneMap();
//    var inBetweens = hack.Inbetweens().ToList();
//    foreach (var wall in inBetweens.Where(x => cheatMap[x.Y][x.X] == '#'))
//        cheatMap[wall.Y][wall.X] = '.';
//    var visited = new Dictionary<Coordinate, int>();
//    var jobs = new PriorityQueue<(Coordinate pos, int steps), int>();
//    jobs.Enqueue((start, 0), 0);
//    while (jobs.Count > 0)
//    {
//        var (pos, steps) = jobs.Dequeue();

//        if (pos == goal)
//        {
//            return steps;
//        }

//        if (!visited.TryGetValue(pos, out var prevSteps) || steps < prevSteps)
//        {
//            visited[pos] = steps;

//            foreach (var candidate in pos.Neighbours().Where(x => cheatMap.Valid(x)))
//            {
//                jobs.Enqueue((candidate, steps + 1), steps + 1 + candidate.Distance(goal));
//            }
//        }
//    }

//    throw new InvalidOperationException();
//}

//int RunTrack2(Coordinate start, Coordinate goal)
//{
//    var visited = new Dictionary<Coordinate, int>();
//    var jobs = new PriorityQueue<(Coordinate pos, int steps), int>();
//    jobs.Enqueue((start, 0), 0);
//    while (jobs.Count > 0)
//    {
//        var (pos, steps) = jobs.Dequeue();

//        if (pos == goal)
//        {
//            return steps;
//        }

//        if (!visited.TryGetValue(pos, out var prevSteps) || steps < prevSteps)
//        {
//            visited[pos] = steps;

//            foreach (var candidate in pos.Neighbours().Where(x => map.Valid(x)))
//            {
//                jobs.Enqueue((candidate, steps + 1), steps + 1 + candidate.Distance(goal));
//            }
//        }
//    }

//    throw new InvalidOperationException();
//}

//record Coordinate(int X, int Y)
//{     
//    public IEnumerable<Coordinate> Neighbours()
//    {
//        yield return new Coordinate(X - 1, Y);
//        yield return new Coordinate(X + 1, Y);
//        yield return new Coordinate(X, Y - 1);
//        yield return new Coordinate(X, Y + 1);
//    }

//    public int Distance(Coordinate goal) => Math.Abs(X - goal.X) + Math.Abs(Y - goal.Y);
//}

//internal static class Extensions
//{
//    public static Coordinate Find(this char[][] map, char target)
//    {
//        for (var y = 0; y < map.Length; y++)
//            for (var x = 0; x < map[y].Length; x++)
//                if (map[y][x] == target)
//                    return new Coordinate(x, y);

//        return new Coordinate(-1, -1);
//    }

//    public static bool Valid(this char[][] map, Coordinate pos)
//    {
//        if (pos.Y < 0 || pos.Y >= map.Length)
//            return false;

//        if (pos.X < 0 || pos.X >= map[pos.Y].Length)
//            return false;

//        return map[pos.Y][pos.X] != '#';
//    }

//    public static bool InBounds(this char[][] map, Coordinate pos)
//    {
//        if (pos.Y < 0 || pos.Y >= map.Length)
//            return false;

//        if (pos.X < 0 || pos.X >= map[pos.Y].Length)
//            return false;

//        return true;
//    }

//    public static IEnumerable<char> InBetweens(this char[][] map, Coordinate source, Coordinate dest)
//    {
//        var x = source.X + (dest.X - source.X) / 2;
//        var y = source.Y + (dest.Y - source.Y) / 2;
//        yield return map[y][x];
//    }

//    public static IEnumerable<Coordinate> Inbetweens(this (Coordinate source, Coordinate dest) hack)
//    {
//        var minX = Math.Min(hack.source.X, hack.dest.X);
//        var minY = Math.Min(hack.source.Y, hack.dest.Y);
//        var maxX = Math.Max(hack.source.X, hack.dest.X);
//        var maxY = Math.Max(hack.source.Y, hack.dest.Y);

//        var start = hack.source.X == minX ? hack.source : hack.dest;
//        var end = hack.source.X == minX ? hack.dest : hack.source;
//        var yOffset = start.Y - minY == 0 ? 1 : -1;
//        for (int y = start.Y + yOffset; yOffset > 0 ? y <= end.Y : y >= end.Y; y += yOffset)
//            yield return new Coordinate(start.X, y);
//        for (int x = minX + 1; x < maxX; x++)
//            yield return new Coordinate(x, end.Y);
//    }

//    internal static bool HasWallOnlyRoute(this char[][] map, Coordinate source, Coordinate dest, Dictionary<Coordinate, int> localCache = null, int steps = 0)
//    {
//        if (steps >= 20)
//            return false;

//        var visited = localCache ?? new Dictionary<Coordinate, int> { [source] = 0 };

//        foreach (var neighbour in source.Neighbours().Where(x => map.InBounds(x) && (!visited.TryGetValue(x, out var prevSteps) || steps < prevSteps)))
//        {
//            if (neighbour == dest)
//                return true;

//            if (map[neighbour.Y][neighbour.X] == '#')
//            {
//                visited[neighbour] = steps + 1;

//                if (map.HasWallOnlyRoute(neighbour, dest, visited, steps + 1))
//                {
//                    return true;
//                }
//            }
//        }


//        //    //var xDiff = dest.X - source.X;
//        //    //var yDiff = dest.Y - source.Y;
//        //    //if (Math.Abs(xDiff) + Math.Abs(yDiff) == 1)
//        //    //    return true;
//        //    //if (xDiff > 0)
//        //    //    if (map[source.Y][source.X + 1] == '#' && map.HasWallOnlyRoute(new Coordinate(source.X + 1, source.Y), dest))
//        //    //        return true;
//        //    //if (xDiff < 0)
//        //    //    if (map[source.Y][source.X - 1] == '#' && map.HasWallOnlyRoute(new Coordinate(source.X - 1, source.Y), dest))
//        //    //        return true;
//        //    //if (yDiff > 0)
//        //    //    if (map[source.Y + 1][source.X] == '#' && map.HasWallOnlyRoute(new Coordinate(source.X, source.Y + 1), dest))
//        //    //        return true;
//        //    //if (yDiff < 0)
//        //    //    if (map[source.Y - 1][source.X] == '#' && map.HasWallOnlyRoute(new Coordinate(source.X, source.Y - 1), dest))
//        //    //        return true;

//        return false;
//    }

//    internal static bool HasWallRoute(this char[][] map, Coordinate source, Coordinate dest)
//    {
//        var xDiff = dest.X - source.X;
//        var yDiff = dest.Y - source.Y;

//        if (xDiff > 0 && map[source.Y][source.X + 1] == '#')
//            return true;
//        if (xDiff < 0 && map[source.Y][source.X - 1] == '#')
//            return true;
//        if (yDiff > 0 && map[source.Y + 1][source.X] == '#')
//            return true;
//        if (yDiff < 0 && map[source.Y - 1][source.X] == '#')
//            return true;

//        return false;
//    }

//    //internal static bool HasWallOnlyRoute(this char[][] map, Coordinate source, Coordinate dest)
//    //{
//    //    var xDiff = dest.X - source.X;
//    //    var yDiff = dest.Y - source.Y;
//    //    if (Math.Abs(xDiff) + Math.Abs(yDiff) == 1)
//    //        return true;
//    //    if (xDiff > 0)
//    //        if (map[source.Y][source.X + 1] == '#' && map.HasWallOnlyRoute(new Coordinate(source.X + 1, source.Y), dest))
//    //            return true;
//    //    if (xDiff< 0)
//    //        if (map[source.Y][source.X - 1] == '#' && map.HasWallOnlyRoute(new Coordinate(source.X - 1, source.Y), dest))
//    //            return true;
//    //    if (yDiff > 0)
//    //        if (map[source.Y + 1][source.X] == '#' && map.HasWallOnlyRoute(new Coordinate(source.X, source.Y + 1), dest))
//    //            return true;
//    //    if (yDiff< 0)
//    //        if (map[source.Y - 1][source.X] == '#' && map.HasWallOnlyRoute(new Coordinate(source.X, source.Y - 1), dest))
//    //            return true;
//    //    return false;
//    //}

//    //internal static int HasWallOnlyRoute(this char[][] map, Coordinate source, Coordinate dest, Dictionary<(Coordinate from, Coordinate to), int> wallRouteCache, Dictionary<Coordinate, int> localCache = null, int steps = 0)
//    //{
//    //    if (wallRouteCache.TryGetValue((source, dest), out var cacheSteps))
//    //    {
//    //        var totalSteps = cacheSteps + steps;
//    //        if (totalSteps <= 20)
//    //            return totalSteps;
//    //        else
//    //            return 1000;
//    //    }

//    //    if (steps >= 20)
//    //        return 1000;

//    //    var visited = localCache ?? new Dictionary<Coordinate, int> { [source] = 0 };

//    //    foreach (var neighbour in source.Neighbours().Where(x => map.InBounds(x) /*&& (!visited.TryGetValue(x, out var prevSteps) || steps < prevSteps)*/))
//    //    {
//    //        if (neighbour == dest)
//    //        {
//    //            wallRouteCache[(source, dest)] = 1;
//    //            wallRouteCache[(dest, source)] = 1;
//    //            return 1;
//    //        }

//    //        if (wallRouteCache.TryGetValue((neighbour, dest), out var innerSteps))
//    //        {
//    //            var totalSteps = innerSteps + steps;
//    //            if (totalSteps <= 20)
//    //                return totalSteps;
//    //            else
//    //                return 1000;

//    //        }

//    //        if (map[neighbour.Y][neighbour.X] == '#')
//    //        {
//    //            //visited[neighbour] = steps + 1;
//    //            var remainingSteps = map.HasWallOnlyRoute(neighbour, dest, wallRouteCache, visited, steps + 1);
//    //            var totalSteps = remainingSteps + steps + 1;

//    //            if (steps == 0)
//    //            {
//    //                wallRouteCache[(source, dest)] = totalSteps;
//    //                wallRouteCache[(dest, source)] = totalSteps;
//    //            }

//    //            if (totalSteps <= 20)
//    //            {
//    //                return totalSteps;
//    //            }
//    //        }
//    //    }


//    //    //    //var xDiff = dest.X - source.X;
//    //    //    //var yDiff = dest.Y - source.Y;
//    //    //    //if (Math.Abs(xDiff) + Math.Abs(yDiff) == 1)
//    //    //    //    return true;
//    //    //    //if (xDiff > 0)
//    //    //    //    if (map[source.Y][source.X + 1] == '#' && map.HasWallOnlyRoute(new Coordinate(source.X + 1, source.Y), dest))
//    //    //    //        return true;
//    //    //    //if (xDiff < 0)
//    //    //    //    if (map[source.Y][source.X - 1] == '#' && map.HasWallOnlyRoute(new Coordinate(source.X - 1, source.Y), dest))
//    //    //    //        return true;
//    //    //    //if (yDiff > 0)
//    //    //    //    if (map[source.Y + 1][source.X] == '#' && map.HasWallOnlyRoute(new Coordinate(source.X, source.Y + 1), dest))
//    //    //    //        return true;
//    //    //    //if (yDiff < 0)
//    //    //    //    if (map[source.Y - 1][source.X] == '#' && map.HasWallOnlyRoute(new Coordinate(source.X, source.Y - 1), dest))
//    //    //    //        return true;

//    //    if (steps == 0)
//    //    {
//    //        wallRouteCache[(source, dest)] = 1000;
//    //        wallRouteCache[(dest, source)] = 1000;
//    //    }
//    //    return 1000;
//    //}

//    public static char[][] CloneMap(this char[][] map)
//    {
//        return map.Select(row => row.ToArray()).ToArray();
//    }

//    public static char[][] ReverseMap(this char[][] map)
//    {
//        return map.Select(row => row.Select(x => x == '#' ? '.' : '#').ToArray()).ToArray();
//    }

//    public static void Print(this char[][] map, Coordinate[] path, Coordinate start, Coordinate goal)
//    {
//        for (var y = 0; y < map.Length; y++)
//        {
//            for (var x = 0; x < map[y].Length; x++)
//            {
//                var pos = new Coordinate(x, y);
//                var tmp = pos == start ? 'S' : pos == goal ? 'E' : path.Contains(pos) ? '*' : map[y][x];
//                if (tmp == '.')
//                    throw new InvalidOperationException();
//                Console.Write(tmp);
//            }
//            Console.WriteLine();
//        }
//    }

//    public static int WallDistance(this char[][] map, Coordinate source, Coordinate dest, Dictionary<(Coordinate from, Coordinate to), int> wallRouteCache)
//    {
//        if (wallRouteCache.TryGetValue((source, dest), out var cacheSteps))
//        {
//            return cacheSteps;
//        }

//        var cheatMap = map.ReverseMap();
//        cheatMap[source.Y][source.X] = '.';
//        cheatMap[dest.Y][dest.X] = '.';

//        var start = source;
//        var goal = dest;
//        var visited = new Dictionary<Coordinate, int>();
//        var jobs = new PriorityQueue<(Coordinate pos, List<Coordinate> path, int steps), int>();
//        jobs.Enqueue((start, [start], 0), 0);
//        while (jobs.Count > 0)
//        {
//            var (pos, path, steps) = jobs.Dequeue();

//            if (pos == goal)
//            {
//                for (int i = 0; i < steps; i++)
//                {
//                    wallRouteCache[(path[i], dest)] = steps - i;
//                    wallRouteCache[(dest, path[i])] = steps - i;

//                    //var hasValue = wallRouteCache.TryGetValue((path[i], dest), out var cachedList);
//                    //var list = hasValue ? cachedList : new List<int>();
//                    //list.Add(steps - i);
//                    //wallRouteCache[(path[i], dest)] = list;

//                    //hasValue = wallRouteCache.TryGetValue((dest, path[i]), out cachedList);
//                    //list = hasValue ? cachedList : new List<int>();
//                    //list.Add(steps - i);
//                    //wallRouteCache[(dest, path[i])] = list;
//                }
//                return steps; // wallRouteCache[(source, dest)];
//            }

//            //if (steps >= 20)
//            //    continue;

//            if (!visited.TryGetValue(pos, out var prevSteps) || steps < prevSteps)
//            {
//                visited[pos] = steps;

//                foreach (var candidate in pos.Neighbours().Where(x => cheatMap.Valid(x) && !path.Contains(x)))
//                {
//                    var newPath = new List<Coordinate>(path);
//                    newPath.Add(candidate);
//                    jobs.Enqueue((candidate, newPath, steps + 1), steps + 1 + candidate.Distance(goal));
//                }
//            }
//        }
//        return 1000;
//    }
//}