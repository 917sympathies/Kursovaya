using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Kursovaya
{
    /// <summary>
    /// Interaction logic for StudentInfoWindow.xaml
    /// </summary>
    public partial class StudentInfoWindow : Window, IUser
    {
        private MyDataBase dataBase;
        private Student stud;
        private Teacher loggedTeacher;

        public int Quarter1 { get; set; }
        public int Quarter2 { get; set; }
        public int Quarter3 { get; set; }
        public int Quarter4 { get; set; }
        public int Final { get; set; }
        public User CurrentUser { get; set; }

        public StudentInfoWindow(Student student, User user) : this(student, user, null) { }
        public StudentInfoWindow(Student student, User user, Teacher loggedTeacher)
        {
            InitializeComponent();
            stud = student;
            CurrentUser = user;
            this.loggedTeacher = loggedTeacher;
            dataBase = new MyDataBase();
            CheckUser();
        }
        public void CheckUser()
        {
            if (CurrentUser == User.Guest)
            {
                marksList.IsReadOnly = true;
                giveMarks.Visibility = Visibility.Hidden;
                stopEdit.Visibility = Visibility.Hidden;
            }
            else if(CurrentUser == User.Teacher)
            {
                marksList.IsReadOnly = false;
                giveMarks.Visibility = Visibility.Visible;
                stopEdit.Visibility = Visibility.Visible;
            }
            else
            {
                marksList.IsReadOnly = false;
                giveMarks.Visibility = Visibility.Visible;
                stopEdit.Visibility = Visibility.Visible;
            }
        }
        private void FillDataGrid()
        {
            int countOfSubjects = 0;
            if (loggedTeacher != null) countOfSubjects = loggedTeacher.Предметы.Count;
            else countOfSubjects = dataBase.subjects.Count();
            for (int i = 0; i < countOfSubjects; i++)
            {
                DataGridCell idCell = GetCell(i, marksList.Columns.Count - 2);
                TextBlock idTb = idCell.Content as TextBlock;
                int subjID = int.Parse(idTb.Text);
                for (int j = 0; j < marksList.Columns.Count - 2; j++)
                {
                    DataGridCell cell = GetCell(i, j);
                    Mark tempMark = dataBase.marks.FirstOrDefault(w => w.StudentId == stud.Id && w.Quarter == j + 1 && w.SubjectId == subjID);
                    if (tempMark == null || tempMark.Value == 0)
                        cell.Content = "";
                    else
                        cell.Content = tempMark.Value;
                }
            }
        }
        private void giveMarks_Click(object sender, RoutedEventArgs e)
        {
            int countOfSubjects = 0;
            if (loggedTeacher != null) countOfSubjects = loggedTeacher.Предметы.Count;
            else countOfSubjects = dataBase.subjects.Count();
            for (int i = 0; i < countOfSubjects;i++)
            {
                DataGridCell idCell = GetCell(i, marksList.Columns.Count - 2);
                TextBlock idTb = idCell.Content as TextBlock;
                int subjID = int.Parse(idTb.Text);
                var marks = dataBase.marks.Where(w => w.StudentId == stud.Id && w.SubjectId == subjID && w.Quarter <= 4).ToArray();
                if (marks.Length < 4 || dataBase.marks.Where(w => w.StudentId == stud.Id && w.SubjectId == subjID && w.Quarter <= 4 && w.Value == 0).Count() != 0)
                {
                    var subject = dataBase.subjects.Find(subjID);
                    MessageBox.Show($"Недостаточно оценок для выведения итоговой оценки по предмету {subject.Name}!");
                    continue;
                }
                else if (dataBase.marks.FirstOrDefault(w => w.StudentId == stud.Id && w.SubjectId == subjID && w.Quarter == 5 && w.Value != 0) != null)
                {
                    var subject = dataBase.subjects.Find(subjID);
                    if (MessageBox.Show($"Оценка по предмету {subject.Name} уже выведена! Желаете вывести заного?", "Оценка уже выведена", MessageBoxButton.YesNo) == MessageBoxResult.No) return;
                }
                double finalMark = 0;
                foreach (var t in marks)
                {
                    finalMark += t.Value;
                }
                finalMark = Math.Round(finalMark / (double)4 + 0.0001);
                DataGridCell finalMarkCell = GetCell(i, 4);
                finalMarkCell.Content = finalMark;
            }
            Save();
        }
        private void updateMarks_Click(object sender, RoutedEventArgs e)
        {
            FillDataGrid();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FillDataGrid();
        }
        private void Save()
        {
            for (int i = 0; i < marksList.Items.Count; i++)
            {
                DataGridCell idCell = GetCell(i, marksList.Columns.Count - 2);
                TextBlock idTb = idCell.Content as TextBlock;
                int subjID = int.Parse(idTb.Text);

                for (int j = 0; j < marksList.Columns.Count - 2; j++)
                {
                    int markValue = 0;
                    DataGridCell cell = GetCell(i, j);
                    TextBlock tb = cell.Content as TextBlock;
                    if (tb == null)
                    {
                        if (cell.Content != "")
                            markValue = int.Parse(cell.Content.ToString());
                    }
                    else
                    {
                        try
                        {
                            if (tb.Text != "")
                                markValue = int.Parse(tb.Text);
                            //int.TryParse(tb.Text, out markValue);
                        }
                        catch (Exception exc)
                        {
                            MessageBox.Show(exc.Message);
                            return;
                        }
                    }

                    Mark tempMark = dataBase.marks.FirstOrDefault(w => w.StudentId == stud.Id && w.Quarter == j + 1 && w.SubjectId == subjID);
                    if (tempMark == default(Mark))
                    {
                        dataBase.marks.Add(new Mark() { StudentId = stud.Id, SubjectId = subjID, Quarter = j + 1, Value = markValue });
                    }
                    else
                    {
                        tempMark.Value = markValue;
                    }
                }
            }
            dataBase.SaveChanges();
        }
        private void stopEdit_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }
        private DataGridCell GetCell(int row, int column)
        {
            DataGridRow rowData = GetRow(row);
            if (rowData != null)
            {
                DataGridCellsPresenter cellPresenter = GetVisualChild<DataGridCellsPresenter>(rowData);
                DataGridCell cell = (DataGridCell)cellPresenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    marksList.ScrollIntoView(rowData, marksList.Columns[column]);
                    cell = (DataGridCell)cellPresenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
                return cell;
            }
            return null;
        }
        private DataGridRow GetRow(int index)
        {
            DataGridRow row = (DataGridRow)marksList.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                marksList.UpdateLayout();
                marksList.ScrollIntoView(marksList.Items[index]);
                row = (DataGridRow)marksList.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }
        private static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
    }
}
