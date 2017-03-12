namespace Dragon.CPRX
{
    public interface ICPRProjection<in T>
        where T : CPRCommand
    {
        void Project(ICPRContext ctx, T cmd);
        void Unproject(ICPRContext ctx, T cmd);

    }
}