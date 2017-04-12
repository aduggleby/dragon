namespace Dragon.CPRX
{
    public interface ICPRProjectionBase<in T>
    {

    }
    public interface ICPRProjection<in T> : ICPRProjectionBase<T>
        where T : CPRCommand
    {
        void Project(ICPRContext ctx, T cmd);
        void Unproject(ICPRContext ctx, T cmd);

    }
}