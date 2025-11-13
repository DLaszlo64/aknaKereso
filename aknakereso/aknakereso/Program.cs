namespace aknakereso
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const int size = 5;          // Tábla mérete (5x5)
            const int mineCount = 5;     // Aknák száma
            char[,] board = new char[size, size];
            bool[,] mines = new bool[size, size];
            bool gameOver = false;
            int revealed = 0;

            Random rnd = new Random();

            // 1️⃣ Aknák elhelyezése
            int placed = 0;
            while (placed < mineCount)
            {
                int x = rnd.Next(size);
                int y = rnd.Next(size);
                if (!mines[x, y])
                {
                    mines[x, y] = true;
                    placed++;
                }
            }

            // 2️⃣ Tábla inicializálása
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    board[i, j] = '#';  // # = ismeretlen mező

            // 3️⃣ Játékciklus
            while (!gameOver)
            {
                Console.Clear();
                Console.WriteLine("AKNAKERESŐ");
                Console.WriteLine($"A táblán összesen {mineCount} bomba van!");
                Console.WriteLine();

                // Oszlopfejléc (1–5)
                Console.Write("    ");
                for (int c = 1; c <= size; c++)
                    Console.Write(c + " ");
                Console.WriteLine();

                // Tábla kirajzolása
                for (int i = 0; i < size; i++)
                {
                    Console.Write((i + 1) + " | ");  // Sorjelzés 1–5
                    for (int j = 0; j < size; j++)
                        Console.Write(board[i, j] + " ");
                    Console.WriteLine();
                }

                Console.WriteLine();
                Console.Write("Add meg a sor számát (1–{0}): ", size);
                if (!int.TryParse(Console.ReadLine(), out int row) || row < 1 || row > size)
                {
                    Console.WriteLine("Érvénytelen sor!");
                    Console.ReadKey();
                    continue;
                }

                Console.Write("Add meg az oszlop számát (1–{0}): ", size);
                if (!int.TryParse(Console.ReadLine(), out int col) || col < 1 || col > size)
                {
                    Console.WriteLine("Érvénytelen oszlop!");
                    Console.ReadKey();
                    continue;
                }

                // Átváltás 1-alapúról 0-alapú indexelésre
                row--;
                col--;

                // 4️⃣ Ellenőrzés
                if (mines[row, col])
                {
                    Console.Clear();
                    Console.WriteLine("💥 Ráléptél egy aknára! 💥");
                    gameOver = true;
                }
                else if (board[row, col] == '#')
                {
                    int nearby = CountNearbyMines(mines, row, col);
                    board[row, col] = nearby == 0 ? ' ' : nearby.ToString()[0];
                    revealed++;

                    // Nyertél, ha minden biztonságos mezőt felfedtél
                    if (revealed == size * size - mineCount)
                    {
                        Console.Clear();
                        Console.WriteLine("🎉 Gratulálok, nyertél! 🎉");
                        gameOver = true;
                    }
                }
                else
                {
                    Console.WriteLine("Ezt a mezőt már felfedted!");
                    Console.ReadKey();
                }
            }

            // 5️⃣ Aknák felfedése a végén
            Console.WriteLine("\nAknák helyei:");
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                    Console.Write((mines[i, j] ? '*' : '.') + " ");
                Console.WriteLine();
            }

            Console.WriteLine("\nNyomj egy gombot a kilépéshez...");
            Console.ReadKey();
        }

        static int CountNearbyMines(bool[,] mines, int row, int col)
        {
            int count = 0;
            int size = mines.GetLength(0);

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int r = row + i;
                    int c = col + j;
                    if (r >= 0 && r < size && c >= 0 && c < size && mines[r, c])
                        count++;
                }
            }

            return count;
        }
    }
}
