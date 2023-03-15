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
using System.Collections.Generic;
using PodcastUtilities.Common.Files;
using PodcastUtilities.Common.Platform;

namespace PodcastUtilities.Common.Multiplatform.Tests.Files.SorterTests
{
    public abstract class WhenTestingTheSorter
        : WhenTestingBehaviour
    {
        protected Sorter FileSorter { get; set; }

        protected List<IFileInfo> OriginalFiles { get; set; }

        protected IList<IFileInfo> SortedFiles { get; set; }

        protected override void GivenThat()
        {
            base.GivenThat();

            var file1 = GenerateMock<IFileInfo>();
            file1.Setup(f => f.Name).Returns("aaa");
            file1.Setup(f => f.CreationTime).Returns(new DateTime(2011, 2, 3));

            var file2 = GenerateMock<IFileInfo>();
            file2.Setup(f => f.Name).Returns("zzz");
            file2.Setup(f => f.CreationTime).Returns(new DateTime(1999, 7, 9));

            var file3 = GenerateMock<IFileInfo>();
            file3.Setup(f => f.Name).Returns("mmm");
            file3.Setup(f => f.CreationTime).Returns(new DateTime(2005, 10, 30));

            OriginalFiles = new List<IFileInfo>
                                {
                                    file1.Object,
                                    file2.Object,
                                    file3.Object
                                };

            FileSorter = new Sorter();
        }
    }
}
