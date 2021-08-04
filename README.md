# SimplifiedEyeTracker
[![CodeQL](https://github.com/willow-micro/SimplifiedEyeTracker/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/willow-micro/SimplifiedEyeTracker/actions/workflows/codeql-analysis.yml) ![.NET Framework is 4.7.2](https://img.shields.io/badge/.NET_Framework-4.7.2-blueviolet) ![Tobii.Research.x64 is 1.8.0.1108](https://img.shields.io/badge/Tobii.Research.x64-1.8.0.1108-orange)

This is a simple wrapper library for Tobii.Research.x64.

## Restrictions
- .NET Framework Environment (Tobii.Research.x64 was built with .NET Framework 4.5)
- x64 Target

## Installation
1. Download the .nupkg and place it in any directory.
1. In the Solution Explorer, Right click *ProjectName* and Select "Manage NuGet Packages..."
1. Click the settings icon in the top right of the NuGet window, then add the directory as a new package source and click "OK".
1. Set the package source of NuGet to "All" or *DirectoryName* from the pull down menu on the left of the settings icon.
1. Click "Browse" in the top left of the NuGet window and Search for "SimplifiedEyeTracker".
1. Select "SimplifiedEyeTracker" and Install it.

## Example
This example shows a gray circle at current gaze position of the left eye in the screen using WPF Canvas. 
(*mainCanvas is defined in the xaml file.)

```MainWindow.xaml.cs
// Default
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
// Additional
using System.Diagnostics;
using SimplifiedEyeTracker;

namespace Example
{
   public partial class MainWindow : Window
    {
        private readonly EyeTracker eyeTracker;
		private	readonly Ellipse gazeCircleShape;
		private readonly double primaryScreenWidth;
		private	readonly double	primaryScreenHeight;

        public MainWindow()
        {
            InitializeComponent();

            this.primaryScreenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            this.primaryScreenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;

            this.gazeCircleShape = new Ellipse()
            {
                Fill = Brushes.Gray,
                Width = 8.0, Height = 8.0
            };
            mainCanvas.Children.Add(this.gazeCircleShape);

            try
            {
                this.eyeTracker = new EyeTracker(System.Windows.SystemParameters.PrimaryScreenWidth, System.Windows.SystemParameters.PrimaryScreenHeight);
                Debug.Print($"Model: {this.eyeTracker.GetModel()}");
                Debug.Print($"SerialNumber: {this.eyeTracker.GetSerialNumber()}");
                this.eyeTracker.OnGazeData += this.OnGazeData;
                this.eyeTracker.StartReceivingGazeData();
            }
            catch(InvalidOperationException e)
            {
                if (MessageBox.Show(e.Message, "OK", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    this.Close();
                }
            }
        }

        private void OnGazeData(object sender, SimplifiedGazeDataEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (e.IsLeftValid && e.LeftEyeMovementType == EyeMovementType.EyeMovementType.Fixation && e.LeftX >= 0.0 && e.LeftX <= this.eyeTracker.GetScreenWidthInPixels() && e.LeftY >= 0.0 && e.LeftY <= this.eyeTracker.GetScreenHeightInPixels())
                {
                    Debug.Print($"DeviceTimeStamp: {e.DeviceTimeStamp}, SystemTimeStamp: {e.SystemTimeStamp}");
                    Debug.Print($"X: {e.LeftX}, Y: {e.LeftY}");
                    if (mainCanvas.Children.Contains(this.gazeCircleShape))
                    {
                        Canvas.SetLeft(this.gazeCircleShape, e.LeftX);
                        Canvas.SetTop(this.gazeCircleShape, e.LeftY);
                    }
                }
            });
        }
    }
}
```

## License
MIT License.