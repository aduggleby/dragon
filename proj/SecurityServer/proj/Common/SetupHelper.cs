namespace Dragon.SecurityServer.Common
{
    public class SetupHelper
    {
        public static bool IsSetupAllowed()
        {
            #if DEBUG
            return true;
            #else
            return false;
            #endif
        }
    }
}
