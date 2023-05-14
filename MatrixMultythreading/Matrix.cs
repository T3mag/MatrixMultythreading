using System.Text;

namespace MatrixMultythreading;

public class Matrix
{
    private readonly int matrixSize;
    private readonly int[,] matrix;

    public Matrix(int size)
    {
        matrixSize = size;
        matrix = new int[size, size];
        Random r = new Random();

        for (int i = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                matrix[i, j] = r.Next(0, 9);
    }

    public Matrix ParallelMultiplicationWithParalleFor(Matrix matrix1, Matrix matrix2 ,int threadsNum)
    {
        var newMatrix = new Matrix(matrixSize);

        Parallel.For(0, threadsNum, i =>
        {
            newMatrix = ParMultiplication(i, threadsNum, matrix1, matrix2, newMatrix);
        });
        return newMatrix;
    }

    public Matrix ParallelMultiplicationWithThread(Matrix matrix1, Matrix matrix2, int threadsNum)
    {
        var newMatrix = new Matrix(matrixSize);
        var k = matrixSize / threadsNum;
        
        Thread[] threads = new Thread[threadsNum];

        if (matrix1.matrixSize != matrix2.matrixSize)
            throw new ArgumentException("Error");
        
        for (int num = 0; num < threadsNum; num++)
        {
            var obj = new object();
            var line1 = num * k;
            var line2 = (num + 1) * k;

            if (num == threadsNum - 1)
                line2 = matrixSize;
            
            threads[num] = new Thread(() =>
            {
                for (int i = 0; i < matrixSize; i++)
                    for (int j = line1; j < line2; j++)
                    {
                        var sum = 0;
                        for (int p = 0; p < matrix2.matrixSize; p++)
                            sum += matrix1.matrix[i, p] * matrix2.matrix[p, j];
                        lock (obj)
                            newMatrix.matrix[i, j] = sum;
                    }
            });
            threads[num].Start();
        }

        for (int i = 0; i < threadsNum; i++)
            threads[i].Join();
        
        return newMatrix;
    }

    private Matrix ParMultiplication(int num, int threadsNum, Matrix matrix1, Matrix matrix2, Matrix newMatrix)
    {
        var k = matrixSize / threadsNum;
        var line1 = k * num;
        var line2 = k * (num + 1);
        
        if (num == threadsNum - 1)
            line2 = matrixSize;

        if (matrix1.matrixSize == matrix2.matrixSize)
        {
            //Console.WriteLine($"Поток = {num}: Строка1 = {line1}, Строка2 = {line2}");
        
            for (int i = line1; i < line2; i++)
            {
                for (int j = 0; j < matrix1.matrixSize; j++)
                {
                    int sum = 0;
                    for (int p = 0; p < matrix1.matrixSize; p++)
                        sum += matrix1.matrix[i, p] * matrix2.matrix[p, j];
                    newMatrix.matrix[i, j] = sum;
                }
            }
        }
        else
        {
            throw new AggregateException("Error");
        }

        return newMatrix;
    }

    public Matrix ParallelMultiplicationWithTask(Matrix matrix1, Matrix matrix2, int threadsNum)
    {
        var newMatrix = new Matrix(matrixSize);
        var k = matrixSize / threadsNum;

        Task[] task = new Task[threadsNum];

        for (int num = 0; num < threadsNum; num++)
        {
            var line1 = num * k;
            var line2 = (num + 1) * k;

            if (num == threadsNum - 1)
                line2 = matrixSize;

            task[num] = Task.Run(() =>
            {
                for (int i = line1; i < line2; i++)
                    for (int j = 0; j < matrix1.matrixSize; j++)
                    {
                        var sum = 0;
                        for (int p = 0; p < matrix2.matrixSize; p++)
                            sum += matrix1.matrix[i, p] * matrix2.matrix[p, j];

                        newMatrix.matrix[i, j] = sum;
                    }
            });
        }

        Task.WaitAll(task);
        return newMatrix;
    }
    

    public int ParallelMinimum(int thereadsNum)
    {
        object ob = new object();
        int max = int.MinValue;

        Parallel.For(0, thereadsNum, i =>
        {
            int maxmin = ParallelSearchMinimum(i, thereadsNum);

            lock (ob)
            {
                if (maxmin > max)
                    max = maxmin;
            }
        });

        return max;
    }

    private int ParallelSearchMinimum(int num, int thereadsNum)
    {
        int k = matrixSize / thereadsNum;
        int line1 = k * num;
        int line2 = k * (num + 1);
        
        if (num == thereadsNum - 1)
            line2 = matrixSize;

        Console.WriteLine($"Поток = {num}: Строка1 = {line1}, Строка2 = {line2}");
        
        int max = int.MinValue;
        for (int lines = line1; lines < line2; lines++)
        {
            int min = int.MaxValue;
            for (int columns = 0; columns < matrixSize; columns++)
            {
                if (matrix[lines, columns] < min)
                    min = matrix[lines, columns];
            }

            if (min > max)
                max = min;
        }

        return max;
    } 

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < matrixSize; i++)
        {
            for (int j = 0; j < matrixSize; j++)
                sb.Append($"{matrix[i, j]}   ");
            sb.AppendLine();
        }
        return sb.ToString();
    }
}