namespace Zomp.EFCore.Testing;

public record TestRow(int Id, int? Col1, Guid SomeGuid, DateTime Date, byte[] IdBytes);
