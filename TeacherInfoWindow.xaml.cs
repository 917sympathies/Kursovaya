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
    /// Interaction logic for TeacherInfoWindow.xaml
    /// </summary>
    public partial class TeacherInfoWindow : Window
    {
        Teacher teacher;
        Subject selectedSubject;
        MyDataBase dataBase;
        SubjectsWindow subjectsWindow;
        public TeacherInfoWindow(Teacher teacher)
        {
            InitializeComponent();
            this.teacher = teacher;
            LoadSubjects();
        }

        private void LoadSubjects()
        {
            dataBase = new MyDataBase();
            var tempTeacher = dataBase.teachers.Include(w => w.Предметы).FirstOrDefault(w => w.Id == teacher.Id);
            if (teacher.Предметы == null) return;
            var queue = from t in tempTeacher.Предметы
                        select new
                        {
                            id = t.Id,
                            Предмет = t.Name
                        };
            subjectsList.ItemsSource = queue.ToArray();
        }

        private void addSubject_Click(object sender, RoutedEventArgs e)
        {
            selectedSubject = null;
            subjectsWindow = new SubjectsWindow(this, teacher);
            subjectsWindow.Owner = this;
            subjectsWindow.Show();
        }

        private void deleteSubject_Click(object sender, RoutedEventArgs e)
        {
            if (selectedSubject == null)
            {
                MessageBox.Show("Выберите предмет!");
                return;
            }
            dataBase = new MyDataBase();
            var tempTeacher = dataBase.teachers.Include(w => w.Предметы).FirstOrDefault(w => w.Id == teacher.Id);
            var newSubjectsList = new List<Subject>();
            var newTeachersList = new List<Teacher>();
            for (int i = 0; i < tempTeacher.Предметы.Count; i++)
            {
                if (tempTeacher.Предметы[i].Id != selectedSubject.Id)
                    newSubjectsList.Add(tempTeacher.Предметы[i]);
            }
            for (int i = 0; i < selectedSubject.Teachers.Count; i++)
            {
                if (selectedSubject.Teachers[i].Id != tempTeacher.Id)
                    newTeachersList.Add(selectedSubject.Teachers[i]);
            }
            tempTeacher.Предметы = newSubjectsList;
            selectedSubject.Teachers = newTeachersList;
            dataBase.SaveChanges();
            LoadSubjects();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            LoadSubjects();
        }

        private void subjectsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (subjectsList.SelectedItem == null) return;
            var idToFind = int.Parse(subjectsList.SelectedItem.ToString().Split(',')[0].Split(' ').Last());
            dataBase = new MyDataBase();
            selectedSubject = dataBase.subjects.Find(idToFind);
        }
    }
}
