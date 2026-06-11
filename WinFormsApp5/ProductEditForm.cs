using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;

namespace WinFormsApp5
{
    public partial class ProductEditForm : Form
    {
        private DataSet.TovarRow _prod;
        private string _newPhotoPath = null;
        private readonly string _imgDir = AppPaths.ImagesFolder;

        public ProductEditForm(DataSet.TovarRow product = null)
        {
            InitializeComponent();
            btnSave.Click += btnSave_Click;
            btnBack.Click += btnBack_Click;
            btnDelete.Click += btnDelete_Click;
            btnSelectPhoto.Click += btnSelectPhoto_Click;
            btnDeletePhoto.Click += btnDeletePhoto_Click;

            _prod = product;
            LoadLists();
            AppStyle.MakeAccentButton(btnSave);

            if (_prod == null)
            {
                AppStyle.ApplyToForm(this, "Добавление");
                btnDelete.Visible = false;
                picProduct.Image = Image.FromFile(Path.Combine(_imgDir, "picture.png"));
                lblID.Text = "(новый)";
            }
            else
            {
                AppStyle.ApplyToForm(this, "Редактирование");
                FillFields();
            }
        }

        private void LoadLists()
        {
            var ds = new DataSet();
            new DataSetTableAdapters.CategoriesTableAdapter().Fill(ds.Categories);
            new DataSetTableAdapters.ManufacturersTableAdapter().Fill(ds.Manufacturers);
            new DataSetTableAdapters.SuppliersTableAdapter().Fill(ds.Suppliers);

            cmbCategory.DataSource = ds.Categories; cmbCategory.DisplayMember = "Category"; cmbCategory.ValueMember = "ID";
            cmbManufacturer.DataSource = ds.Manufacturers; cmbManufacturer.DisplayMember = "Manufacturer"; cmbManufacturer.ValueMember = "ID";
            cmbSuppliers.DataSource = ds.Suppliers; cmbSuppliers.DisplayMember = "Supplier"; cmbSuppliers.ValueMember = "ID";
        }

        private void FillFields()
        {
            if (_prod == null) return;

            lblID.Text = _prod.Article;
            txtName.Text = _prod.Name;
            txtDescription.Text = _prod.Description;
            txtPrice.Text = _prod.Price.ToString();
            txtAmount.Text = _prod.Amount.ToString();
            txtDiscount.Text = _prod.Discount.ToString();
            txtMeasure.Text = _prod.QuantityMeasure;

            cmbCategory.SelectedValue = _prod.Category;
            cmbManufacturer.SelectedValue = _prod.ManufacturerID;
            cmbSuppliers.SelectedValue = _prod.SupplierID;

            string imgName = _prod.IsPhotoNull() || string.IsNullOrEmpty(_prod.Photo) ? "picture.png" : _prod.Photo;
            string imgPath = Path.Combine(_imgDir, imgName);

            if (File.Exists(imgPath))
            {
                using var stream = new FileStream(imgPath, FileMode.Open, FileAccess.Read);
                picProduct.Image = Image.FromStream(stream);
            }
            else
            {
                string defaultPath = Path.Combine(_imgDir, "picture.png");
                if (File.Exists(defaultPath))
                {
                    using var stream = new FileStream(defaultPath, FileMode.Open, FileAccess.Read);
                    picProduct.Image = Image.FromStream(stream);
                }
                else
                {
                    picProduct.Image = null;
                }

                _prod.Photo = "picture.png";
            }
        }

