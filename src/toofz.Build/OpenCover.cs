//
// This source code is released under the MIT License; see the accompanying license file.
//
// OpenCover is released under the following MIT compatible software licence
// this does not apply to any other software, be that source code, compiled 
// libraries or tools, that OpenCover may rely on or use and that that software 
// will continue to retain whatever licence they were released under.
// 
// OpenCover Licence
// ================= 
// 
// Copyright (c) 2011-2012 Shaun Wilde
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace toofz.Build
{
    /// <summary>Executes the OpenCover tool with the specified arguments.</summary>
    public sealed class OpenCover : ToolTask
    {
        #region Target

        /// <summary>
        /// The target application.
        /// </summary>
        [Required]
        public string Target { get; set; }

        /// <summary>
        /// Indicates whether target is a service rather than a regular executable.
        /// </summary>
        public bool Service { get; set; }

        /// <summary>
        /// Arguments to be passed to the target process.
        /// </summary>
        public string TargetArgs { get; set; }

        /// <summary>
        /// The working directory for the target process.
        /// </summary>
        public string TargetWorkingDir { get; set; }

        /// <summary>
        /// Return the target process return code instead of the OpenCover console return code.
        /// </summary>
        public bool ReturnTargetCode { get; set; }

        /// <summary>
        /// Use the offset to return the OpenCover console at a value outside the range returned by the target process.
        /// Valid only if ReturnTargetCode is set.
        /// </summary>
        public int TargetCodeOffset { get; set; }

        #endregion

        #region Filtering

        /// <summary>
        /// Exclude a class or method by filters that match attributes.
        /// </summary>
        public ITaskItem[] ExcludeByAttribute { get; set; }

        /// <summary>
        /// Exclude a class or method by filters that match filenames.
        /// </summary>
        public ITaskItem[] ExcludeByFile { get; set; }

        /// <summary>
        /// Indicates whether default filters should be applied or not.
        /// </summary>
        public bool DefaultFilters { get; set; } = true;

        /// <summary>
        /// A list of filters to apply.
        /// </summary>
        public ITaskItem[] Filter { get; set; }

        #endregion

        /// <summary>
        /// Gather coverage by test.
        /// </summary>
        public ITaskItem[] CoverByTest { get; set; }

        /// <summary>
        /// The location and name of the output XML file.
        /// </summary>
        public string Output { get; set; }

        /// <summary>
        /// Merge the result by assembly file-hash.
        /// </summary>
        public bool MergeByHash { get; set; }

        /// <summary>
        /// Indicates whether the code coverage profiler should be registered or not.
        /// </summary>
        public bool Register { get; set; } = true;

        /// <summary>
        /// Indicates whether the list of unvisited methods and classes should be shown.
        /// </summary>
        public bool ShowUnvisited { get; set; }

        /// <summary>
        /// Use old style instrumentation.
        /// The instrumentation is not Silverlight friendly and is provided to support environments where mscorlib 
        /// instrumentation is not working. ONLY use this option if you are encountering MissingMethodException like 
        /// errors when the code is run under OpenCover. The issue could be down to ngen /Profile of the mscorlib which 
        /// then interferes with the instrumentation.
        /// </summary>
        public bool OldStyle { get; set; }

        /// <summary>
        /// The code coverage results XML file.
        /// </summary>
        [Output]
        public ITaskItem Results { get; private set; }

        /// <summary>
        /// Gets the name of the OpenCover tool executable.
        /// </summary>
        protected override string ToolName => "OpenCover.Console.exe";

        /// <summary>
        /// Returns the  path to the OpenCover tool.
        /// </summary>
        /// <returns>The full path to the OpenCover tool.</returns>
        protected override string GenerateFullPathToTool()
        {
            var path = ToolPath;
            var exe = Path.GetFileName(ToolExe);

            return Path.GetFullPath(Path.Combine(path, exe));
        }

        /// <summary>
        /// Generates the command line arguments for the OpenCover tool.
        /// </summary>
        /// <returns>The command line arguments for the OpenCover tool.</returns>
        protected override string GenerateCommandLineCommands()
        {
            CommandLineBuilder builder = new CommandLineBuilder();

            if (Service)
                builder.AppendSwitch("-service");
            if (Register)
                builder.AppendSwitch("-register:user");
            if (!DefaultFilters)
                builder.AppendSwitch("-nodefaultfilters");
            if (MergeByHash)
                builder.AppendSwitch("-mergebyhash");
            if (ShowUnvisited)
                builder.AppendSwitch("-showunvisited");
            if (ReturnTargetCode)
            {
                builder.AppendSwitch("-returntargetcode" + (TargetCodeOffset != 0 ? string.Format(":{0}", TargetCodeOffset) : null));
            }
            if (OldStyle)
                builder.AppendSwitch("-oldstyle");

            builder.AppendSwitchIfNotNull("-target:", Target);
            builder.AppendSwitchIfNotNull("-targetdir:", TargetWorkingDir);
            builder.AppendSwitchIfNotNull("-targetargs:", TargetArgs);

            if ((Filter != null) && (Filter.Length > 0))
                builder.AppendSwitchIfNotNull("-filter:", string.Join<ITaskItem>(" ", Filter));

            if ((ExcludeByAttribute != null) && (ExcludeByAttribute.Length > 0))
                builder.AppendSwitchIfNotNull("-excludebyattribute:", string.Join<ITaskItem>(";", ExcludeByAttribute));

            if ((ExcludeByFile != null) && (ExcludeByFile.Length > 0))
                builder.AppendSwitchIfNotNull("-excludebyfile:", string.Join<ITaskItem>(";", ExcludeByFile));

            if ((CoverByTest != null) && (CoverByTest.Length > 0))
                builder.AppendSwitchIfNotNull("-coverbytest:", string.Join<ITaskItem>(";", CoverByTest));

            builder.AppendSwitchIfNotNull("-output:", Output);

            return builder.ToString();
        }

        /// <summary>
        /// Gets the working directory for the OpenCover tool.
        /// </summary>
        /// <returns>The working directory for the OpenCover tool.</returns>
        protected override string GetWorkingDirectory()
        {
            string ret = null;
            if (TargetWorkingDir != null)
                ret = Path.GetFullPath(TargetWorkingDir);

            if (string.IsNullOrEmpty(ret))
                ret = base.GetWorkingDirectory();

            return ret;
        }

        /// <summary>
        /// Logs the OpenCover output.
        /// </summary>
        /// <param name="singleLine">A single line output by the OpenCover tool.</param>
        /// <param name="messageImportance">The importance of the message.</param>
        protected override void LogEventsFromTextOutput(string singleLine, MessageImportance messageImportance)
        {
            base.LogEventsFromTextOutput(singleLine, MessageImportance.Normal);
        }

        /// <summary>
        /// This method invokes the tool with the given parameters.
        /// </summary>
        /// <returns>true, if task executes successfully</returns>
        public override bool Execute()
        {
            var success = base.Execute();

            // TODO: Should this check success?
            Results = new TaskItem(Output ?? Path.Combine(Directory.GetCurrentDirectory(), "results.xml"));

            return success;
        }
    }
}
