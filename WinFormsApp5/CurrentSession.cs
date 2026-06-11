namespace WinFormsApp5
{
    internal class CurrentSession
    {
        public const string Guest = "Гость";
        public const string Client = "Авторизованный клиент";
        public const string Manager = "Менеджер";
        public const string Admin = "Администратор";

        public static string UserName { get; set; } = Guest;
        public static string Role { get; set; } = Guest;
    }
}
