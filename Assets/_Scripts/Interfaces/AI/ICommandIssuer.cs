public interface ICommandIssuer{
    public void IssueAICommand(AIStateMachine aIToIssue, AICommandType aICommandType);
    public void CeaseAICommand(AIStateMachine aIToIssue, AICommandType aICommandType);
    public void CommandInterrupted(AIStateMachine aIToIssue, AICommandType aICommandInterrupted);
}
