using BookStore.Helpers.Interfaces;

namespace BookStore.Helpers.Features;

public class CorrelationIdGenerator : ICorrelationGenerator {
    private string _correlationId = Guid.NewGuid ().ToString ();

    public string Get () => _correlationId;

    public void Set (string correlationId) {
        _correlationId = correlationId;
    }
}