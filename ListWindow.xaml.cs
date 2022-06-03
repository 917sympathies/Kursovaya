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
        MyDataBase dataBase;

        Teacher selectedTeacher;
        Student selectedStudent;
        //DataGridRow SelectedRow;

        AddStudentWindow addStudentWindow;
        AddTeacherWindow addTeacherWindow;
        TeacherInfoWindow teacherInfoWindow;
        StudentInfoWindow studentInfoWindow;

        public ListSubject Subject { get; set; }

        public ListWindow()
        {
            InitializeComponent();
            dataBase = new MyDataBase();
            addStudentWindow = new AddStudentWindow();
            addTeacherWindow = new AddTeacherWindow();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
            Owner.Activate();
        }

        private void FillDataGridView()
        {
            if (Subject == ListSubject.Student)
            {
                Title = "Список учеников";
                file1.Header = "Добавить ученика";
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
                file1.Header = "Добавить учителя";
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

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
            {
                FillDataGridView();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            addStudentWindow.Owner = this;
            addTeacherWindow.Owner = this;
        }

        private void file1_Click(object sender, RoutedEventArgs e)
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

        private void dgList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //SelectedRow = ItemsControl.ContainerFromElement((DataGrid)sender,
            //                            e.OriginalSource as DependencyObject) as DataGridRow;
            //if (SelectedRow == null) return;
            //delete1.Visibility = Visibility.Visible;
            //info1.Visibility = Visibility.Visible;
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
                //var idToDelete = int.Parse(SelectedRow.DataContext.ToString().Split(',')[0].Split(' ').Last());
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
            //SelectedRow = null;
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
            //MessageBox.Show(idToFind);
            if (dgList.SelectedItem == null)
            {
                delete1.Visibility = Visibility.Hidden;
                info1.Visibility = Visibility.Hidden;
            }
            else
            {
                delete1.Visibility = Visibility.Visible;
                info1.Visibility = Visibility.Visible;
            }
        }

        private void info1_Click(object sender, RoutedEventArgs e)
        {
            if (Subject == ListSubject.Teacher)
            {
                if (selectedTeacher == null) return;
                //var smth = SelectedRow.DataContext.ToString().Split(',');
                //var surname = smth[1].Split('=')[1].Trim();
                //var name = smth[2].Split('=')[1].Trim();
                //var fatherName = smth[3].Split('=')[1].Trim();
                //var idToFind = int.Parse(SelectedRow.DataContext.ToString().Split(',')[0].Split(' ').Last());
                //var teacher = dataBase.teachers.Include(w => w.Предметы).FirstOrDefault(w => w.Id == idToFind);
                teacherInfoWindow = new TeacherInfoWindow(selectedTeacher);
                teacherInfoWindow.Title = $"{selectedTeacher.Фамилия} {selectedTeacher.Имя} {selectedTeacher.Отчество}";
                teacherInfoWindow.Show();
            }
            else
            {
                if (selectedStudent == null) return;
                //var smth = SelectedRow.DataContext.ToString().Split(',');
                //var surname = smth[1].Split('=')[1].Trim();
                //var name = smth[2].Split('=')[1].Trim();
                //var idToFind = int.Parse(SelectedRow.DataContext.ToString().Split(',')[0].Split(' ').Last());
                //var student = dataBase.students.Find(idToFind);
                studentInfoWindow = new StudentInfoWindow(selectedStudent);
                studentInfoWindow.Owner = this;
                var queue = from t in dataBase.subjects
                            select new
                            {
                                id = t.Id,
                                Предмет = t.Name
                            };
                studentInfoWindow.marksList.ItemsSource = queue.ToArray();
                //studentInfoWindow.marksList.ItemsSource = dataBase.subjects.ToArray();
                studentInfoWindow.Title = $"{selectedStudent.Фамилия} {selectedStudent.Имя}";
                studentInfoWindow.Show();
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            UpdateList();
        }
    }
    public enum ListSubject
    {
        Teacher,
        Student
    }
}
