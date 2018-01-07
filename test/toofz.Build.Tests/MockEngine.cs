using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Build.Framework;
using Moq;

namespace toofz.Build.Tests
{
    internal sealed class MockEngine : IBuildEngine5
    {
        public Mock<ILogger> Logger { get; } = new Mock<ILogger>();

        public bool IsRunningMultipleNodes { get; set; }
        public bool ContinueOnError => false;
        public int LineNumberOfTaskNode => 0;
        public int ColumnNumberOfTaskNode => 0;
        public string ProjectFileOfTaskNode => "";

        public void Reacquire() { }

        public void Yield() { }

        #region Building

        public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs, string toolsVersion)
        {
            throw new NotImplementedException();
        }

        public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
        {
            throw new NotImplementedException();
        }

        public BuildEngineResult BuildProjectFilesInParallel(string[] projectFileNames, string[] targetNames, IDictionary[] globalProperties, IList<string>[] removeGlobalProperties, string[] toolsVersion, bool returnTargetOutputs)
        {
            throw new NotImplementedException();
        }

        public bool BuildProjectFilesInParallel(string[] projectFileNames, string[] targetNames, IDictionary[] globalProperties, IDictionary[] targetOutputsPerProject, string[] toolsVersion, bool useResultsCache, bool unloadProjectsOnCompletion)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Logging

        public List<CustomBuildEventArgs> CustomEvents { get; } = new List<CustomBuildEventArgs>();

        public void LogCustomEvent(CustomBuildEventArgs e)
        {
            CustomEvents.Add(e);
        }

        public List<BuildErrorEventArgs> ErrorEvents { get; } = new List<BuildErrorEventArgs>();

        public void LogErrorEvent(BuildErrorEventArgs e)
        {
            ErrorEvents.Add(e);
        }

        public List<BuildMessageEventArgs> MessageEvents { get; } = new List<BuildMessageEventArgs>();

        public void LogMessageEvent(BuildMessageEventArgs e)
        {
            MessageEvents.Add(e);
        }

        public List<BuildWarningEventArgs> WarningEvents { get; } = new List<BuildWarningEventArgs>();

        public void LogWarningEvent(BuildWarningEventArgs e)
        {
            WarningEvents.Add(e);
        }

        public Dictionary<string, IDictionary<string, string>> Telemetry { get; } = new Dictionary<string, IDictionary<string, string>>();

        public void LogTelemetry(string eventName, IDictionary<string, string> properties)
        {
            Telemetry.Add(eventName, properties);
        }

        #endregion

        #region Task Registration				

        public object GetRegisteredTaskObject(object key, RegisteredTaskObjectLifetime lifetime)
        {
            throw new NotImplementedException();
        }

        public void RegisterTaskObject(object key, object obj, RegisteredTaskObjectLifetime lifetime, bool allowEarlyCollection)
        {
            throw new NotImplementedException();
        }

        public object UnregisterTaskObject(object key, RegisteredTaskObjectLifetime lifetime)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
