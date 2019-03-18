// ================================================================
// FileName:CameraSmoothFollow.cs
// User: Greg
// CreateTime:2014062810:52
// Description:用来做45度角的摄像机跟随脚本
// Copyright (c) 2014 Greg.Co.Ltd. All rights reserved.
// ================================================================

using UnityEngine;

public class CameraSmoothFollow : MonoBehaviour
{

    /*
        This camera smoothes out rotation around the y-axis and height.
        Horizontal Distance to the target is always fixed.

        There are many different ways to smooth the rotation but doing it this way gives you a lot of control over how the camera behaves.

        For every of those smoothed values we calculate the wanted value and the current value.
        Then we smooth it using the Lerp function.
        Then we apply the smoothed values to the transform's position.
    */

    // 摄像机跟随的目标
    public Transform target;

    // 摄像机和 x - z 平面的角度
    public float distance = 10.0f;

    /// <summary>
    /// 楼上的偏移
    /// </summary>
    public float distanceOffset = 0;

    // 摄像机距离目标的高度
    public float height = 5.0f;

    /// <summary>
    /// 楼上的男朋友
    /// </summary>
    public float heightOffset = 0;

    // How much we 
    public float heightDamping = 2.0f;
    public float rotationDamping = 3.0f;


    // 摄像机根据具体地形的一个旋转角度值
    public float myRotation = 0.0f;


    /// <summary>
    /// 中心视角偏移值 x 轴
    /// 这个值会影响到目标在摄像机中的位置，默认位置是在摄像机中间
    /// </summary>
    public float centerOffsetX = 0;

    /// <summary>
    ///  透视Y轴的偏移值
    /// </summary>
    public float centerOffsetY = 0;

    /// <summary>
    /// 以下都是计算时的辅助var
    /// </summary>
    float wantedRotationAngle;
    float wantedHeight;
    float wantedX;
    float wantedZ;
    float currentRotationAngle;
    float currentHeight;
    float currentX;
    float currentZ;

    public float targetY;

    Quaternion currentRotation;

    public bool isLockCamera = true;
    // Place the script in the Camera-Control group in the component menu

    public bool isLockCameraPostion;

    public bool isEnable = true;

    /// <summary>
    /// 初始化函数
    /// </summary>
    public void Init()
    {
        ////偏移初始化
        ////H = bx/c
        ////c = (a^2+b^2)^1/2
        //var c = Mathf.Pow((Mathf.Pow(height, 2) + Mathf.Pow(distance, 2)), 0.5f);
        //heightOffset = (height * HKCommondSetting.CameraDistanceOffset) / c;
        //distanceOffset = (distance * HKCommondSetting.CameraDistanceOffset) / c;
        //if (distanceOffset < 0)
        //{
        //    if (height + heightOffset < 2)
        //    {
        //        heightOffset = -(height);
        //    }

        //    if (distance + distanceOffset < 2)
        //    {
        //        distanceOffset = -(distance - 2);
        //    }
        //}
    }


    void LateUpdate()
    {
        if (!isEnable)
        {
            return;
        }

        // Early out if we don't have a target
        if (!target)
            return;

        // Calculate the current rotation angles
        wantedRotationAngle = target.eulerAngles.y;
        wantedHeight = target.position.y + (height + heightOffset);


        currentRotationAngle = transform.eulerAngles.y;
        currentHeight = transform.position.y;

        // Damp the rotation around the y-axis
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

        // Damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        // Convert the angle into a rotation
        currentRotation = Quaternion.Euler(0, myRotation, 0);
        if (isLockCamera)
        {
            currentRotation = Quaternion.Euler(0, myRotation, 0);
        }
        else
        {
            currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
        }

        if (!isLockCameraPostion)
        {
            currentX = transform.position.x;
            currentZ = transform.position.z;
            // Set the position of the camera on the x-z plane to:
            // distance meters behind the target
            transform.position = target.position;
            transform.position -= currentRotation * Vector3.forward * (distance + distanceOffset);
            // Set the height of the camera
            wantedX = transform.position.x + centerOffsetX;
            wantedZ = transform.position.z + centerOffsetY;

            currentX = Mathf.Lerp(currentX, wantedX, heightDamping * Time.deltaTime);
            currentZ = Mathf.Lerp(currentZ, wantedZ, heightDamping * Time.deltaTime);

            transform.position = new Vector3(currentX, currentHeight, currentZ);

            // Always look at the target
            Vector3 targetPostion = transform.position + currentRotation * Vector3.forward * (distance + distanceOffset);

            transform.LookAt(new Vector3(targetPostion.x, targetPostion.y - (height + heightOffset), targetPostion.z));

        }
        else
        {
            transform.LookAt(target);
        }

        //加上偏移，实现震屏效果。
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
