using System.Diagnostics;
using StreamWriter = System.IO.StreamWriter;

namespace MatrixMultythreading;

public class Program
{
    public static void Main()
    {
        var matrixSize = 50;
        var threadsNum = new[] { 1, 2, 4, 8, 16 };
        StreamWriter file = new StreamWriter("/Users/arturminnusin/Desktop/Information.txt");
        Stopwatch timer = new Stopwatch();
        
        var k = 0;
        while (matrixSize <= 500)
        {
            var matrix1 = new Matrix(matrixSize);
            var matrix2 = new Matrix(matrixSize);
        
            for (int i = 0; i < threadsNum.Length; i++)
            {
                file.Write(matrixSize + "/");
                timer.Start();
                matrix1.ParallelMultiplicationWithThread(matrix1, matrix2, threadsNum[i]);
                timer.Stop();
                file.Write(timer.ElapsedMilliseconds + "/");
                timer.Restart();
                matrix1.ParallelMultiplicationWithParalleFor(matrix1, matrix2, threadsNum[i]);
                timer.Stop();
                file.Write(timer.ElapsedMilliseconds + "/");
                timer.Restart();
                matrix1.ParallelMultiplicationWithTask(matrix1, matrix2, threadsNum[i]);
                timer.Stop();
                file.Write(timer.ElapsedMilliseconds + "/");
                file.Write($"{threadsNum[i]}");
                file.WriteLine();
            }
            
            Console.WriteLine(matrixSize);
            matrixSize += 50;
        }
        file.Close();
    }
}