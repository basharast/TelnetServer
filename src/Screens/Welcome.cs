using System;

namespace TelnetServer.Screens
{
    internal class Welcome : ScreenBase
    {
        private const string SCREEN =
            "Welcome to telnet server by Doron Guttman"+"\r\n"+ "Enhanced by Bashar Astifan" + "\r\n";

        #region Overrides of ScreenBase
        public override string GetScreen() => SCREEN;

        public override void ProcessPresentation(string presentation)
        {
            throw new NotImplementedException();
        }
        #endregion Overrides of ScreenBase
    }
}
