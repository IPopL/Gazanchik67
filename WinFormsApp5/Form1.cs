using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp5
{
    public partial class Form1 : Form
    {
        private DataSet.TovarDataTable tovarTable;

        public Form1()
        {
            InitializeComponent();
            btnAdd.Click += btnAdd_Click;
            btnExit.Click += btnExit_Click;
            FormClosed += Form1_FormClosed;
            txtSearch.TextChanged += txtSearch_TextChanged;
            cmbFilterAmount.SelectedIndexChanged += cmbFilterAmount_SelectedIndexChanged;
            cmbFilterSuppliers.SelectedIndexChanged += cmbFilterSuppliers_SelectedIndexChanged;

            SetupStyle();
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.WrapContents = false;

            cmbFilterAmount.Items.AddRange(new object[] { "Без сортировки", "По возрастанию", "По убыванию" });
            cmbFilterAmount.SelectedIndex = 0;

            lblName.Text = "ФИО: " + CurrentSession.UserName;
            lblRole.Text = "Роль: " + CurrentSession.Role;

            RoleAccesibility();
            LoadSuppliersFilter();
            LoadProducts();
        }

        private void SetupStyle()
        {
            AppStyle.ApplyToForm(this, "Список товаров");
            AppStyle.MakeAccentButton(btnAdd);

            Panel header = new Panel();
            header.Name = "pnlHeader";
            header.Location = new Point(0, 0);
            header.Size = new Size(ClientSize.Width, 52);
            header.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            AppStyle.MakeSecondaryPanel(header);

            PictureBox logo = new PictureBox();
            logo.SizeMode = PictureBoxSizeMode.Zoom;
            logo.Location = new Point(8, 6);
            logo.Size = new Size(130, 40);
            logo.BackColor = Color.Transparent;
            Image logoImage = AppStyle.LoadLogo();
            if (logoImage != null)
            {
                logo.Image = logoImage;
            }
            header.Controls.Add(logo);

            btnAdd.Location = new Point(350, 9);
            btnAdd.Parent = header;

            lblRole.Location = new Point(500, 10);
            lblRole.BackColor = AppStyle.SecondaryBackground;
            lblRole.Parent = header;

            lblName.Location = new Point(500, 30);
            lblName.BackColor = AppStyle.SecondaryBackground;
            lblName.Parent = header;

            btnExit.Location = new Point(ClientSize.Width - 92, 10);
            btnExit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExit.Parent = header;

            Controls.Add(header);
            header.SendToBack();
        }

        public void LoadProducts()
        {
            DataSet dataSetInstance = new DataSet();

            new DataSetTableAdapters.TovarTableAdapter().Fill(dataSetInstance.Tovar);
            new DataSetTableAdapters.ManufacturersTableAdapter().Fill(dataSetInstance.Manufacturers);
            new DataSetTableAdapters.CategoriesTableAdapter().Fill(dataSetInstance.Categories);
            new DataSetTableAdapters.SuppliersTableAdapter().Fill(dataSetInstance.Suppliers);

            tovarTable = dataSetInstance.Tovar;
            ApplyFilters();
        }

        public void LoadSuppliersFilter()
        {
            DataSet.SuppliersDataTable table = new DataSet.SuppliersDataTable();
            new DataSetTableAdapters.SuppliersTableAdapter().Fill(table);

            DataSet.SuppliersRow row = table.NewSuppliersRow();
            row.ID = -1;
            row.Supplier = "Все поставщики";
            table.Rows.InsertAt(row, 0);

            cmbFilterSuppliers.DataSource = table;
            cmbFilterSuppliers.DisplayMember = "Supplier";
            cmbFilterSuppliers.ValueMember = "ID";
            cmbFilterSuppliers.SelectedIndex = 0;
        }

        public void ApplyFilters()
        {
            if (tovarTable == null)
            {
                return;
            }

            flowLayoutPanel1.Controls.Clear();

            DataView view = new DataView(tovarTable);
            string search = txtSearch.Text.Trim().Replace("'", "''");
            string filterString = "";

            if (!string.IsNullOrEmpty(search))
            {
                filterString =
                    $"(Name LIKE '%{search}%' OR Description LIKE '%{search}%' OR QuantityMeasure LIKE '%{search}%' OR Article LIKE '%{search}%')";
            }

            if (cmbFilterSuppliers.SelectedValue != null && (int)cmbFilterSuppliers.SelectedValue != -1)
            {
                if (!string.IsNullOrEmpty(filterString))
                {
                    filterString += " AND ";
                }

                filterString += $"SupplierID = {cmbFilterSuppliers.SelectedValue}";
            }

            view.RowFilter = filterString;

            if (cmbFilterAmount.SelectedIndex == 1)
            {
                view.Sort = "Amount ASC";
            }
            else if (cmbFilterAmount.SelectedIndex == 2)
            {
                view.Sort = "Amount DESC";
            }

            foreach (DataRowView rowView in view)
            {
                ProductCard card = new ProductCard();
                card.FillData((DataSet.TovarRow)rowView.Row);
                flowLayoutPanel1.Controls.Add(card);
            }
        }

        private void RoleAccesibility()
        {
            bool isGuestOrClient =
                CurrentSession.Role == CurrentSession.Guest ||
                CurrentSession.Role == CurrentSession.Client;

            txtSearch.Visible = !isGuestOrClient;
            cmbFilterAmount.Visible = !isGuestOrClient;
            cmbFilterSuppliers.Visible = !isGuestOrClient;
            label2.Visible = !isGuestOrClient;
            label3.Visible = !isGuestOrClient;
            label4.Visible = !isGuestOrClient;
            btnAdd.Visible = CurrentSession.Role == CurrentSession.Admin;

            if (isGuestOrClient)
            {
                label1.Text = "Режим просмотра. Редактирование — у администратора (клик по карточке).";
                label1.AutoSize = true;
                label1.Location = new Point(12, 58);
                label1.Visible = true;
                label2.Visible = false;
                label3.Visible = false;
                label4.Visible = false;
                txtSearch.Visible = false;
                cmbFilterAmount.Visible = false;
                cmbFilterSuppliers.Visible = false;
                flowLayoutPanel1.Location = new Point(12, 82);
                flowLayoutPanel1.Size = new Size(816, 364);
            }
            else
            {
                label1.Visible = false;
                flowLayoutPanel1.Location = new Point(12, 118);
                flowLayoutPanel1.Size = new Size(816, 328);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (new ProductEditForm().ShowDialog() == DialogResult.OK)
            {
                LoadProducts();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            new LoginForm().Show();
            Hide();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e) => Application.Exit();
        private void txtSearch_TextChanged(object sender, EventArgs e) => ApplyFilters();
        private void cmbFilterAmount_SelectedIndexChanged(object sender, EventArgs e) => ApplyFilters();
        private void cmbFilterSuppliers_SelectedIndexChanged(object sender, EventArgs e) => ApplyFilters();
    }
}
