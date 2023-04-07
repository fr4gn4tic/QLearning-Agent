using System;
using System.Drawing;
using System.Threading.Tasks;

namespace AntMaze
{
    public class QLearningLibrary
    {
        private Maze maze;
        private Agent agent;
        private readonly double alpha = 0.3;
        private readonly double gamma = 0.2;
        private readonly double epsilon = 0.7;
        private Random random;
        public int reward = 0;
        private bool[,] visited;


        public QLearningLibrary(Maze maze, Agent agent)
        {
            this.maze = maze;
            this.agent = agent;
            random = new Random();
            visited = new bool[maze.Width, maze.Height];
        }

        private Point GetNeighbor(Point position, int direction)
        {
            Point neighbor = position;

            switch (direction)
            {
                case 0: // Up
                    if (!maze.IsVisitedAt(neighbor.X, neighbor.Y))
                    {
                        neighbor = new Point(position.X, position.Y - 1);
                    }
                    break;
                case 1: // Right
                    if (!maze.IsVisitedAt(neighbor.X, neighbor.Y))
                    {
                        neighbor = new Point(position.X + 1, position.Y);
                    }
                    break;
                case 2: // Down
                    if (!maze.IsVisitedAt(neighbor.X, neighbor.Y))
                    {
                        neighbor = new Point(position.X, position.Y + 1);
                    }
                    break;
                case 3: // Left
                    if (!maze.IsVisitedAt(neighbor.X, neighbor.Y))
                    {
                        neighbor = new Point(position.X - 1, position.Y);
                    }
                    break;
            }

            // Check if neighbor has been visited
            if (!maze.IsVisitedAt(neighbor.X, neighbor.Y))
            {

            }
            else
            {
            }

            return neighbor;
        }
        private int ChooseAction(int currentState)
        {
            double temperature = 0.9; // Controls the level of exploration
            double[] qValues = new double[4];
            double sum = 0;

            // Compute the softmax probabilities for each action and accumulate sum
            for (int i = 0; i < 4; i++)
            {
                Point neighbor = GetNeighbor(agent.Position, i);
                if (maze.IsVisitedAt(agent.Position.X, agent.Position.Y))
                {
                    qValues[i] = 0;
                }
                else if (!maze.IsVisitedAt(agent.Position.X, agent.Position.Y))
                {
                    qValues[i] = agent.QTable[currentState, i];
                }
                else
                {
                    qValues[i] = agent.QTable[currentState, i];
                }
                sum += Math.Exp(qValues[i] / temperature);
            }

            // Normalize the probabilities and sample an action
            double randomValue = random.NextDouble();
            if (randomValue < epsilon)
            {
                return random.Next(0, 4);
            }
            else
            {
                randomValue = random.NextDouble() * sum;
                for (int i = 0; i < 4; i++)
                {
                    randomValue -= Math.Exp(qValues[i] / temperature);
                    if (randomValue < 0)
                    {
                        return i;
                    }
                }
            }

            // Should never get here
            return -1;
        }

        private int MaxQValue(int state)
        {
            int maxQValue = int.MinValue;

            for (int i = 0; i < 4; i++)
            {
                int qValue = agent.QTable[state, i];
                if (qValue > maxQValue)
                {
                    maxQValue = qValue;
                }
            }

            return maxQValue;
        }
        public void PerformQLearningStep()
        {
            int currentState = agent.Position.X + agent.Position.Y * maze.Width;
            int action = ChooseAction(currentState);

            agent.Move(action);
            int nextState = agent.Position.X + agent.Position.Y * maze.Width;

            if (maze.IsWallAt(agent.Position.X, agent.Position.Y))
            {
                reward = -4;
                agent.Move((action + 2) % 4);
            }
            else if (agent.Position == maze.Goal)
            {
                reward += 100;
            }
            else if (!maze.IsVisitedAt(agent.Position.X, agent.Position.Y))
            {
                maze.MarkVisited(agent.Position.X, agent.Position.Y);
                reward++;
            }
            else if (visited[agent.Position.X, agent.Position.Y])
            {
                reward = -4;
            }
            else
            {
                // The agent has visited this position before, but not in the current episode
                reward = -4;
            }

            // Update QTable with reward and new state
            agent.QTable[currentState, action] = (int)((1 - alpha) * agent.QTable[currentState, action] + alpha * (reward + gamma * MaxQValue(nextState)));

            // Mark current position as visited
            visited[agent.Position.X, agent.Position.Y] = true;
        }

        public void MoveAgent()
        {
            maze.Position = agent.Position;
        }

        public void Train(int numEpochs, int numThreads)
        {
            Parallel.For(0, numThreads, t =>
            {
                for (int epoch = 1; epoch <= numEpochs / numThreads; epoch++)
                {
                    agent.Reset();
                    while (agent.Position != maze.Goal)
                    {
                        PerformQLearningStep();
                        MoveAgent();
                    }
                }
            });
        }
    }
}

