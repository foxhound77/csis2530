﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Snake
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public enum SnakePiece
        {
            Space = 0,
            Wall,
            Body,
            Apple,
            Bomb
        }

        // private fields
        private int row = 22, col = 22;
        private Direction direction = Direction.Up;
        private SnakePiece[,] board;
        private LinkedSnake snake;
        private DispatcherTimer timer = new DispatcherTimer();
        private Random rand = new Random();
        private int canvasWidth = 1251, canvasHeight = 660;
        private int applesEaten, snakeMoves;
        private bool hasCrashed;

        public MainWindow()
        {
            InitializeComponent();

            // Initalize game board and timer
            InitializeGameBoard();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            timer.Tick += timer_Tick;


        }

        // Initalize board with SnakePiece.Empty and put
        // a snake in the center of the board, an an apple
        // in the upper left corner.  Make a border of 
        // SnakePiece.Wall around the board, then print
        // the board on the Gui window.
        private void InitializeGameBoard()
        {
            board = new SnakePiece[row, col];
            snake = new LinkedSnake(row / 2, col / 2);
            board[snake.Row, snake.Col] = SnakePiece.Body;
            board[row - 5, 2] = SnakePiece.Apple;
            // make the wall and start the timer
            BuildWall();

            PrintBoard();
        }

        // This is the event handler for the timer, moving the snake
        // and updating its position on the board.  The Canvas is cleared
        // off and is repainted again.
        private void timer_Tick(object sender, EventArgs e)
        {
            UpdateBoard(direction);
            Canvas.Children.Clear();
            PrintBoard();

        }

        // Takes in a game piece and initalizes its height, width, and color
        // Sets the game piece coordinates and adds it to the 
        // gui window
        private void PaintGamePiece(Shape gamePiece, int height, int width, int xCoord, int yCoord, Brush color)
        {


            gamePiece.Height = height;
            gamePiece.Width = width;
            gamePiece.Fill = color;

            Canvas.SetLeft(gamePiece, xCoord);
            Canvas.SetTop(gamePiece, yCoord);

            Canvas.Children.Add(gamePiece);

        }
        // Correlates each value of the board to a
        // component: blue square for wall, 
        // green square for snake body, red ellipse
        // for apple, and yellow ellipse for bomb.
        // the x and y coordinates are based on the height
        // and width of the Canvas in MainWindow.xaml
        private void PrintBoard()
        {
            int startX = canvasWidth / 8;
            int yCoord = canvasHeight;
            int wallWidth = 40;
            int wallHeight = 30;
            int snakeSize = wallHeight - 2;
            int appleSize = wallWidth - 15;


            for (int i = 0; i < board.GetLength(0); i++)
            {
                int xCoord = startX;

                for (int j = 0; j < board.GetLength(1); j++)
                {
                    // draw a blue rectangle for the wall
                    if (board[i, j] == SnakePiece.Wall)
                    {

                        Rectangle wall = new Rectangle();

                        PaintGamePiece(wall, wallHeight, wallWidth, xCoord, yCoord, Brushes.Blue);

                    }
                    // draw a green rectangle for the snake body
                    else if (board[i, j] == SnakePiece.Body)
                    {
                        Rectangle snakeBody = new Rectangle();
                      
                        PaintGamePiece(snakeBody, snakeSize, snakeSize + 7, xCoord, yCoord, Brushes.Green);
                  
                    }
                    // draw a red ellipse for an apple
                    else if (board[i, j] == SnakePiece.Apple)
                    {
                        Ellipse apple = new Ellipse();

                        PaintGamePiece(apple, appleSize, appleSize, xCoord, yCoord, Brushes.Red);
                     
                    }
                    // draw a yellow ellipse for a bomb
                    else if (board[i, j] == SnakePiece.Bomb)
                    {
                        Ellipse bomb = new Ellipse();

                        PaintGamePiece(bomb, appleSize, appleSize, xCoord, yCoord, Brushes.Yellow);
                 
                    }

                    // update x and y coordinates
                    xCoord += wallWidth;
                }

                yCoord -= wallHeight;
            }
        }

        // Places an apple on the board sometimes
        private void CreateApple()
        {

            if (rand.Next(100) % 10 == 0)
            {
                SetRandomPiece(SnakePiece.Apple);
            }
        }

        // places a bomb on the board sometimes
        public void CreateBomb()
        {
            if (rand.Next(100) == 75)
            {
                SetRandomPiece(SnakePiece.Bomb);
            }
        }

        // puts game piece on the board. This method is used for randomly 
        // placing an apple or a bomb on the board.
        private void SetRandomPiece(SnakePiece piece)
        {

            bool set = false;

            do
            {
                int randRow = rand.Next(1, row - 1);
                int randCol = rand.Next(1, col - 1);

                if (board[randRow, randCol] == SnakePiece.Space)
                {
                    board[randRow, randCol] = piece;
                    set = true;
                }
            }
            while (!set);
        }

        /*
         * Creates a new LinkedSnake and moves it in the appropriate direction. It uses the 
         * location of that lookAhead snake to check for walls, bombs, body, or apples.
         * 
         * If the snake eats a apple, then it grows. If the snake runs into itself, a wall, or 
         * a bomb, then the snake crashes and the game is over.
         * 
         * The board is stripped of all snake pieces, and the updated snake is then added back 
         * into to the board.
         */
        private void UpdateBoard(Direction direction)
        {
            LinkedSnake lookAhead = new LinkedSnake(snake.Row, snake.Col);
            lookAhead.Move(direction);

            SnakePiece nextPiece = board[lookAhead.Row, lookAhead.Col];
            if (nextPiece == SnakePiece.Wall || nextPiece == SnakePiece.Bomb || nextPiece == SnakePiece.Body)
            {
                hasCrashed = true;
                return;
            }

            if (nextPiece == SnakePiece.Apple)
            {
                snake.Eat(lookAhead.Row, lookAhead.Col);
                applesEaten++;
            }
            else
            {
                snake.Move(direction);
            }

            RemoveSnakeFromBoard();

            LinkedSnake tempSnake = snake;
            while (tempSnake != null)
            {
                board[tempSnake.Row, tempSnake.Col] = SnakePiece.Body;
                tempSnake = tempSnake.Previous;
            }

            snakeMoves++;
            CreateApple();
            CreateBomb();
        }

        /**
         * This method simply strips the board of snake pieces so the new updated
         * snake can be placed back into the board.
         */ 
        private void RemoveSnakeFromBoard()
        {
            for (int i = 0; i < board.GetLength(0); i++)
            {
                for (int j = 0; j < board.GetLength(1); j++)
                {
                    if (board[i, j] == SnakePiece.Body)
                    {
                        board[i, j] = SnakePiece.Space;
                    }
                }
            }
        }

        // Creates a border of SnakePiece.Wall around the board
        private void BuildWall()
        {
            // builds a wall on the west and east side of the board
            for (int row = 0; row < board.GetLength(0); row++)
            {
                // west
                board[row, 0] = SnakePiece.Wall;
                // east
                board[row, board.GetLength(1) - 1] = SnakePiece.Wall;
            }
            // builds a wall on the north and south side of the board
            for (int col = 0; col < board.GetLength(1); col++)
            {
                // north
                board[0, col] = SnakePiece.Wall;
                // south
                board[board.GetLength(0) - 1, col] = SnakePiece.Wall;
            }
        }

        // Key listener handler event from the Window's property
        // Detect arrows key being pushed and correlates the 
        // direction of the snake with the keys.
        private void MoveSnake(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                direction = Direction.Down;

            }
            else if (e.Key == Key.Down)
            {
                direction = Direction.Up;

            }
            else if (e.Key == Key.Right)
            {
                direction = Direction.Right;

            }
            else if (e.Key == Key.Left)
            {
                direction = Direction.Left;

            }

        }

        // Event handler for the Start button in the Gui, able to restart the 
        // game after the snake has crashed.
        private void StartGame(object sender, RoutedEventArgs e)
        {
            if (hasCrashed)
            {
                InitializeGameBoard();
                hasCrashed = false;
                applesEaten = 0;
            }

            timer.Start();
        }


        private void HighScore(object sender, RoutedEventArgs e)
        {
            HighScore newWindow = new HighScore();
            newWindow.Show();
        }


    }
}

