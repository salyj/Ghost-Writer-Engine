using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleMenuEngine
{
    // ─── Configuration & Styling ─────────────────────────────────────

    /// <summary>
    /// Defines the visual border style for menus.
    /// </summary>
    public class BorderStyle
    {
        public char TopLeft { get; set; }
        public char TopRight { get; set; }
        public char BottomLeft { get; set; }
        public char BottomRight { get; set; }
        public char Horizontal { get; set; }
        public char Vertical { get; set; }
        public char TeeLeft { get; set; }
        public char TeeRight { get; set; }

        public static BorderStyle Single => new BorderStyle
        {
            TopLeft = '┌', TopRight = '┐',
            BottomLeft = '└', BottomRight = '┘',
            Horizontal = '─', Vertical = '│',
            TeeLeft = '├', TeeRight = '┤'
        };

        public static BorderStyle Double => new BorderStyle
        {
            TopLeft = '╔', TopRight = '╗',
            BottomLeft = '╚', BottomRight = '╝',
            Horizontal = '═', Vertical = '║',
            TeeLeft = '╠', TeeRight = '╣'
        };

        public static BorderStyle Rounded => new BorderStyle
        {
            TopLeft = '╭', TopRight = '╮',
            BottomLeft = '╰', BottomRight = '╯',
            Horizontal = '─', Vertical = '│',
            TeeLeft = '├', TeeRight = '┤'
        };

        public static BorderStyle Ascii => new BorderStyle
        {
            TopLeft = '+', TopRight = '+',
            BottomLeft = '+', BottomRight = '+',
            Horizontal = '-', Vertical = '|',
            TeeLeft = '+', TeeRight = '+'
        };
    }

    /// <summary>
    /// Theme controlling colors and border style for the menu.
    /// </summary>
    public class MenuTheme
    {
        public ConsoleColor TitleForeground { get; set; } = ConsoleColor.Cyan;
        public ConsoleColor TitleBackground { get; set; } = ConsoleColor.Black;
        public ConsoleColor BorderForeground { get; set; } = ConsoleColor.DarkGray;
        public ConsoleColor ItemForeground { get; set; } = ConsoleColor.White;
        public ConsoleColor ItemBackground { get; set; } = ConsoleColor.Black;
        public ConsoleColor SelectedForeground { get; set; } = ConsoleColor.Black;
        public ConsoleColor SelectedBackground { get; set; } = ConsoleColor.Cyan;
        public ConsoleColor DisabledForeground { get; set; } = ConsoleColor.DarkGray;
        public ConsoleColor FooterForeground { get; set; } = ConsoleColor.DarkYellow;
        public BorderStyle Border { get; set; } = BorderStyle.Single;

        public static MenuTheme Default => new MenuTheme();

        public static MenuTheme Hacker => new MenuTheme
        {
            TitleForeground = ConsoleColor.Green,
            BorderForeground = ConsoleColor.DarkGreen,
            ItemForeground = ConsoleColor.Green,
            SelectedForeground = ConsoleColor.Black,
            SelectedBackground = ConsoleColor.Green,
            FooterForeground = ConsoleColor.DarkGreen,
            Border = BorderStyle.Single
        };

        public static MenuTheme Elegant => new MenuTheme
        {
            TitleForeground = ConsoleColor.White,
            BorderForeground = ConsoleColor.DarkCyan,
            ItemForeground = ConsoleColor.Gray,
            SelectedForeground = ConsoleColor.White,
            SelectedBackground = ConsoleColor.DarkBlue,
            FooterForeground = ConsoleColor.DarkCyan,
            Border = BorderStyle.Double
        };
    }

    // ─── Menu Items ──────────────────────────────────────────────────

    /// <summary>
    /// Base class for all menu items.
    /// </summary>
    public abstract class MenuItemBase
    {
        public string Label { get; set; } = "";
        public bool Enabled { get; set; } = true;
        public bool Visible { get; set; } = true;

        /// <summary>If true, this item is a visual separator, not selectable.</summary>
        public virtual bool IsSeparator => false;

        /// <summary>If true, this item can receive focus/selection.</summary>
        public virtual bool IsSelectable => Enabled && !IsSeparator;
    }

    /// <summary>
    /// A standard menu item that triggers an action when selected.
    /// </summary>
    public class MenuItem : MenuItemBase
    {
        public Action? Action { get; set; }
        public char? Shortcut { get; set; }
        public string? Description { get; set; }

        public MenuItem() { }

        public MenuItem(string label, Action? action = null, char? shortcut = null)
        {
            Label = label;
            Action = action;
            Shortcut = shortcut;
        }
    }

    /// <summary>
    /// A toggle (checkbox) menu item.
    /// </summary>
    public class ToggleMenuItem : MenuItemBase
    {
        public bool IsChecked { get; set; }
        public Action<bool>? OnToggle { get; set; }

        public ToggleMenuItem() { }

        public ToggleMenuItem(string label, bool isChecked = false, Action<bool>? onToggle = null)
        {
            Label = label;
            IsChecked = isChecked;
            OnToggle = onToggle;
        }
    }

    /// <summary>
    /// A submenu item that opens a child menu.
    /// </summary>
    public class SubMenuItem : MenuItemBase
    {
        public Menu? SubMenu { get; set; }

        public SubMenuItem() { }

        public SubMenuItem(string label, Menu subMenu)
        {
            Label = label;
            SubMenu = subMenu;
        }
    }

    /// <summary>
    /// A horizontal separator line.
    /// </summary>
    public class SeparatorItem : MenuItemBase
    {
        public override bool IsSeparator => true;
        public override bool IsSelectable => false;
    }

    // ─── Menu ────────────────────────────────────────────────────────

    /// <summary>
    /// Represents a single menu with a title, items, and optional footer.
    /// </summary>
    public class Menu
    {
        public string Title { get; set; } = "";
        public string? Subtitle { get; set; }
        public string? Footer { get; set; }
        public List<MenuItemBase> Items { get; set; } = new();
        public MenuTheme Theme { get; set; } = MenuTheme.Default;
        public int MinWidth { get; set; } = 30;
        public bool ShowItemNumbers { get; set; } = false;
        public bool CenterInConsole { get; set; } = true;

        // ── Builder-pattern helpers ──

        public Menu AddItem(string label, Action? action = null, char? shortcut = null)
        {
            Items.Add(new MenuItem(label, action, shortcut));
            return this;
        }

        public Menu AddToggle(string label, bool isChecked = false, Action<bool>? onToggle = null)
        {
            Items.Add(new ToggleMenuItem(label, isChecked, onToggle));
            return this;
        }

        public Menu AddSubMenu(string label, Menu subMenu)
        {
            Items.Add(new SubMenuItem(label, subMenu));
            return this;
        }

        public Menu AddSeparator()
        {
            Items.Add(new SeparatorItem());
            return this;
        }
    }

    // ─── Rendering Engine ────────────────────────────────────────────

    /// <summary>
    /// The core engine that draws menus and handles input.
    /// </summary>
    public class MenuEngine
    {
        private int _selectedIndex;
        private bool _running;

        /// <summary>
        /// Shows the menu, handles keyboard navigation, and returns
        /// the index of the item that was activated (or -1 if cancelled).
        /// </summary>
        public int Run(Menu menu)
        {
            Console.CursorVisible = false;
            _running = true;

            // Start selection on the first selectable item
            _selectedIndex = NextSelectable(menu, -1, forward: true);

            while (_running)
            {
                Draw(menu);
                var result = HandleInput(menu);
                if (result.HasValue)
                {
                    Console.CursorVisible = true;
                    return result.Value;
                }
            }

            Console.CursorVisible = true;
            return -1;
        }

        // ── Drawing ──

        private void Draw(Menu menu)
        {
            var theme = menu.Theme;
            var border = theme.Border;
            var lines = BuildLines(menu, out int innerWidth);
            int boxWidth = innerWidth + 2; // +2 for left/right borders

            int offsetX = 0;
            int offsetY = 0;

            if (menu.CenterInConsole)
            {
                try
                {
                    offsetX = Math.Max(0, (Console.WindowWidth - boxWidth) / 2);
                    offsetY = Math.Max(0, (Console.WindowHeight - lines.Count) / 2);
                }
                catch { /* non-interactive console fallback */ }
            }

            Console.Clear();

            for (int i = 0; i < lines.Count; i++)
            {
                try { Console.SetCursorPosition(offsetX, offsetY + i); }
                catch { /* fallback: just write sequentially */ }

                lines[i].Render(theme);
            }
        }

        private List<RenderLine> BuildLines(Menu menu, out int innerWidth)
        {
            var theme = menu.Theme;
            var border = theme.Border;
            var items = menu.Items.Where(i => i.Visible).ToList();

            // Calculate inner width
            int maxLabel = items.Max(i => FormatItemText(i, menu, 0).Length);
            innerWidth = Math.Max(menu.MinWidth, Math.Max(menu.Title.Length + 2, maxLabel + 4));

            var lines = new List<RenderLine>();

            // Top border
            lines.Add(RenderLine.Border(
                $"{border.TopLeft}{new string(border.Horizontal, innerWidth)}{border.TopRight}",
                theme));

            // Title
            string titlePadded = CenterText(menu.Title, innerWidth);
            lines.Add(RenderLine.Title(
                $"{border.Vertical}{titlePadded}{border.Vertical}",
                theme, 1, titlePadded.Length));

            // Subtitle
            if (!string.IsNullOrEmpty(menu.Subtitle))
            {
                string subPadded = CenterText(menu.Subtitle, innerWidth);
                lines.Add(RenderLine.Subtitle(
                    $"{border.Vertical}{subPadded}{border.Vertical}",
                    theme, 1, subPadded.Length));
            }

            // Separator after title
            lines.Add(RenderLine.Border(
                $"{border.TeeLeft}{new string(border.Horizontal, innerWidth)}{border.TeeRight}",
                theme));

            // Items
            int visibleIndex = 0;
            for (int i = 0; i < menu.Items.Count; i++)
            {
                var item = menu.Items[i];
                if (!item.Visible) continue;

                if (item.IsSeparator)
                {
                    lines.Add(RenderLine.Border(
                        $"{border.TeeLeft}{new string(border.Horizontal, innerWidth)}{border.TeeRight}",
                        theme));
                }
                else
                {
                    bool isSelected = (i == _selectedIndex);
                    string text = FormatItemText(item, menu, visibleIndex);
                    string padded = $" {text}".PadRight(innerWidth);

                    lines.Add(RenderLine.Item(
                        $"{border.Vertical}{padded}{border.Vertical}",
                        theme, isSelected, !item.IsSelectable,
                        1, padded.Length));
                }

                visibleIndex++;
            }

            // Separator before footer
            lines.Add(RenderLine.Border(
                $"{border.TeeLeft}{new string(border.Horizontal, innerWidth)}{border.TeeRight}",
                theme));

            // Footer
            string footerText = menu.Footer ?? "↑↓ Navigate  Enter Select  Esc Back";
            string footerPadded = CenterText(footerText, innerWidth);
            lines.Add(RenderLine.Footer(
                $"{border.Vertical}{footerPadded}{border.Vertical}",
                theme, 1, footerPadded.Length));

            // Bottom border
            lines.Add(RenderLine.Border(
                $"{border.BottomLeft}{new string(border.Horizontal, innerWidth)}{border.BottomRight}",
                theme));

            return lines;
        }

        private string FormatItemText(MenuItemBase item, Menu menu, int index)
        {
            var sb = new StringBuilder();

            if (menu.ShowItemNumbers)
                sb.Append($"{index + 1}. ");

            if (item is ToggleMenuItem toggle)
                sb.Append(toggle.IsChecked ? "[✓] " : "[ ] ");

            sb.Append(item.Label);

            if (item is MenuItem mi && mi.Shortcut.HasValue)
                sb.Append($"  ({mi.Shortcut.Value})");

            if (item is SubMenuItem)
                sb.Append("  ►");

            return sb.ToString();
        }

        // ── Input ──

        private int? HandleInput(Menu menu)
        {
            var key = Console.ReadKey(intercept: true);

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    _selectedIndex = NextSelectable(menu, _selectedIndex, forward: false);
                    break;

                case ConsoleKey.DownArrow:
                    _selectedIndex = NextSelectable(menu, _selectedIndex, forward: true);
                    break;

                case ConsoleKey.Home:
                    _selectedIndex = NextSelectable(menu, -1, forward: true);
                    break;

                case ConsoleKey.End:
                    _selectedIndex = NextSelectable(menu, menu.Items.Count, forward: false);
                    break;

                case ConsoleKey.Enter:
                    return Activate(menu);

                case ConsoleKey.Escape:
                    _running = false;
                    return -1;

                default:
                    // Check shortcuts
                    TryShortcut(menu, key.KeyChar);
                    break;
            }

            return null; // keep looping
        }

        private int? Activate(Menu menu)
        {
            if (_selectedIndex < 0 || _selectedIndex >= menu.Items.Count)
                return null;

            var item = menu.Items[_selectedIndex];

            if (item is MenuItem mi)
            {
                mi.Action?.Invoke();
                return _selectedIndex;
            }

            if (item is ToggleMenuItem toggle)
            {
                toggle.IsChecked = !toggle.IsChecked;
                toggle.OnToggle?.Invoke(toggle.IsChecked);
                return null; // stay in menu
            }

            if (item is SubMenuItem sub && sub.SubMenu != null)
            {
                var subEngine = new MenuEngine();
                subEngine.Run(sub.SubMenu);
                return null; // return to parent menu
            }

            return null;
        }

        private void TryShortcut(Menu menu, char key)
        {
            for (int i = 0; i < menu.Items.Count; i++)
            {
                if (menu.Items[i] is MenuItem mi && mi.Shortcut.HasValue &&
                    char.ToLower(mi.Shortcut.Value) == char.ToLower(key) &&
                    mi.IsSelectable)
                {
                    _selectedIndex = i;
                    Activate(menu);
                    break;
                }
            }
        }

        // ── Helpers ──

        private int NextSelectable(Menu menu, int current, bool forward)
        {
            int count = menu.Items.Count;
            int dir = forward ? 1 : -1;
            int idx = current + dir;

            while (idx >= 0 && idx < count)
            {
                if (menu.Items[idx].Visible && menu.Items[idx].IsSelectable)
                    return idx;
                idx += dir;
            }

            return current < 0 ? 0 : (current >= count ? count - 1 : current);
        }

        private static string CenterText(string text, int width)
        {
            if (text.Length >= width) return text.Substring(0, width);
            int pad = (width - text.Length) / 2;
            return text.PadLeft(pad + text.Length).PadRight(width);
        }
    }

    // ─── Render Line (internal) ──────────────────────────────────────

    internal enum LineType { Border, Title, Subtitle, Footer, Item }

    internal class RenderLine
    {
        public string Text { get; set; } = "";
        public LineType Type { get; set; }
        public MenuTheme Theme { get; set; } = MenuTheme.Default;
        public bool IsSelected { get; set; }
        public bool IsDisabled { get; set; }
        public int ContentStart { get; set; } // index where inner content begins
        public int ContentLength { get; set; }

        public static RenderLine Border(string text, MenuTheme theme) => new()
        {
            Text = text, Type = LineType.Border, Theme = theme
        };

        public static RenderLine Title(string text, MenuTheme theme, int start, int length) => new()
        {
            Text = text, Type = LineType.Title, Theme = theme,
            ContentStart = start, ContentLength = length
        };

        public static RenderLine Subtitle(string text, MenuTheme theme, int start, int length) => new()
        {
            Text = text, Type = LineType.Subtitle, Theme = theme,
            ContentStart = start, ContentLength = length
        };

        public static RenderLine Footer(string text, MenuTheme theme, int start, int length) => new()
        {
            Text = text, Type = LineType.Footer, Theme = theme,
            ContentStart = start, ContentLength = length
        };

        public static RenderLine Item(string text, MenuTheme theme,
            bool selected, bool disabled, int start, int length) => new()
        {
            Text = text, Type = LineType.Item, Theme = theme,
            IsSelected = selected, IsDisabled = disabled,
            ContentStart = start, ContentLength = length
        };

        public void Render(MenuTheme theme)
        {
            switch (Type)
            {
                case LineType.Border:
                    WriteColored(Text, theme.BorderForeground);
                    break;

                case LineType.Title:
                    WriteColored(Text[..ContentStart], theme.BorderForeground);
                    WriteColored(Text[ContentStart..(ContentStart + ContentLength)],
                        theme.TitleForeground, theme.TitleBackground);
                    WriteColored(Text[(ContentStart + ContentLength)..], theme.BorderForeground);
                    break;

                case LineType.Subtitle:
                    WriteColored(Text[..ContentStart], theme.BorderForeground);
                    WriteColored(Text[ContentStart..(ContentStart + ContentLength)],
                        theme.DisabledForeground);
                    WriteColored(Text[(ContentStart + ContentLength)..], theme.BorderForeground);
                    break;

                case LineType.Footer:
                    WriteColored(Text[..ContentStart], theme.BorderForeground);
                    WriteColored(Text[ContentStart..(ContentStart + ContentLength)],
                        theme.FooterForeground);
                    WriteColored(Text[(ContentStart + ContentLength)..], theme.BorderForeground);
                    break;

                case LineType.Item:
                    WriteColored(Text[..ContentStart], theme.BorderForeground);
                    if (IsDisabled)
                        WriteColored(Text[ContentStart..(ContentStart + ContentLength)],
                            theme.DisabledForeground);
                    else if (IsSelected)
                        WriteColored(Text[ContentStart..(ContentStart + ContentLength)],
                            theme.SelectedForeground, theme.SelectedBackground);
                    else
                        WriteColored(Text[ContentStart..(ContentStart + ContentLength)],
                            theme.ItemForeground, theme.ItemBackground);
                    WriteColored(Text[(ContentStart + ContentLength)..], theme.BorderForeground);
                    break;
            }

            Console.WriteLine();
        }

        private static void WriteColored(string text, ConsoleColor fg,
            ConsoleColor? bg = null)
        {
            var prevFg = Console.ForegroundColor;
            var prevBg = Console.BackgroundColor;
            Console.ForegroundColor = fg;
            if (bg.HasValue) Console.BackgroundColor = bg.Value;
            Console.Write(text);
            Console.ForegroundColor = prevFg;
            Console.BackgroundColor = prevBg;
        }
    }
}
