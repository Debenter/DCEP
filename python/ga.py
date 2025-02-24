import numpy as np
import pygad
import sys
import argparse
import time
from typing import List, Tuple, Dict
from collections import OrderedDict

class PathFinder:
    def __init__(self, cache_size: int = 1000000):
        self.graph: Dict[int, Dict] = {}
        self.conditions: Dict[int, Dict[int, Tuple[float, float]]] = {}
        self.best_path: Tuple[int, ...] = ()
        self.best_fitness: float = -1.0
        self.fitness_cache = OrderedDict()
        self.cache_size = cache_size
        self.number_of_vertices = 0

    def perm2tuple(self, perm: List[int]) -> Tuple[int, ...]:
        v = perm[0]
        ind = 0;
        result = (v,)
        has_continuation = True
        while has_continuation and self.graph[v]["out"] is not {}:
            has_continuation = False
            for k in self.graph[v]["out"].keys():
                if k not in result:
                    has_continuation = True
                    break
            if has_continuation:
                while perm[ind] not in self.graph[v]["out"] or perm[ind] in result:
                    ind = (ind + 1) % self.number_of_vertices
                v = perm[ind]
                result = result + (v,)
        return result

    def add_edge(self, source: int, target: int, weight: float) -> None:
        if source not in self.graph:
            self.graph[source] = {"in": {}, "out": {}, "on_path": False}
        if target not in self.graph:
            self.graph[target] = {"in": {}, "out": {}, "on_path": False}
        self.graph[source]["out"][target] = weight
        self.graph[target]["in"][source] = weight
        self.number_of_vertices = len(self.graph)

    def add_condition(self, source: int, target: int, min_length: float, max_length: float) -> None:
        if source not in self.conditions:
            self.conditions[source] = {}
        self.conditions[source][target] = (min_length, max_length)

    def calculate_fitness(self, ga_instance, solution, solution_idx) -> float:
        perm = tuple(solution)
        if perm in self.fitness_cache:
            return self.fitness_cache[perm]
        path = self.perm2tuple(solution)
        fitness = len(path) / (self.number_of_vertices + 1)
        prefix_sum = [0]
        for i in range(1, len(path)):
            prefix_sum.append(prefix_sum[-1] + self.graph[path[i - 1]]["out"][path[i]])
        for i in range(len(path) - 1):
            for j in range(i + 1, len(path)):
                total_length = prefix_sum[j] - prefix_sum[i]
                condition = self.conditions.get(path[i], {}).get(path[j])
                if condition and condition[0] <= total_length <= condition[1]:
                    fitness += 1

        if len(self.fitness_cache) >= self.cache_size:
            self.fitness_cache.popitem(last=False)  
        self.fitness_cache[perm] = fitness

        return fitness

    def init(self, args) -> None:
        self.ga_instance = pygad.GA(
            num_generations=args.generations,                   # Liczba pokoleń
            num_parents_mating=5,                 # Liczba rodziców do krzyżowania
            fitness_func=self.calculate_fitness,  # Funkcja dopasowania
            sol_per_pop=args.pop_size,            # Liczba osobników w populacji
            num_genes=self.number_of_vertices,    # Liczba genów (miast)
            gene_type=int,                        # Typ genu: całkowity (indeksy miast)
            #gene_space=list(range(1, self.number_of_vertices + 1)), # Zaczyna od 1, nie od 0
            gene_space=list(range(self.number_of_vertices)), # Przestrzeń genów: indeksy miast
            allow_duplicate_genes=False,
            parent_selection_type="tournament",   # Selekcja rodziców
            crossover_type="two_points",          # Typ krzyżowania
            mutation_type="swap",                 # Typ mutacji
            mutation_probability=0.1,             # Prawdopodobieństwo mutacji
            suppress_warnings=True
        )

    def run(self) -> None:
        self.ga_instance.run()
    
    def read_input(self) -> None:
        input_data = sys.stdin.read().strip().splitlines()
        for line in input_data:
            if ' ' in line:
                values = line.split(' ')
                if len(values) == 3:
                    source, target, weight = map(int, values)
                    self.add_edge(source, target, weight)
                elif len(values) == 4:
                    source, target, min_length, max_length = map(int, values)
                    self.add_condition(source, target, min_length, max_length)

parser = argparse.ArgumentParser()
parser.add_argument("--iterations", type=int, default=30, help="Number of iterations to run the algorithm")
parser.add_argument("--generations", type=int, default=100, help="Number of generations in a single GA run")
parser.add_argument("--pop_size", type=int, default=200, help="Size of a GA population")
parser.add_argument("--input", type=str, required=True, help="Input file for graph data")
args = parser.parse_args()

finder = PathFinder()

with open(args.input, 'r') as f:
    sys.stdin = f
    finder.read_input()

start_time = time.time()  # Początek pomiaru czasu

for i in range(1, args.iterations + 1):
    iter_start = time.time()  # Czas rozpoczęcia iteracji
    finder.init(args)
    finder.run()
    iter_time = time.time() - iter_start  # Czas trwania iteracji
    print(f"After iteration {i}")
    solution, solution_fitness, solution_idx = finder.ga_instance.best_solution()
    if (solution_fitness > finder.best_fitness):
        print(f"    New best with fitness {solution_fitness}.")
        finder.best_fitness = solution_fitness
        finder.best_path = finder.perm2tuple(solution)
    else:
        print("    No better solution.")

end_time = time.time()  # Koniec pomiaru czasu
total_time = end_time - start_time
print(list(map(int, finder.best_path)))
print(f"{int(finder.best_fitness)} constraints satisfied.")
print(f"Total computation time: {total_time:.2f} seconds")
