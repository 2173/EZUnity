/* Author:          ezhex1991@outlook.com
 * CreateTime:      2019-11-25 13:36:11
 * Organization:    #ORGANIZATION#
 * Description:     
 */
using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace EZhex1991.EZUnity.XLuaExample
{
    /// <summary>
    /// -----Dispose�����Ǳ�Ҫ��-----
    /// -----Dispose�����Ǳ�Ҫ��-----
    /// -----Dispose�����Ǳ�Ҫ��-----
    /// ͨ����˵������Ӧ��ֻ��Ҫһ��LuaEnv���������LuaEnv�������������Ӧ��һ�£����仰˵��Dispose�󲿷�ʱ����ζ��Ӧ��Ҫ�˳���
    /// ����������ں����LuaEnv���˳�ǰ�Ƿ񱻺����ͷ� - ����Ӧ�õ�һ���֣���Ȼ������Ӧ�õĽ�������ϵͳ����
    /// 
    /// ��������Ҫ�ֶ�Dispose����ֻ��Ҫ�ͷŵ������õ�lua�������ɣ�����˵����鿴�ٷ�FAQ
    /// </summary>
    [LuaCallCSharp]
    public class DisposeTest : LuaManager
    {
        public static Action testAction;
        public static event Action testEvent;

        public static Action unregister;

        private void Start()
        {
            unregister = luaEnv.Global.Get<Action>("Unregister");

            testAction();
            testEvent();
            unregister();

            // C#�������õ�lua function��Ϊnull
            unregister = null;
        }

        private void OnDestroy()
        {
            luaEnv.Dispose();
            print("LuaEnv Disposed");
        }
    }

    public static class DisposeTestConfig
    {
        public static List<Type> CSharpCallLua = new List<Type>()
        {
            typeof(Action),
        };
    }
}
