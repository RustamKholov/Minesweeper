using Minesweeper.Domain.Entities;

namespace Minesweeper.Domain.Logic.MineGeneration
{
    public sealed class NoGuessSolver
    {
        private const int MaxBacktrackComponentSize = 20;

        private Cell[,] _grid = null!;
        private int _rows;
        private int _cols;
        private int _totalMines;
        private HashSet<(int Row, int Col)> _deducedMines = null!;

        public SolveResult Solve(HashSet<(int Row, int Col)> mines, int rows, int cols, int totalMines, int safeRow, int safeCol)
        {
            _rows = rows;
            _cols = cols;
            _totalMines = totalMines;
            _grid = GridFactory.CreateBoundGrid(rows, cols);
            foreach (var (row, col) in mines)
            {
                _grid[row, col].IsMine = true;
            }
            _deducedMines = new HashSet<(int Row, int Col)>();

            SimulateReveal(safeRow, safeCol);

            bool progress = true;
            while (progress)
            {
                progress = RunConstraintPropagationPass();
                if (!progress)
                {
                    progress = RunBoundedBacktrackingPass();
                }
            }

            var revealedCells = new HashSet<(int Row, int Col)>();
            var unresolvedFrontier = new HashSet<(int Row, int Col)>();
            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _cols; col++)
                {
                    Cell cell = _grid[row, col];
                    var coord = (row, col);
                    if (cell.IsRevealed)
                    {
                        revealedCells.Add(coord);
                    }
                    else if (!_deducedMines.Contains(coord))
                    {
                        unresolvedFrontier.Add(coord);
                    }
                }
            }

