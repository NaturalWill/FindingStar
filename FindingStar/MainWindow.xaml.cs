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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FindingStar
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MCommon.mw = this;
        }

        const string strMaze = "迷宫问题";
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FlashMazeList();
        }


        #region MazeList

        private void btnAddMaze_Click(object sender, RoutedEventArgs e)
        {
            MCommon.mdcur = new MazeData();
            EditMaze();
        }

        Button CreateMazeButton(string text, bool isbold = false)
        {
            Button btn = new Button();
            if (isbold) btn.FontWeight = FontWeights.Bold;
            btn.FontSize = 16.0;
            btn.Background = Brushes.White;
            btn.BorderBrush = new BrushConverter().ConvertFromString("#FFCBCBCB") as SolidColorBrush;
            btn.Content = text;
            return btn;
        }

        private void BtnMaze_Click(object sender, RoutedEventArgs e)
        {
            MCommon.mdcur.ReadMaze(((Button)sender).Content.ToString());
            ViewMaze();

        }

        #endregion
        
        #region panelView


        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            EditMaze();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            NoMaze();
            MCommon.mdcur.deleteMaze();
            FlashMazeList();
        }

        private void btnDepthSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnWidthSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #region panelEdit

        private void btnCreateRandomMaze_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Error prevention
                int width = Convert.ToInt32(tbMazeWidth.Text);
                int height = Convert.ToInt32(tbMazeHeight.Text);
                string mazename = tbMazeNewName.Text.Trim();
                if (width < 3 && height < 3)
                {
                    MessageBox.Show("迷宫大小输入错误");
                    return;
                }
                if (string.IsNullOrWhiteSpace(mazename))
                {
                    MessageBox.Show("请输入迷宫名字");
                    return;
                }

                MazeData md = new MazeData();
                md.CreateMaze(width, height, mazename);

                md.ShowMaze(ref MCommon.pme.gMaze);

                MCommon.mdcur = md;
            }
            catch
            {
                MessageBox.Show("错误操作！");
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            MCommon.mdcur.SaveMaze();
            rereadMaze();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            rereadMaze();
            if (tbMazeName.Text == strMaze) NoMaze();
        }

        #region Only_Number


        //检测粘贴
        private void textBox1_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!isNumberic(text))
                { e.CancelCommand(); }
            }
            else { e.CancelCommand(); }
        }



        private void textBox1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }



        private void textBox1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!isNumberic(e.Text))
            {
                e.Handled = true;
            }
            else
                e.Handled = false;
        }


        //isDigit是否是数字
        public static bool isNumberic(string _string)
        {
            if (string.IsNullOrEmpty(_string))
                return false;
            foreach (char c in _string)
            {
                if (!char.IsDigit(c))
                    //if(c<'0' c="">'9')//最好的方法,在下面测试数据中再加一个0，然后这种方法效率会搞10毫秒左右
                    return false;
            }
            return true;
        }
        #endregion

        #endregion



        private void FlashMazeList()
        {

            MCommon.mdcur = new MazeData();
            string[] mazelist = MCommon.mdcur.getMazeList();

            this.tbMazeName.Text = strMaze;

            this.MazeList.Children.Clear();

            TextBlock tb = new TextBlock();
            tb.Text = "迷宫列表";
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            tb.Margin = new Thickness(10);
            this.MazeList.Children.Add(tb);

            for (int i = 0; i < mazelist.Length; i++)
            {
                Button btn = CreateMazeButton(mazelist[i]);
                btn.Click += BtnMaze_Click;
                this.MazeList.Children.Add(btn);
            }
            Button btnAddMaze = CreateMazeButton("+", true);
            btnAddMaze.Click += btnAddMaze_Click;
            this.MazeList.Children.Add(btnAddMaze);
        }

        void rereadMaze()
        {
            string curMazeName = MCommon.mdcur.MazeName;
            FlashMazeList();
            MCommon.mdcur.ReadMaze(curMazeName);
            ViewMaze();
        }

        public void importMaze(MazeData md)
        {
            if (md != null)
            {
                tbMazeNewName.Text = md.MazeName;
                tbMazeWidth.Text = md.Width.ToString();
                tbMazeHeight.Text = md.Height.ToString();
                md.ShowMaze(ref MCommon.pme.gMaze);
                if (md != MCommon.mdcur)
                    MCommon.mdcur = md;
            }
        }

        public void EditMaze()
        {
            btnEdit.IsEnabled = btnWidthSearch.IsEnabled = btnDepthSearch.IsEnabled = false;
            btnDelete.IsEnabled = true;
            panelViewMaze.Visibility = Visibility.Hidden;
            panelEditMaze.Visibility = Visibility.Visible;
            if (MCommon.pme == null) MCommon.pme = new PageMazeEdit();
            importMaze(MCommon.mdcur);
            frameContent.Navigate(MCommon.pme);
        }

        public void ViewMaze()
        {
            btnEdit.IsEnabled = btnWidthSearch.IsEnabled = btnDepthSearch.IsEnabled = true;
            btnDelete.IsEnabled = true;
            panelViewMaze.Visibility = Visibility.Visible;
            panelEditMaze.Visibility = Visibility.Hidden;
            if (MCommon.pmv == null) MCommon.pmv = new PageMazeView();

            MCommon.mdcur.ShowMaze(ref MCommon.pmv.gMaze);
            frameContent.Navigate(MCommon.pmv);

            this.tbMazeName.Text = string.IsNullOrWhiteSpace(MCommon.mdcur.MazeName) ? strMaze :
                (MCommon.mdcur.MazeName + " (" + MCommon.mdcur.Height + " x " + MCommon.mdcur.Width + ") ");
        }
        private void NoMaze()
        {
            btnWidthSearch.IsEnabled = btnDepthSearch.IsEnabled = btnEdit.IsEnabled = btnDelete.IsEnabled = false;
            frameContent.Navigate(new Page());
        }

    }
}
