using Domain.Common;

namespace Domain.Unit.Tests;

public class BaseEntityTests
{
    public class SampleEntity : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
    }
    public class SampleDomainEvent : BaseEvent
    {
        public DateTime OccurredOn { get; private set; } = DateTime.UtcNow;
    }
    
    [Fact]
    public void AddDomainEvent_WithValidEvent_AddsEventToList()
    {
        // Arrange
        var entity = new SampleEntity();
        var domainEvent = new SampleDomainEvent();

        // Act
        entity.AddDomainEvent(domainEvent);

        // Assert
        Assert.Contains(domainEvent, entity.DomainEvents);
    }
    [Fact]
    public void RemoveDomainEvent_WithExistingEvent_RemovesEventFromList()
    {
        // Arrange
        var entity = new SampleEntity();
        var domainEvent = new SampleDomainEvent();
        entity.AddDomainEvent(domainEvent);
        // Act
        entity.RemoveDomainEvent(domainEvent);
        // Assert
        Assert.DoesNotContain(domainEvent, entity.DomainEvents);
    }
    [Fact]
    public void ClearDomainEvents_WithEvents_ClearsAllEvents()
    {
        // Arrange
        var entity = new SampleEntity();
        var domainEvent1 = new SampleDomainEvent();
        var domainEvent2 = new SampleDomainEvent();
        entity.AddDomainEvent(domainEvent1);
        entity.AddDomainEvent(domainEvent2);
        // Act
        entity.ClearDomainEvents();
        // Assert
        Assert.Empty(entity.DomainEvents);
    }

}
