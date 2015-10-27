using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FindingStar
{
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

        static string DirPath = Environment.CurrentDirectory + @"\MazeData";

        string MazePath { get { return DirPath + @"\" + MazeName + ".maze"; } }

        public MazeData()
        {
            Width = 0;
            Height = 0;
            startPoint = new Point();
            endPoint = new Point();
        }
        public bool IsEmptyMaze
        {
            get
            {
                if (Width == 0 && Height == 0 && startPoint == endPoint) return true;
                return false;
            }
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
        /// 根据坐标返回位置
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public int p2i(Point p)
        {
            return p.X * Width + p.Y;
        }

        #region Read,Write,Delete

        public static string[] getMazeList()
        {
            try
            {
                List<string> mazes = new List<string>();
                if (!Directory.Exists(DirPath))
                    Directory.CreateDirectory(DirPath);
                DirectoryInfo dir = new DirectoryInfo(DirPath);
                foreach (FileInfo f in dir.GetFiles("*.maze"))
                {
                    mazes.Add(f.Name.Substring(0, f.Name.Length - 5));
                }
                return mazes.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception("获取文件列表错误", ex);
            }
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
