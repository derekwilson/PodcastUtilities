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
using System;
using NUnit.Framework;
using PodcastUtilities.Common;
using PodcastUtilities.Common.Configuration;
using PodcastUtilities.Presentation.ViewModels;
using Rhino.Mocks;

namespace PodcastUtilities.Presentation.Tests.ViewModels.ConfigurePodcastsViewModelTests
{
    public class WhenExecutingAddPodcastCommand
        : WhenTestingConfigurePodcastsViewModel
    {
        public PodcastViewModel CreatedPodcastViewModel { get; set; }

        public PodcastInfo CreatedPodcast { get; set; }
        public IPodcastInfo EditedPodcast { get; set; }

        protected virtual bool EditPodcastDialogReturn
        {
            get { return false; }
        }

        protected override void GivenThat()
        {
            base.GivenThat();

            ViewModel.Podcasts.Add(new PodcastViewModel(new PodcastInfo(ControlFile)));

            CreatedPodcast = new PodcastInfo(ControlFile)
                                 {
                                     Folder = "created",
                                     Feed = new FeedInfo(ControlFile)
                                 };

            PodcastFactory.Stub(f => f.CreatePodcast(null))
                .Return(CreatedPodcast);

            DialogService.Stub(s => s.ShowEditPodcastDialog(null))
                .IgnoreArguments()
                .WhenCalled(invocation =>
                                {
                                    CreatedPodcastViewModel = (PodcastViewModel) invocation.Arguments[0];
                                    EditedPodcast = CreatedPodcastViewModel.Podcast;
                                })
                .Return(EditPodcastDialogReturn);
        }

        protected override void When()
        {
            ViewModel.AddPodcastCommand.Execute(null);
        }

        [Test]
        public void ItShouldCreatePodcastAndShowEditPodcastDialog()
        {
            Assert.That(EditedPodcast, Is.SameAs(CreatedPodcast));
        }
    }
}