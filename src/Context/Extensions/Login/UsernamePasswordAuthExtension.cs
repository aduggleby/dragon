using Dragon.Common.Util;
using Dragon.Context.Exceptions;

namespace Dragon.Context.Extensions.Login
{
    public static class UsernamePasswordAuthExtension
    {
        private const string SERVICE_ID = "LOCALACCOUNT";

        public static bool TryLoginWithUsernamePassword(
            this DragonContext ctx, 
            string username, 
            string password)
        {
            var success= ctx.UserStore.TryLogin(SERVICE_ID, username, (s)=>HashUtil.VerifyHash(password, s));
            if (success)
            {
                DragonContext.ProfileStore.SetProperty(ctx.CurrentUserID,
                                                               DragonContext.PROFILEKEY_SERVICE,
                                                               SERVICE_ID);
            }
            return success;
        }

        public static void RegisterUsernamePassword(this DragonContext ctx, string username, string password)
        {
            if (ctx.UserStore.HasUserByKey(SERVICE_ID, username))
            {
                throw new UserKeyAlreadyExistsForThisServiceException();
            }
            else
            {
                var hashedSaltedSecret = HashUtil.ComputeHash(password);
                ctx.UserStore.Register(SERVICE_ID, username, hashedSaltedSecret);
            }
        }
        
        public static void ChangePassword(this DragonContext ctx, string username, string password)
        {
            if (!ctx.UserStore.HasUserByKey(SERVICE_ID, username))
            {
                throw new UserKeyDoesNotExistException();
            }
            else
            {
                var hashedSaltedSecret = HashUtil.ComputeHash(password);
                ctx.UserStore.UpdateSecret(SERVICE_ID, username, hashedSaltedSecret);
            }
        }

        public static bool IsLocalAccountUser(this DragonContext ctx)
        {
            return DragonContext.ProfileStore.GetProperty<string>(ctx.CurrentUserID, DragonContext.PROFILEKEY_SERVICE) == SERVICE_ID;
        }

    }
}
