using System.Activities;
using System.Collections.Generic;
using ParseDotCover;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var wf = new ParseDotCoverXmlFileActivity();

            var inputs = new Dictionary<string, object> { { "FilePath", @"C:\Builds\2\SilverTurtle\SilverTurtle\DotCoverReport\Coverage.xml" } };

            WorkflowInvoker.Invoke(wf, inputs);

        }
    }
}
