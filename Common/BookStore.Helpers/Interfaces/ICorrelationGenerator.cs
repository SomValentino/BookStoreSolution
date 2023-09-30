namespace BookStore.Helpers.Interfaces;

public interface ICorrelationGenerator {
    string Get ();
    void Set (string correlationId);
}