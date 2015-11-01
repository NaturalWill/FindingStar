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
        MazeData md;

        Thread th1, th2;
        int speed;
        public static int th_num = 1;
        string strDepthPath;
        string strWidthPath;
        public MazePath(MazeData _md, ref Grid g)
        {
            md = _md;
            speed = 10;

        }
        public void stop()
        {
            if (th1 != null)
                th1.Abort();
            if (th2 != null)
                th2.Abort();
            th_num = 1;
        }

        const string noWay = "T_T 没有通路";

        private System.Drawing.Point Next_way(System.Drawing.Point pt, Direction dir)//求出下一个点
        {
            System.Drawing.Point temp = new System.Drawing.Point();
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
        public void draw_path(System.Drawing.Point pt, Direction dir)
        {
            System.Drawing.Point p;
            if (dir == Direction.right)
                p = new System.Drawing.Point(pt.X + 1, pt.Y);
            else if (dir == Direction.left)
                p = new System.Drawing.Point(pt.X - 1, pt.Y);
            else if (dir == Direction.up)
                p = new System.Drawing.Point(pt.X, pt.Y - 1);
            else
                p = new System.Drawing.Point(pt.X, pt.Y + 1);

            strDepthPath += " " + (p.X * MCommon.MazeGridLenght + MCommon.MazeGridLenght / 2)
                + "," + (p.Y * MCommon.MazeGridLenght + MCommon.MazeGridLenght / 2);

            MainWindow.mw.DrawPathInvoke(strDepthPath);
        }
        public void back_path()
        {
            strDepthPath = strDepthPath.Remove(strDepthPath.LastIndexOf(' '));

            MainWindow.mw.DrawPathInvoke(strDepthPath);
        }
        //标示搜索的路径
        public void draw_path(System.Drawing.Point p)
        {
            MainWindow.mw.FillGridInvoke(p, Brushes.White);
        }


        public void findMazeWay(int depth_way = 0)
        {
            if (depth_way == 0)
            {
                th2 = new Thread(new ThreadStart(find_way_weith));
                th2.Start();
            }
            else
            {
                th1 = new Thread(new ThreadStart(find_way_depth));
                th1.Start();
            }

        }
        void find_way_depth()//深度优先搜索
        {
            System.Drawing.Point end_point = md.endPoint;
            System.Drawing.Point now_point = md.startPoint;
            if (md.MazeString[md.p2i(now_point)] == '0')//入口是否可通
            {
                MessageBox.Show(noWay);
                ++th_num;
                th1.Abort();
                return;
            }
            Stack<System.Drawing.Point> stp = new Stack<System.Drawing.Point>();//储存当前路径
            Stack<Direction> std = new Stack<Direction>();//储存各点方向
            Stack<System.Drawing.Point> stp_not = new Stack<System.Drawing.Point>();//储存无法通过的点
            bool is_turn = false;//标志当前点是否转向
            stp.Push(now_point);
            std.Push(Direction.up);

            strDepthPath = "M " + (stp.Peek().X * MCommon.MazeGridLenght + MCommon.MazeGridLenght / 2) + ","
               + (stp.Peek().Y * MCommon.MazeGridLenght + MCommon.MazeGridLenght / 2) + " L";
            Thread.Sleep(speed);
            while (now_point != end_point)
            {
                if (stp.Count == 0)
                {
                    MessageBox.Show(noWay);
                    ++th_num;
                    th1.Abort();
                    return;
                }
                if (is_turn == true && std.Peek() == Direction.up)//4个方向探索完毕,无法通过，退回上一步
                {
                    std.Pop();
                    now_point = stp.Pop();
                    stp_not.Push(now_point);
                    back_path();
                    MainWindow.mw.FillGridInvoke(now_point, Brushes.DarkRed);
                    //g.FillRectangle(Brushes.DarkRed, now_point.X, now_point.Y, 10, 10);
                    if (stp.Count == 0)
                    {
                        MessageBox.Show(noWay);
                        ++th_num;
                        th1.Abort();
                        return;
                    }
                    //MainWindow.mw.FillGridInvoke(stp.Peek(), Brushes.Blue);
                    //g.FillRectangle(backpen.Brush, stp.Peek().X, stp.Peek().Y, 10, 10);
                }
                System.Drawing.Point pt = Next_way(stp.Peek(), std.Peek());
                if (pt.X >= md.Width || pt.X < 0 || pt.Y >= md.Height || pt.Y < 0//超出边界
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

            draw_path(end_point);//画出最后一个点


            MessageBox.Show("搜索完成，成功找到了星星");
            ++th_num;
            th1.Abort();
            return;
        }



        void find_way_weith()//广度搜索
        {

            System.Drawing.Point end_point = md.endPoint;
            System.Drawing.Point now_point = md.startPoint;
            if (md.MazeString[md.p2i(now_point)] == '0')//入口是否可通
            {
                MessageBox.Show(noWay);
                ++th_num;
                th2.Abort();
                return;
            }
            //储存将要遍历的点
            Queue<System.Drawing.Point> qp = new Queue<System.Drawing.Point>();
            //储存点的关系信息，方便还原路径
            Dictionary<System.Drawing.Point, System.Drawing.Point> stp_passed = new Dictionary<System.Drawing.Point, System.Drawing.Point>();

            qp.Enqueue(now_point);
            stp_passed.Add(now_point, new System.Drawing.Point(-1, -1));
            // g.FillEllipse(Brushes.Yellow, now_point.X + 2, now_point.Y + 2, 4, 4);
            //MainWindow.mw.FillGridInvoke(now_point, Brushes.Green);
            Thread.Sleep(speed);
            while (now_point != end_point)
            {
                if (qp.Count == 0)
                {
                    MessageBox.Show(noWay);
                    ++th_num;
                    th2.Abort();
                    return;
                }
                now_point = qp.Dequeue();
                System.Drawing.Point pt = new System.Drawing.Point();
                for (int i = 0; i < 4; i++)//向4个方向探索
                {
                    pt = Next_way(now_point, (Direction)i);
                    if (pt.X >= md.Width || pt.X < 0 || pt.Y >= md.Height || pt.Y < 0//超出边界
                        || md.MazeString[md.p2i(pt)] == '0'//障碍物
                        || (stp_passed.Count > 0 && stp_passed.ContainsKey(pt))//已经探索过
                        )
                    { }
                    else
                    {//开始探索
                        qp.Enqueue(pt);
                        stp_passed.Add(pt, now_point);
                        //g.FillEllipse(Brushes.Yellow, pt.X + 2, pt.Y + 2, 4, 4);
                        MainWindow.mw.FillGridInvoke(pt, Brushes.LightBlue);
                        Thread.Sleep(speed);
                    }
                }
            }
            System.Drawing.Point p = end_point;
            strWidthPath = "M " + (p.X * MCommon.MazeGridLenght + MCommon.MazeGridLenght / 2) + ","
               + (p.Y * MCommon.MazeGridLenght + MCommon.MazeGridLenght / 2) + " L";
            while (p.X != -1)//还原路径
            {
                //g.FillEllipse(Brushes.Green, p.X, p.Y, 10, 10);//画出路径
                //MainWindow.mw.FillGridInvoke(p, Brushes.Yellow);

                strWidthPath += " " + (p.X * MCommon.MazeGridLenght + MCommon.MazeGridLenght / 2)
                    + "," + (p.Y * MCommon.MazeGridLenght + MCommon.MazeGridLenght / 2);
                
                p = stp_passed[p];
            }
            MainWindow.mw.DrawWidthPathInvoke(strWidthPath);
            //g.FillEllipse(Brushes.Green, 0, 0, 10, 10);
            MessageBox.Show("搜索完成，成功找到了星星");
            ++th_num;
            th2.Abort();
            return;
        }

    }

}
