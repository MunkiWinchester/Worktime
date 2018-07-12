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
using Microsoft.Win32;
using System.Reflection;

namespace Worktime.Business
{
	public class UiTheme
	{
		private const string WindowAccentName = "Windows Accent";
		private const string DefaultAccentName = "Crimson";
		private static Color _currentWindowsAccent = SystemParameters.WindowGlassColor;

		public static AppTheme CurrentTheme => ThemeManager.AppThemes.FirstOrDefault(t => t.Name == Properties.Settings.Default.SelectedTheme) ?? ThemeManager.DetectAppStyle().Item1;
		public static Accent CurrentAccent => ThemeManager.Accents.FirstOrDefault(a => a.Name == Properties.Settings.Default.SelectedAccent) ?? ThemeManager.GetAccent(DefaultAccentName);

		public static void InitializeTheme()
		{
			if (IsWindows8() || IsWindows10())
				CreateWindowsAccentStyle();
			else if (Properties.Settings.Default.SelectedAccent == WindowAccentName)
			{
				// In case if somehow user will get "Windows Accent" on Windows which not support this.
				// (For example move whole on diffrent machine instead of fresh install)
				Properties.Settings.Default.SelectedAccent = DefaultAccentName;
				Properties.Settings.Default.Save();
			}
			ThemeManager.ChangeAppStyle(Application.Current, CurrentAccent, CurrentTheme);
		}

		public static void CreateWindowsAccentStyle(bool changeImmediately = false)
		{
			var resourceDictionary = new ResourceDictionary();

			var color = SystemParameters.WindowGlassColor;

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

			var fileName = Path.Combine(LoadAssemblyDirectory(), "WindowsAccent.xaml");

			try
			{
				using (var stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
				using (var writer = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true }))
					XamlWriter.Save(resourceDictionary, writer);
			}
			catch (Exception e)
			{
				Console.WriteLine("Error creating WindowsAccent: " + e);
				return;
			}

			resourceDictionary = new ResourceDictionary { Source = new Uri(Path.GetFullPath(fileName), UriKind.Absolute) };

			ThemeManager.AddAccent(WindowAccentName, resourceDictionary.Source);

			var oldWindowsAccent = ThemeManager.GetAccent(WindowAccentName);
			oldWindowsAccent.Resources.Source = resourceDictionary.Source;

			if (changeImmediately)
				ThemeManager.ChangeAppStyle(Application.Current, CurrentAccent, CurrentTheme);
		}

		private static string LoadAssemblyDirectory()
		{
			var codeBase = Assembly.GetExecutingAssembly().CodeBase;
			var uri = new UriBuilder(codeBase);
			var path = Uri.UnescapeDataString(uri.Path);
			return Path.GetDirectoryName(path);
		}

		public static bool IsWindows10()
		{
			try
			{
				var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
				return reg != null && ((string)reg.GetValue("ProductName")).Contains("Windows 10");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		public static bool IsWindows8()
		{
			try
			{
				var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
				return reg != null && ((string)reg.GetValue("ProductName")).Contains("Windows 8");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}
	}
}
