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

namespace Kursovaya
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ListWindow lWindow;
        private User user;
        public MainWindow()
        {
            InitializeComponent();
            user = User.Guest;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            lWindow = new ListWindow(user);
            lWindow.Owner = this;
            lWindow.Subject = ListSubject.Teacher;
            lWindow.Show();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            lWindow = new ListWindow(user);
            lWindow.Owner = this;
            lWindow.Subject = ListSubject.Student;
            lWindow.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
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
