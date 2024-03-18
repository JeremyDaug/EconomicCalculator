window.SIDEBAR_ITEMS = {"struct":[["Bus","`Bus` is the main interconnect for broadcast messages. It can be used to send broadcast messages, or to connect additional consumers. When the `Bus` is dropped, receivers will continue receiving any outstanding broadcast messages they would have received if the bus were not dropped. After all those messages have been received, any subsequent receive call on a receiver will return a disconnected error."],["BusIntoIter","An owning iterator over messages on a receiver. This iterator will block whenever `next` is called, waiting for a new message, and `None` will be returned when the corresponding bus has been closed."],["BusIter","An iterator over messages on a receiver. This iterator will block whenever `next` is called, waiting for a new message, and `None` will be returned when the corresponding channel has been closed."],["BusReader","A `BusReader` is a single consumer of `Bus` broadcasts. It will see every new value that is passed to `.broadcast()` (or successful calls to `.try_broadcast()`) on the `Bus` that it was created from."]]};