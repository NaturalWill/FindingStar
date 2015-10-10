using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FindingStar
{
    public static class MCommon
    {
        public static MazeData mdcur;
        public static MainWindow mw;

        public static PageMazeEdit pme;
        public static PageMazeView pmv;

        public const int MazeGridLenght = 10;


        public static string getRectangleName(int x, int y)
        {
            return "x" + x + "y" + y;
        }
        public static Brush getCellColor(CellColor cc)
        {
            if (cc == CellColor.obstacle) return Brushes.Black;
            else if (cc == CellColor.access) return Brushes.LightGray;
            else if (cc == CellColor.start) return Brushes.Yellow;
            else return Brushes.LightGreen;
        }


    }
    public enum CellColor
    { obstacle, access, start, end }

    public class Point
    {
        public int X;
        public int Y;
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
        public Point()
        {
            X = Y = 0;
        }
    }
    public enum Direction { up = 0, right = 1, down = 2, left = 3 }
    public class MazePath
    {
        System.Windows.Shapes.Path mazePath;
        GeometryConverter gc;
        MazeData md;
        Grid gMaze;

        Thread th;
        int speed;
        public string strPath { get; private set; }

        public MazePath(MazeData _md, ref Grid g)
        {
            md = _md;
            speed = 20;
            gMaze = g;

            //mazePath = new System.Windows.Shapes.Path();

            //mazePath.Stroke = Brushes.Red;

            //mazePath.StrokeThickness = 3;

            //gc = new GeometryConverter();
        }

        void drawPath(string newPathString)
        {
            strPath = newPathString;
            mazePath.Data = (Geometry)gc.ConvertFromString(newPathString);
        }

        const string noWay = "T_T 没有通路";

        private Point Next_way(Point pt, Direction dir)//求出下一个点
        {
            Point temp = new Point();
            switch (dir)
            {
                case Direction.up: temp.X = pt.X; temp.Y = pt.Y - 1; break;
                case Direction.right: temp.X = pt.X + 1; temp.Y = pt.Y; break;
                case Direction.down: temp.X = pt.X; temp.Y = pt.Y + 1; break;
                case Direction.left: temp.X = pt.X - 1; temp.Y = pt.Y; break;
            }
            return temp;
        }
        Rectangle getRectangle(Point pt)
        {
            return (Rectangle)gMaze.FindName(MCommon.getRectangleName(pt.X, pt.Y));
        }
        void fillgrid(Point pt, Brush b)
        {
            getRectangle(pt).Fill = b;
        }
        public void draw_path(Point pt, Direction dir)//标示深度搜索的路径
        {
            Point p;
            if (dir == Direction.right)
                p = new Point(pt.X + 1, pt.Y);
            else if (dir == Direction.left)
                p = new Point(pt.X - 1, pt.Y);
            else if (dir == Direction.up)
                p = new Point(pt.X, pt.Y - 1);
            else
                p = new Point(pt.X, pt.Y + 1);
            fillgrid(p, Brushes.White);
        }

        int getGridIndex(Point p)
        {
            return p.X * md.Width + p.Y;
        }
        public void find_way_depth()//深度优先搜索
        {
            Point end_point = new Point(10, 10);
            Point now_point = new Point(0, 0);
            if (md.MazeString[getGridIndex(now_point)] == '0')//入口是否可通
            {
                MessageBox.Show(noWay);
                th.Abort();
                return;
            }
            Stack<Point> stp = new Stack<Point>();//储存当前路径
            Stack<Direction> std = new Stack<Direction>();//储存各点方向
            Stack<Point> stp_not = new Stack<Point>();//储存无法通过的点
            bool is_turn = false;//标志当前点是否转向
            stp.Push(now_point);
            std.Push(Direction.up);
            Thread.Sleep(speed);
            while (now_point != end_point)
            {
                if (stp.Count == 0)
                {
                    MessageBox.Show(noWay);
                    th.Abort();
                    return;
                }
                if (is_turn == true && std.Peek() == Direction.up)//4个方向探索完毕,无法通过，退回上一步
                {
                    std.Pop();
                    now_point = stp.Pop();
                    stp_not.Push(now_point);
                    fillgrid(now_point, Brushes.DarkRed);
                    //g.FillRectangle(Brushes.DarkRed, now_point.X, now_point.Y, 10, 10);
                    if (stp.Count == 0)
                    {
                        MessageBox.Show(noWay);
                        th.Abort();
                        return;
                    }
                    fillgrid(stp.Peek(), MCommon.getCellColor(CellColor.access));
                    //g.FillRectangle(backpen.Brush, stp.Peek().X, stp.Peek().Y, 10, 10);
                }
                Point pt = Next_way(stp.Peek(), std.Peek());
                if ((pt.X >= md.Width || pt.X < 0 || pt.Y >= md.Height || pt.Y < 0)//超出边界
                    || md.MazeString[getGridIndex(pt)] == '0'//障碍物
                    || stp.Contains(pt) || (stp_not.Count > 0 && stp_not.Contains(pt)))//路径重复
                {
                    //转向
                    Direction dir = std.Pop();
                    dir = (Direction)((Convert.ToInt32(dir) + 1) % 4);
                    std.Push(dir);
                    is_turn = true;
                }

                else
                {
                    //"进入"
                    now_point = pt;
                    draw_path(stp.Peek(), std.Peek());
                    stp.Push(now_point);
                    std.Push(Direction.up);
                    is_turn = false;
                }
                Thread.Sleep(speed);
            }
            std.Pop();

            draw_path(end_point, std.Peek());//画出最后一个点

            MessageBox.Show("搜索完成，成功找到了星星");
            th.Abort();
            return;
        }
    }
    public class MazeData
    {
        public int MazeGridLenght { get { return MCommon.MazeGridLenght; } }

        public int Width
        {
            get;
            private set;
        }

        public int Height
        {
            get;
            private set;
        }

        public string MazeString;
        public string MazeName;
        public Point startPoint;
        public Point endPoint;

        string DirPath;

        string MazePath { get { return DirPath + @"\" + MazeName + ".maze"; } }

        Ellipse cMove;
        System.Windows.Shapes.Path cEnd;
        public MazeData()
        {
            DirPath = Environment.CurrentDirectory + @"\MazeData";
            Width = 0;
            Height = 0;
            startPoint = new Point();
            endPoint = new Point();
            MazeControl m = new MazeControl();
            cMove = m.ellipseMove;
            cEnd = m.pathStar;
            m.sPanel.Children.Remove(cMove);
            m.sPanel.Children.Remove(cEnd);
        }

        public void CreateMaze(int width, int height, string mazename)
        {
            MazeName = mazename;
            //生成随机的0,1串表示迷宫，障碍占迷宫体积的40%
            Width = width; Height = height;
            startPoint = new Point();
            endPoint = new Point(Width - 1, Height - 1);

            List<char> listCharMaze = new List<char>();
            Random rd = new Random();
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    int temp = rd.Next(1, 100);
                    if (temp > 40)
                    {
                        listCharMaze.Add('1');
                    }
                    else
                    {
                        listCharMaze.Add('0');
                    }
                }
            }
            listCharMaze[0] = '1';
            listCharMaze[Height * Width - 1] = '1';
            MazeString = new String(listCharMaze.ToArray());

        }



        /// <summary>
        /// 在Grid中展示迷宫
        /// </summary>
        /// <param name="gMaze"></param>
        public void ShowMaze(ref Grid gMaze)
        {
            gMaze.Children.Clear();
            gMaze.ColumnDefinitions.Clear();
            gMaze.RowDefinitions.Clear();
            gMaze.Width = Width * MazeGridLenght;
            gMaze.Height = Height * MazeGridLenght;
            for (int r = 0; r < Height; r++)
            {
                RowDefinition rd = new RowDefinition();
                gMaze.RowDefinitions.Add(rd);
            }
            for (int c = 0; c < Width; c++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                gMaze.ColumnDefinitions.Add(cd);
            }
            for (int h = 0; h < Height; h++)
            {
                for (int w = 0; w < Width; w++)
                {
                    Rectangle r = new Rectangle();
                    r.Height = r.Width = MazeGridLenght;
                    r.Name = MCommon.getRectangleName(w, h);
                    //r.StrokeThickness = 0;
                    if (MazeString[h * Width + w] == '1')
                        r.Fill = MCommon.getCellColor(CellColor.access);
                    else
                        r.Fill = MCommon.getCellColor(CellColor.obstacle);
                    gMaze.Children.Add(r);
                    r.SetValue(Grid.RowProperty, h);
                    r.SetValue(Grid.ColumnProperty, w);
                }
            }
            gMaze.Children.Add(cMove);
            gMaze.Children.Add(cEnd);
            cMove.SetValue(Grid.ColumnProperty, startPoint.X);
            cMove.SetValue(Grid.RowProperty, startPoint.Y);
            cEnd.SetValue(Grid.ColumnProperty, endPoint.X);
            cEnd.SetValue(Grid.RowProperty, endPoint.Y);
        }


        #region Read,Write,Delete

        public string[] getMazeList()
        {
            List<string> mazes = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(DirPath);
            foreach (FileInfo f in dir.GetFiles("*.maze"))
            {
                mazes.Add(f.Name.Substring(0, f.Name.Length - 5));
            }
            return mazes.ToArray();
        }
        public void ReadMaze(string mazename)
        {
            MazeName = mazename;
            if (File.Exists(MazePath))
            {
                try
                {

                    FileStream fs = new FileStream(MazePath, FileMode.Open);
                    StreamReader sr = new StreamReader(fs);
                    Width = Convert.ToInt32(sr.ReadLine());
                    Height = Convert.ToInt32(sr.ReadLine());

                    startPoint.X = Convert.ToInt32(sr.ReadLine());
                    startPoint.Y = Convert.ToInt32(sr.ReadLine());

                    endPoint.X = Convert.ToInt32(sr.ReadLine());
                    endPoint.Y = Convert.ToInt32(sr.ReadLine());

                    MazeString = sr.ReadToEnd();

                    fs.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error in reading maze.", ex);
                }
            }
        }
        public void deleteMaze()
        {
            if (File.Exists(MazePath)) { File.Delete(MazePath); }
            this.MazeString = this.MazeName = null;
            this.Width = this.Height = 0;
            startPoint = new Point();
            endPoint = new Point();
        }
        /// <summary>
        /// 迷宫数据保存到外存
        /// </summary>
        public void SaveMaze()
        {
            if (!Directory.Exists(DirPath))
                Directory.CreateDirectory(DirPath);

            try
            {
                FileStream fs = new FileStream(MazePath, FileMode.Create);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(Width);
                sw.WriteLine(Height);
                sw.WriteLine(startPoint.X);
                sw.WriteLine(startPoint.Y);
                sw.WriteLine(endPoint.X);
                sw.WriteLine(endPoint.Y);
                sw.Write(MazeString);
                sw.Flush();
                fs.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in saving maze.", ex);
            }
        }
        #endregion


    }
}
