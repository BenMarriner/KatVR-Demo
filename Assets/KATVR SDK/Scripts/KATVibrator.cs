using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class KATVibrator
{
    //控制震动模块：优先执行新指令
    //【haptic_level】震动等级：0-5
    //【haptic_time】震动持续时长：0-3000【单位：100ms】
    //返回值：
    //True：指令发送成功
    //False：未启动成功，指令发送失败
    [DllImport("WalkerBase", CallingConvention = CallingConvention.Cdecl)]
    public extern static bool Haptic_Module_Control(int haptic_level, int haptic_time);
}
