namespace LuaInterface.Tests 
{

	using System;
	using LuaInterface;
    using System.Threading;
    using System.Diagnostics;
    using System.Reflection;
	
	/*
	 * Delegates used for testing Lua function -> delegate translation
	 */
	public delegate int TestDelegate1(int a, int b);
	public delegate int TestDelegate2(int a, out int b);
	public delegate void TestDelegate3(int a, ref int b);
	public delegate TestClass TestDelegate4(int a, int b);
	public delegate int TestDelegate5(TestClass a, TestClass b);
	public delegate int TestDelegate6(int a, out TestClass b);
	public delegate void TestDelegate7(int a, ref TestClass b);

	/*
	 * Interface used for testing Lua table -> interface translation
	 */
	public interface ITest 
	{
		int intProp 
		{
			get;
			set;
		}
		TestClass refProp 
		{
			get;
			set;
		}
		int test1(int a,int b);
		int test2(int a, out int b);
		void test3(int a, ref int b);
		TestClass test4(int a, int b);
		int test5(TestClass a, TestClass b);
		int test6(int a, out TestClass b);
		void test7(int a, ref TestClass b);
	}

	public interface IFoo1 
	{
		int foo();
	}

	public interface IFoo2 
	{
		int foo();
	}

    class MyClass
    {
        public int	Func1() { return 1;}
    }

    /// <summary>
    /// Use to test threading
    /// </summary>
    class DoWorkClass
    {
        private object _Lock = new object();

        public void DoWork()
        {
            //lock (_Lock)
            //{
                //simulate work by sleeping
                //Console.WriteLine("Started to do work on thread: " + Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(new Random().Next(0, 1000));
                //Console.WriteLine("Finished work on thread: " + Thread.CurrentThread.ManagedThreadId);
            //}
        }
    }

    /// <summary>
    /// test structure passing
    /// </summary>
    public struct TestStruct
    {
        public TestStruct(float val)
        {
            v = val;
        }

        public float v;

        public float val
        {
            get { return v; }
            set { v = value; }
        }
    }

    /*
	 * Sample class used in several test cases to check if
	 * Lua scripts are accessing objects correctly
	 */
    public class TestClass : IFoo1, IFoo2
    {
        public int val;
        private string strVal;
        public TestClass()
        {
            val = 0;
        }
        public TestClass(int val)
        {
            this.val = val;
        }
        public TestClass(string val)
        {
            this.strVal = val;
        }
        public static TestClass makeFromString(String str)
        {
            return new TestClass(str);
        }

        bool? nb2 = null;

        public bool? NullableBool
        {
            get { return nb2; }
            set { nb2 = value; }
        }


        TestStruct s = new TestStruct();

        public TestStruct Struct
        {
            get { return s; }
            set { s = (TestStruct)value; }
        }

        public int testval
        {
            get
            {
                return this.val;
            }
            set
            {
                this.val = value;
            }
        }
        public int this[int index]
        {
            get { return 1; }
            set { }
        }
        public int this[string index]
        {
            get { return 1; }
            set { }
        }
        public int sum(int x, int y)
        {
            return x + y;
        }
        public void setVal(int newVal)
        {
            val = newVal;
        }
        public void setVal(string newVal)
        {
            strVal = newVal;
        }
        public int getVal()
        {
            return val;
        }
        public string getStrVal()
        {
            return strVal;
        }
        public int outVal(out int val)
        {
            val = 5;
            return 3;
        }
        public int outVal(out int val, int val2)
        {
            val = 5;
            return val2;
        }
        public int outVal(int val, ref int val2)
        {
            val2 = val + val2;
            return val;
        }
        public int callDelegate1(TestDelegate1 del)
        {
            return del(2, 3);
        }
        public int callDelegate2(TestDelegate2 del)
        {
            int a = 3;
            int b = del(2, out a);
            return a + b;
        }
        public int callDelegate3(TestDelegate3 del)
        {
            int a = 3;
            del(2, ref a);
            //Console.WriteLine(a);
            return a;
        }
        public int callDelegate4(TestDelegate4 del)
        {
            return del(2, 3).testval;
        }
        public int callDelegate5(TestDelegate5 del)
        {
            return del(new TestClass(2), new TestClass(3));
        }
        public int callDelegate6(TestDelegate6 del)
        {
            TestClass test = new TestClass();
            int a = del(2, out test);
            return a + test.testval;
        }
        public int callDelegate7(TestDelegate7 del)
        {
            TestClass test = new TestClass(3);
            del(2, ref test);
            return test.testval;
        }
        public int callInterface1(ITest itest)
        {
            return itest.test1(2, 3);
        }
        public int callInterface2(ITest itest)
        {
            int a = 3;
            int b = itest.test2(2, out a);
            return a + b;
        }
        public int callInterface3(ITest itest)
        {
            int a = 3;
            itest.test3(2, ref a);
            //Console.WriteLine(a);
            return a;
        }
        public int callInterface4(ITest itest)
        {
            return itest.test4(2, 3).testval;
        }
        public int callInterface5(ITest itest)
        {
            return itest.test5(new TestClass(2), new TestClass(3));
        }
        public int callInterface6(ITest itest)
        {
            TestClass test = new TestClass();
            int a = itest.test6(2, out test);
            return a + test.testval;
        }
        public int callInterface7(ITest itest)
        {
            TestClass test = new TestClass(3);
            itest.test7(2, ref test);
            return test.testval;
        }
        public int callInterface8(ITest itest)
        {
            itest.intProp = 3;
            return itest.intProp;
        }
        public int callInterface9(ITest itest)
        {
            itest.refProp = new TestClass(3);
            return itest.refProp.testval;
        }
        public void exceptionMethod()
        {
            throw new Exception("exception test");
        }
        public virtual int overridableMethod(int x, int y)
        {
            return x + y;
        }
        public static int callOverridable(TestClass test, int x, int y)
        {
            return test.overridableMethod(x, y);
        }
        int IFoo1.foo()
        {
            return 3;
        }
        public int foo()
        {
            return 5;
        }

        private void _PrivateMethod()
        {
            Console.WriteLine("Private method called");
        }

        public void MethodOverload()
        {
            Console.WriteLine("Method with no params");
        }


        public void MethodOverload(TestClass testClass)
        {
            Console.WriteLine("Method with testclass param");
        }

        public void MethodOverload(int i, int j, int k)
        {
            Console.WriteLine("Overload without out param: " + i + ", " + j + ", " + k);
        }

        public void MethodOverload(int i, int j, out int k)
        {
            k = 5;
            Console.WriteLine("Overload with out param" + i + ", " + j);
        }
    }
}