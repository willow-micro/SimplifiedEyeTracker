﻿// A Simple Wrapper Library for the Tobii.Research.x64.
// Version: 0.0.3
// Copyright (C) 2021 T.Kawamura

// Default
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Additional
using System.Numerics;
// Additional (third party)
using Tobii.Research;

namespace SimplifiedEyeTracker
{
    /// <summary>
    /// Enum for specify eyetrackers at overloaded <c>EyeTracker</c> constructor.
    /// </summary>
    /// <seealso cref="SimplifiedEyeTracker.EyeTracker(SimplifiedEyeTracker.EyeTrackerIdentification, System.String)"/>
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
        /// <c>0.0</c> is the left edge, <c>1.0</c> or <c>screenWidth</c> is the right edge of the screen.
        /// </summary>
        public double LeftX;
        /// <summary>
        /// X Coordinate of the Right eye gaze in the user display area. 
        /// <c>0.0</c> is the left edge, <c>1.0</c> or <c>screenWidth</c> is the right edge of the screen.
        /// </summary>
        public double RightX;
        /// <summary>
        /// Y Coordinate of the Left eye gaze in the user display area. 
        /// <c>0.0</c> is the top edge, <c>1.0</c> or <c>screenHeight</c> is the bottom edge of the screen.
        /// </summary>
        public double LeftY;
        /// <summary>
        /// Y Coordinate of the Right eye gaze in the user display area. 
        /// <c>0.0</c> is the top edge, <c>1.0</c> or <c>screenHeight</c> is the bottom edge of the screen.
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
        /// <summary>
        /// [Left Eye] Gaze Vector from Gaze Origin to Gaze Point
        /// </summary>
        public Vector3 LeftGazeVector;
        /// <summary>
        /// [Right Eye] Gaze Vector from Gaze Origin to Gaze Point
        /// </summary>
        public Vector3 RightGazeVector;
        /// <summary>
        /// [Left Eye] Previous Gaze Vector from Gaze Origin to Gaze Point
        /// </summary>
        public Vector3 PrevLeftGazeVector;
        /// <summary>
        /// [Right Eye] Previous Gaze Vector from Gaze Origin to Gaze Point
        /// </summary>
        public Vector3 PrevRightGazeVector;
        /// <summary>
        /// [Left Eye] Angular displacement between previous gaze vector and latest gaze vector in degrees
        /// </summary>
        public Double LeftGazeAngularDisplacementInDeg;
        /// <summary>
        /// [Right Eye] Angular displacement between previous gaze vector and latest gaze vector in degrees
        /// </summary>
        public Double RightGazeAngularDisplacementInDeg;
        /// <summary>
        /// [Left Eye] Angular displacement between previous gaze vector and latest gaze vector in radians
        /// </summary>
        public Double LeftGazeAngularDisplacementInRad;
        /// <summary>
        /// [Right Eye] Angular displacement between previous gaze vector and latest gaze vector in radians
        /// </summary>
        public Double RightGazeAngularDisplacementInRad;
        /// <summary>
        /// Interval between previous system time stamp and latest system time stamp
        /// </summary>
        public int SystemTimeStampInterval;
        /// <summary>
        /// Angular Velocity for Left Gaze in deg/s
        /// </summary>
        public Double LeftGazeAngularVelocity;
        /// <summary>
        /// Angular Velocity for Right Gaze in deg/s
        /// </summary>
        public Double RightGazeAngularVelocity;
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
        /// Screen Width
        /// </summary>
        private readonly double screenWidth;
        /// <summary>
        /// Screen Height
        /// </summary>
        private readonly double screenHeight;

        /// <summary>
        /// [Left Eye] Previous Gaze Vector (from Gaze Origin to Gaze Point)
        /// </summary>
        private Vector3 prevLeftGazeUCSVector = new Vector3(0.0f, 0.0f, 0.0f);
        /// <summary>
        /// [Right Eye] Previous Gaze Vector (from Gaze Origin to Gaze Point)
        /// </summary>
        private Vector3 prevRightGazeUCSVector = new Vector3(0.0f, 0.0f, 0.0f);
        /// <summary>
        /// Previous System Time Stamp [us]
        /// </summary>
        private long prevSystemTimeStamp;

        /// <summary>
        /// Delegate for the event <see cref="E:SimplifiedEyeTracker.EyeTracker.OnGazeData"/>
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments <seealso cref="SimplifiedEyeTracker.SimplifiedGazeDataEventArgs"/></param>
        public delegate void SimplifiedGazeDataEventHandler(object sender, SimplifiedGazeDataEventArgs e);

