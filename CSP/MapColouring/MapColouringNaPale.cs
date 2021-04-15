using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSP.MapColouring
{
    class MapColouringNaPale
    {
        // Kolory: 
        // 1 - czerwony
        // 2 - zielony
        // 3 - niebieski
        // 4 - żółty
        // 5 - fioletowy
        // 6 - pomarańczowy

        public List<int> DomainTemplate { get; set; } = new List<int>() {1, 2, 3, 4};

        private List<Func<bool>> constraints;
        private List<Variable> variables;
        private List<Connection> connections;

        public Action<List<Variable>> OnSolutionFound { get; set; } = (_) => { };

        public void GenerateInstance(int x, int y)
        {
            Random rng = new Random();
            connections = new List<Connection>();

            List<Point> cords = new List<Point>();
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    cords.Add(new Point(i, j));
                }
            }

            for (int i = 0; i < 10000; i++)
            {
                Point randomPoint = cords[rng.Next(cords.Count)];
                List<Point> connectionCandidates = cords
                    .OrderBy(c => rng.Next()).ToList();
                connectionCandidates = connectionCandidates
                    .OrderBy(c => c.GetDistance(randomPoint)).ToList();

                for (int j = 1; j < connectionCandidates.Count; j++)
                {
                    Connection candidateConnection = new Connection(randomPoint, connectionCandidates[j]);
                    if (!connections.Any(c => Point.Crosses(candidateConnection.A, candidateConnection.B, c.A, c.B)))
                    {
                        connections.Add(candidateConnection);
                        break;
                    }
                }
            }

            // DEBUG
            foreach (var connection in connections)
            {
                Console.WriteLine($"A: X{connection.A.X}Y{connection.A.Y}  B: X{connection.B.X}Y{connection.B.Y}");
            }

            // Generate variables
            variables = new List<Variable>();
            foreach (var point in cords)
            {
                variables.Add(new Variable()
                {
                    Cords = point,
                    Domain = new List<int>(DomainTemplate),
                    Current = null
                });
            }
            
            // Generate constraints
            constraints = new List<Func<bool>>();
            foreach (var connection in connections)
            {
                constraints.Add(GenerateConstraint(connection, variables));
            }
        }

        private Func<bool> GenerateConstraint(Connection connection, List<Variable> variables)
        {
            Variable first = variables.First(v => v.Cords == connection.A);
            Variable second = variables.First(v => v.Cords == connection.B);

            return () => first.Current == null || second.Current == null || first.Current.Value != second.Current.Value;
        }

        public void RunAlgo()
        {
            Console.WriteLine(constraints.Count);
            FindSolution(0);
        }

        private void FindSolution(int variableIndex)
        {
            if (variableIndex == variables.Count)
            {
                OnSolutionFound?.Invoke(variables);
                return;
            }

            foreach (var permutation in variables[variableIndex].Domain)
            {
                variables[variableIndex].Current = permutation;
                if (constraints.All(con => con.Invoke()))
                {
                    FindSolution(variableIndex + 1);
                }
                else
                {
                    variables[variableIndex].Current = null;
                }
            }
        }
    }

    class Variable // Country
    {
        public int? Current { get; set; }
        public List<int> Domain { get; set; }
        
        public Point Cords { get; set; }
    }

    class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public double GetDistance(Point other)
        {
            return Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2);
        }

        // Given three colinear points p, q, r, the function checks if
        // point q lies on line segment 'pr'
        private static bool OnSegment(Point p, Point q, Point r)
        {
            if (q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) &&
                q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y))
                return true;

            return false;
        }

        // To find orientation of ordered triplet (p, q, r).
        // The function returns following values
        // 0 --> p, q and r are colinear
        // 1 --> Clockwise
        // 2 --> Counterclockwise
        private static int Orientation(Point p, Point q, Point r)
        {
            // See https://www.geeksforgeeks.org/orientation-3-ordered-points/
            // for details of below formula.
            int val = (q.Y - p.Y) * (r.X - q.X) -
                      (q.X - p.X) * (r.Y - q.Y);

            if (val == 0) return 0; // colinear

            return (val > 0) ? 1 : 2; // clock or counterclock wise
        }

        // The main function that returns true if line segment 'p1q1'
        // and 'p2q2' intersect.
        public static bool Crosses(Point p1, Point q1, Point p2, Point q2)
        {
            // CUSTOM CODE: return true if intersection point is on line ends and lines don't have the same direction
            Point common = null;
            Point otherA = null, otherB = null;
            if (p1 == p2 || p1 == q2)
            {
                common = p1;
                otherA = q1;
                otherB = p1 == p2 ? q2 : p2;
            }
            if (q1 == p2 || q1 == q2)
            {
                common = q1;
                otherA = p1;
                otherB = q1 == p2 ? q2 : p2;
            }

            if (common != null)
            {
                Point deltaA = new Point(otherA.X - common.X, otherA.Y - common.Y);
                Point deltaB = new Point(otherB.X - common.X, otherB.Y - common.Y);

                if (deltaA.Y == 0 && deltaB.Y == 0)
                {
                    return deltaA.X * deltaB.X > 0 ? true : false;
                }
                else if (deltaA.Y == 0 || deltaB.Y == 0)
                {
                    return false;
                }
                else
                {
                    return ((float) deltaA.X / deltaA.Y) == ((float) deltaB.X / deltaB.Y);
                }
            }

            // Find the four orientations needed for general and
            // special cases
            int o1 = Orientation(p1, q1, p2);
            int o2 = Orientation(p1, q1, q2);
            int o3 = Orientation(p2, q2, p1);
            int o4 = Orientation(p2, q2, q1);

            // General case
            if (o1 != o2 && o3 != o4)
                return true;

            // Special Cases
            // p1, q1 and p2 are colinear and p2 lies on segment p1q1
            if (o1 == 0 && OnSegment(p1, p2, q1)) return true;

            // p1, q1 and q2 are colinear and q2 lies on segment p1q1
            if (o2 == 0 && OnSegment(p1, q2, q1)) return true;

            // p2, q2 and p1 are colinear and p1 lies on segment p2q2
            if (o3 == 0 && OnSegment(p2, p1, q2)) return true;

            // p2, q2 and q1 are colinear and q1 lies on segment p2q2
            if (o4 == 0 && OnSegment(p2, q1, q2)) return true;

            return false; // Doesn't fall in any of the above cases
        }
    }

    class Connection
    {
        public Point A { get; set; }
        public Point B { get; set; }

        public Connection(Point a, Point b)
        {
            A = a;
            B = b;
        }

        //public bool Crosses(Connection other)
        //{
        //    // https://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect
            
        //    // y = a1 * x + b1
        //    // y = a2 * x + b2
            
        //    // 1. Calculate a1, a2, b1, b2
        //    // A.Y = a1 * A.X + b1
        //    // B.Y = a1 * B.X + b1
        //    // b1 = A.Y - a1 * A.X
        //    // a1 * B.X = B.Y - A.Y + a1 * A.X
        //    // a1 * (B.X - A.X) = B.Y - A.Y
        //    // a1 = (B.Y - A.Y) / (B.X - A.X)

        //    float a1 = (B.Y - A.Y) / (B.X - A.X);
        //    float b1 = A.Y - a1 * A.X;
        //    float a2 = (other.B.Y - other.A.Y) / (other.B.X - other.A.X);
        //    float b2 = other.A.Y - a2 * other.A.X;

        //    // 2. Calculate
        //    // a1 * x - a2 * x = b2 - b1
        //    // x = (b2 - b1) / (a1 - a2)
        //    // y = a1 * x + b1
        //    float x = (b2 - b1) / (a1 - a2);
        //    float y = a1 * x + b1;
            

        //    return false;
        //}
    }
}
