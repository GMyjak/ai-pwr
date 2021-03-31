using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Point = Genetics.StructureDefinitions.Point;

namespace Genetics
{
    class Picture
    {
        private Bitmap bitmap;
        private Individual individual;
        private int cellSize = 4;
        private static List<Pen> pens = new List<Pen>()
        {
            Pens.DarkGreen, Pens.Aqua, Pens.Blue, 
            Pens.BlueViolet, Pens.Chartreuse, Pens.Brown, 
            Pens.Coral, Pens.Crimson, Pens.DarkOrange, 
            Pens.Indigo, Pens.Sienna, Pens.Yellow
        };

        public Picture(Individual individual)
        {
            this.individual = individual;
        }

        public void Draw()
        {
            bitmap = new Bitmap(individual.Problem.Dimensions.X * cellSize + cellSize, 
                individual.Problem.Dimensions.Y * cellSize + cellSize);

            var graphic = Graphics.FromImage(bitmap);
            graphic.FillRectangle(Brushes.AliceBlue, 0, 0, bitmap.Width, bitmap.Height);

            
            for (int i=0; i<individual.Paths.Count; i++)
            {
                var path = individual.Paths[i];
                Point lastPoint = individual.Problem.ConnectedPoints[i].A;
                lastPoint.X *= cellSize;
                lastPoint.X += cellSize;
                lastPoint.Y *= cellSize;
                lastPoint.Y += cellSize;
                foreach (var segment in path.Segments)
                {
                    Point nextPoint = lastPoint.Append(segment.Length * cellSize, segment.Direction);
                    graphic.DrawLine(pens[i], lastPoint.X, lastPoint.Y, nextPoint.X, nextPoint.Y);
                    lastPoint = nextPoint;
                }
            }

            graphic.Dispose();
        }

        public void Save(string path)
        {
            bitmap.Save(path, ImageFormat.Bmp);
        }
    }
}
