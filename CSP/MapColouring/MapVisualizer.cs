using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace CSP.MapColouring
{
    class MapVisualizer
    {
        private Bitmap bitmap;
        private int cellSize = 36;

        private List<Variable> variables;
        private List<Connection> connections;
        private int x;
        private int y;


        private static List<Pen> pens = new List<Pen>()
        {
            Pens.Crimson, Pens.DarkGreen, Pens.Blue,
            Pens.Yellow, Pens.BlueViolet, Pens.DarkOrange
        };

        private static List<Brush> brushes = new List<Brush>()
        {
            Brushes.Crimson, Brushes.DarkGreen, Brushes.Blue,
            Brushes.Yellow, Brushes.BlueViolet, Brushes.DarkOrange,
        };

        private static Pen black = Pens.Black;

        public MapVisualizer(List<Variable> variables, List<Connection> connections, int xSize, int ySize)
        {
            this.variables = variables;
            this.connections = connections;
            x = xSize;
            y = ySize;
        }

        public void Draw()
        {
            bitmap = new Bitmap(x * cellSize + cellSize, y * cellSize + cellSize);

            var graphic = Graphics.FromImage(bitmap);
            graphic.FillRectangle(Brushes.AliceBlue, 0, 0, bitmap.Width, bitmap.Height);

            foreach (var connection in connections)
            {
                graphic.DrawLine(black, connection.A.X * cellSize + cellSize, 
                    connection.A.Y * cellSize + cellSize, 
                    connection.B.X * cellSize + cellSize, 
                    connection.B.Y * cellSize + cellSize);
            }

            foreach (var variable in variables)
            {
                Brush color;
                if (variable.Current == 0)
                {
                    color = Brushes.Black;
                }
                else
                {
                    color = brushes[variable.Current.Value - 1];
                }

                graphic.FillEllipse(color, variable.Cords.X * cellSize + cellSize - 4, 
                    variable.Cords.Y * cellSize + cellSize - 4, 7, 7);
            }

            graphic.Dispose();
        }

        public void Save(string path)
        {
            if (bitmap == null)
            {
                Draw();
            }
            bitmap.Save(path, ImageFormat.Bmp);
        }
    }
}
