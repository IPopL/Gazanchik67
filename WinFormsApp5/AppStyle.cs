using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WinFormsApp5
{
    // Стили по руководству (Модуль 2). Цвета меняй только здесь.
    internal static class AppStyle
    {
        public static readonly Color MainBackground = Color.White;
        public static readonly Color SecondaryBackground = ColorTranslator.FromHtml("#7FFF00");
        public static readonly Color AccentBackground = ColorTranslator.FromHtml("#00FA9A");
        public static readonly Color HighDiscountBackground = ColorTranslator.FromHtml("#2E8B57");
        public static readonly Color OutOfStockBackground = Color.LightBlue;

        public static readonly Font AppFont = new Font("Times New Roman", 10F);
        public static readonly Font AppFontBold = new Font("Times New Roman", 10F, FontStyle.Bold);

        public static void ApplyToForm(Form form, string title)
        {
            form.Text = title;
            form.BackColor = MainBackground;
            form.Font = AppFont;
            SetFormIcon(form);
        }

        public static void MakeAccentButton(Button button)
        {
            button.BackColor = AccentBackground;
            button.FlatStyle = FlatStyle.Flat;
            button.Font = AppFont;
            button.UseVisualStyleBackColor = false;
        }

        public static void MakeSecondaryPanel(Panel panel)
        {
            panel.BackColor = SecondaryBackground;
        }

        public static Image LoadLogo()
        {
            string png = Path.Combine(AppPaths.ImagesFolder, "Icon.png");
            if (File.Exists(png))
            {
                return Image.FromFile(png);
            }

            string jpg = Path.Combine(AppPaths.ImagesFolder, "Icon.JPG");
            if (File.Exists(jpg))
            {
                return Image.FromFile(jpg);
            }

            return null;
        }

        public static void SetFormIcon(Form form)
        {
            string icon = Path.Combine(AppPaths.ImagesFolder, "Icon.ico");
            if (File.Exists(icon))
            {
                form.Icon = new Icon(icon);
            }
        }
    }
}
