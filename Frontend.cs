using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SKUassignment
{
    static class Frontend
    {
        public static void Menu(string Title, List<KeyValuePair<string,Action>> Entries)
        {
            bool run = true;
            Console.CursorVisible = false;
            Entries.Add(new KeyValuePair<string, Action>("Exit", new Action(()=> { run = false; })));
            List<KeyValuePair<string, ConsoleKey>> Controls = new();
            Controls.Add(new KeyValuePair<string, ConsoleKey>("Down", ConsoleKey.DownArrow));
            Controls.Add(new KeyValuePair<string, ConsoleKey>("Up", ConsoleKey.UpArrow));
            int SelectedEntry = 0;
            while (run)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Clear();
                Console.SetCursorPosition(1,1);
                Console.WriteLine(Title);
                for(int i = 0;i<= Entries.Count - 1; i++)
                {
                    Console.SetCursorPosition(1, i + 2);
                    if (i == SelectedEntry)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine(Entries.ElementAt(i).Key);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                PrintHelpBar(Controls);
                ConsoleKeyInfo input = Console.ReadKey();
                if(input.Key == ConsoleKey.Enter)
                {
                    //Select
                    Console.Clear();
                    Entries.ElementAt(SelectedEntry).Value.Invoke();
                    Console.Write("Press any key to continue...");
                    Console.ReadKey();
                }
                if(input.Key == ConsoleKey.DownArrow)
                {
                    SelectedEntry++;
                }
                if (input.Key == ConsoleKey.UpArrow)
                {
                    SelectedEntry--;
                }
                if (SelectedEntry <= -1)
                {
                    SelectedEntry = Entries.Count - 1;
                }
                if (SelectedEntry >= Entries.Count)
                {
                    SelectedEntry = 0;
                }
            }
            Console.CursorVisible = true;
        }
        private static void PrintHelpBar(List<KeyValuePair<string, ConsoleKey>> Controls)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.SetCursorPosition(0,Console.WindowHeight - 1);
            string help = " ";
            for (int i = 0;i<= Controls.Count - 1; i++)
            {
                help += $"[{Controls.ElementAt(i).Value.ToString()}] {Controls.ElementAt(i).Key}   ";
            }
            if(help.Length<= Console.WindowWidth)
            {
                while(help.Length < Console.WindowWidth)
                {
                    help += " ";
                }
            }
            Console.Write(help);
            Console.SetCursorPosition(0, Console.WindowHeight - 1);
            Console.ResetColor();
        }
    }
}
