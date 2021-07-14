﻿/// <summary>
/// A Simple Wrapper Library for the Tobii.Research.x64.
/// Version: 0.0.1
/// Copyright (C) 2021 T.Kawamura
/// </summary>

// Default
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Additional
// Additional (third party)
using Tobii.Research;

namespace SimplifiedEyeTracker
{
    /// <summary>
    /// Enum for specify eyetrackers at overloaded <c>EyeTracker</c> constructor.
    /// </summary>
    /// <seealso cref="M:SimplifiedEyeTracker.EyeTracker.#ctor(SimplifiedEyeTracker.EyeTrackerIdentification, System.String)"/>
    public enum EyeTrackerIdentification
    {
        /// <summary>
        /// Device Name of the device.
        /// </summary>
        DeviceName,
        /// <summary>
        /// Serial Number of the device.
        /// </summary>
        SerialNumber,
        /// <summary>
        /// Model of the device.
        /// </summary>
        Model,
        /// <summary>
        /// Firmware Version of the device.
        /// </summary>
        FirmwareVersion,
        /// <summary>
        /// Runtime Version of the device.
        /// </summary>
        RuntimeVersion
    }

    /// <summary>
    /// EventArgs for <see cref="E:SimplifiedEyeTracker.EyeTracker.OnGazeData"/>
    /// </summary>
    public sealed class SimplifiedGazeDataEventArgs : EventArgs
    {
        /// <summary>
        /// Device Time Stamp.
        /// </summary>
        public long DeviceTimeStamp;
        /// <summary>
        /// System Time Stamp.
        /// </summary>
        public long SystemTimeStamp;
        /// <summary>
        /// X Coordinate of the Left eye gaze in the user display area. 
        /// <c>0.0</c> is the left edge, <c>1.0</c> is the right edge of the screen.
        /// </summary>
        public double LeftX;
        /// <summary>
        /// X Coordinate of the Right eye gaze in the user display area. 
        /// <c>0.0</c> is the left edge, <c>1.0</c> is the right edge of the screen.
        /// </summary>
        public double RightX;
        /// <summary>
        /// Y Coordinate of the Left eye gaze in the user display area. 
        /// <c>0.0</c> is the top edge, <c>1.0</c> is the bottom edge of the screen.
        /// </summary>
        public double LeftY;
        /// <summary>
        /// Y Coordinate of the Right eye gaze in the user display area. 
        /// <c>0.0</c> is the top edge, <c>1.0</c> is the bottom edge of the screen.
        /// </summary>
        public double RightY;
        /// <summary>
        /// If the Left eye is closed, it becomes false.
        /// </summary>
        public bool IsLeftValid;
        /// <summary>
        /// If the Right eye is closed, it becomes false.
        /// </summary>
        public bool IsRightValid;
    }

    /// <summary>
    /// Simplified wrapper class for Tobii.Research.x64
    /// </summary>
    public sealed class EyeTracker
    {
        /// <summary>
        /// Tobii.Research eye tracker object
        /// </summary>
        private readonly IEyeTracker device;

        /// <summary>
        /// Delegate for the event <see cref="E:SimplifiedEyeTracker.EyeTracker.OnGazeData"/>
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments <seealso cref="T:SimplifiedEyeTracker.SimplifiedGazeDataEventArgs"/></param>
        public delegate void SimplifiedGazeDataEventHandler(object sender, SimplifiedGazeDataEventArgs e);

        /// <summary>
        /// EventHandler for make gaze data available from user
        /// </summary>
        public event SimplifiedGazeDataEventHandler OnGazeData;

