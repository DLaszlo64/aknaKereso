namespace aknakereso
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8; // Unicode támogatás
            Console.Title = "Aknakereső";

            bool playAgain = true;

            while (playAgain)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=== 🎯 AKNAKERESŐ ===\n");
                Console.ResetColor();

                // 🔹 Pálya beállítása (Újra megkérdezi minden játéknál)
                Console.Write("Add meg a tábla méretét (pl. 8): ");
                int size = int.TryParse(Console.ReadLine(), out int s) && s > 1 ? s : 8;

                Console.Write("Add meg az aknák számát: ");
                int mineCount = int.TryParse(Console.ReadLine(), out int m) && m > 0 && m < size * size ? m : size;

                char[,] board = new char[size, size];
                bool[,] mines = new bool[size, size];
                bool[,] flags = new bool[size, size]; // Zászlók tárolása

                bool gameOver = false;
                int revealed = 0;
                int placedFlags = 0;
                Random rnd = new Random();

                // 🔹 Aknák elhelyezése
                int placed = 0;
                while (placed < mineCount)
                {
                    int x = rnd.Next(size), y = rnd.Next(size);
                    if (!mines[x, y])
                    {
                        mines[x, y] = true;
                        placed++;
                    }
                }

                // 🔹 Tábla inicializálása
                for (int i = 0; i < size; i++)
                    for (int j = 0; j < size; j++)
                        board[i, j] = '#';

                // 🔹 Játékciklus
                while (!gameOver)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("=== 🎯 AKNAKERESŐ ===");
                    Console.ResetColor();
                    Console.WriteLine($"Méret: {size}x{size} | Aknák: {mineCount} | Zászlók: {placedFlags}/{mineCount} | Felfedett: {revealed}/{size * size - mineCount}\n");

                    DrawBoard(board, flags);

                    // 💡 Kilépési opció
                    Console.WriteLine("\n(Lépéshez: 'R' = felfedés, 'F' = zászlózás | Kilépéshez a módnál VAGY a koordinátáknál írj be '0'-t)");
                    Console.Write("Választott mód (R/F/0): ");
                    string mode = Console.ReadLine()?.Trim().ToUpper();

                    // Kilépés 0 beírására a mód választásakor
                    if (mode == "0")
                    {
                        gameOver = true;
                        Console.WriteLine("\nKilépés a játékból.");
                        break;
                    }

                    if (mode != "R" && mode != "F") continue;

                    Console.Write("Sor (1–{0}): ", size);
                    if (!int.TryParse(Console.ReadLine(), out int row) || row < 0 || row > size) continue;

                    // Kilépés 0 beírására
                    if (row == 0)
                    {
                        gameOver = true;
                        Console.WriteLine("\nKilépés a játékból.");
                        break;
                    }

                    Console.Write("Oszlop (1–{0}): ", size);
                    if (!int.TryParse(Console.ReadLine(), out int col) || col < 0 || col > size) continue;

                    // Kilépés 0 beírására
                    if (col == 0)
                    {
                        gameOver = true;
                        Console.WriteLine("\nKilépés a játékból.");
                        break;
                    }

                    row--; col--;

                    if (mode == "F")
                    {
                        // 🔹 Zászlózás mód
                        if (board[row, col] != '#' && !flags[row, col])
                        {
                            Console.WriteLine("Nem tehetsz zászlót felfedett mezőre!");
                            Console.ReadKey();
                            continue;
                        }

                        if (flags[row, col])
                        {
                            flags[row, col] = false;
                            placedFlags--;
                        }
                        else if (placedFlags < mineCount)
                        {
                            flags[row, col] = true;
                            placedFlags++;
                        }
                        else
                        {
                            Console.WriteLine("Elérted a maximális zászlószámot!");
                            Console.ReadKey();
                        }
                        continue;
                    }

                    // 🔹 Felfedés mód
                    if (flags[row, col])
                    {
                        Console.WriteLine("Előbb vedd le a zászlót erről a mezőről!");
                        Console.ReadKey();
                        continue;
                    }

                    if (mines[row, col])
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("💥 Ráléptél egy aknára!");
                        Console.ResetColor();
                        gameOver = true;
                    }
                    else if (board[row, col] == '#')
                    {
                        RevealEmpty(board, mines, row, col, ref revealed);

                        if (revealed == size * size - mineCount)
                        {
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("🎉 Gratulálok, nyertél!");
                            Console.ResetColor();
                            gameOver = true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Ezt a mezőt már felfedted!");
                        Console.ReadKey();
                    }
                }

                // ------------------------------------------------

                // 🔹 Játék vége: Aknák felfedése
                Console.WriteLine("\nAknák helyei:");
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        if (mines[i, j])
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("* ");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write(". ");
                        }
                        Console.ResetColor();
                    }
                    Console.WriteLine();
                }

                Console.WriteLine("\n------------------------------------------------");
                Console.Write("Szeretnél új játékot kezdeni? (Igen/Nem): ");
                string choice = Console.ReadLine()?.Trim().ToUpper();

                // A 'playAgain' beállítása a következő ciklushoz
                playAgain = choice == "I" || choice == "IGEN";

                if (!playAgain)
                {
                    Console.WriteLine("\nKöszönöm a játékot! Nyomj egy gombot a kilépéshez...");
                    Console.ReadKey();
                }
            }
        }

        // 🔹 Szomszédos aknák számlálása
        static int CountNearbyMines(bool[,] mines, int row, int col)
        {
            int count = 0;
            int size = mines.GetLength(0);
            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                {
                    int r = row + i, c = col + j;
                    if (r >= 0 && r < size && c >= 0 && c < size && mines[r, c])
                        count++;
                }
            return count;
        }

        // 🔹 Üres mezők automatikus felfedése
        static void RevealEmpty(char[,] board, bool[,] mines, int row, int col, ref int revealed)
        {
            int size = board.GetLength(0);
            if (row < 0 || row >= size || col < 0 || col >= size) return;
            if (board[row, col] != '#') return;

            int nearby = CountNearbyMines(mines, row, col);
            board[row, col] = nearby == 0 ? ' ' : nearby.ToString()[0];
            revealed++;

            if (nearby == 0)
                for (int i = -1; i <= 1; i++)
                    for (int j = -1; j <= 1; j++)
                        if (!(i == 0 && j == 0))
                            RevealEmpty(board, mines, row + i, col + j, ref revealed);
        }

        // 🔹 Színes tábla kirajzolása zászlókkal
        static void DrawBoard(char[,] board, bool[,] flags)
        {
            int size = board.GetLength(0);

            Console.Write("    ");
            for (int c = 1; c <= size; c++) Console.Write(c.ToString().PadLeft(2) + " ");
            Console.WriteLine();
            Console.Write("    ");
            for (int c = 1; c <= size; c++) Console.Write("───");
            Console.WriteLine();

            for (int i = 0; i < size; i++)
            {
                Console.Write((i + 1).ToString().PadLeft(2) + " │ ");
                for (int j = 0; j < size; j++)
                {
                    if (flags[i, j])
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("🚩 ");
                        Console.ResetColor();
                        continue;
                    }

                    char cell = board[i, j];
                    switch (cell)
                    {
                        case '#': Console.ForegroundColor = ConsoleColor.DarkGray; break;
                        case '1': Console.ForegroundColor = ConsoleColor.Blue; break;
                        case '2': Console.ForegroundColor = ConsoleColor.Green; break;
                        case '3': Console.ForegroundColor = ConsoleColor.Red; break;
                        case '4': Console.ForegroundColor = ConsoleColor.DarkBlue; break;
                        case '5': Console.ForegroundColor = ConsoleColor.DarkRed; break;
                        default: Console.ForegroundColor = ConsoleColor.White; break;
                    }
                    Console.Write(cell + "  ");
                    Console.ResetColor();
                }
                Console.WriteLine();

            }
        }
    }
}