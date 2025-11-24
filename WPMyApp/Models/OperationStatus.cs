// Models/OperationStatus.cs
using CommunityToolkit.Mvvm.ComponentModel;

namespace WpMyApp.Models
{
    public enum StatusType
    {
        Idle,
        Loading,
        Success,
        Error,
        Saving,
        Deleting
    }

    public partial class OperationStatus : ObservableObject
    {
        [ObservableProperty]
        private StatusType currentStatus = StatusType.Idle;

        [ObservableProperty]
        private string message = "Ready";

        [ObservableProperty]
        private bool isBusy = false;

        [ObservableProperty]
        private bool hasError = false;

        public void SetStatus(StatusType status, string message = "")
        {
            CurrentStatus = status;
            Message = message;
            IsBusy = status == StatusType.Loading || status == StatusType.Saving || status == StatusType.Deleting;
            HasError = status == StatusType.Error;
        }
    }
}
