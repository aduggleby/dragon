namespace Dragon.CPRX
{
    public interface ICPRExecutor
    {
        CPRExecutionResult Execute(CPRCommand cmd);
    }
}