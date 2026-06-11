using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WinFormsApp5
{
    public partial class LoginForm : Form
    {
        // На экзамене поменяй Data Source, если сервер называется иначе (смотри в SSMS)
        private const string ConnectionString =
            @"Data Source=POPLPC;Initial Catalog=ShoesStore;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
        /*            @"Data Source=localhost\SQLEXPRESS;Initial Catalog=ShoesStore;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
        */
        public LoginForm()
        {
            InitializeComponent();
            btnLogin.Click += btnLogin_Click;
            btnGuest.Click += btnGuest_Click;
            FormClosed += LoginForm_FormClosed;
            AppStyle.ApplyToForm(this, "Авторизация");
            AppStyle.MakeAccentButton(btnLogin);
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtLogin.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                MessageBox.Show("Введите логин и пароль, или войдите как гость", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = @"SELECT Users.Name, Roles.Role FROM Users
                             JOIN Roles ON Users.Role = Roles.RoleID
                             WHERE Users.Login = @login AND Users.Password = @pass";
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@login", txtLogin.Text);
                    cmd.Parameters.AddWithValue("@pass", txtPassword.Text);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            CurrentSession.UserName = reader["Name"].ToString();
                            CurrentSession.Role = reader["Role"].ToString();

                            Form1 form = new Form1();
                            form.Show();
                            Hide();
                        }
                        else
                        {
                            MessageBox.Show("Неверный логин или пароль.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при подключении к базе данных: " + ex.Message);
            }
        }

        private void btnGuest_Click(object sender, EventArgs e)
        {
            CurrentSession.UserName = CurrentSession.Guest;
            CurrentSession.Role = CurrentSession.Guest;

            Form1 form = new Form1();
            form.Show();
            Hide();
        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
