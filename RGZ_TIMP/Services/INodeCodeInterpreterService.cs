namespace RGZ_TIMP.Services;

public interface INodeCodeInterpreterService
{
    int Evaluate(string code, int outgoingCount);
}