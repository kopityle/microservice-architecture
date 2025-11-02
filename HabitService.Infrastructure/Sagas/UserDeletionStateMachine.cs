using Common.Messages.Commands;
using Common.Messages.Events;
using MassTransit;

public class UserDeletionStateMachine : MassTransitStateMachine<UserDeletionState>
{
    // Описываем состояния, через которые проходит наша операция
    public State DeletingAchievements { get; private set; }
    public State DeletingTrackingData { get; private set; }
    public State DeletingHabits { get; private set; }

    // Описываем события, которые могут произойти
    public Event<DeleteUserRequested> DeletionRequested { get; private set; } // Стартовое событие
    public Event<UserAchievementsDeleted> AchievementsDeleted { get; private set; }
    public Event<UserTrackingDataDeleted> TrackingDataDeleted { get; private set; }
    public Event<UserHabitsDeleted> HabitsDeleted { get; private set; }

    public UserDeletionStateMachine()
    {
        InstanceState(x => x.CurrentState); // Где хранить текущее состояние
        Event(() => DeletionRequested, x => x.CorrelateById(context => context.Message.UserId));
        Event(() => AchievementsDeleted, x => x.CorrelateById(context => context.Message.UserId));
        Event(() => TrackingDataDeleted, x => x.CorrelateById(context => context.Message.UserId));
        Event(() => HabitsDeleted, x => x.CorrelateById(context => context.Message.UserId));
        // --- Описание сценария ---
        Initially(
            When(DeletionRequested)
                .Then(context => context.Saga.UserId = context.Message.UserId)
                .TransitionTo(DeletingAchievements) // Переходим в первое состояние
                                                    // Отправляем первую команду
                .Send(new Uri("queue:delete-achievements-queue"), context => new DeleteUserAchievements(context.Saga.UserId))
        );

        During(DeletingAchievements,
            When(AchievementsDeleted) // Когда получили ответ от AchievementService
                .TransitionTo(DeletingTrackingData) // Переходим в следующее состояние
                                                    // Отправляем вторую команду
                .Send(new Uri("queue:delete-tracking-queue"), context => new DeleteUserTrackingData(context.Saga.UserId))
        );

        During(DeletingTrackingData,
            When(TrackingDataDeleted) // Когда получили ответ от TrackingService
                .TransitionTo(DeletingHabits)
                .Send(new Uri("queue:delete-habits-queue"), context => new DeleteUserHabits(context.Saga.UserId))
        );

        During(DeletingHabits,
            When(HabitsDeleted)
                .Finalize() // Завершаем сагу
        );
    }
}