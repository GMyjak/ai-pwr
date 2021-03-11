using System.Collections.Generic;
using System.IO;

namespace Genetics
{
    abstract class ProblemLoader
    {
        public static Problem LoadProblemFromFile(string path)
        {
            // This method will assume that problem files are in correct format

            Problem result = new Problem();
            using var reader = new StreamReader(path);
            {
                var line = reader.ReadLine()?.Split(';');
                result.Dimensions = new Point(line[0], line[1]);

                result.ConnectedPoints = new List<PointPair>();
                line = reader.ReadLine()?.Split(';');
                while (line != null)
                {
                    Point a = new Point(line[0], line[1]);
                    Point b = new Point(line[2], line[3]);
                    result.ConnectedPoints.Add(new PointPair(a, b));
                    line = reader.ReadLine()?.Split(';');
                }
            }

            return result;
        }
    }
}
