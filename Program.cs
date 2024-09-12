using Aiv.Draw;
using System;
using System.Threading;

namespace Memory
{
    class Program
    {
        static void Main(string[] args)
        {
            Window window = new Window(800, 600, "Memory", PixelFormat.RGB);
            Card[,] cards = new Card[4, 4];
            Color corniceColor;
            Color coveredColor;
            Color[,] trueColors = new Color[4, 4];
            int numMoves = 16;
            int indexRowCard1 = 0;
            int indexColCard1 = 0;
            int indexRowCard2 = 0;
            int indexColCard2 = 0;
            bool firstPlayerTurn = true;
            int firstPlayerScore = 0;
            int secondPlayerScore = 0;
            bool scorePrinted = false;

            InitColor(out corniceColor, 255, 0, 0);
            InitColor(out coveredColor, 255, 255, 255);

            //true colors are initialized at white color
            InitCardsAndTrueColors(cards, coveredColor, trueColors);

            //generates true colors with a random color for each one,
            //excluding white color
            GenTrueColors(trueColors);

            Console.WriteLine("First player turn");

            while (window.IsOpened)
            {
                for (int i = 0; i < cards.GetLength(0); i++)
                {
                    for (int j = 0; j < cards.GetLength(1); j++)
                    {
                        DrawCardCornice(window, cards[i, j].X - 1, cards[i, j].Y - 1, cards[i, j].Width + 2, cards[i, j].Height + 2, corniceColor);
                        DrawCard(window, cards[i, j]);
                    }
                }

                if (numMoves % 2 == 0 && CardClicked(window, cards, trueColors, ref indexRowCard1, ref indexColCard1))
                {
                    numMoves--;
                }

                else if (numMoves % 2 != 0 && CardClicked(window, cards, trueColors, ref indexRowCard2, ref indexColCard2))
                {
                    if (IsSameColor(cards[indexRowCard1, indexColCard1].Color, cards[indexRowCard2, indexColCard2].Color))
                    {
                        numMoves--;

                        if (firstPlayerTurn)
                        {
                            firstPlayerScore++;
                        }

                        else
                        {
                            secondPlayerScore++;
                        }
                    }

                    else
                    {
                        if (firstPlayerTurn)
                        {
                            firstPlayerTurn = false;
                            Console.WriteLine("Second player turn");
                        }

                        else
                        {
                            firstPlayerTurn = true;
                            Console.WriteLine("First player turn");
                        }

                        DrawCard(window, cards[indexRowCard2, indexColCard2]);
                        window.Blit();

                        Thread.Sleep(1000);

                        cards[indexRowCard1, indexColCard1].Color = coveredColor;
                        cards[indexRowCard2, indexColCard2].Color = coveredColor;

                        numMoves++;
                    }
                }

                if (numMoves == 0 && !scorePrinted)
                {
                    string res = "";

                    Console.WriteLine("\nFirst player score: " + firstPlayerScore + "\nsecond player score: " + secondPlayerScore);

                    if (firstPlayerScore > secondPlayerScore)
                    {
                        res = "First player won!";
                    }

                    else if (firstPlayerScore == secondPlayerScore)
                    {
                        res = "Tied";
                    }

                    else
                    {
                        res = "Second player won!";
                    }

                    Console.WriteLine(res);

                    scorePrinted = true;
                }

                window.Blit();
            }
        }

        struct Card
        {
            public int X;
            public int Y;
            public int Width;
            public int Height;
            public Color Color;
        }

        struct Color
        {
            public byte R;
            public byte G;
            public byte B;
        }

        static void InitCardsAndTrueColors(Card[,] cards, Color coveredColor, Color[,] trueColors)
        {
            int x = 270;
            int y = 80;

            for (int i = 0; i < cards.GetLength(0); i++)
            {
                for (int j = 0; j < cards.GetLength(1); j++)
                {
                    InitCard(out cards[i, j], x, y, 50, 50, coveredColor);
                    trueColors[i, j] = coveredColor;

                    x += 60;
                }

                x = 270;
                y += 60;
            }
        }

