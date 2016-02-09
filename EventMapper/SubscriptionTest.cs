using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EventMapper
{
    [TestClass]
    public class SubscriptionTest
    {
        [TestMethod]
        public void SubscribeTest()
        {
            TestClass testClass = new TestClass();
            string received = null;
            
            testClass.Subscribe("testRoute", o => received = o);
            testClass.Trigger();

            Assert.IsNotNull(received);
            Assert.AreEqual("msg", received.ToString());
        }
    }

    public class Subscriptions : Dictionary<string, Subscription>
    {
        public void Add<T>(string route, ref EventHandler<T> handler)
        {
            this[route] = new Subscription<T>(ref handler);
        }
    }

    public class Subscription
    {
        protected List<Action<string>> onMessageList = new List<Action<string>>();
        protected void NotifyData(string data)
        {
            foreach (var onMessage in onMessageList)
            {
                onMessage(data);
            }
        }

        public void Add(Action<string> onMessage)
        {
            this.onMessageList.Add(onMessage);
        }
        public void Remove(Action<string> onMessage)
        {
            this.onMessageList.Remove(onMessage);
        }
    }

    public class Subscription<T> : Subscription
    {
        public Subscription(ref EventHandler<T> eventHandler)
        {
            eventHandler += (s, d) => NotifyData(d.ToString());
        }
    }

    public class TestData
    {
        public string Message { get; set; }
        public override string ToString()
        {
            return Message;
        }
    }
    
    public class TestClass
    {
        Subscriptions handlerData;
        public TestClass()
        {
            this.handlerData = new Subscriptions();
            this.handlerData.Add("testRoute", ref this.testEvent);
        }

        public event EventHandler<TestData> testEvent;

        public void Subscribe(string command, Action<string> onMessage)
        {
            handlerData[command].Add(onMessage);
        }

        public void Unsubscribe(string command, Action<string> onMessage)
        {
            handlerData[command].Remove(onMessage);
        }
        
        public void Trigger()
        {
            this.testEvent.Invoke(this, new TestData { Message = "msg" });
        }
    }
}