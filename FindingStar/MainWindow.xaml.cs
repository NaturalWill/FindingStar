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
            mw = this;
        }
        public static MainWindow mw;
        MazeData md;
        MazePath mp;
        const string strMaze = "迷宫问题";
        bool isEditing = false;

        Ellipse cMove;//起点
        System.Windows.Shapes.Path cEnd;//终点
        List<Rectangle> listRect;//矩形列表

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            frameContent.ColumnDefinitions[0].Width = new GridLength(0d);
            FlashMazeList();
        }

        #region MazeList

        private void MazeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.MazeList.SelectedIndex >= 0)
            {
                md.ReadMaze(((TextBlock)MazeList.SelectedItem).Text);
                ViewMaze();
                initMazeGrid();
            }
            else NoMaze();
        }
        private void btnAddMaze_Click(object sender, RoutedEventArgs e)
        {
            md = new MazeData();
            initMazeGrid();
            EditMaze();
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
            md.deleteMaze();
            initMazeGrid();
            FlashMazeList();
        }

        private void btnDepthSearch_Click(object sender, RoutedEventArgs e)
        {
            mp = new MazePath(md, ref gMaze);
            mp.findMazeWay(1);

        }

        private void btnWidthSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        private delegate void DelegateFillGrid(Point pt, Brush b);                //定义委托
        public void FillGridInvoke(Point pt, Brush b)                                      //委托访问接口
        {
            DelegateFillGrid d = fillgrid;
            this.Dispatcher.Invoke(d);
        }

        void fillgrid(Point pt, Brush b)
        {
            ((Rectangle)gMaze.FindName(MCommon.getCName(pt))).Fill = b;
        }

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

                md = new MazeData();
                md.CreateMaze(width, height, mazename);

                initMazeGrid();

            }
            catch
            {
                MessageBox.Show("错误操作！");
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            md.SaveMaze();
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



        public void initMazeGrid()
        {
            #region gMaze

            gMaze.Children.Clear();
            gMaze.ColumnDefinitions.Clear();
            gMaze.RowDefinitions.Clear();

            if (md == null) return;

            gMaze.Width = md.Width * md.MazeGridLenght;
            gMaze.Height = md.Height * md.MazeGridLenght;


            for (int r = 0; r < md.Height; r++)
                gMaze.RowDefinitions.Add(new RowDefinition());

            for (int c = 0; c < md.Width; c++)
                gMaze.ColumnDefinitions.Add(new ColumnDefinition());

            Point p = new Point();
            for (p.Y = 0; p.Y < md.Height; p.Y++)
            {
                for (p.X = 0; p.X < md.Width; p.X++)
                {
                    Rectangle r = new Rectangle();
                    r.Height = r.Width = md.MazeGridLenght;
                    r.Name = MCommon.getCName(p);
                    //r.StrokeThickness = 0;
                    if (md.MazeString[p.Y * md.Width + p.X] == '1')
                        r.Fill = MCommon.getCellColor(CellColor.access);
                    else
                        r.Fill = MCommon.getCellColor(CellColor.obstacle);

                    r.MouseDown += R_MouseDown;

                    gMaze.Children.Add(r);

                    MoveToPoint(p, r);
                }
            }

            if (cMove == null || cEnd == null)
            {
                MazeControl m = new MazeControl();
                cMove = m.ellipseMove;
                cEnd = m.pathStar;

                m.sPanel.Children.Remove(cMove);
                m.sPanel.Children.Remove(cEnd);
            }

            gMaze.Children.Add(cMove);
            gMaze.Children.Add(cEnd);

            #endregion


            tbMazeName.Text = string.IsNullOrWhiteSpace(md.MazeName) ? strMaze :
                (md.MazeName + " (" + md.Height + " x " + md.Width + ") ");
            tbMazeNewName.Text = md.MazeName;
            tbMazeWidth.Text = md.Width.ToString();
            tbMazeHeight.Text = md.Height.ToString();

            autoShowGrid();
        }

        private void R_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isEditing)
            {
                Rectangle r = (Rectangle)sender;
                Point p = MCommon.restoreCName(r.Name);

            }
        }

        private void autoShowGrid()
        {
            if (md == null) return;
            if (md.IsEmptyMaze)
            {
                cMove.Visibility = cEnd.Visibility = Visibility.Hidden;
                frameContent.ColumnDefinitions[0].Width = new GridLength(0d);
            }
            else
            {
                MoveToPoint(md.startPoint, cMove);
                MoveToPoint(md.endPoint, cEnd);
                cMove.Visibility = cEnd.Visibility = Visibility.Visible;
                if (isEditing)
                {
                    frameContent.ColumnDefinitions[0].Width = new GridLength(100d);
                }
                else
                {
                    frameContent.ColumnDefinitions[0].Width = new GridLength(0d);
                }
            }
        }

        /// <summary>
        /// Set the position in grid
        /// </summary>
        /// <param name="p"></param>
        /// <param name="s"></param>
        private static void MoveToPoint(Point p, Shape s)
        {
            s.SetValue(Grid.ColumnProperty, p.X);
            s.SetValue(Grid.RowProperty, p.Y);
        }

        private void FlashMazeList()
        {
            try
            {
                md = new MazeData();
                string[] mazelist = MazeData.getMazeList();
                this.tbMazeName.Text = strMaze;

                this.MazeList.Items.Clear();

                for (int i = 0; i < mazelist.Length; i++)
                {
                    TextBlock tb = new TextBlock();
                    tb.Text = mazelist[i];
                    this.MazeList.Items.Add(tb);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("刷新错误", ex);
            }
        }

        void rereadMaze()
        {
            string curMazeName = md.MazeName;
            isEditing = false;
            FlashMazeList();
            md.ReadMaze(curMazeName);
            initMazeGrid();
            ViewMaze();
        }


        public void EditMaze()
        {
            if (md != null)
            {
                btnEdit.IsEnabled = btnWidthSearch.IsEnabled = btnDepthSearch.IsEnabled = false;
                btnDelete.IsEnabled = true;
                panelViewMaze.Visibility = Visibility.Hidden;
                panelEditMaze.Visibility = Visibility.Visible;
                isEditing = true;
                autoShowGrid();
            }
        }
        public void ViewMaze()
        {
            if (md != null)
            {
                btnEdit.IsEnabled = btnWidthSearch.IsEnabled = btnDepthSearch.IsEnabled = true;
                btnDelete.IsEnabled = true;
                panelViewMaze.Visibility = Visibility.Visible;
                panelEditMaze.Visibility = Visibility.Hidden;
                isEditing = false;
            }
        }
        private void NoMaze()
        {
            btnWidthSearch.IsEnabled = btnDepthSearch.IsEnabled = btnEdit.IsEnabled = btnDelete.IsEnabled = isEditing = false;
        }

    }
}
