using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace Kursovaya
{
    /// <summary>
    /// Interaction logic for ListWindow.xaml
    /// </summary>
    public partial class ListWindow : Window
    {
        private User currUser;
        private ListSubject subject;
        private MyDataBase dataBase;
        private Teacher selectedTeacher;
        private Student selectedStudent;
        private Teacher loggedTeacher;
        private AddStudentWindow addStudentWindow;
        private AddTeacherWindow addTeacherWindow;
        private TeacherInfoWindow teacherInfoWindow;
        private StudentInfoWindow studentInfoWindow;
        private ListSubject Subject
        {
            get
            {
                return subject;
            }
            set
            {
                subject = value;
                FillDataGridView();
            }
        }
        private User CurrentUser
        {
            get
            {
                return currUser;
            }
            set
            {
                currUser = value;
                if (value == User.Guest || value == User.Teacher)
                    add1.Visibility = Visibility.Hidden;
            }
        }

        public ListWindow(ListSubject subj, User user) : this(subj, user, null) { }
        public ListWindow(ListSubject subj, User user, Teacher logTeacher)
        {
            InitializeComponent();
            dataBase = new MyDataBase();
            CurrentUser = user;
            Subject = subj;
            loggedTeacher = logTeacher;
            addStudentWindow = new AddStudentWindow();
            addTeacherWindow = new AddTeacherWindow();
        }

        private void FillDataGridView()
        {
            if (Subject == ListSubject.Student)
            {
                Title = "Список учеников";
                add1.Header = "Добавить ученика";
                var queue = from t in dataBase.students
                            select new
                            {
                                id = t.Id,
                                Фамилия = t.Фамилия,
                                Имя = t.Имя,
                                Класс = t.Класс
                            };
                dgList.ItemsSource = queue.ToArray();
            }
            else
            {
                Title = "Список учителей";
                add1.Header = "Добавить учителя";
                var queue = from t in dataBase.teachers
                            select new
                            {
                                id = t.Id,
                                Фамилия = t.Фамилия,
                                Имя = t.Имя,
                                Отчество = t.Отчество,
                                Кабинет = t.Кабинет
                            };
                dgList.ItemsSource = queue.ToArray();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            addStudentWindow.Owner = this;
            addTeacherWindow.Owner = this;
        }

        private void add1_Click(object sender, RoutedEventArgs e)
        {
            if (Subject == ListSubject.Student) addStudentWindow.Show();
            if (Subject == ListSubject.Teacher) addTeacherWindow.Show();
            delete1.Visibility = Visibility.Hidden;
            info1.Visibility = Visibility.Hidden;
            dgList.SelectedItem = null;
        }

        private void update1_Click(object sender, RoutedEventArgs e)
        {
            UpdateList();
            delete1.Visibility = Visibility.Hidden;
            info1.Visibility = Visibility.Hidden;
            dgList.SelectedItem = null;
        }

        private void UpdateList()
        {
            if (Subject == ListSubject.Student)
            {
                var queue = from t in dataBase.students
                            select new
                            {
                                id = t.Id,
                                Фамилия = t.Фамилия,
                                Имя = t.Имя,
                                Класс = t.Класс
                            };
                dgList.ItemsSource = queue.ToArray();
            }
            if (Subject == ListSubject.Teacher)
            {
                var queue = from t in dataBase.teachers
                            select new
                            {
                                id = t.Id,
                                Фамилия = t.Фамилия,
                                Имя = t.Имя,
                                Отчество = t.Отчество,
                                Кабинет = t.Кабинет
                            };
                dgList.ItemsSource = queue.ToArray();
            }
        }

        private void delete1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Subject == ListSubject.Student)
                {
                    dataBase.students.Remove(dataBase.students.Find(selectedStudent.Id));
                    MessageBox.Show("Вы удалили ученика!");
                }
                if (Subject == ListSubject.Teacher)
                {
                    var teacherToDelete = dataBase.teachers.Find(selectedTeacher.Id);
                    dataBase.teachers.Remove(teacherToDelete);
                    MessageBox.Show("Вы удалили учителя!");
                }
                dataBase.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message);
            }
            UpdateList();
            dgList.SelectedItem = null;
            delete1.Visibility = Visibility.Hidden;
            info1.Visibility = Visibility.Hidden;
        }

        private void dgList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgList.SelectedItem == null) return;
            int idToFind = int.Parse(dgList.SelectedItem.ToString().Split(',')[0].Split(' ').Last());
            if (Subject == ListSubject.Student) selectedStudent = dataBase.students.Find(idToFind);
            if (Subject == ListSubject.Teacher) selectedTeacher = dataBase.teachers.Include(w => w.Предметы).FirstOrDefault(w => w.Id == idToFind);
            if (dgList.SelectedItem == null)
            {
                delete1.Visibility = Visibility.Hidden;
                info1.Visibility = Visibility.Hidden;
            }
            else
            {
                if(CurrentUser == User.HeadTeacher) delete1.Visibility = Visibility.Visible;
                info1.Visibility = Visibility.Visible;
            }
        }

        private void info1_Click(object sender, RoutedEventArgs e)
        {
            if (Subject == ListSubject.Teacher)
            {
                if (selectedTeacher == null) return;
                teacherInfoWindow = new TeacherInfoWindow(selectedTeacher, CurrentUser);
                teacherInfoWindow.Title = $"{selectedTeacher.Фамилия} {selectedTeacher.Имя} {selectedTeacher.Отчество}";
                teacherInfoWindow.Show();
            }
            else
            {
                if (selectedStudent == null) return;
                studentInfoWindow = new StudentInfoWindow(selectedStudent, CurrentUser, loggedTeacher);
                studentInfoWindow.Owner = this;
                if (loggedTeacher == null)
                {
                    var queue = from t in dataBase.subjects
                                select new
                                {
                                    id = t.Id,
                                    Предмет = t.Name
                                };
                    studentInfoWindow.marksList.ItemsSource = queue.ToArray();
                }
                else
                {
                    var queue = from t in loggedTeacher.Предметы
                                select new
                                {
                                    id = t.Id,
                                    Предмет = t.Name
                                };

                    studentInfoWindow.marksList.ItemsSource = queue.ToArray();
                }
                studentInfoWindow.Title = $"{selectedStudent.Фамилия} {selectedStudent.Имя}";
                studentInfoWindow.Show();
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            UpdateList();
        }
    }
}
