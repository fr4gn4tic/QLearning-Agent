using System;
using System.Collections.Generic;
using System.Drawing;

namespace AntMaze
{
    public class Maze
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        private bool[,] walls;

        private bool[,] visited; // NEW: track visited cells
        public Point Position { get; set; }
        public Point Goal { get; private set; } // Zielposition

        // Weitere Eigenschaften und Methoden der Maze-Klasse

        public static Maze CreateRandomMaze(int width, int height)
        {
            Maze maze = new Maze();
            maze.Width = width;
            maze.Height = height;
            maze.walls = new bool[width, height];
            maze.visited = new bool[width, height]; // NEW: initialize visited array

            // Initialisiere alle Zellen als Wände
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    maze.walls[x, y] = true;
                }
            }

            // Starte den Recursive Backtracker-Algorithmus
            Random random = new Random();
            Stack<Point> stack = new Stack<Point>();
            Point startPosition = new Point(1, 1);
            maze.walls[startPosition.X, startPosition.Y] = false;
            maze.visited[startPosition.X, startPosition.Y] = false;
            stack.Push(startPosition);

            int[] dx = { 0, 1, 0, -1 };
            int[] dy = { -1, 0, 1, 0 };

            while (stack.Count > 0)
            {
                Point current = stack.Peek();
                List<int> directions = new List<int> { 0, 1, 2, 3 };
                bool moved = false;

                while (directions.Count > 0)
                {
                    int directionIndex = random.Next(directions.Count);
                    int direction = directions[directionIndex];
                    directions.RemoveAt(directionIndex);

                    int newX = current.X + 2 * dx[direction];
                    int newY = current.Y + 2 * dy[direction];

                    if (newX >= 0 && newX < width && newY >= 0 && newY < height && maze.walls[newX, newY])
                    {
                        maze.walls[newX - dx[direction], newY - dy[direction]] = false;
                        maze.walls[newX, newY] = false;
                        maze.visited[newX, newY] = false;
                        stack.Push(new Point(newX, newY));
                        moved = true;
                        break;
                    }
                }

                if (!moved)
                {
                    stack.Pop();
                }
            }
            // Setze ein zufälliges Ziel, das nicht auf einer Wand liegt
            int goalX, goalY;
            do
            {
                goalX = random.Next(0, width);
                goalY = random.Next(0, height);
            } while (maze.IsWallAt(goalX, goalY));

            maze.Goal = new Point(goalX, goalY);

            return maze;
        }

        public bool IsWallAt(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return false;
            }
            return walls[x, y];
        }


        public bool IsVisitedAt(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return false;
            }
            return visited[x, y];
        }

        public void MarkVisited(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                visited[x, y] = true;
            }
        }

        public Bitmap RenderMazeToBitmap()
        {
            int cellSize = 25; // Größe jeder Zelle in Pixeln
            int wallThickness = 2; // Dicke der Wände in Pixeln
            Color wallColor = Color.Black; // Farbe der Wände
            Color agentColor = Color.Red; // Farbe des Agenten
            Color goal = Color.Green;
            Bitmap bitmap = new Bitmap(Width * cellSize + wallThickness, Height * cellSize + wallThickness);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                // Zeichne die Wände des Labyrinths und die besuchten Zellen
                for (int y = 0; y < Height; y++)
                {
                    for (int x = 0; x < Width; x++)
                    {
                        if (IsWallAt(x, y))
                        {
                            g.FillRectangle(new SolidBrush(wallColor), x * cellSize, y * cellSize, cellSize + wallThickness, cellSize + wallThickness);
                        }
                        else if (IsVisitedAt(x, y))
                        {
                            g.FillRectangle(new SolidBrush(Color.LightBlue), x * cellSize, y * cellSize, cellSize, cellSize);
                        }
                    }
                }

                // Zeichne den Agenten
                g.FillEllipse(new SolidBrush(agentColor), Position.X * cellSize + wallThickness, Position.Y * cellSize + wallThickness, cellSize - wallThickness, cellSize - wallThickness);
                // Zeichne das Ziel
                g.FillRectangle(new SolidBrush(Color.Green), Goal.X * cellSize, Goal.Y * cellSize, cellSize, cellSize);
            }

            return bitmap;
        }
    }
}