        /// <summary>
        /// Constructor. Use the first eye tracker in the <c>Tobii.Research.EyeTrackerCollection</c>.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public EyeTracker()
        {
            EyeTrackerCollection eyeTrackers = EyeTrackingOperations.FindAllEyeTrackers();
            if (eyeTrackers.Count > 0)
            {
                this.device = eyeTrackers[0];
            }
            else
            {
                throw new InvalidOperationException("Eye Tracker(s) not found.");
            }
        }
        /// <summary>
        /// Overloaded Constructor. Use the eye tracker which has <c>identificationString</c> for <c>identificationType</c> in the <c>Tobii.Research.EyeTrackerCollection</c>.
        /// </summary>
        /// <param name="identificationType">The way to specify the eye tracker. <seealso cref="T:SimplifiedEyeTracker.EyeTrackerIdentification"/></param>
        /// <param name="identificationString">The string for specify the eye tracker.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public EyeTracker(EyeTrackerIdentification identificationType, string identificationString)
        {
            EyeTrackerCollection eyeTrackers = EyeTrackingOperations.FindAllEyeTrackers();
            if (eyeTrackers.Count > 0)
            {
                bool initialized = false;
                switch (identificationType)
                {
                    case EyeTrackerIdentification.DeviceName:
                        foreach (IEyeTracker eyeTracker in eyeTrackers)
                        {
                            if (eyeTracker.DeviceName == identificationString)
                            {
                                this.device = eyeTracker;
                                initialized = true;
                                break;
                            }
                        }
                        if (!initialized)
                        {
                            throw new InvalidOperationException($"Eye Tracker not found at the Device Name: {identificationString}");
                        }
                        break;
                    case EyeTrackerIdentification.SerialNumber:
                        foreach (IEyeTracker eyeTracker in eyeTrackers)
                        {
                            if (eyeTracker.SerialNumber == identificationString)
                            {
                                this.device = eyeTracker;
                                initialized = true;
                                break;
                            }
                        }
                        if (!initialized)
                        {
                            throw new InvalidOperationException($"Eye Tracker not found at the Serial Number: {identificationString}");
                        }
                        break;
                    case EyeTrackerIdentification.Model:
                        foreach (IEyeTracker eyeTracker in eyeTrackers)
                        {
                            if (eyeTracker.Model == identificationString)
                            {
                                this.device = eyeTracker;
                                initialized = true;
                                break;
                            }
                        }
                        if (!initialized)
                        {
                            throw new InvalidOperationException($"Eye Tracker not found at the Model: {identificationString}");
                        }
                        break;
                    case EyeTrackerIdentification.FirmwareVersion:
                        foreach (IEyeTracker eyeTracker in eyeTrackers)
                        {
                            if (eyeTracker.FirmwareVersion == identificationString)
                            {
                                this.device = eyeTracker;
                                initialized = true;
                                break;
                            }
                        }
                        if (!initialized)
                        {
                            throw new InvalidOperationException($"Eye Tracker not found at the Firmware Version: {identificationString}");
                        }
                        break;
                    case EyeTrackerIdentification.RuntimeVersion:
                        foreach (IEyeTracker eyeTracker in eyeTrackers)
                        {
                            if (eyeTracker.RuntimeVersion == identificationString)
                            {
                                this.device = eyeTracker;
                                initialized = true;
                                break;
                            }
                        }
                        if (!initialized)
                        {
                            throw new InvalidOperationException($"Eye Tracker not found at the Runtime Version: {identificationString}");
                        }
                        break;
                }
            }
            else
            {
                throw new InvalidOperationException("Eye Tracker(s) not found.");
            }
        }

        /// <summary>
        /// Get the Device Name for the current eye tracker.
        /// </summary>
        /// <returns>Device Name (if there is no device, returns null)</returns>
        public string GetDeviceName()
        {
            return this.device?.DeviceName;
        }
        /// <summary>
        /// Get the Serial Number for the current eye tracker.
        /// </summary>
        /// <returns>Serial Number (if there is no device, returns null)</returns>
        public string GetSerialNumber()
        {
            return this.device?.SerialNumber;
        }
        /// <summary>
        /// Get the Model for the current eye tracker.
        /// </summary>
        /// <returns>Model (if there is no device, returns null)</returns>
        public string GetModel()
        {
            return this.device?.Model;
        }
        /// <summary>
        /// Get the Firmware Version for the current eye tracker.
        /// </summary>
        /// <returns>Firmware Version (if there is no device, returns null)</returns>
        public string GetFirmwareVersion()
        {
            return this.device?.FirmwareVersion;
        }
        /// <summary>
        /// Get the Runtime Version for the current eye tracker.
        /// </summary>
        /// <returns>Runtime Version (if there is no device, returns null)</returns>
        public string GetRuntimeVersion()
        {
            return this.device?.RuntimeVersion;
        }

        /// <summary>
        /// Start for receiving gaze data. Before calling it, you must set <see cref="E:SimplifiedEyeTracker.EyeTracker.OnGazeData"/>
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void StartReceivingGazeData()
        {
            if (this.device != null && this.OnGazeData != null)
            {
                this.device.GazeDataReceived += this.GazeDataReceived;
            }
            else
            {
                throw new InvalidOperationException("EyeTracker.OnGazeData is null. Cannot Start Listening Gaze Data.");
            }
        }
        /// <summary>
        /// Stop for receiving gaze data. Before calling it, you must set <see cref="E:SimplifiedEyeTracker.EyeTracker.OnGazeData"/>
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void StopReceivingGazeData()
        {
            if (this.device != null && this.OnGazeData != null)
            {
                this.device.GazeDataReceived -= this.GazeDataReceived;
            }
            else
            {
                throw new InvalidOperationException("EyeTracker.OnGazeData is null. Cannot Stop Listening Gaze Data.");
            }
        }

        /// <summary>
        /// Internal gaze data receive handler for <c>Tobii.Research.IEyeTracker.GazeDataReceived</c>
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void GazeDataReceived(object sender, GazeDataEventArgs e)
        {
            SimplifiedGazeDataEventArgs gazeData = new SimplifiedGazeDataEventArgs()
            {
                DeviceTimeStamp = e.DeviceTimeStamp,
                SystemTimeStamp = e.SystemTimeStamp,
                LeftX = Convert.ToDouble(e.LeftEye.GazePoint.PositionOnDisplayArea.X),
                RightX = Convert.ToDouble(e.RightEye.GazePoint.PositionOnDisplayArea.X),
                LeftY = Convert.ToDouble(e.LeftEye.GazePoint.PositionOnDisplayArea.Y),
                RightY = Convert.ToDouble(e.RightEye.GazePoint.PositionOnDisplayArea.Y),
                IsLeftValid = e.LeftEye.GazeOrigin.Validity == Validity.Valid,
                IsRightValid = e.RightEye.GazeOrigin.Validity == Validity.Valid
            };
            this.OnGazeData?.Invoke(this, gazeData);
        }
    }
}

