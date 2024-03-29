# Buying and Selling
Buying and selling has Two major sections, the Search, and the Deal.

## Shopping Time

Shopping Time is (as of Dec 22, 2023) a required product like Time. Unlike Time however, Shopping Time is much more specific in it's usage and purpose. Time goes to pretty much everything. Shopping time is only used for shopping. 

Currently (Dec 22, 2023), Shopping Time is spent in one singular block of size 0.2 when leaving to go shopping. This is fixed and includes everything from stepping out of the house to stepping back in. It's very rough, but good enough for version 1.0.

### Shopping Time Improvement

We can improve shopping time by breaking apart the shopping trip. I can think of 3 parts worth making.

1. Travel Time, this covers leaving home to arriving at the business and back, but not the time in the shop. This is defined by how the territory is organized and layed out as well as possible transportation (not just public but private transportation).
2. Store Organization. This is a time modifier created based on how the shop is organized. This organization would allow for faster or slower shopping (at the cost of other factors like Security and aesthetics). This is a per (unique) product cost.
3. Checkout. This is how long it takes to pay for goods and get out. This is modified by the number of customers there as well as how they are checking out. Again, this would effect other parts of the business like Store organization. This and Store organization may roll into each other for particular kinds or organization. IE a store organized like a pharmacy with all goods behind the counter and the employees getting what the customer requests would be relatively high security, but also a slower and have a cost in labor.

I'd like to add this eventually, but it's on the back burner for now in favor of getting basic shopping time done.

## The Search

Before anything a seller must offer up a Sell Order to the market, if they don't then they cannot normally be reached for a deal. Sell Orders include what product they are offering, their price in AMV, and quantity.

The Buyer must act next by sending out a FindProduct message to the market. The FindProduct message is an incomplete buy order, and includes the product being sought.

The Market, once it recieves a FindProduct Message, begins to look. It should have a list of Actors who sell that product, weighted by the product's price, quantity, and Variety of products. 

If there are no sellers, it returns ProductNotFound, and records that the item was sought out. It adds 1 unit of demand to the item and increases the Products AMV a tiny bit.

If the Market has sellers, it selects one at random and returns ProductFound with the buyer's and seller's id.


``` mermaid
---
title: Market Find Product State Machine
---
stateDiagram-v2
    sellerCheck: Market Checks for Sellers.
    ProductNotFound: Send ProductNotFound, \n raise product's price.
    ProductFound: Send ProductFound, with seller
    Deal: Buyer and Seller Go through Deal
    TooExpensiveReduction: Reduce Seller Weight.
    OotRed: Reduce Seller Weight,\n modify prices slighly.
    OosClose: Drastically reduce weight.
    DealSuccess: Record Price accepted (Offer - Change), \n modify prices appropriately, \n weight modification
    SuccessReduction: Reduce weight appropriately.
    SuccessAddition: Add to weight Appropriately.

    [*] --> sellerCheck : Market Recieves FindProduct Message
    ProductNotFound --> [*]

    sellerCheck --> ProductNotFound: No Sellers in Market
    sellerCheck --> ProductFound: Seller Found, selected at random.
    ProductFound --> Deal
    Deal --> TooExpensiveReduction: Buyer Returned CloseDeal(TooExpensive)
    TooExpensiveReduction --> [*]
    Deal --> OotRed: Seller returned CloseDeal(OutOfTime)
    OotRed --> [*]

    Deal --> OosClose: Seller returned CloseDeal(OutOfStock)
    OosClose --> [*]

    Deal --> DealSuccess: Seller accepted offer, sends purchase, and any change
    DealSuccess --> SuccessReduction: Price Response was Expensive or overpriced.
    SuccessReduction --> [*]

    DealSuccess --> SuccessAddition: Price Responce was Reasonable, Cheap, or Steal
    SuccessAddition --> [*]
```

## Emergency Find

Additionally, in an emergency scenario, such as being totally out of a life good, a pop may force through a Product Find. This ActorMessage::EmergencyFind message expands the search and lowers the bar to find. Instead of looking for sellers, they look for pops who might have it. Weight is measured by pop wealth rather than price or stock, as wealthy people are more likely to have resources to spare. 

The logic here is simple, it recieves the EmergencyFind message, pulls from all pops randomly weighted by their per capita wealth, then sends that person's info over. The buyer either succeeds and continues on or fails and tries again, going back to the start.

This could be improved further through the information expansion. 

```mermaid
---
title: Emergency Find
---
stateDiagram-v2
    [*] --> SendRandomBack: RecieveEmergencyFind
    SendRandomBack --> PopsEnterDeal
    PopsEnterDeal --> RecordResult
    RecordResult --> [*]: No additional EmergencyFinds
    RecordResult --> SendRandomBack: Additional EmergencyFind Sent
```

## Deals In Action

Should the Market send out a ProductFound Message, the buyer and seller mentioned will eventually read it and enter their 'deal' state. 

During the Deal State, both buyer and seller are focused on the deal above everything else. The will not accept any other deal requests so they can insure consistent stock in each deal.

Once they enter we can assume they are in step with each other for the most part. They don't necissarily have the same info, but will share readily and info should remain mostly consistent between them. Both can recieve items, but not lose them. Whether they can use those new times is not currently known, but unlikely.

If the Seller is out of stock the seller Sends a CloseDeal{OutOfStock} message and the buyer doesn't wait, and just drops out of the deal. The Market Recieves this message and reduces the weight of the seller for that item as they are out of stock.

If the seller has stock, then the buyer looks at the price and stock, they check the price per unit vs their price per unit budget. If the item is too expensive (x1.5 their budget value) then they'll reject, Sending a CloseDeal{TooExpensive} then leaving. The seller recieves the response back and records it. The Market also recieves the response, and reduces the Sellers weight. Typically, if the Buyer has enough time, they'll try to buy the same item again, going back to the Search Phase.

