using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class KATVibrator
{
    //������ģ�飺����ִ����ָ��
    //��haptic_level���𶯵ȼ���0-5
    //��haptic_time���𶯳���ʱ����0-3000����λ��100ms��
    //����ֵ��
    //True��ָ��ͳɹ�
    //False��δ�����ɹ���ָ���ʧ��
    [DllImport("WalkerBase", CallingConvention = CallingConvention.Cdecl)]
    public extern static bool Haptic_Module_Control(int haptic_level, int haptic_time);
}
