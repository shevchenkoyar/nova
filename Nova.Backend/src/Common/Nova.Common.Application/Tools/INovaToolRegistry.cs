namespace Nova.Common.Application.Tools;

public interface INovaToolRegistry
{
    IReadOnlyCollection<INovaTool> GetAll();

    IReadOnlyCollection<ToolDescriptor> GetDescriptors();
    
    INovaTool? Find(string name);
}