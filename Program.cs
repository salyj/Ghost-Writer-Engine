using System;
using ConsoleMenuEngine;

namespace ConsoleMenuDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // ── Build a settings submenu ──
            var settingsMenu = new Menu
            {
                Title = "⚙  Settings",
                Subtitle = "Configure your preferences",
                Theme = MenuTheme.Elegant
            };

            settingsMenu
                .AddToggle("Sound Effects", isChecked: true,
                    on => Console.Beep())
                .AddToggle("Music", isChecked: true)
                .AddToggle("Fullscreen Mode", isChecked: false)
                .AddSeparator()
                .AddItem("Reset to Defaults", () =>
                {
                    // Reset logic here
                }, shortcut: 'r')
                .AddItem("Back", shortcut: 'b');

            // ── Build the main menu ──
            var mainMenu = new Menu
            {
                Title = "◆  CONSOLE MENU ENGINE  ◆",
                Subtitle = "v1.0 — A flexible text-based menu system",
                Theme = MenuTheme.Default,
                MinWidth = 42
            };

            mainMenu
                .AddItem("New Game", () =>
                {
                    Console.Clear();
                    Console.WriteLine("\n  Starting new game...\n  Press any key.");
                    Console.ReadKey(true);
                }, shortcut: 'n')
                .AddItem("Load Game", () =>
                {
                    Console.Clear();
                    Console.WriteLine("\n  Loading saved game...\n  Press any key.");
                    Console.ReadKey(true);
                }, shortcut: 'l')
                .AddSeparator()
                .AddSubMenu("Settings", settingsMenu)
                .AddItem("View Credits", () =>
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\n  ╔══════════════════════════════╗");
                    Console.WriteLine("  ║     Console Menu Engine      ║");
                    Console.WriteLine("  ║     Built with C# & Love     ║");
                    Console.WriteLine("  ╚══════════════════════════════╝");
                    Console.ResetColor();
                    Console.WriteLine("\n  Press any key.");
                    Console.ReadKey(true);
                }, shortcut: 'c')
                .AddSeparator()
                .AddItem("Exit", shortcut: 'x');

            // ── Run it ──
            var engine = new MenuEngine();

            while (true)
            {
                int result = engine.Run(mainMenu);

                // "Exit" is the last item (index 7, accounting for separators)
                // or Escape was pressed (result == -1)
                if (result == -1 || mainMenu.Items[result] is MenuItem mi &&
                    mi.Label == "Exit")
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine("\n  Goodbye!\n");
                    Console.ResetColor();
                    break;
                }
            }
        }
    }
}
