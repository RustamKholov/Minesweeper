namespace Minesweeper.Interfaces
{
    public interface IGameTimer
    {
        int SecondInLastGame { get; }
        List<ITimerObserver> TimerObservers { get; set; }

        void Dispose();
        void NotifyObservers();
        void StartTimer();
        void StopTimer();
        void Subscribe(ITimerObserver observer);
        void Unsubscribe(ITimerObserver observer);
        void UnsubscribeAll();
    }
}