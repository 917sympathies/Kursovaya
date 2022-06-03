using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;

namespace Kursovaya
{
    /// <summary>
    /// Interaction logic for SubjectsWindow.xaml
    /// </summary>
    public partial class SubjectsWindow : Window
    {
        MyDataBase dataBase;
        TeacherInfoWindow teacherInfoWindow;
        Teacher teacher;
        public SubjectsWindow(TeacherInfoWindow window, Teacher teacher)
        {
            InitializeComponent();
            dataBase = new MyDataBase();
            teacherInfoWindow = window;
            this.teacher = teacher;
            dataBase.subjects.Load();
            addSubjectTextBox.ToolTip = "Введите название предмета и нажмите кнопку ниже, чтобы добавить предмет в общий список";
            var queue = from t in dataBase.subjects
                        select new
                        {
                            id = t.Id,
                            Название = t.Name
                        };
            subjectsList.ItemsSource = queue.ToArray();
        }

        //public void SetWindow (TeacherInfoWindow window)
        //{
        //    teacherInfoWindow = window;
        //}

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
            if (Owner != null) Owner.Activate();
        }

        private void addSubject_Click(object sender, RoutedEventArgs e)
        {
            if (addSubjectTextBox.Text == "")
            {
                MessageBox.Show("Введите название предмета!");
                return;
            }
            //dataBase.subjects.Add(new Subject { Name = addSubjectTextBox.Text, Teachers = new List<Teacher>()});
            dataBase.subjects.Add(new Subject { Name = addSubjectTextBox.Text });
            dataBase.SaveChanges();
            var queue = from t in dataBase.subjects
                        select new
                        {
                            id = t.Id,
                            Название = t.Name
                        };
            subjectsList.ItemsSource = queue.ToArray();
            addSubjectTextBox.Text = "";
        }

        private void subjectsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow selectedRow = ItemsControl.ContainerFromElement((DataGrid)sender,
                                        e.OriginalSource as DependencyObject) as DataGridRow;
            if (selectedRow == null) return;
            var idToFind = int.Parse(selectedRow.DataContext.ToString().Split(',')[0].Split(' ').Last());
            var subj = dataBase.subjects.Find(idToFind);
            var curTeacher = dataBase.teachers.Find(teacher.Id);
            curTeacher.Предметы.Add(subj);
            subj.Teachers.Add(curTeacher);
            //if(curTeacher.Предметы != null)
            //    curTeacher.Предметы.Add(subj);
            //else
            //{
            //    curTeacher.Предметы = new List<Subject>();
            //    curTeacher.Предметы.Add(subj);
            //}
            dataBase.SaveChanges();
            Close();
        }
    }
}