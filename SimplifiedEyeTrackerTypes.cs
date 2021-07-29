// Type Definitions
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


namespace SimplifiedEyeTracker
{
    /// <summary>
    /// Enum to specify eyetrackers at overloaded <c>EyeTracker</c> constructor.
    /// </summary>
    /// <seealso cref="SimplifiedEyeTracker.EyeTracker(EyeTrackerIdentification, System.String, double, double, SimplifiedEyeTracker.VelocityCalcType, int)"/>
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
    /// Enum to specify gaze angular velocity calculation type
    /// </summary>
    public enum VelocityCalcType
    {
        /// <summary>
        /// Calculate velocity using UCS gaze vectors from gaze origin to gaze point.
        /// </summary>
        UCSGazeVector,
        /// <summary>
        /// Calculate velocity using the pixel pitch and the distance from gaze origin to gaze point.
        /// </summary>
        PixelPitchAndUCSDistance
    }

    /// <summary>
    /// Enum to describe eye movement
    /// </summary>
    public enum EyeMovementType
    {
        /// <summary>
        /// Fixation
        /// </summary>
        Fixation,
        /// <summary>
        /// Saccade
        /// </summary>
        Saccade,
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown
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
        /// Interval between the previous system time stamp and the latest system time stamp
        /// </summary>
        public int SystemTimeStampInterval;
        /// <summary>
        /// Angular Velocity for Left Gaze in deg/s
        /// </summary>
        public double LeftGazeAngularVelocity;
        /// <summary>
        /// Angular Velocity for Right Gaze in deg/s
        /// </summary>
        public double RightGazeAngularVelocity;
        /// <summary>
        /// [Left Eye] Movement Type (Fixation or Saccade)
        /// </summary>
        public EyeMovementType LeftEyeMovementType;
        /// <summary>
        /// [Left Eye] Movement Type (Fixation or Saccade)
        /// </summary>
        public EyeMovementType RightEyeMovementType;
    }
}