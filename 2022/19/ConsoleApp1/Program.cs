using System.Text.RegularExpressions;
using static ConsoleApp1.Program;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Parse input
            var blueprints = new List<Blueprint>();
            foreach (var line in File.ReadAllLines(@"c:\data\aoc\19\aoc19-1.txt"))
            {
                var blueprint = new Blueprint();
                var matches = Regex.Matches(line, @"Each (.*?) robot costs (.*?)\.");
                foreach (Match match in matches)
                    blueprint.Recipees.Add(match.Groups[1].Value.ToMaterials(), match.Groups[2].Value.Split(" and ").Select(mat => new Price(mat)));
                blueprints.Add(blueprint);
            }

            //Puzzle 1
            //const int TotalTime = 24;

            //Puzzle 2
            const int TotalTime = 32;
            blueprints = blueprints.Take(3).ToList();

            //Find optimal order
            var blueprintId = 0;
            var totalQuality = 0;
            var bestScores = new Dictionary<int, int>();
            foreach (var blueprint in blueprints)
            {
                blueprintId++;
                Console.WriteLine($"Processing blueprint #{blueprintId}:");
                int bestCollected = 0;
                var activeJobs = new Stack<Job>();
                activeJobs.Push(new Job { ActivePlan = new Plan(blueprint), nextRobotToBuild = Materials.Ore });
                activeJobs.Push(new Job { ActivePlan = new Plan(blueprint), nextRobotToBuild = Materials.Clay });

                while (activeJobs.Any())
                {
                    var currentJob = activeJobs.Pop();
                    var currentPlan = currentJob.ActivePlan;
                    
                    Materials robotToBuild = Materials.Undefined;
                    while (robotToBuild == Materials.Undefined && currentJob.TimeSpent < TotalTime)
                    {
                        //Abort if it's not theoretically possible to beat the current best
                        if (currentPlan.CalculateMaxGeodesIgnoringOre(TotalTime - currentJob.TimeSpent) <= bestCollected)
                            break;

                        //Always prioritize Geode if possible
                        if (currentPlan.CanBuild(Materials.Geode))
                            robotToBuild = Materials.Geode;
                        else
                        {
                            if (currentPlan.CanBuild(currentJob.nextRobotToBuild))
                                robotToBuild = currentJob.nextRobotToBuild;
                        }

                        //Harvest
                        currentPlan.Harvest();
                        currentJob.TimeSpent++;

                        //Build - if the robot is built earlier, we'll harvest too much
                        if (robotToBuild != Materials.Undefined)
                        {
                            currentPlan.Build(robotToBuild);
                            //Spawn child jobs
                            if (currentJob.TimeSpent < TotalTime)
                            {
                                activeJobs.Push(new Job { ActivePlan = new Plan(currentPlan, blueprint), TimeSpent = currentJob.TimeSpent, nextRobotToBuild = Materials.Ore });
                                if (currentPlan.Robots[Materials.Clay] > 0)
                                    activeJobs.Push(new Job { ActivePlan = new Plan(currentPlan, blueprint), TimeSpent = currentJob.TimeSpent, nextRobotToBuild = Materials.Obsidian });
                                activeJobs.Push(new Job { ActivePlan = new Plan(currentPlan, blueprint), TimeSpent = currentJob.TimeSpent, nextRobotToBuild = Materials.Clay });
                            }
                        }
                    }

                    //Store if better
                    if (currentJob.TimeSpent == TotalTime && currentPlan.Mats[Materials.Geode] > bestCollected)
                    {
                        bestCollected = currentPlan.Mats[Materials.Geode];
                        Console.WriteLine($"Found new best: {bestCollected}");
                    }
                }

                bestScores.Add(blueprintId, bestCollected);
                Console.WriteLine($"Round: {blueprintId}, Collected: {bestCollected}");
            }

            //Puzzle 2
            int product = 1;
            foreach (var score in bestScores)
                product *= score.Value;
            Console.WriteLine($"Product of 3 blueprints: {product}");

            //Puzzle 1
            //totalQuality = bestScores.Sum(s => s.Key * s.Value);
            //Console.WriteLine($"Quality level of all blueprints: {totalQuality}");

            Console.ReadLine();
        }

        internal enum Materials { Undefined, Ore, Clay, Obsidian, Geode }
        internal struct Price
        {
            public Materials Material;
            public int Amount;

            public Price(string materialCost)
            {
                var parts = materialCost.Split(' ');
                Amount = int.Parse(parts[0]);
                Material = parts[1].ToMaterials();
            }
        }
        
        internal class Blueprint
        {
            public Dictionary<Materials, IEnumerable<Price>> Recipees = new Dictionary<Materials, IEnumerable<Price>>();
        }

        internal class Plan
        {
            public Dictionary<Materials, int> Robots= new Dictionary<Materials, int>();
            public Dictionary<Materials, int> Mats = new Dictionary<Materials, int>();

            private Blueprint blueprint;
            public Plan(Blueprint blueprint) 
            { 
                this.blueprint = blueprint;
                Robots.Add(Materials.Ore, 1);
                Robots.Add(Materials.Clay, 0);
                Robots.Add(Materials.Obsidian, 0);
                Robots.Add(Materials.Geode, 0);
                Mats.Add(Materials.Ore, 0);
                Mats.Add(Materials.Clay, 0);
                Mats.Add(Materials.Obsidian, 0);
                Mats.Add(Materials.Geode, 0);
            }

            public Plan(Plan source, Blueprint blueprint)
            {
                this.blueprint = blueprint;
                this.Robots = new Dictionary<Materials, int>(source.Robots);
                this.Mats = new Dictionary<Materials, int>(source.Mats);
            }

            public int NumberOfRobots => Robots.Sum(r => r.Value);

            public int CalculateMaxGeodesIgnoringOre(int timeRemaining)
            {
                var simulation = new Plan(this, this.blueprint);
                simulation.Mats[Materials.Ore] = 1000;
                for (int i = 0; i < timeRemaining; i++)
                {
                    foreach (var mat in new[] { Materials.Geode, Materials.Obsidian, Materials.Clay })
                        if (simulation.CanBuild(mat))
                        {
                            simulation.Build(mat);
                            break;
                        }
                    simulation.Harvest();
                }
                return simulation.Mats[Materials.Geode];
            }

            public bool CanBuild(Materials material)
            {
                foreach (var matCost in blueprint.Recipees[material])
                    if (Mats[matCost.Material] < matCost.Amount)
                        return false;

                return true;
            }

            public void Build(Materials material)
            {
                foreach (var matCost in blueprint.Recipees[material])
                    Mats[matCost.Material] -= matCost.Amount;

                Robots[material]++;
            }

            internal void Harvest()
            {
                foreach (var robot in Robots)
                    Mats[robot.Key] += robot.Value;
            }
        }
    }

    internal class Job
    {
        public Plan ActivePlan;
        public int TimeSpent = 0;
        public Materials nextRobotToBuild;
    }

    internal static class Extensions
    {
        public static Materials ToMaterials(this string source)
        {
            return source switch
            {
                "ore" => Materials.Ore,
                "clay" => Materials.Clay,
                "obsidian" => Materials.Obsidian,
                "geode" => Materials.Geode,
                _ => throw new InvalidOperationException("Invalid material")
            }; ;
        }
    }
}