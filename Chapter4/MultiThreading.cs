using System.Diagnostics;

namespace GrokkingConcurrency.Chapter4
{
    internal class MultiThreading
    {
        static void Main(string[] args)
        {
            DisplayThreads();

            byte numThreads = 5;
            Console.WriteLine($"Starting {numThreads} CPU wasters...");

            // Task 리스트로 작업 관리
            var tasks = new List<Task>();
            for (int i = 0; i < numThreads; i++)
            {
                int threadId = i; // 캡처 변수 문제 방지
                tasks.Add(Task.Run(() => CpuWaster(threadId)));
            }

            // 모든 작업이 끝날 때까지 대기
            Task.WaitAll(tasks.ToArray());

            DisplayThreads();

            // 콘솔 유지
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void CpuWaster(int threadId)
        {
            // 스레드 이름 설정
            string name = $"Thread-{threadId}";
            Thread.CurrentThread.Name = name;

            Console.WriteLine($"{name} starting work {threadId}");

            // CPU 작업 시뮬레이션 (3초 대기)
            Thread.Sleep(3000); // Task.Delay 대신 동기적 Sleep 사용 (의도적 CPU 낭비 시뮬레이션)
            // 또는 비동기 방식: await Task.Delay(3000); (async 메서드로 바꿔야 함)

            Console.WriteLine($"{name} finished work {threadId}");
        }

        private static void DisplayThreads()
        {
            Console.WriteLine(new string('-', 10));
            Console.WriteLine($"Current process PID: {GetPid()}");
            Console.WriteLine($"Thread Count: {GetActiveThreadCount()}");
            Console.WriteLine("Active threads:");
            var process = Process.GetCurrentProcess();
            foreach (ProcessThread thread in process.Threads)
            {
                Console.WriteLine($"Thread ID: {thread.Id}, State: {thread.ThreadState}");
            }
        }

        private static int GetActiveThreadCount()
        {
            return Process.GetCurrentProcess().Threads.Count;
        }

        private static int GetPid()
        {
            return Environment.ProcessId;
        }
    }
}