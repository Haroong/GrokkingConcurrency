namespace GrokkingConcurrency.Chapter8
{
    //주차장의 하루!
    internal class ParkingLot
    {
        private const int TotalSpots = 3;
        private readonly Semaphore _semaphore;
        private readonly object _lockObject = new object();
        private readonly List<string> _parkedCars;
        private static readonly Random _random = new Random();

        public ParkingLot()
        {
            //주차장 초기화
            _semaphore = new Semaphore(initialCount: TotalSpots, maximumCount: TotalSpots);
            _parkedCars = new List<string>();
        }


        public static void TestGarage(ParkingLot parkingLot, int numberOfCars)
        {
            //스레드 초기화
            Thread[] threads = new Thread[numberOfCars];

            //주차 시작
            for (int carNum = 0; carNum < numberOfCars; carNum++)
            {
                string carName = $"Car #{carNum}";
                threads[carNum] = new Thread(() => ParkCar(parkingLot, carName));
                threads[carNum].Start();
            }

            //모든 스레드가 종료될 때까지 기다리기
            foreach (var thread in threads)
            {
                thread.Join();
            }
        }

        private static void ParkCar(ParkingLot parkingLot, string carName)
        {
            //입차하기
            parkingLot.Enter(carName);

            //스레드 1~2초 대기
            Thread.Sleep(_random.Next(1000, 2001));

            //출차하기
            parkingLot.Exit(carName);
        }

        private void Exit(string carName)
        {
            lock (_lockObject)
            {
                _parkedCars.Remove(carName);
                Console.WriteLine($"출차 완료: {carName}");
            }
            _semaphore.Release();
        }

        private void Enter(string carName)
        {
            //주차장에 들어가기
            _semaphore.WaitOne();
            lock (_lockObject)
            {
                //락을 획득했으면 주차 고고
                _parkedCars.Add(carName);
                Console.WriteLine($"주차 완료: {carName}");
            }
        }
        private int CountParkedCars()
        {
            //현재 주차된 차량수
            return _parkedCars.Count;
        }

        public static void Main()
        {
            ParkingLot parkingLot = new ParkingLot();

            //주차 시작
            int numberOfCars = 10;
            TestGarage(parkingLot, numberOfCars);

            //결과
            Console.WriteLine("주차장의 하루 완료!");
            Console.WriteLine($"Actual: {parkingLot.CountParkedCars()}\nExpected: 0");
        }
    }
}
