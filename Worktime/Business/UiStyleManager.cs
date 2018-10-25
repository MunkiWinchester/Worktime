using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using MahApps.Metro;
using Point = System.Windows.Point;
using Application = System.Windows.Application;
using Worktime.DataObjetcs;

namespace Worktime.Business
{
    public static class UiStyleManager
    {
        public const string WindowsAccentName = "Windows Accent";
        private const string DefaultAccentName = "Crimson";

        public static event EventHandler<StyleChangeEventArgs> IsStyleChanged;

        public static AppTheme CurrentTheme =>
                ThemeManager.AppThemes.FirstOrDefault(t => t.Name == Settings.Default.SelectedTheme)
                ?? ThemeManager.DetectAppStyle().Item1;
        public static Accent CurrentAccent =>
                ThemeManager.Accents.FirstOrDefault(a => a.Name == Settings.Default.SelectedAccent)
                ?? ThemeManager.GetAccent(DefaultAccentName);

        public static void InitializeTheme()
        {
            if (Helper.IsWindows8() || Helper.IsWindows10())
                CreateWindowsAccentStyle();
            else if (Settings.Default.SelectedAccent == WindowsAccentName)
            {
                // In case if somehow user will get "Windows Accent" on Windows which not support this.
                // (For example move whole on diffrent machine instead of fresh install)
                Settings.Default.SelectedAccent = DefaultAccentName;
                Settings.Save();
            }
            ChangeAppStyle(CurrentAccent, CurrentTheme);
        }

        public static void CreateWindowsAccentStyle(bool changeImmediately = false, string themeName = null)
        {
            var resourceDictionary = new ResourceDictionary();

            var color = new SolidColorBrush(AccentColorSet.ActiveSet["SystemAccent"]).Color; //SystemParameters.WindowGlassColor;

            resourceDictionary.Add("HighlightColor", color);
            resourceDictionary.Add("AccentColor", Color.FromArgb(204, color.R, color.G, color.B));
            resourceDictionary.Add("AccentColor2", Color.FromArgb(153, color.R, color.G, color.B));
            resourceDictionary.Add("AccentColor3", Color.FromArgb(102, color.R, color.G, color.B));
            resourceDictionary.Add("AccentColor4", Color.FromArgb(51, color.R, color.G, color.B));
            resourceDictionary.Add("HighlightBrush", new SolidColorBrush((Color)resourceDictionary["HighlightColor"]));
            resourceDictionary.Add("AccentColorBrush", new SolidColorBrush((Color)resourceDictionary["AccentColor"]));
            resourceDictionary.Add("AccentColorBrush2", new SolidColorBrush((Color)resourceDictionary["AccentColor2"]));
            resourceDictionary.Add("AccentColorBrush3", new SolidColorBrush((Color)resourceDictionary["AccentColor3"]));
            resourceDictionary.Add("AccentColorBrush4", new SolidColorBrush((Color)resourceDictionary["AccentColor4"]));
            resourceDictionary.Add("WindowTitleColorBrush", new SolidColorBrush((Color)resourceDictionary["AccentColor"]));
            resourceDictionary.Add("ProgressBrush", new LinearGradientBrush(new GradientStopCollection(new[]
                {
                    new GradientStop((Color)resourceDictionary["HighlightColor"], 0),
                    new GradientStop((Color)resourceDictionary["AccentColor3"], 1)
                }),
                new Point(0.001, 0.5), new Point(1.002, 0.5)));

            resourceDictionary.Add("CheckmarkFill", new SolidColorBrush((Color)resourceDictionary["AccentColor"]));
            resourceDictionary.Add("RightArrowFill", new SolidColorBrush((Color)resourceDictionary["AccentColor"]));

            resourceDictionary.Add("IdealForegroundColor", Colors.White);

            resourceDictionary.Add("IdealForegroundColorBrush", new SolidColorBrush((Color)resourceDictionary["IdealForegroundColor"]));
            resourceDictionary.Add("AccentSelectedColorBrush", new SolidColorBrush((Color)resourceDictionary["IdealForegroundColor"]));
            resourceDictionary.Add("MetroDataGrid.HighlightBrush", new SolidColorBrush((Color)resourceDictionary["AccentColor"]));
            resourceDictionary.Add("MetroDataGrid.HighlightTextBrush", new SolidColorBrush((Color)resourceDictionary["IdealForegroundColor"]));
            resourceDictionary.Add("MetroDataGrid.MouseOverHighlightBrush", new SolidColorBrush((Color)resourceDictionary["AccentColor3"]));
            resourceDictionary.Add("MetroDataGrid.FocusBorderBrush", new SolidColorBrush((Color)resourceDictionary["AccentColor"]));
            resourceDictionary.Add("MetroDataGrid.InactiveSelectionHighlightBrush", new SolidColorBrush((Color)resourceDictionary["AccentColor2"]));
            resourceDictionary.Add("MetroDataGrid.InactiveSelectionHighlightTextBrush", new SolidColorBrush((Color)resourceDictionary["IdealForegroundColor"]));

            var fileName = Path.Combine(Helper.LoadAssemblyDirectory(), "WindowsAccent.xaml");

            try
            {
                using (var stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true }))
                    {
                        XamlWriter.Save(resourceDictionary, writer);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error("Error creating WindowsAccent.", e);
                return;
            }

            resourceDictionary = new ResourceDictionary { Source = new Uri(Path.GetFullPath(fileName), UriKind.Absolute) };

            ThemeManager.AddAccent(WindowsAccentName, resourceDictionary.Source);

            var windowsAccent = ThemeManager.GetAccent(WindowsAccentName);
            windowsAccent.Resources.Source = resourceDictionary.Source;

            var theme = CurrentTheme;
            if (!string.IsNullOrWhiteSpace(themeName))
            {
                var themeLoaded = ThemeManager.GetAppTheme(themeName);
                if(themeLoaded != null)
                    theme = themeLoaded;
            }
                
            if (changeImmediately)
                ChangeAppStyle(windowsAccent, theme);
        }

        public static void ChangeAppStyle(Accent accent, AppTheme theme)
        {
            ThemeManager.ChangeAppStyle(Application.Current, accent, theme);
            IsStyleChanged?.Invoke(typeof(UiStyleManager), new StyleChangeEventArgs(accent, theme));
        }

        public static void ChangeAppStyle(string accent, string theme)
        {
            ChangeAppStyle(ThemeManager.GetAccent(accent), ThemeManager.GetAppTheme(theme));
        }
    }
}
