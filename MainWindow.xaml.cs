using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;

namespace Kursovaya
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ListWindow lWindow;
        private User currentUser;
        private Teacher loggedTeacher;
        public MainWindow()
        {
            InitializeComponent();
            currentUser = User.Guest;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            lWindow = new ListWindow(currentUser);
            lWindow.Owner = this;
            lWindow.Subject = ListSubject.Teacher;
            lWindow.Show();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (loggedTeacher == null)
                lWindow = new ListWindow(currentUser);
            else
                lWindow = new ListWindow(currentUser, loggedTeacher);
            lWindow.Owner = this;
            lWindow.Subject = ListSubject.Student;
            lWindow.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            if(loginTextBox.Text == "")
            {
                MessageBox.Show("Введите логин!");
                return;
            }
            if(passTextBox.Text == "")
            {
                MessageBox.Show("Введите пароль!");
                return;
            }
            if (loginTextBox.Text == "admin" && passTextBox.Text == "admin")
            {
                currentUser = User.HeadTeacher;
                MessageBox.Show("Вы вошли как завуч!");
                logoutButton.Visibility = Visibility.Visible;
                loginButton.Visibility = Visibility.Hidden;
                loginTextBox.Visibility = Visibility.Hidden;
                passTextBox.Visibility = Visibility.Hidden;
                loginTextBox.Text = "";
                passTextBox.Text = "";
                return;
            }
            var dataBase = new MyDataBase();
            var logNpass = dataBase.logsAndPass.FirstOrDefault(w => w.Login == loginTextBox.Text && w.Password == passTextBox.Text);
            if(logNpass == null)
            {
                MessageBox.Show("Такого пользователя нет!");
                return;
            }
            int userId = logNpass.UserId;
            var teacher = dataBase.teachers.Include(w => w.Предметы).FirstOrDefault(w => w.Id == userId);
            MessageBox.Show($"Добро пожаловать {teacher.Фамилия} {teacher.Имя}!");
            loggedTeacher = teacher;
            currentUser = User.Teacher;
            logoutButton.Visibility = Visibility.Visible;
            loginButton.Visibility = Visibility.Hidden;
            loginTextBox.Visibility = Visibility.Hidden;
            passTextBox.Visibility = Visibility.Hidden;
            loginTextBox.Text = "";
            passTextBox.Text = "";
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            loggedTeacher = null;
            currentUser = User.Guest;
            logoutButton.Visibility = Visibility.Hidden;
            loginButton.Visibility = Visibility.Visible;
            loginTextBox.Visibility = Visibility.Visible;
            passTextBox.Visibility = Visibility.Visible;
        }
    }
    public enum User
    {
        Guest,
        Teacher,
        HeadTeacher
    }
    public enum ListSubject
    {
        Teacher,
        Student
    }
    public interface IUser
    {
        public void CheckUser();
        public User CurrentUser { get; set; }
    }
}
