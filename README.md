# BookStoreSolution

### Architecture

![Alt text](Architecture_digram.png "Architecture diagram")

### Book Order Process Flow

![Alt text](Payment_flow.png "Architecture diagram")

1) The user makes request to book catalog api to fetch books that have stock.
2) The user adds books to cart by making request to the basket api.
3) Once the user has finshed shopping a checkout request is sent to the basket api. The basket api.
publishes a basketcheckout event to the rabbitMQ event bus. The order api consumes the basketcheckout event received on basketcheckout-queue and creates an order with pending order status.
4) The user makes a payment authorization request on the pending order on the authorize endpoint of the order api.
5) The order api makes rpc call to the purchase token api to authorize order. If the authorize call is unsuccessful the order status is changed to failed and response is returned to the user.
6) If the payment authorization request was successful the order api publishes an orderstatusconfirmed event to the orderstatusconfirmed-queue in the eventbus to be consumed by the bookcatalog api in order to reduce the stock of books purchased in the order.
7) The order api returns an order confirmed response to the user to complete the process.

