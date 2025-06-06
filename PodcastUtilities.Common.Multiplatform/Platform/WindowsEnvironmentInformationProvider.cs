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
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace PodcastUtilities.Common.Platform
{
    /// <summary>
    /// provides information on the application environment
    /// </summary>
    public class WindowsEnvironmentInformationProvider : IEnvironmentInformationProvider
    {
        /// <summary>
        /// gett the directory that the currently executing application was loaded from
        /// </summary>
        /// <returns></returns>
        public IDirectoryInfo GetCurrentApplicationDirectory()
        {
            return new SystemDirectoryInfo(System.IO.Directory.GetParent(Assembly.GetEntryAssembly().Location));
        }

        public static string GetCommonAssembleVersion()
        {
            Assembly me = System.Reflection.Assembly.GetExecutingAssembly();
            AssemblyName name = me.GetName();
            return name.Version.ToString();
        }

        public static List<string> GetEnvironmentRuntimeDisplayInformation()
        {
            List<string> result = new List<string>();
            result.Add($".NET CLR: {Environment.Version.ToString()}");
			result.Add($"PodcastUtilities.Common v{GetCommonAssembleVersion()}");
#if NETSTANDARD
            // RuntimeInformation was only added in .net 4
            result.Add($"{RuntimeInformation.OSDescription}, Framework: {RuntimeInformation.FrameworkDescription}, OS: {RuntimeInformation.OSArchitecture}, Processor: {RuntimeInformation.ProcessArchitecture}");
#endif
			return result;
        }
    }
}