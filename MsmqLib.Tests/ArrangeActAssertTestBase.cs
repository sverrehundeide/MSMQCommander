using System;
using NUnit.Framework;

namespace MsmqLib.Tests
{
    /// <summary>
    /// Base class for tests which will use state based assertions.
    /// This class is inteneded for  BDD style tests which have one class
    /// per test fixture and one test method per assertion.
    /// </summary>
    [TestFixture]
    public abstract class ArrangeActAssertTestBase
    {
        /// <summary>
        /// Initialize is executed once only. This enables us to have multiple
        /// assertion methods without having the overhead of executing the Act part
        /// for each assertion.
        /// </summary>
        [TestFixtureSetUp]
        public void Initialize()
        {
            Arrange();
            Act();

            if (PrepareResultForAsserts != null)
                Result = PrepareResultForAsserts();
        }

        [TestFixtureTearDown]
        public virtual void Cleanup()
        {
        }

        protected virtual void Arrange()
        {
        }

        protected abstract void Act();

        /// <summary>
        /// Stores the result for one test fixture.The purpose of this property is to 
        /// reuse the result between multiple assert methods. This is useful for integration
        /// tests where it's expensive to query the result (e.g. query databases)
        /// </summary>
        protected object Result { get; set; }

        /// <summary>
        /// Set this method if you want to reuse the Result object for 
        /// multiple assertion methods in situations where the result is not returned from the method
        /// executed in Act. 
        /// This will improve the performance for integration tests when you have multiple assertion methods
        /// for one test class / fixture.
        /// </summary>
        /// <returns></returns>
        protected virtual Func<object> PrepareResultForAsserts { private get; set; }

        /// <summary>
        /// Get the result. Call this method from assertion methods.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T GetResult<T>()
        {
            return (T) Result;
        }
    }
}