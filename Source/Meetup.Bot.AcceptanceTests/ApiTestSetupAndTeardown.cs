using System;
using System.Diagnostics;
using System.IO;
using NUnit.Framework;

namespace Meetup.Bot.AcceptanceTests
{
    [SetUpFixture]
    public class ApiTestSetupAndTeardown
    {
        private static readonly IISHelper IisHelper;

        static ApiTestSetupAndTeardown()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var localPublishedWebsitesPath = Path.Combine(baseDirectory, "_PublishedWebsites", "Meetup.Bot");

            var sitePath = Directory.Exists(localPublishedWebsitesPath)
                ? localPublishedWebsitesPath
                : new DirectoryInfo(Path.Combine(baseDirectory, "..", "..", "..", "Meetup.Bot")).FullName;

            IisHelper = new IISHelper("meetupbot", sitePath, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "applicationhost.config"));
        }

        [OneTimeSetUp]
        public void SetupEnvironment()
        {
            IisHelper.Start();
            AttachDebugger();
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            IisHelper.Stop();
        }

        private static void AttachDebugger()
        {
            if (!Debugger.IsAttached) return;
            // Take this opportunity to attach the debugger to web server
            // Make sure "Attach To" is not in Automatic, use Managed (v4.0, v4.5)

            Debugger.Break();
        }
    }
}
