using BookCatalog.API.Services.interfaces;
using EventBus.Messages.Common;
using EventBus.Messages.Events;
using MassTransit;

namespace BookCatalog.API.Events;

public class OrderStatusConfirmedEventConsumer : IConsumer<OrderStatusConfirmedEvent> {
    private readonly IBookCatalogService _bookCatalogService;
    private readonly ILogger<OrderStatusConfirmedEventConsumer> _logger;

    public OrderStatusConfirmedEventConsumer (IBookCatalogService bookCatalogService,
        ILogger<OrderStatusConfirmedEventConsumer> logger) {
        _bookCatalogService = bookCatalogService;
        _logger = logger;
    }
    public async Task Consume (ConsumeContext<OrderStatusConfirmedEvent> context) {
        var orderStatusConfirmedEvent = context.Message;
        if (orderStatusConfirmedEvent.Metadata.TryGetValue (EventBusConstants._correlationIdHeader, out string correlationId)) {
            _logger.LogInformation("Consuming OrderStatusConfirmedEvent for correlationId : {id}", correlationId);
        }
        _logger.LogInformation ("fetching books to update from database");

        var books = await _bookCatalogService
            .GetBookByIdsAsync (orderStatusConfirmedEvent
                .BookItems.Select (_ => new Guid (_.BookId)));

        _logger.LogInformation ("Fetched books of count: {count}", books.Count ());

        if (books != null && books.Any ()) {
            foreach (var purchasedBook in orderStatusConfirmedEvent.BookItems) {
                var bookCatalog = books.SingleOrDefault (_ => _.BookId.ToString () == purchasedBook.BookId);
                if (bookCatalog != null) {
                    if (bookCatalog.Quantity >= purchasedBook.Quantity) {
                        _logger.LogInformation ("Reducing book of quantity: {bookquantity} with Id: {id} by {quantity}", bookCatalog.Quantity, bookCatalog.BookId, purchasedBook.Quantity);
                        bookCatalog.Quantity -= purchasedBook.Quantity;
                    } else {
                        _logger.LogInformation ("Setting book of quantity: {bookquantity} with Id: {id} to out of stock", bookCatalog.Quantity, bookCatalog.BookId);
                        bookCatalog.Quantity = 0;
                    }
                }
            }
            await _bookCatalogService.UpdateBookAsync (books);
            _logger.LogInformation ("orderStatusConfirmedEvent consumed successfully. Updated books of count: {count}", books!.Count ());
        }
    }
}