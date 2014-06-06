using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Documents;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Build.Controls;

namespace DotCoverSummaryTFSAddin
{
    public class MySectionFactory : IBuildDetailFactory
    {
        private string _codeCoveragePath;
        private string _dropLocation;

        /// <summary>
        /// This method should make all necessary server calls to aquire the data
        /// needed to display the sections and nodes for this factory.
        /// This method is called on a background thread (no UI work can be done).
        /// </summary>
        /// <param name="build">a reference to the IBuildDetail being displayed</param>
        public void LoadData(IBuildDetail build)
        {
            _dropLocation = build.DropLocation;

            var htmlDocName = string.Format(@"{0}\DotCoverReport\coverage.html", _dropLocation);

            _codeCoveragePath = build.BuildFinished ? htmlDocName : string.Empty;
        }

        /// <summary>
        /// This method should return all IBuildDetailSections for this factory.
        /// </summary>
        /// <param name="view">a reference to IBuildDetailsView</param>
        /// <returns>array of IBuildDetailSections</returns>
        public IBuildDetailSection[] CreateSections(IBuildDetailView view)
        {
            return new IBuildDetailSection[] { new MySection() };
        }

        /// <summary>
        /// This method should return all IBuildDetailNodes for this factory.
        /// </summary>
        /// <param name="view">a reference to IBuildDetailsView</param>
        /// <returns>array of IBuildDetailNodes</returns>
        public IBuildDetailNode[] CreateNodes(IBuildDetailView view)
        {
            var p = new Paragraph();

            if (string.IsNullOrEmpty(_codeCoveragePath))
            {
                p.Inlines.Add(new Italic(new Run("Build Still Running - Code Coverage Report Not Available")));
            }
            else
            {
                var hyperlink = new Hyperlink();
                hyperlink.Inlines.Add("DotCover Report");
                hyperlink.NavigateUri = new Uri(_codeCoveragePath);
                hyperlink.RequestNavigate += HyperlinkRequestNavigate;
                p.Inlines.Add(hyperlink);
            }

            return new IBuildDetailNode[] { 
                new MySectionNode() { 
                    SectionName = "MySection", 
                    Content = p
                }
            };
        }

        static void HyperlinkRequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            var navigateUri = ((Hyperlink)sender).NavigateUri.ToString();
            // if the URI somehow came from an untrusted source, make sure to
            // validate it before calling Process.Start(), e.g. check to see
            // the scheme is HTTP, etc.
            try
            {
                Process.Start(new ProcessStartInfo(navigateUri));

                ((Hyperlink)sender).Inlines.Clear();
                ((Hyperlink)sender).Inlines.Add("DotCover Report");
            }
            catch (Win32Exception)
            {
                ((Hyperlink) sender).Inlines.Clear();
                ((Hyperlink)sender).Inlines.Add("DotCover Report - Report does not exist");
            }
            
            e.Handled = true;

        }

        /// <summary>
        /// Returns the name of the factory (this name should never be localized).
        /// </summary>
        public string Name
        {
            get { return "MySectionFactory"; }
        }
    }

    public class MySection : IBuildDetailSection
    {
        /// <summary>
        /// Returns the header for this section. Usually this is just a localized string.
        /// However, it can also return a Windows Presentation Foundation document paragraph 
        /// that contains any valid elements for display.
        /// </summary>
        public object Header
        {
            get { return "Code Coverage (DotCover)"; }
        }

        /// <summary>
        /// Returns the name of the section (this name should never be localized).
        /// </summary>
        public string Name
        {
            get { return "MySection"; }
        }

        /// <summary>
        /// Returns the relative priority of this section. This value is used to sort 
        /// the sections in ascending order before display. Standard sections have 
        /// values like 100, 200, 300, etc.
        /// </summary>
        public int Priority
        {
            get { return 550; }
        }
    }

    public class MySectionNode : IBuildDetailNode
    {
        public object Content { get; set; }
        public string SectionName { get; set; }
    }
}