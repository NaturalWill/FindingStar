using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace FindingStar
{
    public class MazePath
    {
        Path mazePath;
        GeometryConverter gc;
        MazeData md;

        Thread th;
        int speed;
        public string strPath { get; private set; }

        public MazePath(MazeData _md, ref Grid g)
        {
            md = _md;
            speed = 20;

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

        //标示深度搜索的路径
        public void draw_path(Point pt, Direction dir)
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
            MainWindow.mw.FillGridInvoke(p, Brushes.White);
        }


        public void findMazeWay(int depth_way = 0)
        {
            if (depth_way == 0)
            {
                th = new Thread(new ThreadStart(find_way_weith));
                th.Start();
            }
            else
            {
                th = new Thread(new ThreadStart(find_way_depth));
                th.Start();
            }

        }
        void find_way_depth()//深度优先搜索
        {
            Point end_point = md.endPoint;
            Point now_point = md.startPoint;
            if (md.MazeString[md.p2i(now_point)] == '0')//入口是否可通
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
                    MainWindow.mw.FillGridInvoke(now_point, Brushes.DarkRed);
                    //g.FillRectangle(Brushes.DarkRed, now_point.X, now_point.Y, 10, 10);
                    if (stp.Count == 0)
                    {
                        MessageBox.Show(noWay);
                        th.Abort();
                        return;
                    }
                    MainWindow.mw.FillGridInvoke(stp.Peek(), MCommon.getCellColor(CellColor.access));
                    //g.FillRectangle(backpen.Brush, stp.Peek().X, stp.Peek().Y, 10, 10);
                }
                Point pt = Next_way(stp.Peek(), std.Peek());
                if ((pt.X >= md.Width || pt.X < 0 || pt.Y >= md.Height || pt.Y < 0)//超出边界
                    || md.MazeString[md.p2i(pt)] == '0'//障碍物
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



        void find_way_weith()//广度搜索
        {

            Point end_point = md.endPoint;
            Point now_point = md.startPoint;
            if (md.MazeString[md.p2i(now_point)] == '0')//入口是否可通
            {
                MessageBox.Show(noWay);
                th.Abort();
                return;
            }
            //储存将要遍历的点
            Queue<Point> qp = new Queue<Point>();
            //储存点的关系信息，方便还原路径
            Dictionary<Point, Point> stp_passed = new Dictionary<Point, Point>();

            qp.Enqueue(now_point);
            stp_passed.Add(now_point, new Point(-1, -1));
            // g.FillEllipse(Brushes.Yellow, now_point.X + 2, now_point.Y + 2, 4, 4);
            Thread.Sleep(speed);
            while (now_point != end_point)
            {
                if (qp.Count == 0)
                {
                    MessageBox.Show(noWay);
                    th.Abort();
                    return;
                }
                now_point = qp.Dequeue();
                Point pt = new Point();
                for (int i = 0; i < 4; i++)//向4个方向探索
                {
                    pt = Next_way(now_point, (Direction)i);
                    if ((pt.X >= md.Width || pt.X < 0 || pt.Y >= md.Height || pt.Y < 0)//超出边界
                        || md.MazeString[md.p2i(pt)] == '0'//障碍物
                        || (stp_passed.Count > 0 && stp_passed.ContainsKey(pt))//已经探索过
                        )
                        ;
                    else
                    {//开始探索
                        qp.Enqueue(pt);
                        stp_passed.Add(pt, now_point);
                        //g.FillEllipse(Brushes.Yellow, pt.X + 2, pt.Y + 2, 4, 4);
                        Thread.Sleep(speed);
                    }
                }
            }
            Point p = end_point;
            while (p.X != -1)//还原路径
            {
                //g.FillEllipse(Brushes.Green, p.X, p.Y, 10, 10);//画出路径
                p = stp_passed[p];
            }
            //g.FillEllipse(Brushes.Green, 0, 0, 10, 10);
            MessageBox.Show("搜索完成，成功找到了星星");
            th.Abort();
            return;
        }

    }

}
