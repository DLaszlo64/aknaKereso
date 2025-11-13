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

                // 🔹 Pálya méretének beolvasása 🔄
                int size;
                bool validSize;
                do
                {
                    Console.Write("Add meg a tábla méretét (pl. 8, minimum 2): ");
                    // ELLENŐRZÉS: Helyes számformátum és érvényes érték
                    validSize = int.TryParse(Console.ReadLine(), out size) && size >= 2;
                    if (!validSize)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("❌ Helytelen bemenet! Kérlek adj meg egy számot, ami legalább 2.");
                        Console.ResetColor();
                    }
                } while (!validSize);


                // 🔹 Aknák számának beolvasása 🔄
                int mineCount;
                int maxSize = size * size;
                bool validMineCount;
                do
                {
                    Console.Write($"Add meg az aknák számát (1 - {maxSize - 1}): ");
                    // ELLENŐRZÉS: Helyes számformátum és érvényes érték
                    validMineCount = int.TryParse(Console.ReadLine(), out mineCount) && mineCount >= 1 && mineCount < maxSize;
                    if (!validMineCount)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"❌ Helytelen bemenet! Kérlek adj meg egy számot 1 és {maxSize - 1} között.");
                        Console.ResetColor();
                    }
                } while (!validMineCount);

                // --- Játék inicializálása ---

                char[,] board = new char[size, size];
                bool[,] mines = new bool[size, size];
                bool[,] flags = new bool[size, size];

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
                    Console.WriteLine($"Méret: {size}x{size} | Aknák: {mineCount} | Zászlók: {placedFlags}/{mineCount} | Felfedett: {revealed}/{maxSize - mineCount}\n");

                    DrawBoard(board, flags);

                    string mode;
                    int row, col;
                    bool validInput = false;

                    // 🔹 Mód beolvasása (R/F/0) 🔄
                    do
                    {
                        Console.WriteLine("\n(Lépéshez: 'R' = felfedés, 'F' = zászlózás | Kilépéshez írj be '0'-t)");
                        Console.Write("Választott mód (R/F/0): ");
                        mode = Console.ReadLine()?.Trim().ToUpper();

                        // Kilépés 0 beírására a mód választásakor
                        if (mode == "0")
                        {
                            gameOver = true;
                            validInput = true;
                            break;
                        }

                        validInput = mode == "R" || mode == "F";
                        if (!validInput)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("❌ Helytelen mód! Kérlek írj be R, F vagy 0-t.");
                            Console.ResetColor();
                        }
                    } while (!validInput);

                    if (mode == "0") break; // Kilépés az inner while ciklusból

                    // 🔹 Sor beolvasása 🔄
                    do
                    {
                        Console.Write("Sor (1–{0} vagy 0 a kilépéshez): ", size);
                        if (!int.TryParse(Console.ReadLine(), out row))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("❌ Helytelen bemenet! Kérlek **számot** adj meg.");
                            Console.ResetColor();
                            validInput = false;
                        }
                        else if (row < 0 || row > size)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"❌ Érvénytelen sorszám. Kérlek adj meg 1 és {size} közötti számot, vagy 0-t.");
                            Console.ResetColor();
                            validInput = false;
                        }
                        else
                        {
                            validInput = true;
                        }
                    } while (!validInput);

                    // Kilépés 0 beírására
                    if (row == 0)
                    {
                        gameOver = true;
                        Console.WriteLine("\nKilépés a játékból.");
                        break;
                    }

                    // 🔹 Oszlop beolvasása 🔄
                    do
                    {
                        Console.Write("Oszlop (1–{0} vagy 0 a kilépéshez): ", size);
                        if (!int.TryParse(Console.ReadLine(), out col))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("❌ Helytelen bemenet! Kérlek **számot** adj meg.");
                            Console.ResetColor();
                            validInput = false;
                        }
                        else if (col < 0 || col > size)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"❌ Érvénytelen oszlopszám. Kérlek adj meg 1 és {size} közötti számot, vagy 0-t.");
                            Console.ResetColor();
                            validInput = false;
                        }
                        else
                        {
                            validInput = true;
                        }
                    } while (!validInput);

                    // Kilépés 0 beírására
                    if (col == 0)
                    {
                        gameOver = true;
                        Console.WriteLine("\nKilépés a játékból.");
                        break;
                    }

                    row--; col--; // 0-alapú indexelés

                    // 🔹 Zászlózás mód
                    if (mode == "F")
                    {
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

                        if (revealed == maxSize - mineCount)
                        {
                            Console.Clear();
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("🎉 Gratulálok, nyertél!");
                            Console.ResetColor();
                            gameOver = true;
                        }
                    }
                    else // 💡 MÓDOSÍTÁS: Ha a mező már fel van fedve (nem akna és nem is '#')
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("⚠️ Ezt a mezőt már felfedted!");
                        Console.WriteLine("Nyomj meg egy gombot a játék folytatásához...");
                        Console.ResetColor();
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

                // 🔹 Újrajátszás kérdése (Ellenőrzéssel) 🔄
                string choice;
                do
                {
                    Console.WriteLine("\n------------------------------------------------");
                    Console.Write("Szeretnél új játékot kezdeni? (Igen/Nem): ");
                    choice = Console.ReadLine()?.Trim().ToUpper();

                    if (choice == "I" || choice == "IGEN")
                    {
                        playAgain = true;
                        break;
                    }
                    else if (choice == "N" || choice == "NEM")
                    {
                        playAgain = false;
                        break;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("❌ Helytelen bemenet! Kérlek 'Igen' vagy 'Nem' (vagy I/N) választ adj.");
                        Console.ResetColor();
                    }
                } while (true); // Végtelen ciklus, amíg érvényes választ nem kap

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