using System.Linq;

namespace aknakereso
{
    internal class Program
    {
        // 💡 KONSTANS: Meghatározza a tábla maximális méretét (20x20)
        private const int MAX_BOARD_SIZE = 20;

        // =================================================================
        // 🔹 FŐ PROGRAM
        // =================================================================

        static void Main(string[] args)
        {
            // Konzol alapbeállítások
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Aknakereső";

            bool playAgain = true;

            // Fő külső ciklus: amíg a felhasználó újra akar játszani
            while (playAgain)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=== 🎯 AKNAKERESŐ ===\n");
                Console.ResetColor();

                // 1. Játék beállításai és inicializálás
                int size = GetValidSizeInput($"Add meg a tábla méretét (min. 2, max. {MAX_BOARD_SIZE}): ", 2);
                int mineCount = GetValidMineCountInput(size);

                char[,] board = new char[size, size];
                bool[,] mines = new bool[size, size];
                bool[,] flags = new bool[size, size];

                int revealed = 0;
                int placedFlags = 0;
                bool gameOver = false;

                InitializeMines(mines, mineCount);
                InitializeBoard(board);

                // 2. Fő játékciklus futtatása
                RunGameLoop(board, mines, flags, size, mineCount, ref revealed, ref placedFlags, ref gameOver);

                // 3. Játék befejezése és eredmény kijelzése
                DisplayEndGame(mines, board, gameOver);

                // 4. Új játék indításának megkérdezése
                playAgain = AskForNewGame();
            }

            Console.WriteLine("\nKöszönöm a játékot! Nyomj egy gombot a kilépéshez...");
            Console.ReadKey();
        }

        // =================================================================
        // 🔹 INPUT KEZELŐ METÓDUSOK
        // =================================================================

