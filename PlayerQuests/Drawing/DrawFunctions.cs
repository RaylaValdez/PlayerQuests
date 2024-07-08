// DrawFunctions.cs
using PlayerQuests.Drawing;
using System;
using System.Numerics;
using CameraManager = FFXIVClientStructs.FFXIV.Client.Game.Control.CameraManager;


namespace PlayerQuests.Drawing
{
    internal class DrawFunctions
    {
        private static float currentRotationAngle = 0f;
        private static float PreviousSmoothedSigmoidValue = 0.5f; // Initialize to a neutral value for smoothing

        // Unsafe method to get the camera distance
        private static unsafe float GetCameraDistance()
        {
            var cameraInstance = CameraManager.Instance();
            var activeCamera = cameraInstance->GetActiveCamera();
            return activeCamera->Distance;
        }

        // Method to calculate the scale factor based on camera distance using sigmoid
        private static float CalculateScaleFactor(float cameraDistance, float standardDistance = 1f, float smoothingFactor = 0.1f)
        {
            // Use sigmoid to normalize the camera distance
            float normalizedDistance = Sigmoid(cameraDistance - standardDistance);
            // Use smoothed sigmoid for smoother transitions
            return SmoothedSigmoid(normalizedDistance, smoothingFactor);
        }

        internal static void CircleXZ(Vector3 gamePos, float radius, Brush brush)
        {
            float cameraDistance = GetCameraDistance();
            float scaleFactor = CalculateScaleFactor(cameraDistance);

            radius *= scaleFactor; // Adjust radius based on camera distance
            brush.Thickness *= scaleFactor;

            var startRads = 0f;
            var endRads = MathF.Tau;
            var shape = new ConvexShape(brush);
            shape.Arc(gamePos, radius, startRads, endRads);
            shape.Done();
        }

        internal static void RotatingCircle4SegmentsXZ(Vector3 gamePos, float radius, Brush brush, float rotationOffset = 0f, float gapRads = MathF.PI / 180f * 45)
        {
            float cameraDistance = GetCameraDistance();
            float scaleFactor = CalculateScaleFactor(cameraDistance);

            radius *= scaleFactor; // Adjust radius based on camera distance
            gapRads *= scaleFactor; // Adjust gapRads based on camera distance
            brush.Thickness *= scaleFactor;

            float segmentAngle = MathF.Tau / 4; // 360 degrees / 4 = 90 degrees per segment
            float radiansPerDegree = MathF.PI / 180f;
            float rotationRads = (currentRotationAngle + rotationOffset) * radiansPerDegree;

            var shape = new ConvexShape(brush);

            for (int i = 0; i < 4; i++)
            {
                float startRads = i * segmentAngle + rotationRads;
                float endRads = startRads + segmentAngle - gapRads; // Subtract the adjusted gapRads
                shape.Arc(gamePos, radius, startRads, endRads);
                shape.Done(); // Ensure each segment is finalized before drawing the next
            }

            // Increment rotation angle by 1 degree for the next frame
            currentRotationAngle -= 1f;
            if (currentRotationAngle < 0f)
            {
                currentRotationAngle += 360f;
            }
            if (currentRotationAngle >= 360f)
            {
                currentRotationAngle -= 360f; // Reset angle to keep it within 0-360 degrees
            }
        }


        public static float Sigmoid(float value)
        {
            return 1 / (1 + (float)Math.Exp(-value));
        }

        public static float SmoothedSigmoid(float value, float smoothingFactor)
        {
            if (smoothingFactor <= 0 || smoothingFactor > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(smoothingFactor), "Smoothing factor must be between 0 and 1.");
            }

            var sigmoidValue = Sigmoid(value);
            var smoothedValue = (smoothingFactor * sigmoidValue) + ((1 - smoothingFactor) * PreviousSmoothedSigmoidValue);
            PreviousSmoothedSigmoidValue = smoothedValue;
            return smoothedValue;
        }
    }
}