        static void InitCard(out Card card, int x, int y, int width, int height, Color c)
        {
            card.X = x;
            card.Y = y;
            card.Width = width;
            card.Height = height;
            card.Color = c;
        }

        static void GenTrueColors(Color[,] c)
        {
            int i = 0;
            int j = 0;
            Color randColor;
            Random r = new Random();

            do
            {
                randColor = GetRandomColor(r);

                GetTwoRandomIndexes(r, c, ref i, ref j);
                c[i, j] = randColor;

                GetTwoRandomIndexes(r, c, ref i, ref j);
                c[i, j] = randColor;
            }
            while (!AllTrueColorsGened(c));
        }

        static void GetTwoRandomIndexes(Random r, Color[,] c, ref int i, ref int j)
        {
            do
            {
                i = r.Next(c.GetLength(0));
                j = r.Next(c.GetLength(1));
            }
            while (c[i, j].R != 255 && c[i, j].G != 255 && c[i, j].B != 255);
        }

        static bool AllTrueColorsGened(Color[,] c)
        {
            for (int i = 0; i < c.GetLength(0); i++)
            {
                for (int j = 0; j < c.GetLength(1); j++)
                {
                    if (c[i, j].R == 255 && c[i, j].G == 255 && c[i, j].B == 255)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        static bool IsSameColor(Color c1, Color c2)
        {
            if (c1.R == c2.R && c1.G == c2.G && c1.B == c2.B)
            {
                return true;
            }

            return false;
        }

        static bool CardClicked(Window w, Card[,] cards, Color[,] trueColors, ref int indexRow, ref int indexCol)
        {
            if (w.MouseLeft)
            {
                for (int i = 0; i < cards.GetLength(0); i++)
                {
                    for (int j = 0; j < cards.GetLength(1); j++)
                    {
                        if (w.MouseX >= cards[i, j].X && w.MouseX <= cards[i, j].X + (cards[i, j].Width - 1) && w.MouseY >= cards[i, j].Y && w.MouseY <= cards[i, j].Y + (cards[i, j].Height - 1))
                        {
                            if (cards[i, j].Color.R == 255 && cards[i, j].Color.G == 255 && cards[i, j].Color.B == 255)
                            {
                                cards[i, j].Color = trueColors[i, j];
                                indexRow = i;
                                indexCol = j;

                                return true;
                            }

                            return false;
                        }
                    }
                }
            }

            return false;
        }

        //excludes white color
        static Color GetRandomColor(Random r)
        {
            Color c;

            do
            {
                c.R = (byte)r.Next(256);
                c.G = (byte)r.Next(256);
                c.B = (byte)r.Next(256);
            }
            while (c.R > 230 && c.G > 230 && c.B > 230);

            return c;
        }

        static void InitColor(out Color c, byte r, byte g, byte b)
        {
            c.R = r;
            c.G = g;
            c.B = b;
        }

        static void DrawCardCornice(Window w, int x, int y, int width, int height, Color c)
        {
            DrawHorizontalLine(w, x, y, width, c);
            DrawHorizontalLine(w, x, height + y, width, c);
            DrawVerticalLine(w, x, y + 1, height, c);
            DrawVerticalLine(w, width + x, y + 1, height, c);
        }

        static void DrawCard(Window w, Card c)
        {
            for (int i = 0; i < c.Height; i++)
            {
                DrawHorizontalLine(w, c.X, c.Y + i, c.Width, c.Color);
            }
        }

        static void DrawHorizontalLine(Window w, int x, int y, int width, Color c)
        {
            for (int i = 0; i < width; i++)
            {
                PutPixel(w, x + i, y, c);
            }
        }

        static void DrawVerticalLine(Window w, int x, int y, int height, Color c)
        {
            for (int i = 0; i < height; i++)
            {
                PutPixel(w, x, y + i, c);
            }
        }

        static void PutPixel(Window w, int x, int y, Color c)
        {
            int i = (w.Width * y + x) * 3;

            w.Bitmap[i] = c.R;
            w.Bitmap[i + 1] = c.G;
            w.Bitmap[i + 2] = c.B;
        }
    }
}
