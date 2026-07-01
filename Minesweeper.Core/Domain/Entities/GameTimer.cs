using Minesweeper.Application.Interfaces;

namespace Minesweeper.Domain.Entities
{

    public class GameTimer : IDisposable, IGameTimer
    {
        private readonly System.Threading.Timer _timer;
        private int _elapsedTime = 0;
        private int _secondsInLastGame = 0;
        private bool _isRunning = false;
        public List<ITimerObserver> TimerObservers { get; set; } = new List<ITimerObserver>();
        public int SecondInLastGame => _secondsInLastGame;
        public GameTimer()
        {
            _timer = new System.Threading.Timer(OnTick, null, Timeout.Infinite, Timeout.Infinite);
        }

        private void OnTick(object? state)
        {
            _elapsedTime++;
            NotifyObservers();
        }

        public void StartTimer()
        {
            if (_isRunning) return; // Prevent multiple timers
            _isRunning = true;
            _timer.Change(1000, 1000);
        }



        public void StopTimer()
        {
            if (!_isRunning) return; // Prevent stopping if not running
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _secondsInLastGame = _elapsedTime;
            _elapsedTime = 0;
            _isRunning = false;
        }
        public void Subscribe(ITimerObserver observer)
        {
            if (!TimerObservers.Contains(observer))
            {
                TimerObservers.Add(observer);
            }
        }
        public void Unsubscribe(ITimerObserver observer)
        {
            if (TimerObservers.Contains(observer))
            {
                TimerObservers.Remove(observer);
            }
        }
        public void UnsubscribeAll()
        {
            TimerObservers.Clear();
        }
        public void NotifyObservers()
        {
            foreach (var observer in TimerObservers)
            {
                observer.UpdateTime(_elapsedTime);
            }
        }

        public void Dispose()
        {
            UnsubscribeAll();
            _timer.Dispose();
            TimerObservers.Clear();
        }
    }
}
