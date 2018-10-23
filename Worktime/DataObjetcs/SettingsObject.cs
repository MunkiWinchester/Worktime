namespace Worktime.DataObjects
{
    public class SettingsObject
    {
        public double Top { get; set; }
        public double Left { get; set; }
        public string SelectedAccent { get; set; }
        public string SelectedTheme { get; set; }
        public bool IsAlwaysOnTop { get; set; }
        public string SelectedState { get; set; }

        public SettingsObject()
        {
        }

        public SettingsObject(bool useDefaults)
        {
            Top = 50;
            Left = 50;
            SelectedAccent = "Crimson";
            SelectedTheme = "BaseDark";
            SelectedState = "RP";
        }
    }
}
