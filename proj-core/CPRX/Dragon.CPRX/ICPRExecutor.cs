namespace Dragon.CPRX
{
    public interface ICPRExecutor 
    {
        CPRExecutionResult Execute<T>(T cmd, bool ensureCommandsHaveSecurityValidators = true) where T : CPRCommand<T>;
    }
}