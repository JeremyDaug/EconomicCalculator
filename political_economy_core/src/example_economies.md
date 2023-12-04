# Example Economies

This is a summary document for possible economies, partially for testing, but also
for general brainstorming.

# Absolute Simplest Economy

The idea with this absolute simplest economy is to create the basics of land,
labor, and capital required to produce final consumables. This should result 
in a  very simple, but ultimately growing amonut of capital which produces an 
increasing amount of stock and eventually consumables/wealth. All resources 
are seen as the same, all capital is equivalent, and all desires can be
satisfied with the same product(s).

The result which should be expected is a growth up to an eventual maximum. 
This maximum would be limited up by Time, Land, and Decay Rates of all goods.

I would imagine the exact limit in this case would be calculable from 
Population, Land, process rates, and the Decay rates of the various goods. 
Each population group should end up with a similar amount of 
Wealth/Consumables at the end. 

The simplest would be that all resources (except land) decay at the end of the 
day

Product:
- Time
- Land
- Stock
- Capital
- Consumables

Processes (Simplified):
- Time + Land => Consumables (1TL)
- Time + Land => Stock (1TL)
- Time + Land + Stock => Capital (2TL)
- Time + Land + Stock => (>2) Stock  
- Time + Land + Stock => Consumables (Fixed Capital)
- Time + Land + Capital => Consumables (Entertainment)
- Time + Land + Capital => Stock (Improved Extraction)
- Time + Land + Capital + Stock => Stock
- Time + Land + Capital + Stock => Consumables

[Values Not final]

# Simple++

This is exactly the same as above, but we add in degrees of capital and stock. Higher Ranking Captial can be combined with raw resources and/or higher ranking stock to produce lower ranking capital or stock. The lowest level of stock and capital can only produce consumables.

This could be a good test for dynamic item creation. Each level up produces the needs for the level below and allows for silghly higher efficiency.

Products: 
- Time
- Land
- Raw
- Stock 0-N
- Capital 0-N
- Consumables

Processes:
- Time + Land => Raw
- Time + Land => Consumables
- Time + Land + Raw => Capital N
- Time + Land + Raw => 2 Consumables
- Time + Land + Raw + Capital N => Capital N-1
- Time + Land + Raw + Capital N => Stock N-1