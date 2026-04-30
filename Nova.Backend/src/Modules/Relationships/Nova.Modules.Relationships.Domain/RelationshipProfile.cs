using Nova.Modules.Relationships.Contracts;

namespace Nova.Modules.Relationships.Domain;

public sealed class RelationshipProfile
{
    private RelationshipProfile()
    {
    }

    public Guid Id { get; private set; }

    public Guid PersonId { get; private set; }

    public int Trust { get; private set; }

    public int Warmth { get; private set; }

    public int Respect { get; private set; }

    public int Familiarity { get; private set; }

    public int Annoyance { get; private set; }

    public int OffenseScore { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public RelationshipAccessLevel AccessLevel => CalculateAccessLevel();

    public static RelationshipProfile Create(Guid personId)
    {
        if (personId == Guid.Empty)
            throw new ArgumentException("PersonId is required.", nameof(personId));

        var now = DateTimeOffset.UtcNow;

        return new RelationshipProfile
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            Trust = 50,
            Warmth = 50,
            Respect = 50,
            Familiarity = 0,
            Annoyance = 0,
            OffenseScore = 0,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void RegisterInteraction(
        RelationshipInteractionKind kind,
        string? reason = null)
    {
        switch (kind)
        {
            case RelationshipInteractionKind.Polite:
                IncreaseRespect(3);
                IncreaseWarmth(2);
                DecreaseAnnoyance(2);
                DecreaseOffense(1);
                break;

            case RelationshipInteractionKind.Helpful:
                IncreaseTrust(2);
                IncreaseWarmth(1);
                break;

            case RelationshipInteractionKind.Apology:
                IncreaseRespect(5);
                IncreaseWarmth(3);
                DecreaseAnnoyance(8);
                DecreaseOffense(5);
                break;

            case RelationshipInteractionKind.Rude:
                DecreaseRespect(8);
                DecreaseWarmth(4);
                IncreaseAnnoyance(10);
                IncreaseOffense(8);
                break;

            case RelationshipInteractionKind.Aggressive:
                DecreaseRespect(15);
                DecreaseWarmth(10);
                DecreaseTrust(5);
                IncreaseAnnoyance(20);
                IncreaseOffense(15);
                break;

            case RelationshipInteractionKind.BoundaryViolation:
                DecreaseRespect(20);
                DecreaseTrust(10);
                IncreaseAnnoyance(25);
                IncreaseOffense(25);
                break;

            case RelationshipInteractionKind.DangerousRequest:
                DecreaseTrust(15);
                IncreaseOffense(15);
                break;

            case RelationshipInteractionKind.Neutral:
            default:
                IncreaseFamiliarity(1);
                break;
        }

        IncreaseFamiliarity(1);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private RelationshipAccessLevel CalculateAccessLevel()
    {
        var score = CalculateRelationshipScore();

        if (score >= 70)
            return RelationshipAccessLevel.Full;

        if (score >= 50)
            return RelationshipAccessLevel.Limited;

        if (score >= 30)
            return RelationshipAccessLevel.BasicOnly;

        if (score >= 15)
            return RelationshipAccessLevel.ReadOnly;

        return RelationshipAccessLevel.Blocked;
    }

    private int CalculateRelationshipScore()
    {
        var positive = (Trust * 35) + (Respect * 35) + (Warmth * 20) + (Familiarity * 10);
        var normalized = positive / 100;

        var penalty = (Annoyance * 40 + OffenseScore * 60) / 100;

        return Clamp(normalized - penalty);
    }

    private void IncreaseTrust(int value) => Trust = Clamp(Trust + value);
    private void DecreaseTrust(int value) => Trust = Clamp(Trust - value);

    private void IncreaseWarmth(int value) => Warmth = Clamp(Warmth + value);
    private void DecreaseWarmth(int value) => Warmth = Clamp(Warmth - value);

    private void IncreaseRespect(int value) => Respect = Clamp(Respect + value);
    private void DecreaseRespect(int value) => Respect = Clamp(Respect - value);

    private void IncreaseFamiliarity(int value) => Familiarity = Clamp(Familiarity + value);

    private void IncreaseAnnoyance(int value) => Annoyance = Clamp(Annoyance + value);
    private void DecreaseAnnoyance(int value) => Annoyance = Clamp(Annoyance - value);

    private void IncreaseOffense(int value) => OffenseScore = Clamp(OffenseScore + value);
    private void DecreaseOffense(int value) => OffenseScore = Clamp(OffenseScore - value);

    private static int Clamp(int value) => Math.Clamp(value, 0, 100);
}