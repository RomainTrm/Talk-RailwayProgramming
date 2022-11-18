// ReSharper disable SuggestVarOrType_BuiltInTypes
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_Elsewhere
namespace TalkRailwayProgramming.Railway;
#nullable enable

public class MaitreD
{
    private const int MaxCapacity = 10;
    private readonly IRepository _repository;
    public MaitreD(IRepository repository) => _repository = repository;

    public async Task<Result<Unit, Errors>> RegisterReservation(RegisterReservationCommand command)
    {
        // IO
        IReadOnlyCollection<Reservation> reservations = await _repository.ReadReservations(command.At);

        // Pure
        Result<Reservation, Errors> reservationResult = 
            CreateReservation(command.At, command.Email, command.Quantity, command.Name)
            .Bind(reservation => EnsureEnoughSeatsAvailable(reservations, reservation));

        // IO
        return await reservationResult
            .Traverse(async reservation =>
            {
                await _repository.Create(reservation);
                return new Unit();
            });
    }

    private static Result<Reservation, Errors> CreateReservation(DateTime at, string email, int quantity, Option<string> name)
    {
        if (string.IsNullOrWhiteSpace(email))
            return new Error<Reservation, Errors>(Errors.InvalidEmail);
        if (quantity < 1)
            return new Error<Reservation, Errors>(Errors.InvalidQuantity);

        string nameValue = name.Match(
            some => some,
            () => string.Empty);
        
        Reservation reservation = new Reservation(at, email, quantity, nameValue);
        return new Ok<Reservation, Errors>(reservation);
    }

    private static Result<Reservation, Errors> EnsureEnoughSeatsAvailable(IReadOnlyCollection<Reservation> reservations, Reservation reservation)
    {
        int reservedSeats = reservations.Sum(r => r.Quantity);
        if (MaxCapacity < reservedSeats + reservation.Quantity)
            return new Error<Reservation, Errors>(Errors.NotEnoughSeats);
        return new Ok<Reservation, Errors>(reservation);
    }
}

public enum Errors { NotEnoughSeats, InvalidQuantity, InvalidEmail }

public sealed record RegisterReservationCommand(DateTime At, string Email, int Quantity, Option<string> Name);
public sealed record Reservation(DateTime At, string Email, int Quantity, string Name);

public interface IRepository
{
    Task<IReadOnlyCollection<Reservation>> ReadReservations(DateTime date);
    Task Create(Reservation reservation);
}