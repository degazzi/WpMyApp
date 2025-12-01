using System.ComponentModel;
using WpMyApp.Models;
using WpMyApp.Models;

namespace WpMyApp.Services
{
    public class StatusService : INotifyPropertyChanged
    {
        private OperationStatus _currentStatus = new OperationStatus();

        public OperationStatus CurrentStatus
        {
            get => _currentStatus;
            set
            {
                _currentStatus = value;
                OnPropertyChanged(nameof(CurrentStatus));
            }
        }

        public void SetStatus(StatusType type, string message)
        {
            CurrentStatus = new OperationStatus
            {
                StatusType = type,
                Message = message
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