If the unit price is less than x1.5 the unit price then they'll buy it, but make their opinion known.

Knowing they'll try to buy what they can, they'll select which method of purchase they'll attempt. They can attempt to pay Cash, if the market has currency, Barter, if the target can barter (only pops can), or if all else fails, they'll attempt an AMV Overload.

# The Deal

The Deal is what we call the period in which an exchange is being made. The system doesn't necissarily allow for haggling in the sense of a back and forth with both sides giving and taking. The seller sets the price and the buyer can send goods to try and buy them. They don't necissarily get better chances, just more information and a chance to try again.

Typically, the buyer and seller step in, knowing the item and price, but not it's quantity. The Seller, once they step in, send their stock. If out of stock, they'll send that and close immediately.

If the Seller is in stock, they'll send the amount available. The Buyer, while waiting also checks the price against it's budget. Currently, if the price is above 1.5x it's budget per unit it and it has excess item, it will reject and roll again. If it is below that 1.5x threshold, or it lacks the time to try again, it will buy.

<mark>TODO</mark> There should likely be a mechanism to decide between buying despite being too expensive or to skip out and try again. Most likely mechanism is a combination of population despiration, population excess wealth, and remaining time to try again, immediacy of the desire, market volatility, and a comparison of the current price vs the market price.

Before it goes further it checks it's available currency, calculating their estimated value (Current AMV * Salability) and checking that against the total price requested. If they don't have enough they ask for a Barter Hint. If they have enough, they skip past the following step onto just trying to buy.

Giving a Barter hint costs the seller a small amount of time for each response sent past the first. It tries to give equal time to everyone, so calculates it's expendable budget based on the amount being requested divided by their current stock. With that time availabe, they send at least one hint, the item they desire the most, then repeat it, sending the next item they desire most until they either run out of time, or run out of desired items. It starts with the most desired, walks up their desire tiers, not repeating anything. If they have run out of time, they will give one hint, and stop there.

The Buyer will take these hint(s) and if they feel they need more leverage, will also cross reference with current market demand and make additional educated guesses from there. They will begin gathering an exchange offer in a list of goods. It will prioritize cash, then believed barter items, then everything else. It will also give weight to value dense items as they are less likely to be rejected or ignored by the seller.

<mark>TODO</mark> Value Density preference may become another shared market property. Items that are more dense than this value have their value increased proportionally, or at least not reduced, items with less density have their value reduced proportionally.

With the buyer's offer prepared, they send it to the seller. The seller then runs the calculation to see if the trade is worth it.

First it organizes the items being offered by personal desire priority. Those items that are desired and have a slot are put in the front. Those that are desired but don't have a slot to fill are put next. Then everything else, organized by salability.

```mermaid
---
title: Deal State Machine Logic
---
stateDiagram-v2
    [*] --> SellerChecksStock: Buyer/Seller Recieve ProductFound
    SellerChecksStock --> SendOutOfStockMessage: Seller Out of Stock
    SendOutOfStockMessage --> [*]: Deal Closed Out

    SellerChecksStock --> BuyerCheckPrice: Buyer In Stock
    BuyerCheckPrice --> SendTooExpensiveMessage: Unit Price > 1.5x times budget.
    SendTooExpensiveMessage --> [*]: Deal Closed Out

    BuyerCheckPrice --> CalculateBuyOffer: Buyer Accepts price
    CalculateBuyOffer --> SendBuyOfferMessage: Completes Calculation
    SendBuyOfferMessage --> SellerChecksOffer: Offer Message(s) recieved

    SellerChecksOffer --> SendOfferAcceptance: Accepts offer As Is
    SellerChecksOffer --> SendAcceptanceAndChange: Accepts offer with change in deal.
    SellerChecksOffer --> SendRejectionOffer: Rejects Offer, reduction not valid.

    SendOfferAcceptance --> BuyerAcceptsAndCloses
    SendAcceptanceAndChange --> BuyerRecalculatesWithChange

    SendRejectionOffer --> BuyerDealsWithRejection

    BuyerRecalculatesWithChange --> BuyerAcceptsAndCloses: Change is accepted
    BuyerRecalculatesWithChange --> SendRejectChangeAndClose: Change is rejected

    SendRejectChangeAndClose --> [*]: Both Finish out Deal

    BuyerDealsWithRejection --> CalculateBuyOffer: Buyer wishes to try again.
    BuyerDealsWithRejection --> SendConfirmRejection: Accepts Rejection
    SendConfirmRejection --> [*]: Deal Closed Out

    BuyerAcceptsAndCloses --> [*]: Both Finish the deal
```

# Thoughts to take forward and update around

1. Buyers don't have 3 phases of thinking, just 1. Sellers are the ones who deal with the more complex stuff.
2. Everything offered is dealth with in a few ways
    1. Items Both Desired and with an empty slot for the seller get full AMV value.
    2. Items Desired, but no slot to take it get value = (current Salability Value + Full Salability Value) / 2.
    3. Items not desired get their AMV * Salability (min 5%)
3. If what is being asked for from the seller is desired and in a slot, it calculates based around that.
   1. If the items offered for that desire are not desired, they have their AMV reduced by 1/2 again.
   2. If the items offered are desired, but have no slots to accept it, it gets it's AMV from 2.2 (ave. between full and current Salability)
   3. If the item has a slot it's given full AMV, but it's modified by the tier difference between it and what's being asked for.
   4. If the item being requested stretches over multiple tiers, deal with each tier separately, starting from the top.
4. An EmergencyFind message from a pop forces through finding a product. Specifically, it adds in all of the market's pops as well, allowing them to be met then selects from them at random, regardless of whether they say they're selling or not.