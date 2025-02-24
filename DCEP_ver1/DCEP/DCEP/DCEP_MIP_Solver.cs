using System;
using System.Collections.Generic;
using Google.OrTools.LinearSolver;

namespace DCEP // Wersja działająca poprawnie
{
    public class DCEP_MIP_Solver
    {
        public void Solve(Graph graph)
        {
            // Tworzenie solvera
            Solver solver = Solver.CreateSolver("SCIP");

            if (solver == null)
            {
                Console.WriteLine("Solver not available.");
                return;
            }

            // Zmienne decyzyjne dla krawędzi
            Dictionary<Edge, Variable> edgeVariables = new Dictionary<Edge, Variable>();
            foreach (var edge in graph.Edges)
            {
                edgeVariables[edge] = solver.MakeBoolVar($"x_{edge.Start.Id}_{edge.End.Id}");
            }

            Dictionary<Vertex, Variable> orderVariables = new Dictionary<Vertex, Variable>();
            double W = graph.Edges.Sum(edge => edge.Weight); // W jako suma maksymalnych wag krawędzi
            // Zmienne pomocnicze dla numeracji wierzchołków w ścieżce (y_v)
            foreach (var vertex in graph.Vertices)
            {
                orderVariables[vertex] = solver.MakeVar(0, double.PositiveInfinity, false, $"y_{vertex.Id}");
            }
            // Zmienne pomocnicze dla zarządzania ograniczeniami odległości
            Dictionary<DistanceConstraint, Variable> distanceVariables = new Dictionary<DistanceConstraint, Variable>();
            foreach (var constraint in graph.Constraints)
            {
                distanceVariables[constraint] = solver.MakeBoolVar($"g_{constraint.Start.Id}_{constraint.End.Id}");
            }

            // Ograniczenia przepływu dla każdego wierzchołka
            foreach (var vertex in graph.Vertices)
            {
                if (vertex.Id == -1) // Źródło 's'
                {
                    Constraint startConstraint = solver.MakeConstraint(1, 1, "start");
                    foreach (var edge in vertex.OutgoingEdges)
                    {
                        startConstraint.SetCoefficient(edgeVariables[edge], 1);
                    }
                }
                else if (vertex.Id == -2) // Cel 't'
                {
                    Constraint endConstraint = solver.MakeConstraint(-1, -1, "end");
                    foreach (var edge in vertex.IncomingEdges)
                    {
                        endConstraint.SetCoefficient(edgeVariables[edge], -1);
                    }
                }
                else
                {
                    // Suma krawędzi wychodzących = suma krawędzi wchodzących
                    Constraint firstDCEP = solver.MakeConstraint(0, 1, $"DCEPFirst_flow_{vertex.Id}");
                    Constraint flowConstraint = solver.MakeConstraint(0, 0, $"flow_{vertex.Id}");
                    foreach (var edge in vertex.OutgoingEdges)
                    {
                        firstDCEP.SetCoefficient(edgeVariables[edge], 1);
                        flowConstraint.SetCoefficient(edgeVariables[edge], 1);
                    }
                    foreach (var edge in vertex.IncomingEdges)
                    {
                        flowConstraint.SetCoefficient(edgeVariables[edge], -1);
                    }
                }
            }
            // Ograniczenia MTZ - eliminacja pod-cykli
            foreach (var edge in graph.Edges) // Wzór 4
            {
                if (edge.Start.Id != -1 && edge.End.Id != -2) // Pomijamy krawędzie sztuczne
                {
                    Constraint mtzConstraint = solver.MakeConstraint(-W, double.PositiveInfinity, $"mtz_{edge.Start.Id}_{edge.End.Id}");
                    mtzConstraint.SetCoefficient(orderVariables[edge.End], -1); // edge.End=v
                    mtzConstraint.SetCoefficient(orderVariables[edge.Start], 1); // edge.Start=u
                    mtzConstraint.SetCoefficient(edgeVariables[edge], -(edge.Weight + W));
                }
            }

            foreach (var vertex in graph.Vertices) // Wzór 5
            {
                Constraint mtzConstraint = solver.MakeConstraint(0, double.PositiveInfinity, $"mtz_{vertex.Id}_{vertex.Id}");
                mtzConstraint.SetCoefficient(orderVariables[vertex], -1);
                foreach (var edge in vertex.IncomingEdges)
                {
                    mtzConstraint.SetCoefficient(edgeVariables[edge], W);
                }
            }

            // Wzór 6 oraz 7

            foreach (var constraint in graph.Constraints)
            {
                var startVertex = constraint.Start;
                var endVertex = constraint.End; 
                // wzór 6
                Constraint gConstraint1 = solver.MakeConstraint(0, double.PositiveInfinity, $"g_constraint1_{startVertex.Id}_{endVertex.Id}");
                gConstraint1.SetCoefficient(distanceVariables[constraint], -1);
                gConstraint1.SetCoefficient(orderVariables[startVertex], 1);

                Constraint gConstraint2 = solver.MakeConstraint(0, double.PositiveInfinity, $"g_constraint2_{startVertex.Id}_{endVertex.Id}");
                gConstraint2.SetCoefficient(distanceVariables[constraint], -1);
                gConstraint2.SetCoefficient(orderVariables[endVertex], 1);
                
                // wzór 7
                Constraint constraint7left = solver.MakeConstraint(-W, double.PositiveInfinity, $"7Left_constraint_{startVertex.Id}_{endVertex.Id}");
                constraint7left.SetCoefficient(orderVariables[startVertex], -1);
                constraint7left.SetCoefficient(orderVariables[endVertex], 1);
                constraint7left.SetCoefficient(distanceVariables[constraint], constraint.MaximumDistance - W);

                Constraint constraint7right = solver.MakeConstraint(-W, double.PositiveInfinity, $"7Right_constraint_{startVertex.Id}_{endVertex.Id}");
                constraint7right.SetCoefficient(orderVariables[startVertex], 1);
                constraint7right.SetCoefficient(orderVariables[endVertex], -1);
                constraint7right.SetCoefficient(distanceVariables[constraint], -(constraint.MinimumDistance + W));
            }

            // Funkcja celu - maksymalizacja liczby spełnionych ograniczeń odległości
            Objective objective = solver.Objective();
            foreach (var constraint in graph.Constraints)
            {
                objective.SetCoefficient(distanceVariables[constraint], W);
            }
            foreach (var vertex in graph.Vertices)
            {
                objective.SetCoefficient(orderVariables[vertex], 1);
            }
            objective.SetMaximization();

            // Rozwiązanie problemu
            Solver.ResultStatus resultStatus = solver.Solve();

            // Sprawdzanie wyników
            if (resultStatus == Solver.ResultStatus.OPTIMAL)
            {
                Console.WriteLine("Znaleziono optymalne rozwiązanie! Optimal path found!");
                foreach (var edge in graph.Edges)
                {
                    if (edgeVariables[edge].SolutionValue() == 1)
                    {
                        Console.WriteLine($"Edge from {edge.Start.Id} to {edge.End.Id} with weight {edge.Weight}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Nie znaleziono optymalnego rozwiązania. Optimal path not found.");
            }
        }
    }
}