        /// <summary>
        /// EventHandler for make gaze data available from user
        /// </summary>
        /// <remarks>
        /// If you specify a screen dimension in the constructor, that changes max values for gaze positions.
        /// </remarks>
        public event SimplifiedGazeDataEventHandler OnGazeData;

        /// <summary>
        /// Constructor. Use the first eye tracker in the <c>Tobii.Research.EyeTrackerCollection</c>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Eye Tracker(s) not found</exception>
        public EyeTracker()
        {
            EyeTrackerCollection eyeTrackers = EyeTrackingOperations.FindAllEyeTrackers();
            if (eyeTrackers.Count > 0)
            {
                this.device = eyeTrackers[0];
                this.screenWidth = 0.0;
                this.screenHeight = 0.0;
            }
            else
            {
                throw new InvalidOperationException("Eye Tracker(s) not found.");
            }
        }
        /// <summary>
        /// Overloaded constructor. Use the first eye tracker in the <c>Tobii.Research.EyeTrackerCollection</c>.
        /// </summary>
        /// <param name="screenWidth">Max value of gaze x positions in <see cref="E:SimplifiedEyeTracker.EyeTracker.OnGazeData"/></param>
        /// <param name="screenHeight">Max value of gaze y positions in <see cref="E:SimplifiedEyeTracker.EyeTracker.OnGazeData"/></param>
        /// <exception cref="ArgumentOutOfRangeException">Provided screen dimension is invalid</exception>
        /// <exception cref="InvalidOperationException">Eye tracker(s) not found</exception>
        public EyeTracker(double screenWidth, double screenHeight)
        {
            EyeTrackerCollection eyeTrackers = EyeTrackingOperations.FindAllEyeTrackers();
            if (screenWidth <= 0 || screenHeight <= 0)
            {
                throw new ArgumentOutOfRangeException("Provided screen dimension is invalid.");
            }            
            else if (eyeTrackers.Count > 0)
            {
                this.device = eyeTrackers[0];
                this.screenWidth = screenWidth;
                this.screenHeight = screenHeight;
            }
            else
            {
                throw new InvalidOperationException("Eye Tracker(s) not found.");
            }
        }
        /// <summary>
        /// Overloaded constructor. Use the eye tracker which has <c>identificationString</c> for <c>identificationType</c> in the <c>Tobii.Research.EyeTrackerCollection</c>.
        /// </summary>
        /// <param name="identificationType">The way to specify the eye tracker. <seealso cref="T:SimplifiedEyeTracker.EyeTrackerIdentification"/></param>
        /// <param name="identificationString">The string for specify the eye tracker.</param>
        /// <exception cref="InvalidOperationException">Eye Tracker(s) not found</exception>
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
                                this.screenWidth = 0.0;
                                this.screenHeight = 0.0;
                                initialized = true;
                                break;
                            }
                        }
                        if (!initialized)
                        {
                            throw new InvalidOperationException($"Eye Tracker not found with the Device Name: {identificationString}");
                        }
                        break;
                    case EyeTrackerIdentification.SerialNumber:
                        foreach (IEyeTracker eyeTracker in eyeTrackers)
                        {
                            if (eyeTracker.SerialNumber == identificationString)
                            {
                                this.device = eyeTracker;
                                this.screenWidth = 0.0;
                                this.screenHeight = 0.0;
                                initialized = true;
                                break;
                            }
                        }
                        if (!initialized)
                        {
                            throw new InvalidOperationException($"Eye Tracker not found with the Serial Number: {identificationString}");
                        }
                        break;
                    case EyeTrackerIdentification.Model:
                        foreach (IEyeTracker eyeTracker in eyeTrackers)
                        {
                            if (eyeTracker.Model == identificationString)
                            {
                                this.device = eyeTracker;
                                this.screenWidth = 0.0;
                                this.screenHeight = 0.0;
                                initialized = true;
                                break;
                            }
                        }
                        if (!initialized)
                        {
                            throw new InvalidOperationException($"Eye Tracker not found with the Model: {identificationString}");
                        }
                        break;
                    case EyeTrackerIdentification.FirmwareVersion:
                        foreach (IEyeTracker eyeTracker in eyeTrackers)
                        {
                            if (eyeTracker.FirmwareVersion == identificationString)
                            {
                                this.device = eyeTracker;
                                this.screenWidth = 0.0;
                                this.screenHeight = 0.0;
                                initialized = true;
                                break;
                            }
                        }
                        if (!initialized)
                        {
                            throw new InvalidOperationException($"Eye Tracker not found with the Firmware Version: {identificationString}");
                        }
                        break;
                    case EyeTrackerIdentification.RuntimeVersion:
                        foreach (IEyeTracker eyeTracker in eyeTrackers)
                        {
                            if (eyeTracker.RuntimeVersion == identificationString)
                            {
                                this.device = eyeTracker;
                                this.screenWidth = 0.0;
                                this.screenHeight = 0.0;
                                initialized = true;
                                break;
                            }
                        }
                        if (!initialized)
                        {
                            throw new InvalidOperationException($"Eye Tracker not found with the Runtime Version: {identificationString}");
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
        /// Overloaded constructor. Use the eye tracker which has <c>identificationString</c> for <c>identificationType</c> in the <c>Tobii.Research.EyeTrackerCollection</c>.
        /// </summary>
        /// <param name="identificationType">The way to specify the eye tracker. <seealso cref="T:SimplifiedEyeTracker.EyeTrackerIdentification"/></param>
        /// <param name="identificationString">The string for specify the eye tracker.</param>
        /// <param name="screenWidth">Max value of gaze x positions in <see cref="E:SimplifiedEyeTracker.EyeTracker.OnGazeData"/></param>
        /// <param name="screenHeight">Max value of gaze y positions in <see cref="E:SimplifiedEyeTracker.EyeTracker.OnGazeData"/></param>
        /// <exception cref="ArgumentOutOfRangeException">Provided screen dimension is invalid</exception>
        /// <exception cref="InvalidOperationException">Eye Tracker(s) not found</exception>
        public EyeTracker(EyeTrackerIdentification identificationType, string identificationString, double screenWidth, double screenHeight)
        {
            EyeTrackerCollection eyeTrackers = EyeTrackingOperations.FindAllEyeTrackers();
            if (screenWidth <= 0 || screenHeight <= 0)
            {
                throw new ArgumentOutOfRangeException("Provided screen dimension is invalid.");
            }
            else if (eyeTrackers.Count > 0)
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
                                this.screenWidth = screenWidth;
                                this.screenHeight = screenHeight;
                                initialized = true;
                                break;
                            }
                        }
                        if (!initialized)
                        {
                            throw new InvalidOperationException($"Eye Tracker not found with the Device Name: {identificationString}");
                        }
                        break;
                    case EyeTrackerIdentification.SerialNumber:
                        foreach (IEyeTracker eyeTracker in eyeTrackers)
                        {
                            if (eyeTracker.SerialNumber == identificationString)
                            {
                                this.device = eyeTracker;
                                this.screenWidth = screenWidth;
                                this.screenHeight = screenHeight;
                                initialized = true;
                                break;
                            }
                        }
                        if (!initialized)
                        {
                            throw new InvalidOperationException($"Eye Tracker not found with the Serial Number: {identificationString}");
                        }
                        break;
                    case EyeTrackerIdentification.Model:
                        foreach (IEyeTracker eyeTracker in eyeTrackers)
                        {
                            if (eyeTracker.Model == identificationString)
                            {
                                this.device = eyeTracker;
                                this.screenWidth = screenWidth;
                                this.screenHeight = screenHeight;
                                initialized = true;
                                break;
                            }
                        }
                        if (!initialized)
                        {
                            throw new InvalidOperationException($"Eye Tracker not found with the Model: {identificationString}");
                        }
                        break;
                    case EyeTrackerIdentification.FirmwareVersion:
                        foreach (IEyeTracker eyeTracker in eyeTrackers)
                        {
                            if (eyeTracker.FirmwareVersion == identificationString)
                            {
                                this.device = eyeTracker;
                                this.screenWidth = screenWidth;
                                this.screenHeight = screenHeight;
                                initialized = true;
                                break;
                            }
                        }
                        if (!initialized)
                        {
                            throw new InvalidOperationException($"Eye Tracker not found with the Firmware Version: {identificationString}");
                        }
                        break;
                    case EyeTrackerIdentification.RuntimeVersion:
                        foreach (IEyeTracker eyeTracker in eyeTrackers)
                        {
                            if (eyeTracker.RuntimeVersion == identificationString)
                            {
                                this.device = eyeTracker;
                                this.screenWidth = screenWidth;
                                this.screenHeight = screenHeight;
                                initialized = true;
                                break;
                            }
                        }
                        if (!initialized)
                        {
                            throw new InvalidOperationException($"Eye Tracker not found with the Runtime Version: {identificationString}");
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
        /// Get the Device Name for the eye tracker.
        /// </summary>
        /// <returns>Device Name (if there is no device, returns null)</returns>
        public string GetDeviceName()
        {
            return this.device?.DeviceName;
        }
        /// <summary>
        /// Get the Serial Number for the eye tracker.
        /// </summary>
        /// <returns>Serial Number (if there is no device, returns null)</returns>
        public string GetSerialNumber()
        {
            return this.device?.SerialNumber;
        }
        /// <summary>
        /// Get the Model for the eye tracker.
        /// </summary>
        /// <returns>Model (if there is no device, returns null)</returns>
        public string GetModel()
        {
            return this.device?.Model;
        }
        /// <summary>
        /// Get the Firmware Version for the eye tracker.
        /// </summary>
        /// <returns>Firmware Version (if there is no device, returns null)</returns>
        public string GetFirmwareVersion()
        {
            return this.device?.FirmwareVersion;
        }
        /// <summary>
        /// Get the Runtime Version for the eye tracker.
        /// </summary>
        /// <returns>Runtime Version (if there is no device, returns null)</returns>
        public string GetRuntimeVersion()
        {
            return this.device?.RuntimeVersion;
        }

        /// <summary>
        /// Get the display area of the eye tracker.
        /// </summary>
        /// <returns>Tobii.Research.DisplayArea (if there is no device, returns null)</returns>
        public DisplayArea GetDisplayArea()
        {
            return this.device?.GetDisplayArea();
        }

        /// <summary>
        /// Calculate the pixel pitch. To use this method, you must specify a screen dimension in the constructor.
        /// </summary>
        /// <param name="calcHorizontalPitch">If false, calculate a vertical pixel pitch</param>
        /// <returns>A horizontal or vertical pixel pitch</returns>
        /// <exception cref="InvalidOperationException">Screen dimensions are not available (You must set in the constructor).</exception>
        /// <exception cref="InvalidOperationException">Eye tracker not found.</exception>
        public double CalcPixelPitch(bool calcHorizontalPitch = true)
        {
            if (this.screenWidth <= 0.0 && this.screenHeight <= 0.0)
            {
                throw new InvalidOperationException("Screen dimensions are not available.");
            } 
            else if (this.device == null)
            {
                throw new InvalidOperationException("Eye tracker not found.");
            }
            if (calcHorizontalPitch)
            {
                return (double)(this.device.GetDisplayArea().Width / this.screenWidth);
            }
            else
            {
                return (double)(this.device.GetDisplayArea().Height / this.screenHeight);
            }
        }

        /// <summary>
        /// Calculate pixels from millimeters. To use this method, you must specify a screen dimension in the constructor.
        /// </summary>
        /// <param name="millimeters">Length in millimeters</param>
        /// <param name="useHorizontalPitch">if false, use vertical pixel pitch</param>
        /// <returns>pixels (based on the dimensions set in the constructor)</returns>
        /// <exception cref="InvalidOperationException">Screen dimensions are not available (You must set in the constructor).</exception>
        /// <exception cref="InvalidOperationException">Eye tracker not found.</exception>
        public double CalcPixelsFromMillimeters(double millimeters, bool useHorizontalPitch = true)
        {
            if (this.screenWidth <= 0.0 || this.screenHeight <= 0.0)
            {
                throw new InvalidOperationException("Screen dimensions are not available.");
            }
            else if (this.device == null)
            {
                throw new InvalidOperationException("Eye tracker not found.");
            }

            if (useHorizontalPitch)
            {
                double hpp = (double)(this.device.GetDisplayArea().Width / this.screenWidth);
                return millimeters / hpp;
            }
            else
            {
                double vpp = (double)(this.device.GetDisplayArea().Height / this.screenHeight);
                return millimeters / vpp;
            }
        }

        /// <summary>
        /// Calculate millimeters from pixels. To use this method, you must specify a screen dimension in the constructor.
        /// </summary>
        /// <param name="pixels">Length in pixels</param>
        /// <param name="useHorizontalPitch">if false, use vertical pixel pitch</param>
        /// <returns>millimeters (dependent on the dimensions set in the constructor)</returns>
        /// <exception cref="InvalidOperationException">Screen dimensions are not available (You must set in the constructor).</exception>
        /// <exception cref="InvalidOperationException">Eye tracker not found.</exception>
        public double CalcMillimetersFromPixels(double pixels, bool useHorizontalPitch = true)
        {
            if (this.screenWidth <= 0.0 || this.screenHeight <= 0.0)
            {
                throw new InvalidOperationException("Screen dimensions are not available.");
            }
            else if (this.device == null)
            {
                throw new InvalidOperationException("Eye tracker not found.");
            }

            if (useHorizontalPitch)
            {
                double hpp = (double)(this.device.GetDisplayArea().Width / this.screenWidth);
                return pixels * hpp;
            }
            else
            {
                double vpp = (double)(this.device.GetDisplayArea().Height / this.screenHeight);
                return pixels * vpp;
            }
        }

        /// <summary>
        /// Start for receiving gaze data. Before calling it, you must set <see cref="E:SimplifiedEyeTracker.EyeTracker.OnGazeData"/>
        /// </summary>
        /// <exception cref="InvalidOperationException">EyeTracker.OnGazeData is null</exception>
        public void StartReceivingGazeData()
        {
            if (this.device != null && this.OnGazeData != null)
            {
                this.device.GazeDataReceived += this.GazeDataReceived;
            }
            else
            {
                throw new InvalidOperationException("EyeTracker.OnGazeData is null. Cannot Start.");
            }
        }
        /// <summary>
        /// Stop for receiving gaze data. Before calling it, you must set <see cref="E:SimplifiedEyeTracker.EyeTracker.OnGazeData"/>
        /// </summary>
        /// <exception cref="InvalidOperationException">EyeTracker.OnGazeData is null</exception>
        public void StopReceivingGazeData()
        {
            if (this.device != null && this.OnGazeData != null)
            {
                this.device.GazeDataReceived -= this.GazeDataReceived;
            }
            else
            {
                throw new InvalidOperationException("EyeTracker.OnGazeData is null. Cannot Stop.");
            }
        }

        /// <summary>
        /// Internal gaze data receive handler for <c>Tobii.Research.IEyeTracker.GazeDataReceived</c>
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void GazeDataReceived(object sender, GazeDataEventArgs e)
        {
            bool isLeftValid = e.LeftEye.GazeOrigin.Validity == Validity.Valid && e.LeftEye.GazePoint.Validity == Validity.Valid;
            bool isRightValid = e.RightEye.GazeOrigin.Validity == Validity.Valid && e.RightEye.GazePoint.Validity == Validity.Valid;

            int systemTimeStampInterval = (int)(e.SystemTimeStamp - this.prevSystemTimeStamp);

            Vector3 leftGazePointUCSVector = new Vector3(e.LeftEye.GazePoint.PositionInUserCoordinates.X, e.LeftEye.GazePoint.PositionInUserCoordinates.Y, e.LeftEye.GazePoint.PositionInUserCoordinates.Z);
            Vector3 rightGazePointUCSVector = new Vector3(e.RightEye.GazePoint.PositionInUserCoordinates.X, e.RightEye.GazePoint.PositionInUserCoordinates.Y, e.RightEye.GazePoint.PositionInUserCoordinates.Z);
            Vector3 leftGazeOriginUCSVector = new Vector3(e.LeftEye.GazeOrigin.PositionInUserCoordinates.X, e.LeftEye.GazeOrigin.PositionInUserCoordinates.Y, e.LeftEye.GazeOrigin.PositionInUserCoordinates.Z);
            Vector3 rightGazeOriginUCSVector = new Vector3(e.RightEye.GazeOrigin.PositionInUserCoordinates.X, e.RightEye.GazeOrigin.PositionInUserCoordinates.Y, e.RightEye.GazeOrigin.PositionInUserCoordinates.Z);
            Vector3 leftGazeUCSVector = Vector3.Subtract(leftGazePointUCSVector, leftGazeOriginUCSVector);
            Vector3 rightGazeUCSVector = Vector3.Subtract(rightGazePointUCSVector, rightGazeOriginUCSVector);

            Double leftCosineTheta = (Double)(Vector3.Dot(this.prevLeftGazeUCSVector, leftGazeUCSVector) / (this.prevLeftGazeUCSVector.Length() * leftGazeUCSVector.Length()));
            Double leftThetaRad = Math.Acos(leftCosineTheta);
            Double leftThetaDeg = leftThetaRad * 180.0 / Math.PI;

            Double rightCosineTheta = (Double)(Vector3.Dot(this.prevRightGazeUCSVector, rightGazeUCSVector) / (this.prevRightGazeUCSVector.Length() * rightGazeUCSVector.Length()));
            Double rightThetaRad = Math.Acos(rightCosineTheta);
            Double rightThetaDeg = rightThetaRad * 180.0 / Math.PI;

            Double leftAngularVelocity = leftThetaDeg * 1000000 / systemTimeStampInterval;
            if (leftAngularVelocity == double.NaN)
            {
                isLeftValid = false;
            }
            Double rightAngularVelocity = rightThetaDeg * 1000000 / systemTimeStampInterval;
            if (rightAngularVelocity == double.NaN)
            {
                isRightValid = false;
            }

            if (this.screenWidth <= 0.0 || this.screenHeight <= 0.0)
            {
                SimplifiedGazeDataEventArgs gazeData = new SimplifiedGazeDataEventArgs()
                {
                    DeviceTimeStamp = e.DeviceTimeStamp,
                    SystemTimeStamp = e.SystemTimeStamp,
                    LeftX = Convert.ToDouble(e.LeftEye.GazePoint.PositionOnDisplayArea.X),
                    RightX = Convert.ToDouble(e.RightEye.GazePoint.PositionOnDisplayArea.X),
                    LeftY = Convert.ToDouble(e.LeftEye.GazePoint.PositionOnDisplayArea.Y),
                    RightY = Convert.ToDouble(e.RightEye.GazePoint.PositionOnDisplayArea.Y),
                    IsLeftValid = isLeftValid,
                    IsRightValid = isRightValid,
                    LeftGazeVector = leftGazeUCSVector,
                    RightGazeVector = rightGazeUCSVector,
                    PrevLeftGazeVector = prevLeftGazeUCSVector,
                    PrevRightGazeVector = prevRightGazeUCSVector,
                    LeftGazeAngularDisplacementInDeg = leftThetaDeg,
                    LeftGazeAngularDisplacementInRad = leftThetaRad,
                    RightGazeAngularDisplacementInDeg = rightThetaDeg,
                    RightGazeAngularDisplacementInRad = rightThetaRad,
                    SystemTimeStampInterval = systemTimeStampInterval,
                    LeftGazeAngularVelocity = leftAngularVelocity,
                    RightGazeAngularVelocity = rightAngularVelocity
                };
                this.OnGazeData?.Invoke(this, gazeData);
            }
            else
            {
                SimplifiedGazeDataEventArgs gazeDataUsingScreenDimension = new SimplifiedGazeDataEventArgs()
                {
                    DeviceTimeStamp = e.DeviceTimeStamp,
                    SystemTimeStamp = e.SystemTimeStamp,
                    LeftX = Convert.ToDouble(e.LeftEye.GazePoint.PositionOnDisplayArea.X) * this.screenWidth,
                    RightX = Convert.ToDouble(e.RightEye.GazePoint.PositionOnDisplayArea.X) * this.screenWidth,
                    LeftY = Convert.ToDouble(e.LeftEye.GazePoint.PositionOnDisplayArea.Y) * this.screenHeight,
                    RightY = Convert.ToDouble(e.RightEye.GazePoint.PositionOnDisplayArea.Y) * this.screenHeight,
                    IsLeftValid = isLeftValid,
                    IsRightValid = isRightValid,
                    LeftGazeVector = leftGazeUCSVector,
                    RightGazeVector = rightGazeUCSVector,
                    PrevLeftGazeVector = prevLeftGazeUCSVector,
                    PrevRightGazeVector = prevRightGazeUCSVector,
                    LeftGazeAngularDisplacementInDeg = leftThetaDeg,
                    LeftGazeAngularDisplacementInRad = leftThetaRad,
                    RightGazeAngularDisplacementInDeg = rightThetaDeg,
                    RightGazeAngularDisplacementInRad = rightThetaRad,
                    SystemTimeStampInterval = systemTimeStampInterval,
                    LeftGazeAngularVelocity = leftAngularVelocity,
                    RightGazeAngularVelocity = rightAngularVelocity
                };
                this.OnGazeData?.Invoke(this, gazeDataUsingScreenDimension);
            }

            this.prevLeftGazeUCSVector = leftGazeUCSVector;
            this.prevRightGazeUCSVector = rightGazeUCSVector;
            this.prevSystemTimeStamp = e.SystemTimeStamp;
        }
    }
}

