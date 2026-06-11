using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WinFormsApp5;

namespace WinFormsApp5
{
    public partial class ProductCard : UserControl
    {
        private DataSet.TovarRow myProduct;
        public ProductCard()
        {
            InitializeComponent();
            WireClickEvents();
        }

        private void WireClickEvents()
        {
            Click += ProductCard_Click;
            pictureBox1.Click += ProductCard_Click;
            foreach (Control control in Controls)
            {
                control.Click += ProductCard_Click;
            }
        }

        public void FillData(DataSet.TovarRow product)
        {
            myProduct = product;
            lblDiscountPrice.Visible = true;
            lblPrice.ForeColor = SystemColors.ControlText;
            lblPrice.Font = new Font(lblPrice.Font, FontStyle.Regular);

            lblCategory.Text = $"Категория: {product.CategoriesRow.Category}";
            lblName.Text = $"Название: {product.Name}";
            lblDescription.Text = $"Описание: {product.Description}";
            lblManufacturer.Text = $"Производитель: {product.ManufacturersRow.Manufacturer}";
            lblSupplier.Text = $"Поставщик: {product.SuppliersRow.Supplier}";
            lblMeasure.Text = $"Еденица измерения: {product.QuantityMeasure}";
            lblAmount.Text = $"Кол-во на складе: {product.Amount}";

            if (product.Discount > 0)
            {
                lblDiscount.Text = $"Скидка: {product.Discount}%";
                lblPrice.Text = $"Старая цена: {product.Price:C}";
                lblDiscountPrice.Text = $"Цена со скидкой: {(product.Price * (1 - (product.Discount) / 100m)):C}";
                lblPrice.ForeColor = Color.Red;
                lblPrice.Font = new Font(lblPrice.Font, FontStyle.Strikeout);
            }
            else
            {
                lblDiscount.Text = "Скидка 0";
                lblPrice.Text = $"Цена: {product.Price:C}";
                lblDiscountPrice.Visible = false;
            }


            if (product.Amount == 0)
            {
                this.BackColor = AppStyle.OutOfStockBackground;
            }
            else if (product.Discount > 15)
            {
                this.BackColor = AppStyle.HighDiscountBackground;
                lblDiscount.Font = AppStyle.AppFontBold;
            }
            else
            {
                this.BackColor = AppStyle.MainBackground;
            }

            Cursor = CurrentSession.Role == CurrentSession.Admin
                ? Cursors.Hand
                : Cursors.Default;

            string imagesFolderPath = AppPaths.ImagesFolder;
            string fileName = product.IsPhotoNull() || string.IsNullOrWhiteSpace(product.Photo)
            ? "picture.png" : product.Photo;
            string fullImagesPath = Path.Combine(imagesFolderPath, fileName);

            if (File.Exists(fullImagesPath))
            {
                using (var stream = new FileStream(fullImagesPath, FileMode.Open, FileAccess.Read))
                {
                    pictureBox1.Image = Image.FromStream(stream);
                }
            }
            else
            {
                string Zaglushka = Path.Combine(imagesFolderPath, "picture.png");
                using (var stream = new FileStream(Zaglushka, FileMode.Open, FileAccess.Read))
                {
                    pictureBox1.Image = Image.FromStream(stream);
                }
            }
        }

        private void ProductCard_Click(object sender, EventArgs e)
        {
            if (CurrentSession.Role != CurrentSession.Admin)
            {
                return;
            }

            if (myProduct == null)
            {
                return;
            }

            ProductEditForm editForm = new ProductEditForm(myProduct);

            if (editForm.ShowDialog() == DialogResult.OK)
            {
                if (this.ParentForm is Form1 mainForm)
                {
                    mainForm.LoadProducts();
                }
            }
        }
    }
}
