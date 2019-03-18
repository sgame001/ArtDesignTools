using System;
using FairyGUI;
using FairyGUI.Utils;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;


public class GButtonAdaptor : CrossBindingAdaptor
{
    public override Type BaseCLRType
    {
        get
        {
            //            return typeof(TestClassBase);//这是你想继承的那个类
            return typeof(GButton);//这是你想继承的那个类
        }
    }

    public override Type AdaptorType
    {
        get
        {
            return typeof(Adaptor);//这是实际的适配器类
        }
    }

    public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
    {
        return new Adaptor(appdomain, instance);//创建一个新的实例
    }

    //实际的适配器类需要继承你想继承的那个类，并且实现CrossBindingAdaptorType接口
    public class Adaptor : GButton, CrossBindingAdaptorType
    {
        ILTypeInstance instance;
        ILRuntime.Runtime.Enviorment.AppDomain appdomain;
        //缓存这个数组来避免调用时的GC Alloc
        object[] param1 = new object[1];

        public Adaptor()
        {

        }

        public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            this.appdomain = appdomain;
            this.instance = instance;
        }

        public ILTypeInstance ILInstance { get { return instance; } }

        AdaptorMethodRecorder constructFromXMLRecorder = new AdaptorMethodRecorder();
        //你需要重写所有你希望在热更脚本里面重写的方法，并且将控制权转到脚本里去
        public override void ConstructFromXML(XML xml)
        {
            if (!constructFromXMLRecorder.mMethodGot)
            {
                //属性的Getter编译后会以get_XXX存在，如果不确定的话可以打开Reflector等反编译软件看一下函数名称
                constructFromXMLRecorder.mMethod = instance.Type.GetMethod("ConstructFromXML", 1);
                param1[0] = xml;
                constructFromXMLRecorder.mMethodGot = true;
            }
            //对于虚函数而言，必须设定一个标识位来确定是否当前已经在调用中，否则如果脚本类中调用base.Value就会造成无限循环，最终导致爆栈
            if (constructFromXMLRecorder.mMethod != null && !constructFromXMLRecorder.isMethodInvoking)
            {
                constructFromXMLRecorder.isMethodInvoking = true;
                appdomain.Invoke(constructFromXMLRecorder.mMethod, instance, param1);
                constructFromXMLRecorder.isMethodInvoking = false;
            }
            else
                base.ConstructFromXML(xml);
        }

        public override string ToString()
        {
            IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
            m = instance.Type.GetVirtualMethod(m);
            if (m == null || m is ILMethod)
            {
                return instance.ToString();
            }
            else
                return instance.Type.FullName;
        }
    }
}