        /// <summary>
        /// Bekéri a tábla méretét, és validálja a bemenetet a minimum és maximum korlátok között.
        /// </summary>
        private static int GetValidSizeInput(string prompt, int min)
        {
            int value;
            bool valid;
            do
            {
                Console.Write(prompt);
                // Ellenőrzi: érvényes szám-e, min >= 2, max <= 20
                valid = int.TryParse(Console.ReadLine(), out value) && value >= min && value <= MAX_BOARD_SIZE;
                if (!valid)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"❌ Helytelen bemenet! Kérlek adj meg egy számot {min} és {MAX_BOARD_SIZE} között.");
                    Console.ResetColor();
                }
            } while (!valid);
            return value;
        }

        /// <summary>
        /// Bekéri az aknák számát, biztosítva, hogy legalább 1 legyen és ne fedje le az egész táblát.
        /// </summary>
        private static int GetValidMineCountInput(int size)
        {
            int mineCount;
            int maxSize = size * size;
            bool valid;
            do
            {
                Console.Write($"Add meg az aknák számát (1 - {maxSize - 1}): ");
                // Ellenőrzi: érvényes szám-e, 1 <= szám < maximális mezőszám
                valid = int.TryParse(Console.ReadLine(), out mineCount) && mineCount >= 1 && mineCount < maxSize;
                if (!valid)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"❌ Helytelen bemenet! Kérlek adj meg egy számot 1 és {maxSize - 1} között.");
                    Console.ResetColor();
                }
            } while (!valid);
            return mineCount;
        }

        /// <summary>
        /// Bekéri, hogy a felhasználó szeretne-e új játékot (Igen/Nem validáció).
        /// </summary>
        private static bool AskForNewGame()
        {
            string choice;
            do
            {
                Console.WriteLine("\n------------------------------------------------");
                Console.Write("Szeretnél új játékot kezdeni? (Igen/Nem): ");
                choice = Console.ReadLine()?.Trim().ToUpper();

                if (choice == "I" || choice == "IGEN") return true;
                if (choice == "N" || choice == "NEM") return false;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Helytelen bemenet! Kérlek 'Igen' vagy 'Nem' (vagy I/N) választ adj.");
                Console.ResetColor();

            } while (true);
        }

        // =================================================================
        // 🔹 JÁTÉK BEÁLLÍTÓ METÓDUSOK
        // =================================================================

        /// <summary>
        /// Elhelyezi az aknákat véletlenszerűen a táblán (bool[,] mines).
        /// </summary>
        private static void InitializeMines(bool[,] mines, int count)
        {
            int size = mines.GetLength(0);
            int placed = 0;
            Random rnd = new Random();

            while (placed < count)
            {
                int x = rnd.Next(size), y = rnd.Next(size);
                if (!mines[x, y])
                {
                    mines[x, y] = true;
                    placed++;
                }
            }
        }

        /// <summary>
        /// Inicializálja a látható táblát felfedetlen mezőkkel ('#').
        /// </summary>
        private static void InitializeBoard(char[,] board)
        {
            int size = board.GetLength(0);
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    board[i, j] = '#';
        }

        // =================================================================
        // 🔹 JÁTÉK MENET METÓDUSOK
        // =================================================================

        /// <summary>
        /// Futtatja a fő játékhurkot.
        /// </summary>
        private static void RunGameLoop(char[,] board, bool[,] mines, bool[,] flags, int size, int mineCount, ref int revealed, ref int placedFlags, ref bool gameOver)
        {
            int maxSize = size * size;

            while (!gameOver)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=== 🎯 AKNAKERESŐ ===");
                Console.ResetColor();
                Console.WriteLine($"Méret: {size}x{size} | Aknák: {mineCount} | Zászlók: {placedFlags}/{mineCount} | Felfedett: {revealed}/{maxSize - mineCount}\n");

                DrawBoard(board, flags);

                // Beolvassa a felhasználó lépését és feldolgozza azt.
                if (!HandlePlayerMove(board, mines, flags, size, ref revealed, ref placedFlags, ref gameOver, mineCount))
                {
                    // Ha a HandlePlayerMove false-szal tér vissza, a felhasználó '0'-val kilépett.
                    break;
                }
            }
        }

        /// <summary>
        /// Kezeli a mód (R/F) és a koordináták beolvasását és validálását.
        /// </summary>
        /// <returns>Igaz, ha a lépés feldolgozásra került; Hamis, ha a felhasználó kilépett ('0').</returns>
        private static bool HandlePlayerMove(char[,] board, bool[,] mines, bool[,] flags, int size, ref int revealed, ref int placedFlags, ref bool gameOver, int mineCount)
        {
            string mode;
            int row, col;
            bool validInput;

            // 1. Mód beolvasása (R/F/0)
            do
            {
                Console.WriteLine("\n(Lépéshez: 'R' = felfedés, 'F' = zászlózás | Kilépéshez írj be '0'-t)");
                Console.Write("Választott mód (R/F/0): ");
                mode = Console.ReadLine()?.Trim().ToUpper();

                if (mode == "0") return false;

                validInput = mode == "R" || mode == "F";
                if (!validInput)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("❌ Helytelen mód! Kérlek írj be R, F vagy 0-t.");
                    Console.ResetColor();
                }
            } while (!validInput);

            // 2. Sor beolvasása (Koordináta érvényesítése)
            do
            {
                validInput = true;
                Console.Write("Sor (1–{0} vagy 0 a kilépéshez): ", size);
                if (!int.TryParse(Console.ReadLine(), out row))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("❌ Helytelen bemenet! Kérlek **számot** adj meg.");
                    Console.ResetColor();
                    validInput = false;
                }
                else if (row == 0) return false;
                else if (row < 1 || row > size)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"❌ Érvénytelen sorszám. Kérlek adj meg 1 és {size} közötti számot.");
                    Console.ResetColor();
                    validInput = false;
                }
            } while (!validInput);

            // 3. Oszlop beolvasása (Koordináta érvényesítése)
            do
            {
                validInput = true;
                Console.Write("Oszlop (1–{0} vagy 0 a kilépéshez): ", size);
                if (!int.TryParse(Console.ReadLine(), out col))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("❌ Helytelen bemenet! Kérlek **számot** adj meg.");
                    Console.ResetColor();
                    validInput = false;
                }
                else if (col == 0) return false;
                else if (col < 1 || col > size)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"❌ Érvénytelen oszlopszám. Kérlek adj meg 1 és {size} közötti számot.");
                    Console.ResetColor();
                    validInput = false;
                }
            } while (!validInput);

            // A mozgás feldolgozása a 0-alapú indexekkel
            ProcessMove(board, mines, flags, row - 1, col - 1, mode, ref revealed, ref placedFlags, ref gameOver, mineCount);
            return true;
        }

        /// <summary>
        /// Felfedez egy mezőt, zászlóz, vagy felrobbantja az aknát.
        /// </summary>
        private static void ProcessMove(char[,] board, bool[,] mines, bool[,] flags, int row, int col, string mode, ref int revealed, ref int placedFlags, ref bool gameOver, int mineCount)
        {
            int maxSize = board.GetLength(0) * board.GetLength(0);

            // 🔹 Zászlózás mód kezelése (mode == "F")
            if (mode == "F")
            {
                string message = "";
                ConsoleColor color = ConsoleColor.White;

                if (board[row, col] != '#' && !flags[row, col])
                {
                    message = "❌ Nem tehetsz zászlót felfedett mezőre!";
                    color = ConsoleColor.Red;
                }
                else if (flags[row, col])
                {
                    // Zászló eltávolítása
                    flags[row, col] = false;
                    placedFlags--;
                    message = "🚩 A zászló eltávolítva a területről.";
                    color = ConsoleColor.Yellow;
                }
                else if (placedFlags < mineCount)
                {
                    // Zászló elhelyezése
                    flags[row, col] = true;
                    placedFlags++;
                    message = "🚩 A terület meg lett jelölve zászlóval.";
                    color = ConsoleColor.Green;
                }
                else
                {
                    // Hiba: Túl sok zászló
                    message = "❌ Elérted a maximális zászlószámot!";
                    color = ConsoleColor.Red;
                }

                // 💡 MÓDOSÍTÁS: Üzenet kijelzése és gombnyomás megvárása
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.WriteLine("Nyomj meg egy gombot a folytatáshoz...");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            // 🔹 Felfedés mód kezelése (mode == "R")

            if (flags[row, col])
            {
                // Hiba: Zászlós mező felfedése
                Console.WriteLine("Előbb vedd le a zászlót erről a mezőről!");
                Console.ReadKey();
                return;
            }

            if (mines[row, col])
            {
                // Akna eltalálva - Vége a játéknak!
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("💥 Ráléptél egy aknára!");
                Console.ResetColor();
                gameOver = true;
            }
            else if (board[row, col] == '#')
            {
                // Új, felfedetlen mező felfedezése
                RevealEmpty(board, mines, row, col, ref revealed);

                if (revealed == maxSize - mineCount)
                {
                    // Nyerés feltétel: minden nem-akna mező fel van fedve.
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("🎉 Gratulálok, nyertél!");
                    Console.ResetColor();
                    gameOver = true;
                }
            }
            else
            {
                // Már felfedett mezőre kattintás (UI feedback)
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("⚠️ Ezt a mezőt már felfedted!");
                Console.WriteLine("Nyomj meg egy gombot a játék folytatásához...");
                Console.ResetColor();
                Console.ReadKey();
            }
        }


        // =================================================================
        // 🔹 SEGÉD ÉS KIJELZŐ METÓDUSOK
        // =================================================================

        /// <summary>
        /// Megjeleníti a játék végi eredményeket és az aknák helyét (csak veszteség/nyerés esetén).
        /// </summary>
        private static void DisplayEndGame(bool[,] mines, char[,] board, bool gameOver)
        {
            int size = mines.GetLength(0);
            if (!gameOver) return; // Ha manuális kilépés történt (0-val), itt nem csinál semmit.

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
        }

        /// <summary>
        /// Kiszámolja a szomszédos aknák számát (segédfüggvény a felfedéshez).
        /// </summary>
        private static int CountNearbyMines(bool[,] mines, int row, int col)
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

        /// <summary>
        /// Rekurzívan felfedez üres mezőket és szomszédjaikat (autómatikus felfedés, 'cascade').
        /// </summary>
        private static void RevealEmpty(char[,] board, bool[,] mines, int row, int col, ref int revealed)
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

        /// <summary>
        /// Kirajzolja a táblát a konzolra, formázással és színekkel.
        /// </summary>
        private static void DrawBoard(char[,] board, bool[,] flags)
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
                    // Színezés a szomszédos aknák száma szerint
                    switch (cell)
                    {
                        case '#': Console.ForegroundColor = ConsoleColor.DarkGray; break; // Felfedetlen
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