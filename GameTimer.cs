using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Timer = System.Windows.Forms.Timer;


namespace Minesweeper
{
    
    public class GameTimer : IDisposable
    {
        private Timer _timer;
        private int _elapsedTime = 0;
        private int _secondsInLastGame = 0;
        private bool _isRunning = false;
        public List<ITimerObserver> TimerObservers { get; set; } = new List<ITimerObserver>();
        public int SecondInLastGame => _secondsInLastGame;
        public GameTimer()
        {
            _timer = new Timer();
            _timer.Interval = 1000; // 1 second
            _timer.Tick += (s,e) =>
                {
                    _elapsedTime++;
                    NotifyObservers();
                };
        }
        public void StartTimer()
        {
            if (_isRunning) return; // Prevent multiple timers
            _isRunning = true;
            _timer.Start();
        }

        

        public void StopTimer()
        {
            if (!_isRunning) return; // Prevent stopping if not running
            _timer.Stop();
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
