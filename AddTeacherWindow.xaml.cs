using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Kursovaya
{
    /// <summary>
    /// Interaction logic for AddTeacherWindow.xaml
    /// </summary>
    public partial class AddTeacherWindow : Window
    {
        MyDataBase dataBase;
        public AddTeacherWindow()
        {
            InitializeComponent();
            dataBase = new MyDataBase();
            LoadTextBoxes();
        }
        private void LoadTextBoxes()
        {
            addTeacherGrid.RowDefinitions.Add(new RowDefinition());
            var props = typeof(Teacher).GetProperties();
            for (int i = 1; i < props.Length - 1; i++)
            {
                addTeacherGrid.RowDefinitions.Add(new RowDefinition());
                TextBox tx = new TextBox();
                tx.Text = props[i].Name;
                tx.Margin = new Thickness(15, 15, 15, 15);
                tx.MinWidth = 20;
                tx.TextChanged += TextBoxTextChanged;
                Grid.SetRow(tx, i);
                addTeacherGrid.Children.Add(tx);
            }
            addTeacherGrid.RowDefinitions.Add(new RowDefinition());
            Button bt = new Button();
            bt.Click += AddPerson;
            bt.Margin = new Thickness(15, 15, 15, 15);
            bt.MinWidth = 20;
            bt.Content = "Добавить учителя";
            Grid.SetRow(bt, addTeacherGrid.RowDefinitions.Count);
            addTeacherGrid.Children.Add(bt);
        }
        private void AddPerson(object sender, RoutedEventArgs e)
        {
            dataBase.teachers.Add(new Teacher
            {
                Фамилия = (addTeacherGrid.Children[0] as TextBox).Text,
                Имя = (addTeacherGrid.Children[1] as TextBox).Text,
                Отчество = (addTeacherGrid.Children[2] as TextBox).Text,
                Кабинет = (addTeacherGrid.Children[3] as TextBox).Text,
                //Предметы = new List<Subject>()
            });
            dataBase.SaveChanges();
            MessageBox.Show("Вы добавили учителя!");
            Close();
        }
        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
            {
                var props = typeof(Teacher).GetProperties();
                for (int i = 1; i < props.Length - 1; i++)
                {
                    (addTeacherGrid.Children[i - 1] as TextBox).Text = props[i].Name;
                }
                addTeacherGrid.Children[0].Focus();
            }
        }
        private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = e.Source as TextBox;
            if (tb.Text == "")
            {
                tb.BorderBrush = Brushes.Red;
                tb.BorderThickness = new Thickness(1.01);
                tb.ToolTip = "Поле не должно быть пустым";
            }
            else
            {
                tb.BorderBrush = Brushes.LightGray;
                tb.BorderThickness = new Thickness(1.01);
                tb.ToolTip = null;
            }
        }
        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = e.Source as TextBox;
            if (tb == null) return;
            tb.Select(0, tb.Text.Length);
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
            if (Owner != null) Owner.Activate();
        }
    }
}
