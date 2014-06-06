using System;
using System.Activities;
using System.Linq;
using System.Xml.Linq;
using Microsoft.TeamFoundation.Build.Client;

namespace ParseDotCover
{
    [BuildActivity(HostEnvironmentOption.All)] 
    public sealed class ParseDotCoverXmlFileActivity : CodeActivity<int>
    {
        public InArgument<string> FilePath { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override int Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument
            var filePath = context.GetValue(this.FilePath);

            var ncoverDocument = XDocument.Load(filePath);

            var coveragePercent = (from root in ncoverDocument.Descendants("Root")
                                  select root.Attribute("CoveragePercent").Value).FirstOrDefault();

            return Convert.ToInt32(coveragePercent);
        }
    }
}
