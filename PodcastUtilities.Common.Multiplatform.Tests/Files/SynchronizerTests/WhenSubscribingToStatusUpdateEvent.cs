﻿#region License
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

namespace PodcastUtilities.Common.Multiplatform.Tests.Files.SynchronizerTests
{
    public class WhenSubscribingToStatusUpdateEvent : WhenTestingTheSynchronizer
    {

        protected override void When()
        {
            PodcastSynchronizer.StatusUpdate += PodcastSynchronizerStatusUpdate;
        }

        static void PodcastSynchronizerStatusUpdate(object sender, StatusUpdateEventArgs e)
        {
        }

        [Test]
        public void ItShouldSubscribeToFileCopierStatusUpdate()
        {
            FileCopier.VerifyAdd(c => c.StatusUpdate += PodcastSynchronizerStatusUpdate, Times.Once);
        }

        [Test]
        public void ItShouldSubscribeToFileRemoverStatusUpdate()
        {
            FileRemover.VerifyAdd(c => c.StatusUpdate += PodcastSynchronizerStatusUpdate, Times.Once);
        }
    }
}