            return new SolveResult
            {
                FullySolved = unresolvedFrontier.Count == 0,
                UnresolvedFrontier = unresolvedFrontier,
                RevealedCells = revealedCells,
            };
        }

        private void SimulateReveal(int row, int col)
        {
            Cell cell = _grid[row, col];
            if (cell.IsRevealed) return;
            cell.IsRevealed = true;

            if (cell.AdjacentMines == 0)
            {
                foreach (Cell neighbor in cell.AdjacentCells)
                {
                    if (!neighbor.IsRevealed)
                    {
                        SimulateReveal(neighbor.Row, neighbor.Col);
                    }
                }
            }
        }

        private sealed record Constraint(HashSet<(int Row, int Col)> UnknownNeighbors, int RequiredMines, bool IsGlobal);

        // Every revealed numbered cell contributes a constraint over its unrevealed neighbors.
        // One extra synthetic constraint covers every other unknown cell against the remaining
        // mine count, so the same subset-elimination logic below also captures global deductions
        // (e.g. "no mines left, everything else is safe") without a separate algorithm.
        private List<Constraint> BuildConstraints()
        {
            var constraints = new List<Constraint>();
            var allUnknown = new HashSet<(int Row, int Col)>();

            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _cols; col++)
                {
                    if (!_grid[row, col].IsRevealed && !_deducedMines.Contains((row, col)))
                    {
                        allUnknown.Add((row, col));
                    }
                }
            }

            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _cols; col++)
                {
                    Cell cell = _grid[row, col];
                    if (!cell.IsRevealed || cell.IsMine || cell.AdjacentMines == 0) continue;

                    var unknownNeighbors = new HashSet<(int Row, int Col)>();
                    int knownMineNeighbors = 0;
                    foreach (Cell neighbor in cell.AdjacentCells)
                    {
                        if (neighbor.IsRevealed) continue;
                        var coord = (neighbor.Row, neighbor.Col);
                        if (_deducedMines.Contains(coord))
                        {
                            knownMineNeighbors++;
                        }
                        else
                        {
                            unknownNeighbors.Add(coord);
                        }
                    }

                    if (unknownNeighbors.Count > 0)
                    {
                        constraints.Add(new Constraint(unknownNeighbors, cell.AdjacentMines - knownMineNeighbors, false));
                    }
                }
            }

            if (allUnknown.Count > 0)
            {
                constraints.Add(new Constraint(allUnknown, _totalMines - _deducedMines.Count, true));
            }

            return constraints;
        }

        private bool RunConstraintPropagationPass()
        {
            var constraints = BuildConstraints();
            bool progress = false;

            foreach (Constraint constraint in constraints)
            {
                if (ResolveConstraint(constraint.UnknownNeighbors, constraint.RequiredMines))
                {
                    progress = true;
                }
            }

            for (int i = 0; i < constraints.Count; i++)
            {
                for (int j = 0; j < constraints.Count; j++)
                {
                    if (i == j) continue;
                    Constraint a = constraints[i];
                    Constraint b = constraints[j];
                    if (a.UnknownNeighbors.Count >= b.UnknownNeighbors.Count) continue;
                    if (!a.UnknownNeighbors.IsSubsetOf(b.UnknownNeighbors)) continue;

                    var diffCells = new HashSet<(int Row, int Col)>(b.UnknownNeighbors);
                    diffCells.ExceptWith(a.UnknownNeighbors);
                    int diffMines = b.RequiredMines - a.RequiredMines;

                    if (ResolveConstraint(diffCells, diffMines))
                    {
                        progress = true;
                    }
                }
            }

            return progress;
        }

        private bool ResolveConstraint(HashSet<(int Row, int Col)> unknownNeighbors, int requiredMines)
        {
            if (unknownNeighbors.Count == 0) return false;
            bool progress = false;

            if (requiredMines == 0)
            {
                foreach (var (row, col) in unknownNeighbors)
                {
                    if (!_grid[row, col].IsRevealed)
                    {
                        SimulateReveal(row, col);
                        progress = true;
                    }
                }
            }
            else if (requiredMines == unknownNeighbors.Count)
            {
                foreach (var coord in unknownNeighbors)
                {
                    if (_deducedMines.Add(coord))
                    {
                        progress = true;
                    }
                }
            }

            return progress;
        }

        // Only invoked once constraint propagation stalls. Frontier cells are grouped into
        // connected components (cells sharing a revealed numbered neighbor); each component up
        // to MaxBacktrackComponentSize is solved exactly. Larger components are left unresolved
        // this iteration — a deliberate, bounded cost cap rather than exhaustive search on the
        // whole frontier.
        private bool RunBoundedBacktrackingPass()
        {
            var localConstraints = BuildConstraints().Where(c => !c.IsGlobal && c.UnknownNeighbors.Count > 0).ToList();

            var frontierCells = new HashSet<(int Row, int Col)>();
            foreach (var constraint in localConstraints)
            {
                frontierCells.UnionWith(constraint.UnknownNeighbors);
            }

            var adjacency = new Dictionary<(int Row, int Col), HashSet<(int Row, int Col)>>();
            foreach (var cell in frontierCells)
            {
                adjacency[cell] = new HashSet<(int Row, int Col)>();
            }
            foreach (var constraint in localConstraints)
            {
                foreach (var cellA in constraint.UnknownNeighbors)
                {
                    foreach (var cellB in constraint.UnknownNeighbors)
                    {
                        if (!cellA.Equals(cellB))
                        {
                            adjacency[cellA].Add(cellB);
                        }
                    }
                }
            }

            var visited = new HashSet<(int Row, int Col)>();
            bool progress = false;

            foreach (var start in frontierCells)
            {
                if (visited.Contains(start)) continue;

                var component = new HashSet<(int Row, int Col)>();
                var queue = new Queue<(int Row, int Col)>();
                queue.Enqueue(start);
                visited.Add(start);
                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();
                    component.Add(current);
                    foreach (var neighbor in adjacency[current])
                    {
                        if (visited.Add(neighbor))
                        {
                            queue.Enqueue(neighbor);
                        }
                    }
                }

                if (component.Count > MaxBacktrackComponentSize) continue;

                var componentConstraints = localConstraints
                    .Where(c => c.UnknownNeighbors.IsSubsetOf(component))
                    .ToList();

                if (SolveComponentExact(component, componentConstraints))
                {
                    progress = true;
                }
            }

            return progress;
        }

        private bool SolveComponentExact(HashSet<(int Row, int Col)> component, List<Constraint> constraints)
        {
            if (constraints.Count == 0) return false;

            var cells = component
                .OrderByDescending(cell => constraints.Count(c => c.UnknownNeighbors.Contains(cell)))
                .ToList();

            var isMineAssignment = new bool[cells.Count];
            var everMine = new bool[cells.Count];
            var everSafe = new bool[cells.Count];
            int completions = 0;

            void Backtrack(int index)
            {
                if (index == cells.Count)
                {
                    completions++;
                    for (int i = 0; i < cells.Count; i++)
                    {
                        if (isMineAssignment[i]) everMine[i] = true;
                        else everSafe[i] = true;
                    }
                    return;
                }

                isMineAssignment[index] = true;
                if (IsPartiallyConsistent(constraints, cells, isMineAssignment, index))
                {
                    Backtrack(index + 1);
                }

                isMineAssignment[index] = false;
                if (IsPartiallyConsistent(constraints, cells, isMineAssignment, index))
                {
                    Backtrack(index + 1);
                }
            }

            Backtrack(0);

            if (completions == 0)
            {
                throw new InvalidOperationException("Solver found no valid mine assignment for a component of a real board.");
            }

            bool progress = false;
            for (int i = 0; i < cells.Count; i++)
            {
                var (row, col) = cells[i];
                if (everMine[i] && !everSafe[i])
                {
                    if (_deducedMines.Add((row, col))) progress = true;
                }
                else if (everSafe[i] && !everMine[i])
                {
                    if (!_grid[row, col].IsRevealed)
                    {
                        SimulateReveal(row, col);
                        progress = true;
                    }
                }
            }

            return progress;
        }

        private static bool IsPartiallyConsistent(List<Constraint> constraints, List<(int Row, int Col)> cells, bool[] isMineAssignment, int assignedUpTo)
        {
            foreach (var constraint in constraints)
            {
                int assignedMineCount = 0;
                int assignedCount = 0;
                for (int i = 0; i <= assignedUpTo; i++)
                {
                    if (constraint.UnknownNeighbors.Contains(cells[i]))
                    {
                        assignedCount++;
                        if (isMineAssignment[i]) assignedMineCount++;
                    }
                }

                int unassignedInConstraint = constraint.UnknownNeighbors.Count - assignedCount;
                if (assignedMineCount > constraint.RequiredMines) return false;
                if (assignedMineCount + unassignedInConstraint < constraint.RequiredMines) return false;
            }

            return true;
        }
    }
}
