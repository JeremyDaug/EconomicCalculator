# Actor Message Info

This is just a document to help organize and mantain how Actor Message is to be used and the states that can be entered by actors for various purposes.

Lock is whether the actor(s) in question are allowed to deal with other messages. Y/N

---
| Message     | Market | Actor | State | Expects | Lock |
|-------------|--------|-------|-------|---------|---|
| StartDay    | Sender | Recievers | Market Day, Start | Finished | N |
| SellOrder  | Reciever | Sender | Market Day, Operation |  | N |
| FindProduct | Reciever | Sender | Market Day, Operation | ProductNotFound / ProductFound | N |
| FindClass | Reciever | Sender | Market Day, Operation | ClassNotFound / FoundClass | N |
| FindWant | Reciever | Sender | Market Day, Operation | FoundWant / WantNotFound |  N |
| ProductNotFound | Sender | Reciever | Market Day, Operation |  | N |
| ClassNotFound | Sender | Reciever | Market Day, Operation |  | N |
| WantNotFound | Sender | Reciever | Market Day, Operation |  | N |
| FoundClass | Sender | Reciever | Market Day, Operation |  | N |
| FoundWant | Sender | Reciever | Market Day, Operation |  | N |
| FoundProduct | Sender | Reciever (buyer / seller) | Market Day -> Buy State | InStock / NotInStock | N |
| InStock |  | Sender and Reciever (Seller to Buyer) | AskBarterHint / RejectPurchase / BuyOffer | Enter Lock |
| NotInStock |  | Sender and Reciever (Seller to Buyer) |  | Leave Lock |
| AskBarterHint | Sender and Reciever (Buyer to Seller) |  |  |  |
| BarterHint |  |  |  |  |
| RejectPurchase |  |  |  |  |
| BuyOffer |  |  |  |  |
| BuyOfferFollowup |  |  |  |  |
| SellerAcceptOfferAsIs |  |  |  |  |
| OfferAcceptedWithChange |  |  |  |  |
| ChangeFollowup |  |  |  |  |
| RejectOffer |  |  |  |  |
| FinishDeal |  |  |  |  |
| CloseDeal |  |  |  |  |
| CheckItem |  |  |  |  |
| SendProduct |  |  |  |  |
| SendWant |  |  |  |  |
| DumpProduct |  |  |  |  |
| WantSplash |  |  |  |  |
| FirmToEmployee |  |  |  |  |
| EmployeeToFirm |  |  |  |  |
| Finished    | Reciever | Senders | Market Day, Actor End | AllFinished |
| AllFinished | Sender | Recievers | Market Day, End |  |

# Message notes

- Start Day
- Finished
- AllFinished |  |  |  |  
- SellOrder  |  |  |  |  |
- FindProduct |  |  |  |  
- FindClass   |  |  |  |  
- FindWant |  |  |  |  |
- ProductNotFound |  |  | 
- ClassNotFound |  |  |  |
- WantNotFound |  |  |  | 
- FoundProduct |  |  |  | 
- FoundClass |  |  |  |  |
- FoundWant |  |  |  |  |
- InStock |  |  |  |  |
- NotInStock |  |  |  |  |
- AskBarterHint |  |  |  |
- BarterHint |  |  |  |  |
- RejectPurchase |  |  |  
- BuyOffer |  |  |  |  |
- BuyOfferFollowup |  |  |
- SellerAcceptOfferAsIs | 
- OfferAcceptedWithChange 
- ChangeFollowup |  |  |  
- RejectOffer |  |  |  |  
- FinishDeal |  |  |  |  |
- CloseDeal |  |  |  |  |
- CheckItem |  |  |  |  |
- SendProduct |  |  |  |  
- SendWant |  |  |  |  |
- DumpProduct |  |  |  |  
- WantSplash |  |  |  |  |
- FirmToEmployee |  |  |  
- EmployeeToFirm |  |  |  