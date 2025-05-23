using Pine.Formatters;

namespace Pine.Targets;

public class JsonFileTarget(string filePath) : FileTarget(filePath, new JsonFormatter());