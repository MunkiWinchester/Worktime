using System.ComponentModel;
using System.Runtime.CompilerServices;
using Worktime.Business;

namespace Worktime.Updater
{
    public partial class SplashScreenWindow : INotifyPropertyChanged
    {
        private const string LocLoading = "SplashScreen_Text_Loading";
        private const string LocUpdating = "SplashScreen_Text_Updating";
        private const string LocInstalling = "SplashScreen_Text_Installing";
        private readonly string _updating = LocUpdating;
        private readonly string _installing = LocInstalling;
        private string _loadingString = LocLoading;
        private string _versionString = Helpers.GetCurrentVersion().ToVersionString();

        public SplashScreenWindow()
        {
            InitializeComponent();
        }

        public string VersionString
        {
            get { return _versionString; }
            set
            {
                _versionString = value;
                OnPropertyChanged();
            }
        }

        public string LoadingString
        {
            get { return _loadingString; }
            set
            {
                _loadingString = value;
                OnPropertyChanged();
            }
        }

        public void Updating(int percentage)
        {
            LoadingString = _updating;
            VersionString = percentage + "%";
        }

        public void Installing(int percentage)
        {
            LoadingString = _installing;
            VersionString = percentage + "%";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
