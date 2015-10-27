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

        public const int MazeGridLenght = 10;


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

    //public class Point
    //{
    //    public int X;
    //    public int Y;
    //    public Point(Point p)
    //    {
    //        X = p.X;
    //        Y = p.Y;
    //    }
    //    public Point(int x, int y)
    //    {
    //        X = x;
    //        Y = y;
    //    }
    //    public Point()
    //    {
    //        X = Y = 0;
    //    }
    //    public Point(System.Windows.Point wp)
    //    {
    //        X = (int)wp.X;
    //        Y = (int)wp.Y;
    //    }
    //    public static bool operator ==(Point lp, Point rp)
    //    {
    //        if (lp.X == rp.X && lp.Y == rp.Y)
    //            return true;
    //        else return false;
    //    }
    //    public static bool operator !=(Point lp, Point rp)
    //    {
    //        if (lp.X != rp.X || lp.Y != rp.Y)
    //            return true;
    //        else return false;
    //    }
    //    // override object.Equals
    //    public override bool Equals(object obj)
    //    {
    //        //       
    //        // See the full list of guidelines at
    //        //   http://go.microsoft.com/fwlink/?LinkID=85237  
    //        // and also the guidance for operator== at
    //        //   http://go.microsoft.com/fwlink/?LinkId=85238
    //        //

    //        if (obj == null || GetType() != obj.GetType())
    //        {
    //            return false;
    //        }

    //        // TODO: write your implementation of Equals() here

    //        if (this.X == ((Point)obj).X && this.Y == ((Point)obj).Y)
    //            return true;
    //        else return false;
    //    }

    //    // override object.GetHashCode
    //    public override int GetHashCode()
    //    {
    //        // TODO: write your implementation of GetHashCode() here
    //        return base.GetHashCode();
    //    }
    //}
    public enum Direction { up = 0, right = 1, down = 2, left = 3 }

}
