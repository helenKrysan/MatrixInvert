using Parcs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InverseMatrix
{
    public class MainModuleInverse : MainModule
    {
        private const string fileName = "resMatrix.mtr";

        private static CommandLineOptions options;

        private static IChannel[] channels;
        private static IChannel[] channelsMult;

        public static void Main(string[] args)
        {
            options = new CommandLineOptions();
            if (args != null)
            {
                if (!CommandLine.Parser.Default.ParseArguments(args, options))
                {
                    throw new ArgumentException($@"Cannot parse the arguments. Possible usages:
{options.GetUsage()}");
                }
            }
            (new MainModuleInverse()).RunModule(options);
        }

        public override void Run(ModuleInfo info, CancellationToken token = default(CancellationToken))
        {

            string file = options.File;
            Matrix a;

            if (Math.Sqrt(options.N) % 1 != 0)
            {
                Console.WriteLine("Incorrect number of point");
                return;
            }
            try
            {
                a = Matrix.LoadFromFile(file);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("File with a given fileName not found, stopping the application...");
                return;
            }
          /*  for (int i = 0; i < a.Height; ++i)
            {
                for (int j = 0; j < a.Width; ++j)
                {
                    Console.Write(a[i, j] + " ");
                }
                Console.WriteLine();
            }*/
            int[] possibleValues = { 1, 2, 4 };

            int pointsNum = options.PointsNum;

            if (!possibleValues.Contains(pointsNum))
            {
                Console.WriteLine("Incorrect number of point");
                return;
            }

            var points = new IPoint[pointsNum];
            channels = new IChannel[pointsNum];
            var pointsMult = new IPoint[pointsNum];
            channelsMult = new IChannel[pointsNum];
            for (int i = 0; i < pointsNum; ++i)
            {
                Console.WriteLine(i);
                points[i] = info.CreatePoint();
                channels[i] = points[i].CreateChannel();
                points[i].ExecuteClass("InverseMatrix.InverseMatrix");

                /*   pointsMult[i] = info.CreatePoint();
                   channelsMult[i] = pointsMult[i].CreateChannel();
                   pointsMult[i].ExecuteClass("InverseMatrix.MultMatrix");*/
            }

            DateTime time = DateTime.Now;
            Console.WriteLine("Start working...");
            var resMatrix = Calc(a, pointsNum, 0).Result;
            LogResultFoundTime(time);
            for (int i = 0; i < resMatrix.Height; ++i)
            {
                for (int j = 0; j < resMatrix.Width; ++j)
                {
                    Console.Write(resMatrix[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        private static Task<Matrix> Calc(Matrix a, int pointsNum, int n)
        {
       //     Console.WriteLine("inside");
            Matrix[] tempArray = null;
            DateTime time = DateTime.Now;
            channels[n].WriteObject(a);
            tempArray = (Matrix[])channels[n].ReadObject<object>();
            //    Console.WriteLine($"recieved : channel {n}");
            Task<Matrix> res = null;
            if (tempArray.Length > 1)
            {
         //       Console.WriteLine(tempArray[0][0, 0]);
         //       Console.WriteLine(tempArray[3][0, 0]);
                Matrix a1 = null;
                Matrix c1 = null;
                switch (pointsNum)
                {
                    case 1:
                        var at1 = Calc(tempArray[0], pointsNum, n);
                        var ct1 = Calc(tempArray[3], pointsNum, n);
                        a1 = at1.Result;
                        c1 = ct1.Result;
                        break;
                    case 2:
                        var at12 = Calc(tempArray[0], pointsNum/2, 0);
                        var ct12 = Calc(tempArray[3], pointsNum/2, 1);
                        a1 = at12.Result;
                        c1 = ct12.Result;
                        break;                 
                    default:
                        Console.WriteLine("Unexpected error.");
                        return null;
                }
                var b = a1.MultiplyBy(tempArray[2]).MultiplyBy(c1);
                for (int i = 0; i < b.Height; ++i)
                {
                    for (int j = 0; j < b.Width; ++j)
                    {
                        b[i, j] = b[i, j] * -1;
                    }
                }
                var nullMatrix = new Matrix(a1.Height, a1.Width, false);
         //       Console.WriteLine("time to return");
                var newMatrix = new Matrix(a1.Height * 2, a1.Width * 2, false);
                var matrixArray = new Matrix[4] { a1, nullMatrix, b, c1 };
                res = Task<Matrix>.Run(() => SplitMatrixHelper.JoinMatrix(newMatrix, matrixArray));
            }
            else
            {
                res = Task<Matrix>.Run(() => tempArray[0]);
            }
            return res;
        }

        private static void LogResultFoundTime(DateTime time)
        {
            Console.WriteLine(
                "Result found: time = {0}, saving the result to the file {1}",
                Math.Round((DateTime.Now - time).TotalSeconds, 3),
                fileName);
        }

        private static void LogSendingTime(DateTime time)
        {
            Console.WriteLine("Sending finished: time = {0}", Math.Round((DateTime.Now - time).TotalSeconds, 3));
        }
    }
}