namespace GrokkingConcurrency.Chapter8
{
    internal class SyncedBackAccount
    {
        private uint balance; //계좌 잔고
        private readonly object lockObject = new object(); //락을 위한 객체

        public SyncedBackAccount(uint balance)
        {
            this.balance = balance;
        }

        private void Deposit(uint amount)
        {
            lock (lockObject)
            {
                Console.WriteLine($"입금: {amount}. 현재 잔고: {balance}");
                balance += amount;
                Console.WriteLine($"입금 완료. 현재 잔고: {balance}");
            }
        }

        private void Withdraw(uint amount)
        {
            lock (lockObject)
            {
                if (balance >= amount)
                {
                    Console.WriteLine($"출금: {amount}. 현재 잔고: {balance}");
                    balance -= amount;
                    Console.WriteLine($"출금 완료. 현재 잔고: {balance}");
                }
                else
                {
                    Console.WriteLine($"출금 금액 부족: {amount}. 현재 잔고: {balance}");
                }
            }
        }

        static void Main(string[] args)
        {
            var account = new SyncedBackAccount(10000); //초기 금액 설정

            // 여러 스레드에서 입출금 테스트
            var tasks = new Task[4];
            tasks[0] = Task.Run(() => account.Deposit(500));
            tasks[1] = Task.Run(() => account.Withdraw(300));
            tasks[2] = Task.Run(() => account.Deposit(200));
            tasks[3] = Task.Run(() => account.Withdraw(1000));

            Task.WaitAll(tasks); // 모든 작업이 끝날 때까지 대기
            Console.WriteLine("모든 트랜잭션 완료");
        }
    }
}
