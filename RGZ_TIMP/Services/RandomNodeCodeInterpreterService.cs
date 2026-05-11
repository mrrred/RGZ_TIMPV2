namespace RGZ_TIMP.Services;

public sealed class RandomNodeCodeInterpreterService : INodeCodeInterpreterService
{
    private readonly Random _random = new();
    public int Evaluate(string code, int outgoingCount) => _random.Next(1, outgoingCount + 1);
}