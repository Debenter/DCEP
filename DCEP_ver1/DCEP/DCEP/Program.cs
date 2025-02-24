using System;
using Google.OrTools.LinearSolver;
using System.Collections.Generic;
using DCEP;
using System.Globalization;
using System.Diagnostics;

partial class DCEPsolver
{
    /// <summary>
    /// "msbuild DCEP.sln" w odpowiednim folderze lub kompilacja ręczna
    /// cd C:\GitHub\DCEP_CSHARP\DCEP_ver1\DCEP\DCEP\bin\Debug\net8.0
    /// DCEP.exe C:\GitHub\DCEP_CSHARP\graphs\testGraph.txt
    /// DCEP.exe C:\GitHub\DCEP_CSHARP\graphs\testing_group\graph_1.txt
    /// </summary>

    static void Main(string[] args)
    {
        // Wyświetlenie pomocy, jeśli brak argumentów
        if (args.Length == 0)
        {
            Console.WriteLine("Użycie:");
            Console.WriteLine("DCEPsolver.exe <ścieżka_do_pliku_grafu>");
            return;
        }

        string graphFile = args[0];

        if (!File.Exists(graphFile))
        {
            Console.WriteLine($"Plik grafu '{graphFile}' nie istnieje.");
            return;
        }

        try
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            GraphReader reader = new GraphReader();
            Graph graph = reader.ReadGraphFromFile(graphFile);
            graph.AddDummyVertices();

            DCEP_MIP_Solver solver = new DCEP_MIP_Solver();

            Console.WriteLine("Rozpoczynam badanie ścieżki...");
            solver.Solve(graph);

            stopwatch.Stop();

            Console.WriteLine($"\nCzas wykonania programu: {stopwatch.ElapsedMilliseconds} ms");

            var currentProcess = Process.GetCurrentProcess();
            Console.WriteLine($"Zużycie pamięci: {currentProcess.PrivateMemorySize64 / (1024 * 1024)} MB");
            Console.WriteLine($"Czas procesora: {currentProcess.TotalProcessorTime.TotalMilliseconds} ms");

            Console.WriteLine("Program zakończył działanie.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wystąpił błąd podczas działania programu: {ex.Message}");
        }
    }

    // Poniżej stara metoda gdy graf był w folderze kompilacji
    /*
    static void Main(string[] args)
    {
        GraphReader reader = new GraphReader();
        Graph graph = reader.ReadGraphFromFile("graph.txt");
        graph.AddDummyVertices();
        DCEP_MIP_Solver solver = new DCEP_MIP_Solver();
        solver.Solve(graph);

        //// Wyświetlenie krawędzi
        //Console.WriteLine("Edges:");
        //foreach (var edge in graph.Edges)
        //{
        //    Console.WriteLine($"Edge from {edge.Start.Id} to {edge.End.Id} with weight {edge.Weight}");
        //}

        //// Wyświetlenie ograniczeń odległościowych
        //Console.WriteLine("\nDistance Constraints:");
        //foreach (var constraint in graph.Constraints)
        //{
        //    Console.WriteLine($"Constraint between {constraint.Start.Id} and {constraint.End.Id}, distance between {constraint.MinimumDistance} and {constraint.MaximumDistance}");
        //}
        
}
    */

}
