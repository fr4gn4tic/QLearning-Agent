using System.Collections.Generic;
using System.Drawing;

namespace AntMaze
{
    public class Agent
    {
        public Point Position { get; set; }
        public int[,] QTable { get; set; }
        public int Epoch { get; set; }
        public bool[,] Visited { get; set; }

        public Maze Maze { get; set; }

        private int width;
        private int height;

        private Stack<Point> previousPositions = new Stack<Point>();

        public Agent(int width, int height, Maze maze)
        {
            this.width = width;
            this.height = height;
            this.Maze = maze;
            QTable = new int[width * height, 4];
            Visited = new bool[width, height];
            Epoch = 0;
        }

        private int[] dx = { 0, 1, 0, -1 };
        private int[] dy = { -1, 0, 1, 0 };

        public void Move(int direction)
        {
            int newX = Position.X + dx[direction];
            int newY = Position.Y + dy[direction];

            if (newX >= 0 && newX < width && newY >= 0 && newY < height && !Visited[newX, newY] && !Maze.IsWallAt(newX, newY))
            {
                Visited[Position.X, Position.Y] = true;
                previousPositions.Push(Position);
                Position = new Point(newX, newY);
            }
            else if (previousPositions.Count > 0)
            {
                // Backtrack to the last position with a choice of directions
                Point lastPosition = previousPositions.Pop();
                while (previousPositions.Count > 0 && GetAvailableDirections(lastPosition).Count == 1)
                {
                    lastPosition = previousPositions.Pop();
                }

                List<int> availableDirections = GetAvailableDirections(lastPosition);
                if (availableDirections.Count > 0)
                {
                    // Try a different direction from the last position
                    int newDirection = availableDirections[0]; // TODO: choose a random direction
                    newX = lastPosition.X + dx[newDirection];
                    newY = lastPosition.Y + dy[newDirection];
                    Visited[Position.X, Position.Y] = true;
                    Position = new Point(newX, newY);
                }
            }
            else if (newX >= 0 && newX < width && newY >= 0 && newY < height && !Maze.IsWallAt(newX, newY))
            {
                Visited[Position.X, Position.Y] = true;
                previousPositions.Push(Position);
                Position = new Point(newX, newY);
            }
        }

        private List<int> GetAvailableDirections(Point position)
        {
            List<int> directions = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                int newX = position.X + dx[i];
                int newY = position.Y + dy[i];
                if (newX >= 0 && newX < width && newY >= 0 && newY < height && !Visited[newX, newY] && !Maze.IsWallAt(newX, newY))
                {
                    directions.Add(i);
                }
            }
            return directions;
        }


        public void Reset()
        {
            Position = new Point(1, 1);
            Visited = new bool[width, height];
        }
    }
}