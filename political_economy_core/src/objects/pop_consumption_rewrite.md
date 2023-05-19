# Pop Consumption, Purchase, and Sale Rewrite

This is to go over my thoughts on pop rewriting and to get the ideas out.

So, currently, a pop's market day goes
1. Add time
2. Wait for ActorMsg::StartDay
3. pointless product sift
4. Check and update if we should sell.
5. Go To work (Send over time/property and wait for the job to end the day also.)
6. Free time
   1. Divide up goods between Reserved and Unreserved
   2. If selling, offer Unreserved for sale at yesterday's market price.
   3. Enter Loop while item and property are available for trade.
      1. Catch-up and deal with any Msgs for us
      2. Try to satisfy a desire
         1. Try to satisfy in any way, reserving that item for the desire, and adding satisfaction preemptively to the desire.
         2. If unable to satisfy, go out to attempt to buy whatever satisfies the desire then wrap up until either unable to purchase or desire is satisfied.
   4. Wait for end of day, dealing with any additional messages.
7. Consume Goods
8. Decay Goods
9.  Adapt Plans

## The Change

The Change I'm going to make to this is to push consumption reservations forward and to continue to de-emphasize the plan mechanism. The Plan mechanism's failing is that it's unable to properly distinguish between time/items used now vs consumed later. 

Instead, the idea would go like this

1. Add Time
2. Go through reservation process for consumption.
   1. We go through all existing property we have and organize it into various camps, satisfying desires as best we can, but not consuming just yet (we actually consume/use them at the end of the day).
3. Recieve DayStart Msg.
4. Do Sale Check for the pop and offer up what's being sold for sale.
   1. Logic here still not fully thought out, but disorganized pops and pops in an emergency state (ie unable to satisfy all desires below T10) will offer up stuff for sale. Disorganized Pops do so for general trade purposes. Panicking Pops do so to satisfy more basic desires.
      1. Pops regardless offer everything that is above their tier of Hard Satisfaction (Ie the highest tier we can reach without unsatisfied desires being found.)
5. Do work day processing. How much will be given and recieved from the Firm will depend on the type of firm and work contract.
   1. Work vs Leisure calculations occur here. Pops will attempt to balance the AMV Wage brought in vs the liesure and shopping time lost. 
6. Free time
   1. Loop over dealing with messages (including selling) and shopping to improve our situation. Periodically update what we're offering for sale or for use based on improving satisfaction. Continue until either the pop runs out of excess property (including time) to spend.
7. Consume Goods, actually follow through with consumption and confirm our satisfaction.
8. Decay our goods.
9. Record success, and consolidate to simplify future purchases.
   1. This is based on the old rollover mechanism, but instead of being used for buy priorities, this instead allows products bought in multiple trips to be consolidated into singular purchases and help with buying for multiple days rather.


## Additional Change

Time now has 3 uses. Labor, Liesure, and Shopping.

Time is sold to firms which results in the pop recieving a wage, but nothing else.

Liesure is time spent idly. It's a roughly 1:1 ratio.

Shopping is time spent shopping, it also satisfies liesure, but at a lesser ratio, 2:1. This means that Liesure can be satisfied even with all free time spent on shopping, but people will still prefer to buy shopping time and spend their liesure properly.

Note, Shopping has an additional Firm Variation, ie Professional Buyers, who get more shopping time per unit of time put in, but no liesure.