using System;
using System.Drawing;
using System.Windows.Forms;

namespace AntMaze
{
    public partial class MainForm : Form
    {
        private Maze maze;
        private QLearningLibrary qLearning;
        private System.Windows.Forms.Timer qLearningTimer;
        private System.Windows.Forms.Timer epochTimer;
        public int width = 25;
        public int height = 25;
        private bool goalReached = false;

        public MainForm()
        {
            InitializeComponent();
            InitializeQLearning();
            InitializeQLearningTimer();
            PerformQLearningAndMoveAgent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Maze maze = Maze.CreateRandomMaze(width, height);
            maze.Position = new Point(1, 1);

            pictureBoxMaze.Image = maze.RenderMazeToBitmap();

            epochTimer = new System.Windows.Forms.Timer();
            epochTimer.Tick += new EventHandler(EpochTimer_Tick);
            epochTimer.Interval = 600000;
            epochTimer.Start();
        }
        private void InitializeQLearning()
        {
            maze = Maze.CreateRandomMaze(width, height);
            maze.Position = new Point(1, 1);
            Agent agent = new Agent(width, height, maze);
            agent.Position = maze.Position;
            qLearning = new QLearningLibrary(maze, agent);
            qLearning.Train(5, 32);
            qLearning.PerformQLearningStep();
            qLearning.MoveAgent();
        }

        private void EpochTimer_Tick(object sender, EventArgs e)
        {
            epochTimer.Stop();
            maze.Position = new Point(1, 1);
            Agent agent = new Agent(width, height, maze);
            agent.Position = maze.Position;
            qLearning = new QLearningLibrary(maze, agent);
            qLearning.PerformQLearningStep();
            qLearning.MoveAgent();
            epochTimer.Start();
        }
        private void InitializeQLearningTimer()
        {
            qLearningTimer = new System.Windows.Forms.Timer();
            qLearningTimer.Interval = 100;
            qLearningTimer.Tick += QLearningTimer_Tick;
            qLearningTimer.Start();
        }

        private void QLearningTimer_Tick(object sender, EventArgs e)
        {
            this.Text = $"Erreichte Ziele: {goalsReached} Punkte: {qLearning.reward}";
            PerformQLearningAndMoveAgent();
            pictureBoxMaze.Image = maze.RenderMazeToBitmap();
        }

        public void UpdateStatusStrip(string text)
        {
            statusStrip1.Text = text;
        }

        private int goalsReached = 0;
        private void PerformQLearningAndMoveAgent()
        {

            if (goalReached)
            {
                return;
            }
            qLearning.PerformQLearningStep();
            qLearning.MoveAgent();

            pictureBoxMaze.Image = maze.RenderMazeToBitmap();

            if (maze.Position == maze.Goal)
            {
                goalReached = true;
                goalsReached++;
                this.Text = $"Erreichte Ziele: {goalsReached}";
                maze = Maze.CreateRandomMaze(width, height);
                maze.Position = new Point(1, 1);
                Agent agent = new Agent(width, height, maze);
                agent.Position = maze.Position;
                qLearning = new QLearningLibrary(maze, agent);
                qLearning.Train(5, 32);
                qLearning.PerformQLearningStep();
                qLearning.MoveAgent();
            }

            goalReached = false;
        }
    }
}

