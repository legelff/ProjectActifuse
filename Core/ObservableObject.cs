using ProjectActifuse.Core;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ProjectActifuse.Core
{
    class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private RelayCommand openFileExplorerCommand;
        public ICommand OpenFileExplorerCommand => openFileExplorerCommand ??= new RelayCommand(OpenFileExplorer);

        private void OpenFileExplorer(object commandParameter)
        {
        }

        protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }

            return false;
        }

        private System.Windows.Media.ImageSource selectedProfilePicture;

        public System.Windows.Media.ImageSource SelectedProfilePicture { get => selectedProfilePicture; set => SetProperty(ref selectedProfilePicture, value); }
    }
}
