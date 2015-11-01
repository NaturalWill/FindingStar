using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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

        Ellipse cStart;//起点
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
            if (MazePath.th_num > 0)
            {
                --MazePath.th_num;
                initMazeGrid();
                mp = new MazePath(md, ref gMaze);
                mp.findMazeWay(1);
                btnClearPath.IsEnabled = true;
            }

        }

        private void btnWidthSearch_Click(object sender, RoutedEventArgs e)
        {
            if (MazePath.th_num > 0)
            {
                --MazePath.th_num;
                initMazeGrid();
                mp = new MazePath(md, ref gMaze);
                mp.findMazeWay();
                btnClearPath.IsEnabled = true;
            }
        }
        private void btnClearPath_Click(object sender, RoutedEventArgs e)
        {
            mp.stop();
            initMazeGrid();
            btnClearPath.IsEnabled = false;
        }

        private void btnSearchBoth_Click(object sender, RoutedEventArgs e)
        {
            if (MazePath.th_num > 0)
            {
                --MazePath.th_num; --MazePath.th_num;
                initMazeGrid();
                mp = new MazePath(md, ref gMaze);
                mp.findMazeWay(1);
                mp.findMazeWay();
                btnClearPath.IsEnabled = true;
            }
        }
        #endregion
        //定义委托
        delegate void DelegateFillGrid(System.Drawing.Point pt, Brush b);
        //委托访问接口
        public void FillGridInvoke(System.Drawing.Point pt, Brush b)
        {
            DelegateFillGrid d = fillgrid;
            this.Dispatcher.BeginInvoke(d, new object[] { pt, b });
        }

        void fillgrid(System.Drawing.Point pt, Brush b)
        {
            listRect[md.p2i(pt)].Fill = b;
        }

        delegate void DelegateDrawPath(string s);
        Path mazePath;
        Path mazeWidthPath;
        GeometryConverter gc;
        GeometryConverter gc2;
        public void DrawPathInvoke(string s)
        {
            DelegateDrawPath d = drawPath;
            this.Dispatcher.BeginInvoke(d, new object[] { s });

        }
        public void DrawWidthPathInvoke(string s)
        {
            DelegateDrawPath d = drawWidthPath;
            this.Dispatcher.BeginInvoke(d, new object[] { s });

        }
        void drawPath(string strPath)
        {
            if (mazePath == null || gc == null)
            {
                mazePath = new Path();
                gc = new GeometryConverter();
                mazePath.Stroke = Brushes.Red;
                mazePath.StrokeThickness = 3;
                Grid.SetRowSpan(mazePath, md.Height);
                Grid.SetColumnSpan(mazePath, md.Width);
            }
            if (!gMaze.Children.Contains(mazePath))
                gMaze.Children.Add(mazePath);
            mazePath.Data = (Geometry)gc.ConvertFromString(strPath);

        }
        void drawWidthPath(string strPath)
        {
            if (mazeWidthPath == null || gc2 == null)
            {
                mazeWidthPath = new Path();
                gc2 = new GeometryConverter();
                mazeWidthPath.Stroke = Brushes.Blue;
                mazeWidthPath.StrokeThickness = 3;
                Grid.SetRowSpan(mazeWidthPath, md.Height);
                Grid.SetColumnSpan(mazeWidthPath, md.Width);
            }
            if (!gMaze.Children.Contains(mazeWidthPath))
                gMaze.Children.Add(mazeWidthPath);
            mazeWidthPath.Data = (Geometry)gc2.ConvertFromString(strPath);

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

        delegate void DelegateInitGrid();
        public void initMazeGrid()
        {
            DelegateInitGrid d = initGrid;
            this.Dispatcher.BeginInvoke(d, new object[] { });
        }

        void initGrid()
        {
            if (listRect == null)
                listRect = new List<Rectangle>();
            else
                listRect.Clear();

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

            System.Drawing.Point p = new System.Drawing.Point();
            for (p.Y = 0; p.Y < md.Height; p.Y++)
            {
                for (p.X = 0; p.X < md.Width; p.X++)
                {
                    Rectangle r = new Rectangle();
                    r.Height = r.Width = md.MazeGridLenght;
                    listRect.Add(r);
                    //r.StrokeThickness = 0;
                    if (md.MazeString[p.Y * md.Width + p.X] == '1')
                        r.Fill = MCommon.getCellColor(CellState.access);
                    else
                        r.Fill = MCommon.getCellColor(CellState.obstacle);

                    gMaze.Children.Add(r);

                    MoveToPoint(p, r);
                }
            }

            if (cStart == null || cEnd == null)
            {
                MazeControl m = new MazeControl();
                cStart = m.sStart;
                cEnd = m.sStar;

                m.sPanel.Children.Remove(cStart);
                m.sPanel.Children.Remove(cEnd);
            }

            gMaze.Children.Add(cStart);
            gMaze.Children.Add(cEnd);

            #endregion


            tbMazeName.Text = string.IsNullOrWhiteSpace(md.MazeName) ? strMaze :
                (md.MazeName + " (" + md.Height + " x " + md.Width + ") ");
            tbMazeNewName.Text = md.MazeName;
            tbMazeWidth.Text = md.Width.ToString();
            tbMazeHeight.Text = md.Height.ToString();

            autoShowGrid();
        }


        private void autoShowGrid()
        {
            if (md == null) return;
            if (md.IsEmptyMaze)
            {
                cStart.Visibility = cEnd.Visibility = Visibility.Hidden;
                frameContent.ColumnDefinitions[0].Width = new GridLength(0d);
            }
            else
            {
                MoveToPoint(md.startPoint, cStart);
                MoveToPoint(md.endPoint, cEnd);
                cStart.Visibility = cEnd.Visibility = Visibility.Visible;
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
        private static void MoveToPoint(System.Drawing.Point p, Shape s)
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
                btnSearchBoth.IsEnabled = btnEdit.IsEnabled = btnWidthSearch.IsEnabled = btnDepthSearch.IsEnabled = false;
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
                btnSearchBoth.IsEnabled = btnEdit.IsEnabled = btnWidthSearch.IsEnabled = btnDepthSearch.IsEnabled = true;
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

        CellState cs = CellState.access;

        private void gMaze_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isEditing)
            {
                Point pt = e.GetPosition(gMaze);
                System.Drawing.Point p = new System.Drawing.Point((int)(pt.X / MCommon.MazeGridLenght), (int)(pt.Y / MCommon.MazeGridLenght));


                if (cs == CellState.obstacle)
                {
                    if (md.startPoint != p && md.endPoint != p)
                    {
                        //点击处不为起点和终点，变为障碍
                        md.MazeString[md.p2i(p)] = '0';
                        FillGridInvoke(p, MCommon.getCellColor(CellState.obstacle));
                    }
                }
                else if (cs == CellState.access)
                {
                    //变为通路
                    md.MazeString[md.p2i(p)] = '1';
                    FillGridInvoke(p, MCommon.getCellColor(CellState.access));
                }
                else if (md.MazeString[md.p2i(p)] == '1')
                {
                    //点击处为通路
                    if (cs == CellState.start)
                    {
                        //设置为起点
                        md.startPoint = p;
                        MoveToPoint(p, cStart);
                    }
                    else if (cs == CellState.end)
                    {
                        //设置为终点
                        md.endPoint = p;
                        MoveToPoint(p, cEnd);
                    }
                }

            }
        }

        private void spSet_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (isEditing)
            {
                Point pt = e.GetPosition(spSet);
                cs = (CellState)((int)pt.Y / 50);
                for (int i = 0; i < spSet.Children.Count; i++)
                {
                    if ((int)cs == i)
                    {
                        ((Border)spSet.Children[i]).BorderBrush = Brushes.Red;

                    }
                    else
                    {
                        ((Border)spSet.Children[i]).BorderBrush = Brushes.White;
                    }
                }
            }
        }
    }
}