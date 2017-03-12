namespace Dragon.CPRX
{
    public interface ICPRProjection
    {
        void Project(ICPRContext ctx, CPRCommand cmd);
    }
}