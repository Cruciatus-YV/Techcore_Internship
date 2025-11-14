using MassTransit;

namespace Techcore_Internship.WebApi
{
    // РЕАЛИЗАЦИЯ ORDER SAGA ДЛЯ ОРКЕСТРАЦИИ ПРОДАЖИ БИЛЕТОВ

    // === МОДЕЛЬ СОСТОЯНИЯ SAGA ===
    public class OrderState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }  // OrderId - ключ корреляции
        public string CurrentState { get; set; }
        public Guid? ReservationId { get; set; }
        public Guid? PaymentId { get; set; }
        public DateTime? ReservationExpiresAt { get; set; }
        public Guid? PaymentTimeoutTokenId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }

    // === СООБЩЕНИЯ ===
    public record TicketReserved(Guid OrderId, Guid ReservationId, Guid TicketId, int Quantity);
    public record TicketReservationFailed(Guid OrderId, string Reason);
    public record PaymentSucceeded(Guid OrderId, Guid PaymentId);
    public record PaymentFailed(Guid OrderId, Guid PaymentId, string Reason);
    public record PaymentTimeoutExpired(Guid OrderId);
    public record OrderConfirmed(Guid OrderId, Guid ReservationId);
    public record OrderCancelled(Guid OrderId, string Reason);

    // === КОНЕЧНЫЙ АВТОМАТ SAGA ===
    public class OrderSaga : MassTransitStateMachine<OrderState>
    {
        public OrderSaga()
        {
            // === СОСТОЯНИЯ ===
            InstanceState(x => x.CurrentState);

            // === СОБЫТИЯ ===
            Event(() => TicketReservedEvent, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => TicketReservationFailedEvent, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => PaymentSucceededEvent, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => PaymentFailedEvent, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => PaymentTimeoutExpiredEvent, x => x.CorrelateById(context => context.Message.OrderId));

            // === РАСПИСАНИЕ ТАЙМАУТА ===
            Schedule(() => PaymentTimeoutSchedule, instance => instance.PaymentTimeoutTokenId, schedule =>
            {
                schedule.Delay = TimeSpan.FromMinutes(15);
                schedule.Received = r => r.CorrelateById(context => context.Message.OrderId);
            });

            // === НАЧАЛЬНАЯ КОНФИГУРАЦИЯ ===
            Initially(
                // Когда приходит успешная резервация
                When(TicketReservedEvent)
                    .Then(context =>
                    {
                        context.Saga.ReservationId = context.Message.ReservationId;
                        context.Saga.ReservationExpiresAt = DateTime.UtcNow.AddMinutes(15);
                    })
                    // Запланировать таймаут оплаты
                    .Schedule(PaymentTimeoutSchedule, context => new PaymentTimeoutExpired(context.Saga.CorrelationId))
                    .TransitionTo(WaitingForPayment),

                // Когда резервация не удалась
                When(TicketReservationFailedEvent)
                    .Then(context => Console.WriteLine($"Reservation failed: {context.Message.Reason}"))
                    .Publish(context => new OrderCancelled(context.Saga.CorrelationId, context.Message.Reason))
                    .TransitionTo(Cancelled)
                    .Finalize()
            );

            // === ПОВЕДЕНИЕ В СОСТОЯНИИ ОЖИДАНИЯ ОПЛАТЫ ===
            During(WaitingForPayment,
                // УСПЕШНАЯ ОПЛАТА
                When(PaymentSucceededEvent)
                    .Then(context =>
                    {
                        context.Saga.PaymentId = context.Message.PaymentId;
                    })
                    .Unschedule(PaymentTimeoutSchedule)
                    .Publish(context => new OrderConfirmed(context.Saga.CorrelationId, context.Saga.ReservationId.Value))
                    .TransitionTo(Completed)
                    .Finalize(),

                // ТАЙМАУТ ОПЛАТЫ
                When(PaymentTimeoutExpiredEvent)
                    .Publish(context => new OrderCancelled(context.Saga.CorrelationId, "Payment timeout"))
                    .TransitionTo(Cancelled)
                    .Finalize(),

                // ОШИБКА ОПЛАТЫ
                When(PaymentFailedEvent)
                    .Unschedule(PaymentTimeoutSchedule)
                    .Publish(context => new OrderCancelled(context.Saga.CorrelationId, context.Message.Reason))
                    .TransitionTo(Cancelled)
                    .Finalize()
            );

            SetCompletedWhenFinalized();
        }

        // === СОСТОЯНИЯ ===
        public State WaitingForPayment { get; private set; }
        public State Completed { get; private set; }
        public State Cancelled { get; private set; }

        // === СОБЫТИЯ ===
        public Event<TicketReserved> TicketReservedEvent { get; private set; }
        public Event<TicketReservationFailed> TicketReservationFailedEvent { get; private set; }
        public Event<PaymentSucceeded> PaymentSucceededEvent { get; private set; }
        public Event<PaymentFailed> PaymentFailedEvent { get; private set; }
        public Event<PaymentTimeoutExpired> PaymentTimeoutExpiredEvent { get; private set; }

        // === РАСПИСАНИЕ ===
        public Schedule<OrderState, PaymentTimeoutExpired> PaymentTimeoutSchedule { get; private set; }
    }

    // === КОНФИГУРАЦИЯ ===
    // services.AddMassTransit(x => 
    // {
    //     x.AddSagaStateMachine<OrderSaga, OrderState>()
    //         .InMemoryRepository();
    //     
    //     x.UsingRabbitMq((context, cfg) => 
    //     {
    //         cfg.ReceiveEndpoint("order-saga", e => 
    //         {
    //             e.ConfigureSaga<OrderState>(context);
    //         });
    //     });
    // });

    // === ОПИСАНИЕ ПОТОКА ===

    // 1. НАЧАЛО: TicketReserved → Saga создается → WaitingForPayment
    //    - Сохраняет ReservationId
    //    - Планирует таймаут на 15 минут

    // 2. УСПЕХ: PaymentSucceeded → Отмена таймаута → OrderConfirmed → Completed

    // 3. ТАЙМАУТ: PaymentTimeoutExpired → OrderCancelled → Cancelled

    // 4. ОШИБКА: PaymentFailed → Отмена таймаута → OrderCancelled → Cancelled
}
