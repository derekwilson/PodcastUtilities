#region License
// FreeBSD License
// Copyright (c) 2010 - 2013, Andrew Trevarrow and Derek Wilson
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
// WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
// PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
// ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED
// TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.
#endregion
using Moq;
using NUnit.Framework;

namespace PodcastUtilities.Common.Multiplatform.Tests
{
    public abstract class WhenTestingBehaviour
    {
        /// <summary>
        /// Seal the method so it can not be overriden. We want all context to be
        /// set in the <see cref="GivenThat" /> method.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            GivenThat();

            When();
        }

        /// <summary>
        /// Perform any clean up after the test has completed.
        /// </summary>
        [TearDown]
        public virtual void CleanupAfterTest()
        {
        }

        /// <summary>
        /// Set up the context of the test.
        /// </summary>
        protected virtual void GivenThat()
        {
        }

        /// <summary>
        /// Invoke the action being tested.
        /// </summary>
        protected abstract void When();

        protected Mock<MOCKTYPE> GenerateMock<MOCKTYPE>()
            where MOCKTYPE : class
        {
            return new Mock<MOCKTYPE>(MockBehavior.Loose);
        }

        protected Mock<MOCKTYPE> GenerateStrictMock<MOCKTYPE>()
            where MOCKTYPE : class
        {
            return new Mock<MOCKTYPE>(MockBehavior.Strict);
        }

        protected Mock<MOCKTYPE> GeneratePartialMock<MOCKTYPE>(params object[] args)
            where MOCKTYPE : class
        {
            return new Mock<MOCKTYPE>(MockBehavior.Loose);
        }
    }
}
