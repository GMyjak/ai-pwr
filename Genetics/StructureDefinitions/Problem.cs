using System.Collections.Generic;

namespace Genetics.StructureDefinitions
{
    public class Problem
    {
        public Point Dimensions { get; set; }
        public List<PointPair> ConnectedPoints { get; set; }
    }
}
