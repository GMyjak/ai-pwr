using System;
using System.Collections.Generic;
using System.Linq;
using CSP.Abstract;

namespace CSP.MapColouring
{
    class MapColouringNaPale : AbstractConstraintSatisfactionProblem<int?>
    {
        // Kolory: 
        // 1 - czerwony
        // 2 - zielony
        // 3 - niebieski
        // 4 - żółty
        // 5 - fioletowy
        // 6 - pomarańczowy

        public List<int?> DomainTemplate { get; set; } = new List<int?>() {1, 2, 3, 4};
        public int X { get; set; }
        public int Y { get; set; }

        private List<Connection> connections;

        protected override void DefineVariables()
        {
            Random rng = new Random();
            connections = new List<Connection>();

            List<Point> cords = new List<Point>();
            for (int i = 0; i < X; i++)
            {
                for (int j = 0; j < Y; j++)
                {
                    cords.Add(new Point(i, j));
                }
            }

            List<Connection> connectionCandidates = new List<Connection>();
            for (int i = 0; i < cords.Count - 1; i++)
            {
                for (int j = i + 1; j < cords.Count; j++)
                {
                    Connection conn = new Connection(cords[i], cords[j]);
                    connectionCandidates.Add(conn);
                }
            }

            while (connectionCandidates.Count > 0)
            {
                List<Point> randomPointCandidates = new List<Point>();
                connectionCandidates.ForEach(c =>
                {
                    if (!randomPointCandidates.Contains(c.A))
                    {
                        randomPointCandidates.Add(c.A);
                    }
                    if (!randomPointCandidates.Contains(c.B))
                    {
                        randomPointCandidates.Add(c.B);
                    }
                });

                Point p = randomPointCandidates[rng.Next(randomPointCandidates.Count)];
                Connection choice = connectionCandidates
                    .Where(c => c.A == p || c.B == p)
                    .OrderBy(c => c.A.GetDistance(c.B))
                    .First();

                connections.Add(choice);
                connectionCandidates.Remove(choice);
                connectionCandidates = connectionCandidates
                    .Where(c => !Point.Crosses(choice.A, choice.B, c.A, c.B))
                    .ToList();
            }

            foreach (var connection in connections)
            {
                Console.WriteLine($"A:({connection.A.X},{connection.A.Y}), B:({connection.B.X},{connection.B.Y})");
            }

            Variables = new List<IVariable<int?>>();
            foreach (var point in cords)
            {
                Variables.Add(new Variable()
                {
                    Cords = point,
                    Domain = new List<int?>(DomainTemplate),
                    Current = null
                });
            }
        }

        protected override void DefineConstraints()
        {
            Constraints = new List<Constraint>();
            foreach (var connection in connections)
            {
                Constraints.Add(GenerateConstraint(connection));
            }
        }

        //public void GenerateInstance(int x, int y)
        //{
        //    for (int i = 0; i < 10000; i++)
        //    {
        //        Point randomPoint = cords[rng.Next(cords.Count)];
        //        List<Point> connectionCandidates = cords
        //            .OrderBy(c => rng.Next()).ToList();
        //        connectionCandidates = connectionCandidates
        //            .OrderBy(c => c.GetDistance(randomPoint)).ToList();

        //        for (int j = 1; j < connectionCandidates.Count; j++)
        //        {
        //            Connection candidateConnection = new Connection(randomPoint, connectionCandidates[j]);
        //            if (!connections.Any(c => Point.Crosses(candidateConnection.A, candidateConnection.B, c.A, c.B)))
        //            {
        //                connections.Add(candidateConnection);
        //                break;
        //            }
        //        }
        //    }

        //    // Generate variables
        //    variables = new List<Variable>();
        //    foreach (var point in cords)
        //    {
        //        variables.Add(new Variable()
        //        {
        //            Cords = point,
        //            Domain = new List<int>(DomainTemplate),
        //            Current = null
        //        });
        //    }

        //    // Generate constraints
        //    Constraints = new List<Constraint>();
        //    foreach (var connection in connections)
        //    {
        //        Constraints.Add(GenerateConstraint(connection, Variables));
        //    }
        //}

        private BinaryConstraint<int?> GenerateConstraint(Connection connection)
        {
            Variable first = Variables.First(v => ((Variable)v).Cords == connection.A) as Variable;
            Variable second = Variables.First(v => ((Variable)v).Cords == connection.B) as Variable;
            return new BinaryConstraint<int?>()
            {
                VariableA = first,
                VariableB = second,
                Check = () => first.Current == null || second.Current == null || first.Current.Value != second.Current.Value
            };
        }
    }

    class Variable : IVariable<int?>
    {
        public int? Current { get; set; }
        public List<int?> Domain { get; set; }
        
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
    }
}
