# The Old Way

The current way the idea works, we have 2 divisions for buying logic, the desires we are trying to satisfy, and the prior knowledge of what we bought and our budgets for them. Day 0 we effectively assume failure and add in a set of desires based on what we want to get. We try to buy based off of our knowledge, then consume everything at the end of the day.

This has lead to problems with how things might work out and have made trying to figure out how well it will work difficult.

For how well it works, while it's impossible to say without running it, it immediately runs into trouble with time. It has no good way to balance between shopping time and leisure/consumption. It would have to guess and check blindly, and is unlikely to stabilize in any useful position without some nasty work.

# The New Way

The new way is to alter how things work. Rather than buying everything all at once, then consuming all at once, we instead interweave them. All time remaining after work is then grouped together and put into a pile. Our property is divided between two groups. An 'undesired' group, this group is available possibly for sale but definitely for spending. And, a 'desired' group, based on our prior knowledge and memories. Undesired Items will be put up for sale in most cases, while desired items will only be put up for sale under duress. Time is not included in this process.

**Note:** Currency or high salability items will not under the presumption they will be more useful in purchase than in sale and to keep everyone from throwing their money out to the field every day.

With these two divisions and our unusued time available, we begin walking up our desires. Each desire we run into, we first try to 'consume' from our desired and undesired items, spending time as needed. We do not actually consume, merely shift items from desired or undesired into 'expending'. We record these successful 'consumptions' when we actually consume at the end of the day. Time is not recorded in this consumption.

If we cannot 'consume' currently we instead try to buy items that will satisfy it. For specific items, we go for that item, for wants, we go for things that satisfy that want. In this case, we ask the market to supply anything which satisfies that want, and it returns one which can, selected at semi-random, or nothing if it cannot be satisfied.

If it returns something we can buy, we try to buy the item(s) it offers. We try to buy first how much our memory says we bought yesterday sans whatever we've currently bought today, or how much we want to satisfy at this moment. If we buy enough for the current satisfaction at minimum, we call that a success. If we didn't buy enough for the immediate satisfaction, we try again. Regardless of the result, we record any Time and AMV spent and the amount of items/satisfaction purchased.

**Note:** When buying, the buyer breaks buying logic in 2 based on undesireable items to trade away vs not. All items are treated as being worth their AMV * Salability. When offering items from the Desired box however, we walk down the desires, starting from the least valued to the most valued. IE, give up a hypothetical tier 20 item first, then tier 19, etc.

With purchasing done, we shift the items which will go to satisfying it into our Expending group, then move on to the next desire.

We repeat this until we either run out of time to spend, or run out of items to exchange.

**Note:** If we have a lot of extra time (hours) or extra AMV to spend (greater have 10% or more leftover from our start), the pop will seek additional ways to expend these. Extra time will tend to be shifted into work, extra AMV will tend to be shifted towards buying more shopping time from the market.

We wrap up the day by consuming all of our consumption goods and sorting the results into our desires.

**Note:** During free time we rotate between buying, selling, and 'consuming' in even measure. Selling is always de-prioritized for pops as selling is a crap-shoot for us.

## Summary

```
given property, memory, desires, freetime

let desired, undesired, expending
let originaltime = freetime

for item in property:
    if memory.contains(property):
        shift memory.amount from property to desired
        shift remainder from property to undesired
    else:
        shift property to undesired

let original_amv = desired.sum(amv*quantity) + undesired.sum(amv*quantity);

let step = null
while freetime > 0 && desired > 0 && undesired > 0:
    check for any sale and clear out msg queue.

    let step = desires.walk_up_desires(step);

    if satisfier in desired:
        shift from desired to expending
        include time if needed
        record shift in memory
    if desire unsatisfied && satisfier in undesired:
        shift from undesired to expending
        include time if needed
        record shift in memory
    if still unsatisfied
        while in in stock try to purchase for desire
        record purchase time expenditure
        
        if successful purchase:
            record time and amv expenditure to memory
            record amount purchased to memory
            shift from purchase to expending to meet desire
            shift excess to undesired for future use.
            break purchase loop.
    if originaltime * 0.1 < freetime:
        shift time to work_time
    if original_amv < (desired.sum(amv) + undesired.sum(amv)) && originaltime * 0.1 > freetime
        try to purchase more shopping time
    end loop
```