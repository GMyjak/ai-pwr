using System.Collections.Generic;

namespace Genetics.StructureDefinitions
{
    public struct Path
    {
        public List<Segment> Segments { get; set; }

        public Path Copy()
        {
            Path copy = new Path()
            {
                Segments = new List<Segment>(this.Segments)
            };

            return copy;
        }
    }
}