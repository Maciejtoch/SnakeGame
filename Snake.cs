using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

class Program
{
    static int screenWidth = 32;
    static int screenHeight = 16;
    static Random random = new Random();
    static List<Snake> snakes = new List<Snake>();
    static List<Obstacle> obstacles = new List<Obstacle>();
    static bool gameRunning = true;

    static void Main()
    {
        Console.WindowHeight = screenHeight;
        Console.WindowWidth = screenWidth;

        snakes.Add(new Snake(screenWidth / 3, screenHeight / 2, ConsoleColor.Green, ConsoleKey.W, ConsoleKey.S, ConsoleKey.A, ConsoleKey.D));
        snakes.Add(new Snake(2 * screenWidth / 3, screenHeight / 2, ConsoleColor.Yellow, ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.LeftArrow, ConsoleKey.RightArrow));

        obstacles.Add(new Obstacle(random.Next(1, screenWidth - 1), random.Next(1, screenHeight - 1)));

        Thread inputThread = new Thread(ReadInput);
        inputThread.Start();

        while (gameRunning)
        {
            Console.Clear();
            DrawBorders();

            foreach (var obstacle in obstacles)
                obstacle.Draw();

            foreach (var snake in snakes)
                snake.Move();

            CheckCollisions();
            Thread.Sleep(100);
        }
    }

    static void DrawBorders()
    {
        Console.ForegroundColor = ConsoleColor.White;
        for (int i = 0; i < screenWidth; i++)
        {
            Console.SetCursorPosition(i, 0);
            Console.Write("#");
            Console.SetCursorPosition(i, screenHeight - 1);
            Console.Write("#");
        }
        for (int i = 0; i < screenHeight; i++)
        {
            Console.SetCursorPosition(0, i);
            Console.Write("#");
            Console.SetCursorPosition(screenWidth - 1, i);
            Console.Write("#");
        }
    }

    static void ReadInput()
    {
        while (gameRunning)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            foreach (var snake in snakes)
                snake.HandleInput(keyInfo.Key);
        }
    }

    static void CheckCollisions()
    {
        foreach (var snake in snakes)
        {
            if (snake.X == 0 || snake.X == screenWidth - 1 || snake.Y == 0 || snake.Y == screenHeight - 1)
            {
                gameRunning = false;
                Console.Clear();
                Console.WriteLine("Game Over! Snake hit the wall.");
                return;
            }
            
            foreach (var otherSnake in snakes)
            {
                if (otherSnake != snake && otherSnake.X == snake.X && otherSnake.Y == snake.Y)
                {
                    gameRunning = false;
                    Console.Clear();
                    Console.WriteLine("Game Over! Snakes collided.");
                    return;
                }
            }
            
            foreach (var obstacle in obstacles)
            {
                if (snake.X == obstacle.X && snake.Y == obstacle.Y)
                {
                    gameRunning = false;
                    Console.Clear();
                    Console.WriteLine("Game Over! Snake hit an obstacle.");
                    return;
                }
            }
        }
    }
}

class Snake
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public ConsoleColor Color { get; }
    public ConsoleKey Up { get; }
    public ConsoleKey Down { get; }
    public ConsoleKey Left { get; }
    public ConsoleKey Right { get; }
    private string direction;

    public Snake(int startX, int startY, ConsoleColor color, ConsoleKey up, ConsoleKey down, ConsoleKey left, ConsoleKey right)
    {
        X = startX;
        Y = startY;
        Color = color;
        Up = up;
        Down = down;
        Left = left;
        Right = right;
        direction = "RIGHT";
    }

    public void HandleInput(ConsoleKey key)
    {
        if (key == Up && direction != "DOWN") direction = "UP";
        else if (key == Down && direction != "UP") direction = "DOWN";
        else if (key == Left && direction != "RIGHT") direction = "LEFT";
        else if (key == Right && direction != "LEFT") direction = "RIGHT";
    }

    public void Move()
    {
        if (direction == "UP") Y--;
        else if (direction == "DOWN") Y++;
        else if (direction == "LEFT") X--;
        else if (direction == "RIGHT") X++;

        Console.ForegroundColor = Color;
        Console.SetCursorPosition(X, Y);
        Console.Write("O");
    }
}

class Obstacle
{
    public int X { get; }
    public int Y { get; }

    public Obstacle(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void Draw()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.SetCursorPosition(X, Y);
        Console.Write("*");
    }
}
