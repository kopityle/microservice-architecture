using MassTransit;

public class UserDeletionState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; } // ID самой операции
    public string CurrentState { get; set; }
    public Guid UserId { get; set; } // ID удаляемого пользователя
}