        private void btnSelectPhoto_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "Images|*.jpg;*.png" };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            using (Image img = Image.FromFile(ofd.FileName))
            {
                if (img.Width > 300 || img.Height > 200)
                {
                    MessageBox.Show("Максимум 300x200!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            _newPhotoPath = ofd.FileName;
            using var stream = new FileStream(_newPhotoPath, FileMode.Open, FileAccess.Read);
            picProduct.Image = Image.FromStream(stream);
        }

        private string GenerateUniqueArticle(DataSet.TovarDataTable table)
        {
            string art;
            var rnd = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            do
            {
                var buffer = new char[6];
                for (int i = 0; i < 6; i++) buffer[i] = chars[rnd.Next(chars.Length)];
                art = new string(buffer);
            } while (table.FindByArticle(art) != null);
            return art;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text)
                || !int.TryParse(txtPrice.Text, out int price) || price < 0
                || !int.TryParse(txtAmount.Text, out int amount) || amount < 0)
            {
                MessageBox.Show("Проверьте поля! Данные не могут быть пустыми или отрицательными.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtDiscount.Text, out int discount)) discount = 0;

            try
            {
                var adapter = new DataSetTableAdapters.TovarTableAdapter();
                var table = new DataSet.TovarDataTable();
                adapter.Fill(table);

                string photoName = _prod == null || _prod.IsPhotoNull() ? "picture.png" : _prod.Photo;

                if (_newPhotoPath != null)
                {
                    photoName = Guid.NewGuid().ToString("N") + Path.GetExtension(_newPhotoPath);
                    File.Copy(_newPhotoPath, Path.Combine(_imgDir, photoName), true);

                    if (_prod != null && !_prod.IsPhotoNull() && _prod.Photo != "picture.png")
                    {
                        picProduct.Image?.Dispose();
                        string oldPath = Path.Combine(_imgDir, _prod.Photo);
                        if (File.Exists(oldPath)) File.Delete(oldPath);
                    }
                }

                DataSet.TovarRow row;
                bool isNew = _prod == null;

                if (isNew)
                {
                    row = table.NewTovarRow();
                    row.Article = GenerateUniqueArticle(table);
                }
                else
                {
                    row = table.FindByArticle(_prod.Article);
                    if (row == null) throw new Exception("Строка товара не найдена.");
                }

                row.Name = txtName.Text;
                row.Description = txtDescription.Text;
                row.Price = price;
                row.Amount = amount;
                row.Discount = discount;
                row.QuantityMeasure = txtMeasure.Text;
                row.Category = (int)cmbCategory.SelectedValue;
                row.ManufacturerID = (int)cmbManufacturer.SelectedValue;
                row.SupplierID = (int)cmbSuppliers.SelectedValue;
                row.Photo = photoName;

                if (isNew)
                {
                    table.AddTovarRow(row);
                }

                adapter.Update(table);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message, "Критическая ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Удалить товар навсегда?", "Внимание", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            var orderTable = new DataSet.OrderDetailsDataTable();
            new DataSetTableAdapters.OrderDetailsTableAdapter().Fill(orderTable);

            if (orderTable.Select($"Article = '{_prod.Article}'").Length > 0)
            {
                MessageBox.Show("Товар есть в заказах! Удаление запрещено.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            var adapter = new DataSetTableAdapters.TovarTableAdapter();
            var table = new DataSet.TovarDataTable();
            adapter.Fill(table);

            var found = table.FindByArticle(_prod.Article);
            if (found != null)
            {
                found.Delete();
                adapter.Update(table);
            }

            if (!_prod.IsPhotoNull() && _prod.Photo != "picture.png")
            {
                picProduct.Image?.Dispose();
                string path = Path.Combine(_imgDir, _prod.Photo);
                if (File.Exists(path)) File.Delete(path);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDeletePhoto_Click(object sender, EventArgs e)
        {
            string currentPhoto = _prod?.IsPhotoNull() == false ? _prod.Photo : "picture.png";
            if (_newPhotoPath == null && currentPhoto == "picture.png")
            {
                MessageBox.Show("У товара и так нет изображения!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            picProduct.Image?.Dispose();

            string defaultPicPath = Path.Combine(_imgDir, "picture.png");
            if (File.Exists(defaultPicPath))
            {
                using var stream = new FileStream(defaultPicPath, FileMode.Open, FileAccess.Read);
                picProduct.Image = Image.FromStream(stream);
            }
            else
            {
                picProduct.Image = null;
            }

            _newPhotoPath = null;

            if (_prod != null)
            {
                _prod.Photo = "picture.png";
            }

            MessageBox.Show("Фото удалено.", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}