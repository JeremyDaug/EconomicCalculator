# The Problem

Currently, products are specific items and pretty much nothing else. We assume that time cannot be bought or sold, except through special circumstances. Firms directly gain time instead of directly trading. This is "OK", but that's it. It's too inflexible to allow things like, hiring servants, and makes it a bit difficult to tell the value gained for time is correct or not. In general, it's treating time as a bit too special for the system's own good.

# Solution, the first (Buying Time)

Allow time to be sold like other goods... and that's it. This is a very naive and simple solution, but is obviously flawed. First, while it allows for us to get around the problem of odd jobs and pops with multiple jobs, and so on, it immediately creates a problem in that we MUST, sell skills in a similar way. Skills sold this way would be slightly special in that the buyer wouldn't keep the levels they bought, any levels they gained would need to be returned to the original pop(s), and allow for weird stuff like, purchasing the time of a malnourished orphan but the physical strength skill of a world class body builder, then use both for a process. This makes no sense.

Alternatively, the skill would be tied to the firm long term and only sent to the pop when they move out of the pop. IE, a farmer taking his farming skills with him when he becomes a mechanic. This just sounds messy though, and I'm not keen on dealing with it like that. Which leads to...

# Better Solution, The Second (Goods)

Instead of allowing the purchasing of time and skills, we make them special by allowing them to only be sold in a new type of purchase item. A Good.

A good is not a singular product, but a bundle of them, plus possibly additional context and information, which can be sought out and purchased more specifically. Some goods can only be purchased in these good bundles, other things that can be desireable are best purchased as goods. Eventually, Goods may be the prefered way to purchase things instead of the norm. So, for our selling time issue. Buying time would be to purchase a labor Good, a bundle of time and adjoining skills available to the pop in question. The products and information in the good would define not only what is there but how it should be treated more broadly.

Goods could also be used as part of a more general contracting system. Such as debt and credit being in the form of paired goods. Debtors having a negative value, creditors having a posivite value, both decay as the debt is met. If one is abandoned, the other is destroyed, possibly with additional consequences.

Current Estimated Structure

``` rust
struct Good {
    /// The unique id of the good (maybe us a random ID or hash instead)
    /// May also be abandoned entirely for practical reasons.
    pub id: usize,
    /// The products and the quantity of them in the good.
    pub products: HashMap<usize, f64>,
    /// The Wants which are included in this good.
    pub wants: HashMap<usize, f64>,
    /// The source(s) of the good and the weights applied to them.
    /// Weights may not be useful in all cases.
    pub source: HashMap<ActorInfo, f64>,
    /// The data tags of the good, defining how it should be treated and operate.
    pub tags: vec![GoodTags]
}
```

Good tags would include stuff like
- Labor, products, if not used today will disappear at the end of the day. If used in part of a process, the skills gained are sent to this pop. Special outputs (like worker risk) are transfered to this pop.
- Transferrable, good may be resold by buyer.
- Consumable, good can be combined with other goods, generally to be resold.
- Deadline, the good has a deadline attached to it, such as a deadline for a debt. May be a specific date, and may include some inequality (before/by/after date.)
- Interest, the good is a form of debt which grows over time at the interest rate specified here.
- Promissory note, the good is a promise for the products/wants in the good at the date given, to either be sent to or recieved from the source(s).