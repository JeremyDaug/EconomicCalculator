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
        Disorganized Firm Work
            Get Everything from Pop {cm}
            Active Wait until RequestSent Recieved. {cm}
            Do work and recieve results of our plan {cm}
            Send All wants and goods back {cm}
            Record plan results {cm}
            Send over production needs to pop for them to purchase {cm}
        Organized Firm Work
            If payday, pay worker wages
            Always request workers time and skills
            Do our plan as best we can
            Return skills and any training recieved
            Update Plans
    Organized Firm Work Followups
    Buy and Sell Processing
    Decay Goods {cm}
    Plan for Tomorrow
        Disorganized Firm Planning {cm}
        Organized Firm Planning {cm}
    Send Finished Message and end {cm}
    Enter Holding Pattern {cm}
Institution Internal Market Day
State Internal Market Day
Testing
    Firm Tests
        get_production_requirements {cm:2025-01-31}
        get_optional_production_goods {cm:2025-01-31}
        get_production_goods {cm:2025-01-31}
        get_full_name {cm:2025-01-31}
        push_message (may not bother) {cm}
        quick_msg_catchup (may not bother) {cm:2025-01-31}
        get_next_message (may not bother) {cm:2025-01-31}
        active_wait (may not bother) {cm:2025-01-31}
        exclusive_wait (may not bother) {cm:2025-01-31}
        process_common_msg {cm:2025-01-31}
        work_time_processing {cm:2025-01-31}
        do_plan {cm:2025-01-31}
        buy_and_sell_processing
        update_plans {cm:2025-01-31}
        run_market_day [Didn't Bother] {cm:2025-01-31}
In Consideration
    Allow a pop to have multiple jobs Issue[#70](https://github.com/JeremyDaug/EconomicCalculator/issues/70)
    Add Load Check on Processes for part tags Issue[#67](https://github.com/JeremyDaug/EconomicCalculator/issues/67)
    Decouple Message using functions in Pop Issue[#66](https://github.com/JeremyDaug/EconomicCalculator/issues/66)
    Improve Sifting Intelligence Issue[#64](https://github.com/JeremyDaug/EconomicCalculator/issues/64)