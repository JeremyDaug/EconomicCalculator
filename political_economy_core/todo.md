Internal Market Day Work {c}
    Spin up Actors {cm}
    Send ActorMessage::StartDay {cm}
    Wait for all actors to end their market day and deal with various messages {cm}
    Send Close market message to confirm all actors can wrap up their internal market day work {cm}
        Recond Finished Actors {cm}
        Add Sell Orders recieved {cm}
        Add Buy Orders recieved {cm}
        Deal with dumped product {cm}
        Deal with Finds
            Find Product {cm}
            Find Want {cm}
            Find Class
        Deal Message Handling
            Found Product: Open Deal and record who's making the deal {cm}
            In Stock: Record in stock message {cm}
            Not In Stock: Record the failure and close the deal {cm}
            Buy Offer and Followup: Record Buy offer {cm}
            Seller Responses {cm}
                Accept as is: record {cm}
                Accept with change and followup: Record {cm}
                Reject Offer: Record and Close Deal {cm}
            Finish Deal: Close out deal regardless of state {cm}
    Send Close Market Message for actors to begin/finish wrapping up {cm}
    Wait for all actor threads to end
    Wrap up Market Day
        Clear Sell Orders and weights
Pop internal Market day
    Add Time for this new day {cm}
    Sift all property for future use {cm}
    Wait for day start message {cm}
    Check if the pop is selling
        Pops in disorganized firms always offer for sale {cm}
        Check if organized pop is selling
            Panic Mode Selling State (Starving but not broke)
            Lopsided Desires Selling State
    Do Work Day {cm}
    Do free time {cm}
    Deal with Taxes
    Consume Goods for satisfaction {cm}
    Decay Goods {cm}
    Update plans based on our successes and failure today
Firm Internal Market Day
    Firm Prep
    Do Work
        Disorganized Firm Work {cm}
        Organized Firm Work
    Organized Firm Work Followups
    Buy and Sell Processing
    Decay Goods
    Plan for Tomorrow
    Send Finished Message and end {cm}
    Enter Holding Pattern
Institution Internal Market Day
State Internal Market Day