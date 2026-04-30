namespace Nova.Common.Application.Tools;

public sealed class NovaToolRegistry(
    IEnumerable<INovaTool> tools) : INovaToolRegistry
{
    private readonly List<INovaTool> _tools = tools.ToList();

    public IReadOnlyCollection<INovaTool> GetAll() => _tools;

    public IReadOnlyCollection<ToolDescriptor> GetDescriptors() =>
        _tools
            .Select(t => new ToolDescriptor(
                t.Name, 
                t.Description, 
                t.UsageRules, 
                t.ParametersSchema, 
                t.SafetyLevel))
            .ToArray();
    
    public INovaTool? Find(string name) =>
        _tools.FirstOrDefault(x =>
            x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
}