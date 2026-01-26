
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using GameLogic;
using System;
using GameFramework.WebRequest;
// 平滑器接口
public interface IInputSmoother<T> where T : struct
{
    T GetSmoothedValue(T rawValue, float deltaTime);
    void Reset();
}

// float 类型的平滑器
public class FloatSmoother : IInputSmoother<float>
{
    public float sensitivity = 3f;
    public float gravity = 3f;
    public float deadZone = 0.001f;
    public bool snap = false;

    private float currentValue = 0f;

    public float GetSmoothedValue(float rawValue, float deltaTime)
    {
        if (Mathf.Abs(rawValue) <= deadZone)
            rawValue = 0f;

        if (rawValue != 0f)
        {
            if (snap && Mathf.Sign(rawValue) != Mathf.Sign(currentValue))
            {
                currentValue = 0f;
            }

            var delta = rawValue - currentValue;
            currentValue += delta * sensitivity * deltaTime;
        }
        else
        {
            if (currentValue > deadZone)
            {
                currentValue -= gravity * deltaTime;
                if (currentValue < 0f) currentValue = 0f;
            }
            else if (currentValue < -deadZone)
            {
                currentValue += gravity * deltaTime;
                if (currentValue > 0f) currentValue = 0f;
            }
            else
            {
                currentValue = 0f;
            }
        }

        return Mathf.Clamp(currentValue, -1f, 1f);
    }

    public void Reset() => currentValue = 0f;
}

// Vector2 类型的平滑器
public class Vector2Smoother : IInputSmoother<Vector2>
{
    private readonly FloatSmoother xSmoother = new();
    private readonly FloatSmoother ySmoother = new();

    public Vector2Smoother(float sensitivity = 3f, float gravity = 3f, float deadZone = 0.001f, bool snap = false)
    {
        xSmoother.sensitivity = ySmoother.sensitivity = sensitivity;
        xSmoother.gravity = ySmoother.gravity = gravity;
        xSmoother.deadZone = ySmoother.deadZone = deadZone;
        xSmoother.snap = ySmoother.snap = snap;
    }

    public Vector2 GetSmoothedValue(Vector2 rawValue, float deltaTime)
    {
        return new Vector2(
            xSmoother.GetSmoothedValue(rawValue.x, deltaTime),
            ySmoother.GetSmoothedValue(rawValue.y, deltaTime)
        );
    }

    public void Reset()
    {
        xSmoother.Reset();
        ySmoother.Reset();
    }
}

